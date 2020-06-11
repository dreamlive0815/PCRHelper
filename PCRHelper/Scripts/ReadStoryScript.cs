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
            if (viewportCapture.Width < 10 || viewportCapture.Height < 10)
            {
                logTools.Info("Capture Size Sucks, Special Process, Click Skip");
                MumuState.ClickSkipConfirmButton(viewportRect);
                return;
            }

            var viewportMat = viewportCapture.ToOpenCvMat();

            if (IsStoryMainScene(viewportMat, viewportRect))
            {
                DoMainSceneThings(viewportMat, viewportRect);
            }
            else if (IsStoryListScene(viewportMat, viewportRect))
            {
                DoListSceneThings(viewportMat, viewportRect, 1);
            }
            else if (IsDataDownloadWin(viewportMat, viewportRect))
            {
                MumuState.ClickDataDownloadButton(viewportRect, false);
                ClickMenuButtonTimes = 0;
            }
            else if (HasSkipConfirmButton(viewportMat, viewportRect))
            {
                MumuState.ClickSkipConfirmButton(viewportRect);
            }
            else if (HasSkipButton(viewportMat, viewportRect))
            {
                MumuState.ClickSkipButton(viewportRect);
            }
            else if (HasMenuButton(viewportMat, viewportRect))
            {
                MumuState.ClickMenuButton(viewportRect);
                ClickMenuButtonTimes += 1;
                if (ClickMenuButtonTimes >= 5)
                {
                    MumuState.ClickBack(viewportRect);
                }
            }
            else
            {
                logTools.Info("Found Nothing, Click Back");
                MumuState.ClickBack(viewportRect);
            }
        }

        public int ClickMenuButtonTimes { get; set; }

        Dictionary<PCRStory, Vec4f> mainsceneStoryTypeExMap = new Dictionary<PCRStory, Vec4f>()
        {
            { PCRStory.Mainline, new Vec4f(0.5240f, 0.1064f, 0.7562f, 0.4111f) },
            { PCRStory.Character, new Vec4f(0.5298f, 0.4198f, 0.6885f, 0.6370f) },
            { PCRStory.Guild, new Vec4f(0.6849f, 0.4198f, 0.8377f, 0.6297f) },
            { PCRStory.Extra, new Vec4f(0.8355f, 0.4213f, 0.9869f, 0.6239f) },
        };

        public PCRStory CurStory { get; set; }

        private int Depth { get; set; }

        public bool DoMainSceneThings(Mat viewportMat, RECT viewportRect, PCRStory story)
        {
            var rectRate = mainsceneStoryTypeExMap[story];
            var matchRes = MatchImage(viewportMat, viewportRect, rectRate, "story_new_tag.png");
            if (!matchRes.Success) return false;
            logTools.Info($"Found Story New Tag: {story}");
            CurStory = story;
            MumuState.ClickStoryEntrance(viewportRect, story);
            return true;
        }


        public void DoMainSceneThings(Mat viewportMat, RECT viewportRect)
        {
            logTools.Info("DoMainSceneThings");
            foreach (var pair in mainsceneStoryTypeExMap)
            {
                //if (pair.Key == PCRStory.Mainline) continue;
                if (DoMainSceneThings(viewportMat, viewportRect, pair.Key))
                {
                    return;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewportMat"></param>
        /// <param name="viewportRect"></param>
        /// <param name="depth">主线剧情界面这种算第一层</param>
        public void DoListSceneThings(Mat viewportMat, RECT viewportRect, int depth)
        {
            if (depth >= 3) return;

            logTools.Info($"DoListSceneThings; CurStory: {CurStory}; Depth: {depth}");

            var listRectRate = new Vec4f(0.5342f, 0.1210f, 0.9789f, 0.8790f);
            var matchRes = MatchImage(viewportMat, viewportRect, listRectRate, "story_new_tag_inner.png", 0.5);
            if (matchRes.Success)
            {
                ClickListItem(viewportRect, listRectRate, matchRes.MatchedRect);
                Thread.Sleep(2000);
                var newViewportRect = MumuState.ViewportRect;
                var newViewportCapture = MumuState.DoCapture(newViewportRect);
                DoListSceneThings(newViewportCapture.ToOpenCvMat(), viewportRect, depth + 1);
            }
            else
            {
                MumuState.DoDrag(viewportRect, new Vec2f(0.7700f, 0.7012f), new Vec2f(0.7700f, 0.2332f), 1200);
            }
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

        public bool IsDataDownloadWin(Mat viewportMat, RECT viewportRect)
        {
            var dataDownloadTitleRectRate = new Vec4f(0.4105f, 0.2172f, 0.5983f, 0.3353f);
            var matchRes = MatchImage(viewportMat, viewportRect, dataDownloadTitleRectRate, "data_download_title.png");
            return matchRes.Success;
        }

        public bool HasMenuButton(Mat viewportMat, RECT viewportRect)
        {
            var menuButtonRectRate = new Vec4f(0.8523f, 0.0058f, 0.9636f, 0.1633f);
            var matchRes = MatchImage(viewportMat, viewportRect, menuButtonRectRate, "menu_button.png");
            return matchRes.Success;
        }

        public bool HasSkipButton(Mat viewportMat, RECT viewportRect)
        {
            var skipButtonRectRate = new Vec4f(0.7365f, 0.0087f, 0.8748f, 0.1487f);
            var matchRes = MatchImage(viewportMat, viewportRect, skipButtonRectRate, "skip_button.png");
            return matchRes.Success;
        }

        public bool HasSkipConfirmButton(Mat viewportMat, RECT viewportRect)
        {
            var skipConfirmButtonRectRate = new Vec4f(0.5364f, 0.6254f, 0.6674f, 0.7493f);
            var matchRes = MatchImage(viewportMat, viewportRect, skipConfirmButtonRectRate, "skip_confirm_button.png");
            return matchRes.Success;
        }

        public bool ClickConfrimIfHas(Mat viewportMat, RECT viewportRect)
        {
            var skipConfirmButtonRectRate = new Vec4f(0.5364f, 0.6254f, 0.6674f, 0.7493f);
            var matchRes = MatchImage(viewportMat, viewportRect, skipConfirmButtonRectRate, "skip_confirm_button.png");
            if (!matchRes.Success) return false;
            var confirmAbsoluteRect = GetMatchedAbsoluteRect(viewportRect, skipConfirmButtonRectRate, matchRes.MatchedRect);
            //这个是opencv中的坐标
            var cvX = (confirmAbsoluteRect.x1 + confirmAbsoluteRect.x2) / 2;
            var cvY = (confirmAbsoluteRect.y1 + confirmAbsoluteRect.y2) / 2;
            var clickCvPoint = new RawPoint(cvX, cvY);
            var emulatorPoint = MumuState.GetEmulatorPoint(viewportRect, clickCvPoint);
            MumuState.DoClick(emulatorPoint);
            return true;
        }


    }

}
