namespace AudioCPURGB {
    public class RGBValue
    {
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }

        public RGBValue()
        {
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

        public void Set(int rn, int gn, int bn)
        {
            this.R = (byte)rn;
            this.G = (byte)gn;
            this.B = (byte)bn;
        }

        public void Set(double rn, double gn, double bn)
        {
            this.R = (byte)rn;
            this.G = (byte)gn;
            this.B = (byte)bn;
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

        /// <summary>
        /// Instance Equal check between intance and given object
        /// </summary>
        /// <param name="obj">Object to compare to</param>
        /// <returns>true if equal</returns>
        public override bool Equals(object obj)
        {
            if (obj == null || this.GetType() != obj.GetType())
            {
                return false;
            } else
            {
                return StaticEquals(this, (RGBValue)obj);
            }
        }

        /// <summary>
        /// Static Equal check between two given RGBValues
        /// </summary>
        /// <param name="val1">First object to compare to</param>
        /// <param name="val2">Second object to compare to</param>
        /// <returns></returns>
        public static bool StaticEquals(RGBValue val1, RGBValue val2)
        {
            if (val1 != null && val2 != null)
            {
                RGBValue that = (RGBValue)val2;
                if (val1.R == that.R && val1.G == that.G && val1.B == that.B)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Static Equal check between list of RGBValues
        /// </summary>
        /// <param name="val1"></param>
        /// <param name="val2"></param>
        /// <returns></returns>
        public static bool Equals(RGBValue[] val1, RGBValue[] val2)
        {
            if (val1 == null || val2 == null || val1.Length != val2.Length)
            {
                return false;
            }

            for (int i = 0; i < val2.Length; i++)
            {
                if (!RGBValue.StaticEquals(val1[i], val2[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public override int GetHashCode()
        {
            return R + G + B;
        }
    }
}
