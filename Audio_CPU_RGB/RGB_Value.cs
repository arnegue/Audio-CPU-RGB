namespace AudioCPURGB {
    // TODO: set r,g,b, functioms
    class RGB_Value
    {
        public byte r;
        /*{
            get { return r; }
            set
            {
                if (r > 255)
                {
                    r = 255;
                }
                else if (r < 0)
                {
                    r = 0;
                }
            }
        }*/
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
        // Convert rgb to (rrr,ggg,bbb) with leading 0
     /*   public override string ToString()
        {
            string sendString = "(" + System.Convert.ToChar(r) + "," + System.Convert.ToChar(g) + "," + System.Convert.ToChar(b) + ")" + "\n";

            return sendString;
            //return "(" + ($"{r:D3}") + "," + ($"{g:D3}") + "," + ($"{b:D3}") + ")";
            //return "(" + (char) r + "," + (char) g + "," + (char) b + ")" + "\n";
            //  return sendString;
            //return "(F,F,F)\n";
        }
        */
        public override bool Equals(object obj)
        {
            if (obj.GetType() == this.GetType())
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
