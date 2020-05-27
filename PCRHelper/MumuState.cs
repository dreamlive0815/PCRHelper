using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            var samplingX = (int) Math.Floor(width * samplingWidthRate);


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
                    TopY = r - 1;
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
                    BottomY = r + 1;
                    break;
                }
                preSum = sum;
            }


        }
    }
}
