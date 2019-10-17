using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Un4seen.Bass;
using System.Text;
using Un4seen.BassWasapi;
using System.Threading;
using AudioCPURGB.RGB_Creator;
using AudioCPURGB.RGB_Output;

namespace AudioCPURGB
{

    internal class Audio_RGB_Creator : RGB_Creator_Interface
    {
        private bool _enable;               //enabled status
        private DispatcherTimer _t;         //timer that refreshes the display
        private float[] _fft;               //buffer for fft data
        private ProgressBar _l, _r;         //progressbars for left and right channel intensity
        private WASAPIPROC _process;        //callback function to obtain data
        private int _lastlevel;             //last output level
        private int _hanctr;                //last output level counter
        private List<byte> _spectrumdata;   //spectrum data buffer
        private Spectrum _spectrum;         //spectrum dispay control
        private ComboBox _devicelist;       //device list
        private bool _initialized;          //initialized flag
        private int devindex;               //used device indexs
        private MyRangeSlider _mrs;
        private Slider _triggerSlider;
        private System.Windows.Controls.Primitives.ToggleButton _btnEnable;

        private RGB_Output_Interface _rgbOutput;

        private int _lines = 16;            // number of spectrum lines
        //ctor
        public Audio_RGB_Creator(System.Windows.Controls.Primitives.ToggleButton btnEnable, ProgressBar left, ProgressBar right, Spectrum spectrum, ComboBox devicelist, ComboBox algoChoser, MyRangeSlider mrs, Slider triggerSlider)
        {
            _fft = new float[1024];
            _lastlevel = 0;
            _hanctr = 0;
            _t = new DispatcherTimer();
            _t.Tick += _t_Tick;
            _t.Interval = TimeSpan.FromMilliseconds(25); //40hz refresh rate
            _t.IsEnabled = false;
            _l = left;
            _r = right;
            _l.Minimum = 0;
            _r.Minimum = 0;
            _r.Maximum = ushort.MaxValue;
            _l.Maximum = ushort.MaxValue;
            _process = new WASAPIPROC(Process);
            _spectrumdata = new List<byte>();
            _spectrum = spectrum;
            _devicelist = devicelist;
            _initialized = false;
            _mrs = mrs;
            _btnEnable = btnEnable;
            _triggerSlider = triggerSlider;

            algoChoser.Items.Add(new AudioAlgorithm("Algorithm 1", () => showAudioToRGBA1()));
            algoChoser.Items.Add(new AudioAlgorithm("Algorithm 2", () => showAudioToRGBA2()));
            algoChoser.Items.Add(new AudioAlgorithm("Algorithm 3", () => showAudioToRGBA3()));
            algoChoser.Items.Add(new AudioAlgorithm("Algorithm 4", () => showAudioToRGBA4()));
            algoChoser.Items.Add(new AudioAlgorithm("Algorithm 5", () => showAudioToRGBA5()));
            algoChoser.Items.Add(new AudioAlgorithm("Algorithm 6", () => showAudioToRGBA6()));
            
            //algoChoser.SelectedIndex = 0;

            Init();
        }

        private int minSliderValue;
        public AudioAlgorithm activeAlgo { get; set; }
        public Boolean absNotRel { get; set; }

        // Serial port for arduino output
        //public SerialPort _serial { get; set; }

        // flag for display enable
        bool DisplayEnable = true;

        public void start()
        {
            //  _t.IsEnabled = true;
        }

        public void pause()
        {
            // _btnEnable.IsChecked= false;
        }

        public void enableClick(bool enable)
        {
            _t.IsEnabled = enable;
            _initialized = enable;
            if(enable == false)
            {
                BassWasapi.BASS_WASAPI_Stop(true);
                BassWasapi.BASS_WASAPI_Free();
                Bass.BASS_Free();
            }
            Enable = enable;
        }

