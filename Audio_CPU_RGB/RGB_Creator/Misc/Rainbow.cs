using System;
using System.Threading;
using AudioCPURGB.RGB_Creator;

namespace AudioCPURGB
{
    /// <summary>
    /// Fades from one random color to another of each LED (new Protocol)
    /// </summary>
    class Rainbow : RGB_Creator_Interface
    {
        RGB_Value[] old_rgbs = null;
        int amount_rgbs = 0;

        Random random = new Random();

        private RGB_Value[] create_new_rgbs(int amount_rgbs)
        {
            RGB_Value[] new_rgbs = new RGB_Value[amount_rgbs];
            for (int led = 0; led < amount_rgbs; led++)
            {
                new_rgbs[led] = new RGB_Value();
            }
            return new_rgbs;
        }

        protected override void callback()
        {
            int new_amount_rgbs = _rgbOutput.getAmountRGBs(); // TODO this is used in new_audio-agortihm too
            // In case the amount of RGBs changed create new array (usually if output changes)
            if (old_rgbs == null || amount_rgbs != new_amount_rgbs)
            {
                old_rgbs = create_new_rgbs(new_amount_rgbs);
                amount_rgbs = new_amount_rgbs;
            }
            RGB_Value[] newRGBs = create_new_rgbs(new_amount_rgbs);
            
            for (int i = 0; i < old_rgbs.Length; i++)
            {
                newRGBs[i].copy_values(new RGB_Value((byte)random.Next(0, 255), (byte)random.Next(0, 255), (byte)random.Next(0, 255)));
            }
            _rgbOutput.fade(old_rgbs, newRGBs, 0);
            Thread.Sleep(250);
        }        
    }
}
