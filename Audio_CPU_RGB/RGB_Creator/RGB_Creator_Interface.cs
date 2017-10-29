using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioCPURGB.RGB_Creator
{
    interface RGB_Creator_Interface
    {
        void setRGBOutput(RGB_Output.RGB_Output_Interface rgbOutput);
        
        void start();

        void pause();
    }
}
