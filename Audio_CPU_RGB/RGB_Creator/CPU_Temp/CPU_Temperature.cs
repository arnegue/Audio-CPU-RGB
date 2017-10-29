using System;
using System.Threading;
using AudioCPURGB.RGB_Output;
using OpenHardwareMonitor.Hardware;

namespace AudioCPURGB.RGB_Creator
{
    class CPU_Temperature : RGB_Creator_Interface
    {
        private int _ms_sleepInterval;
        private Thread _workerThread;
        private ManualResetEvent _pauseEvent = new ManualResetEvent(false);
        private RGB_Output_Interface _rgbOutput;

        private RGB_Value _lastRGB; // last RGB_Value needed for fading
        private const float m = 12.75F; // Constant needed for Algo1
        private const int ms_sleepInterval = 1000;

        private ISensor _cpuPackageSensor;
        private IHardware _cpuHardware;

        public CPU_Temperature() {
            _lastRGB = new RGB_Value(0, 0, 0);
            _ms_sleepInterval = ms_sleepInterval;
            
            _workerThread = new Thread(cpuTempThread);
            _workerThread.IsBackground = true;

            _pauseEvent.Reset();
            _workerThread.Start();
            
            Computer myComputer = new Computer();
            myComputer.Open();

            myComputer.CPUEnabled = true;
            foreach (var hardware in myComputer.Hardware)
            {
                hardware.Update();
                foreach (var sensor in hardware.Sensors)
                {
                    if (sensor.SensorType == SensorType.Temperature && sensor.Name.Equals("CPU Package"))
                    {
                        _cpuPackageSensor = sensor;
                        _cpuHardware = hardware;
                    }
                }
            }
            if(_cpuPackageSensor == null)
            {
                // TODO Popup warning
                System.Diagnostics.Debug.WriteLine("No \"CPU Package\" was found.");
            }
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

        private void cpuTempThread() {
            while (true)
            {
                _pauseEvent.WaitOne();
                try
                {
                    _cpuHardware.Update(); // Update Sensors
                    float value = _cpuPackageSensor.Value ?? 0; // Get value from Sensor

                    System.Diagnostics.Debug.WriteLine("CPU-Temp: " + value);
                    RGB_Value newRgb = calculateRGBValueAlgo1(value);

                    // Now fade to that color
                    int rFactor = 1;
                    int gFactor = 1;
                    int bFactor = 1;

                    // Look if decrement or increment
                    if (_lastRGB.r > newRgb.r)
                    {
                        rFactor = -1;
                    }
                    if (_lastRGB.g > newRgb.g)
                    {
                        gFactor = -1;
                    }
                    if (_lastRGB.b > newRgb.b)
                    {
                        bFactor = -1;
                    }

                    int lastR = _lastRGB.r;
                    int lastG = _lastRGB.g;
                    int lastB = _lastRGB.b;

                    while (!_lastRGB.Equals(newRgb))
                    {
                        if (lastR != newRgb.r)
                        {
                            lastR += rFactor;
                        }
                        if (lastG != newRgb.g)
                        {
                            lastG += gFactor;
                        }
                        if (lastB != newRgb.b)
                        {
                            lastB += bFactor;
                        }

                        RGB_Value rgb = new RGB_Value(lastR, lastG, lastB);
                        if (_rgbOutput != null)
                        {
                            _rgbOutput.showRGB(rgb);
                        }
                        _lastRGB = rgb;
                        // Wait 10 Millisec to fade to new Color
                        Thread.Sleep(10);
                    }
                    
                    Thread.Sleep(_ms_sleepInterval);
                }
                catch (System.Threading.Tasks.TaskCanceledException) {
                    // Don't do any
                    break;
                }
            }
        }

             private RGB_Value calculateRGBValueAlgo1(float temp)
        {
          
            // Calculate Colours to temperature
            int r, g, b;
            if (temp < 30)
            {
                r = 0;
                g = 0;
                b = 255;
            }
            else if (temp >= 30F && temp < 50F)
            {
                r = 0;
                g = (int)(m * temp - 382.5F);
                b = (int)(-m * temp + 637.5F);
            }
            else if (temp >= 50F && temp < 70F)
            {
                r = (int)(m * temp - 637.5F);
                g = (int)(-m * temp + 892.5F);
                b = 0;

            }
            else //temp >= 70
            {
                r = 255;
                g = 0;
                b = 0;
            }
            return new RGB_Value(r, g, b);          
        }
    }
}
