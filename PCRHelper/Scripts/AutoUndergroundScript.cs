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
            var normalBoxThreshold = 0.8;
            var rectRate = fullPartRectRate;
            bool f = false;
            for (int i = 0; i < 3; i++)
            {
                var matchRes = MatchImage(viewportMat, viewportRect, rectRate, "underground_normal_box.png", normalBoxThreshold);
                if (!matchRes.Success) break;
                f = true;
                var normalBoxAbsoluteRect = GetMatchedAbsoluteRect(viewportRect, rectRate, matchRes.MatchedRect);
                var newX1Rate = 1.0f * normalBoxAbsoluteRect.x2 / viewportRect.Width;
                rectRate.Item0 = newX1Rate;
                var boxCenterPos = normalBoxAbsoluteRect.GetCenterPos();
                var emulatorPoint = MumuState.GetEmulatorPoint(viewportRect, boxCenterPos);
                MumuState.DoClick(emulatorPoint);
                Thread.Sleep(500);
            }
            return f;
        }

        public bool TryClickSpecailBox(Mat viewportMat, RECT viewportRect)
        {
            var specialBoxThreshold = 0.8;
            var rectRate = fullPartRectRate;
            bool f = false;
            for (int i = 0; i < 3; i++)
            {
                var matchRes = MatchImage(viewportMat, viewportRect, rectRate, "underground_special_box.png", specialBoxThreshold);
                if (!matchRes.Success) break;
                f = true;
                var specialBoxAbsoluteRect = GetMatchedAbsoluteRect(viewportRect, rectRate, matchRes.MatchedRect);
                var newX1Rate = 1.0f * specialBoxAbsoluteRect.x2 / viewportRect.Width;
                rectRate.Item0 = newX1Rate;
                var boxCenterPos = specialBoxAbsoluteRect.GetCenterPos();
                var emulatorPoint = MumuState.GetEmulatorPoint(viewportRect, boxCenterPos);
                MumuState.DoClick(emulatorPoint);
                Thread.Sleep(500);
            }
            return f;
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
    }

    enum UndergroundDifficulty
    {
        Normal,
        Hard,
        Veryhard,
    }
}