        //flag for enabling and disabling program functionality
        public bool Enable
        {
            get { return _enable; }
            set
            {
                _enable = value;
                if (value)
                {
                    if (!_initialized)
                    {
                        var array = (_devicelist.Items[_devicelist.SelectedIndex] as string).Split(' ');
                        devindex = Convert.ToInt32(array[0]);
                        bool result = BassWasapi.BASS_WASAPI_Init(devindex, 0, 0, BASSWASAPIInit.BASS_WASAPI_BUFFER, 1f, 0.05f, _process, IntPtr.Zero);
                        if (!result)
                        {
                            var error = Bass.BASS_ErrorGetCode();
                            MessageBox.Show(error.ToString());
                        }
                        else
                        {
                            _initialized = true;
                            // _devicelist.IsEnabled = false;
                        }
                    }
                    BassWasapi.BASS_WASAPI_Start();
                }
                else
                {
                    BassWasapi.BASS_WASAPI_Stop(true);
                }
                _t.IsEnabled = value;
            }
        }

        // initialization
        private void Init()
        {
            bool result = false;
            try
            {
                for (int i = 0; i < BassWasapi.BASS_WASAPI_GetDeviceCount(); i++)
                {
                    var device = BassWasapi.BASS_WASAPI_GetDeviceInfo(i);
                    if (device.IsEnabled && device.IsLoopback)
                    {
                        _devicelist.Items.Add(string.Format("{0} - {1}", i, device.name));
                        System.Diagnostics.Debug.Print("Found device: " + device.name);
                        System.Diagnostics.Debug.Print("Default? " + device.IsDefault);
                        System.Diagnostics.Debug.Print("Input? " + device.IsInput);
                        System.Diagnostics.Debug.Print("Loopb? " + device.IsLoopback);
                        System.Diagnostics.Debug.Print("Unplugged? " + device.IsUnplugged);
                        System.Diagnostics.Debug.Print("Init?? " + device.IsInitialized);
                        System.Diagnostics.Debug.Print(device + "\n");
                    }
                    //  BASS_CONFIG_DEV_DEFAULT
                }
            } catch(Exception)
            {
                System.Diagnostics.Debug.Print("It seems like you loaded the wrong basswasapi.dll. Be sure to put the x64-dll to the x64-Build folder and the same with x32-dll in the x32 folder");
            }
            _devicelist.SelectedIndex = 0;
            Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_UPDATETHREADS, false);
            result = Bass.BASS_Init(0, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);
            BASS_DEVICEINFO info =  Bass.BASS_GetDeviceInfo(-1);

            if (!result) throw new Exception("Init Error");
            /*foreach (Un4seen.BassWasapi.BASS_WASAPI_DEVICEINFO device in _devicelist.Items)
            {
                System.Diagnostics.Debug.Print("Found device: " + device.name);
                System.Diagnostics.Debug.Print("Default? " + device.IsDefault);
                System.Diagnostics.Debug.Print("Input? " + device.IsInput);
                System.Diagnostics.Debug.Print("Loopb? " + device.IsLoopback);
                System.Diagnostics.Debug.Print("Unplugged? " + device.IsUnplugged);
                System.Diagnostics.Debug.Print("Init?? " + device.IsInitialized);
                System.Diagnostics.Debug.Print(device + "\n");
            }*/

            absNotRel = false;
            minSliderValue = 0;
        }


        // WASAPI callback, required for continuous recording
        private int Process(IntPtr buffer, int length, IntPtr user)
        {
            return length;
        }

        //cleanup
        public void Free()
        {
            BassWasapi.BASS_WASAPI_Free();
            Bass.BASS_Free();
        }

