using OpenCvSharp;
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
using RawSize = System.Drawing.Size;

namespace PCRHelper
{
    public partial class FrmGetRectRate : Form
    {
        public FrmGetRectRate()
        {
            InitializeComponent();
        }

        private void FrmGetRectRate_Load(object sender, EventArgs e)
        {
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

        public void LoadImage(Image img)
        {
            pictureBox1.Image = img;
            Size = pictureBox1.Size + new RawSize(0, 50);
        }

        public void LoadImage(string imgPath)
        {
            if (!File.Exists(imgPath))
            {
                return;
            }
            LoadImage(Image.FromFile(imgPath));
        }

        bool press = false;
        int startX, startY;
        Rectangle rectangle;

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            rectangle = new Rectangle();
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

        private void FrmGetRectRate_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.S)
            {
                var pwid = pictureBox1.Width;
                var phei = pictureBox1.Height;
                var mat = new Bitmap(pictureBox1.Image).ToOpenCvMat();
                var rectangle = e.Alt ? new Rectangle(0, 0, pictureBox1.Width, pictureBox1.Height) : this.rectangle;
                if (rectangle.Width == 0 || rectangle.Height == 0)
                {
                    return;
                }
                var r1 = 1.0f * rectangle.X / pwid;
                var r2 = 1.0f * rectangle.Y / phei;
                var r3 = 1.0f * (rectangle.X + rectangle.Width) / pwid;
                var r4 = 1.0f * (rectangle.Y + rectangle.Height) / phei;
                var rectRate = new Vec4f(r1, r2, r3, r4);
                var childMat = mat.GetChildMatByRectRate(rectRate);
                var saveDialog = new SaveFileDialog();
                saveDialog.Title = "选择图片保存路径(" + (e.Alt ? "完整" : "部分");
                saveDialog.Filter = "*.png|*.png";
                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    childMat.SaveImage(saveDialog.FileName);
                }
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            press = false;
            var width = pictureBox1.Width;
            var height = pictureBox1.Height;

            var rect = rectangle;
            if (rect.Width > 5 && rect.Height > 5)
            {
                var rx1 = 1.0 * rectangle.X / width; var r1 = FormatFloat(rx1);
                var ry1 = 1.0 * rectangle.Y / height; var r2 = FormatFloat(ry1);
                var rx2 = 1.0 * (rectangle.X + rectangle.Width) / width; var r3 = FormatFloat(rx2);
                var ry2 = 1.0 * (rectangle.Y + rectangle.Height) / height; var r4 = FormatFloat(ry2);
                var s = string.Format("new Vec4f({0}f, {1}f, {2}f, {3}f)", r1, r2, r3, r4);
                Clipboard.SetText(s);
                Text = s;
            }
            else
            {
                var midrx = 1.0 * e.X / width; var r5 = FormatFloat(midrx);
                var midry = 1.0 * e.Y / height; var r6 = FormatFloat(midry);
                var s = string.Format("new Vec2f({0}f, {1}f)", r5, r6);
                Clipboard.SetText(s);
                Text = s;
            }
        }
        
    }
}
