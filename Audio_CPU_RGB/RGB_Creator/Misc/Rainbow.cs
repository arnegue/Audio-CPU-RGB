using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AudioCPURGB.RGB_Output;
using System.Threading;
using AudioCPURGB.RGB_Creator;

namespace AudioCPURGB
{
    class Rainbow : RGB_Creator_Interface
    {
        RGB_Value[] rgbs = null;
        int amount_rgbs = 0;
        RGB_Value empty_rgb = new RGB_Value();

        Random random = new Random();

        protected override void callback()
        {
            int new_amount_rgbs = _rgbOutput.getAmountRGBs(); // TODO this is used in new_audio-agortihm too
            // In case the amount of RGBs changed create new array (usually if output changes)
            if (rgbs == null || amount_rgbs != new_amount_rgbs)
            {
                rgbs = new RGB_Value[new_amount_rgbs];
                for (int led = 0; led < new_amount_rgbs; led++)
                {
                    rgbs[led] = new RGB_Value();
                }
                amount_rgbs = new_amount_rgbs;
            }
            
            for (int i = 0; i < rgbs.Length; i++)
            {
                // TODO fade somehow?
                rgbs[i].copy_values(new RGB_Value((byte)random.Next(0, 255), (byte)random.Next(0, 255), (byte)random.Next(0, 255)));                  
            }
            _rgbOutput.showRGBs(rgbs);
            Thread.Sleep(250);
        }
    }
}
