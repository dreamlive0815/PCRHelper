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
using PCRHelper.Scripts;

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

            //var mumuState = GetMumuState();
            //viewportRect = mumuState.ViewportRect;
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
                logTools.Info($"Script: {script.Name} OnStart");
                script.OnStart();
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
                    logTools.Error("Canceld");
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
    }
}
