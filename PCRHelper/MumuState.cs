using OpenCvSharp;
using System;
using System.Diagnostics;
using System.Drawing;
using RawPoint = System.Drawing.Point;

namespace PCRHelper
{
    class MumuState
    {

        public static MumuState Create()
        {
            return new MumuState();
        }

        bool checkResolution = true;//分辨率
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

        public void CheckResolution(Bitmap capture)
        {
            var wdivhCap = 1.0 * capture.Height / capture.Width;
            var wdivh = 0.5625;
            var validWdivhOff = 0.05;
            if (Math.Abs(wdivhCap - wdivh) > validWdivhOff)
            {
                throw new Exception("请使用1920*1080的分辨率");
            }
        }

        public Bitmap DoCapture(RECT rect)
        {
            var capture = Tools.GetInstance().CaptureWindow(rect);
            GraphicsTools.GetInstance().DisplayImage("DoCapture", capture);
            return capture;
        }


        /// <summary>
        /// 包括标题栏
        /// </summary>
        /// <returns></returns>
        public Bitmap DoRealTimeCapture()
        {
            var rect = Rect;
            var capture = DoCapture(rect);
            return capture;
        }

        public Bitmap DoRealTimeCaptureAndAnalyze()
        {
            var capture = DoRealTimeCapture();
            GetViewportTopBottomBounds(capture);
            return capture;
        }

        public Bitmap DoRealTimeViewportCapture()
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

        public void GetViewportTopBottomBounds(Bitmap bitmap)
        {
            var graphicsTools = GraphicsTools.GetInstance();
            var mat = bitmap.ToOpenCvMat();
            var bin = graphicsTools.ToGrayBinary(mat, 50);

            var width = bitmap.Width;

            var threshold = (int)(255 * bin.Rows * 0.5);
            var preSum = 0;
            for (int r = 0; r < bin.Rows; r++)
            {
                var sum = 0;
                for (int c = 0; c < bin.Cols; c++)
                {
                    var clr = bin.GetPixel(r, c);
                    sum += clr.R;
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
                    var clr = bin.GetPixel(r, c);
                    sum += clr.R;
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

        public Bitmap GetCaptureRect(Vec4f rectRate)
        {
            var viewportRect = ViewportRect;
            var viewportCapture = DoCapture(viewportRect);
            return GetCaptureRect(viewportCapture, viewportRect, rectRate);
        }

        public Bitmap GetCaptureRect(Bitmap capture, RECT captureRect, Vec4f rectRate)
        {
            var mat = capture.ToOpenCvMat();
            return GetCaptureRect(mat, captureRect, rectRate);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="capture">完整的截图捕获</param>
        /// <param name="captureRect">完整的截图捕获矩形 相对于屏幕</param>
        /// <param name="rectRate">子矩形的比率 相对于capture</param>
        /// <returns></returns>
        public Bitmap GetCaptureRect(Mat mat, RECT captureRect, Vec4f rectRate)
        {
            var relativeRect = captureRect.GetChildRectByRate(rectRate);
            var childMat = GraphicsTools.GetInstance().GetChildMatByRECT(mat, relativeRect);
            return childMat.ToRawBitmap();
        }

        /// <summary>
        /// 相对坐标
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="pointRate"></param>
        /// <returns></returns>
        public RawPoint GetRelativePoint(RECT rect, Vec2f pointRate)
        {
            var wid = rect.Width;
            var hei = rect.Height;
            wid = 2160;
            hei = 1080;
            return new RawPoint((int)(wid * pointRate.Item0), (int)(hei * pointRate.Item1));
        }

        public void DoClick(RawPoint point)
        {
            AdbTools.GetInstance().DoTap(point);
        }

        Vec2f[] mainlandArenaRectPointRateArr = new Vec2f[]
        {
            new Vec2f(0.6669f, 0.3312f),
            new Vec2f(0.6653f, 0.5357f),
            new Vec2f(0.6448f, 0.7433f),
        };
        
        Vec2f[] jjcRectPointRateArr = new Vec2f[]
        {
            new Vec2f(0.6669f, 0.3312f),
            new Vec2f(0.6653f, 0.5357f),
            new Vec2f(0.6448f, 0.7433f),
        };

        public void ClickJJCRect(RECT viewportRect, int index)
        {
            var point = GetRelativePoint(viewportRect, jjcRectPointRateArr[index]);
            DoClick(point);
        }

        Vec2f jjcRefreshButtonPointRate = new Vec2f(0.8663f, 0.1680f);

        public void ClickJJCRefreshButton(RECT viewportRect)
        {
            var point = GetRelativePoint(viewportRect, jjcRefreshButtonPointRate);
            DoClick(point);
        }


        Vec4f[] jjcNameRectRateArr = new Vec4f[]
        {
            new Vec4f(0.5135f, 0.2511f, 0.6608f, 0.2823f),
            new Vec4f(0.5135f, 0.4638f, 0.6608f, 0.4980f),
            new Vec4f(0.5135f, 0.6766f, 0.6608f, 0.7116f),
        };

        public Vec4f GetJJCNameRectRate(int index)
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
            var jjcNameRect = viewportRect.GetChildRectByRate(rectRate);
            return jjcNameRect;
        }

        public Bitmap GetJJCNameCaptureRect(int index)
        {
            var viewportRect = ViewportRect;
            var viewportCapture = DoCapture(viewportRect);
            return GetJJCNameCaptureRect(viewportCapture, viewportRect, index);
        }

        public Bitmap GetJJCNameCaptureRect(Bitmap viewportCapture, RECT viewportRect, int index)
        {
            var viewportMat = viewportCapture.ToOpenCvMat();
            return GetJJCNameCaptureRect(viewportMat, viewportRect, index);
        }

        public Bitmap GetJJCNameCaptureRect(Mat viewportMat, RECT viewportRect, int index)
        {
            var jjcNameRectRate = GetJJCNameRectRate(index);
            return GetCaptureRect(viewportMat, viewportRect, jjcNameRectRate);
        }


        Vec4f[] jjcRankRectRateArr = new Vec4f[]
        {
            new Vec4f(0.7450f, 0.2227f, 0.8258f, 0.2582f),
            new Vec4f(0.7450f, 0.4369f, 0.8258f, 0.4723f),
	        new Vec4f(0.7450f, 0.6496f, 0.8258f, 0.6879f),
        };

        public Vec4f GetJJCRankRectRate(int index)
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
            var jjcRankRect = viewportRect.GetChildRectByRate(rectRate);
            return jjcRankRect;
        }

        public Bitmap GetJJCRankCaptureRect(int index)
        {
            var viewportRect = ViewportRect;
            var viewportCapture = DoCapture(viewportRect);
            return GetJJCRankCaptureRect(viewportCapture, viewportRect, index);
        }

        public Bitmap GetJJCRankCaptureRect(Bitmap viewportCapture, RECT viewportRect, int index)
        {
            var viewportMat = viewportCapture.ToOpenCvMat();
            return GetJJCRankCaptureRect(viewportMat, viewportRect, index);
        }

        public Bitmap GetJJCRankCaptureRect(Mat viewportMat, RECT viewportRect, int index)
        {
            var jjcRankRectRate = GetJJCRankRectRate(index);
            return GetCaptureRect(viewportMat, viewportRect, jjcRankRectRate);
        }
        
    }
}
