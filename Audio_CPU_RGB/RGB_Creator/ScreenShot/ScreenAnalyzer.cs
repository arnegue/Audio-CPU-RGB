﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using AudioCPURGB.RGB_Output;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using AudioCPURGB.RGB_Creator;

namespace AudioCPURGB
{
    class ScreenAnalyzer : RGB_Creator_Interface
    {
        private Thread _workerThread;
        private ManualResetEvent _pauseEvent = new ManualResetEvent(false);
        private RGB_Output_Interface _rgbOutput;
        private Screen activeScreen = Screen.PrimaryScreen;

        private int xSkipper_;
        private int ySkipper_;
        private int xStart_;
        private int yStart_;
        private int xStop_;
        private int yStop_;

        private float emphaser_;
        private RGB_Value lastRGB_;

        private Mutex varMutex = new Mutex();

        public ScreenAnalyzer()
        {
            // Create a new Thread
            _workerThread = new Thread(screenshotThread);
            _workerThread.IsBackground = true;

            _pauseEvent.Reset(); // Don't let the thread run
            _workerThread.Start(); // But start it (until it comes to the pauseEvent)

            emphaser_ = 1.0F;
            xSkipper_ = 100;
            ySkipper_ = 100;
            xStart_ = 0; // TODO
            yStart_ = 0;
            xStop_ = activeScreen.Bounds.Width;
            yStop_ = activeScreen.Bounds.Height;
            lastRGB_ = new RGB_Value();
        }
        public int xSkipper
        {
            set
            {
                if (value < 0)
                {
                    value = 0;
                }
                else if (value > activeScreen.Bounds.Width)
                {
                    value = activeScreen.Bounds.Width;
                }
                varMutex.WaitOne();
                xSkipper_ = value;
                varMutex.ReleaseMutex();
            }
            get
            {
                return xSkipper_;
            }
        }

        public int ySkipper
        {
            set
            {
                if (value < 0)
                {
                    value = 0;
                }
                else if (value > activeScreen.Bounds.Width)
                {
                    value = activeScreen.Bounds.Width;
                }
                varMutex.WaitOne();
                ySkipper_ = value;
                varMutex.ReleaseMutex();
            }
            get
            {
                return ySkipper_;
            }
        }

        public int xStart
        {
            set
            {
                if (value < 0)
                {
                    value = 0;
                }
                else if (value > xStop)
                {
                    xStart_ = xStop - 1;
                }
                varMutex.WaitOne();
                xStart_ = value;
                varMutex.ReleaseMutex();
            }
            get
            {
                return xStart_;
            }
        }

        public int xStop
        {
            set
            {
                if (value < xStart)
                {
                    value = xStart + 1;
                }
                else if (value > activeScreen.Bounds.Width)
                {
                    value = activeScreen.Bounds.Width;
                }
                varMutex.WaitOne();
                xStop_ = value;
                varMutex.ReleaseMutex();
            }
            get
            {
                return xStop_;
            }
        }
        public int yStart
        {
            set
            {
                if (value < 0)
                {
                    value = 0;
                }
                else if (value > yStop)
                {
                    value = yStop - 1;
                }
                yStart_ = value;
            }
            get
            {
                return yStart_;
            }
        }

        public int yStop
        {
            set
            {
                if (value < yStart)
                {
                    value = yStart + 1;
                }
                else if (value > activeScreen.Bounds.Width)
                {
                    value = activeScreen.Bounds.Width;
                }
                yStop_ = value;
            }
            get
            {
                return yStop_;
            }
        }

        public void start()
        {
            _pauseEvent.Set();
        }

        public void pause()
        {
            _pauseEvent.Reset();
        }

        public void setRGBOutput(RGB_Output_Interface rgbOutput)
        {
            _rgbOutput = rgbOutput;
        }

        public void setEmphaser(float value)
        {
            if (value < 0)
            {
                value = 0;
            } else if (value > 1)
            {
                value = 1;
            }
            emphaser_ = 1 + value;
        }

        private Bitmap memoryImage = new Bitmap(1920, 1080); // TODO screen resolution??
        private Size s;//= new Size(memoryImage.Width, memoryImage.Height);
        private Graphics memoryGraphics;
        private void screenshotThread()
        {
            s = new Size(memoryImage.Width, memoryImage.Height);
            while (true)
            {
                _pauseEvent.WaitOne();
                if (_rgbOutput.isEnabled())
                {
                    try
                    {
                        memoryGraphics = Graphics.FromImage(memoryImage);
                        memoryGraphics.CopyFromScreen(0, 0, 0, 0, s);
                        memoryGraphics.Dispose();

                        double red = 0;
                        double green = 0;
                        double blue = 0;
                        float brightness = 0.0F;
                        int cntr = 0;
                        varMutex.WaitOne();
                        
                        for (int x = 0; x < activeScreen.Bounds.Width; x += xSkipper_)
                        {
                            for (int y = 0; y < activeScreen.Bounds.Height; y += ySkipper_)
                            {
                                Color pixel = memoryImage.GetPixel(x, y);
                                brightness += pixel.GetBrightness();
                                
                                red += pixel.R;
                                green += pixel.G;
                                blue += pixel.B;
                                cntr++;
                            }
                        }
                        varMutex.ReleaseMutex();
                        RGB_Value rgb;
                        //long divider = x * y;  //(activeScreen.Bounds.Width / xSkipper_) * (activeScreen.Bounds.Height / ySkipper_);
                        red /= cntr;
                        green /= cntr;
                        blue /= cntr;
                        brightness /= cntr;

                        red *= brightness * emphaser_;
                        green *= brightness * emphaser_;
                        blue *= brightness * emphaser_;
                        rgb = new RGB_Value((byte)red, (byte)green, (byte)blue);
                        // Brightness 0

                        // Brightness 1

                        // Emphaser 0
                        /*if (red > blue && red > green)
                        {
                            rgb = new RGB_Value((byte)(red * (1 + emphaser_)), (byte)(green * (1 - emphaser_)), (byte)(blue * (1 - emphaser_)));
                        } else if (green > red && green > blue)
                        {
                            rgb = new RGB_Value((byte)(red * (1 - emphaser_)), (byte)(green * (1 + emphaser_)), (byte)(blue * (1 - emphaser_)));
                        } else
                        {
                            rgb = new RGB_Value((byte)(red * (1 - emphaser_)), (byte)(green * (1 - emphaser_)), (byte)(blue * (1 + emphaser_)));
                        }*/
                        // Emphaser 1

                        // Average 0
                        // byte rByte = (byte)red;
                        //byte gByte = (byte)green;
                        //byte bByte = (byte)blue;
                        //rgb = new RGB_Value(rByte, gByte, bByte);
                        // Averrage 1
                        _rgbOutput.showRGB(rgb);
                        //_rgbOutput.fade(lastRGB_, rgb, 0);
                        lastRGB_ = rgb;
                        //Thread.Sleep(200);
                    }
                    catch (System.Threading.Tasks.TaskCanceledException)
                    {
                        // Don't do any
                        break;
                    }
                    /*  catch (ArgumentException)
                      {
                         // break;
                      }
                      /*catch (System.ComponentModel.Win32Exception)
                      {

                      }*/
                }
            }
        }
    }
}