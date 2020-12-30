using System.Threading;
using AudioCPURGB.RGBCreator;

namespace AudioCPURGB
{
    class ColorChooser : SingleRGBCreator
    {
        readonly RGBValue new_rgb = new RGBValue();
        readonly Mutex newRGBMutex = new Mutex(); // mutex ensuring correct exclusion between GUI- and RGBCreator-Thread

        public void rgbChanged(int r, int g, int b)
        {
            newRGBMutex.WaitOne();
            new_rgb.Set(r, g, b);
            newRGBMutex.ReleaseMutex();
        }

        protected override void Callback()
        {
            RGBValue copy_val = new RGBValue();
            newRGBMutex.WaitOne();
            copy_val.CopyValues(new_rgb);
            newRGBMutex.ReleaseMutex();

            if (!lastRGB_.Equals(copy_val))
            {
                Fade(lastRGB_, copy_val, 10);
                lastRGB_ = copy_val;
            }
            Thread.Sleep(100);
        }
    }
}
