using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PCRHelper.Scripts
{
    class ReceivePresentScript : ScriptBase
    {

        private LogTools logTools = LogTools.GetInstance();

        public override string Name
        {
            get { return "ReceivePresentScript"; }
        }

        public override void OnStart(Bitmap viewportCapture, RECT viewportRect)
        {

            MumuState.ClickTab(viewportRect, PCRTab.Mainpage);
            Thread.Sleep(3000);
            MumuState.ClickMainpageButton(viewportRect, PCRMainpageButton.Present);
        }

        public override void Tick(Bitmap viewportCapture, RECT viewportRect)
        {
            var viewportMat = viewportCapture.ToOpenCvMat();


            if (TryClickConfirmReceiveButton(viewportMat, viewportRect))
            {
                logTools.Info("TryClickConfirmReceiveButton");
            }
            else if (TryClickReceiveAllButton(viewportMat, viewportRect))
            {
                logTools.Info("TryClickReceiveAllButton");
            }
            
        }


        public bool TryClickReceiveAllButton(Mat viewportMat, RECT viewportRect)
        {
            var threshold = 0.8;
            var rectRate = new Vec4f(0.7094f, 0.8447f, 0.9007f, 0.9303f);
            return TryClickButton(viewportMat, viewportRect, "receive_all_present.png", rectRate, threshold);
        }

        public bool TryClickConfirmReceiveButton(Mat viewportMat, RECT viewportRect)
        {
            var threshold = 0.8;
            var rectRate = new Vec4f(0.5080f, 0.8453f, 0.6978f, 0.9311f);
            return TryClickButton(viewportMat, viewportRect, "confirm_receive_present.png", rectRate, threshold);
        }
    }
}
