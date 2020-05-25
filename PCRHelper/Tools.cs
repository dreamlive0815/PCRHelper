using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace PCRHelper
{
    class Tools
    {
        private static Tools instance;

        public static Tools GetInstance()
        {
            if (instance == null)
            {
                instance = new Tools();
            }
            return instance;
        }

        private Tools()
        {
        }

        /// <summary>
        /// 获取Mumu模拟器进程
        /// </summary>
        /// <returns></returns>
        public Process GetMumuProcess()
        {
            var processes = Process.GetProcesses();
            foreach (var process  in processes)
            {
                var procName = process.ProcessName;
                if (procName.Contains("NemuPlayer"))
                {
                    return process;
                }
            }
            throw new Exception("无法找到Mumu模拟器进程");
        }

        public RECT GetMumuWindowRect()
        {
            var proc = GetMumuProcess();
            var rect = new RECT();
            Win32ApiHelper.GetWindowRect(proc.MainWindowHandle, out rect);
            return rect;
        }

        public Image CaptureMumuWindow()
        {
            var rect = GetMumuWindowRect();
            if (rect.x1 < 0 || rect.y1 < 0) throw new Exception("左上角坐标不合法");
            if (rect.x2 < 0 || rect.y2 < 0) throw new Exception("右下角坐标不合法");
            var width = Math.Abs(rect.x1 - rect.x2);
            var height = Math.Abs(rect.y1 - rect.y2);
            var bitmap = new Bitmap(width, height);
            var g = Graphics.FromImage(bitmap);
            g.CopyFromScreen(rect.x1, rect.y1, 0, 0, new Size(width, height));
            g.Dispose();
            return bitmap;
        }

        public void OpenFileInExplorer(string filePath)
        {
            Process.Start("Explorer.exe", filePath);
        }
    }

    struct RECT
    {
        public int x1;
        public int y1;
        public int x2;
        public int y2;
    }

    class Win32ApiHelper
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetWindowRect(IntPtr hWnd, out RECT rect);

        public static readonly int SW_HIDE = 0;
        public static readonly int SW_SHOWNORMAL = 1;
        public static readonly int SW_SHOWMINIMIZED = 2;
        public static readonly int SW_SHOWMAXIMIZED = 3;
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern int ShowWindow(IntPtr hwnd, int nCmdShow);
    }

}
