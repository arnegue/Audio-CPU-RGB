using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Un4seen.Bass;
using Un4seen.BassWasapi;
using AudioCPURGB.RGBCreator;
using AudioCPURGB.RGBCreator.Audio;

namespace AudioCPURGB
{

    internal class AudioRGBCreator : IRGBCreator
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

        private int _lines = 16;            // number of spectrum lines
        //ctor
        public AudioRGBCreator(System.Windows.Controls.Primitives.ToggleButton btnEnable, ProgressBar left, ProgressBar right, Spectrum spectrum, ComboBox devicelist, ComboBox algoChoser, MyRangeSlider mrs, Slider triggerSlider)
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

            algoChoser.Items.Add(new ShowAudioToRGB1());
            algoChoser.Items.Add(new ShowAudioToRGB2());
            algoChoser.Items.Add(new ShowAudioToRGB3());
            algoChoser.Items.Add(new ShowAudioToRGB4());
            algoChoser.Items.Add(new ShowAudioToRGB5());
            algoChoser.Items.Add(new ShowAudioToRGB6());

            Init();
        }

        private int minSliderValue;
        public AudioAlgorithm activeAlgo { get; set; }
        public Boolean absNotRel { get; set; }
      
        // flag for display enable
        bool DisplayEnable = true;

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

        protected override void Callback()
        {

        }

        //flag for enabling and disabling program functionality
        public bool Enable // TODO pause?
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
                        devindex = Convert.ToInt32(array[0], System.Globalization.CultureInfo.CurrentCulture);
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
            try
            {
                Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_UPDATETHREADS, false);
                Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_UNICODE, true);
            }
            catch (System.TypeInitializationException e)
            {
                return; //   throw new Exception("Init Error", e);
            }

            if (!(Bass.BASS_Init(0, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero)))
                throw new Exception("Init Error");
            
            try
            {
                for (int i = 0; i < BassWasapi.BASS_WASAPI_GetDeviceCount(); i++)
                {
                    var device = BassWasapi.BASS_WASAPI_GetDeviceInfo(i);
                    if (device.IsEnabled && device.IsLoopback)
                    {
                        _devicelist.Items.Add(string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0} - {1}", i, device.name));
                        System.Diagnostics.Debug.Print("Found device: " + device.name);
                        System.Diagnostics.Debug.Print("Default? " + device.IsDefault);
                        System.Diagnostics.Debug.Print("Input? " + device.IsInput);
                        System.Diagnostics.Debug.Print("Loopb? " + device.IsLoopback);
                        System.Diagnostics.Debug.Print("Unplugged? " + device.IsUnplugged);
                        System.Diagnostics.Debug.Print("Init? " + device.IsInitialized);
                        System.Diagnostics.Debug.Print(device + "\n");
                    }
                }
            } catch (System.DllNotFoundException e)
            {
                System.Diagnostics.Debug.Print("It seems like you loaded the wrong basswasapi.dll. Be sure to put the x64-dll to the x64-Build folder and the same with x32-dll in the x32 folder: " + e.Message);
            }
            _devicelist.SelectedIndex = 0;

            absNotRel = false;
            minSliderValue = 0;
        }

        // WASAPI callback, required for continuous recording
        private int Process(IntPtr buffer, int length, IntPtr user)
        {
            return length;
        }

        //cleanup
        public static void Free()
        {
            BassWasapi.BASS_WASAPI_Free();
            Bass.BASS_Free();
        }

        //timer 
        private const int skipper = 2;
        private int skip;
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
                if (activeAlgo != null && _rgbOutput != null && _rgbOutput.IsEnabled())
                {
                    activeAlgo.showRGB(_spectrumdata.ToArray(), (int)_mrs.RangeMin, (int)_mrs.RangeMax, minSliderValue, absNotRel, _rgbOutput);  // Now show Audio_RGB-Algorithm
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
         
        public void TriggerValueChanged(int value)
        {
            minSliderValue = value;            
        }
    }
}
