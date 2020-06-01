﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using OpenCvSharp.Extensions;
using OpenCvSharp;

namespace PCRHelper
{
    static class RichTextBoxExtension
    {

        public static void ScrollToEnd(this RichTextBox richTextBox)
        {
            richTextBox.SelectionStart = richTextBox.TextLength;
            richTextBox.ScrollToCaret();
        }

        public static void AppendLineThreadSafe(this RichTextBox richTextBox, string s)
        {
            AppendLineThreadSafe(richTextBox, s, Color.Black);
        }

        public static void AppendLineThreadSafe(this RichTextBox richTextBox, string s, Color color)
        {
            AppendTextThreadSafe(richTextBox, s + Environment.NewLine, color);
        }

        public static void AppendTextThreadSafe(this RichTextBox richTextBox, string s)
        {
            AppendTextThreadSafe(richTextBox, s, Color.Black);
        }

        public static void AppendTextThreadSafe(this RichTextBox richTextBox, string s, Color color)
        {
            if (richTextBox == null)
            {
                return;
            }
            if (richTextBox.InvokeRequired)
            {
                if (richTextBox.IsDisposed) return;
                richTextBox.Invoke(new Action<RichTextBox, string, Color>(AppendTextThreadSafe), richTextBox, s, color);
            }
            else
            {
                richTextBox.SelectionColor = color;
                richTextBox.AppendText(s);
                richTextBox.ScrollToEnd();
            }
        }
    }

    static class OpenCvExtension
    {
        public static Mat ToOpenCvMat(this Bitmap bitmap)
        {
            return bitmap.ToMat();
        }

        public static Bitmap ToRawBitmap(this Mat mat)
        {
            return mat.ToBitmap();
        }

        public static Color GetPixel(this Mat mat, int r, int c)
        {
            var channels = mat.Channels();
            Color clr;
            if (mat.Channels() == 1)
            {
                var v = mat.Get<byte>(r, c);
                clr = Color.FromArgb(v, v, v);
            }
            else
            {
                var vec3b = mat.Get<Vec3b>(r, c);
                clr = Color.FromArgb(vec3b.Item0, vec3b.Item1, vec3b.Item2);
            }
            return clr;
        }

        public static void SetPixel(this Mat mat, int r, int c, params byte[] rbga)
        {
            var channels = mat.Channels();
            var getV = new Func<int, byte>((int index) =>
            {
                return index < rbga.Length ? rbga[index] : (byte)0;
            });
            if (mat.Channels() == 1)
            {
                var v = mat.Get<byte>(r, c);
                mat.Set(r, c, getV(0));
            }
            else
            {
                var vec3b = new Vec3b(getV(0), getV(1), getV(2));
                mat.Set(r, c, vec3b);
            }
        }
    }
}
