using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PCRHelper
{
    class LogTools
    {
        private static LogTools instance;

        public static LogTools GetInstance()
        {
            if (instance == null)
            {
                instance = new LogTools();
            }
            return instance;
        }

        private LogTools()
        {

        }

        private RichTextBox richText;

        public void SetRichTextBox(RichTextBox richTextBox)
        {
            richText = richTextBox;
        }

        void AppendLine(string s)
        {
            Append(string.Format("{0}{1}", s, Environment.NewLine));
        }

        void Append(string s)
        {
            var txt = richText;
            if (txt == null)
            {
                return;
            }
            if (txt.InvokeRequired)
            {
                if (txt.IsDisposed) return;
                txt.Invoke(new Action<string>(Append), s);
            }
            else
            {
                txt.AppendText(s);

                txt.SelectionStart = txt.TextLength;
                txt.ScrollToCaret();
            }
        }

        public string FilterMsg(string msg)
        {
            msg = msg.Replace("\n", "");
            msg = msg.Replace("\r", "");
            msg = msg.Replace("\f", "");
            return msg;
        }

        public void Error(string msg)
        {
            msg = FilterMsg(msg);
            if (richText != null && !richText.IsDisposed)
            {
                AppendLine(msg);
            }

        }

        public void Info(string msg)
        {
            msg = FilterMsg(msg);
            if (richText != null && !richText.IsDisposed)
            {
                AppendLine(msg);
            }
        }
    }
}
