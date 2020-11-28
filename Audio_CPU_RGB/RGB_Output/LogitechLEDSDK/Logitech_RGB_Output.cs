using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using LedCSharp;

namespace AudioCPURGB.RGB_Output.LogitechLEDSDK
{
    class Logitech_RGB_Output : RGB_Output_Interface
    {
        private bool _enabled;
        private string name = "Logitech G502";
        private LogitechGSDK sdk;
        private const double rgb_to_perc = 100.0 / 255.0;
        public void Fade(RGB_Value oldValue, RGB_Value newValue, int fade_time_ms)
        {
            ShowRGB(newValue);
        }

        public void Fade(RGB_Value[] oldValues, RGB_Value[] newValues, int fade_time_ms)
        {
            ShowRGB(newValues[0]);
        }

        public int GetAmountRGBs()
        {
            return 1;
        }

        public string[] GetAvailableOutputList()
        {
            string[] str_list = { name }; // Half the truth
            return str_list;
        }

        public string GetName()
        {
            return name;
        }

        public void Initialize(string output)
        {
            sdk = new LogitechGSDK(); // not really sure if needed

            if (!LogitechGSDK.LogiLedInitWithName(output))
            {
                Console.WriteLine("LogiLedInit() failed.");
                return;
            }

            Thread.Sleep(2);

            int major = 0, minor = 0, build = 0;
            if (!LogitechGSDK.LogiLedGetSdkVersion(ref major, ref minor, ref build))
            {
                throw new Exception("Could not retrieve SDK version");
            }
            else
            {
                Console.WriteLine("Major: " + major + " , Minor: " + minor + "  Build: " + build);
            }

            Thread.Sleep(2);

            if (!LogitechGSDK.LogiLedSetTargetDevice(LogitechGSDK.LOGI_DEVICETYPE_ALL))
            {
                throw new Exception("Did not work to set LogiLedSetTargetDevice");
            }
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
            // Logitech works in percent (0-100), usual rgb is 0-255
            
            double r = (double)rgb.r * rgb_to_perc;
            double g = (double)rgb.g * rgb_to_perc;
            double b = (double)rgb.b * rgb_to_perc;


            if (!LogitechGSDK.LogiLedSetLighting((int)r, (int)g, (int)b))
            {
                Console.WriteLine("Did not work to set LogiLedSetLighting");
            }
        }

        public void ShowRGBs(RGB_Value[] rgbs)
        {
            ShowRGB(rgbs[0]);
        }

        public void Shutdown()
        {
            // TODO
        }
    }
}
