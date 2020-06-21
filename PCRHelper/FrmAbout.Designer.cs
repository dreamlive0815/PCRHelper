namespace PCRHelper
{
    partial class FrmAbout
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.labelContact = new System.Windows.Forms.Label();
            this.labelGithub = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // labelContact
            // 
            this.labelContact.AutoSize = true;
            this.labelContact.Location = new System.Drawing.Point(172, 21);
            this.labelContact.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelContact.Name = "labelContact";
            this.labelContact.Size = new System.Drawing.Size(260, 22);
            this.labelContact.TabIndex = 0;
            this.labelContact.Text = "Contact: 995928339@qq.com";
            this.labelContact.DoubleClick += new System.EventHandler(this.labelContact_DoubleClick);
            // 
            // labelGithub
            // 
            this.labelGithub.AutoSize = true;
            this.labelGithub.Location = new System.Drawing.Point(60, 61);
            this.labelGithub.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelGithub.Name = "labelGithub";
            this.labelGithub.Size = new System.Drawing.Size(510, 22);
            this.labelGithub.TabIndex = 1;
            this.labelGithub.Text = "Github: https://github.com/dreamlive0815/PCRHelper";
            this.labelGithub.DoubleClick += new System.EventHandler(this.labelGithub_DoubleClick);
            // 
            // FrmAbout
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(633, 104);
            this.Controls.Add(this.labelGithub);
            this.Controls.Add(this.labelContact);
            this.Font = new System.Drawing.Font("Consolas", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmAbout";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FrmAbout";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelContact;
        private System.Windows.Forms.Label labelGithub;
    }
}