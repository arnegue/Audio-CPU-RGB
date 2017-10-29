using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioCPURGB.RGB_Output
{
    interface RGB_Output_Interface
    {
        void showRGB(RGB_Value rgb);

        void initialize(String param);
        void shutdown();
    }
}
