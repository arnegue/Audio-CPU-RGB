using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

        CPU_Temperature_RGB_Creator _cpuRgbCreator;
        Audio_RGB_Creator _audioRgbCreator;
        ScreenAnalyzer _screenAnalyzer;
        ColorChooser _colorChooser;
        Stroboscope _stroboscope;
        Rainbow _rainbow;
        RunningColorChangingDot _runningDot;
        

        Cyotek.Windows.Forms.ColorWheel colorWheel;

        public TabMainWindow()
        {
            try
            {
                InitializeComponent();
            }catch (Exception e)
            {
                System.Diagnostics.Debug.Print(e.ToString());
            }
            colorWheel = new Cyotek.Windows.Forms.ColorWheel();
            colorWheel.Color = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(64)))), ((int)(((byte)(48)))));
            colorWheel.Dock = System.Windows.Forms.DockStyle.Fill;
            colorWheel.Location = new System.Drawing.Point(0, 0);
            colorWheel.Name = "colorWheel";
            colorWheel.Size = new System.Drawing.Size(624, 224);
            colorWheel.TabIndex = 0;
            //this.colorWheel.ColorChanged += new System.EventHandler(this.colorWheel_ColorChanged);
            // colorGrid.Children.Add(colorWheel);

            _cpuRgbCreator = new CPU_Temperature_RGB_Creator(cpuTempTB);
            _audioRgbCreator = new Audio_RGB_Creator(BtnEnable, PbL, PbR, Spectrum, DeviceBox, AlgoChoice, SpectrumSlider, MinSlider);
            _screenAnalyzer = new ScreenAnalyzer();
            _colorChooser = new ColorChooser();
            _stroboscope = new Stroboscope();
            _rainbow = new Rainbow();
            _runningDot = new RunningColorChangingDot();
            


            _rgbOutputI = new RGB_Output.Serial.Serial_RGB_Output();

            var ports = _rgbOutputI.getAvailableOutputList();
            foreach (var port in ports)
            {
                Comports.Items.Add(port);
            }
            Comports.SelectedIndex = 0;

            xSkipper.Text = _screenAnalyzer.xSkipper.ToString();
            ySkipper.Text = _screenAnalyzer.ySkipper.ToString();
            xStart.Text = _screenAnalyzer.xStart.ToString();
            yStart.Text = _screenAnalyzer.yStart.ToString();
            xStop.Text = _screenAnalyzer.xStop.ToString();
            yStop.Text = _screenAnalyzer.yStop.ToString();

            Application.Current.MainWindow.Activate();
            Application.Current.MainWindow.Focus();
            Application.Current.MainWindow.ShowDialog();

            ///minSliderBindValue = 2;
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

                if (tabControl.SelectedIndex != 0)
                {
                    BtnEnable.Content = "Enable";
                    BtnEnable.IsChecked = false;
                    _audioRgbCreator.enableClick(false);
                }

                switch (tabControl.SelectedIndex)
                {
                    case 0: // Audio
                        _rgbCreatorI = _audioRgbCreator;
                        break;
                    case 1: // CPU-Temp
                        _rgbCreatorI = _cpuRgbCreator;
                        // TODO
                        break;
                    case 2: // Image
                        _rgbCreatorI = _screenAnalyzer;
                        break;
                    case 3: // Color-Chooser
                        _rgbCreatorI = _colorChooser;
                        break;
                    case 4: // Stroboscope
                        _rgbCreatorI = _stroboscope;
                        break;
                  //  case 5: // Misc
                    //    _

                }

                if (_rgbCreatorI != null && _rgbOutputI != null)
                {
                    //if(_rgbCreatorI == _audioRgbCreator && BtnEnable.IsChecked != true )
                    //  {
                    //     return;
                    // }
                    _rgbCreatorI.setRGBOutput(_rgbOutputI);
                    _rgbCreatorI.start(); // start new rgbCreator
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
                    _rgbOutputI.initialize(Comports.Items[Comports.SelectedIndex] as string);

                    _rgbOutputI.setEnable(true);
                }
                else
                {
                    _rgbOutputI.shutdown();

                    _rgbOutputI.setEnable(false);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }


        /// <summary>
        /// Method which is called when the comport-choser was opened
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Comports_DropDownOpened(object sender, EventArgs e)
        {
           String curComportName = Comports.Items[Comports.SelectedIndex] as String;

            Comports.Items.Clear();
            var ports = _rgbOutputI.getAvailableOutputList();
          //  ports.OrderBy("Foo asc");

            int newSelectedIndex = 0;
            for (int i = 0; i < ports.Length; i++) // foreach (var port in ports)
            {
                Comports.Items.Add(ports[i]);
                // Get new Index of selected COM port before (if a new port was added)
                if (ports[i] == curComportName)
                {
                    newSelectedIndex = i;
                }
            }
            Comports.SelectedIndex = newSelectedIndex;
        }


        private void Comports_DropDownClosed(object sender, EventArgs e)
        {
            if (this.IsLoaded)
            {
                if (CkbSerial.IsChecked == true  )
                {
                    _rgbOutputI.shutdown();
                    _rgbOutputI.setEnable(false);

                    _rgbOutputI.initialize(Comports.Items[Comports.SelectedIndex] as string);
                    _rgbOutputI.setEnable(true);
                }             
            }
        }


        /// ################################### Audio-GUI ################################### 

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
                _audioRgbCreator.enableClick(true);
                //_rgbCreatorI.start();

                // TODO _analyzer.Enable = true;
            }
            else
            {
                //  _audioRgbCreator.audioEnable = false;
                // TODO _analyzer.Enable = false;
                BtnEnable.Content = "Enable";
                _audioRgbCreator.enableClick(false);
                // _rgbCreatorI.pause();
            }
        }

        /// <summary>
        /// Method which is called when the Rel/Abs-Checkobx changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RelAbs_Click(object sender, RoutedEventArgs e)
        {
            if (RelAbs.IsChecked.Value == true)
            {
                _audioRgbCreator.absNotRel = false;
            }
            else
            {
                _audioRgbCreator.absNotRel = true;
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
            _audioRgbCreator.triggerValueChanged((int)slider.Value);
        }

        /// <summary>
        /// Method which is called when the Algorithm-Choser changed it's value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AlgoChoice_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AudioAlgorithm audioAlgo = (AudioAlgorithm)AlgoChoice.SelectedItem;
            _audioRgbCreator.activeAlgo = audioAlgo;
        }

        /// ################################### Screenshot-Controll ################################### 
        private void xSkipper_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.IsLoaded)
            {
                int val;
                if (Int32.TryParse(xSkipper.Text, out val))
                {
                    _screenAnalyzer.xSkipper = val;
                }
                xSkipper.Text = _screenAnalyzer.xSkipper.ToString();
            }
        }

        private void ySkipper_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.IsLoaded)
            {
                int val;
                if (Int32.TryParse(ySkipper.Text, out val))
                {
                    _screenAnalyzer.ySkipper = val;
                }
                ySkipper.Text = _screenAnalyzer.ySkipper.ToString();
            }
        }

        private void xStart_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.IsLoaded)
            {
                int val;
                if (Int32.TryParse(xStart.Text, out val))
                {
                    _screenAnalyzer.xStart = val;
                }
                xStart.Text = _screenAnalyzer.xStart.ToString();
            }
        }

        private void yStart_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.IsLoaded)
            {
                int val;
                if (Int32.TryParse(yStart.Text, out val))
                {
                    _screenAnalyzer.yStart = val;
                }
                yStart.Text = _screenAnalyzer.yStart.ToString();
            }
        }

        private void xStop_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.IsLoaded)
            {
                int val;
                if (Int32.TryParse(xStop.Text, out val))
                {
                    _screenAnalyzer.xStop = val;
                }
                xStop.Text = _screenAnalyzer.xStop.ToString();
            }
        }

        private void yStop_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.IsLoaded)
            {
                int val;
                if (Int32.TryParse(yStop.Text, out val))
                {
                    _screenAnalyzer.yStop = val;
                }
                yStop.Text = _screenAnalyzer.yStop.ToString();
            }
        }

        private void ClrPicker_Changed(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            _colorChooser.rgbChanged(Clr_picker.SelectedColor.Value.R, Clr_picker.SelectedColor.Value.G, Clr_picker.SelectedColor.Value.B);
        }

        private void Frequency_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _stroboscope.setFrequency(100 - (int)Frequency.Value);
        }

        private void Emphaser_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _screenAnalyzer.setEmphaser((float)ColorEmphaser.Value);
        }

        private void Window_Activated(object sender, EventArgs e)
        {

            Application.Current.MainWindow.Activate();
        }
    }

}
