using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Threading;

namespace AudioCPURGB.RGB_Output.Serial
{

    class Serial_RGB_Output : RGB_Output_Interface
    {
        private SerialPort _port;
        private bool _enabled;
        private int _rgbs;
        public String _name = "";
        private Mutex _ser_mutex;

        public Serial_RGB_Output()
        {
            _ser_mutex = new Mutex();
            _enabled = false;
        }

        public void initialize(String output)
        {
            _name = output;
            _port = new SerialPort(output);
             _port.BaudRate = 115200; 
          // _port.BaudRate = 500000;
            _port.StopBits = StopBits.One;
            _port.Parity = Parity.None;
            _port.DataBits = 8;
            _port.DtrEnable = true;
            _rgbs = 0;

            _ser_mutex.WaitOne();
            _port.Open();      
            _port.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
            _enabled = false;
            _ser_mutex.ReleaseMutex();

            // Wait till COM-port returns Amount of LEDS
            while (_rgbs == 0)
            {
                Thread.Sleep(10);
            }
            if(_rgbs == 0)
            {
                // TODO: ERROR
            }
        }

        public String getName()
        {
            return _name;
        }

        public int getAmountRGBs()
        {
            return _rgbs;
        }

        public void showRGBs(RGB_Value[] rgbs)
        {
            _ser_mutex.WaitOne();
            if (_enabled)
            {
                String sendToSerial = "(";
                for (int i = 0; i < rgbs.Length; i++)
                {
                    char r, g, b;
                    RGB_Value rgb = rgbs[i];
                    if (rgb == null)
                    {
                        r = '\0';
                        g = '\0';
                        b = '\0';
                    }
                    else
                    {
                        r = System.Convert.ToChar(rgb.r);
                        g = System.Convert.ToChar(rgb.g);
                        b = System.Convert.ToChar(rgb.b);
                    }
                    
                    sendToSerial += r;
                    sendToSerial += g;
                    sendToSerial += b;
                }
                sendToSerial += ")";

               /* if (sendToSerial.Length == 128 && sendToSerial[0] == '(' && sendToSerial[127] == ')')
                {
                    try
                    {*/
                        _port.Write(sendToSerial);
                    /*}
                    catch (Exception e)
                    {
                        // System.Diagnostics.Debug.Print("Hallo Welt\n!");
                        // ignore problems sending

                    }
                }*/
            }
            _ser_mutex.ReleaseMutex();
        }

        public void showRGB(RGB_Value rgb)
        {
            _ser_mutex.WaitOne();
            if (_enabled)
            {
                byte[] bytes = { System.Convert.ToByte('('), rgb.r, System.Convert.ToByte(','), rgb.g, System.Convert.ToByte(','), rgb.b, System.Convert.ToByte(')') };
                try
                {
                    _port.Write(bytes, 0, bytes.Length);
                    /// TODO only allow sending if serial returned an acknowledge
                }
                catch (Exception)
                {
                   // System.Diagnostics.Debug.Print("Exception\n!");
                    // ignore problems sending
                }
            }
            _ser_mutex.ReleaseMutex();
        }

        public void shutdown()
        {
            _ser_mutex.WaitOne();
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
            System.Diagnostics.Debug.Write("Data Received: ");
            System.Diagnostics.Debug.WriteLine(indata);
            //  indata += _port.R ReadExisting();
            if (_rgbs == 0) {
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
            /*if (_rgbs == 0 && /*indata[i] == '(' && indata[i +1] == ')')
            {
                _rgbs = indata[1];
                System.Diagnostics.Debug.Print("RGBS: " + _rgbs);
            }*/
        }

        public string[] getAvailableOutputList()
        {
            string[] portNames = SerialPort.GetPortNames();
            Array.Sort(portNames, StringComparer.InvariantCulture);
            return portNames;
        }

        public void setEnable(bool enable)
        {
            _ser_mutex.WaitOne();
            _enabled = enable;
            _ser_mutex.ReleaseMutex();
        }

        public bool isEnabled()
        {
            bool en;
            _ser_mutex.WaitOne();
             en = _enabled;
            _ser_mutex.ReleaseMutex();
            return en;
        }

        public void fade(RGB_Value oldValue, RGB_Value newValue, int fade_time_ms=50)
        {
            RGB_Value lastRGB = new RGB_Value();
            lastRGB.copy_values(oldValue);
            
            int rFactor = 1;
            int gFactor = 1;
            int bFactor = 1;

            // Look if decrement or increment
            if (lastRGB.r > newValue.r)
            {
                rFactor = -1;
            }
            if (lastRGB.g > newValue.g)
            {
                gFactor = -1;
            }
            if (lastRGB.b > newValue.b)
            {
                bFactor = -1;
            }

            showRGB(lastRGB);
            while (!lastRGB.Equals(newValue))
            {
                if (lastRGB.r != newValue.r)
                {
                    lastRGB.r += (byte)rFactor;
                }
                if (lastRGB.g != newValue.g)
                {
                    lastRGB.g += (byte)gFactor;
                }
                if (lastRGB.b != newValue.b)
                {
                    lastRGB.b += (byte)bFactor;
                }

                showRGB(lastRGB);

                // Wait a few Millisec to fade to new Color
                Thread.Sleep(fade_time_ms);
            }
        }
    }
}
