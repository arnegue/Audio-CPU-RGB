using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using AudioCPURGB.RGBCreator;
using AudioCPURGB.RGBOutput;

namespace AudioCPURGB
{
    /// <summary>
    /// Interaktionslogik für Window1.xaml
    /// </summary>
    public partial class TabMainWindow : Window, IDisposable
    {
        IRGBOutput _current_interface;
        RGBOutputManager _rgb_manager;

        RGBCreator.IRGBCreator _rgbCreatorI;
        RGBCreator.IRGBCreator _selectedMisc;

        CPUTemperatureRGBCreator _cpuRgbCreator;
        AudioRGBCreator _audioRgbCreator;
        ScreenAnalyzer _screenAnalyzer;
        ColorChooser _colorChooser;
        Stroboscope _stroboscope;
        Rainbow _rainbow;
        RunningColorChangingDot _runningDot;
        ColorChanger _colorchanger;
        RunningColors _runningColors;

        public TabMainWindow()
        {
            // Initialize these before gui... i know bad
            _rainbow = new Rainbow();
            _runningDot = new RunningColorChangingDot();

            InitializeComponent();    
           
            _rgb_manager = new RGBOutputManager();

            _audioRgbCreator = new AudioRGBCreator(BtnEnable, PbL, PbR, Spectrum, DeviceBox, AlgoChoice, SpectrumSlider, MinSlider);
            _cpuRgbCreator = new CPUTemperatureRGBCreator(cpuTempTB);            
            _screenAnalyzer = new ScreenAnalyzer();
            _colorChooser = new ColorChooser();
            _stroboscope = new Stroboscope();
            _colorchanger = new ColorChanger();
            _runningColors = new RunningColors();

            _cpuRgbCreator.Initialize();
            _audioRgbCreator.Initialize();
            _screenAnalyzer.Initialize();
            _colorChooser.Initialize();
            _stroboscope.Initialize();
            _colorchanger.Initialize();
            _runningColors.Initialize();
            _rainbow.Initialize();
            _runningDot.Initialize();

            FillRGBOutputList();

            xSkipper.Text = _screenAnalyzer.xSkipper.ToString(System.Globalization.CultureInfo.CurrentCulture);
            ySkipper.Text = _screenAnalyzer.ySkipper.ToString(System.Globalization.CultureInfo.CurrentCulture);
            xStart.Text = _screenAnalyzer.xStart.ToString(System.Globalization.CultureInfo.CurrentCulture);
            yStart.Text = _screenAnalyzer.yStart.ToString(System.Globalization.CultureInfo.CurrentCulture);
            xStop.Text = _screenAnalyzer.xStop.ToString(System.Globalization.CultureInfo.CurrentCulture);
            yStop.Text = _screenAnalyzer.yStop.ToString(System.Globalization.CultureInfo.CurrentCulture);

            Application.Current.MainWindow.Activate();
            Application.Current.MainWindow.Focus();
            Application.Current.MainWindow.ShowDialog();
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TabControl tabControl = sender as TabControl;
            if (e.Source is TabControl) //if this event fired from TabControl then enter
            {
                RGBCreator.IRGBCreator new_rgb_creator;


                if (tabControl.SelectedIndex != 0)
                {
                    BtnEnable.Content = "Enable";
                    BtnEnable.IsChecked = false;
                    _audioRgbCreator.enableClick(false);
                }

                switch (tabControl.SelectedIndex)
                {
                    case 0: // Audio
                        new_rgb_creator = _audioRgbCreator;
                        break;
                    case 1: // CPU-Temp
                        new_rgb_creator = _cpuRgbCreator;
                        break;
                    case 2: // Image
                        new_rgb_creator = _screenAnalyzer;
                        break;
                    case 3: // Color-Chooser
                        new_rgb_creator = _colorChooser;
                        break;
                    case 4: // Stroboscope
                        new_rgb_creator = _stroboscope;
                        break;
                    case 5:
                        new_rgb_creator = _selectedMisc;
                        break;
                    default:
                        return;
                }
                SetNewRGBCreator(new_rgb_creator);
            }
        }

        private void SetNewRGBCreator(RGBCreator.IRGBCreator new_rgb_creator)
        {
            if (_rgbCreatorI != null)
            {
                _rgbCreatorI.Pause(); // pause current rgbCreator
            }

            if (new_rgb_creator != null && _current_interface != null)
            {
                try
                {
                    new_rgb_creator.SetRGBOutput(_current_interface);
                    new_rgb_creator.Start(); // start new rgbCreator
                }
                catch (RGBCreator.RGBCreatorException ex)
                {        
                    MessageBox.Show(ex.Message, $"Error setting RGB-Creator {new_rgb_creator.GetType().Name}");
                }
            }
            _rgbCreatorI = new_rgb_creator;
        }


        private void RadioButtonChanged(object sender, RoutedEventArgs e)
        {
            RadioButton li = (sender as RadioButton);

            switch (li.Name)
            {
                case "RunningDot":
                    _selectedMisc = _runningDot;
                    break;
                case "Rainbow":
                    _selectedMisc = _rainbow;
                    break;
                case "ColorChanger":
                    _selectedMisc = _colorchanger;
                    break;
                case "RunningColors":
                    _selectedMisc = _runningColors;
                    break;
                default:
                    System.Diagnostics.Debug.Print("Weird radio button selected");
                    break;
            }
            SetNewRGBCreator(_selectedMisc);
        }
 
