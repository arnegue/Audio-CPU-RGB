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
        Random random = new Random();

        protected override RGB_Value[] callback(RGB_Value[] new_rgbs)
        {           
            for (int i = 0; i < old_rgbs.Length; i++)
            {
                new_rgbs[i].copy_values(new RGB_Value((byte)random.Next(0, 255), (byte)random.Next(0, 255), (byte)random.Next(0, 255)));
            }
            _rgbOutput.fade(old_rgbs, new_rgbs, 5);
            Thread.Sleep(250);

            return new_rgbs;
        }        
    }
}
