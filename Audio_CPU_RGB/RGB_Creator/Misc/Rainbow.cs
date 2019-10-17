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
    class Rainbow : RGB_Creator_Interface
    {
        private RGB_Output_Interface _rgbOutput;
        private ManualResetEvent _pauseEvent = new ManualResetEvent(false);

        public Rainbow()
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
            while (true)
            {
                _pauseEvent.WaitOne();
                if (_rgbOutput.isEnabled())
                {
                    int amount_rgbs = _rgbOutput.getAmountRGBs();
                    // create n RGB_Values
                    // set rainbow
                    // walk rainow
                }
            }
        }
    }
}