        /// ################################### Serial-Control ################################### 

        /// <summary>
        /// Method which is called when checkbox for serial enable is called
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CkbSerial_Click(object sender, RoutedEventArgs e)
        {
            SetNewInterface();
        }

        private void SetNewInterface()
        {
            string selected_name = RGB_Output.Items[RGB_Output.SelectedIndex] as string;
            try
            {
                IRGBOutput new_interface = null;
                foreach (IRGBOutput _interface in _rgb_manager.GetAvailableOutputs())
                {
                    string name = _interface.GetName();
                    
                    if (name == selected_name)
                    {
                        new_interface = _interface;
                        break;
                    }                    
                    
                }
                _rgbCreatorI.Pause();
                // Look if interface changed or checkbox is set to false
                if (_current_interface != null && (_current_interface.GetName() != selected_name || CkbSerial.IsChecked == false))
                {
                    if (_current_interface.IsEnabled())
                    {
                        _current_interface.Shutdown();
                        _current_interface.SetEnable(false);
                    }
                }
                _current_interface = new_interface;


                if (CkbSerial.IsChecked == true)
                {
                    if (!_current_interface.IsEnabled())
                    {
                        _current_interface.Initialize();
                        _current_interface.SetEnable(true);
                    }
                }

                SetNewRGBCreator(_rgbCreatorI);
            }
            catch (RGBOutputException ex)
            {
                CkbSerial.IsChecked = !CkbSerial.IsChecked; // Reset state                
                string test = "Error setting new Interace \"" + selected_name + "\":\n\n" + ex.Message;
                MessageBox.Show(test, "Error setting RGB Output");
            }
        }

        /// <summary>
        /// Method which is called when the RGB Output-choser was opened
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RGB_Output_DropDownOpened(object sender, EventArgs e)
        {
            String curOutput = "";
            if (RGB_Output.Items.Count > 0) {
                curOutput = RGB_Output.Items[RGB_Output.SelectedIndex] as String;
            }

            RGB_Output.Items.Clear();
            FillRGBOutputList();

            int newSelectedIndex = 0;

            for (int i = 0; i < RGB_Output.Items.Count; i++)
            {
                // Get new Index of selected RGB Output before (if a new port was added)
                if (RGB_Output.Items[i] as string == curOutput)
                {
                    newSelectedIndex = i;
                }
            }
            RGB_Output.SelectedIndex = newSelectedIndex;
        }


        private void FillRGBOutputList()
        {
            foreach (var out_put_interface in _rgb_manager.GetAvailableOutputs())
            {
                RGB_Output.Items.Add(out_put_interface.GetName());                
            }
            RGB_Output.SelectedIndex = 0;
        }


        private void RGB_Output_DropDownClosed(object sender, EventArgs e)
        {
            if (this.IsLoaded)
            {
                SetNewInterface();
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
            }
            else
            {
                BtnEnable.Content = "Enable";
                _audioRgbCreator.enableClick(false);
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
            _audioRgbCreator.TriggerValueChanged((int)slider.Value);
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
                xSkipper.Text = _screenAnalyzer.xSkipper.ToString(System.Globalization.CultureInfo.CurrentCulture);
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
                ySkipper.Text = _screenAnalyzer.ySkipper.ToString(System.Globalization.CultureInfo.CurrentCulture);
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
                xStart.Text = _screenAnalyzer.xStart.ToString(System.Globalization.CultureInfo.CurrentCulture);
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
                yStart.Text = _screenAnalyzer.yStart.ToString(System.Globalization.CultureInfo.CurrentCulture);
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
                xStop.Text = _screenAnalyzer.xStop.ToString(System.Globalization.CultureInfo.CurrentCulture);
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
                yStop.Text = _screenAnalyzer.yStop.ToString(System.Globalization.CultureInfo.CurrentCulture);
            }
        }

        private void ClrPicker_Changed(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (_colorChooser != null) // While initialization
            {
                _colorChooser.rgbChanged(Clr_picker.SelectedColor.Value.R, Clr_picker.SelectedColor.Value.G, Clr_picker.SelectedColor.Value.B);
            }
        }

        private void Frequency_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _stroboscope.setFrequency((int)Frequency.Value);
        }

        private void Emphaser_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _screenAnalyzer.setEmphaser((float)ColorEmphaser.Value);
        }

        private void Window_Activated(object sender, EventArgs e)
        {

            Application.Current.MainWindow.Activate();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _cpuRgbCreator.Dispose();
                _audioRgbCreator.Dispose();
                _screenAnalyzer.Dispose();
                _colorChooser.Dispose();
                _stroboscope.Dispose();
                _rainbow.Dispose();
                _runningDot.Dispose();
                _colorchanger.Dispose();
                _runningColors.Dispose();
            }
            else
            {
                // what do i have to do here?            
            }
        }
    }
}
