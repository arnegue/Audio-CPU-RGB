using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    class ColorChooser : RGB_Creator_Interface
    {
        private RGB_Output_Interface _rgbOutput;


        public void rgbChanged(int r, int g, int b)
        {
            RGB_Value rgb = new RGB_Value((byte)r, (byte)g, (byte)b);
            //String write =
            System.Diagnostics.Debug.Print("New RGB: " + rgb.r + ", " + rgb.g + ", " + rgb.b);
            _rgbOutput.showRGB(rgb);
        }

        public void start()
        {
            //_pauseEvent.Set();
        }

        public void pause()
        {
           // _pauseEvent.Reset();
        }

        public void setRGBOutput(RGB_Output_Interface rgbOutput)
        {
            _rgbOutput = rgbOutput;
        }

    }
}
