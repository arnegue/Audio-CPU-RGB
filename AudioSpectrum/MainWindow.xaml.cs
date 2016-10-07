using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO.Ports;
using System.Threading;

namespace AudioSpectrum {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private Analyzer _analyzer;
        private SerialPort _port;

        public MainWindow() {
            InitializeComponent();
            cput = CPU_Temperature.getInstance();

            // Some stupid workaround...
            MyRangeSlider mrs = (MyRangeSlider)FindName("RangeSlider");

            _analyzer = new Analyzer(PbL, PbR, Spectrum, DeviceBox, mrs);

            Thread displayTempThread = new Thread(displayCPUTemp);

            displayTempThread.IsBackground = true;
            displayTempThread.Start();

        }

        private void displayCPUTemp() {
            CPU_Temperature cput = CPU_Temperature.getInstance();
            while (true) {
                float temp = cput.getCPUTemperature();
                if (_analyzer.Serial != null && _analyzer.cpuNotAudio) {
                    showCPUTempToRGB(temp);
                }
                try {
                    this.Dispatcher.Invoke(new Action(() => {
                        cpuTemp.Text = temp + " °";
                    }));
                }
                catch (System.Threading.Tasks.TaskCanceledException) {
                    // Don't do anything, usually happens when closing this Program
                    break;
                }
                Thread.Sleep(1000);
            }
        }

        private void BtnEnable_Click(object sender, RoutedEventArgs e) {
            if (BtnEnable.IsChecked == true) {
                BtnEnable.Content = "Disable";
                _analyzer.Enable = true;
            }
            else {
                _analyzer.Enable = false;
                BtnEnable.Content = "Enable";
                DeviceBox.IsEnabled = true;
            }
        }

        private void Comports_DropDownOpened(object sender, EventArgs e) {
            Comports.Items.Clear();
            var ports = SerialPort.GetPortNames();
            foreach (var port in ports) Comports.Items.Add(port);
        }

        private static void DataReceivedHandler(
                    object sender,
                    SerialDataReceivedEventArgs e) {
            SerialPort sp = (SerialPort)sender;
            string indata = sp.ReadExisting();
            System.Diagnostics.Debug.Write("Data Received: ");
            System.Diagnostics.Debug.WriteLine(indata);
        }

        private void CkbSerial_Click(object sender, RoutedEventArgs e) {
            try {
                if (CkbSerial.IsChecked == true) {
                    Comports.IsEnabled = false;
                    _port = new SerialPort((Comports.Items[Comports.SelectedIndex] as string));
                    _port.BaudRate = 9600;
                    _port.StopBits = StopBits.One;
                    _port.Parity = Parity.None;
                    _port.DataBits = 8;
                    _port.DtrEnable = true;
                    _port.Open();
                    _analyzer.Serial = _port;
                    _port.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
                }
                else {
                    Comports.IsEnabled = true;
                    _analyzer.Serial = null;
                    if (_port != null) {
                        _port.Close();
                        _port.Dispose();
                        _port = null;
                    }
                }
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e) {
            // ... Get RadioButton reference.
            var button = sender as RadioButton;


            if (button.Content != null) {
                String radioButtonName = button.Content.ToString();
                if (radioButtonName.Equals("CPU-Temperature")) {
                    _analyzer.cpuNotAudio = true;
                }
                else if (radioButtonName.Equals("Audio-Display")) {
                    _analyzer.cpuNotAudio = false;
                }
                else {
                    System.Diagnostics.Debug.WriteLine("Weird radiobutton: " + radioButtonName);
                }
            }
        }

        private const double m = 12.75;
        RGBValue lastRGB = new RGBValue(0, 0, 0);
        private CPU_Temperature cput;

        private void showCPUTempToRGB(float temp) {
            // Calculate Colours to temperature
            double r, g, b;
            if (temp < 30) {
                r = 0.0;
                g = 0.0;
                b = 255;
            }
            else if (temp >= 30 && temp < 50) {
                r = 0;
                g = m * temp - 382.5;
                b = -m * temp + 637.5;
            }
            else if (temp >= 50 && temp < 70) {
                r = m * temp - 637.5;
                g = -m * temp + 892.5;
                b = 0.0;

            }
            else //temp >= 70
            {
                r = 255;
                g = 0;
                b = 0;
            }

            RGBValue newRGB = new RGBValue((int)r, (int)g, (int)b);

            // Now fade to that color
            int rFactor = 1;
            int gFactor = 1;
            int bFactor = 1;

            // Look if decrement or increment
            if (lastRGB.r > newRGB.r) {
                rFactor = -1;
            }
            if (lastRGB.g > newRGB.g) {
                gFactor = -1;
            }
            if (lastRGB.b > newRGB.b) {
                bFactor = -1;
            }

            int lastR = lastRGB.r;
            int lastG = lastRGB.g;
            int lastB = lastRGB.b;

            while (!lastRGB.Equals(newRGB)) {
                if (lastR != newRGB.r) {
                    lastR += rFactor;
                }
                if (lastG != newRGB.g) {
                    lastG += gFactor;
                }
                if (lastB != newRGB.b) {
                    lastB += bFactor;
                }

                RGBValue rgb = new RGBValue(lastR, lastG, lastB);

                try {
                    _analyzer.Serial.WriteLine(rgb.ToString());
                    System.Diagnostics.Debug.WriteLine(rgb.ToString());
                }
                catch (System.NullReferenceException e) {
                    // Seems normal when switching on/off
                }
                lastRGB = rgb;
            }
        }
    }
}
