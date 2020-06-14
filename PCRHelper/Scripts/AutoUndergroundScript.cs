﻿using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PCRHelper.Scripts
{
    class AutoUndergroundScript : ScriptBase
    {
        public override string Name
        {
            get { return "AutoUndergroundScript"; }
        }

        public override bool CanKeepOnWhenException
        {
            get { return true; }
        }

        private UndergroundDifficulty CurDifficulty { get; set; }

        private LogTools logTools = LogTools.GetInstance();

        public override void OnStart(Bitmap viewportCapture, RECT viewportRect)
        {
            CurDifficulty = UndergroundDifficulty.Veryhard;
            //TryClickButton(viewportCapture.ToOpenCvMat(), viewportRect, "underground_special_box.png", leftPartRectRate, 0.6);

            MumuState.ClickTab(viewportRect, PCRTab.Battle);
            Thread.Sleep(3000);
            MumuState.ClickBattleEntrance(viewportRect, PCRBattleMode.Underground);
            Thread.Sleep(2000);
        }

        public override void Tick(Bitmap viewportCapture, RECT viewportRect)
        {
            var viewportMat = viewportCapture.ToOpenCvMat();

            var f = true;
            if (TryClickChallengeButton(viewportMat, viewportRect))
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
            else if (TryClickGobackUnderGroundButton(viewportMat, viewportRect))
            {
                logTools.Info("TryClickGobackUnderGroundButton");
            }
            else
            {
                f = false;
            }

            if (!f)
            {
                var b1 = TryClickNormalBox(viewportMat, viewportRect);
                var b2 = TryClickSpecailBox(viewportMat, viewportRect);
                if (!b1 && !b2)
                {
                    logTools.Info("Click Back");
                    MumuState.ClickBack(viewportRect);
                }
            }

            TryClickNormalBox(viewportMat, viewportRect);
        }


        Dictionary<UndergroundDifficulty, string> entranceTagMap = new Dictionary<UndergroundDifficulty, string>()
        {
            { UndergroundDifficulty.Veryhard, "underground_veryhard_tag.png" },
        };

        public string GetEntranceTagName(UndergroundDifficulty difficulty)
        {
            return entranceTagMap[difficulty];
        }

        Vec4f fullPartRectRate = new Vec4f(0.0000f, 0.0000f, 1.0000f, 1.0000f);
        Vec4f leftPartRectRate = new Vec4f(0.0000f, 0.0000f, 0.5000f, 1.0000f);
        Vec4f rightPartRectRate = new Vec4f(0.5000f, 0.0000f, 1.0000f, 1.0000f);

        public bool TryClickNormalBox(Mat viewportMat, RECT viewportRect)
        {
            var threshold = 0.8;
            var rectRate = fullPartRectRate;
            var matchRes = MatchImage(viewportMat, viewportRect, rectRate, "underground_normal_box.png", threshold);
            if (!matchRes.Success) return false;
            var absoluteRect = GetMatchedAbsoluteRect(viewportRect, rectRate, matchRes.MatchedRect);
            var left2right = absoluteRect.GetCenterPos().X < 1.0f * viewportRect.Width / 2;
            var emulatorPoint = MumuState.GetEmulatorPoint(viewportRect, absoluteRect.GetCenterPos());
            MumuState.DoClick(emulatorPoint);
            Thread.Sleep(500);
            for (int i = 1; i < 3; i++)
            {
                if (left2right)
                {
                    var newX1Rate = 1.0f * absoluteRect.x2 / viewportRect.Width;
                    rectRate.Item0 = newX1Rate;
                }
                else
                {
                    var newX2Rate = 1.0f * absoluteRect.x1 / viewportRect.Width;
                    rectRate.Item2 = newX2Rate;
                }
                matchRes = MatchImage(viewportMat, viewportRect, rectRate, "underground_normal_box.png", threshold);
                if (!matchRes.Success) break;
                absoluteRect = GetMatchedAbsoluteRect(viewportRect, rectRate, matchRes.MatchedRect);
                emulatorPoint = MumuState.GetEmulatorPoint(viewportRect, absoluteRect.GetCenterPos());
                MumuState.DoClick(emulatorPoint);
                Thread.Sleep(500);
            }
            return true;
        }

        public bool TryClickSpecailBox(Mat viewportMat, RECT viewportRect)
        {
            var threshold = 0.8;
            var rectRate = fullPartRectRate;
            var matchRes = MatchImage(viewportMat, viewportRect, rectRate, "underground_special_box.png", threshold);
            if (!matchRes.Success) return false;
            var absoluteRect = GetMatchedAbsoluteRect(viewportRect, rectRate, matchRes.MatchedRect);
            var left2right = absoluteRect.GetCenterPos().X < 1.0f * viewportRect.Width / 2;
            var emulatorPoint = MumuState.GetEmulatorPoint(viewportRect, absoluteRect.GetCenterPos());
            MumuState.DoClick(emulatorPoint);
            Thread.Sleep(500);
            for (int i = 1; i < 3; i++)
            {
                if (left2right)
                {
                    var newX1Rate = 1.0f * absoluteRect.x2 / viewportRect.Width;
                    rectRate.Item0 = newX1Rate;
                }
                else
                {
                    var newX2Rate = 1.0f * absoluteRect.x1 / viewportRect.Width;
                    rectRate.Item2 = newX2Rate;
                }
                matchRes = MatchImage(viewportMat, viewportRect, rectRate, "underground_special_box.png", threshold);
                if (!matchRes.Success) break;
                absoluteRect = GetMatchedAbsoluteRect(viewportRect, rectRate, matchRes.MatchedRect);
                emulatorPoint = MumuState.GetEmulatorPoint(viewportRect, absoluteRect.GetCenterPos());
                MumuState.DoClick(emulatorPoint);
                Thread.Sleep(500);
            }
            return true;
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

        public bool TryClickRewardConfirmButton(Mat viewportMat, RECT viewportRect)
        {
            var threshold = 0.6;
            var rectRate = new Vec4f(0.4039f, 0.8382f, 0.5968f, 0.9344f);
            return TryClickButton(viewportMat, viewportRect, "reward_confirm_ok.png", rectRate, threshold);
        }


        public bool TryClickGobackUnderGroundButton(Mat viewportMat, RECT viewportRect)
        {
            var threshold = 0.6;
            var rectRate = new Vec4f(0.7074f, 0.8557f, 0.9054f, 0.9694f);
            return TryClickButton(viewportMat, viewportRect, "goback_underground.png", rectRate, threshold);
        }
    }

    enum UndergroundDifficulty
    {
        Normal,
        Hard,
        Veryhard,
    }
}
