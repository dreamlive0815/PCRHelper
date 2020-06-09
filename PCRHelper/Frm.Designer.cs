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
            this.menuStopArenaSearchLoop = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStartActStageExchangeLoop = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStopActStageExchangeLoop = new System.Windows.Forms.ToolStripMenuItem();
            this.menuClearConsole = new System.Windows.Forms.ToolStripMenuItem();
            this.menuTemp = new System.Windows.Forms.ToolStripMenuItem();
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
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuTools,
            this.menuScripts,
            this.menuClearConsole,
            this.menuTemp});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(8, 3, 0, 3);
            this.menuStrip1.Size = new System.Drawing.Size(532, 27);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // menuTools
            // 
            this.menuTools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuGetRectRate,
            this.menuOpenCacheDir});
            this.menuTools.Name = "menuTools";
            this.menuTools.Size = new System.Drawing.Size(67, 21);
            this.menuTools.Text = "Tools(&T)";
            // 
            // menuGetRectRate
            // 
            this.menuGetRectRate.Name = "menuGetRectRate";
            this.menuGetRectRate.Size = new System.Drawing.Size(160, 22);
            this.menuGetRectRate.Text = "GetRectRate";
            this.menuGetRectRate.Click += new System.EventHandler(this.menuGetRectRate_Click_1);
            // 
            // menuOpenCacheDir
            // 
            this.menuOpenCacheDir.Name = "menuOpenCacheDir";
            this.menuOpenCacheDir.Size = new System.Drawing.Size(160, 22);
            this.menuOpenCacheDir.Text = "OpenCacheDir";
            this.menuOpenCacheDir.Click += new System.EventHandler(this.menuOpenCacheDir_Click_1);
            // 
            // menuScripts
            // 
            this.menuScripts.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuStartArenaSearchLoop,
            this.menuStopArenaSearchLoop,
            this.menuStartActStageExchangeLoop,
            this.menuStopActStageExchangeLoop});
            this.menuScripts.Name = "menuScripts";
            this.menuScripts.Size = new System.Drawing.Size(74, 21);
            this.menuScripts.Text = "Scripts(&S)";
            // 
            // menuStartArenaSearchLoop
            // 
            this.menuStartArenaSearchLoop.Name = "menuStartArenaSearchLoop";
            this.menuStartArenaSearchLoop.Size = new System.Drawing.Size(239, 22);
            this.menuStartArenaSearchLoop.Text = "StartArenaSearchLoop";
            this.menuStartArenaSearchLoop.Click += new System.EventHandler(this.menuStartArenaCaptureLoop_Click);
            // 
            // menuStopArenaSearchLoop
            // 
            this.menuStopArenaSearchLoop.Name = "menuStopArenaSearchLoop";
            this.menuStopArenaSearchLoop.Size = new System.Drawing.Size(239, 22);
            this.menuStopArenaSearchLoop.Text = "StopArenaSearchLoop";
            this.menuStopArenaSearchLoop.Click += new System.EventHandler(this.menuStopArenaCaptureLoop_Click);
            // 
            // menuStartActStageExchangeLoop
            // 
            this.menuStartActStageExchangeLoop.Name = "menuStartActStageExchangeLoop";
            this.menuStartActStageExchangeLoop.Size = new System.Drawing.Size(239, 22);
            this.menuStartActStageExchangeLoop.Text = "StartActStageExchangeLoop";
            this.menuStartActStageExchangeLoop.Click += new System.EventHandler(this.menuStartActStageExchangeLoop_Click);
            // 
            // menuStopActStageExchangeLoop
            // 
            this.menuStopActStageExchangeLoop.Name = "menuStopActStageExchangeLoop";
            this.menuStopActStageExchangeLoop.Size = new System.Drawing.Size(239, 22);
            this.menuStopActStageExchangeLoop.Text = "StopActStageExchangeLoop";
            this.menuStopActStageExchangeLoop.Click += new System.EventHandler(this.menuStopActStageExchangeLoop_Click);
            // 
            // menuClearConsole
            // 
            this.menuClearConsole.Name = "menuClearConsole";
            this.menuClearConsole.Size = new System.Drawing.Size(97, 21);
            this.menuClearConsole.Text = "ClearConsole";
            this.menuClearConsole.Click += new System.EventHandler(this.menuClearConsole_Click);
            // 
            // menuTemp
            // 
            this.menuTemp.Name = "menuTemp";
            this.menuTemp.Size = new System.Drawing.Size(53, 21);
            this.menuTemp.Text = "Temp";
            this.menuTemp.Click += new System.EventHandler(this.menuTemp_Click);
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
            this.label1.Size = new System.Drawing.Size(42, 17);
            this.label1.TabIndex = 2;
            this.label1.Text = "名字";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(24, 79);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(42, 17);
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
            this.txtConsole.Size = new System.Drawing.Size(508, 338);
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
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(532, 472);
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
        private System.Windows.Forms.ToolStripMenuItem menuStopArenaSearchLoop;
        private System.Windows.Forms.ToolStripMenuItem menuStartActStageExchangeLoop;
        private System.Windows.Forms.ToolStripMenuItem menuStopActStageExchangeLoop;
        private System.Windows.Forms.ToolStripMenuItem menuTemp;
    }
}

