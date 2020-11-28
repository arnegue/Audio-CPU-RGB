using System;
using System.Threading;
using AudioCPURGB.RGB_Creator;


namespace AudioCPURGB
{
    /// <summary>
    /// Fades from one random color to another fror every LED (old Protocol)
    /// </summary>
    class ColorChanger : RGB_Creator_Interface
    {
        Random random = new Random();
        RGB_Value old_rgb = new RGB_Value();
        protected override void callback()
        {
            RGB_Value new_rgb = new RGB_Value((byte)random.Next(0, 255), (byte)random.Next(0, 255), (byte)random.Next(0, 255));
            _rgbOutput.Fade(old_rgb, new_rgb, 50);
            old_rgb = new_rgb;
        }
    }
}
