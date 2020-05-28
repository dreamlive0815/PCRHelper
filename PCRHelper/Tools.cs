using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.IO;

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

        public RECT GetWindowRect(Process proc)
        {
            var rect = new RECT();
            Win32ApiHelper.GetWindowRect(proc.MainWindowHandle, out rect);
            return rect;
        }

        public Image CaptureWindow(RECT rect)
        {
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

    struct Vec4<T>
    {
        /// <summary>
        /// x1
        /// </summary>
        public T item1;
        /// <summary>
        /// y1
        /// </summary>
        public T item2;
        /// <summary>
        /// x2
        /// </summary>
        public T item3;
        /// <summary>
        /// y2
        /// </summary>
        public T item4;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item1">x1</param>
        /// <param name="item2">y1</param>
        /// <param name="item3">x2</param>
        /// <param name="item4">y2</param>
        public Vec4(T item1, T item2, T item3, T item4)
        {
            this.item1 = item1;
            this.item2 = item2;
            this.item3 = item3;
            this.item4 = item4;
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
        /// <param name="other">相对this的坐标</param>
        /// <returns></returns>
        public RECT Add(RECT other)
        {
            RECT rect = this;
            rect.x1 += other.x1;
            rect.y1 += other.y1;
            rect.x2 += other.x2;
            rect.y2 += other.y2;
            return rect;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rate">基于width和height的比率</param>
        /// <returns></returns>
        public RECT Add(Vec4<float> rate)
        {
            RECT rect = this;
            var width = Width;
            var height = Height;
            rect.x1 += (int)(Width * rate.item1);
            rect.y1 += (int)(Height * rate.item2);
            rect.x2 += (int)(Width * rate.item3);
            rect.y2 += (int)(Height * rate.item4);
            return rect;
        }

        public RECT Mult(Vec4<float> rate)
        {
            RECT rect = this;
            var width = Width;
            var height = Height;
            rect.x1 = (int)(Width * rate.item1);
            rect.y1 = (int)(Height * rate.item2);
            rect.x2 = (int)(Width * rate.item3);
            rect.y2 = (int)(Height * rate.item4);
            return rect;
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
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern int ShowWindow(IntPtr hwnd, int nCmdShow);
    }

}
