using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace AudioCPURGB.RGB_Output.Serial
{

    class Serial_RGB_Output : RGB_Output_Interface
    {
        private SerialPort _port;
        
        public void initialize(String output)
        {
            _port = new SerialPort(output);
            _port.BaudRate = 115200; //
            _port.StopBits = StopBits.One;
            _port.Parity = Parity.None;
            _port.DataBits = 8;
            _port.DtrEnable = true;
            _port.Open();

            _port.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
        }

        public void showRGB(RGB_Value rgb)
        {
            String sendToSerial = "(" + System.Convert.ToChar(rgb.r) + "," + System.Convert.ToChar(rgb.g) + "," + System.Convert.ToChar(rgb.b) + ")";
            _port.Write(sendToSerial);
        }

        public void shutdown()
        {
            _port.Close();
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
        }

        public string[] getAvailableOutputList() {
           return SerialPort.GetPortNames();
        }
    }
}
