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

        void showRGBs(RGB_Value[] rgbs);

        String getName();

        int getAmountRGBs();

        void initialize(String output);

        void shutdown();

        String[] getAvailableOutputList();

        void setEnable(bool enable);

        bool isEnabled();

        void fade(RGB_Value oldValue, RGB_Value newValue, int fade_time_ms);

        void fade(RGB_Value[] oldValues, RGB_Value[] newValues, int fade_time_ms);
    }
}
