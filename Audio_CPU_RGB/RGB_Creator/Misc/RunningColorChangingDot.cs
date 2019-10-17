using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AudioCPURGB.RGB_Output;
using System.Threading;
using AudioCPURGB.RGB_Creator;

namespace AudioCPURGB
{
    class RunningColorChangingDot : RGB_Creator_Interface
    {
        private RGB_Output_Interface _rgbOutput;
        private ManualResetEvent _pauseEvent = new ManualResetEvent(false);

        public RunningColorChangingDot()
        {
            _pauseEvent.Reset(); // Don't let the thread run
        }

        public void pause()
        {
            throw new NotImplementedException();
        }

        public void setRGBOutput(RGB_Output_Interface rgbOutput)
        {
            _rgbOutput = rgbOutput;
        }

        public void start()
        {
            RGB_Value[] rgbs = null;
            RGB_Value empty_rgb = new RGB_Value();
            Random random = new Random();

            int last_rgb_index = 0;
            while (true)
            {             
                _pauseEvent.WaitOne();
                if (_rgbOutput.isEnabled())
                {
                    if (rgbs == null)
                    {
                        rgbs = new RGB_Value[_rgbOutput.getAmountRGBs()];
                    }  

                    for (int i = 0; i < rgbs.Length; i++)
                    {
                        if(i == last_rgb_index)
                        {
                            // create a random shiny dot
                            rgbs[i].copy_values(new RGB_Value((byte)random.Next(0, 255), (byte)random.Next(0, 255), (byte)random.Next(0, 255)));
                        }
                        rgbs[i].copy_values(empty_rgb);
                    }
                    int amount_rgbs = _rgbOutput.getAmountRGBs();


                }
            }
        }
    }
}
