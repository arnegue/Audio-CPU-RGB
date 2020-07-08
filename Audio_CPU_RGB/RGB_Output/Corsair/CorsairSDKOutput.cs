using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CUE.NET;
using CUE.NET.Brushes;
using CUE.NET.Devices.Generic;
using CUE.NET.Devices.Headset;
using CUE.NET.Exceptions;

namespace AudioCPURGB.RGB_Output.Corsair
{
    class CorsairSDKOutput : RGB_Output_Interface
    {
        private string name = "Corsair";
        private CorsairHeadset _headset;
        private IEnumerable<CorsairLed> _leds;
        private bool _enabled;
        private CorsairColor _lastColor;

        public void fade(RGB_Value oldValue, RGB_Value newValue, int fade_time_ms)
        {
            showRGB(newValue);
        }

        public void fade(RGB_Value[] oldValues, RGB_Value[] newValues, int fade_time_ms)
        {
            showRGB(newValues[0]);
        }

        public int getAmountRGBs()
        {
            if (_leds == null)
            {
                _leds = _headset.GetLeds();
            }
            return _leds.Count();
        }

        public string[] getAvailableOutputList()
        {
            string[] str_list = { name }; // Half the truth
            return str_list;
        }

        public string getName()
        {
            return name;
        }

        public void initialize(string output)
        {
            if (_headset == null)
            {
                CueSDK.Initialize();
                _headset = CueSDK.HeadsetSDK;
                getAmountRGBs();
            }
            if (_headset == null)
            {
                throw new WrapperException("Something went wrong. No headset?");
            }
            Console.WriteLine(_headset.DeviceInfo.Model);
            Console.WriteLine(_headset.DeviceInfo.Type.ToString());
            Console.WriteLine(_headset.DeviceInfo.CapsMask.ToString());
        }

        public bool isEnabled()
        {
            return _enabled;
        }

        public void setEnable(bool enable)
        {
            _enabled = enable;
        }

        public void showRGB(RGB_Value rgb)
        {
            if (_lastColor == null)
            {
                _lastColor = new CorsairColor(rgb.r, rgb.g, rgb.b);
                _headset.Brush = new SolidColorBrush(_lastColor);
            }
            else
            {
                // Don't need to instantiate it again and again
                _lastColor.R = rgb.r;
                _lastColor.G = rgb.g;
                _lastColor.B = rgb.b;
            }
            _headset.Update();
        }

        public void showRGBs(RGB_Value[] rgbs)
        {
            int i = 0;
            foreach (var led in _leds)
            {
                led.Color.R = rgbs[i].r;
                led.Color.G = rgbs[i].g;
                led.Color.B = rgbs[i].b;
                i++;
            }
            _headset.Update(true, true); 

        }

        public void shutdown()
        {
        }
    }
}
