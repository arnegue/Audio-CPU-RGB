using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioCPURGB.RGB_Output
{
    public interface RGB_Output_Interface
    {
        /// <summary>
        /// Shows given RGB_Value
        /// </summary>
        /// <param name="rgb">RGB-value to show</param>
        void ShowRGB(RGB_Value rgb);

        /// <summary>
        /// Shows RGBs individually
        /// </summary>
        /// <param name="rgbs">List of RGB-Values to show</param>
        void ShowRGBs(RGB_Value[] rgbs);

        /// <summary>
        /// Name of Output
        /// </summary>
        /// <returns></returns>
        String GetName();

        /// <summary>
        /// Get Amount of individually controllable RGBs
        /// </summary>
        /// <returns></returns>
        int GetAmountRGBs();

        /// <summary>
        /// Initializes Output
        /// </summary>
        void Initialize();

        /// <summary>
        /// Stops/Shutdowns current Output
        /// </summary>
        void Shutdown();

        /// <summary>
        /// Enables or disables output
        /// </summary>
        /// <param name="enable">boolean to set</param>
        void SetEnable(bool enable);

        /// <summary>
        /// Getter for enable
        /// </summary>
        /// <returns>True if enabled, False if not</returns>
        bool IsEnabled();
    }
}
