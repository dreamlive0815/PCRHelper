using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using OpenCvSharp;
using RawPoint = System.Drawing.Point;

namespace PCRHelper.Scripts
{
    class ReadStoryScript : ScriptBase
    {
        private ConfigMgr configMgr = ConfigMgr.GetInstance();
        private GraphicsTools graphicsTools = GraphicsTools.GetInstance();
        private LogTools logTools = LogTools.GetInstance();

        public override string Name
        {
            get { return "ReadStoryScript"; }
        }

        public override void OnStart(Bitmap viewportCapture, RECT viewportRect)
        {
            MumuState.ClickTab(viewportRect, PCRTab.Story);
            Thread.Sleep(3000);
        }

        public override void Tick(Bitmap viewportCapture, RECT viewportRect)
        {
            var viewportMat = viewportCapture.ToOpenCvMat();

            if (IsStoryMainScene(viewportMat, viewportRect))
            {
                DoMainSceneThings(viewportMat, viewportRect);
            }
            else if (IsStoryListScene(viewportMat, viewportRect))
            {
                DoListSceneThings(viewportMat, viewportRect);
            }
            else
            {
                logTools.Info("Do Nothing");
            }
        }

        Dictionary<PCRStory, Vec4f> mainsceneStoryTypeExMap = new Dictionary<PCRStory, Vec4f>()
        {
            { PCRStory.Mainline, new Vec4f(0.5240f, 0.1064f, 0.7562f, 0.4111f) },
            { PCRStory.Character, new Vec4f(0.5298f, 0.4198f, 0.6885f, 0.6370f) },
            { PCRStory.Guild, new Vec4f(0.6849f, 0.4198f, 0.8377f, 0.6297f) },
            { PCRStory.Extra, new Vec4f(0.8355f, 0.4213f, 0.9869f, 0.6239f) },
        };

        public PCRStory CurStory { get; set; }

        private int Depth { get; set; }
        
        public void DoMainSceneThings(Mat viewportMat, RECT viewportRect)
        {
            logTools.Info("DoMainSceneThings");
            foreach (var pair in mainsceneStoryTypeExMap)
            {
                var matchRes = MatchImage(viewportMat, viewportRect, pair.Value, "story_new_tag.png");
                if (matchRes.Success)
                {
                    logTools.Info($"Found Story New Tag: {pair.Key}");
                    CurStory = pair.Key;
                    MumuState.ClickStoryEntrance(viewportRect, pair.Key);
                    return;
                }
            }
        }

        public void DoListSceneThings(Mat viewportMat, RECT viewportRect)
        {
            logTools.Info($"DoListSceneThings; CurStory: {CurStory}");

            var listRectRate = new Vec4f(0.5342f, 0.1210f, 0.9789f, 0.8790f);
            var matchRes = MatchImage(viewportMat, viewportRect, listRectRate, "story_new_tag_inner.png");
            if (matchRes.Success)
            {
                ClickListItem(viewportRect, listRectRate, matchRes.MatchedRect);
            }
            //new Vec4f(0.5531f, 0.1458f, 0.9483f, 0.3353f)
        }
 
        public void ClickListItem(RECT viewportRect, Vec4f listRectRate, RECT listItemNewTagRect)
        {
            var newTagAbsoluteRect = GetMatchedAbsoluteRect(viewportRect, listRectRate, listItemNewTagRect);
            //这个是opencv中的坐标
            var clickCvPoint = new RawPoint((newTagAbsoluteRect.x1 + newTagAbsoluteRect.x2) / 2, newTagAbsoluteRect.y2);
            var emulatorPoint = MumuState.GetEmulatorPoint(viewportRect, clickCvPoint);
            MumuState.DoClick(emulatorPoint);
        }

        public bool IsStoryTabSelected(Mat viewportMat, RECT viewportRect)
        {
            var tabRectRate = new Vec4f(0.3108f, 0.9417f, 0.4243f, 0.9985f);
            var matchRes = MatchImage(viewportMat, viewportRect, tabRectRate, "story_tab_selected.png");
            return matchRes.Success;
        }

        public bool IsStoryMainScene(Mat viewportMat, RECT viewportRect)
        {
            var mainsceneTagRectRate = new Vec4f(0.5400f, 0.1210f, 0.8057f, 0.3965f);
            var matchRes = MatchImage(viewportMat, viewportRect, mainsceneTagRectRate, "story_main_scene_tag.png");
            return matchRes.Success;
        }

        public bool IsStoryListScene(Mat viewportMat, RECT viewportRect)
        {
            var listsceneTagRectRate = new Vec4f(0.9207f, 0.0190f, 0.9840f, 0.1108f);
            var matchRes = MatchImage(viewportMat, viewportRect, listsceneTagRectRate, "story_list_scene_tag.png");
            return matchRes.Success;
        }


    }

}
