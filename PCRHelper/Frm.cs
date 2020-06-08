using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using ThreadingTimer = System.Threading.Timer;
using System.IO;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System.Text.RegularExpressions;

namespace PCRHelper
{
    public partial class Frm : Form
    {
        ConfigMgr configMgr = ConfigMgr.GetInstance();
        Tools tools = Tools.GetInstance();
        GraphicsTools graphicsTools = GraphicsTools.GetInstance();
        LogTools logTools = LogTools.GetInstance();

        public Frm()
        {
            InitializeComponent();
        }

        private void Frm_Load(object sender, EventArgs e)
        {
            configMgr.Init();
            logTools.SetRichTextBox(txtConsole);

            var mumuState = GetMumuState();
            viewportRect = mumuState.ViewportRect;
            viewportCapture = mumuState.DoCapture(viewportRect);
            mumuState.ClickArenaRefresh(viewportRect);
            //mumuState.ClickArenaPlayer(viewportRect, 1);

            //var mat = new Mat(configMgr.GetCacheFileFullPath("RankBin0.png"));


        }

        RECT viewportRect;
        Bitmap viewportCapture;
        MumuState mumuState;
        string name = "";
        string rank = "";
        Task scriptTask;
        CancellationTokenSource tokenSource;
        CancellationToken ct;

        MumuState GetMumuState()
        {
            if (mumuState == null)
            {
                mumuState = MumuState.Create();
            }
            return mumuState;
        }

        void StartArenaCaptureLoop()
        {
            name = txtName.Text;
            rank = txtRank.Text;

            logTools.Info("StartCaptureLoop...");
            mumuState = MumuState.Create();

            tokenSource = new CancellationTokenSource();
            ct = tokenSource.Token;
            scriptTask = new Task(() =>
            {

                while (true)
                {
                    if (ct.IsCancellationRequested)
                    {
                        ct.ThrowIfCancellationRequested();
                    }
                    Thread.Sleep(2000);
                    int idx = -1;
                    bool hasError = false;
                    try
                    {
                        viewportRect = mumuState.ViewportRect;
                        viewportCapture = mumuState.DoCapture(viewportRect);
                        idx = ArenaCaptureLoopFunc();
                    }
                    catch (Exception e)
                    {
                        hasError = true;
                        logTools.Error(e.Message);
                    }
                    logTools.Info("INDEX: " + idx);
                    if (idx != -1)
                    {
                        mumuState.ClickArenaPlayer(viewportRect, idx);
                        break;
                    }
                    else if (!hasError)
                    {
                        mumuState.ClickArenaRefresh(viewportRect);//不要移动到try里面
                    }
                }
            }, tokenSource.Token);
            scriptTask.ContinueWith((t) => {
                if (t.IsFaulted)
                {
                    logTools.Error(t.Exception.Message);
                }
                else if (t.IsCanceled)
                {
                    logTools.Error("Canceld");
                }
            });
            scriptTask.Start();
        }

        void StopArenaCaptureLoop()
        {
            tokenSource?.Cancel();
        }

        int ArenaCaptureLoopFunc()
        {
            var list = GetCaptureResults();
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

        struct CaptureResult
        {
            public int Index;
            public string Name;
            public string Rank;
        }

        List<CaptureResult> GetCaptureResults()
        {
            var r = new List<CaptureResult>();
            for (int i = 0; i < 3; i++)
            {
                r.Add(new CaptureResult());
            }
            var tasks = new Task[3];
            var viewportCaptureClone = viewportCapture.ToOpenCvMat();
            for (int i = 0; i < 3; i++)
            {
                var index = i;
                var task = new Task(() =>
                {
                    
                    var name = mumuState.DoArenaPlayerNameOCR(viewportCaptureClone, viewportRect, index);
                    var rank = mumuState.DoArenaPlayerRankOCR(viewportCaptureClone, viewportRect, index);
                    r[index] = new CaptureResult()
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

        void StartActStageExchangeLoop()
        {
            logTools.Info("StartActStageExchangeLoop...");
            mumuState = MumuState.Create();

            tokenSource = new CancellationTokenSource();
            ct = tokenSource.Token;
            scriptTask = new Task(() =>
            {
                while (true)
                {
                    if (ct.IsCancellationRequested)
                    {
                        ct.ThrowIfCancellationRequested();
                    }
                    Thread.Sleep(2000);
                    viewportRect = mumuState.ViewportRect;
                    mumuState.ClickActStageExchange(viewportRect);
                }
            }, tokenSource.Token);
            scriptTask.ContinueWith((t) =>
            {
                if (t.IsFaulted)
                {
                    logTools.Error(t.Exception.Message);
                }
                else if (t.IsCanceled)
                {
                    logTools.Error("Canceld");
                }
            });
            scriptTask.Start();
        }

        void StopActStageExchangeLoop()
        {
            tokenSource?.Cancel();
        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            name = txtName.Text;
            rank = txtRank.Text;
        }

        private void menuClearConsole_Click(object sender, EventArgs e)
        {
            txtConsole.Clear();
        }

        private void menuGetRectRate_Click_1(object sender, EventArgs e)
        {
            var mumuState = GetMumuState();
            viewportRect = mumuState.ViewportRect;
            viewportCapture = mumuState.DoCapture(viewportRect);
            var getRectRateFrm = new FrmGetRectRate();
            getRectRateFrm.LoadImage(viewportCapture);
            getRectRateFrm.Show();
        }

        private void menuOpenCacheDir_Click_1(object sender, EventArgs e)
        {
            tools.OpenDirInExplorer(configMgr.CacheDir);
        }

        private void menuStartArenaCaptureLoop_Click(object sender, EventArgs e)
        {
            if (scriptTask != null && scriptTask.Status == TaskStatus.Running)
            {
                logTools.Error("Already has one running Script Task");
                return;
            }
            StartArenaCaptureLoop();
        }

        private void menuStopArenaCaptureLoop_Click(object sender, EventArgs e)
        {
            StopArenaCaptureLoop();
        }

        private void menuStartActStageExchangeLoop_Click(object sender, EventArgs e)
        {
            if (scriptTask != null && scriptTask.Status == TaskStatus.Running)
            {
                logTools.Error("Already has one running Script Task");
                return;
            }
            StartActStageExchangeLoop();
        }

        private void menuStopActStageExchangeLoop_Click(object sender, EventArgs e)
        {
            StopActStageExchangeLoop();
        }
    }
}
