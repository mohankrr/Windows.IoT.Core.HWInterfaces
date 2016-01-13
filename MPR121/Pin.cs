using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Windows.IoT.Core.HWInterfaces.MPR121
{
    public class Pin
    {
        private PinId __pinId = PinId.None;
        private PinType __pinType = PinType.CapSense;
        private int __pinNumber = 0;
        private bool __isTouched = false;
        private int __baseLineValue = 0;
        private int __filteredValue = 0;


        public PinId PinId { get { return this.__pinId; } }
        public int PinNumber { get { return this.__pinNumber; } }

        public bool IsTouched { get { return this.__isTouched; } }
        
        /// <summary>
        /// Type of the Pin. Can either be CapSense or GPIO.. Library currently supports only CapSense
        /// </summary>
        public PinType PinType { get { return this.__pinType; } }

        public int BaselineValue { get { return this.__baseLineValue; } }

        public int FilteredValue { get { return this.__filteredValue; } }


        private Pin(){}

        public Pin(PinId pinId, PinType pinType)
        {
            this.__pinId = pinId;
            this.__pinType = pinType;
            this.__pinNumber = Array.IndexOf(Enum.GetValues(typeof(PinId)), pinId)-1;
        }     


        internal void SetTouched(bool status)
        {
            this.__isTouched = status;
        }


    }
}
