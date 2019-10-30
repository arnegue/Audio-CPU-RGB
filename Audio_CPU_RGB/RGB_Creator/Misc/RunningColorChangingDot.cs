using System;
using System.Threading;
using AudioCPURGB.RGB_Creator;

namespace AudioCPURGB
{
    /// <summary>
    /// Lets one dot with random Colour run around the LED-Strip
    /// </summary>
    class RunningColorChangingDot : IndividualRGBOutput
    {      
        RGB_Value empty_rgb = new RGB_Value();
        int last_rgb_index = 0;
        int direction = 1;
        Random random = new Random();

        protected override RGB_Value[] callback(RGB_Value[] new_rgbs)
        {           
            // Show random dot at last_rgb_index
            for (int i = 0; i < new_rgbs.Length; i++)
            {
                if (i == last_rgb_index)
                {
                    new_rgbs[i] = new RGB_Value((byte)random.Next(0, 255), (byte)random.Next(0, 255), (byte)random.Next(0, 255));
                }
                else
                {
                    new_rgbs[i] =  empty_rgb;
                }
            }
            
            last_rgb_index += direction;
          
            // Look if direction has to change
            if (last_rgb_index % amount_rgbs == 0)
            {
                direction *= -1;
            }

            // Finally show them
            _rgbOutput.showRGBs(new_rgbs);
            Thread.Sleep(100);
            return new_rgbs;
        }        
    }
}
