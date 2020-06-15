using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCRHelper
{

    class BreakException : Exception
    {
        public BreakException(string msg) : base(msg)
        {
        }
    }
}
