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

        public void rgbChanged(int r, int g, int b)
        {
            RGB_Value rgb = new RGB_Value((byte)r, (byte)g, (byte)b);
            //String write =
            System.Diagnostics.Debug.Print("New RGB: " + rgb.r + ", " + rgb.g + ", " + rgb.b);
            _rgbOutput.showRGB(rgb);
        }

        protected override void callback()
        {

        }
    }
}
