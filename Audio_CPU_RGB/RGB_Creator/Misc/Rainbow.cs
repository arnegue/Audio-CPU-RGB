using System;
using System.Threading;
using AudioCPURGB.RGB_Creator;

namespace AudioCPURGB
{
    /// <summary>
    /// Fades from one random color to another of each LED (new Protocol)
    /// </summary>
    class Rainbow : IndividualRGBOutput
    {
        RGB_Value[] _rainbow_rgbs;
        int _phase_length;
        int last_index = 0;
 
        private int[] get_rainbow_values()
        {
            int[] rgb_values = new int[amount_rgbs];
            double slope = 255.0 / ((float)amount_rgbs / 6.0);
            int idx = 0;
            int p;
            for (int i = 0; i < amount_rgbs; i++)
            {
                // Phase 1 getting up
                for (p = 0; p < _phase_length; p++, idx++)
                {
                    rgb_values[idx % amount_rgbs] = (int)(slope * p);
                }
                // Phase 2 stay up
                for (p = 0; p < _phase_length * 2; p++, idx++)
                {
                    rgb_values[idx % amount_rgbs] = 255;
                }
                // Phase 3 getting down
                for (p = 0; p < _phase_length; p++, idx++)
                {
                    rgb_values[idx % amount_rgbs] = rgb_values[_phase_length - p]; // TODO modulo?
                }
                // Phase 4 stay down
                for (p = 0; p < _phase_length * 2; p++, idx++)
                {
                    rgb_values[idx % amount_rgbs] = 0;
                }
            }
            return rgb_values;
        }
        private void get_rainbow_rgbs()
        {
            _phase_length = amount_rgbs / 6;
            if (_phase_length < 1)
            {
                _phase_length = 1;
            }

            int[] rainbow_values = get_rainbow_values();

            _rainbow_rgbs = new RGB_Value[amount_rgbs];
            for (int i = 0; i < amount_rgbs; i++)
            {
                byte r, g, b;
                r = (byte)rainbow_values[(i + _phase_length * 2) % amount_rgbs];
                g = (byte)rainbow_values[i];
                b = (byte)rainbow_values[(i + _phase_length * 4) % amount_rgbs];

                _rainbow_rgbs[i] = new RGB_Value(r, g, b);
            }
        }

        protected override RGB_Value[] callback(RGB_Value[] new_rgbs)
        {
            if (_rainbow_rgbs == null || _rainbow_rgbs.Length != amount_rgbs)
            {
                get_rainbow_rgbs();
            }

            for (int i = 0; i < amount_rgbs; i++)
            {
                new_rgbs[i] = _rainbow_rgbs[(i + last_index) % amount_rgbs];   
                
            }
            last_index++;

            _rgbOutput.fade(old_rgbs, new_rgbs, 20);
            old_rgbs = new_rgbs;

            return new_rgbs;
        }        
    }
}
