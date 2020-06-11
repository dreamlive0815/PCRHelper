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
using PCRHelper.Scripts;
using RawSize = System.Drawing.Size;
using CvSize = OpenCvSharp.Size;
using OpenCvSharp;

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
            RefreshRegions();

            var mumuState = GetMumuState();
            //viewportRect = mumuState.ViewportRect;
            //mumuState.ClickTab(viewportRect, PCRTab.Menu);
            //viewportCapture = mumuState.DoCapture(viewportRect);
        }

        RECT viewportRect;
        Bitmap viewportCapture;
        MumuState mumuState;
        string name = "";
        string rank = "";
        ScriptBase script;
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

        void StartArenaSearchLoop()
        {
            name = txtName.Text;
            rank = txtRank.Text;

            logTools.Info("StartCaptureLoop...");
            mumuState = GetMumuState();

            var script = new ArenaSearchScript();
            script.SetGetPatternNameFunc(new Func<string>(() =>
            {
                return name;
            }));
            script.SetGetPatternRankFunc(new Func<string>(() =>
            {
                return rank;
            }));
            this.script = script;
            StartScriptLoop(script);
        }

        void StopArenaSearchLoop()
        {
            StopScriptLoop();
        }

        void StartActStageExchangeLoop()
        {
            logTools.Info("StartActStageExchangeLoop...");
            mumuState = GetMumuState();

            script = new ActivityStageExchangeScript();
            StartScriptLoop(script);
        }

        void StopActStageExchangeLoop()
        {
            StopScriptLoop();
        }

        void StartStorySkipLoop()
        {
            logTools.Info("StartStorySkipLoop...");
            mumuState = GetMumuState();

            script = new ReadStoryScript();
            StartScriptLoop(script);
        }

        void StopStorySkipLoop()
        {
            StopScriptLoop();
        }

        void StartScriptLoop(ScriptBase script)
        {
            if (scriptTask != null && scriptTask.Status == TaskStatus.Running)
            {
                logTools.Error("Already has one running Script Task");
                return;
            }
            logTools.Info("StartScriptLoop...");
            mumuState = MumuState.Create();
            script.SetMumuState(mumuState);
            tokenSource = new CancellationTokenSource();
            ct = tokenSource.Token;
            scriptTask = new Task(() =>
            {
                viewportRect = mumuState.ViewportRect;
                viewportCapture = mumuState.DoCapture(viewportRect);
                logTools.Info($"Script: {script.Name} OnStart");
                script.OnStart(viewportCapture, viewportRect);
                while (true)
                {
                    if (ct.IsCancellationRequested)
                    {
                        ct.ThrowIfCancellationRequested();
                    }
                    Thread.Sleep(script.Interval);
                    viewportRect = mumuState.ViewportRect;
                    viewportCapture = mumuState.DoCapture(viewportRect);
                    logTools.Info($"Script: {script.Name} Tick");
                    script.Tick(viewportCapture, viewportRect);
                    //try
                    //{
                        
                    //}
                    //catch (Exception e)
                    //{
                    //    logTools.Error(e.Message);
                    //}
                }
            }, tokenSource.Token);
            scriptTask.ContinueWith((t) =>
            {
                if (t.IsFaulted)
                {
                    var msg = t.Exception.Message;
                    if (msg.Contains("一个或多个") && t.Exception.InnerException != null)
                    {
                        msg = t.Exception.InnerException.Message;
                    }
                    logTools.Error(msg);
                }
                else if (t.IsCanceled)
                {
                    logTools.Error("Script Canceled");
                }
            });
            scriptTask.Start();
        }

        void StopScriptLoop()
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
            StartArenaSearchLoop();
        }

        private void menuStopArenaCaptureLoop_Click(object sender, EventArgs e)
        {
            StopArenaSearchLoop();
        }

        private void menuStartActStageExchangeLoop_Click(object sender, EventArgs e)
        {
            StartActStageExchangeLoop();
        }

        private void menuStopActStageExchangeLoop_Click(object sender, EventArgs e)
        {
            StopActStageExchangeLoop();
        }

        private void menuTemp_Click(object sender, EventArgs e)
        {
            menuGetRectRate_Click_1(sender, e);
        }

        private void menuStartStorySkipLoop_Click(object sender, EventArgs e)
        {
            StartStorySkipLoop();
        }

        private void menuStopStorySkipLoop_Click(object sender, EventArgs e)
        {
            StopStorySkipLoop();
        }

        void RefreshRegions()
        {
            var pcrRegion = configMgr.PCRRegion;
            menuMainland.Checked = pcrRegion == PCRRegion.Mainland;
            menuTaiwan.Checked = pcrRegion == PCRRegion.Taiwan;
            menuJapan.Checked = pcrRegion == PCRRegion.Japan;
            Text = $"Current Region: {pcrRegion}";
        }

        private void menuMainland_Click(object sender, EventArgs e)
        {
            configMgr.PCRRegion = PCRRegion.Mainland;
            RefreshRegions();
        }

        private void menuTaiwan_Click(object sender, EventArgs e)
        {
            configMgr.PCRRegion = PCRRegion.Taiwan;
            RefreshRegions();
        }

        private void menuJapan_Click(object sender, EventArgs e)
        {
            configMgr.PCRRegion = PCRRegion.Japan;
            RefreshRegions();
        }
    }
}
