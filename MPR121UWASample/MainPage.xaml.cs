using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Enumeration;
using Windows.Devices.I2c;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.IoT.Core.HWInterfaces.MPR121;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MPR121UWASample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        private MPR121 __mpr121 = null;

        UIElement[] pinStatusUIElements = null;
        public MainPage()
        {
            this.InitializeComponent();

            //uses the default mpr121 address and Pin #5 on the RaspberryPi as IRQ Pin
            __mpr121 = new MPR121();

            //            this.pinList.DataContext = this.__mpr121;
            //this.pinList.ItemsSource = this.__mpr121.Pins;
            //this.pinList.UpdateLayout();

            pinStatusUIElements = new UIElement[] { pin0Status,pin1Status, pin2Status, pin3Status,
                                                    pin4Status, pin5Status,pin6Status, pin7Status,
                                                    pin8Status,pin9Status, pin10Status, pin11Status };

            this.__initMPR121();
        }


        private async void __initMPR121()
        {
            //Get the I2C device list on the Raspberry Pi.
            string aqs = I2cDevice.GetDeviceSelector(); //get the device selector AQS  (adavanced query string)
            var i2cDeviceList = await DeviceInformation.FindAllAsync(aqs); //get the I2C devices that match the device selector aqs

            //if the device list is not null, try to establish I2C connection between the master and the MPR121
            if (i2cDeviceList != null && i2cDeviceList.Count > 0)
            {
                bool connected = await __mpr121.OpenConnection(i2cDeviceList[0].Id);
                if (connected)
                {
                    this.txtStatus.Text = "Connected..";
                    //MPR121 will raise Touched and Released events if the IRQ pin is connected and configured corectly.. 
                    //Adding event handlers for those events
                    __mpr121.PinTouched += __mpr121_PinTouched;
                    __mpr121.PinReleased += __mpr121_PinReleased; ;
                }
            }

        }

        private void __mpr121_PinTouched(object sender, PinTouchedEventArgs e)
        {
            
            var task=Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                this.txtStatus.Text = e.Touched[0].ToString() + " Touched"; //just the first touched pin

                __updatePinStatusUI(e.Touched, true);
            });
            
        }

        private void __mpr121_PinReleased(object sender, PinReleasedEventArgs e)
        {
            var task = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                
                this.txtStatus.Text = e.Released[0].ToString() + " Released"; //just the first released pin

                __updatePinStatusUI(e.Released,false);
            });
        }


        private void __updatePinStatusUI(List<PinId> pins, bool turnOn)
        {
            SolidColorBrush pinBrush = new SolidColorBrush(Windows.UI.Colors.Red) ;
            if (turnOn)
                pinBrush = new SolidColorBrush(Color.FromArgb(100, 38, 247, 5));

            foreach (PinId pin in pins)
            {
                (pinStatusUIElements[(Array.IndexOf(Enum.GetValues(typeof(PinId)), pin) - 1)] as Ellipse).Fill = pinBrush;
            }


        }
    }
}
