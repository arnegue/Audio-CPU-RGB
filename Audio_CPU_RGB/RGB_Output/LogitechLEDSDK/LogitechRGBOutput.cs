using System;
using System.Threading;
using LedCSharp;

namespace AudioCPURGB.RGBOutput.LogitechLEDSDK
{
    class LogitechRGBOutput : IRGBOutput
    {
        private bool _enabled;
        private string name = "Logitech G502";
        private const double rgb_to_perc = 100.0 / 255.0;

        public int GetAmountRGBs()
        {
            return 1;
        }

        public string GetName()
        {
            return name;
        }

        public void Initialize()
        {

            if (!NativeMethods.LogiLedInitWithName(GetName()))
            {
                throw new RGBOutputException("LogiLedInit() failed.");
            }

            Thread.Sleep(2);

            int major = 0, minor = 0, build = 0;
            if (!NativeMethods.LogiLedGetSdkVersion(ref major, ref minor, ref build))
            {
                throw new Exception("Could not retrieve SDK version");
            }
            else
            {
                Console.WriteLine("Major: " + major + " , Minor: " + minor + "  Build: " + build);
            }

            Thread.Sleep(2);

            if (!NativeMethods.LogiLedSetTargetDevice(LogitechSDKConstants.LogiDevicetypeAll))
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

        public void ShowRGB(RGBValue rgb)
        {
            // Logitech works in percent (0-100), usual rgb is 0-255            
            double r = (double)rgb.R * rgb_to_perc;
            double g = (double)rgb.G * rgb_to_perc;
            double b = (double)rgb.B * rgb_to_perc;


            if (!NativeMethods.LogiLedSetLighting((int)r, (int)g, (int)b))
            {
                throw new RGBOutputException("Did not work to set LogiLedSetLighting");
            }
        }

        public void ShowRGBs(RGBValue[] rgbs)
        {
            ShowRGB(rgbs[0]);
        }

        public void Shutdown()
        {
            // TODO
        }
    }
}
