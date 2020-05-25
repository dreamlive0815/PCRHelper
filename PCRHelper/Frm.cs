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

namespace PCRHelper
{
    public partial class Frm : Form
    {

        Tools tools = Tools.GetInstance();

        public Frm()
        {
            InitializeComponent();
        }

        private void Frm_Load(object sender, EventArgs e)
        {

            ConfigMgr.GetInstance().Init();
            
            ThreadingTimer timer = null;
            timer = new System.Threading.Timer(new TimerCallback((arg) => {
                DoJob();
                timer.Dispose();
            }), null, 2000, Timeout.Infinite);
            //DoJob();

        }

        void DoJob()
        {
            var storePath = "1.png";
            var img = tools.CaptureMumuWindow();
            img.Save(storePath);
            tools.OpenFileInExplorer(storePath);
        }
    }
}
