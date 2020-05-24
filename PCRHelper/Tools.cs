using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace PCRHelper
{
    class Tools
    {
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
            return null;
        }

        public Image CaptureMumuWindow()
        {
            var proc = GetMumuProcess();
            if (proc == null)
            {
                return null;
            }
            var rect = new RECT();
            Win32ApiHelper.GetWindowRect(proc.MainWindowHandle, out rect);
            var width = Math.Abs(rect.x1 - rect.x2);
            var height = Math.Abs(rect.y1 - rect.y2);
            var bitmap = new Bitmap(width, height);
            var g = Graphics.FromImage(bitmap);
            g.CopyFromScreen(rect.x1, rect.y1, 0, 0, new Size(width, height));
            g.Dispose();
            return bitmap;
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
