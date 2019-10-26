using System;
using System.Threading;
using AudioCPURGB.RGB_Creator;

namespace AudioCPURGB
{
    class RunningColorChangingDot : RGB_Creator_Interface
    {
        RGB_Value[] rgbs = null;
        int amount_rgbs = 0;
        RGB_Value empty_rgb = new RGB_Value();
        int last_rgb_index = 0;
        bool direction = true;
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

            // Show random dot at last_rgb_index
            for (int i = 0; i < rgbs.Length; i++)
            {
                if (i == last_rgb_index)
                {
                    rgbs[i].copy_values(new RGB_Value((byte)random.Next(0, 255), (byte)random.Next(0, 255), (byte)random.Next(0, 255)));
                }
                else
                {
                    rgbs[i].copy_values(empty_rgb);
                }
            }

            // calculate next last_rgb_index
            if (direction)
            {
                last_rgb_index++;
            }
            else
            {
                last_rgb_index--;
            }
            // Look if direction as to change
            if (last_rgb_index % amount_rgbs == 0)
            {
                direction = !direction;
            }

            // Finally show them
            _rgbOutput.showRGBs(rgbs);
            Thread.Sleep(100);
        }        
    }
}
