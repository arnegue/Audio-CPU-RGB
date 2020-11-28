using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioCPURGB.RGB_Output
{
    interface RGB_Output_Interface
    {
        /// <summary>
        /// Shows given RGB_Value
        /// </summary>
        /// <param name="rgb">RGB-value to show</param>
        void showRGB(RGB_Value rgb);

        /// <summary>
        /// Shows RGBs individually
        /// </summary>
        /// <param name="rgbs">List of RGB-Values to show</param>
        void showRGBs(RGB_Value[] rgbs);

        /// <summary>
        /// Name of Output
        /// </summary>
        /// <returns></returns>
        String getName();

        /// <summary>
        /// Get Amount of individually controllable RGBs
        /// </summary>
        /// <returns></returns>
        int getAmountRGBs();

        /// <summary>
        /// Initializes Output
        /// </summary>
        /// <param name="output">If RGB_outout has SubOutputs, the name is for identification</param>
        void initialize(String output);

        /// <summary>
        /// Stops/Shutdowns current Output
        /// </summary>
        void shutdown();

        /// <summary>
        /// If RGB_outout has SubOutputs, the name is for identification
        /// </summary>
        /// <returns>List of available names</returns>
        String[] getAvailableOutputList();

        /// <summary>
        /// Enables or disables output
        /// </summary>
        /// <param name="enable">boolean to set</param>
        void setEnable(bool enable);

        /// <summary>
        /// Getter for enable
        /// </summary>
        /// <returns>True if enabled, False if not</returns>
        bool isEnabled();

        /// <summary>
        /// Fades from one value to new one
        /// </summary>
        /// <param name="oldValue">Old RGB-Value</param>
        /// <param name="newValue">New RGB-Value to fade to</param>
        /// <param name="fade_time_ms">Time between each fade step</param>
        void fade(RGB_Value oldValue, RGB_Value newValue, int fade_time_ms);

        /// <summary>
        /// Fades from one list of individual RGB-Value to another
        /// </summary>
        /// <param name="oldValues">Old RGB-Values</param>
        /// <param name="newValues">New RGB-Values to fade to</param>
        /// <param name="fade_time_ms">Time between each fade step</param>
        void fade(RGB_Value[] oldValues, RGB_Value[] newValues, int fade_time_ms);
    }
}
