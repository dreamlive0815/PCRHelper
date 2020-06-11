using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PCRHelper.Scripts
{
    class ArenaSearchScript : ScriptBase
    {

        private LogTools logTools = LogTools.GetInstance();
        private Func<string> getNameFunc;
        private Func<string> getRankFunc;

        public override string Name
        {
            get { return "ArenaSearchScript"; }
        }

        public void SetGetPatternNameFunc(Func<string> getNameFunc)
        {
            this.getNameFunc = getNameFunc;
        }

        public void SetGetPatternRankFunc(Func<string> getRankFunc)
        {
            this.getRankFunc = getRankFunc;
        }

        public override void OnStart(Bitmap viewportCapture, RECT viewportRect)
        {
            
        }

        public override void Tick(Bitmap viewportCapture, RECT viewportRect)
        {
            var idx = -1;
            bool hasError = false;
            List<ArenaPlayerPCRResult> list = null;
            try
            {
                list = GetArenaPlayerOCRResults(viewportCapture, viewportRect);
                idx = GetArenaPlayerIndex(list);
            }
            catch (Exception e)
            {
                hasError = true;
                logTools.Error(e.Message);
            }
            logTools.Info("Player Index: " + idx + (idx == -1 ? "(Not Found)" : ""));
            if (idx != -1)
            {
                MumuState.ClickArenaPlayer(viewportRect, idx);
                var res = list[idx];
                throw new Exception($"已找到目标玩家 名字:{res.Name} 排名:{res.Rank}, 脚本终止");
            }
            else if (!hasError)
            {
                MumuState.ClickArenaRefresh(viewportRect);//不要移动到try里面
            }
        }

        int GetArenaPlayerIndex(List<ArenaPlayerPCRResult> list)
        {
            var name = getNameFunc();
            var rank = getRankFunc();
            logTools.Info($"Name Pattern: {name}");
            logTools.Info($"Rank Pattern: {rank}");
            for (int i = 0; i < list.Count; i++)
            {
                var item = list[i];
                logTools.Info($"Name{i}: {item.Name}");
                if (!string.IsNullOrWhiteSpace(name) && Regex.IsMatch(item.Name, name))
                {
                    return i;
                }
                logTools.Info($"Rank{i}: {item.Rank}");
                if (!string.IsNullOrWhiteSpace(rank) && Regex.IsMatch(item.Rank, rank))
                {
                    return i;
                }
            }
            return -1;
        }

        List<ArenaPlayerPCRResult> GetArenaPlayerOCRResults(Bitmap viewportCapture, RECT viewportRect)
        {
            var r = new List<ArenaPlayerPCRResult>();
            for (int i = 0; i < 3; i++)
            {
                r.Add(new ArenaPlayerPCRResult());
            }
            var tasks = new Task[3];
            var viewportCaptureClone = viewportCapture.ToOpenCvMat();
            for (int i = 0; i < 3; i++)
            {
                var index = i;
                var task = new Task(() =>
                {

                    var name = MumuState.DoArenaPlayerNameOCR(viewportCaptureClone, viewportRect, index);
                    var rank = MumuState.DoArenaPlayerRankOCR(viewportCaptureClone, viewportRect, index);
                    r[index] = new ArenaPlayerPCRResult()
                    {
                        Index = index,
                        Name = name,
                        Rank = rank,
                    };
                });
                task.Start();
                tasks[i] = task;
            }
            Task.WaitAll(tasks);
            return r;
        }
    }

    struct ArenaPlayerPCRResult
    {
        public int Index;
        public string Name;
        public string Rank;
    }
}
