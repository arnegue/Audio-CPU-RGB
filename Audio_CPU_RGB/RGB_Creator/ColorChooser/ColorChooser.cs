using System.Threading;
using AudioCPURGB.RGBCreator;

namespace AudioCPURGB
{
    class ColorChooser : IRGBCreator
    {
        RGBValue old_rgb = new RGBValue();
        RGBValue new_rgb = new RGBValue();
        Mutex new_rgb_mutex = new Mutex();

        public void rgbChanged(int r, int g, int b)
        {
            new_rgb_mutex.WaitOne();
            new_rgb = new RGBValue((byte)r, (byte)g, (byte)b);
            new_rgb_mutex.ReleaseMutex();
        }

        protected override void Callback()
        {
            RGBValue copy_val = new RGBValue();
            new_rgb_mutex.WaitOne();
            copy_val.CopyValues(new_rgb);
            new_rgb_mutex.ReleaseMutex();

            if (!old_rgb.Equals(copy_val))
            {
                Fade(old_rgb, copy_val, 10);
                old_rgb = copy_val;
            }
            Thread.Sleep(100);
        }
    }
}