        //timer 
        private const int skipper = 2;
        private int skip = 0;
        private void _t_Tick(object sender, EventArgs e)
        {
            int ret = BassWasapi.BASS_WASAPI_GetData(_fft, (int)BASSData.BASS_DATA_FFT2048); //get channel fft data
            if (ret < -1) return;
            int x, y;
            int b0 = 0;

            //computes the spectrum data, the code is taken from a bass_wasapi sample.
            for (x = 0; x < _lines; x++)
            {
                float peak = 0;
                int b1 = (int)Math.Pow(2, x * 10.0 / (_lines - 1));
                if (b1 > 1023) b1 = 1023;
                if (b1 <= b0) b1 = b0 + 1;
                for (; b0 < b1; b0++)
                {
                    if (peak < _fft[1 + b0]) peak = _fft[1 + b0];
                }
                y = (int)(Math.Sqrt(peak) * 3 * 255 - 4);
                if (y > 255) y = 255;
                if (y < 0) y = 0;
                _spectrumdata.Add((byte)y);
            }

            if (DisplayEnable) _spectrum.Set(_spectrumdata);

            if (skip >= skipper)
            {
                if (activeAlgo != null && _rgbOutput != null && _rgbOutput.isEnabled())
                {
                    activeAlgo._method();  // Now show Audio_RGB-Algorithm
                }
                skip = 0;
            }
            skip++;
            _spectrumdata.Clear();


            int level = BassWasapi.BASS_WASAPI_GetLevel();
            _l.Value = Utils.LowWord32(level);
            _r.Value = Utils.HighWord32(level);
            if (level == _lastlevel && level != 0) _hanctr++;
            _lastlevel = level;

            //Required, because some programs hang the output. If the output hangs for a 75ms
            //this piece of code re initializes the output so it doesn't make a gliched sound for long.
            if (_hanctr > 3)
            {
                _hanctr = 0;
                _l.Value = 0;
                _r.Value = 0;
                Free();
                Bass.BASS_Init(0, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);
                _initialized = false;
                Enable = true;
            }
        }

        private double m = 1020.0 / (double)255;
        private const double b_b2 = 510.0;
        private const double b_g1 = -255.0;
        private const double b_g2 = 765.0;
        private const double b_r = -510.0;
        private RGB_Value valueToRGB(byte value)
        {
            int minTrigger = minSliderValue;
            if (minTrigger == 0)
            {
                minTrigger = 1;
            }

            byte r = 0, g = 0, b = 0;
            if (value < minTrigger / 3)
            {
                b = (byte)(m * value);
            }
            else if (value < (minTrigger * 2) / 3)
            {
                b = (byte)(-m * value + b_b2);
                g = (byte)(m * value + b_g1);
            }
            else if (value < minTrigger)
            {
                g = (byte)(-m * value + b_g2);
                r = (byte)(m * value + b_r);
            }
            else
            {
                r = 255;
            }

            return new RGB_Value(r, g, b);
        }

        // ############################################# AUDIO-ALGORITHMS #############################################
        const int colors = 3;  // Colours per RGB

        /// <summary>
        /// Takes every value between min and max slider and calculates average for each color range. 
        /// RangeMin and RangeMax are also min and max limit for colors (left = red, right = blue)
        /// Sends average to EVERY RGB ("old algorithm")
        /// </summary>
        private void showAudioToRGBA1()
        {
            byte[] specArray = _spectrumdata.ToArray();
            byte[] rgb = new byte[colors];

            // Now convert these 16 lines (from 0 to 255) to 3 RGB-Colours (from 0 to 255)
            byte right = (byte)(_mrs.RangeMax + 1);
            byte left = (byte)_mrs.RangeMin;

            byte stepsPerColor = (byte)((right - left) / 3);

            for (byte rgbIndex = 0; rgbIndex < colors; rgbIndex++)
            {
                for (int i = left + (stepsPerColor * rgbIndex); i < left + (stepsPerColor * rgbIndex) + stepsPerColor; i++)
                {
                    // Only take values that are higher than that value given by the vertical slider
                    if (specArray[i] > minSliderValue)
                    {
                        rgb[rgbIndex] += specArray[i];
                    }
                }

                byte avg = (byte)(rgb[rgbIndex] / stepsPerColor); // Calculate Average

                if (!absNotRel && minSliderValue != 0)
                { // Now Relativize them with steps
                    if (minSliderValue == 255)
                    {
                        minSliderValue = 254;
                    }
                    float x = 255.0F / minSliderValue;
                    float val = (100 * avg) / x;
                   // float test = (255 * (avg - minSliderValue)) / (255 - minSliderValue);
                    rgb[rgbIndex] = (byte)val;
                  //  rgb[rgbIndex] = (255 * (avg - minSliderValue)) / (255 - minSliderValue);
                }
                else
                {
                    rgb[rgbIndex] = avg;
                }
            }


            sendRGB(rgb[0], rgb[1], rgb[2]);
        }

