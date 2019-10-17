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
        private Thread _workerThread;
        private ManualResetEvent _pauseEvent = new ManualResetEvent(false);
        private RGB_Output_Interface _rgbOutput;
        RGB_Value _empty; 
        RGB_Value _white;
        int _freq;

        public Stroboscope()
        {
            _freq = 0;
            _empty = new RGB_Value();
            _white = new RGB_Value();
            _white.r = 255;
            _white.g = 255;
            _white.b = 255;
            // Create a new Thread
            _workerThread = new Thread(cpuTempThread);
            _workerThread.IsBackground = true;

            

            _pauseEvent.Reset(); // Don't let the thread run
            _workerThread.Start(); // But start it (until it comes to the pauseEvent)         
        }


        public void setRGBOutput(RGB_Output_Interface rgbOutput)
        {
            _rgbOutput = rgbOutput;
        }

        public void start()
        {
            _pauseEvent.Set();
        }

        public void pause()
        {
            _pauseEvent.Reset();
        }

  
        private void cpuTempThread()
        {
            while (true)
            {
                _pauseEvent.WaitOne();
                if (_freq != 0)
                {
                    try
                    {
                        _rgbOutput.showRGB(_white);
                        Thread.Sleep(_freq);
                        _rgbOutput.showRGB(_empty);
                        Thread.Sleep(_freq);
                    }
                    catch (System.Threading.Tasks.TaskCanceledException)
                    {
                        // Don't do any
                        break;
                    }
                }
            }
        }

        public void setFrequency(int freq)
        {
            _freq = freq;
        }
    }
}
