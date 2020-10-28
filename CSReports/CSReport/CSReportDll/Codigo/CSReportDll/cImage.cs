using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace CSReportDll
{
    internal static class cImage
    {
        /*
         
         Private Type BITMAP '14 bytes
              bmType As Long
              bmWidth As Long
              bmHeight As Long
              bmWidthBytes As Long
              bmPlanes As Integer
              bmBitsPixel As Integer
              bmBits As Long
        End Type
         * 
         */

        struct GDIBitmap {
            public int bmType;
            public int bmWidth;
            public int bmHeight;
            public int bmWidthBytes;
            public short bmPlanes;
            public short bmBitsPixel;
            public int bmBits;
        }

        private static int getIntFromByteArray(byte[] bytes, int index) 
        {
            byte[] intInBytes = new byte[4];

            Array.Copy(bytes, index, intInBytes, 0, 4);

            //
            // TODO: after some checking remove
            //
            // MSDN states that we should check architecture but I found that the values in the byte array are
            //      in the correct order
            //
            // If the system architecture is little-endian (that is, little end first),
            // reverse the byte array.
            /*
            if (BitConverter.IsLittleEndian)
                Array.Reverse(intInBytes);
            */
            return BitConverter.ToInt32(intInBytes, 0);
        }

        private static Int16 getShortFromByteArray(byte[] bytes, int index)
        {
            byte[] int16InBytes = new byte[2];

            Array.Copy(bytes, index, int16InBytes, 0, 2);

            //
            // TODO: after some checking remove
            //
            // MSDN states that we should check architecture but I found that the values in the byte array are
            //      in the correct order
            //
            // If the system architecture is little-endian (that is, little end first),
            // reverse the byte array.
            /*
            if (BitConverter.IsLittleEndian)
                Array.Reverse(intInBytes);
            */
            return BitConverter.ToInt16(int16InBytes, 0);
        }

        internal static Image deSerialiseBitmap(byte[] bytes) {
            try
            {
                Bitmap bmp;
                using (var ms = new MemoryStream(bytes))
                {
                    bmp = new Bitmap(ms);
                }
                return bmp;
            }
            catch {
                return null;
            }
        }
        /*
        internal static Image deSerialiseBitmap(byte[] bytes) 
        {
            GDIBitmap gdiBitmap = new GDIBitmap();
            
            gdiBitmap.bmType = getIntFromByteArray(bytes, 0);
            gdiBitmap.bmWidth = getIntFromByteArray(bytes, 4);
            gdiBitmap.bmHeight = getIntFromByteArray(bytes, 8);
            gdiBitmap.bmWidthBytes = getIntFromByteArray(bytes, 12);
            gdiBitmap.bmPlanes = getShortFromByteArray(bytes, 16);
            gdiBitmap.bmBitsPixel = getShortFromByteArray(bytes, 18);
            gdiBitmap.bmBits = getIntFromByteArray(bytes, 20);

            //
            // from
            // http://stackoverflow.com/questions/742236/how-to-create-a-bmp-file-from-byte-in-c-sharp
            //
            unsafe
            {
                fixed (byte* ptr = bytes)
                {
                    int stride = gdiBitmap.bmWidth * 3;
                    //
                    // the new Bitmap(new Bitmap is to get a new copy of the bitmap)
                    //
                    // first we create a bitmap from the bytes array but this array will be garbage collected in the future
                    // so we make a deep copy and then refresh the unsafe bitmap
                    //
                    Bitmap unsafeBitmap = new Bitmap(gdiBitmap.bmWidth, gdiBitmap.bmHeight, -stride, PixelFormat.Format24bppRgb, new IntPtr(ptr + 24 + stride * (gdiBitmap.bmHeight - 1)));
                    Bitmap safeBitmap = new Bitmap(unsafeBitmap);
                    unsafeBitmap.Dispose();
                    return safeBitmap;
                }
            }
        }*/

        internal static void serialiseBitmap(object image, object bytes) { }
    }
}
