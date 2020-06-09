using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCRHelper.Scripts
{
    abstract class ScriptBase
    {

        public void SetMumuState(MumuState mumuState)
        {
            MumuState = mumuState;
        }

        protected MumuState MumuState { get; set; }

        /// <summary>
        /// 单位：毫秒
        /// </summary>
        public virtual int Interval { get; set; } = 2000;

        public abstract string Name { get; }

        public abstract void OnStart();

        public abstract void Tick(Bitmap viewportCapture, RECT viewportRect);
    }
}
