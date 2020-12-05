namespace AudioCPURGB {
    public class RGBValue
    {
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }

        public RGBValue()
        {
            R = 0;
            G = 0;
            B = 0;
        }

        public RGBValue(byte rn, byte gn, byte bn)
        {
            Set(rn, gn, bn);
        }

        public void SetArray(byte[] rgbArray)
        {
            if (rgbArray != null)
            {
                Set(rgbArray[0], rgbArray[1], rgbArray[2]);
            }
        }

        public void Set(byte rn, byte gn, byte bn)
        {         
            this.R = rn;
            this.G = gn;
            this.B = bn;
        }

        public void CopyValues(RGBValue rgb)
        {
            if (rgb != null)
            {
                this.R = rgb.R;
                this.G = rgb.G;
                this.B = rgb.B;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj.GetType() == this.GetType())
            {
                RGBValue that = (RGBValue)obj;
                if (this.R == that.R && this.G == that.G && this.B == that.B)
                {
                    return true;
                }
            }
            return false;
        }

        public override int GetHashCode()
        {
            return R + G + B;
        }

    }
}
