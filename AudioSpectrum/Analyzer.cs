﻿using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Un4seen.Bass;
using System.Text;
using Un4seen.BassWasapi;
using System.Threading;

namespace AudioSpectrum {

    internal class Analyzer {
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
        private int devindex;               //used device index
        private MyRangeSlider _mrs;

        private int _lines = 16;            // number of spectrum lines
        //ctor
        public Analyzer(ProgressBar left, ProgressBar right, Spectrum spectrum, ComboBox devicelist, MyRangeSlider mrs) {
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
            
            Init();
        }

        public Boolean cpuNotAudio { get; set; }
        private CPU_Temperature cput;

        // Serial port for arduino output
        public SerialPort Serial { get; set; }

        // flag for display enable
        bool DisplayEnable = true;

        //flag for enabling and disabling program functionality
        public bool Enable {
            get { return _enable; }
            set {
                _enable = value;
                if (value) {
                    if (!_initialized) {
                        var array = (_devicelist.Items[_devicelist.SelectedIndex] as string).Split(' ');
                        devindex = Convert.ToInt32(array[0]);
                        bool result = BassWasapi.BASS_WASAPI_Init(devindex, 0, 0, BASSWASAPIInit.BASS_WASAPI_BUFFER, 1f, 0.05f, _process, IntPtr.Zero);
                        if (!result) {
                            var error = Bass.BASS_ErrorGetCode();
                            MessageBox.Show(error.ToString());
                        }
                        else {
                            _initialized = true;
                            _devicelist.IsEnabled = false;
                        }
                    }
                    BassWasapi.BASS_WASAPI_Start();
                }
                else BassWasapi.BASS_WASAPI_Stop(true);
                _t.IsEnabled = value;
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
            cput = CPU_Temperature.getInstance();
            cpuNotAudio = false;
        }

        //timer 
        private void _t_Tick(object sender, EventArgs e) {
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

            if (DisplayEnable) _spectrum.Set(_spectrumdata);

            if (Serial != null) {
                if (!cpuNotAudio) {
                    showAudioToRGB();
                }
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
                Enable = true;
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

        const int colors = 3;
        private void showAudioToRGB() {
            byte[] specArray = _spectrumdata.ToArray();
            int[] rgb = new int[colors];

            // Now convert these 16 lines (from 0 to 255) to 3 RGB-Colours (from 0 to 255)
            // 16 / 3 is not that good, so i will use 5, 6, 5 and calculate the average value

            /* // Whole Spectrum
            r = (specArray[0] + specArray[1] + specArray[2] + specArray[3] + specArray[4]) / 5;
            g = (specArray[5] + specArray[6] + specArray[7] + specArray[8] + specArray[9] + specArray[10]) / 6;
            b = (specArray[11] + specArray[12] + specArray[13] + specArray[14] + specArray[15]) / 5;
            */
            int right = ((int)_mrs.RangeMax) + 1;
            int left = (int)_mrs.RangeMin;

            int stepsPerColor = (right - left) / 3;
            // int mod = (right - left) % 3;

            //IF check je nach algo
             for (int rgbIndex = 0; rgbIndex < colors; rgbIndex++) {
                 for (int i = left + (stepsPerColor * rgbIndex); i < left + (stepsPerColor * rgbIndex) + stepsPerColor; i++) {
                     rgb[rgbIndex] += specArray[i];
                 }
                 rgb[rgbIndex] /= stepsPerColor;
             }
             // else
           /* int rCount = 0, gCount = 0, bCount = 0;
            for (int i = left; i < right; i++) {
                if (i < 5) {
                    rgb[0] += specArray[i];
                    rCount++;
                } else if (i < 10) {
                    rgb[1] += specArray[i];
                    gCount++;
                } else {
                    rgb[2] += specArray[i];
                    bCount++;
                }
            }
            if (rCount != 0) {
                rgb[0] /= rCount;
            }
            if (gCount != 0) {
                rgb[1] /= gCount;
            }
            if (bCount != 0) {
                rgb[2] /= bCount;
            }*/
            // if else check ende


            /*  // First 9 lines
              r = (specArray[0] + specArray[1] + specArray[2]) / 3;
              g = (specArray[3] + specArray[4] + specArray[5]) / 3;
              b = (specArray[6] + specArray[7] + specArray[8]) / 3;
              // TODO: maybe without objectcreation it will get faster?*/





            RGBValue rgbv = new RGBValue(rgb[0], rgb[1], rgb[2]);
            try {
                Serial.WriteLine(rgbv.ToString());
            } catch (System.NullReferenceException e) {
                // Seems normal when switching on/off
            }
        }
    }
}
