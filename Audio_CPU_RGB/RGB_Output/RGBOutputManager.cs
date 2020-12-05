using AudioCPURGB.RGBOutput;
using System.Collections.Generic;


namespace AudioCPURGB.RGBCreator
{
    class RGBOutputManager
    {
        private RGBOutput.LogitechLEDSDK.LogitechRGBOutput _mouse_output;
        private RGBOutput.Corsair.CorsairSDKOutput _corsair_output;
        private RGBOutput.BondedOutPut.BondedOutput _bonded_poutput;

        private RGBOutput.Serial.SerialFactory _serial_factory;
        
        public RGBOutputManager()
        {
            _mouse_output = new RGBOutput.LogitechLEDSDK.LogitechRGBOutput();
            _corsair_output = new RGBOutput.Corsair.CorsairSDKOutput();
            _serial_factory = new RGBOutput.Serial.SerialFactory();
            _bonded_poutput = new RGBOutput.BondedOutPut.BondedOutput(this);
        }

        public List<RGBOutput.IRGBOutput> GetAvailableOutputs()
        {
            List<IRGBOutput> _available_interfaces = new List<IRGBOutput>();
            _available_interfaces.Add(_mouse_output);
            _available_interfaces.Add(_corsair_output);
            _available_interfaces.Add(_bonded_poutput);

            _available_interfaces.AddRange(_serial_factory.GetAvailableOutputList());

            return _available_interfaces;
        }
    }    
}
