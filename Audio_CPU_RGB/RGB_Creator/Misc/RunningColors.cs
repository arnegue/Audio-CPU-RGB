using System;
using AudioCPURGB.RGBCreator;
using System.Threading;

namespace AudioCPURGB
{
    class RunningColors : IndividualRGBCreator
    {
        private RGBValue filled_rgb;
        private RGBValue middle_rgb;
        private RGBValue diff_rgb;

        private int last_rgb_index;
        private int direction = 1;
        private Random random = new Random();

        public override void Start()
        {
            filled_rgb = new RGBValue((byte)random.Next(0, 255), (byte)random.Next(0, 255), (byte)random.Next(0, 255));
            middle_rgb = new RGBValue((byte)random.Next(0, 255), (byte)random.Next(0, 255), (byte)random.Next(0, 255));

            byte diff_r = (byte)(Math.Abs((int)filled_rgb.R - (int)middle_rgb.R) / 2);
            byte diff_g = (byte)(Math.Abs((int)filled_rgb.G - (int)middle_rgb.G) / 2);
            byte diff_b = (byte)(Math.Abs((int)filled_rgb.B - (int)middle_rgb.B) / 2);

            diff_rgb = new RGBValue(diff_r, diff_g, diff_b);

            base.Start();
        }

        protected override RGBValue[] Callback(RGBValue[] new_rgbs)
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
