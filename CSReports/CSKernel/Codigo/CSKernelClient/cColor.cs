using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace CSKernelClient
{
    public class cColor
    {
        public static Color colorFromRGB(int rgb)
        {
            if (rgb < 0)
            {
                return Color.FromArgb(rgb);
            }
            else
            {
                byte[] values = BitConverter.GetBytes(rgb);
                if (!BitConverter.IsLittleEndian) Array.Reverse(values);
                return Color.FromArgb(values[2], values[1], values[0]);
            }
        }
    }
}
