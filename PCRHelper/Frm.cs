﻿using System;
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
            //viewportRect = mumuState.ViewportRect;
            //viewportCapture = mumuState.DoCapture(viewportRect);
            //mumuState.ClickArenaRefresh(viewportRect);
            //mumuState.ClickArenaPlayer(viewportRect, 1);

            //var mat = new Mat(configMgr.GetCacheFileFullPath("RankBin0.png"));

        }

        RECT viewportRect;
        Bitmap viewportCapture;
        MumuState mumuState;
        string name = "";
        string rank = "";
        Task captureTask;
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

        void DelayStartCaptureLoop()
        {
            logTools.Info("DelayStartCaptureLoop...");
            name = txtName.Text;
            rank = txtRank.Text;
            StartCaptureLoop();
        }

        void StartCaptureLoop()
        {
            logTools.Info("StartCaptureLoop...");
            mumuState = MumuState.Create();

            tokenSource = new CancellationTokenSource();
            ct = tokenSource.Token;
            captureTask = new Task(() =>
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
                        idx = CaptureLoopFunc();
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
            captureTask.ContinueWith((t) => {
                if (t.IsFaulted)
                {
                    logTools.Error(t.Exception.Message);
                }
                else if (t.IsCanceled)
                {
                    logTools.Error("Canceld");
                }
            });
            captureTask.Start();
        }

        int CaptureLoopFunc()
        {
            var list = GetCaptureResults();
            for (int i = 0; i < list.Count; i++)
            {
                var item = list[i];
                logTools.Info($"Name Pattern: {name}");
                if (!string.IsNullOrWhiteSpace(name) && Regex.IsMatch(item.Name, name))
                {
                    return i;
                }
                logTools.Info($"Rank Pattern: {rank}");
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
                    r[i] = new CaptureResult()
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

        private void menuGetRectRate_Click(object sender, EventArgs e)
        {
            var mumuState = GetMumuState();
            viewportRect = mumuState.ViewportRect;
            viewportCapture = mumuState.DoCapture(viewportRect);
            var getRectRateFrm = new FrmGetRectRate();
            getRectRateFrm.LoadImage(viewportCapture);
            getRectRateFrm.Show();
        }

        private void menuOpenCacheDir_Click(object sender, EventArgs e)
        {
            tools.OpenDirInExplorer(configMgr.CacheDir);
        }

        private void menuStartCaptureLoop_Click(object sender, EventArgs e)
        {
            if (captureTask != null && captureTask.Status == TaskStatus.Running)
            {
                logTools.Error("Already has one running Task");
                return;
            }
            DelayStartCaptureLoop();
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

        private void menuStopCaptureLoop_Click(object sender, EventArgs e)
        {
            tokenSource?.Cancel();
        }
    }
}
