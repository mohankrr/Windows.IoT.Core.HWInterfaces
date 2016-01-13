using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Windows.IoT.Core.HWInterfaces.MPR121
{
    public class PinReleasedEventArgs:EventArgs
    {
        private List<PinId> __released = null;

        public List<PinId> Released { get { return this.__released; } }
        public PinReleasedEventArgs(List<PinId> releasedElectrodes)
        {
            this.__released = releasedElectrodes;
        }

    }
}
