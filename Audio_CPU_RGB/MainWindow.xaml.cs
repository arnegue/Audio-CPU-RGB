﻿using System;
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

/**
 * Attention: A very big part of this code is copied from: http://www.codeproject.com/Articles/797537/Making-an-Audio-Spectrum-analyzer-with-Bass-dll-Cs
 * I only extended this code by my needs and do not claim that it's mine. I just wanted to contribute my code to other with similar ideas! 
 */
namespace AudioCPURGB {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private Analyzer _analyzer;
        private SerialPort _port;

        public MainWindow() {
            InitializeComponent();
            cput = CPU_Temperature.getInstance();
            _analyzer = new Analyzer(PbL, PbR, Spectrum, DeviceBox, AlgoChoice, SpectrumSlider);

            Thread displayTempThread = new Thread(displayCPUTemp);

            displayTempThread.IsBackground = true;
            displayTempThread.Start();

        }

        /// <summary>
        /// Thread which permanently checks the cpu-temperature and shows it to gui and to serial if it's checkbox is checked
        /// </summary>
        private void displayCPUTemp() {
            CPU_Temperature cput = CPU_Temperature.getInstance();
            while (true) {
                float temp = cput.getCPUTemperature();
                if (_analyzer._serial != null && _analyzer.cpuNotAudio) {
                    showCPUTempToRGB(temp);
                }
                try {
                    System.Diagnostics.Debug.WriteLine("Try to show new CPU-Temp: " + temp);
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

        /// <summary>
        /// Method which is called when the enable-button was clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Method which is called when the comport-choser changed it's value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Comports_DropDownOpened(object sender, EventArgs e) {
            Comports.Items.Clear();
            var ports = SerialPort.GetPortNames();
            foreach (var port in ports) Comports.Items.Add(port);
        }

        /// <summary>
        /// Small handler which receives data from serial
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void DataReceivedHandler(
                    object sender,
                    SerialDataReceivedEventArgs e) {
            SerialPort sp = (SerialPort)sender;
            string indata = sp.ReadExisting();
            System.Diagnostics.Debug.Write("Data Received: ");
            System.Diagnostics.Debug.WriteLine(indata);
        }

        /// <summary>
        /// Method which is called when checkbox for serial enable is called
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CkbSerial_Click(object sender, RoutedEventArgs e) {
            try {
                if (CkbSerial.IsChecked == true) {
                    Comports.IsEnabled = false;
                    _port = new SerialPort((Comports.Items[Comports.SelectedIndex] as string));
                    _port.BaudRate = 115200; //
                    _port.StopBits = StopBits.One;
                    _port.Parity = Parity.None;
                    _port.DataBits = 8;
                    _port.DtrEnable = true;
                    _port.Open();
                    _analyzer._serial = _port;
                    _port.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
                }
                else {
                    Comports.IsEnabled = true;
                    _analyzer._serial = null;
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

        /// <summary>
        /// Method which is called when the radioButton for chosing between CPU-Temperature and Audio changed it's value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        private const float m = 12.75F;
        RGBValue lastRGB = new RGBValue(0, 0, 0);
        private CPU_Temperature cput;

        /// <summary>
        /// Method which calculates the RGB-Values on given temperature and afterwards sends it to serial
        /// </summary>
        /// <param name="temp">CPU-Temperature in Celsius</param>
        private void showCPUTempToRGB(float temp) {
            // Calculate Colours to temperature
            int r, g, b;
            if (temp < 30) {
                r = 0;
                g = 0;
                b = 255;
            }
            else if (temp >= 30F && temp < 50F) {
                r = 0;
                g = (int)(m * temp - 382.5F);
                b = (int)(-m * temp + 637.5F);
            }
            else if (temp >= 50F && temp < 70F) {
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

            // Now fade to that color
            int rFactor = 1;
            int gFactor = 1;
            int bFactor = 1;

            // Look if decrement or increment
            if (lastRGB.r > r) {
                rFactor = -1;
            }
            if (lastRGB.g > g) {
                gFactor = -1;
            }
            if (lastRGB.b > b) {
                bFactor = -1;
            }

            int lastR = lastRGB.r;
            int lastG = lastRGB.g;
            int lastB = lastRGB.b;

            RGBValue newRGB = new RGBValue(r, g, b);
            while (!lastRGB.Equals(newRGB) && _analyzer.cpuNotAudio && _analyzer.Enable == true) {
                // TODO. Falscher algorithmus irgendwie wieder!
                if (lastR != r) {
                    lastR += rFactor;
                }
                if (lastG != g) {
                    lastG += gFactor;
                }
                if (lastB != b) {
                    lastB += bFactor;
                }

                RGBValue rgb = new RGBValue(lastR, lastG, lastB);

                try {
                    _analyzer._serial.WriteLine(rgb.ToString());
                    System.Diagnostics.Debug.WriteLine("CPU-RGB: " + rgb.ToString());
                }
                catch (System.NullReferenceException) {
                    // Seems normal when switching on/off
                }
                lastRGB = rgb;
                Thread.Sleep(10);
            }
        }

        /// <summary>
        /// Method which is called when the Trigger-Slider changed it's value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MinSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            Slider slider = sender as Slider;
            _analyzer.minSliderValue = (int)slider.Value;
        }

        /// <summary>
        /// Method which is called when the Algorithm-Choser changed it's value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AlgoChoice_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            AudioAlgorithm audioAlgo = (AudioAlgorithm)AlgoChoice.SelectedItem;
            _analyzer.activeAlgo = audioAlgo;
        }

        /// <summary>
        /// Method which is called when the Rel/Abs-Checkobx changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RelAbs_Click(object sender, RoutedEventArgs e) {
            if (RelAbs.IsChecked.Value == true) {
                _analyzer.absNotRel = false;
            } else {
                _analyzer.absNotRel = true;
            }
        }
    }
}
