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

namespace PCRHelper
{
    public partial class Frm : Form
    {

        Tools tools = Tools.GetInstance();
        readonly string imgName = "1.png";

        public Frm()
        {
            InitializeComponent();
        }

        private void Frm_Load(object sender, EventArgs e)
        {
            ConfigMgr.GetInstance().Init();

            ThreadingTimer timer = null;
            timer = new System.Threading.Timer(new TimerCallback((arg) =>
            {
                CaptureWindow();
                timer.Dispose();
                ProcessImage();
            }), null, 2000, Timeout.Infinite);
        }

        void CaptureWindow()
        {
            var storePath = Tools.GetInstance().JoinPath(ConfigMgr.GetInstance().CacheDir, imgName);
            var img = MumuState.Create().CaptureJJCNameRect(1);
            img.Save(storePath, ImageFormat.Png);
        }

        void ProcessImage()
        {
            //GraphicsTools.GetInstance().ToBinary(imgPath, 50);
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
    }
}