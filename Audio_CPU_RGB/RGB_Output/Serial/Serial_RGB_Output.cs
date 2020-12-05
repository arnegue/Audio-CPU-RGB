using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Threading;

namespace AudioCPURGB.RGB_Output.Serial

{
    public class SerialFactory {
        Dictionary<String, Serial_RGB_Output> _curent_outputs;  // Map with COM-Portnames as Key and it's Serial_RGB_Output as instance

        public SerialFactory()
        {
            _curent_outputs = new Dictionary<string, Serial_RGB_Output>();
            GetAvailableOutputList();
        }

        /// <summary>
        /// Searches for every COM-Port and if it is not already added to _current_outputs, appends it to it
        /// </summary>
        /// <returns></returns>
        public List<Serial_RGB_Output> GetAvailableOutputList()
        {
            string[] portNames = SerialPort.GetPortNames();
            
            foreach (var portName in portNames)
            {
                if (!(_curent_outputs.ContainsKey(portName))) {
                    _curent_outputs[portName] = new Serial_RGB_Output(portName);
                }
            }
            // TODO old items? How about ports which got removed inbetween?

            List<Serial_RGB_Output> returnItems = new List<Serial_RGB_Output>();
            returnItems.AddRange(_curent_outputs.Values);
            return returnItems;
        }
    }

    public class Serial_RGB_Output : RGB_Output_Interface, IDisposable
    {
        private SerialPort _port;
        private bool _enabled;
        private int _rgbs;
        public String _name = "";
        private Mutex _ser_mutex;

        public Serial_RGB_Output(String port)
        {
            _ser_mutex = new Mutex();
            _enabled = false;
            _name = port;
        }

        public void Initialize()
        {
            _port = new SerialPort(_name)
            {
                BaudRate = 115200,
                StopBits = StopBits.One,
                Parity = Parity.None,
                DataBits = 8,
                DtrEnable = true
            };
            _rgbs = 0;

            _ser_mutex.WaitOne();
            _port.Open();
            _port.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
            _enabled = false;
            _ser_mutex.ReleaseMutex();

            // Wait till COM-port returns Amount of LEDS for 5 seconds
            int max_time_waiting_us = 5 * 1000;
            int time_waited_us = 0;
            int sleep_time_us = 10;
            while (_rgbs == 0)
            {
                Thread.Sleep(sleep_time_us);
                time_waited_us += sleep_time_us;
                if (time_waited_us >= max_time_waiting_us)
                {
                    Shutdown();
                    throw new Exception(String.Format("TimeOut ({0:0.##} seconds) waiting for replay on {1}", time_waited_us / (1000 * 1000), _name));
                }
            }
        }

        public String GetName()
        {
            return _name;
        }

        public int GetAmountRGBs()
        {
            return _rgbs;
        }

        public void ShowRGBs(RGB_Value[] rgbs)
        {
            _ser_mutex.WaitOne();
            if (_enabled)
            {
                byte[] bytes = new byte[(rgbs.Length * 3) + 2];
                bytes[0] = System.Convert.ToByte('(');
                RGB_Value rgb;
                byte r, g, b;

                int byte_index = 1;  // first byte is (

                for (int i = 0; i < rgbs.Length; i++)
                {
                    rgb = rgbs[i];
                    if (rgb == null)
                    {
                        r = 0;
                        g = 0;
                        b = 0;
                    }
                    else
                    {
                        r = System.Convert.ToByte(rgb.r);
                        g = System.Convert.ToByte(rgb.g);
                        b = System.Convert.ToByte(rgb.b);
                    }

                    bytes[byte_index] = r;
                    bytes[byte_index + 1] = g;
                    bytes[byte_index + 2] = b;
                    byte_index += 3;
                }

                bytes[bytes.Length - 1] = System.Convert.ToByte(')');

                _port.Write(bytes, 0, bytes.Length);
            }
            _ser_mutex.ReleaseMutex();
        }

        public void ShowRGB(RGB_Value rgb)
        {
            _ser_mutex.WaitOne();
            if (_enabled)
            {
                byte[] bytes = { System.Convert.ToByte('('), rgb.r, System.Convert.ToByte(','), rgb.g, System.Convert.ToByte(','), rgb.b, System.Convert.ToByte(')') };
                
                _port.Write(bytes, 0, bytes.Length);              
            }
            _ser_mutex.ReleaseMutex();
        }

        public void Shutdown()
        {
            _ser_mutex.WaitOne();
            _enabled = false;
            _port.Close();
            _ser_mutex.ReleaseMutex();
        }

        /// <summary>
        /// Small handler which receives data from serial (Only necessary for debugging)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            string indata = sp.ReadExisting();
            System.Diagnostics.Debug.WriteLine(indata);
            if (_rgbs == 0)
            {
                for (int i = 0; i < indata.Length; i++)
                {
                    if (i + 2 < indata.Length)
                    {
                        if (indata[i] == '(' && indata[i + 2] == ')')
                        {
                            _rgbs = indata[i + 1];
                            System.Diagnostics.Debug.Print("RGB-Serial received: " + _rgbs);
                            break;
                        }
                    }
                }
            }
        }

        public void SetEnable(bool enable)
        {
            _ser_mutex.WaitOne();
            _enabled = enable;
            _ser_mutex.ReleaseMutex();
        }

        public bool IsEnabled()
        {
            bool en;
            _ser_mutex.WaitOne();
            en = _enabled;
            _ser_mutex.ReleaseMutex();
            return en;
        }

        ~Serial_RGB_Output()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _ser_mutex.WaitOne();
                _ser_mutex.ReleaseMutex();
                _ser_mutex.Dispose();
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
