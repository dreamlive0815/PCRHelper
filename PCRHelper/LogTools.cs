using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

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
            AppendIntoFile("./error.log", msg);
            richText?.AppendLineThreadSafe(msg, Color.Red);
        }

        public void Info(string msg)
        {
            richText?.AppendLineThreadSafe(msg, Color.Black);
        }

        private void AppendIntoFile(string filePath, string s)
        {
            using (var file = new FileStream(filePath, FileMode.Append))
            {
                using (var writer = new StreamWriter(file))
                {
                    var time = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss:ffff");
                    writer.WriteLine($"[{time}] {s}");
                }
            }
        }
    }
}
