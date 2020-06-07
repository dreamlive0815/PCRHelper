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

        bool checkResolution = false;//分辨率
        bool checkTopBottomBounds = false;

        private Process process;
        private RECT rect;
        private RECT viewportRect;
        private ConfigMgr configMgr = ConfigMgr.GetInstance();
        private GraphicsTools graphicsTools = GraphicsTools.GetInstance();
        private OCRTools ocrTools = OCRTools.GetInstance();
        private LogTools logTools = LogTools.GetInstance();

        private MumuState()
        {

        }

        private bool UseCache
        {
            get
            {
                return false;
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
                if (!UseCache || TopY == 0 || BottomY == 0)
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

        public Bitmap GetCaptureRect(Bitmap capture, RECT captureRect, Vec4f rectRate)
        {
            var mat = capture.ToOpenCvMat();
            return GetCaptureRect(mat, captureRect, rectRate).ToRawBitmap();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="capture">完整的截图捕获</param>
        /// <param name="captureRect">完整的截图所对应的矩形 相对于屏幕</param>
        /// <param name="rectRate">子矩形的比率 相对于capture</param>
        /// <returns></returns>
        public Mat GetCaptureRect(Mat mat, RECT captureRect, Vec4f rectRate)
        {
            var relativeRect = captureRect.GetChildRectByRate(rectRate);
            var childMat = GraphicsTools.GetInstance().GetChildMatByRECT(mat, relativeRect);
            return childMat;
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

        Vec2f[] mainlandArenaPlayerPointRateArr = new Vec2f[]
        {
            new Vec2f(0.6669f, 0.3312f),
            new Vec2f(0.6653f, 0.5357f),
            new Vec2f(0.6448f, 0.7433f),
        };

        Vec2f[] taiwanArenaPlayerPointRateArr = new Vec2f[]
        {
            new Vec2f(0.6669f, 0.3312f),
            new Vec2f(0.6653f, 0.5357f),
            new Vec2f(0.6448f, 0.7433f),
        };

        public Vec2f GetArenaPlayerPointRate(int index)
        {
            if (configMgr.PCRRegion == PCRRegion.Mainland)
            {
                return mainlandArenaPlayerPointRateArr[index];
            }
            else if (configMgr.PCRRegion == PCRRegion.Taiwan)
            {
                return taiwanArenaPlayerPointRateArr[index];
            }
            throw new Exception("GetArenaPlayerPointRateArr" + index);
        }

        public void ClickArenaPlayer(RECT viewportRect, int index)
        {
            var arenaPlayerPointRate = GetArenaPlayerPointRate(index);
            var point = GetRelativePoint(viewportRect, arenaPlayerPointRate);
            DoClick(point);
        }

        Vec2f arenaRefreshPointRate = new Vec2f(0.8663f, 0.1680f);

        public Vec2f GetArenaRefreshPointRate()
        {
            return arenaRefreshPointRate;
        }

        public void ClickArenaRefresh(RECT viewportRect)
        {
            var arenaRefreshPointRate = GetArenaRefreshPointRate();
            var point = GetRelativePoint(viewportRect, arenaRefreshPointRate);
            DoClick(point);
        }


        Vec4f[] mainlandArenaPlayerNameRectRateArr = new Vec4f[]
        {
            new Vec4f(0.5135f, 0.2511f, 0.6608f, 0.2823f),
            new Vec4f(0.5135f, 0.4638f, 0.6608f, 0.4980f),
            new Vec4f(0.5135f, 0.6766f, 0.6608f, 0.7116f),
        };

        Vec4f[] taiwanArenaPlayerNameRectRateArr = new Vec4f[]
        {
        };

        public Vec4f GetArenaPlayerNameRectRate(int index)
        {
            if (configMgr.PCRRegion == PCRRegion.Mainland)
            {
                return mainlandArenaPlayerNameRectRateArr[index];
            }
            else if (configMgr.PCRRegion == PCRRegion.Taiwan)
            {
                return taiwanArenaPlayerNameRectRateArr[index];
            }
            throw new Exception("GetArenaPlayerNamePointRateArr" + index);
        }

        public Bitmap GetArenaPlayerNameRectCapture(Bitmap viewportCapture, RECT viewportRect, int index)
        {
            var arenaPlayerNameRectRate = GetArenaPlayerNameRectRate(index);
            return GetCaptureRect(viewportCapture, viewportRect, arenaPlayerNameRectRate);
        }

        public Mat GetArenaPlayerNameRectCapture(Mat viewportMat, RECT viewportRect, int index)
        {
            var arenaPlayerNameRectRate = GetArenaPlayerNameRectRate(index);
            return GetCaptureRect(viewportMat, viewportRect, arenaPlayerNameRectRate);
        }

        public string DoArenaPlayerNameOCR(Bitmap viewportCapture, RECT viewportRect, int index)
        {
            var capture = GetArenaPlayerNameRectCapture(viewportCapture, viewportRect, index);
            graphicsTools.DisplayImage("NameToOCR" + index, capture);
            var name = ocrTools.ToGrayAndOCR(capture);
            return name;
        }

        public string DoArenaPlayerNameOCR(Mat viewportMat, RECT viewportRect, int index)
        {
            var capture = GetArenaPlayerNameRectCapture(viewportMat, viewportRect, index);
            graphicsTools.DisplayImage("NameToOCR" + index, capture);
            var name = ocrTools.ToGrayAndOCR(capture);
            return name;
        }

        Vec4f[] mainlandArenaPlayerRankRectRateArr = new Vec4f[]
        {
            new Vec4f(0.7450f, 0.2227f, 0.8258f, 0.2582f),
            new Vec4f(0.7450f, 0.4369f, 0.8258f, 0.4723f),
            new Vec4f(0.7450f, 0.6496f, 0.8258f, 0.6879f),
        };

        Vec4f[] taiwanArenaPlayerRankRectRateArr = new Vec4f[]
        {
        };

        public Vec4f GetArenaPlayerRankRectRate(int index)
        {
            if (configMgr.PCRRegion == PCRRegion.Mainland)
            {
                return mainlandArenaPlayerRankRectRateArr[index];
            }
            else if (configMgr.PCRRegion == PCRRegion.Taiwan)
            {
                return taiwanArenaPlayerRankRectRateArr[index];
            }
            throw new Exception("GetArenaPlayerRankPointRateArr" + index);
        }

        public Bitmap GetArenaPlayerRankRectCapture(Bitmap viewportCapture, RECT viewportRect, int index)
        {
            var arenaPlayerRankRectRate = GetArenaPlayerRankRectRate(index);
            return GetCaptureRect(viewportCapture, viewportRect, arenaPlayerRankRectRate);
        }

        public Mat GetArenaPlayerRankRectCapture(Mat viewportMat, RECT viewportRect, int index)
        {
            var arenaPlayerRankRectRate = GetArenaPlayerRankRectRate(index);
            return GetCaptureRect(viewportMat, viewportRect, arenaPlayerRankRectRate);
        }

        public string DoArenaPlayerRankOCR(Bitmap viewportCapture, RECT viewportRect, int index)
        {
            var capture = GetArenaPlayerRankRectCapture(viewportCapture, viewportRect, index);
            var gray = graphicsTools.ToGray(capture);
            var reverse = graphicsTools.ToReverse(gray);
            graphicsTools.DisplayImage("RankReverse" + index, reverse);
            var bin = graphicsTools.ToBinaryPlus(reverse, 90);
            bin = graphicsTools.CleanBinCorner(bin);
            graphicsTools.DisplayImage("RankToOCR" + index, bin);
            var rank = ocrTools.OCR(bin);
            return rank;
        }

        public string DoArenaPlayerRankOCR(Mat viewportMat, RECT viewportRect, int index)
        {
            var capture = GetArenaPlayerRankRectCapture(viewportMat, viewportRect, index);
            var gray = graphicsTools.ToGray(capture);
            var reverse = graphicsTools.ToReverse(gray);
            graphicsTools.DisplayImage("RankReverse" + index, reverse);
            var bin = graphicsTools.ToBinaryPlus(reverse, 90);
            bin = graphicsTools.CleanBinCorner(bin);
            graphicsTools.DisplayImage("RankToOCR" + index, bin);
            var rank = ocrTools.OCR(bin);
            return rank;
        }
    }
}
