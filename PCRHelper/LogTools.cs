using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

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

        public void Error(string msg)
        {
            richText?.AppendLineThreadSafe(msg, Color.Red);
        }

        public void Info(string msg)
        {
            richText?.AppendLineThreadSafe(msg, Color.Black);
        }
    }
}
