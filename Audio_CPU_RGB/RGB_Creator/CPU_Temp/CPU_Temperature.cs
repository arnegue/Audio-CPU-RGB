using System;
using System.Threading;
using AudioCPURGB.RGB_Output;
using OpenHardwareMonitor.Hardware;
using System.Windows.Controls;
using AudioCPURGB.RGB_Creator;

namespace AudioCPURGB
{
    class CPU_Temperature_RGB_Creator : RGB_Creator_Interface
    {
        private TextBlock _temperatureTextBlock;

        private RGB_Value _lastRGB = new RGB_Value(); // last RGB_Value needed for fading
        private const float m = 12.75F; // Constant needed for Algo1
        private const int _ms_sleepInterval = 1000;

        private ISensor _cpuPackageSensor;
        private IHardware _cpuHardware;
        private Computer _myComputer;

        float[] temp_avg = new float[10];
        int avg_index = 0;

        public CPU_Temperature_RGB_Creator(TextBlock temperatureTextBlock)
        {
            _temperatureTextBlock = temperatureTextBlock;


            // Iterate throught the sensors, to get the CPU-Package-Sensor
            _myComputer = new Computer();
            _myComputer.Open();


            _myComputer.CPUEnabled = true;

            foreach (var hardware in _myComputer.Hardware)
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
            if (_cpuPackageSensor == null)
            {
                // TODO Popup warning
                System.Diagnostics.Debug.WriteLine("No \"CPU Package\" was found.");
            }
        }

        protected override void callback()
        {
            _cpuHardware.Update(); // Update Sensors
            float value = _cpuPackageSensor.Value ?? 0; // Get value from Sensor


            // Show temperature on GUI
            System.Diagnostics.Debug.WriteLine("CPU-Temp: " + value);
            String tempString = value + " °C";

            _temperatureTextBlock.Dispatcher.Invoke(new Action(() =>
            {
                _temperatureTextBlock.Text = tempString;
            }));

            // Calculate average to last values
            temp_avg[avg_index] = value;

            float avg = 0.0F;
            for (int i = 0; i < temp_avg.Length; i++)
            {
                avg += temp_avg[i];
            }
            avg /= temp_avg.Length;

            avg_index = (avg_index + 1) % temp_avg.Length;

            // Calculate value and fade into it
            RGB_Value newRgb = calculateRGBValueAlgo1(avg);

            Fade(_lastRGB, newRgb, 50);
            _lastRGB = newRgb;

            Thread.Sleep(_ms_sleepInterval);            
        }

        private RGB_Value calculateRGBValueAlgo1(float temp)
        {

            // Calculate Colours to temperature
            byte r, g, b;
            if (temp < 30)
            {
                r = 0;
                g = 0;
                b = 255;
            }
            else if (temp >= 30F && temp < 50F)
            {
                r = 0;
                g = (byte)(m * temp - 382.5F);
                b = (byte)(-m * temp + 637.5F);
            }
            else if (temp >= 50F && temp < 70F)
            {
                r = (byte)(m * temp - 637.5F);
                g = (byte)(-m * temp + 892.5F);
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
