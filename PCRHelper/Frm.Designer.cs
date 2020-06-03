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
            this.menuGetRectRate = new System.Windows.Forms.ToolStripMenuItem();
            this.menuOpenCacheDir = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStartCaptureLoop = new System.Windows.Forms.ToolStripMenuItem();
            this.menuClearConsole = new System.Windows.Forms.ToolStripMenuItem();
            this.txtName = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtRank = new System.Windows.Forms.RichTextBox();
            this.txtConsole = new System.Windows.Forms.RichTextBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.menuStopCaptureLoop = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuGetRectRate,
            this.menuOpenCacheDir,
            this.menuStartCaptureLoop,
            this.menuStopCaptureLoop,
            this.menuClearConsole});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(8, 3, 0, 3);
            this.menuStrip1.Size = new System.Drawing.Size(532, 27);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // menuGetRectRate
            // 
            this.menuGetRectRate.Name = "menuGetRectRate";
            this.menuGetRectRate.Size = new System.Drawing.Size(91, 21);
            this.menuGetRectRate.Text = "GetRectRate";
            this.menuGetRectRate.Click += new System.EventHandler(this.menuGetRectRate_Click);
            // 
            // menuOpenCacheDir
            // 
            this.menuOpenCacheDir.Name = "menuOpenCacheDir";
            this.menuOpenCacheDir.Size = new System.Drawing.Size(104, 21);
            this.menuOpenCacheDir.Text = "OpenCacheDir";
            this.menuOpenCacheDir.Click += new System.EventHandler(this.menuOpenCacheDir_Click);
            // 
            // menuStartCaptureLoop
            // 
            this.menuStartCaptureLoop.Name = "menuStartCaptureLoop";
            this.menuStartCaptureLoop.Size = new System.Drawing.Size(93, 21);
            this.menuStartCaptureLoop.Text = "StartCapture";
            this.menuStartCaptureLoop.Click += new System.EventHandler(this.menuStartCaptureLoop_Click);
            // 
            // menuClearConsole
            // 
            this.menuClearConsole.Name = "menuClearConsole";
            this.menuClearConsole.Size = new System.Drawing.Size(97, 21);
            this.menuClearConsole.Text = "ClearConsole";
            this.menuClearConsole.Click += new System.EventHandler(this.menuClearConsole_Click);
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(100, 71);
            this.txtName.Margin = new System.Windows.Forms.Padding(4);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(233, 38);
            this.txtName.TabIndex = 1;
            this.txtName.Text = "";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(36, 74);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 17);
            this.label1.TabIndex = 2;
            this.label1.Text = "名字";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(36, 148);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(42, 17);
            this.label2.TabIndex = 4;
            this.label2.Text = "排名";
            // 
            // txtRank
            // 
            this.txtRank.Location = new System.Drawing.Point(100, 145);
            this.txtRank.Margin = new System.Windows.Forms.Padding(4);
            this.txtRank.Name = "txtRank";
            this.txtRank.Size = new System.Drawing.Size(233, 38);
            this.txtRank.TabIndex = 3;
            this.txtRank.Text = "";
            // 
            // txtConsole
            // 
            this.txtConsole.Location = new System.Drawing.Point(12, 201);
            this.txtConsole.Name = "txtConsole";
            this.txtConsole.ReadOnly = true;
            this.txtConsole.Size = new System.Drawing.Size(508, 320);
            this.txtConsole.TabIndex = 5;
            this.txtConsole.Text = "";
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // menuStopCaptureLoop
            // 
            this.menuStopCaptureLoop.Name = "menuStopCaptureLoop";
            this.menuStopCaptureLoop.Size = new System.Drawing.Size(93, 21);
            this.menuStopCaptureLoop.Text = "StopCapture";
            this.menuStopCaptureLoop.Click += new System.EventHandler(this.menuStopCaptureLoop_Click);
            // 
            // Frm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(532, 533);
            this.Controls.Add(this.txtConsole);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtRank);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.menuStrip1);
            this.Font = new System.Drawing.Font("Consolas", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4);
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
        private System.Windows.Forms.ToolStripMenuItem menuGetRectRate;
        private System.Windows.Forms.ToolStripMenuItem menuOpenCacheDir;
        private System.Windows.Forms.RichTextBox txtName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RichTextBox txtRank;
        private System.Windows.Forms.ToolStripMenuItem menuStartCaptureLoop;
        private System.Windows.Forms.RichTextBox txtConsole;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ToolStripMenuItem menuClearConsole;
        private System.Windows.Forms.ToolStripMenuItem menuStopCaptureLoop;
    }
}

