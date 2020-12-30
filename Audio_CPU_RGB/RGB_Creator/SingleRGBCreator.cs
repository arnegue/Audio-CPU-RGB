namespace AudioCPURGB.RGBCreator
{   
    /// <summary>
    /// Creator class for Single RGBs (Old Algorithm)
    /// </summary>
    abstract class SingleRGBCreator : IRGBCreator
    {
        protected RGBValue lastRGB_ = new RGBValue();

        /// <summary>
        /// Resets RGB on Start
        /// </summary>
        public override void Reset()
        {
            lastRGB_.Set(0, 0, 0);
            _rgbOutput.ShowRGB(lastRGB_);
        }
    }
}
