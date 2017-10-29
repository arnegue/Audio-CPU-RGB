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
using System.Windows.Shapes;

namespace AudioCPURGB
{
    /// <summary>
    /// Interaktionslogik für Window1.xaml
    /// </summary>
    public partial class TabMainWindow : Window
    {
        RGB_Output.RGB_Output_Interface _rgbOutputI;
        RGB_Creator.RGB_Creator_Interface _rgbCreatorI;

        RGB_Creator.CPU_Temperature _cpuRgbCreator;
        
        public TabMainWindow()
        {
            InitializeComponent();
            
            _cpuRgbCreator = new RGB_Creator.CPU_Temperature();
            // _rgbCreator = RGB_Creator. // TODO: Audioalgo is first algo
        }


        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TabControl tabControl = sender as TabControl;
            if (e.Source is TabControl) //if this event fired from TabControl then enter
            {
                if (_rgbCreatorI != null)
                {
                    _rgbCreatorI.pause(); // pause current rgbCreator
                }
                
                switch (tabControl.SelectedIndex) {
                    case 0: // Audio
                        // Todo
                        break;
                    case 1: // CPU-Temp
                        _rgbCreatorI = _cpuRgbCreator;
                        System.Diagnostics.Debug.WriteLine("Changed tab to: CPU-Temperature");
                        // TODO
                        break;
                    case 2: // Image
                        // TODO
                        break;
                    case 3: // Color-Chooser
                        // TODO
                        break;
                }

                if (_rgbCreatorI != null)
                {
                    _rgbCreatorI.start(); // pause current rgbCreator
                }
            }
        }

        /// ################################### Serial-Controll ################################### 

        /// <summary>
        /// Method which is called when checkbox for serial enable is called
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CkbSerial_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CkbSerial.IsChecked == true)
                {
                    _rgbOutputI = new RGB_Output.Serial.Serial_RGB_Output();
                    _rgbOutputI.initialize(Comports.Items[Comports.SelectedIndex] as string);
                }
                else
                {
                    _rgbOutputI.shutdown();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }


        /// <summary>
        /// Method which is called when the enable-button was clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnEnable_Click(object sender, RoutedEventArgs e)
        {
            if (BtnEnable.IsChecked == true)
            {
                BtnEnable.Content = "Disable";
                // TODO _analyzer.Enable = true;
            }
            else
            {
                // TODO _analyzer.Enable = false;
                BtnEnable.Content = "Enable";
                DeviceBox.IsEnabled = true;
            }
        }

        /// <summary>
        /// Method which is called when the comport-choser changed it's value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Comports_DropDownOpened(object sender, EventArgs e)
        {
            Comports.Items.Clear();
            // TODO var ports = SerialPort.GetPortNames();
            // TODO foreach (var port in ports) Comports.Items.Add(port);
        }


        /// ################################### Audio-GUI ################################### 

        /// <summary>
        /// Method which is called when the Rel/Abs-Checkobx changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RelAbs_Click(object sender, RoutedEventArgs e)
        {
            if (RelAbs.IsChecked.Value == true)
            {
               // TODO _analyzer.absNotRel = false;
            }
            else
            {
                // TODO _analyzer.absNotRel = true;
            }
        }

        /// <summary>
        /// Method which is called when the Trigger-Slider changed it's value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MinSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider slider = sender as Slider;
            // TODO_analyzer.minSliderValue = (int)slider.Value;
        }

        /// <summary>
        /// Method which is called when the Algorithm-Choser changed it's value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AlgoChoice_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AudioAlgorithm audioAlgo = (AudioAlgorithm)AlgoChoice.SelectedItem;
            // TODO_analyzer.activeAlgo = audioAlgo;
        }
    }
}

