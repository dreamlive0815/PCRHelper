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
    }
}