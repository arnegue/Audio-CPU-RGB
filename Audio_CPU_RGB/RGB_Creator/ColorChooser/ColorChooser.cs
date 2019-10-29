using System.Threading;
using AudioCPURGB.RGB_Creator;

namespace AudioCPURGB
{
    class ColorChooser : RGB_Creator_Interface
    {
        RGB_Value old_rgb = new RGB_Value();
        RGB_Value new_rgb = new RGB_Value();
        Mutex new_rgb_mutex = new Mutex();

        public void rgbChanged(int r, int g, int b)
        {
            new_rgb_mutex.WaitOne();
            new_rgb = new RGB_Value((byte)r, (byte)g, (byte)b);
            new_rgb_mutex.ReleaseMutex();
        }

        protected override void callback()
        {
            RGB_Value copy_val = new RGB_Value();
            new_rgb_mutex.WaitOne();
            copy_val.copy_values(new_rgb);
            new_rgb_mutex.ReleaseMutex();

            _rgbOutput.fade(old_rgb, copy_val, 10);
            old_rgb = copy_val;
            Thread.Sleep(100);
        }
    }
}
