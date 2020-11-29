using AudioCPURGB.RGB_Output;
using System.Collections.Generic;


namespace AudioCPURGB.RGB_Creator
{
    /// <summary>
    /// Class which 
    /// </summary>
    ///       

    class RGBOutputManager
    {

        private RGB_Output.LogitechLEDSDK.Logitech_RGB_Output _mouse_output;
        private RGB_Output.Corsair.CorsairSDKOutput _corsair_output;
        private RGB_Output.Serial.SerialFactory _serial_factory;

        public RGBOutputManager()
        {
            _mouse_output = new RGB_Output.LogitechLEDSDK.Logitech_RGB_Output();
            _corsair_output = new RGB_Output.Corsair.CorsairSDKOutput();
            _serial_factory = new RGB_Output.Serial.SerialFactory();
        }

        public List<RGB_Output.RGB_Output_Interface> GetAvailableOutputs()
        {
            List<RGB_Output_Interface> _available_interfaces = new List<RGB_Output_Interface>();
            _available_interfaces.Add(_mouse_output);
            _available_interfaces.Add(_corsair_output);

            _available_interfaces.AddRange(_serial_factory.GetAvailableOutputList());

            return _available_interfaces;
        }
    }
    
}
