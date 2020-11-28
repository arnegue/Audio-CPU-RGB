using System;
using System.Threading;
using AudioCPURGB.RGB_Output;
using OpenHardwareMonitor.Hardware;
using System.Windows.Controls;
using AudioCPURGB.RGB_Creator;
using AudioCPURGB;

namespace AudioCPURGB
{
    class Stroboscope : RGB_Creator_Interface
    {
        RGB_Value _empty;
        RGB_Value _white;
        int half_freq;

        public Stroboscope()
        {
            half_freq = 0;
            _empty = new RGB_Value();
            _white = new RGB_Value
            {
                r = 255,
                g = 255,
                b = 255
            };
        }
        
        protected override void callback()
        {
            if (half_freq != 0)
            {
                // Thats only half the truth since showRGB takes time, which should result in less sleep time
                _rgbOutput.ShowRGB(_white);
                Thread.Sleep(half_freq);
                _rgbOutput.ShowRGB(_empty);
                Thread.Sleep(half_freq);
            }
        }

        public void setFrequency(int frequency)
        {
            if (frequency == 0)
            {
                half_freq = 0;
            }
            else
            {
                // Calculate half the sleep time in ms
                half_freq = Math.Abs((int)(float)((1.0 / (float)frequency * 1000.0) / 2.0));
            }
        }
    }
}
