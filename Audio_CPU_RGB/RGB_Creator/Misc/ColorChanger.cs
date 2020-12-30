using System;
using AudioCPURGB.RGBCreator;


namespace AudioCPURGB
{
    /// <summary>
    /// Fades from one random color to another fror every LED (old Protocol)
    /// </summary>
    class ColorChanger : SingleRGBCreator
    {
        Random random = new Random();
        protected override void Callback()
        {
            RGBValue new_rgb = new RGBValue((byte)random.Next(0, 255), (byte)random.Next(0, 255), (byte)random.Next(0, 255));
            Fade(lastRGB_, new_rgb, 50);
            lastRGB_ = new_rgb;
        }
    }
}
