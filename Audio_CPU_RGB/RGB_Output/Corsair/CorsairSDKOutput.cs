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

        public int GetAmountRGBs()
        {
            if (_leds == null)
            {
                _leds = _headset.GetLeds();
            }
            return _leds.Count();
        }

        public string GetName()
        {
            return name;
        }

        public void Initialize()
        {
            if (_headset == null)
            {
                CueSDK.Initialize();
                _headset = CueSDK.HeadsetSDK;
                GetAmountRGBs();
            }
            if (_headset == null)
            {
                throw new WrapperException("Something went wrong. No headset?");
            }
            Console.WriteLine(_headset.DeviceInfo.Model);
            Console.WriteLine(_headset.DeviceInfo.Type.ToString());
            Console.WriteLine(_headset.DeviceInfo.CapsMask.ToString());
        }

        public bool IsEnabled()
        {
            return _enabled;
        }

        public void SetEnable(bool enable)
        {
            _enabled = enable;
        }

        public void ShowRGB(RGB_Value rgb)
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

        public void ShowRGBs(RGB_Value[] rgbs)
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

        public void Shutdown()
        {
        }
    }
}
