﻿using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCRHelper.Scripts
{
    abstract class ScriptBase
    {
        //1357
        public static event Action<string, bool> ScriptEnded;

        public static void OnScriptEnded(string scriptName, bool noError)
        {
            ScriptEnded?.Invoke(scriptName, noError);
        }

        public void SetMumuState(MumuState mumuState)
        {
            MumuState = mumuState;
        }

        protected MumuState MumuState { get; set; }

        /// <summary>
        /// 单位：毫秒
        /// </summary>
        public virtual int Interval { get; set; } = 2000;

        public abstract string Name { get; }

        public virtual bool CanKeepOnWhenException { get { return false; } }

        public abstract void OnStart(Bitmap viewportCapture, RECT viewportRect);

        public abstract void Tick(Bitmap viewportCapture, RECT viewportRect);

        public MatchImageResult MatchImage(Mat viewportMat, RECT viewportRect, Vec4f exRectRate, string exName)
        {
            var exRectMat = viewportMat.GetChildMatByRectRate(exRectRate);
            var exImgMat = ConfigMgr.GetInstance().GetPCRExImg(exName, viewportMat, viewportRect);
            var matchRes = GraphicsTools.GetInstance().MatchImage(exRectMat, exImgMat);
            return matchRes;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewportMat"></param>
        /// <param name="viewportRect"></param>
        /// <param name="exRectRate">采样区域</param>
        /// <param name="exName"></param>
        /// <returns></returns>
        public MatchImageResult MatchImage(Mat viewportMat, RECT viewportRect, Vec4f exRectRate, string exName, double threshold)
        {
            var exRectMat = viewportMat.GetChildMatByRectRate(exRectRate);
            var exImgMat = ConfigMgr.GetInstance().GetPCRExImg(exName, viewportMat, viewportRect);
            var matchRes = GraphicsTools.GetInstance().MatchImage(exRectMat, exImgMat, threshold);
            return matchRes;
        }

        public MatchImageResult MatchImageFlipUpDown(Mat viewportMat, RECT viewportRect, Vec4f exRectRate, string exName, double threshold)
        {
            var exRectMat = viewportMat.GetChildMatByRectRate(exRectRate);
            var exImgMat = ConfigMgr.GetInstance().GetPCRExImg(exName, viewportMat, viewportRect);
            var exRectFlipMat = new Mat();
            var exImgFlipMat = new Mat();
            Cv2.Flip(exRectMat, exRectFlipMat, FlipMode.XY);
            Cv2.Flip(exImgMat, exImgFlipMat, FlipMode.XY);
            var flipMatchRes = GraphicsTools.GetInstance().MatchImage(exRectFlipMat, exImgFlipMat, threshold);
            var flipMatchedRect = flipMatchRes.MatchedRect;
            var rectWidth = exRectMat.Width;
            var rectHeight = exRectMat.Height;
            var matchRes = new MatchImageResult()
            {
                Success = flipMatchRes.Success,
                MatchedRect = new RECT()
                {
                    x1 = rectWidth - flipMatchedRect.x2,
                    y1 = rectHeight - flipMatchedRect.y2,
                    x2 = rectWidth - flipMatchedRect.x1,
                    y2 = rectHeight - flipMatchedRect.y1,
                }
            };
            return matchRes;
        }

        /// <summary>
        /// 这里的绝对是指相对于viewport左上角
        /// </summary>
        /// <param name="viewportRect"></param>
        /// <param name="rectRate"></param>
        /// <param name="matchedRelativeRect"></param>
        /// <returns></returns>
        public RECT GetMatchedAbsoluteRect(RECT viewportRect, Vec4f rectRate, RECT matchedRelativeRect)
        {
            var rect = viewportRect.GetChildRectByRate(rectRate);
            return new RECT()
            {
                x1 = rect.x1 + matchedRelativeRect.x1,
                y1 = rect.y1 + matchedRelativeRect.y1,
                x2 = rect.x1 + matchedRelativeRect.x2,
                y2 = rect.y1 + matchedRelativeRect.y2,
            };
        }

        public bool TryClickButton(Mat viewportMat, RECT viewportRect, string exImgName, Vec4f rectRate, double threshold)
        {
            var matchRes = MatchImage(viewportMat, viewportRect, rectRate, exImgName, threshold);
            if (!matchRes.Success) return false;
            var absoluteRect = GetMatchedAbsoluteRect(viewportRect, rectRate, matchRes.MatchedRect);
            var centerPos = absoluteRect.GetCenterPos();
            var emulatorPoint = MumuState.GetEmulatorPoint(viewportRect, centerPos);
            MumuState.DoClick(emulatorPoint);
            return true;
        }

        public bool IsDataDownloadWin(Mat viewportMat, RECT viewportRect)
        {
            var dataDownloadTitleRectRate = new Vec4f(0.4105f, 0.2172f, 0.5983f, 0.3353f);
            var matchRes = MatchImage(viewportMat, viewportRect, dataDownloadTitleRectRate, "data_download_title.png");
            return matchRes.Success;
        }

        public bool TryClickChallengeButton(Mat viewportMat, RECT viewportRect)
        {
            var threshold = 0.6;
            var rectRate = new Vec4f(0.7540f, 0.7536f, 0.9192f, 0.9242f);
            return TryClickButton(viewportMat, viewportRect, "fight.png", rectRate, threshold);
        }

        public bool TryClickStartFightButton(Mat viewportMat, RECT viewportRect)
        {
            var threshold = 0.6;
            var rectRate = new Vec4f(0.7482f, 0.7667f, 0.9221f, 0.9197f);
            return TryClickButton(viewportMat, viewportRect, "start_fight.png", rectRate, threshold);
        }

        public bool TryClickAutoOffButton(Mat viewportMat, RECT viewportRect)
        {
            var threshold = 0.6;
            var rectRate = new Vec4f(0.9163f, 0.7041f, 0.9854f, 0.8542f);
            return TryClickButton(viewportMat, viewportRect, "battle_auto_off.png", rectRate, threshold);
        }

        public bool TryClickNextStepButton(Mat viewportMat, RECT viewportRect)
        {
            var threshold = 0.6;
            var rectRate = new Vec4f(0.7460f, 0.8717f, 0.9083f, 0.9869f);
            return TryClickButton(viewportMat, viewportRect, "next_step.png", rectRate, threshold);
        }
    }
}
