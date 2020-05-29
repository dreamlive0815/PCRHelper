using OpenCvSharp;
using System;
using System.Diagnostics;
using System.Drawing;

namespace PCRHelper
{
    class MumuState
    {

        public static MumuState Create()
        {
            return new MumuState();
        }

        bool checkResolution = false;//分辨率
        bool checkTopBottomBounds = true;

        private Process process;
        private RECT rect;
        private RECT viewportRect;

        private MumuState()
        {

        }

        private bool UseCache
        {
            get
            {
                return true;
            }
        }

        public Process Process
        {
            get
            {
                if (!UseCache || process == null)
                {
                    process = Tools.GetInstance().GetMumuProcess();
                }
                return process;
            }
        }

        public RECT Rect
        {
            get
            {
                if (!UseCache || rect.Width == 0)
                {
                    rect = Tools.GetInstance().GetWindowRect(Process);
                }
                return rect;
            }
        }


        /// <summary>
        /// getCapture -> Analyse -> return
        /// </summary>
        public RECT ViewportRect
        {
            get
            {
                //if (!UseCache || TopY == 0 || BottomY == 0)
                if (true)
                {
                    var capture = DoRealTimeCaptureAndAnalyze();
                    if (checkResolution)
                    {
                        CheckResolution(capture);
                    }
                    var crect = Rect;
                    viewportRect = new RECT() {
                        x1 = rect.x1,
                        y1 = rect.y1 + (int)TopY,
                        x2 = rect.x2,
                        y2 = rect.y1 + (int)BottomY,
                    };
                }
                return viewportRect;
            }
        }

        public void CheckResolution(Image capture)
        {
            var wdivhCap = 1.0 * capture.Height / capture.Width;
            var wdivh = 0.5625;
            var validWdivhOff = 0.01;
            if (Math.Abs(wdivhCap - wdivh) > validWdivhOff)
            {
                throw new Exception("请使用1920*1080的分辨率");
            }
        }

        public Image DoCapture(RECT rect)
        {
            var capture = Tools.GetInstance().CaptureWindow(rect);
            GraphicsTools.GetInstance().ShowImage("DoCapture", capture);
            return capture;
        }

        public Image DoRealTimeCaptureAndAnalyze()
        {
            var capture = DoRealTimeCapture();
            GetViewportTopBottomBounds(capture);
            return capture;
        }

        public Image DoRealTimeCapture()
        {
            var rect = Rect;
            var capture = DoCapture(rect);
            return capture;
        }

        public Image DoRealTimeViewportCapture()
        {
            var viewportRect = ViewportRect;
            var capture = DoCapture(viewportRect);
            return capture;
        }

        float topY;
        public float TopY
        {
            get { return topY; }
            private set
            {
                topY = value;
            }
        }

        float bottomY;
        public float BottomY
        {
            get { return bottomY; }
            private set
            {
                bottomY = value;
            }
        }

        public void GetViewportTopBottomBounds(Image image)
        {
            var graphicsTools = GraphicsTools.GetInstance();
            var mat = graphicsTools.ToMat(image);
            var bin = graphicsTools.ToGrayBinary(mat, 50);

            var width = image.Width;
            var samplingWidthRate = 0.22;
            var samplingX = (int)Math.Floor(width * samplingWidthRate);

            var threshold = (int)(255 * bin.Rows * 0.5);
            var preSum = 0;
            for (int r = 0; r < bin.Rows; r++)
            {
                var sum = 0;
                for (int c = 0; c < bin.Cols; c++)
                {
                    var clr = bin.Get<Vec3b>(r, c);
                    sum += (int)clr.Item0;
                }

                var diff = Math.Abs(sum - preSum);
                if (diff > threshold)
                {
                    TopY = r;
                    break;
                }
                preSum = sum;
            }

            preSum = 0;
            for (int r = bin.Rows - 1; r >= 0; r--)
            {
                var sum = 0;
                for (int c = 0; c < bin.Cols; c++)
                {
                    var clr = bin.Get<Vec3b>(r, c);
                    sum += (int)clr.Item0;
                }

                var diff = Math.Abs(sum - preSum);
                if (diff > threshold)
                {
                    BottomY = r;
                    break;
                }
                preSum = sum;
            }

            if (checkTopBottomBounds)
            {
                var height = width * 0.5;

                var validTopY = 36;
                var validTopYOff = 10;

                var validBottomY = height + 36;
                var validBottomYOff = 10;

                var topYOff = Math.Abs(TopY - validTopY);
                if (topYOff > validTopYOff)
                {
                    throw new Exception("无法检测到Mumu模拟器上边界");
                }
                var bottomYOff = Math.Abs(BottomY - validBottomY);
                if (bottomYOff > validBottomYOff)
                {
                    throw new Exception("无法检测到Mumu模拟器下边界");
                }
            }
        }

        

        public Image GetCaptureRect(Vec4<float> rectRate)
        {
            var viewportRect = ViewportRect;
            var viewportCapture = DoCapture(viewportRect);
            return GetCaptureRect(viewportCapture, viewportRect, rectRate);
        }

