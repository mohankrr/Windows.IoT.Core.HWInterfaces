using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Windows.IoT.Core.HWInterfaces.MPR121
{
    [Flags]
    public enum PinId
    {
        None = 0,
        PIN_0 = 1 << 0,
        PIN_1 = 1 << 1,
        PIN_2 = 1 << 2,
        PIN_3 = 1 << 3,
        PIN_4 = 1 << 4,
        PIN_5 = 1 << 5,
        PIN_6 = 1 << 6,
        PIN_7 = 1 << 7,
        PIN_8 = 1 << 8,
        PIN_9 = 1 << 9,
        PIN_10 = 1 << 10,
        PIN_11 = 1 << 11
    }
}

