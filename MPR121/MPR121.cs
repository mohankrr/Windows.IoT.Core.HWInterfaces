using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using Windows.Devices.I2c;

namespace Windows.IoT.Core.HWInterfaces.MPR121
{
    public class MPR121:IDisposable
    {
        private const byte MPR121_I2CADDR_DEFAULT = 0x5A;
        private const int IRQ_HOOKUPPIN_DEFAULT = 5;
        private List<Pin> __pins =null;
        private byte __i2cAddress;
        private I2cDevice __connection=null;
        private int __mpr121IRQHookupPIN = 5; //connected to 5 by default. Can be set to any other GPIO pin by user
        private GpioPin __mpr122IRQpin;

        public List<Pin> Pins { get { return this.__pins; } }

        #region "Constructors"
        public MPR121():this(MPR121_I2CADDR_DEFAULT, IRQ_HOOKUPPIN_DEFAULT){}

        public MPR121(byte mprAddress, int mprIRQHookupPin)
        {
            __i2cAddress = mprAddress;
            __mpr121IRQHookupPIN = mprIRQHookupPin;

            __initPins();
        }

        /// <summary>
        /// default init as Capacitive sense electrodes
        /// </summary>
        private void __initPins()
        {
            __pins = new List<Pin>();

            foreach(PinId pinId in Enum.GetValues(typeof(PinId)))
            {
                if (pinId != PinId.None)
                {
                    __pins.Add(new Pin(pinId, PinType.CapSense));
                }
            }
        }

        #endregion "Constructors"

        public async Task<bool> OpenConnection(string i2cMasterId)
        {

            //Establish I2C connection
            __connection = await I2cDevice.FromIdAsync(i2cMasterId, new I2cConnectionSettings(this.__i2cAddress));
            
            // soft reset
            I2cTransferResult result = __connection.WritePartial(new byte[] { Registers.MPR121_SOFTRESET, 0x63 });

            if (result.Status == I2cTransferStatus.SlaveAddressNotAcknowledged)
            {
                throw new Exception(string.Format("MPR121 at address {0} not responding.", this.__i2cAddress));
            }

            await Task.Delay(1);

            writeRegister(Registers.MPR121_ECR, 0x0);

            byte c = readRegister8(Registers.MPR121_CONFIG2);

            if (c != 0x24) return false;


            SetThresholds(12, 6);

            //Section A Registers - Ref: AN3944, MPR121 Quick Start Guide
            //This group of setting controls the filtering of the system when the data is greater than the baseline. 
            //Settings from Adafruit Libary.. Most probably callibrated for the Adafruit's MPR121 breakout board.
            writeRegister(Registers.MPR121_MHDR, 0x01);
            writeRegister(Registers.MPR121_NHDR, 0x01);
            writeRegister(Registers.MPR121_NCLR, 0x0E);
            writeRegister(Registers.MPR121_FDLR, 0x00);

            //Section B Registers - Ref: AN3944, MPR121 Quick Start Guide
            writeRegister(Registers.MPR121_MHDF, 0x01);
            writeRegister(Registers.MPR121_NHDF, 0x05);
            writeRegister(Registers.MPR121_NCLF, 0x01);
            writeRegister(Registers.MPR121_FDLF, 0x00);

            writeRegister(Registers.MPR121_NHDT, 0x00);
            writeRegister(Registers.MPR121_NCLT, 0x00);
            writeRegister(Registers.MPR121_FDLT, 0x00);

            writeRegister(Registers.MPR121_DEBOUNCE, 0);
            writeRegister(Registers.MPR121_CONFIG1, 0x10); // default, 16uA charge current
            writeRegister(Registers.MPR121_CONFIG2, 0x20); // 0.5uS encoding, 1ms period

            writeRegister(Registers.MPR121_ECR, 0x8F);  // start with first 5 bits of baseline tracking

            InitMPR121TouchInterrupt();

            return true;
        }

        
        private void InitMPR121TouchInterrupt()
        {
            GpioController gpio = GpioController.GetDefault();

            __mpr122IRQpin = gpio.OpenPin(__mpr121IRQHookupPIN);

            __mpr122IRQpin.SetDriveMode(GpioPinDriveMode.Input); //Adafruit IRQ already has a pull up. When MPR121 detects touch it pulls IRQ low.

            __mpr122IRQpin.ValueChanged += Mpr122IRQpin_ValueChanged; //hook up the interrupt event.
        }

