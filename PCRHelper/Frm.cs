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
        readonly string imgName = "1.png";

        public Frm()
        {
            InitializeComponent();
        }

        private void Frm_Load(object sender, EventArgs e)
        {
            configMgr.Init();
            logTools.SetRichTextBox(txtConsole);

            //var im = Regex.IsMatch("初雪(6群在挖)", "6[\s\S]*群");

        }

        RECT viewportRect;
        Image viewportCapture;
        MumuState mumuState;
        string name = "";
        string rank = "";
        Task captureTask;

        void DelayStartCaptureLoop()
        {
            logTools.Info("DelayStartCaptureLoop...");
            name = txtName.Text;
            rank = txtRank.Text;
            //ThreadingTimer timer = null;
            //timer = new System.Threading.Timer(new TimerCallback((arg) =>
            //{
            //    StartCaptureLoop();
            //}), null, 2000, Timeout.Infinite);
            StartCaptureLoop();
        }

        void StartCaptureLoop()
        {
            logTools.Info("StartCaptureLoop...");
            mumuState = MumuState.Create();

            captureTask = new Task(() =>
            {

                while (true)
                {
                    Thread.Sleep(2000);
                    int idx = 0;
                    try
                    {
                        viewportRect = mumuState.ViewportRect;
                        viewportCapture = mumuState.DoCapture(viewportRect);
                        idx = CaptureLoopFunc();
                    }
                    catch (Exception e)
                    {
                        logTools.Error(e.Message);
                    }
                    logTools.Info("INDEX: " + idx);
                    if (idx != 0)
                    {
                        mumuState.ClickJJCRect(viewportRect, idx);
                        break;
                    }
                    else
                    {
                        mumuState.ClickJJCRefreshButton(viewportRect);
                    }
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
            return 0;
        }

        struct CaptureResult
        {
            public int index;
            public string Name;
            public string Rank;
        }

        List<CaptureResult> GetCaptureResults()
        {
            var ocr = OCRTools.GetInstance();
            var r = new List<CaptureResult>();
            for (int i = 0; i < 3; i++)
            {
                var nameCR = mumuState.GetJJCNameCaptureRect(viewportCapture, viewportRect, i);
                graphicsTools.ShowImage("NameToOCR" + i, nameCR);
                var name = ocr.ToGrayAndOCR(nameCR);
                logTools.Info($"Name{i}: {name}");
                var rankCR = mumuState.GetJJCRankCaptureRect(viewportCapture, viewportRect, i);
                var gray = graphicsTools.ToGray(rankCR);
                var reverse = graphicsTools.ToReverse(gray);
                graphicsTools.ShowImage("RankReverse" + i, reverse);
                var bin = graphicsTools.ToBinaryPlus(reverse, 90);
                //graphicsTools.ShowImage("RankBin" + i, bin);
                var binStorePath = configMgr.GetCacheFileFullPath($"RankBin{i}.png");
                bin.SaveImage(binStorePath);
                var bitmap = graphicsTools.CleanBinCorner((Bitmap)Bitmap.FromFile(binStorePath));
                graphicsTools.ShowImage("RankToOCR" + i, bitmap);
                var rank = ocr.OCR(bitmap);
                logTools.Info($"Rank{i}: {rank}");
                r.Add(new CaptureResult() { index = 1, Name = name, Rank = rank });
            }
            return r;
        }

        private void menuGetRectRate_Click(object sender, EventArgs e)
        {
            new FrmGetRectRate().Show();
        }

        private void menuOpenCacheDir_Click(object sender, EventArgs e)
        {
            var cacheDir = ConfigMgr.GetInstance().CacheDir;
            tools.OpenDirInExplorer(cacheDir);
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
    }
}
