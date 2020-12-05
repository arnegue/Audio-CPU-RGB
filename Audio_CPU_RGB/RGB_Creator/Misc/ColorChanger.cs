using System;
using System.Threading;
using AudioCPURGB.RGBCreator;


namespace AudioCPURGB
{
    /// <summary>
    /// Fades from one random color to another fror every LED (old Protocol)
    /// </summary>
    class ColorChanger : IRGBCreator
    {
        Random random = new Random();
        RGBValue old_rgb = new RGBValue();
        protected override void Callback()
        {
            RGBValue new_rgb = new RGBValue((byte)random.Next(0, 255), (byte)random.Next(0, 255), (byte)random.Next(0, 255));
            Fade(old_rgb, new_rgb, 50);
            old_rgb = new_rgb;
        }
    }
}
