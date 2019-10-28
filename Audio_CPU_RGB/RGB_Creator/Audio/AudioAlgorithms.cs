using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AudioCPURGB.RGB_Output;

namespace AudioCPURGB.RGB_Creator.Audio
{
    class ShowAudioToRGB1 : OldAudioAlgorithm
    {
        public ShowAudioToRGB1() : base("Algorithm 1") { }

        protected override RGB_Value my_callback(RGB_Value rgb_template, byte[] specArray, int min_slider, int max_slider, int min_trigger)
        {
            byte[] rgb = new byte[colors];

            // Now convert these 16 lines (from 0 to 255) to 3 RGB-Colours (from 0 to 255)
            byte right = (byte)(max_slider + 1);
            byte left = (byte)min_slider;

            byte stepsPerColor = (byte)((right - left) / 3);

            for (byte rgbIndex = 0; rgbIndex < colors; rgbIndex++)
            {
                for (int i = left + (stepsPerColor * rgbIndex); i < left + (stepsPerColor * rgbIndex) + stepsPerColor; i++)
                {
                    // Only take values that are higher than that value given by the vertical slider
                    if (specArray[i] > min_trigger)
                    {
                        rgb[rgbIndex] += specArray[i];
                    }
                }

                rgb[rgbIndex] = (byte)(rgb[rgbIndex] / stepsPerColor); // Calculate Average
            }
            rgb_template.set_array(rgb);
            return rgb_template;
        }
    }

    /// <summary>
    /// Takes every value between min and max slider and calculates average for each color range. 
    /// Min and max slider are also min and limit for colour. The limit does not change the area but the colors that are displayed
    /// Sends average to EVERY RGB
    /// </summary>       
    class ShowAudioToRGB2 : OldAudioAlgorithm
    {
        public ShowAudioToRGB2() : base("Algorithm 2") { }

        protected override RGB_Value my_callback(RGB_Value rgb_template, byte[] specArray, int min_slider, int max_slider, int min_trigger)
        {

            byte[] rgb = new byte[colors];

            // Now convert these lines (from 0 to 255) to 3 RGB-Colours (from 0 to 255)
            int right = ((int)max_slider) + 1;
            int left = (int)min_slider;

            int rCount = 0, gCount = 0, bCount = 0;
            for (int i = left; i < right; i++)
            {
                if (i < 5)
                {
                    rgb[0] += specArray[i];
                    rCount++;
                }
                else if (i < 10)
                {
                    rgb[1] += specArray[i];
                    gCount++;
                }
                else
                {
                    rgb[2] += specArray[i];
                    bCount++;
                }
            }
            if (rCount != 0)
            {
                rgb[0] = (byte)(rgb[0] / rCount);
            }
            if (gCount != 0)
            {
                rgb[1] = (byte)(rgb[1] / gCount);
            }
            if (bCount != 0)
            {
                rgb[2] = (byte)(rgb[2] / bCount);
            }

            rgb_template.set_array(rgb);
            return rgb_template;
        }
    }

    class ShowAudioToRGB3 : NewAudioAlgorithm
    {
        int[] bassAvg = new int[2];
        int bass_avg_Index = 0;
        public ShowAudioToRGB3() : base("Algorithm 3")
        {
        }

        protected override RGB_Value[] my_callback(byte[] specArray, int min_slider, int max_slider, int min_trigger)
        {
            int bass = 0;
            int right = ((int)max_slider) + 1;
            int left = (int)min_slider;
            for (int i = left; i < right; i++)
            {
                bass += specArray[i];
                /*  if(specArray[i] > _triggerSlider.Value)
                  {
                      _triggerSlider.Value = specArray[i];
                  }*/
            }
            if (right - left > 0)
            {
                bass /= (right - left);
            }

            bassAvg[bass_avg_Index] = bass;
            bass_avg_Index = (bass_avg_Index + 1) % bassAvg.Length;
            int currBassAvg = 0;
            for (int i = 0; i < bassAvg.Length; i++)
            {
                currBassAvg += bassAvg[i];
            }
            currBassAvg /= bassAvg.Length;
            bass = currBassAvg;


            int leds = 0;
            if (bass > 0 && min_trigger > 0)
            {
                leds = (int)(((double)amount_rgbs * (double)bass) / (double)min_trigger);
            }

            if (bass > min_trigger)
            {
                min_trigger = bass;
                // _triggerSlider.Value = bass;           // TODO      
            }

            RGB_Value emptyRGB = new RGB_Value();
            RGB_Value setRGB = valueToRGB((byte)bass, min_trigger);

            for (int i = 0; i < amount_rgbs; i++)
            {
                if (i <= leds)
                {
                    rgbs[i] = setRGB;
                }
                else
                {
                    rgbs[i] = emptyRGB;
                }
            }
            return rgbs;
        }
    }


