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

namespace PCRHelper
{
    public partial class Frm : Form
    {

        Tools tools = Tools.GetInstance();
        GraphicsTools graphicsTools = GraphicsTools.GetInstance();
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
                DoCapture();
                timer.Dispose();
            }), null, 2000, Timeout.Infinite);
        }

        void DoCapture()
        {
            var state = MumuState.Create();
            var viewportRect = state.ViewportRect;
            var viewportCapture = state.DoCapture(viewportRect);
            for (int i = 0; i < 3; i++)
            {
                var nameCR = state.GetJJCNameCaptureRect(viewportCapture, viewportRect, i);
                graphicsTools.ShowImage("JJCNameCaptureRect" + i, nameCR);
                var rankCR = state.GetJJCRankCaptureRect(viewportCapture, viewportRect, i);
                graphicsTools.ShowImage("JJCRankCaptureRect" + i, rankCR);
            }
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