        PinId currentReading = PinId.None; 
        PinId lastReading = PinId.None;

        private void Mpr122IRQpin_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs args)
        {
            if (args.Edge == GpioPinEdge.FallingEdge)
            {
                //read the touch status register and convert it in to CapsenseElectrode Enum
                int rawTouchRegisterData = readTouchStatusRegister();
                currentReading = (PinId)rawTouchRegisterData;

                List<PinId> allTouchedPins = new List<PinId>();
                List<PinId> allReleasedPins = new List<PinId>();

                foreach (PinId val in Enum.GetValues(typeof(PinId))) //loop through all CapSense Pins
                {
                    if (val != PinId.None)
                    {
                        if (currentReading.HasFlag(val) && !lastReading.HasFlag(val))
                        {
                            allTouchedPins.Add(val);
                            this.__pins.Find(p => p.PinId == val).SetTouched(true);
                        }

                        if (!currentReading.HasFlag(val) && lastReading.HasFlag(val))
                        {
                            allReleasedPins.Add(val);
                            this.__pins.Find(p => p.PinId == val).SetTouched(false);
                        }
                    }
                }

                allTouchedPins.TrimExcess();
                allReleasedPins.TrimExcess();
                if (allTouchedPins.Count > 0)
                {
                    PinTouchedEventArgs touchedData = new PinTouchedEventArgs(allTouchedPins);
                    OnPinTouched(touchedData);
                }

                if (allReleasedPins.Count>0)
                {
                    PinReleasedEventArgs releasedData = new PinReleasedEventArgs(allReleasedPins);
                    OnPinReleased(releasedData);
                }

                lastReading = currentReading;
            }
        }

        private int readTouchStatusRegister()
        {
            ushort touched = readRegister16(Registers.MPR121_TOUCHSTATUS_L);
            return touched & 0x0FFF;
        }



        private void writeRegister(byte reg, byte value)
        {
            __connection.Write(new byte[] { reg, value });
        }


        private byte readRegister8(byte reg)
        {
            byte[] i2CReadBuffer = new byte[1];
            __connection.WriteRead(new byte[] { reg }, i2CReadBuffer);

            return i2CReadBuffer[0];
        }
        private ushort readRegister16(byte reg)
        {
            byte[] i2cReadBuffer = new byte[2];

            __connection.WriteRead(new byte[] { reg }, i2cReadBuffer);

            //turn two bytes in to short
            return (ushort)(i2cReadBuffer[0] + (i2cReadBuffer[1] << 8));
        }

        public void SetThresholds(byte touch, byte release)
        {
            for (byte i = 0; i < 12; i++)
            {
                writeRegister((byte)(Registers.MPR121_TOUCHTH_0 + 2 * i), touch);
                writeRegister((byte)(Registers.MPR121_RELEASETH_0 + 2 * i), release);
            }
        }

        #region "Events"
        public event EventHandler<PinTouchedEventArgs> PinTouched;
        public event EventHandler<PinReleasedEventArgs> PinReleased;
        protected virtual void OnPinTouched(PinTouchedEventArgs touchedEventArgs)
        {
            if (PinTouched != null) { PinTouched(this, touchedEventArgs); }
        }

        protected virtual void OnPinReleased(PinReleasedEventArgs releasedEventArgs)
        {
            if (PinReleased != null) { PinReleased(this, releasedEventArgs); }
        }

        public void Dispose()
        {
            __connection.Dispose();
        }
        #endregion "Events"

    }
}
