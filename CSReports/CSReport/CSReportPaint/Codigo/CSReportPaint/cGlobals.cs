using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace CSReportPaint
{
    static class cGlobals
    {
        private static int m_nextKey = 1000;

        public static int getNextKey()
        {
            m_nextKey++;
            return m_nextKey;
        }

        public static String getKey(String value)
        {
            if (value.Length > 0)
            {
                if ("0123456789".Contains(value.Substring(0, 1)))
                {
                    value = "K" + value;
                }
            }
            return value;
        }

        public static void getBitmapSize(Image image, out int imgWidth, out int imgHeight, bool inTwips)
        {
            imgWidth = image.Width;
            imgHeight = image.Height;
        }

        public static Single setRectangleWidth(Single width)
        {
            if (width < 0)
                width = 0;
            return width;
        }

        public static Single setRectangleHeight(Single height)
        {
            if (height < 0)
                height = 0;
            return height;
        }

        public static RectangleF newRectangle(Single left, Single top, Single right, Single bottom)
        {
            if (left < 0) left = 0;
            if (top < 0) top = 0;
            if (right < left) right = left;
            if (bottom < top) bottom = top;

            return new RectangleF(left, top, right, bottom);
        }
    }

    public enum csETypeGrid {
        CSEGRIDNONE,
        CSEGRIDPOINTS,
        CSEGRIDLINES,
        CSEGRIDLINESVERTICAL,
        CSEGRIDLINESHORIZONTAL
    }
    public enum csRptPaintRptType {
        CSRPTPAINTRPTTYPESECTIONHEADER = 0,
        CSRPTPAINTRPTTYPESECTIONDETAIL = 1,
        CSRPTPAINTRPTTYPESECTIONFOOTER = 2,
        CSRPTPAINTRPTTYPEMAINSECTIONHEADER = 100,
        CSRPTPAINTRPTTYPEMAINSECTIONDETAIL = 101,
        CSRPTPAINTRPTTYPEMAINSECTIONFOOTER = 102,
        CSRPTPAINTRPTTYPEGROUPSECTIONHEADER = 3,
        CSRPTPAINTRPTTYPEGROUPSECTIONFOOTER = 4,
        CSRPTPAINTRPTTYPECONTROL = 50
    }

    public enum csRptPaintObjType
    {
        CSRPTPAINTOBJBOX,
        CSRPTPAINTOBJLINE,
        CSRPTPAINTOBJCIRCLE,
        CSRPTPAINTOBJIMAGE
    }

    public enum csColors
    {
        C_COLOR_BLACK = 0,
        C_COLOR_WHITE = 255
    }

    public enum csRptPaintRegionType
    {
        CRPTPNTRGNTYPEBODY,
        CRPTPNTRGNTYPELEFTUP,
        CRPTPNTRGNTYPELEFTDOWN,
        CRPTPNTRGNTYPERIGHTUP,
        CRPTPNTRGNTYPERIGHTDOWN,
        CRPTPNTRGNTYPEUP,
        CRPTPNTRGNTYPEDOWN,
        CRPTPNTRGNTYPELEFT,
        CRPTPNTRGNTYPERIGHT
    }

    public enum csEMoveTo
    {
        C_FIRSTPAGE = 1,
        C_NEXTPAGE = -1,
        C_PREVIOUSPAGE = -2,
        C_LASTPAGE = -3
    }


    public enum csPDFQuality
    {
        PDFQUALITYFULL = 1,
        PDFQUALITYSMALL = 2,
        PDFQUALITYMEDIUM = 3
    }

}
