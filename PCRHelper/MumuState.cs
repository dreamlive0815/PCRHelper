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
                if (!UseCache || TopY == 0 || BottomY == 0)
                {
                    GetRealTimeCaptureAndAnalyze();
                    var rect = Rect;
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

        public Image GetRealTimeCaptureAndAnalyze()
        {
            var capture = GetRealTimeCapture();
            GetViewportTopBottomBounds(capture);
            return capture;
        }

        public Image GetRealTimeCapture()
        {
            var rect = Rect;
            var capture = Tools.GetInstance().CaptureWindow(rect);
            return capture;
        }

        public Image GetRealTimeViewportCapture()
        {
            var viewportRect = ViewportRect;
            var capture = Tools.GetInstance().CaptureWindow(viewportRect);
            GraphicsTools.GetInstance().ShowImage("GetRealTimeViewportCapture", capture);
            return capture;
        }

        public float TopY
        {
            get; private set;
        }

        public float BottomY
        {
            get; private set;
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
        }

        Vec4<float>[] jjcNameRectRateArr = new Vec4<float>[] {
            new Vec4<float>(0.5000f, 0.2396f, 0.6567f, 0.2912f),
            new Vec4<float>(0.5000f, 0.4559f, 0.6567f, 0.5058f),
            new Vec4<float>(0.5000f, 0.6689f, 0.6567f, 0.7205f),
        };

        /// <summary>
        /// 相对坐标
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public RECT GetJJCNameRect(int index)
        {
            var viewportRect = ViewportRect;
            var rectRate = jjcNameRectRateArr[index];
            var jjcNameRect = viewportRect.Mult(rectRate);
            return jjcNameRect;
        }

        public Image CaptureJJCNameRect(int index)
        {
            var capture = GetRealTimeViewportCapture();
            return CaptureJJCNameRect(capture, index);
        }


        public Image CaptureJJCNameRect(Image capture, int index)
        {
            var mat = GraphicsTools.GetInstance().ToMat(capture);
            return CaptureJJCNameRect(mat, index);
        }

        public Image CaptureJJCNameRect(Mat mat, int index)
        {
            var jjcNameRect = GetJJCNameRect(index);
            var nameMat = GraphicsTools.GetInstance().GetChildMatByRECT(mat, jjcNameRect);
            return GraphicsTools.GetInstance().ToImage(nameMat);
        }


    }
}