        RGB_Value _oldRGBValue = new RGB_Value(0, 0, 0);

        /// <summary>
        /// Takes every value between min and max slider and calculates average for each color range. 
        /// Min and max slider are also min and limit for colour. The limit does not change the area but the colors that are displayed
        /// Sends average to EVERY RGB
        /// </summary>
        private void showAudioToRGBA2()
        {
            byte[] specArray = _spectrumdata.ToArray();
            byte[] rgb = new byte[colors];

            // Now convert these lines (from 0 to 255) to 3 RGB-Colours (from 0 to 255)
            int right = ((int)_mrs.RangeMax) + 1;
            int left = (int)_mrs.RangeMin;

            int rCount = 0, gCount = 0, bCount = 0;
            for (int i = left; i < right; i++)
            {
                if (i < 5)
                {
                    rgb[0] += specArray[i];
                    rCount++;
                }
                else if (i < 10)
                {
                    rgb[1] += specArray[i];
                    gCount++;
                }
                else
                {
                    rgb[2] += specArray[i];
                    bCount++;
                }
            }
            if (rCount != 0)
            {
                rgb[0] = (byte)(rgb[0] / rCount);
            }
            if (gCount != 0)
            {
                rgb[1] = (byte)(rgb[1] / gCount);
            }
            if (bCount != 0)
            {
                rgb[2] = (byte)(rgb[2] / bCount);
            }


            sendRGB(rgb[0], rgb[1], rgb[2]);
        }
    

        public void triggerValueChanged(int value)
        {
            minSliderValue = value;
            if (value < 0)
            {
                value = 1;
            }
            m = 765 / (double)value;
        }

        int[] bassAvg = new int[2];
        int bass_avg_Index = 0;
        RGB_Value[] rgbs = null;

        /// <summary>
        /// Takes every value between min and max slider and calculates average for each color range.
        /// Displays every AudioValue to specific RGB (depending on AmountRGBs)
        /// RangeMin and RangeMax are also min and max limit for colors (left = red, right = blue)
        /// Sends average to RGBs ("new algorithm")
        /// </summary>
        private void showAudioToRGBA3()
        {
            byte[] specArray = _spectrumdata.ToArray();

            int left = (int)_mrs.RangeMin;
            int right = ((int)_mrs.RangeMax) + 1;
            int bass = 0;
            for (int i = left; i < right; i++)
            {
                bass += specArray[i];
              /*  if(specArray[i] > _triggerSlider.Value)
                {
                    _triggerSlider.Value = specArray[i];
                }*/
            }
            if (right - left > 0)
            {
                bass /= (right - left);
            }

            bassAvg[bass_avg_Index] = bass;
            bass_avg_Index = (bass_avg_Index + 1) % bassAvg.Length;
            int currBassAvg = 0;
            for(int i = 0; i < bassAvg.Length; i++)
            {
                currBassAvg += bassAvg[i];
            }
            currBassAvg /= bassAvg.Length;
            bass = currBassAvg;

            int minTrigger = minSliderValue;

            int leds = 0;
            if (bass > 0 && minTrigger > 0)
            {
                leds = (int)(((double)_rgbOutput.getAmountRGBs() * (double)bass) / (double)minTrigger);
            }
        
            if(bass > minTrigger)
            {
                minTrigger = bass;
                _triggerSlider.Value = bass;                
            }

            RGB_Value emptyRGB = new RGB_Value();
            RGB_Value setRGB = valueToRGB((byte)bass); 
            
            if (rgbs == null)
            {
                rgbs = new RGB_Value[_rgbOutput.getAmountRGBs()]; // TODO if serial output changes, it wont take affect
            }

            // TODO amount leds in gui
            for (int i = 0; i < rgbs.Length; i++)
            {
                //rgbs[i] = new RGB_Value();
                if (i <= leds)
                {
                    rgbs[i] = setRGB;
                }
                else
                {
                    rgbs[i] = emptyRGB;
                }
            }
            _rgbOutput.showRGBs(rgbs);
        }

