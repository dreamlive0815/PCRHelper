using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PCRHelper
{
    public partial class FrmGetRectRate : Form
    {
        public FrmGetRectRate()
        {
            InitializeComponent();
        }

        string imgPath;

        private void FrmGetRectRate_Load(object sender, EventArgs e)
        {
            imgPath = ChooseImage();
            LoadImage(imgPath);
            Show();
        }

        string ChooseImage()
        {
            var openDialog = new OpenFileDialog();
            openDialog.Title = "请选择图片";
            openDialog.FileName = "";
            openDialog.Filter = "*.png|*.png|*.jpg|*.jpg|*.jpeg|*.jpeg";
            if (openDialog.ShowDialog() != DialogResult.OK)
            {
                Close();
                return "";
            }
            return openDialog.FileName;
        }

        void LoadImage(string imgPath)
        {
            if (!File.Exists(imgPath))
            {
                return;
            }
            pictureBox1.Image = Image.FromFile(imgPath);
            Size = pictureBox1.Size + new Size(0, 50); 
        }

        bool press = false;
        int startX, startY;
        Rectangle rectangle;

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            press = true;
            startX = e.X;
            startY = e.Y;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            Text = $"{e.X},{e.Y}";
            if (!press)
            {
                return;
            }
            pictureBox1.Refresh();
            var g = pictureBox1.CreateGraphics();
            var pen = new Pen(Color.Red, 2);
            var x1 = Math.Min(startX, e.X);
            var y1 = Math.Min(startY, e.Y);
            var x2 = Math.Max(startX, e.X);
            var y2 = Math.Max(startY, e.Y);
            rectangle = new Rectangle(x1, y1, x2 - x1, y2 - y1);
            g.DrawRectangle(pen, rectangle);
        }

        string FormatFloat(double f)
        {
            return f.ToString("f4");
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            press = false;
            var width = pictureBox1.Image.Width;
            var height = pictureBox1.Image.Height;

            var rect = rectangle;
            var r1 = FormatFloat(1.0 * rectangle.X / width);
            var r2 = FormatFloat(1.0 * rectangle.Y / height);
            var r3 = FormatFloat(1.0 * (rectangle.X + rectangle.Width) / width);
            var r4 = FormatFloat(1.0 * (rectangle.Y + rectangle.Height) / height);

            var s = string.Format("new Vec4<float>({0}f, {1}f, {2}f, {3}f);", r1, r2, r3, r4);
            Clipboard.SetText(s);
        }
        
    }
}
