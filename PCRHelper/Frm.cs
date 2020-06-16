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
            RefreshFixedTopBottomY();

            var mumuState = GetMumuState();
            //viewportRect = mumuState.ViewportRect;
            //viewportCapture = mumuState.DoCapture(viewportRect);
            //mumuState.ClickTab(viewportRect, PCRTab.Menu);
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
        FrmGetRectRate getRectRateFrm;
        bool oldFixedTopBottomY;

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
            configMgr.InitTesseractConfig();
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
            oldFixedTopBottomY = configMgr.FixedViewportTopBottomY;
            configMgr.FixedViewportTopBottomY = true;
            logTools.Info("StartStorySkipLoop...");
            mumuState = GetMumuState();

            script = new ReadStoryScript();
            StartScriptLoop(script);
        }

        void StopStorySkipLoop()
        {
            StopScriptLoop();
        }

        void StartAutoUndergroundLoop()
        {
            logTools.Info("StartAutoUndergroundLoop...");
            mumuState = GetMumuState();

            script = new AutoUndergroundScript();
            StartScriptLoop(script);
        }

        void StopAutoUndergroundLoop()
        {
            StopScriptLoop();
        }

        void StartReadReliabilityLoop()
        {
            logTools.Info("StartReadReliabilityLoop...");
            mumuState = GetMumuState();

            script = new ReadReliabilityScript();
            StartScriptLoop(script);
        }

        void PrintException(Exception e)
        {
            var ex = e.InnerException ?? e;
            logTools.Error(ex);
            if (logTools.IsSelfOrChildrenNoTrackTraceException(ex))
            {
                //
            }
            else
            {
                logTools.Error(ex.StackTrace);
            }            
        }

        

        void StartScriptLoop(ScriptBase script)
        {
            if (scriptTask != null && scriptTask.Status == TaskStatus.Running)
            {
                logTools.Error("Already has one running Script Task", false);
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
                    try
                    {
                        Thread.Sleep(script.Interval);
                        viewportRect = mumuState.ViewportRect;
                        viewportCapture = mumuState.DoCapture(viewportRect);
                        logTools.Info($"Script: {script.Name} Tick");
                        script.Tick(viewportCapture, viewportRect);
                    }
                    catch (Exception e)
                    {
                        logTools.Error($"Script: {script.Name} Tick ERROR", false);
                        if (!script.CanKeepOnWhenException || logTools.IsSelfOrChildrenBreakException(e))
                        {
                            throw e;
                        }
                        else
                        {
                            PrintException(e);
                        }
                    }
                }
            }, tokenSource.Token);
            scriptTask.ContinueWith((t) =>
            {
                if (t.IsFaulted)
                {
                    PrintException(t.Exception);
                }
                else if (t.IsCanceled)
                {
                    logTools.Error("Script Canceled", false);
                }
                ScriptBase.OnScriptEnded(script.Name, !t.IsFaulted && !t.IsCanceled);
            });
            scriptTask.Start();
        }

        void StopScriptLoop()
        {
            tokenSource?.Cancel();
            configMgr.FixedViewportTopBottomY = oldFixedTopBottomY;
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

        void ShowGetRectRateForm()
        {
            var mumuState = GetMumuState();
            viewportRect = mumuState.ViewportRect;
            viewportCapture = mumuState.DoCapture(viewportRect);
            ShowGetRectRateForm(viewportCapture);
        }

        void ShowGetRectRateFormFlipUpDown()
        {
            var mumuState = GetMumuState();
            viewportRect = mumuState.ViewportRect;
            viewportCapture = mumuState.DoCapture(viewportRect);
            var mat = viewportCapture.ToOpenCvMat();
            var flip = new Mat();
            Cv2.Flip(mat, flip, FlipMode.XY);
            //Cv2.Circle(flip, new OpenCvSharp.Point(800, 400), 10, Scalar.Red);
            flip.SaveImage(configMgr.GetCacheFileFullPath("flip.png"));
            ShowGetRectRateForm(flip.ToRawBitmap());
        }

        void ShowGetRectRateForm(Bitmap bitmap)
        {
            getRectRateFrm?.Close();
            getRectRateFrm = new FrmGetRectRate();
            getRectRateFrm.LoadImage(bitmap);
            getRectRateFrm.Show();
        }

        private void menuGetRectRate_Click_1(object sender, EventArgs e)
        {
            ShowGetRectRateForm();
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
            ShowGetRectRateFormFlipUpDown();
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

        private void menuSetTesseract_Click(object sender, EventArgs e)
        {
            configMgr.SetTesseractPath();
        }

        private void menuSetPCRExImgDir_Click(object sender, EventArgs e)
        {
            configMgr.SetPCRExImgsDir();
        }

        private void menuSetAdbServer_Click(object sender, EventArgs e)
        {
            configMgr.SetAdbServerExePath();
        }

        private void menuStopScriptLoop_Click(object sender, EventArgs e)
        {
            StopScriptLoop();
        }

        private void menuStartAutoUnderground_Click(object sender, EventArgs e)
        {
            StartAutoUndergroundLoop();
        }

        void RefreshFixedTopBottomY()
        {
            menuSetFixedTopBottomY.Checked = configMgr.FixedViewportTopBottomY;
        }

        private void menuSetFixedTopBottomY_Click(object sender, EventArgs e)
        {

            configMgr.FixedViewportTopBottomY = !configMgr.FixedViewportTopBottomY;
            RefreshFixedTopBottomY();
        }

        private void menuAbout_Click(object sender, EventArgs e)
        {
            var frmAbout = new FrmAbout();
            frmAbout.Show();
        }

        private void menuStartReadReliabilityScript_Click(object sender, EventArgs e)
        {
            StartReadReliabilityLoop();
        }
    }
}
