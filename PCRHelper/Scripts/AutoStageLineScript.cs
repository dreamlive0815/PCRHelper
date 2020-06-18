using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace PCRHelper.Scripts
{
    class AutoStageLineScript : ScriptBase
    {

        private GraphicsTools graphicsTools = GraphicsTools.GetInstance();
        private LogTools logTools = LogTools.GetInstance();

        public override string Name
        {
            get { return "AutoStageLineScript"; }
        }

        public override bool CanKeepOnWhenException
        {
            get { return true; }
        }

        public void F()
        {
            //var viewportMat = viewportCapture.ToOpenCvMat();
            //var viewportMat = new Mat(configMgr.GetCacheFileFullPath("battle.png"), ImreadModes.Unchanged);
            //var mat = FilterMat(viewportMat);
            //Cv2.ImShow("viewportMat", mat);
            //var nextMat = configMgr.GetPCRExImg("battle_next_tag.png", mat, viewportRect);
            //nextMat = FilterMat(nextMat);
            //Cv2.ImShow("nextMat", nextMat);
            //var matchRes = graphicsTools.MatchImage(mat, nextMat, 0.4);
        }

        public override void OnStart(Bitmap viewportCapture, RECT viewportRect)
        {
            
        }

        public override void Tick(Bitmap viewportCapture, RECT viewportRect)
        {
            var viewportMat = viewportCapture.ToOpenCvMat();

            if (TryClickNextTag(viewportMat, viewportRect))
            {
                logTools.Info("TryClickNextTag");
            }
            else if (TryClickChallengeButton(viewportMat, viewportRect))
            {
                logTools.Info("TryClickChallengeButton");
            }
            else if (TryClickStartFightButton(viewportMat, viewportRect))
            {
                logTools.Info("TryClickStartFightButton");
            }
            else if (TryClickAutoOffButton(viewportMat, viewportRect))
            {
                logTools.Info("TryClickAutoOffButton");
            }
            else if (TryClickNextStepButton(viewportMat, viewportRect))
            {
                logTools.Info("TryClickNextStepButton");
            }
            else
            {
                logTools.Info("ClickBack");
                MumuState.DoClick(viewportRect, new Vec2f(0.1f, 0.7f));
                //MumuState.ClickBack(viewportRect);
            }

        }

        public bool TryClickNextTag(Mat viewportMat, RECT viewportRect)
        {
            var threshold = 0.6;
            var rectRate = new Vec4f(0.0044f, 0.1122f, 0.9833f, 0.8950f);
            var matchRes = MatchImage(viewportMat, viewportRect, rectRate, "battle_next_tag.png", threshold);
            if (!matchRes.Success) return false;
            var absoluteRect = GetMatchedAbsoluteRect(viewportRect, rectRate, matchRes.MatchedRect);
            var pos = absoluteRect.GetCenterPos();
            pos.Y = pos.Y + (int)(viewportRect.Height * 0.1500f);
            var emulatorPoint = MumuState.GetEmulatorPoint(viewportRect, pos);
            MumuState.DoClick(emulatorPoint);
            return true;
        }

        Mat PreProcessMat(Mat source)
        {
            return graphicsTools.ToGrayBinary(source, 150);
        }
    }
}
