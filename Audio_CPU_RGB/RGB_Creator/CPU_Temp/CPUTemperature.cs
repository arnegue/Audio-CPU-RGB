﻿using System;
using System.Threading;
using OpenHardwareMonitor.Hardware;
using System.Windows.Controls;
using AudioCPURGB.RGBCreator;
using System.Security.Principal;

namespace AudioCPURGB
{
    class CPUTemperatureRGBCreator : SingleRGBCreator
    {
        private TextBlock _temperatureTextBlock;

        private const float m = 12.75F; // Constant needed for Algo1
        private const int _ms_sleepInterval = 1000;

        private ISensor _cpuPackageSensor;
        private IHardware _cpuHardware;
        private Computer _myComputer;

        private float[] temp_avg = new float[10];
        private int avg_index;

        public CPUTemperatureRGBCreator(TextBlock temperatureTextBlock)
        {
            if (IsAdministrator()) {
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
                    System.Diagnostics.Debug.WriteLine("No \"CPU Package\" was found.");
                }
            }
        }

        /// <summary>
        /// Taken from https://stackoverflow.com/a/3600338
        /// </summary>
        private static bool IsAdministrator()
        {
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }

        public override void Start()
        {
            if (_cpuHardware == null || _cpuPackageSensor == null)
            {
                throw new InitializationException("No CPU-Temperature sensor was found.");
            }
            else if (IsAdministrator())
            {
                base.Start();
            } else
            {
                throw new InitializationException("CPU-Temperature needs to be started with administrator privilegies. Restart this program as admin.");
            }
        }

        protected override void Callback()
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
            RGBValue newRgb = calculateRGBValueAlgo1(avg);

            Fade(lastRGB_, newRgb, 50);
            lastRGB_ = newRgb;

            Thread.Sleep(_ms_sleepInterval);            
        }

        static private RGBValue calculateRGBValueAlgo1(float temp)
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
            return new RGBValue(r, g, b);
        }    
    }
}
