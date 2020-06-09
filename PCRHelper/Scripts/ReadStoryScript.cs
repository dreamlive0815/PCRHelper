using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using OpenCvSharp;

namespace PCRHelper.Scripts
{
    class ReadStoryScript : ScriptBase
    {
        private ConfigMgr configMgr = ConfigMgr.GetInstance();
        private GraphicsTools graphicsTools = GraphicsTools.GetInstance();

        public override string Name
        {
            get { return "ReadStoryScript"; }
        }

        public override void OnStart(Bitmap viewportCapture, RECT viewportRect)
        {
            MumuState.ClickTab(viewportRect, PCRTab.Story);
            Thread.Sleep(4000);
            viewportCapture = MumuState.DoCapture(viewportRect);
            var viewportMat = viewportCapture.ToOpenCvMat();
            var mainlineStoryRectRate = new Vec4f(0.5276f, 0.1054f, 0.6914f, 0.4128f);
            var mainlineStoryMat = viewportMat.GetChildMatByRectRate(mainlineStoryRectRate);
            var newTagEx = new Mat(configMgr.GetPCRExImgFullPath("story_new_tag.png"), ImreadModes.Unchanged);
            var matchRes = graphicsTools.MatchImage(mainlineStoryMat, newTagEx);

            //mainlineStoryMat.SaveImage(ConfigMgr.GetInstance().GetCacheFileFullPath("mainline_story.png"));
        }

        public override void Tick(Bitmap viewportCapture, RECT viewportRect)
        {
            
        }
    }
}
