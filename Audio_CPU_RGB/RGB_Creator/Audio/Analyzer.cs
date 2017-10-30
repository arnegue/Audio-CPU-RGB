using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Un4seen.Bass;
using System.Text;
using Un4seen.BassWasapi;
using AudioCPURGB.RGB_Creator;
using System.Threading;
using AudioCPURGB.RGB_Output;

namespace AudioCPURGB.RGB_Creator {

    internal class Audio_RGB_Creator : RGB_Creator_Interface {
        private bool _enable;               //enabled status
     //   private DispatcherTimer _t;         //timer that refreshes the display
        private float[] _fft;               //buffer for fft data
        private ProgressBar _l, _r;         //progressbars for left and right channel intensity
        private WASAPIPROC _process;        //callback function to obtain data
        private int _lastlevel;             //last output level
        private int _hanctr;                //last output level counter
        private List<byte> _spectrumdata;   //spectrum data buffer
        private Spectrum _spectrum;         //spectrum dispay control
        private ComboBox _devicelist;       //device list
        private bool _initialized;          //initialized flag
        private int devindex;               //used device index
        private MyRangeSlider _mrs;

        private Thread _workerThread;
        private ManualResetEvent _pauseEvent = new ManualResetEvent(false);
        private const int _ms_sleepInterval = 25;
        private RGB_Output_Interface _rgbOutput;


        private int _lines = 16;            // number of spectrum lines
        //ctor
        public Audio_RGB_Creator(ProgressBar left, ProgressBar right, Spectrum spectrum, ComboBox devicelist, ComboBox algoChoser, MyRangeSlider mrs) {
            _fft = new float[1024];
            _lastlevel = 0;
            _hanctr = 0;
          //  _t = new DispatcherTimer();
           // _t.Tick += _t_Tick;
         //   _t.Interval = TimeSpan.FromMilliseconds(25); //40hz refresh rate
         //   _t.IsEnabled = false;
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
       

            algoChoser.Items.Add(new AudioAlgorithm("Algorithm 1", () => showAudioToRGBA1()));
            algoChoser.Items.Add(new AudioAlgorithm("Algorithm 2", () => showAudioToRGBA2()));
            // TODO algoChoser.SelectedIndex = 0;

            // Create a new Thread
            _workerThread = new Thread(cpuTempThread);
            _workerThread.IsBackground = true;

            _pauseEvent.Reset(); // Don't let the thread run
            _workerThread.Start(); // But start it (until it comes to the pauseEvent)

            Init();
        }

        public void start() {
            _pauseEvent.Set();
        }

        public void pause() {
            _pauseEvent.Reset();
        }

        public int minSliderValue { get; set; }
        public AudioAlgorithm activeAlgo { get; set; }
        public Boolean absNotRel { get; set; }
        

        // flag for display enable
        bool DisplayEnable = true;

