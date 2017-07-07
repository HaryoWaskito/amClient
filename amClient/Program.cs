using Gma.System.MouseKeyHook;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Collections.Generic;

namespace amClient
{
    class Program
    {
        private static Dictionary<string, int> appGlobalCounter = new Dictionary<string, int>();

        #region Attribute to Control Console Window

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        #endregion

        #region Attribute To Get Active Application

        [DllImport("user32.dll")]
        static extern int GetForegroundWindow();

        [DllImport("user32")]
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
            //Console.Title = "Testing";
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

            Application.Run();
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
            //Log(string.Format("KeyPress \t\t {0}\n", e.KeyChar));

            Int32 hwnd = 0;
            hwnd = GetForegroundWindow();
            //string appProcessName = Process.GetProcessById(GetWindowProcessID(hwnd)).ProcessName;
            string appExePath = Process.GetProcessById(GetWindowProcessID(hwnd)).MainModule.FileName;
            string appExeName = appExePath.Substring(appExePath.LastIndexOf(@"\") + 1);

            var monitor = new amModel();
            monitor.amModelId = Guid.NewGuid().ToString();
            //monitor.appProcessName = appProcessName;
            monitor.appExePath = appExePath;
            monitor.appExeName = appExeName;
            //monitor.KeyLogCatch = e.KeyChar.ToString()
            monitor.InputClickedCounter = 1;
            monitor.InputType = "Keyboard";
            monitor.TimeStamp = DateTime.Now;
            monitor.userID = "HWK";

            CreateMonitoringAsync(monitor);
        }

        private static void OnMouseClick(object sender, MouseEventArgs e)
        {
            //Log(string.Format("MouseClick \t\t {0}\n", e.Button));

            Int32 hwnd = 0;
            hwnd = GetForegroundWindow();
            //string appProcessName = Process.GetProcessById(GetWindowProcessID(hwnd)).ProcessName;
            string appExePath = Process.GetProcessById(GetWindowProcessID(hwnd)).MainModule.FileName;
            string appExeName = appExePath.Substring(appExePath.LastIndexOf(@"\") + 1);

            var monitor = new amModel();
            monitor.amModelId = Guid.NewGuid().ToString();
            //monitor.appProcessName = appProcessName;
            monitor.appExePath = appExePath;
            monitor.appExeName = appExeName;
            //monitor.KeyLogCatch = e.Button.ToString();
            ///monitor.InputClickedCounter = 1;
            monitor.InputType = "Mouse";
            monitor.TimeStamp = DateTime.Now;
            monitor.userID = "HWK";

            CreateMonitoringAsync(monitor);
        }

        private static void Log(string text)
        {
            //if (IsDisposed) return;
            //textBoxLog.AppendText(text);
            //textBoxLog.ScrollToCaret();
        }

        #region To Send Data Using Rest API

        static HttpClient client = new HttpClient();

        static async Task RunAsync()
        {
            // New code:
            client.BaseAddress = new Uri("http://localhost:5000");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        static async Task<amModel> GetMonitoringModelSampleData(string path)
        {
            amModel monitor = null;
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                monitor = await response.Content.ReadAsAsync<amModel>();
            }
            return monitor;
        }

        static async Task<Uri> CreateMonitoringAsync(amModel monitor)
        {
            HttpResponseMessage response = await client.PostAsJsonAsync("api/amController/Create", monitor);
            response.EnsureSuccessStatusCode();

            // Return the URI of the created resource.
            return response.Headers.Location;
        }

        #endregion
    }
}
