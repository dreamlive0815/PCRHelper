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

        public override void OnStart()
        {
            
        }

        public override void Tick(Bitmap viewportCapture, RECT viewportRect)
        {
            MumuState.ClickActStageExchange(viewportRect);
        }
    }
}
