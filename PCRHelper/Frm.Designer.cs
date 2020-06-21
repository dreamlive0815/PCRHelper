namespace PCRHelper
{
    partial class Frm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.menuTools = new System.Windows.Forms.ToolStripMenuItem();
            this.menuGetRectRate = new System.Windows.Forms.ToolStripMenuItem();
            this.menuOpenCacheDir = new System.Windows.Forms.ToolStripMenuItem();
            this.menuScripts = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStartArenaSearchLoop = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStartActStageExchangeLoop = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStartStorySkipLoop = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStartAutoUnderground = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStartReadReliabilityScript = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStartAutoStageLine = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStartReceivePresentLoop = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStopScriptLoop = new System.Windows.Forms.ToolStripMenuItem();
            this.menuRegions = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMainland = new System.Windows.Forms.ToolStripMenuItem();
            this.menuTaiwan = new System.Windows.Forms.ToolStripMenuItem();
            this.menuJapan = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSetTesseract = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSetAdbServer = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSetPCRExImgDir = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSetFixedTopBottomY = new System.Windows.Forms.ToolStripMenuItem();
            this.menuClearConsole = new System.Windows.Forms.ToolStripMenuItem();
            this.menuTemp = new System.Windows.Forms.ToolStripMenuItem();
            this.menuAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.txtName = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtRank = new System.Windows.Forms.RichTextBox();
            this.txtConsole = new System.Windows.Forms.RichTextBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuTools,
            this.menuScripts,
            this.menuRegions,
            this.menuSettings,
            this.menuClearConsole,
            this.menuTemp,
            this.menuAbout});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(8, 3, 0, 3);
            this.menuStrip1.Size = new System.Drawing.Size(675, 30);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // menuTools
            // 
            this.menuTools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuGetRectRate,
            this.menuOpenCacheDir});
            this.menuTools.Name = "menuTools";
            this.menuTools.Size = new System.Drawing.Size(80, 24);
            this.menuTools.Text = "Tools(&T)";
            // 
            // menuGetRectRate
            // 
            this.menuGetRectRate.Name = "menuGetRectRate";
            this.menuGetRectRate.Size = new System.Drawing.Size(189, 26);
            this.menuGetRectRate.Text = "GetRectRate";
            this.menuGetRectRate.Click += new System.EventHandler(this.menuGetRectRate_Click_1);
            // 
            // menuOpenCacheDir
            // 
            this.menuOpenCacheDir.Name = "menuOpenCacheDir";
            this.menuOpenCacheDir.Size = new System.Drawing.Size(189, 26);
            this.menuOpenCacheDir.Text = "OpenCacheDir";
            this.menuOpenCacheDir.Click += new System.EventHandler(this.menuOpenCacheDir_Click_1);
            // 
            // menuScripts
            // 
            this.menuScripts.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuStartArenaSearchLoop,
            this.menuStartActStageExchangeLoop,
            this.menuStartStorySkipLoop,
            this.menuStartAutoUnderground,
            this.menuStartReadReliabilityScript,
            this.menuStartAutoStageLine,
            this.menuStartReceivePresentLoop,
            this.menuStopScriptLoop});
            this.menuScripts.Name = "menuScripts";
            this.menuScripts.Size = new System.Drawing.Size(90, 24);
            this.menuScripts.Text = "Scripts(&S)";
            // 
            // menuStartArenaSearchLoop
            // 
            this.menuStartArenaSearchLoop.Name = "menuStartArenaSearchLoop";
            this.menuStartArenaSearchLoop.Size = new System.Drawing.Size(293, 26);
            this.menuStartArenaSearchLoop.Text = "StartArenaSearchLoop";
            this.menuStartArenaSearchLoop.Click += new System.EventHandler(this.menuStartArenaCaptureLoop_Click);
            // 
            // menuStartActStageExchangeLoop
            // 
            this.menuStartActStageExchangeLoop.Name = "menuStartActStageExchangeLoop";
            this.menuStartActStageExchangeLoop.Size = new System.Drawing.Size(293, 26);
            this.menuStartActStageExchangeLoop.Text = "StartActStageExchangeLoop";
            this.menuStartActStageExchangeLoop.Click += new System.EventHandler(this.menuStartActStageExchangeLoop_Click);
            // 
            // menuStartStorySkipLoop
            // 
            this.menuStartStorySkipLoop.Name = "menuStartStorySkipLoop";
            this.menuStartStorySkipLoop.Size = new System.Drawing.Size(293, 26);
            this.menuStartStorySkipLoop.Text = "StartStorySkipLoop";
            this.menuStartStorySkipLoop.Click += new System.EventHandler(this.menuStartStorySkipLoop_Click);
            // 
            // menuStartAutoUnderground
            // 
            this.menuStartAutoUnderground.Name = "menuStartAutoUnderground";
            this.menuStartAutoUnderground.Size = new System.Drawing.Size(293, 26);
            this.menuStartAutoUnderground.Text = "StartAutoUnderground";
            this.menuStartAutoUnderground.Click += new System.EventHandler(this.menuStartAutoUnderground_Click);
            // 
            // menuStartReadReliabilityScript
            // 
            this.menuStartReadReliabilityScript.Name = "menuStartReadReliabilityScript";
            this.menuStartReadReliabilityScript.Size = new System.Drawing.Size(293, 26);
            this.menuStartReadReliabilityScript.Text = "StartReadReliabilityScript";
            this.menuStartReadReliabilityScript.Click += new System.EventHandler(this.menuStartReadReliabilityScript_Click);
            // 
            // menuStartAutoStageLine
            // 
            this.menuStartAutoStageLine.Name = "menuStartAutoStageLine";
            this.menuStartAutoStageLine.Size = new System.Drawing.Size(293, 26);
            this.menuStartAutoStageLine.Text = "StartAutoStageLine";
            this.menuStartAutoStageLine.Click += new System.EventHandler(this.menuStartAutoStageLine_Click);
            // 
            // menuStartReceivePresentLoop
            // 
            this.menuStartReceivePresentLoop.Name = "menuStartReceivePresentLoop";
            this.menuStartReceivePresentLoop.Size = new System.Drawing.Size(293, 26);
            this.menuStartReceivePresentLoop.Text = "StartReceivePresentLoop";
            this.menuStartReceivePresentLoop.Click += new System.EventHandler(this.menuStartReceivePresentLoop_Click);
            // 
            // menuStopScriptLoop
            // 
            this.menuStopScriptLoop.Name = "menuStopScriptLoop";
            this.menuStopScriptLoop.Size = new System.Drawing.Size(293, 26);
            this.menuStopScriptLoop.Text = "StopScriptLoop";
            this.menuStopScriptLoop.Click += new System.EventHandler(this.menuStopScriptLoop_Click);
            // 
            // menuRegions
            // 
            this.menuRegions.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuMainland,
            this.menuTaiwan,
            this.menuJapan});
            this.menuRegions.Name = "menuRegions";
            this.menuRegions.Size = new System.Drawing.Size(100, 24);
            this.menuRegions.Text = "Regions(&R)";
            // 
            // menuMainland
            // 
            this.menuMainland.Name = "menuMainland";
            this.menuMainland.Size = new System.Drawing.Size(181, 26);
            this.menuMainland.Text = "Mainland";
            this.menuMainland.Click += new System.EventHandler(this.menuMainland_Click);
            // 
            // menuTaiwan
            // 
            this.menuTaiwan.Name = "menuTaiwan";
            this.menuTaiwan.Size = new System.Drawing.Size(181, 26);
            this.menuTaiwan.Text = "Taiwan";
            this.menuTaiwan.Click += new System.EventHandler(this.menuTaiwan_Click);
            // 
            // menuJapan
            // 
            this.menuJapan.Name = "menuJapan";
            this.menuJapan.Size = new System.Drawing.Size(181, 26);
            this.menuJapan.Text = "Japan";
            this.menuJapan.Click += new System.EventHandler(this.menuJapan_Click);
            // 
            // menuSettings
            // 
            this.menuSettings.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuSetTesseract,
            this.menuSetAdbServer,
            this.menuSetPCRExImgDir,
            this.menuSetFixedTopBottomY});
            this.menuSettings.Name = "menuSettings";
            this.menuSettings.Size = new System.Drawing.Size(81, 24);
            this.menuSettings.Text = "Settings";
            // 
            // menuSetTesseract
            // 
            this.menuSetTesseract.Name = "menuSetTesseract";
            this.menuSetTesseract.Size = new System.Drawing.Size(240, 26);
            this.menuSetTesseract.Text = "SetTesseract";
            this.menuSetTesseract.Click += new System.EventHandler(this.menuSetTesseract_Click);
            // 
            // menuSetAdbServer
            // 
            this.menuSetAdbServer.Name = "menuSetAdbServer";
            this.menuSetAdbServer.Size = new System.Drawing.Size(240, 26);
            this.menuSetAdbServer.Text = "SetAdbServer";
            this.menuSetAdbServer.Click += new System.EventHandler(this.menuSetAdbServer_Click);
            // 
            // menuSetPCRExImgDir
            // 
            this.menuSetPCRExImgDir.Name = "menuSetPCRExImgDir";
            this.menuSetPCRExImgDir.Size = new System.Drawing.Size(240, 26);
            this.menuSetPCRExImgDir.Text = "SetPCRExImgDir";
            this.menuSetPCRExImgDir.Click += new System.EventHandler(this.menuSetPCRExImgDir_Click);
            // 
            // menuSetFixedTopBottomY
            // 
            this.menuSetFixedTopBottomY.Name = "menuSetFixedTopBottomY";
            this.menuSetFixedTopBottomY.Size = new System.Drawing.Size(240, 26);
            this.menuSetFixedTopBottomY.Text = "SetFixedTopBottomY";
            this.menuSetFixedTopBottomY.Visible = false;
            this.menuSetFixedTopBottomY.Click += new System.EventHandler(this.menuSetFixedTopBottomY_Click);
            // 
            // menuClearConsole
            // 
            this.menuClearConsole.Name = "menuClearConsole";
            this.menuClearConsole.Size = new System.Drawing.Size(117, 24);
            this.menuClearConsole.Text = "ClearConsole";
            this.menuClearConsole.Click += new System.EventHandler(this.menuClearConsole_Click);
            // 
            // menuTemp
            // 
            this.menuTemp.Name = "menuTemp";
            this.menuTemp.Size = new System.Drawing.Size(63, 24);
            this.menuTemp.Text = "Temp";
            this.menuTemp.Click += new System.EventHandler(this.menuTemp_Click);
            // 
            // menuAbout
            // 
            this.menuAbout.Name = "menuAbout";
            this.menuAbout.Size = new System.Drawing.Size(88, 24);
            this.menuAbout.Text = "About(&A)";
            this.menuAbout.Click += new System.EventHandler(this.menuAbout_Click);
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(74, 31);
            this.txtName.Margin = new System.Windows.Forms.Padding(4);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(233, 34);
            this.txtName.TabIndex = 1;
            this.txtName.Text = "";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 34);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 22);
            this.label1.TabIndex = 2;
            this.label1.Text = "名字";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(24, 79);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 22);
            this.label2.TabIndex = 4;
            this.label2.Text = "排名";
            // 
            // txtRank
            // 
            this.txtRank.Location = new System.Drawing.Point(74, 76);
            this.txtRank.Margin = new System.Windows.Forms.Padding(4);
            this.txtRank.Name = "txtRank";
            this.txtRank.Size = new System.Drawing.Size(233, 34);
            this.txtRank.TabIndex = 3;
            this.txtRank.Text = "";
            // 
            // txtConsole
            // 
            this.txtConsole.Location = new System.Drawing.Point(12, 117);
            this.txtConsole.Name = "txtConsole";
            this.txtConsole.ReadOnly = true;
            this.txtConsole.Size = new System.Drawing.Size(651, 343);
            this.txtConsole.TabIndex = 5;
            this.txtConsole.Text = "";
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // Frm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(675, 472);
            this.Controls.Add(this.txtConsole);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtRank);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.menuStrip1);
            this.Font = new System.Drawing.Font("Consolas", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Frm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Frm_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.RichTextBox txtName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RichTextBox txtRank;
        private System.Windows.Forms.RichTextBox txtConsole;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ToolStripMenuItem menuClearConsole;
        private System.Windows.Forms.ToolStripMenuItem menuTools;
        private System.Windows.Forms.ToolStripMenuItem menuGetRectRate;
        private System.Windows.Forms.ToolStripMenuItem menuOpenCacheDir;
        private System.Windows.Forms.ToolStripMenuItem menuScripts;
        private System.Windows.Forms.ToolStripMenuItem menuStartArenaSearchLoop;
        private System.Windows.Forms.ToolStripMenuItem menuStartActStageExchangeLoop;
        private System.Windows.Forms.ToolStripMenuItem menuTemp;
        private System.Windows.Forms.ToolStripMenuItem menuStartStorySkipLoop;
        private System.Windows.Forms.ToolStripMenuItem menuStopScriptLoop;
        private System.Windows.Forms.ToolStripMenuItem menuRegions;
        private System.Windows.Forms.ToolStripMenuItem menuMainland;
        private System.Windows.Forms.ToolStripMenuItem menuTaiwan;
        private System.Windows.Forms.ToolStripMenuItem menuJapan;
        private System.Windows.Forms.ToolStripMenuItem menuSettings;
        private System.Windows.Forms.ToolStripMenuItem menuSetTesseract;
        private System.Windows.Forms.ToolStripMenuItem menuSetPCRExImgDir;
        private System.Windows.Forms.ToolStripMenuItem menuSetAdbServer;
        private System.Windows.Forms.ToolStripMenuItem menuStartAutoUnderground;
        private System.Windows.Forms.ToolStripMenuItem menuSetFixedTopBottomY;
        private System.Windows.Forms.ToolStripMenuItem menuAbout;
        private System.Windows.Forms.ToolStripMenuItem menuStartReadReliabilityScript;
        private System.Windows.Forms.ToolStripMenuItem menuStartAutoStageLine;
        private System.Windows.Forms.ToolStripMenuItem menuStartReceivePresentLoop;
    }
}

