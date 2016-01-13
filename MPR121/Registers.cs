using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Windows.IoT.Core.HWInterfaces.MPR121
{
    public class Registers
    {
        public const byte MPR121_SOFTRESET = 0x80;
        public const byte MPR121_TOUCHSTATUS_L = 0x00;
        public const byte MPR121_TOUCHSTATUS_H = 0x01;
        public const byte MPR121_FILTDATA_0L = 0x04;
        public const byte MPR121_FILTDATA_0H = 0x05;
        public const byte MPR121_BASELINE_0 = 0x1E;
        public const byte MPR121_MHDR = 0x2B;
        public const byte MPR121_NHDR = 0x2C;
        public const byte MPR121_NCLR = 0x2D;
        public const byte MPR121_FDLR = 0x2E;
        public const byte MPR121_MHDF = 0x2F;
        public const byte MPR121_NHDF = 0x30;
        public const byte MPR121_NCLF = 0x31;
        public const byte MPR121_FDLF = 0x32;
        public const byte MPR121_NHDT = 0x33;
        public const byte MPR121_NCLT = 0x34;
        public const byte MPR121_FDLT = 0x35;

        public const byte MPR121_TOUCHTH_0 = 0x41;
        public const byte MPR121_RELEASETH_0 = 0x42;
        public const byte MPR121_DEBOUNCE = 0x5B;
        public const byte MPR121_CONFIG1 = 0x5C;
        public const byte MPR121_CONFIG2 = 0x5D;
        public const byte MPR121_CHARGECURR_0 = 0x5F;
        public const byte MPR121_CHARGETIME_1 = 0x6C;
        public const byte MPR121_ECR = 0x5E;
        public const byte MPR121_AUTOCONFIG0 = 0x7B;
        public const byte MPR121_AUTOCONFIG1 = 0x7C;
        public const byte MPR121_UPLIMIT = 0x7D;
        public const byte MPR121_LOWLIMIT = 0x7E;
        public const byte MPR121_TARGETLIMIT = 0x7F;

        public const byte MPR121_GPIODIR = 0x76;
        public const byte MPR121_GPIOEN = 0x77;
        public const byte MPR121_GPIOSET = 0x78;
        public const byte MPR121_GPIOCLR = 0x79;
        public const byte MPR121_GPIOTOGGLE = 0x7A;

    }
}