        /// <summary>
        /// Takes values between min and max slider. Divides Range into 3 colours range, Gets peak. Peak +- 1 --> Average.
        /// RangeMin and RangeMax are also min and max limit for colors (left = red, right = blue) (like showAudioToRGBA1)
        /// Sends average to EVERY RGB ("old algorithm")
        /// </summary>
        private void showAudioToRGBA4()
        {
            bool with_avg = true;
            byte[] specArray = _spectrumdata.ToArray();

            byte right_max = (byte)(_mrs.RangeMax + 1);
            byte left_max =  (byte)_mrs.RangeMin;
            int diff = (right_max - left_max) / colors;
            byte peak_volume; // TODO slider too much to the right -> exception

            byte[] rgb = new byte[colors];

            int left;
            int right;

            for (int i = 0; i < colors; i++) {
                left = left_max + (diff * i);
                right = left_max + (diff * (i + 1));

                int maximumIndex = this.get_maximum_peak_index_range(left, right, specArray);

                if (with_avg)
                {
                    // To avoid OutOfBounds
                    if (maximumIndex <= left)
                    {
                        maximumIndex += 1;
                    }
                    else if (maximumIndex >= right)
                    {
                        maximumIndex -= 1;
                    }
                    // TODO maybe too much casting right now
                    peak_volume = (byte)((int)((int)specArray[maximumIndex - 1] + (int)specArray[maximumIndex] + (int)specArray[maximumIndex + 1]) / 3);
                } else
                {
                    peak_volume = specArray[maximumIndex];
                }
                if (peak_volume < minSliderValue)
                {
                    peak_volume = 0;
                }

                rgb[i] = peak_volume;
            }


            sendRGB(rgb[0], rgb[1], rgb[2]);
        }

        /// <summary>
        /// Takes values between min and max slider. Divides Range into 3 colours range, Gets peak. Peak +- 1 --> Average.
        /// Min and max slider are also min and limit for colour. The limit does not change the area but the colors that are displayed (like showAudioToRGBA2)
        /// Sends average to EVERY RGB ("old algorithm")
        /// </summary>
        private void showAudioToRGBA5()
        {
            bool with_avg = true;
            byte[] specArray = _spectrumdata.ToArray();

            byte right_max = (byte)(_mrs.RangeMax + 1);
            byte left_max = (byte)_mrs.RangeMin;
            int diff = 5; // Slidermax / colours ;  TODO change in code! // (right_max - left_max) / colors;
            byte peak_volume;

            byte[] rgb = new byte[colors];

            int left, search_left;
            int right, search_right;

            for (int i = 0; i < colors; i++)
            {
                left = left_max + (diff * i);
                right = left_max + (diff * (i + 1));

                if (left < left_max)
                {
                    search_left = left_max;
                } else
                {
                    search_left = left;
                }
                if (right > right_max)
                {
                    search_right = right_max;
                } else
                {
                    search_right = right;
                }
                
                int maximumIndex = this.get_maximum_peak_index_range(search_left, search_right, specArray);

                if (maximumIndex < left || maximumIndex > right)
                {
                    rgb[i] = 0;
                }


                if (with_avg)
                {
                    // To avoid OutOfBounds
                    if (maximumIndex <= left)
                    {
                        maximumIndex += 1;
                    }
                    else if (maximumIndex >= right)
                    {
                        maximumIndex -= 1;
                    }
                    // TODO maybe too much casting right now
                    peak_volume = (byte)((int)((int)specArray[maximumIndex - 1] + (int)specArray[maximumIndex] + (int)specArray[maximumIndex + 1]) / 3);
                }
                else
                {
                    peak_volume = specArray[maximumIndex];
                }
                if (peak_volume < minSliderValue)
                {
                    peak_volume = 0;
                }

                rgb[i] = peak_volume;
            }


            sendRGB(rgb[0], rgb[1], rgb[2]);
        }

