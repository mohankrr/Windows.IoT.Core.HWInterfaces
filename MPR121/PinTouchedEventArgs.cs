using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Windows.IoT.Core.HWInterfaces.MPR121
{
    public class PinTouchedEventArgs:EventArgs
    {
        private List<PinId> __touched = null;

        public List<PinId> Touched { get { return this.__touched; } }
        public PinTouchedEventArgs(List<PinId> touchedElectrodes)
        {
            this.__touched = touchedElectrodes;
        }
    }
}
