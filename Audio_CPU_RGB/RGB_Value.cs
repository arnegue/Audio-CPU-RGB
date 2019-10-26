namespace AudioCPURGB {
    class RGB_Value
    {
        public byte r;
       
        public byte g;
        public byte b;

        public RGB_Value()
        {
            r = 0;
            g = 0;
            b = 0;
        }

        public RGB_Value(byte rn, byte gn, byte bn)
        {
            set(rn, gn, bn);
        }

        public void set_array(byte[] rgb_array)
        {
            set(rgb_array[0], rgb_array[1], rgb_array[2]);
        }

        public void set(byte rn, byte gn, byte bn)
        {
            if (rn > 255)
            {
                r = 255;
            }
            else if (rn < 0)
            {
                r = 0;
            }
            if (gn > 255)
            {
                g = 255;
            }
            else if (gn < 0)
            {
                g = 0;
            }
            if (bn > 255)
            {
                b = 255;
            }
            else if (bn < 0)
            {
                b = 0;
            }
            this.r = rn;
            this.g = gn;
            this.b = bn;
        }

        public void copy_values(RGB_Value rgb)
        {
            this.r = rgb.r;
            this.g = rgb.g;
            this.b = rgb.b;
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj.GetType() == this.GetType())
            {
                RGB_Value that = (RGB_Value)obj;
                if (this.r == that.r && this.g == that.g && this.b == that.b)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
