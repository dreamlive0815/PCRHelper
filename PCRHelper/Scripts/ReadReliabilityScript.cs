using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCRHelper.Scripts
{
    class ReadReliabilityScript : ScriptBase
    {

        private LogTools logTools = LogTools.GetInstance();

        public override string Name
        {
            get { return "ReadReliabilityScript"; }
        }

        public override void OnStart(Bitmap viewportCapture, RECT viewportRect)
        {
            if (ConfigMgr.GetInstance().PCRRegion == PCRRegion.Mainland)
            {
                throw new BreakException("国服暂不支持");
            }
        }

        public override void Tick(Bitmap viewportCapture, RECT viewportRect)
        {
            var viewportMat = viewportCapture.ToOpenCvMat();


            if (IsDataDownloadWin(viewportMat, viewportRect))
            {
                logTools.Info("DataDownloadWin");
                MumuState.ClickDataDownloadButton(viewportRect, false);
            }
            else if (TryClickChoiceOne(viewportMat, viewportRect))
            {
                logTools.Info("TryClickChoiceOne");
            }
            else if (IsReliabilityMainScene(viewportMat, viewportRect))
            {
                DoMainSceneThings(viewportMat, viewportRect);
            }
            else if (IsReliabilityEpisodeScene(viewportMat, viewportRect))
            {
                DoEpisodeSceneThings(viewportMat, viewportRect);
            }
            else
            {
                logTools.Info("ClickBack");
                MumuState.ClickBack(viewportRect);
            }

        }

        public bool IsReliabilityMainScene(Mat viewportMat, RECT viewportRect)
        {
            var threshold = 0.7;
            var rectRate = new Vec4f(0.0138f, 0.0058f, 0.1601f, 0.1079f);
            var matchRes = MatchImage(viewportMat, viewportRect, rectRate, "reliability_main_scene_tag.png", threshold);
            return matchRes.Success;
        }

        public void DoMainSceneThings(Mat viewportMat, RECT viewportRect)
        {
            logTools.Info("DoMainSceneThings");

            var listItemNewTagThreshold = 0.6;
            var listRectRate = new Vec4f(0.5378f, 0.1254f, 0.9760f, 0.8834f);
            var matchRes = MatchImage(viewportMat, viewportRect, listRectRate, "reliability_new_tag.png", listItemNewTagThreshold);
            if (matchRes.Success)
            {
                var absoluteRect = GetMatchedAbsoluteRect(viewportRect, listRectRate, matchRes.MatchedRect);
                var emulatorPoint = MumuState.GetEmulatorPoint(viewportRect, absoluteRect.GetCenterPos());
                MumuState.DoClick(emulatorPoint);
            }
            else
            {
                logTools.Info("DoMainSceneThings Cannot Find New Tag");
                MumuState.DoDrag(viewportRect, new Vec2f(0.7700f, 0.7012f), new Vec2f(0.7700f, 0.2332f), 1200);
            }
        }

        public bool IsReliabilityEpisodeScene(Mat viewportMat, RECT viewportRect)
        {
            var threshold = 0.7;
            var rectRate = new Vec4f(0.0167f, 0.7799f, 0.2322f, 0.8892f);
            var matchRes = MatchImage(viewportMat, viewportRect, rectRate, "reliability_episode_scene_tag.png", threshold);
            return matchRes.Success;
        }

        public void DoEpisodeSceneThings(Mat viewportMat, RECT viewportRect)
        {
            logTools.Info("DoEpisodeSceneThings");

            var threshold = 0.6;
            var rectRate = new Vec4f(0.0408f, 0.0350f, 0.9651f, 0.8499f);
            var matchRes = MatchImage(viewportMat, viewportRect, rectRate, "reliability_episode_new_tag.png", threshold);
            if (matchRes.Success)
            {
                var absoluteRect = GetMatchedAbsoluteRect(viewportRect, rectRate, matchRes.MatchedRect);
                var emulatorPoint = MumuState.GetEmulatorPoint(viewportRect, absoluteRect.GetCenterPos());
                MumuState.DoClick(emulatorPoint);
            }
            else
            {
                logTools.Info("DoEpisodeSceneThings Cannot Find New Tag");
                MumuState.ClickBack(viewportRect);
            }
        }

        public bool TryClickChoiceOne(Mat viewportMat, RECT viewportRect)
        {
            var threshold = 0.7;
            var rectRate = new Vec4f(0.2525f, 0.1531f, 0.4469f, 0.3134f);
            return TryClickButton(viewportMat, viewportRect, "choice_pink_tag.png", rectRate, threshold);
        }
    }
}
