using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioCPURGB
{
    interface LEDStripInterface
    {
        void sendToSerial(RGBValue rgbValue);
    }
}
