namespace AudioCPURGB {
    class RGB_Value
    {
        public int r;
        public int g;
        public int b;

        public RGB_Value()
        {
            r = 0;
            g = 0;
            b = 0;
        }

        public RGB_Value(int r, int g, int b)
        {
            if (r > 255)
            {
                r = 255;
            }
            else if (r < 0)
            {
                r = 0;
            }
            if (g > 255)
            {
                g = 255;
            }
            else if (g < 0)
            {
                g = 0;
            }
            if (b > 255)
            {
                b = 255;
            }
            else if (b < 0)
            {
                b = 0;
            }
            this.r = r;
            this.g = g;
            this.b = b;
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
