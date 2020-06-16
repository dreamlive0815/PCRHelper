using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.IO;
using OpenCvSharp;
using RawPoint = System.Drawing.Point;

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
            throw new BreakException("无法找到Mumu模拟器进程");
        }

        public RECT GetWindowRect(Process proc)
        {
            var rect = new RECT();
            Win32ApiHelper.GetWindowRect(proc.MainWindowHandle, out rect);
            return rect;
        }

        public Bitmap CaptureWindow(RECT rect)
        {
            if (rect.x1 < 0 || rect.y1 < 0) throw new NoTrackTraceException("左上角坐标不合法");
            if (rect.x2 < 0 || rect.y2 < 0) throw new NoTrackTraceException("右下角坐标不合法");
            var width = Math.Abs(rect.x1 - rect.x2);
            var height = Math.Abs(rect.y1 - rect.y2);
            width = Math.Max(width, 10);
            height = Math.Max(height, 10);
            var bitmap = new Bitmap(width, height);
            var g = Graphics.FromImage(bitmap);
            g.CopyFromScreen(rect.x1, rect.y1, 0, 0, new System.Drawing.Size(width, height));
            g.Dispose();
            return bitmap;
        }

        public void OpenFileInExplorer(string filePath)
        {
            var fileInfo = new FileInfo(filePath);
            var fullPath = fileInfo.FullName;
            Process.Start("Explorer.exe", $"/select,{fullPath}");
        }

        public void OpenDirInExplorer(string dirPath)
        {
            var dirInfo = new DirectoryInfo(dirPath);
            var fullPath = dirInfo.FullName;
            Process.Start("Explorer.exe", $"{fullPath}");
        }

        public string JoinPath(params string[] names)
        {
            return string.Join("/", names);
        }
    }

    struct RECT
    {
        public int x1;
        public int y1;
        public int x2;
        public int y2;

        public int Width
        {
            get { return Math.Abs(x1 - x2); }
        }

        public int Height
        {
            get { return Math.Abs(y1 - y2); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rate">两个数字分别是横坐标 纵坐标相对于父矩形的比率</param>
        /// <returns></returns>
        public RawPoint GetChildPointByRate(Vec2f rate)
        {
            var point = new RawPoint();
            var width = Width;
            var height = Height;
            point.X = (int)(x1 + width * rate.Item0);
            point.Y = (int)(y1 + height * rate.Item1);
            return point;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rate">四个数字分别是左上角横坐标 左上角纵坐标 右下角横坐标 右下角纵坐标相对于父矩形的比率</param>
        /// <returns></returns>
        public RECT GetChildRectByRate(Vec4f rate)
        {
            RECT rect = this;
            var width = Width;
            var height = Height;
            rect.x1 = (int)(Width * rate.Item0);
            rect.y1 = (int)(Height * rate.Item1);
            rect.x2 = (int)(Width * rate.Item2);
            rect.y2 = (int)(Height * rate.Item3);
            return rect;
        }

        public RawPoint GetCenterPos()
        {
            var x = (x1 + x2) / 2;
            var y = (y1 + y2) / 2;
            return new RawPoint(x, y);
        }

    }

    class Win32ApiHelper
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetWindowRect(IntPtr hWnd, out RECT rect);

        public static readonly int SW_HIDE = 0;
        public static readonly int SW_SHOWNORMAL = 1;
        public static readonly int SW_SHOWMINIMIZED = 2;
        public static readonly int SW_SHOWMAXIMIZED = 3;
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int ShowWindow(IntPtr hwnd, int nCmdShow);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SetCursorPos(int x, int y);

        [DllImport("user32", CharSet = CharSet.Auto)]
        public static extern int mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        //移动鼠标
        public const int MOUSEEVENTF_MOVE = 0x0001;
        //模拟鼠标左键按下
        public const int MOUSEEVENTF_LEFTDOWN = 0x0002;
        //模拟鼠标左键抬起
        public const int MOUSEEVENTF_LEFTUP = 0x0004;
        //模拟鼠标右键按下
        public const int MOUSEEVENTF_RIGHTDOWN = 0x0008;
        //模拟鼠标右键抬起
        public const int MOUSEEVENTF_RIGHTUP = 0x0010;
        //模拟鼠标中键按下
        public const int MOUSEEVENTF_MIDDLEDOWN = 0x0020;
        //模拟鼠标中键抬起
        public const int MOUSEEVENTF_MIDDLEUP = 0x0040;
        //标示是否采用绝对坐标
        public const int MOUSEEVENTF_ABSOLUTE = 0x8000;

        public static void MoveToAndClick(int x, int y)
        {
            SetCursorPos(x, y);
            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
        }
    }

}
