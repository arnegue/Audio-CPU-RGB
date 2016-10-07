using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using OpenHardwareMonitor.Hardware;

namespace AudioSpectrum {
    class CPU_Temperature {
        private static readonly CPU_Temperature instance = new CPU_Temperature(1000);

        public static CPU_Temperature getInstance() {
            return instance;
        }

        private volatile float cpuTemp;
        private int ms_sleepInterval;

        private CPU_Temperature(int ms_sleepInterval) {
            this.ms_sleepInterval = ms_sleepInterval;
            cpuTemp = 0;

            Thread workerThread = new Thread(cpuTempThread);
            workerThread.IsBackground = true;
            workerThread.Start();
        }

        private void cpuTempThread() {
            while (true) {
                try {
                    cpuTemp = thread_getCPUTemp();
                    Thread.Sleep(ms_sleepInterval);
                }
                catch (System.Threading.Tasks.TaskCanceledException) {
                    // Don't do any
                    break;
                }
            }
        }

        private float thread_getCPUTemp() {
            float value = 0;
            Computer myComputer = new Computer();
            myComputer.Open();
            myComputer.CPUEnabled = true;
            foreach (var hardware in myComputer.Hardware) {
                hardware.Update();
                foreach (var sensor in hardware.Sensors) {
                    if (sensor.SensorType == SensorType.Temperature && sensor.Name.Equals("CPU Package")) {
                        return sensor.Value.GetValueOrDefault();
                    }
                    else {
                        // I think it maybe return then the temperature of the last iterated  cpu-core
                        value = sensor.Value.GetValueOrDefault();
                    }
                }
            }
            // Return 0 if no sensor in such way was found
            return value;
        }

        public float getCPUTemperature() {
            return cpuTemp;
        }
    }
}
