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

        public abstract void OnStart(Bitmap viewportCapture, RECT viewportRect);

        public abstract void Tick(Bitmap viewportCapture, RECT viewportRect);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewportMat"></param>
        /// <param name="viewportRect"></param>
        /// <param name="exRectRate">采样区域</param>
        /// <param name="exName"></param>
        /// <returns></returns>
        public MatchImageResult MatchImage(Mat viewportMat, RECT viewportRect, Vec4f exRectRate, string exName)
        {
            var exRectMat = viewportMat.GetChildMatByRectRate(exRectRate);
            var exImgMat = ConfigMgr.GetInstance().GetPCRExImg(exName, viewportMat, viewportRect);
            var matchRes = GraphicsTools.GetInstance().MatchImage(exRectMat, exImgMat);
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
    }
}
