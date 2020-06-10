using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCRHelper.Scripts
{
    class ActivityStageExchangeScript : ScriptBase
    {
        public override string Name
        {
            get { return "ActivityStageExchangeScript"; }
        }

        public override int Interval
        {
            get { return 3500; }
            set { base.Interval = value; }
        }

        public override void OnStart(Bitmap viewportCapture, RECT viewportRect)
        {
            
        }

        public override void Tick(Bitmap viewportCapture, RECT viewportRect)
        {
            MumuState.ClickActStageExchange(viewportRect);
        }
    }
}
