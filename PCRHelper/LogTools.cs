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
            Error(msg, true);
        }

        public void Error(string msg, bool writeIntoFile)
        {
            richText?.AppendLineThreadSafe(msg, Color.Red);
            if (writeIntoFile)
            {
                AppendIntoFile(ConfigMgr.GetInstance().ErrorLogPath, msg);
            }
        }

        public void Error(Exception ex)
        {
            var msg = ex.InnerException?.Message ?? ex.Message;
            richText?.AppendLineThreadSafe(msg, Color.Red);
            if (IsSelfOrChildrenNoTrackTraceException(ex))
            {
                return;
            }
            AppendIntoFile(ConfigMgr.GetInstance().ErrorLogPath, msg);
        }

        public void Info(string msg)
        {
            richText?.AppendLineThreadSafe(msg, Color.Black);
        }

        public void AppendIntoFile(string filePath, string s)
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

        public bool IsSelfOrChildrenBreakException(Exception e)
        {
            if (e is BreakException) return true;
            if (e.InnerException is BreakException) return true;
            return false;
        }

        public bool IsSelfOrChildrenNoTrackTraceException(Exception e)
        {
            if (e is NoTrackTraceException) return true;
            if (e.InnerException is NoTrackTraceException) return true;
            return false;
        }
    }
}
