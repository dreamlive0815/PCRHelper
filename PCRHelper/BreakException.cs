using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCRHelper
{

    class BreakException : NoTrackTraceException
    {
        public BreakException(string msg) : base(msg)
        {
        }
    }

    class NoTrackTraceException : Exception
    {
        public NoTrackTraceException(string msg) : base(msg)
        {
        }
    }
}