        /// <summary>
        /// Takes values between min and max slider. Divides Range into 3 colours range, Gets peak. Peak +- 1 --> Average.
        /// RangeMin and RangeMax are only limiting the "recorded" sound), and are used for steps per colour
        /// Sends average to EVERY RGB ("old algorithm")
        /// </summary>
        private void showAudioToRGBA6()
        {
            bool with_avg = true;
            byte[] specArray = _spectrumdata.ToArray();

            byte right_max = (byte)(_mrs.RangeMax + 1);
            byte left_max = (byte)_mrs.RangeMin;

            //byte stepsPerColor = (byte)((right_max - left_max) / 3);
            byte ledsPerColor = (byte)((_rgbOutput.getAmountRGBs()) / 3);

            int diff = (right_max - left_max) / colors;
            byte peak_volume; // TODO slider too much to the right -> exception
            
            int left;
            int right;

            if (rgbs == null)
            {
                rgbs = new RGB_Value[_rgbOutput.getAmountRGBs()]; // TODO if serial output changes, it wont take affect
            }

            for (int i = 0; i < colors; i++)
            {
                left = left_max + (diff * i);
                right = left_max + (diff * (i + 1));

                int maximumIndex = this.get_maximum_peak_index_range(left, right, specArray);

                if (with_avg)
                {
                    // To avoid OutOfBounds
                    if (maximumIndex <= left)
                    {
                        maximumIndex += 1;
                    }
                    else if (maximumIndex >= right)
                    {
                        maximumIndex -= 1;
                    }
                    // TODO maybe too much casting right now
                    peak_volume = (byte)((int)((int)specArray[maximumIndex - 1] + (int)specArray[maximumIndex] + (int)specArray[maximumIndex + 1]) / 3);
                }
                else
                {
                    peak_volume = specArray[maximumIndex];
                }
                if (peak_volume < minSliderValue)
                {
                    peak_volume = 0;
                }
                
                // TODO r is on right side right now and b on left. Change it.
                for (int o = (ledsPerColor * i); o < (ledsPerColor * (i+1)); o++)
                {
                    rgbs[o] = valueToRGB((byte)(peak_volume / 2));  // TODO shiat
                }
            }
            
            _rgbOutput.showRGBs(rgbs);
        }

        /// <summary>
        /// Returns the index of the maximum peak
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        private int get_maximum_peak_index_range(int left, int right, byte[] specArray)
        {
            int maximumIndex = 0;
            byte maxVal = 0;

            // Get maximum index and its value from current array
            for (int i = left; i < right; i++)
            {
                if (specArray[i] > maxVal)
                {
                    maximumIndex = i;
                    maxVal = specArray[i];
                }
            }

            return maximumIndex;
        }

        /// <summary>
        /// Same as sendRGBValue but takes 3 bytes as parameter instead of RGB_Value
        /// </summary>
        /// <param name="red"></param>
        /// <param name="green"></param>
        /// <param name="blue"></param>
        private void sendRGB(byte red, byte green, byte blue)
        {
            sendRGBValue(new RGB_Value(red, green, blue));
        }

        /// <summary>
        /// Sends one RGB_Value to every LED (old Protocol)
        /// </summary>
        /// <param name="rgbv"></param>
        private void sendRGBValue(RGB_Value rgbv)
        {
            if (!rgbv.Equals(_oldRGBValue))
            {
                _rgbOutput.showRGB(rgbv);
                _oldRGBValue = rgbv;
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Did not send Value, because it's the same as the old one");
            }
        }

        /// <summary>
        /// Setter to RGB_Output
        /// </summary>
        /// <param name="rgbOutput"></param>
        public void setRGBOutput(RGB_Output_Interface rgbOutput)
        {
            _rgbOutput = rgbOutput;
        }
    }
}
