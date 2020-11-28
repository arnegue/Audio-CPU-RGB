using System;

namespace AudioCPURGB
{
    /**
     * Small class which contains a name for an Audio-RGB-algorithm and it's method
     */
    abstract class AudioAlgorithm
    {
        private String _name;
        protected const byte colors = 3;

        protected double m;
        private const double b_b2 = 510.0;
        private const double b_g1 = -255.0;
        private const double b_g2 = 765.0;
        private const double b_r = -510.0;

        public AudioAlgorithm(String name)
        {
            _name = name;
            m = 1020.0 / (double)255;
        }

        public override String ToString()
        {
            return _name;
        }

        public abstract void showRGB(byte[] specArray, int min_slider, int max_slider, int min_trigger, bool absNotRel, RGB_Output.RGB_Output_Interface rgbOutput);

        protected byte rel_check(byte b, int min_trigger)
        {
            if (min_trigger == 255)
            {
                min_trigger = 254;
            }
            float x = 255.0F / min_trigger;
            float val = (100 * b) / x;
            return (byte)val;
        }


        /* protected RGB_Value rel_check(RGB_Value rgb, int min_trigger)
         {
             rgb.r = rel_check(rgb.r, min_trigger); // TODO input, not output!
             rgb.r = rel_check(rgb.g, min_trigger);
             rgb.r = rel_check(rgb.b, min_trigger);
             return rgb;
         }

         protected RGB_Value abs_check(RGB_Value rgb, int min_trigger)
         {
             rgb.r = rgb.r < min_trigger ? (byte)0 : rgb.r;
             rgb.g = rgb.g < min_trigger ? (byte)0 : rgb.g;
             rgb.b = rgb.b < min_trigger ? (byte)0 : rgb.b;
             return rgb;
         }*/


        protected RGB_Value valueToRGB(byte value, int minTrigger)
        {
            if (minTrigger == 0)
            {
                minTrigger = 1;
            }

            byte r = 0, g = 0, b = 0;
            if (value < minTrigger / 3)
            {
                b = (byte)(m * value);
            }
            else if (value < (minTrigger * 2) / 3)
            {
                b = (byte)(-m * value + b_b2);
                g = (byte)(m * value + b_g1);
            }
            else if (value < minTrigger)
            {
                g = (byte)(-m * value + b_g2);
                r = (byte)(m * value + b_r);
            }
            else
            {
                r = 255;
            }

            return new RGB_Value(r, g, b);
        }


        /// <summary>
        /// Returns the index of the maximum peak
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        protected int get_maximum_peak_index_range(int left, int right, byte[] specArray)
        {
            int maximumIndex = 0;
            byte maxVal = 0;

            // Get maximum index and its value from current array
            for (int i = left; i < right; i++)
            {
                if (specArray[i] > maxVal)
                {
                    maximumIndex = i;
                    maxVal = specArray[i];
                }
            }

            return maximumIndex;
        }
    }

    /// <summary>
    /// Old RGB-Algorithm (rrr, ggg, bbb)
    /// </summary>
    abstract class OldAudioAlgorithm : AudioAlgorithm
    {
        private RGB_Value _oldRGBValue;
        private RGB_Value _newRGBValue;
        public OldAudioAlgorithm(String name) : base(name)
        {
            _oldRGBValue = new RGB_Value();
            _newRGBValue = new RGB_Value();
        }


        public override void showRGB(byte[] specArray, int min_slider, int max_slider, int min_trigger, bool absNotRel, RGB_Output.RGB_Output_Interface rgbOutput)
        {
            if (min_trigger != 0)
            {
                for (int i = 0; i < specArray.Length; i++)
                {
                    specArray[i] = specArray[i] < min_trigger ? (byte)0 : specArray[i];
                }
                if (!absNotRel)
                {
                    for (int i = 0; i < specArray.Length; i++)
                    {
                        specArray[i] = rel_check(specArray[i], min_trigger);
                    }
                }
            }
            if (min_trigger < 0)
            {
                min_trigger = 1;
            }
            m = 765 / (double)min_trigger;

            _newRGBValue = my_callback(_newRGBValue, specArray, min_slider, max_slider, min_trigger);
            sendRGBValue(rgbOutput);
        }

        protected abstract RGB_Value my_callback(RGB_Value rgb_template, byte[] specArray, int min_slider, int max_slider, int min_trigger);

        private void sendRGBValue(RGB_Output.RGB_Output_Interface rgbOutput)
        {
            if (!_newRGBValue.Equals(_oldRGBValue))
            {
                rgbOutput.ShowRGB(_newRGBValue);

                _oldRGBValue.copy_values(_newRGBValue);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Did not send Value, because it's the same as the old one");
            }
        }
    }

    abstract class NewAudioAlgorithm : AudioAlgorithm
    {
        protected RGB_Value[] rgbs;
        protected int amount_rgbs = -1;
        public NewAudioAlgorithm(String name) : base(name)
        {
        }

        protected abstract RGB_Value[] my_callback(byte[] specArray, int min_slider, int max_slider, int min_trigger);

        public override void showRGB(byte[] specArray, int min_slider, int max_slider, int min_trigger, bool absNotRel, RGB_Output.RGB_Output_Interface rgbOutput)
        {
            if (min_trigger != 0)
            {
                for (int i = 0; i < specArray.Length; i++)
                {
                    specArray[i] = specArray[i] < min_trigger ? (byte)0 : specArray[i];
                }
                if (!absNotRel)
                {
                    for (int i = 0; i < specArray.Length; i++)
                    {
                        specArray[i] = rel_check(specArray[i], min_trigger);
                    }
                }
            }
            // TODO still neded?
            if (min_trigger < 0)
            {
                min_trigger = 1;
            }
            // TODO end
            m = 765 / (double)min_trigger;


            int new_amount_rgbs = rgbOutput.GetAmountRGBs();

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

            rgbs = my_callback(specArray, min_slider, max_slider, min_trigger);
            rgbOutput.ShowRGBs(rgbs);
        }
    }
}
