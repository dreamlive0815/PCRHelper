using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PCRHelper
{
    public partial class FrmAbout : Form
    {
        public FrmAbout()
        {
            InitializeComponent();
        }

        private void labelContact_DoubleClick(object sender, EventArgs e)
        {
            Clipboard.SetText("995928339@qq.com");
            MessageBox.Show("已复制");
        }

        private void labelGithub_DoubleClick(object sender, EventArgs e)
        {
            Clipboard.SetText("https://github.com/dreamlive0815/PCRHelper");
            MessageBox.Show("已复制");
        }
    }
}
