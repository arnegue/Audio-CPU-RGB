using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using AudioCPURGB.RGBOutput;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using AudioCPURGB.RGBCreator;

namespace AudioCPURGB
{
    /// <summary>
    /// Takes screenshot of screen, analyzes color-average and show (emphasized) colors to RGB
    /// </summary>
    class ScreenAnalyzer : SingleRGBCreator
    {
        private readonly Screen activeScreen = Screen.PrimaryScreen;

        private int xSkipper_;
        private int ySkipper_;
        private int xStart_;
        private int yStart_;
        private int xStop_;
        private int yStop_;

        private Bitmap memoryImage;
        private Size s;//= new Size(memoryImage.Width, memoryImage.Height);
        private Graphics memoryGraphics;

        private float emphaser_;

        private Mutex varMutex = new Mutex();

        public ScreenAnalyzer()
        {

            emphaser_ = 1.0F;
            xSkipper_ = 100;
            ySkipper_ = 100;
            xStart_ = 0;
            yStart_ = 0;
            xStop_ = activeScreen.Bounds.Width;
            yStop_ = activeScreen.Bounds.Height;
            memoryImage = new Bitmap(activeScreen.Bounds.Width, activeScreen.Bounds.Height);
            s = new Size(memoryImage.Width, memoryImage.Height);
        }
        public int XSkipper
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

        public int YSkipper
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

        public int XStart
        {
            set
            {
                if (value < 0)
                {
                    value = 0;
                }
                else if (value > XStop)
                {
                    xStart_ = XStop - 1;
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

        public int XStop
        {
            set
            {
                if (value < XStart)
                {
                    value = XStart + 1;
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
        public int YStart
        {
            set
            {
                if (value < 0)
                {
                    value = 0;
                }
                else if (value > YStop)
                {
                    value = YStop - 1;
                }
                yStart_ = value;
            }
            get
            {
                return yStart_;
            }
        }

        public int YStop
        {
            set
            {
                if (value < YStart)
                {
                    value = YStart + 1;
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

        public void SetEmphaser(float value)
        {
            if (value < 0)
            {
                value = 0;
            }
            else if (value > 1)
            {
                value = 1;
            }
            emphaser_ = 1 + value;
        }

        protected override void Callback()
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
            red /= cntr;
            green /= cntr;
            blue /= cntr;
            brightness /= cntr;

            red *= brightness * emphaser_;
            green *= brightness * emphaser_;
            blue *= brightness * emphaser_;

            lastRGB_.Set(red, green, blue);
            _rgbOutput.ShowRGB(lastRGB_);
        
          //  Thread.Sleep(20);                
        }

        public new void Dispose()
        {
            base.Dispose();
            memoryImage.Dispose();
            varMutex.Dispose();
        }
    }
}