    /// <summary>
    /// Takes values between min and max slider. Divides Range into 3 colours range, Gets peak. Peak +- 1 --> Average.
    /// RangeMin and RangeMax are also min and max limit for colors (left = red, right = blue) (like showAudioToRGBA1)
    /// Sends average to EVERY RGB ("old algorithm")
    /// </summary>
    class ShowAudioToRGB4 : OldAudioAlgorithm
    {
        public ShowAudioToRGB4() : base("Algorithm 4") { }
        protected override RGB_Value my_callback(RGB_Value rgb_template, byte[] specArray, int min_slider, int max_slider, int min_trigger)
        {
            bool with_avg = true;

            byte right_max = (byte)(max_slider + 1);
            byte left_max = (byte)min_slider;
            int diff = (right_max - left_max) / colors;
            byte peak_volume; // TODO slider too much to the right -> exception

            byte[] rgb = new byte[colors];

            int left;
            int right;

            for (int i = 0; i < colors; i++)
            {
                left = left_max + (diff * i);
                right = left_max + (diff * (i + 1));

                int maximumIndex = this.get_maximum_peak_index_range(left, right, specArray);

                if (with_avg)
                {
                    // To avoid OutOfBounds
                    if (maximumIndex <= left)
                    {
                        maximumIndex += 1;
                    }
                    else if (maximumIndex >= right)
                    {
                        maximumIndex -= 1;
                    }
                    // TODO maybe too much casting right now
                    peak_volume = (byte)((int)((int)specArray[maximumIndex - 1] + (int)specArray[maximumIndex] + (int)specArray[maximumIndex + 1]) / 3);
                }
                else
                {
                    peak_volume = specArray[maximumIndex];
                }
                if (peak_volume < min_slider)
                {
                    peak_volume = 0;
                }

                rgb[i] = peak_volume;
            }

            rgb_template.set_array(rgb);
            return rgb_template;
        }
    }

    /// <summary>
    /// Takes values between min and max slider. Divides Range into 3 colours range, Gets peak. Peak +- 1 --> Average.
    /// Min and max slider are also min and limit for colour. The limit does not change the area but the colors that are displayed (like showAudioToRGBA2)
    /// Sends average to EVERY RGB ("old algorithm")
    /// </summary>
    class ShowAudioToRGB5 : OldAudioAlgorithm
    {
        bool with_avg = true;
        public ShowAudioToRGB5() : base("Algorithm 5") { }
        protected override RGB_Value my_callback(RGB_Value rgb_template, byte[] specArray, int min_slider, int max_slider, int min_trigger)
        {
            byte right_max = (byte)(max_slider + 1);
            byte left_max = (byte)min_slider;
            int diff = 5; // Slidermax / colours ;  TODO change in code! // (right_max - left_max) / colors;
            byte peak_volume;

            byte[] rgb = new byte[colors];

            int left, search_left;
            int right, search_right;

            for (int i = 0; i < colors; i++)
            {
                left = left_max + (diff * i);
                right = left_max + (diff * (i + 1));

                if (left < left_max)
                {
                    search_left = left_max;
                }
                else
                {
                    search_left = left;
                }
                if (right > right_max)
                {
                    search_right = right_max;
                }
                else
                {
                    search_right = right;
                }

                int maximumIndex = this.get_maximum_peak_index_range(search_left, search_right, specArray);

                if (maximumIndex < left || maximumIndex > right)
                {
                    rgb[i] = 0;
                }


                if (with_avg)
                {
                    // To avoid OutOfBounds
                    if (maximumIndex <= left)
                    {
                        maximumIndex += 1;
                    }
                    else if (maximumIndex >= right)
                    {
                        maximumIndex -= 1;
                    }
                    // TODO maybe too much casting right now
                    peak_volume = (byte)((int)((int)specArray[maximumIndex - 1] + (int)specArray[maximumIndex] + (int)specArray[maximumIndex + 1]) / 3);
                }
                else
                {
                    peak_volume = specArray[maximumIndex];
                }
                if (peak_volume < min_trigger)
                {
                    peak_volume = 0;
                }

                rgb[i] = peak_volume;
            }


            rgb_template.set_array(rgb);
            return rgb_template;
        }
    }
    class ShowAudioToRGB6 : NewAudioAlgorithm
    {
        int[] bassAvg = new int[2];
        int bass_avg_Index = 0;

        public ShowAudioToRGB6() : base("Algorithm 6") { }

        protected override RGB_Value[] my_callback(byte[] specArray, int min_slider, int max_slider, int min_trigger) // TODO TRIGGER!?
        {
            int left = (int)min_slider;
            int right = ((int)max_slider) + 1;
            int bass = 0;
            for (int i = left; i < right; i++)
            {
                bass += specArray[i];
            }
            if (right - left > 0)
            {
                bass /= (right - left);
            }

            bassAvg[bass_avg_Index] = bass;
            bass_avg_Index = (bass_avg_Index + 1) % bassAvg.Length;
            int currBassAvg = 0;
            for (int i = 0; i < bassAvg.Length; i++)
            {
                currBassAvg += bassAvg[i];
            }
            currBassAvg /= bassAvg.Length;
            bass = currBassAvg;

            int leds = 0;
            if (bass > 0 && min_trigger > 0)
            {
                leds = (int)(((float)(amount_rgbs * bass)) / ((float)min_trigger));
            }

            /* if (bass > min_trigger) // TODO this is not possible anymore to automatically adjust trigger value
             {
                 min_trigger = bass;
                 _triggerSlider.Value = bass;
             }
             */
            RGB_Value emptyRGB = new RGB_Value();
            RGB_Value setRGB = valueToRGB((byte)bass, min_trigger);


            for (int i = 0; i < rgbs.Length; i++)
            {
                if (i <= leds)
                {
                    rgbs[i] = setRGB;
                }
                else
                {
                    rgbs[i] = emptyRGB;
                }
            }
            return rgbs;
        }
    }
}