        public Image GetCaptureRect(Image capture, RECT captureRect, Vec4<float> rectRate)
        {
            var mat = GraphicsTools.GetInstance().ToMat(capture);
            return GetCaptureRect(mat, captureRect, rectRate);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="capture">完整的截图捕获</param>
        /// <param name="captureRect">完整的截图捕获矩形 相对于屏幕</param>
        /// <param name="rectRate">子矩形的比率 相对于capture</param>
        /// <returns></returns>
        public Image GetCaptureRect(Mat mat, RECT captureRect, Vec4<float> rectRate)
        {
            var relativeRect = captureRect.Mult(rectRate);
            var childMat = GraphicsTools.GetInstance().GetChildMatByRECT(mat, relativeRect);
            return GraphicsTools.GetInstance().ToImage(childMat);
        }

        /// <summary>
        /// 相对坐标
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="pointRate"></param>
        /// <returns></returns>
        public System.Drawing.Point GetRelativePoint(RECT rect, Vec2<float> pointRate)
        {
            var wid = rect.Width;
            var hei = rect.Height;
            wid = 1920;
            hei = 1080;
            return new System.Drawing.Point((int)(wid * pointRate.item1), (int)(hei * pointRate.item2));
        }


        public void DoClick(System.Drawing.Point point)
        {
            //Win32ApiHelper.MoveToAndClick(point.X, point.Y);
            var startInfo = new ProcessStartInfo()
            {
                FileName = "adb_server.exe",
                Arguments = $"shell input tap {point.X} {point.Y}",
                WindowStyle = ProcessWindowStyle.Hidden,
            };
            Process.Start(startInfo);
        }

        Vec2<float>[] jjcRectPointRateArr = new Vec2<float>[]
        {
            new Vec2<float>(0.7500f, 0.3100f),
            new Vec2<float>(0.7500f, 0.5300f),
            new Vec2<float>(0.7500f, 0.7300f),
        };


        public void ClickJJCRect(RECT viewportRect, int index)
        {
            var point = GetRelativePoint(viewportRect, jjcRectPointRateArr[index]);
            DoClick(point);
        }


        Vec2<float> jjcRefreshButtonPointRate = new Vec2<float>(0.97f, 0.1625f);

        public void ClickJJCRefreshButton(RECT viewportRect)
        {
            var point = GetRelativePoint(viewportRect, jjcRefreshButtonPointRate);
            DoClick(point);
        }


        Vec4<float>[] jjcNameRectRateArr = new Vec4<float>[]
        {
            new Vec4<float>(0.5135f, 0.2511f, 0.6608f, 0.2823f),
            new Vec4<float>(0.5135f, 0.4638f, 0.6608f, 0.4980f),
            new Vec4<float>(0.5135f, 0.6766f, 0.6608f, 0.7116f),
        };

        public Vec4<float> GetJJCNameRectRate(int index)
        {
            return jjcNameRectRateArr[index];
        }

        /// <summary>
        /// 相对坐标
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public RECT GetJJCNameRect(int index)
        {
            var viewportRect = ViewportRect;
            var rectRate = GetJJCNameRectRate(index);
            var jjcNameRect = viewportRect.Mult(rectRate);
            return jjcNameRect;
        }

        public Image GetJJCNameCaptureRect(int index)
        {
            var viewportRect = ViewportRect;
            var viewportCapture = DoCapture(viewportRect);
            return GetJJCNameCaptureRect(viewportCapture, viewportRect, index);
        }

        public Image GetJJCNameCaptureRect(Image viewportCapture, RECT viewportRect, int index)
        {
            var viewportMat = GraphicsTools.GetInstance().ToMat(viewportCapture);
            return GetJJCNameCaptureRect(viewportMat, viewportRect, index);
        }

        public Image GetJJCNameCaptureRect(Mat viewportMat, RECT viewportRect, int index)
        {
            var jjcNameRectRate = GetJJCNameRectRate(index);
            return GetCaptureRect(viewportMat, viewportRect, jjcNameRectRate);
        }


        Vec4<float>[] jjcRankRectRateArr = new Vec4<float>[]
        {
            new Vec4<float>(0.7450f, 0.2227f, 0.8258f, 0.2582f),
            new Vec4<float>(0.7450f, 0.4369f, 0.8258f, 0.4723f),
	        new Vec4<float>(0.7450f, 0.6496f, 0.8258f, 0.6879f),
        };

        public Vec4<float> GetJJCRankRectRate(int index)
        {
            return jjcRankRectRateArr[index];
        }

        /// <summary>
        /// 相对坐标
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public RECT GetJJCRankRect(int index)
        {
            var viewportRect = ViewportRect;
            var rectRate = GetJJCRankRectRate(index);
            var jjcRankRect = viewportRect.Mult(rectRate);
            return jjcRankRect;
        }

        public Image GetJJCRankCaptureRect(int index)
        {
            var viewportRect = ViewportRect;
            var viewportCapture = DoCapture(viewportRect);
            return GetJJCRankCaptureRect(viewportCapture, viewportRect, index);
        }

        public Image GetJJCRankCaptureRect(Image viewportCapture, RECT viewportRect, int index)
        {
            var viewportMat = GraphicsTools.GetInstance().ToMat(viewportCapture);
            return GetJJCRankCaptureRect(viewportMat, viewportRect, index);
        }

        public Image GetJJCRankCaptureRect(Mat viewportMat, RECT viewportRect, int index)
        {
            var jjcRankRectRate = GetJJCRankRectRate(index);
            return GetCaptureRect(viewportMat, viewportRect, jjcRankRectRate);
        }
        
    }
}