        //flag for enabling and disabling program functionality
        public bool startBassWasapi
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
                            _devicelist.IsEnabled = false;
                        }
                    }
                    BassWasapi.BASS_WASAPI_Start();
                }
                else
                {
                    BassWasapi.BASS_WASAPI_Stop(true);
                    BassWasapi.BASS_WASAPI_Free();
                    _initialized = false;
                }
                //_t.IsEnabled = value;
            }
        }

        // initialization
        private void Init() {
            bool result = false;
            for (int i = 0; i < BassWasapi.BASS_WASAPI_GetDeviceCount(); i++) {
                var device = BassWasapi.BASS_WASAPI_GetDeviceInfo(i);
                if (device.IsEnabled && device.IsLoopback) {
                    _devicelist.Items.Add(string.Format("{0} - {1}", i, device.name));
                }
            }
            _devicelist.SelectedIndex = 0;
            Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_UPDATETHREADS, false);
            result = Bass.BASS_Init(0, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);
            if (!result) throw new Exception("Init Error");
     
            absNotRel = false;
            minSliderValue = 0;
        }


        // TODO: das ist nachher der Thread (vorher start-methode ausfuehren)
        //timer 
       // private void _t_Tick(object sender, EventArgs e) {
            private void cpuTempThread() {
            while (true) {
                _pauseEvent.WaitOne();

                int ret = BassWasapi.BASS_WASAPI_GetData(_fft, (int)BASSData.BASS_DATA_FFT2048); //get channel fft data
                if (ret < -1) return;
                int x, y;
                int b0 = 0;

                //computes the spectrum data, the code is taken from a bass_wasapi sample.
                for (x = 0; x < _lines; x++) {
                    float peak = 0;
                    int b1 = (int)Math.Pow(2, x * 10.0 / (_lines - 1));
                    if (b1 > 1023) b1 = 1023;
                    if (b1 <= b0) b1 = b0 + 1;
                    for (; b0 < b1; b0++) {
                        if (peak < _fft[1 + b0]) peak = _fft[1 + b0];
                    }
                    y = (int)(Math.Sqrt(peak) * 3 * 255 - 4);
                    if (y > 255) y = 255;
                    if (y < 0) y = 0;
                    _spectrumdata.Add((byte)y);
                }

                if (DisplayEnable) {
                    _spectrum.Set(_spectrumdata);
                }

                if (activeAlgo != null) {
                    activeAlgo._method();
                }
                _spectrumdata.Clear();


                int level = BassWasapi.BASS_WASAPI_GetLevel();
                _l.Value = Utils.LowWord32(level);
                _r.Value = Utils.HighWord32(level);
                if (level == _lastlevel && level != 0) _hanctr++;
                _lastlevel = level;

                //Required, because some programs hang the output. If the output hangs for a 75ms
                //this piece of code re initializes the output so it doesn't make a gliched sound for long.
                if (_hanctr > 3) {
                    _hanctr = 0;
                    _l.Value = 0;
                    _r.Value = 0;
                    Free();
                    Bass.BASS_Init(0, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);
                    _initialized = false;
                    startBassWasapi = true;
                }

                Thread.Sleep(_ms_sleepInterval);
            }
        }

        // WASAPI callback, required for continuous recording
        private int Process(IntPtr buffer, int length, IntPtr user) {
            return length;
        }

        //cleanup
        public void Free() {
            BassWasapi.BASS_WASAPI_Free();
            Bass.BASS_Free();
        }

        // ###### AUDIO-ALGORITHMS ######
        const int colors = 3;

        // Average
        private void showAudioToRGBA1() {
            byte[] specArray = _spectrumdata.ToArray();
            int[] rgb = new int[colors];

            // Now convert these 16 lines (from 0 to 255) to 3 RGB-Colours (from 0 to 255)
            int right = ((int)_mrs.RangeMax) + 1;
            int left = (int)_mrs.RangeMin;

            int stepsPerColor = (right - left) / colors;

            for (int rgbIndex = 0; rgbIndex < colors; rgbIndex++) {
                for (int i = left + (stepsPerColor * rgbIndex); i < left + (stepsPerColor * rgbIndex) + stepsPerColor; i++) {
                    // Only take values that are higher than that value given by the vertical slider
                    if (specArray[i] > minSliderValue) {
                        rgb[rgbIndex] += specArray[i];
                    }
                }

                int avg = rgb[rgbIndex] / stepsPerColor; // Calculate Average

                if (!absNotRel && minSliderValue != 0) { // Now Relativize them with steps
                    if (minSliderValue == 255) {
                        minSliderValue = 254;
                    }
                    rgb[rgbIndex] = (255 * (avg - minSliderValue)) / (255 - minSliderValue);
                } else {
                    rgb[rgbIndex] = avg;
                }
            }

            RGB_Value rgbv = new RGB_Value(rgb[0], rgb[1], rgb[2]);
            try {
                _rgbOutput.showRGB(rgbv);
            }
            catch (System.NullReferenceException) {
                // Seems normal when switching on/off
            }
        }

        RGB_Value _oldRGBValue = new RGB_Value();

        // Move the 2-Point-Slider to show only the colours from R-G-B 
        private void showAudioToRGBA2() {
            byte[] specArray = _spectrumdata.ToArray();
            RGB_Value rgbv = new RGB_Value();
           // int[] rgb = new int[colors];

            // Now convert these lines (from 0 to 255) to 3 RGB-Colours (from 0 to 255)
            int right = ((int)_mrs.RangeMax) + 1;
            int left = (int)_mrs.RangeMin;

            int rCount = 0, gCount = 0, bCount = 0;
            for (int i = left; i < right; i++) {
                if (i < 5) {
                    rgbv.r += specArray[i];
                    rCount++;
                }
                else if (i < 10) {
                    rgbv.g += specArray[i];
                    gCount++;
                }
                else {
                    rgbv.b += specArray[i];
                    bCount++;
                }
            }
            if (rCount != 0) {
                rgbv.r /= rCount;
            }
            if (gCount != 0) {
                rgbv.g /= gCount;
            }
            if (bCount != 0) {
                rgbv.b /= bCount;
            }
            
            if (!rgbv.Equals(_oldRGBValue))
            {
                // Finally, try to send the Value via Serial
                try
                {
                    String sendToSerial = rgbv.ToString();
                    _oldRGBValue = rgbv;
                    System.Diagnostics.Debug.Write("Trying to send to RGB: " + sendToSerial[0] + (int)sendToSerial[1] + sendToSerial[2] + (int)sendToSerial[3] + sendToSerial[4] + (int)sendToSerial[5] + sendToSerial[6] + sendToSerial[7]);
                    if (sendToSerial.Length == 8)
                    {
                        System.Diagnostics.Debug.WriteLine(" - completed");
                        _rgbOutput.showRGB(rgbv);
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine(" - failed");
                    }
                }
                catch (System.NullReferenceException)
                {
                    // Seems normal when switching on/off
                }
            } else
            {
                System.Diagnostics.Debug.WriteLine("Did not send Value, because it's the same as the old one");
            }
        }

        public void setRGBOutput(RGB_Output_Interface rgbOutput) {
            _rgbOutput = rgbOutput;
        }
    }
}
