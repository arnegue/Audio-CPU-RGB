using System;
using System.Threading;
using AudioCPURGB.RGBCreator;

namespace AudioCPURGB
{
    /// <summary>
    /// Lets one dot with random Colour run around the LED-Strip
    /// </summary>
    class RunningColorChangingDot : IndividualRGBCreator
    {
        private RGBValue empty_rgb = new RGBValue();
        private int last_rgb_index;
        private int direction = 1;
        private Random random = new Random();

        protected override RGBValue[] callback(RGBValue[] new_rgbs)
        {           
            // Show random dot at last_rgb_index
            for (int i = 0; i < new_rgbs.Length; i++)
            {
                if (i == last_rgb_index)
                {
                    new_rgbs[i] = new RGBValue((byte)random.Next(0, 255), (byte)random.Next(0, 255), (byte)random.Next(0, 255));
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
            _rgbOutput.ShowRGBs(new_rgbs);
            Thread.Sleep(100);
            return new_rgbs;
        }        
    }
}
