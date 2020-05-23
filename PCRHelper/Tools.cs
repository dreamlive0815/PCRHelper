using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace PCRHelper
{
    class Tools
    {
        /// <summary>
        /// 获取Mumu模拟器进程
        /// </summary>
        /// <returns></returns>
        public Process GetMumuProcess()
        {
            var processes = Process.GetProcesses();
            foreach (var process  in processes)
            {
                var procName = process.ProcessName;
                if (procName.Contains("Nemu"))
                {

                }
            }
            return null;
        }
    }
}
