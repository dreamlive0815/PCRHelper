
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace PCRHelper
{
    class AdbTools
    {
        private static AdbTools instance;

        public static AdbTools GetInstance()
        {
            if (instance == null)
            {
                instance = new AdbTools();
            }
            return instance;
        }

        private AdbTools()
        {
            ConnectToMumu();
        }

        public string GetAdbServerExePath()
        {
            return ConfigMgr.GetInstance().AdbServerExePath;
        }

        public void ConnectToMumu()
        {
            var startInfo = new ProcessStartInfo()
            {
                FileName = GetAdbServerExePath(),
                Arguments = $"connect 127.0.0.1:7555",
                WindowStyle = ProcessWindowStyle.Hidden,
            };
            Process.Start(startInfo);
        }

        public void DoShell(string command)
        {
            var startInfo = new ProcessStartInfo()
            {
                FileName = GetAdbServerExePath(),
                Arguments = $"shell {command}",
                WindowStyle = ProcessWindowStyle.Hidden,
            };
            Process.Start(startInfo);
        }

        public void DoTap(Point point)
        {
            var command = $"input tap {point.X} {point.Y}";
            DoShell(command);
        }
    }
}
