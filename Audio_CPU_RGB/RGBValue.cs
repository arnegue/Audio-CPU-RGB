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
            if (this.GetType() == obj.GetType())
            {
                return StaticEquals(this, (RGBValue) obj);
            } else
            {
                return false;
            }
        }

        /// <summary>
        /// Static Equal check between two given RGBValues
        /// </summary>
        /// <param name="obj_1">First object to compare to</param>
        /// <param name="val_2">Second object to compare to</param>
        /// <returns></returns>
        public static bool StaticEquals(RGBValue obj_1, RGBValue val_2)
        {
            if (obj_1 != null && val_2 != null)
            {
                RGBValue that = (RGBValue)val_2;
                if (obj_1.R == that.R && obj_1.G == that.G && obj_1.B == that.B)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Static Equal check between list of RGBValues
        /// </summary>
        /// <param name="val_1"></param>
        /// <param name="val_2"></param>
        /// <returns></returns>
        public static bool Equals(RGBValue[] val_1, RGBValue[] val_2)
        {
            if (val_1.Length != val_2.Length)
            {
                return false;
            }

            for (int i = 0; i < val_2.Length; i++)
            {
                if (!RGBValue.Equals(val_1[i], val_2[i]))
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
