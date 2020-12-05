using System;

namespace AudioCPURGB.RGB_Creator
{
    /// <summary>
    /// Wraps creation of new RGBArrays.
    /// </summary>
    abstract class IndividualRGBOutput : RGB_Creator_Interface
    {
        protected RGB_Value[] old_rgbs;
        protected int amount_rgbs;
        protected int minimum_amount_rgbs = 1;

        private RGB_Value[] create_new_rgbs(int amount_rgbs)
        {
            RGB_Value[] new_rgbs = new RGB_Value[amount_rgbs];
            for (int led = 0; led < amount_rgbs; led++)
            {
                new_rgbs[led] = new RGB_Value();
            }
            return new_rgbs;
        }

        public override void start()
        {
            int new_amount_rgbs = _rgbOutput.GetAmountRGBs();
            if (new_amount_rgbs < minimum_amount_rgbs)
            {
                throw new Exception($"{this.GetType().Name} requires a minimum length of {minimum_amount_rgbs} RGBs, but current output only supports {new_amount_rgbs} RGBs.");
            }
            if (old_rgbs == null || amount_rgbs != new_amount_rgbs)
            {
                amount_rgbs = new_amount_rgbs;
                old_rgbs = create_new_rgbs(amount_rgbs); // Reset old ones
            }
            base.start();
        }

        protected override void callback()
        {
            RGB_Value[] new_values = create_new_rgbs(amount_rgbs); // Create template for callback
            old_rgbs = callback(new_values);
        }

        protected abstract RGB_Value[] callback(RGB_Value[] new_rgbs);
    }
}
