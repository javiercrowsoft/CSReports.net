using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using CSReportDll;
using CSReportGlobals;

namespace CSReportPaint
{
    public static class cGlobals
    {
        private static int m_nextKey = 1000;

        private const string C_MODULE = "cGlobals";

        private static Bitmap _flag = new Bitmap(1, 1);
        private static Graphics _g = Graphics.FromImage(_flag);

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

        public static RectangleF newRectangleF(Single left, Single top, Single right, Single bottom)
        {
            if (left < 0) left = 0;
            if (top < 0) top = 0;
            if (right < left) right = left;
            if (bottom < top) bottom = top;

            return new RectangleF(left, top, right, bottom);
        }

        public static Rectangle newRectangle(int left, int top, int right, int bottom)
        {
            if (left < 0) left = 0;
            if (top < 0) top = 0;
            if (right < left) right = left;
            if (bottom < top) bottom = top;

            return new Rectangle(left, top, right, bottom);
        }

        private static float getPixelsFromCmX(float cm)
        {
            return cm * _g.DpiX / 2.54f;
        }
        private static float getPixelsFromCmY(float cm)
        {
            return cm * _g.DpiY / 2.54f;
        }

        public static RectangleF getRectFromPaperSize(cReportPaperInfo info, csReportPaperType paperSize, int orientation)
        {
            RectangleF rtn = new RectangleF();

            switch (paperSize)
            {
                case csReportPaperType.CSRPTPAPERTYPELETTER:
                    rtn.Height = getPixelsFromCmY(27.94f); // 15840;
                    rtn.Width = getPixelsFromCmX(21.59f);  // 12240;
                    break;

                case csReportPaperType.CSRPTPAPERTYPELEGAL:
                    rtn.Height = getPixelsFromCmY(35.56f); // 20160;
                    rtn.Width = getPixelsFromCmX(21.59f);  // 12060;
                    break;

                case csReportPaperType.CSRPTPAPERTYPEA4:
                    rtn.Height = getPixelsFromCmY(29.7f); // 16832;
                    rtn.Width = getPixelsFromCmX(21f);    // 11908;
                    break;

                case csReportPaperType.CSRPTPAPERTYPEA3:
                    rtn.Height = getPixelsFromCmY(42f); // 23816;
                    rtn.Width = getPixelsFromCmX(29.7f);    // 16832;
                    break;

                case csReportPaperType.CSRPTPAPERUSER:
                    if (info == null)
                    {
                        string msg = "The settings for the custome user paper size is not defined";
                        throw new ReportPaintException(csRptPaintErrors.CSRPT_PAINT_ERR_OBJ_CLIENT, C_MODULE, msg);
                    }
                    else
                    {
                        rtn.Width = getPixelsFromCmY(info.getCustomWidth());
                        rtn.Height = getPixelsFromCmX(info.getCustomHeight());
                    }
                    break;
            }

            if (orientation == (int)csRptPageOrientation.LANDSCAPE)
            {
                float tmp = 0;
                tmp = rtn.Height;
                rtn.Height = rtn.Width;
                rtn.Width = tmp;
            }

            return rtn;
        }
    }

    public enum csETypeGrid {
        CSEGRIDNONE,
        CSEGRIDPOINTS,
        CSEGRIDLINES,
        CSEGRIDLINESVERTICAL,
        CSEGRIDLINESHORIZONTAL
    }

    public enum csRptPaintObjType
    {
        CSRPTPAINTOBJBOX,
        CSRPTPAINTOBJLINE,
        CSRPTPAINTOBJCIRCLE,
        CSRPTPAINTOBJIMAGE
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
