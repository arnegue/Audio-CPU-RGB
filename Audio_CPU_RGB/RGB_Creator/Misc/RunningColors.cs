using System;
using AudioCPURGB.RGB_Creator;
using System.Threading;

namespace AudioCPURGB
{
    class RunningColors : IndividualRGBOutput
    {
        RGB_Value filled_rgb;
        RGB_Value middle_rgb;
        RGB_Value diff_rgb;

        int last_rgb_index = 0;
        int direction = 1;
        Random random = new Random();

        public override void start()
        {
            filled_rgb = new RGB_Value((byte)random.Next(0, 255), (byte)random.Next(0, 255), (byte)random.Next(0, 255));
            middle_rgb = new RGB_Value((byte)random.Next(0, 255), (byte)random.Next(0, 255), (byte)random.Next(0, 255));

            byte diff_r = (byte)(Math.Abs((int)filled_rgb.r - (int)middle_rgb.r) / 2);
            byte diff_g = (byte)(Math.Abs((int)filled_rgb.g - (int)middle_rgb.g) / 2);
            byte diff_b = (byte)(Math.Abs((int)filled_rgb.b - (int)middle_rgb.b) / 2);

            diff_rgb = new RGB_Value(diff_r, diff_g, diff_b);

            base.start();
        }

        protected override RGB_Value[] callback(RGB_Value[] new_rgbs)
        {
            // Show random dot at last_rgb_index
            int before_index = (last_rgb_index - 1) % amount_rgbs;
            int after_index = (last_rgb_index + 1) % amount_rgbs;
            for (int i = 0; i < new_rgbs.Length; i++)
            {
                if (i == before_index)
                {
                    new_rgbs[i] = diff_rgb;
                }
                else if (i == last_rgb_index)
                {
                    new_rgbs[i] = middle_rgb;
                }
                else if (i == after_index)
                {
                    new_rgbs[i] = diff_rgb;
                }
                else
                {
                    new_rgbs[i] = filled_rgb;
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
