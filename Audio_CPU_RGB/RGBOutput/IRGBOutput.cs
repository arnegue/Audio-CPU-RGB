using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioCPURGB.RGBOutput
{
    public interface IRGBOutput
    {
        /// <summary>
        /// Shows given RGB_Value
        /// </summary>
        /// <param name="rgb">RGB-value to show</param>
        void ShowRGB(RGBValue rgb);

        /// <summary>
        /// Shows RGBs individually
        /// </summary>
        /// <param name="rgbs">List of RGB-Values to show</param>
        void ShowRGBs(RGBValue[] rgbs);

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

    [Serializable()]
    public class RGBOutputException : Exception
    {
        public RGBOutputException()
        {
            // Add any type-specific logic, and supply the default message.
        }

        public RGBOutputException(string message) : base(message)
        {
            // Add any type-specific logic.
        }

        public RGBOutputException(string message, Exception innerException) :
           base(message, innerException)
        {
            // Add any type-specific logic for inner exceptions.
        }

        protected RGBOutputException(System.Runtime.Serialization.SerializationInfo info,
           System.Runtime.Serialization.StreamingContext context) : base(info, context)
        {
            // Implement type-specific serialization constructor logic.
        }
    }

    [Serializable()]
    public class InitializationException : RGBOutputException
    {
        public InitializationException()
        {
            // Add any type-specific logic, and supply the default message.
        }

        public InitializationException(string message) : base(message)
        {
            // Add any type-specific logic.
        }

        public InitializationException(string message, Exception innerException) :
           base(message, innerException)
        {
            // Add any type-specific logic for inner exceptions.
        }

        protected InitializationException(System.Runtime.Serialization.SerializationInfo info,
           System.Runtime.Serialization.StreamingContext context) : base(info, context)
        {
            // Implement type-specific serialization constructor logic.
        }
    }
}
