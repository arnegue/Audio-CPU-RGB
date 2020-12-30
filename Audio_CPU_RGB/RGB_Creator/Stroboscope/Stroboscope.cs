using System;
using System.Threading;
using AudioCPURGB.RGBOutput;
using OpenHardwareMonitor.Hardware;
using System.Windows.Controls;
using AudioCPURGB.RGBCreator;
using AudioCPURGB;

namespace AudioCPURGB
{
    /// <summary>
    /// Flashes and resets white RGB-Value
    /// </summary>
    class Stroboscope : SingleRGBCreator
    {
        RGBValue _white;
        int half_freq;

        public Stroboscope()
        {
            half_freq = 0;
            _white = new RGBValue(255, 255, 255);
        }
        
        protected override void Callback()
        {
            if (half_freq != 0)
            {
                // Thats only half the truth since showRGB takes time, which should result in less sleep time
                _rgbOutput.ShowRGB(_white);
                Thread.Sleep(half_freq);
                _rgbOutput.ShowRGB(lastRGB_);
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
