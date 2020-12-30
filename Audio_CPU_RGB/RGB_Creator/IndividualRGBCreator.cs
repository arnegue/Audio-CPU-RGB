using System;

namespace AudioCPURGB.RGBCreator
{
    /// <summary>
    /// Creator class for individual RGBs (New Algorithm)
    /// </summary>
    abstract class IndividualRGBCreator : IRGBCreator
    {
        protected RGBValue[] old_rgbs;
        protected int amount_rgbs;
        protected int minimum_amount_rgbs = 1;

        static private RGBValue[] CreateNewRGBs(int amount_rgbs)
        {
            RGBValue[] new_rgbs = new RGBValue[amount_rgbs];
            for (int led = 0; led < amount_rgbs; led++)
            {
                new_rgbs[led] = new RGBValue();
            }
            return new_rgbs;
        }

        public override void Start()
        {
            int new_amount_rgbs = _rgbOutput.GetAmountRGBs();
            if (new_amount_rgbs < minimum_amount_rgbs)
            {
                throw new AudioCPURGB.RGBCreator.InitializationException($"{this.GetType().Name} requires a minimum length of {minimum_amount_rgbs} RGBs, but current output only supports {new_amount_rgbs} RGBs.");
            }
            if (old_rgbs == null || amount_rgbs != new_amount_rgbs)
            {
                amount_rgbs = new_amount_rgbs;
                old_rgbs = CreateNewRGBs(amount_rgbs); // Reset old ones
            }
            base.Start();
        }

        protected override void Callback()
        {
            RGBValue[] new_values = CreateNewRGBs(amount_rgbs); // Create template for callback
            old_rgbs = Callback(new_values);
        }

        protected abstract RGBValue[] Callback(RGBValue[] new_rgbs);
    }
}
