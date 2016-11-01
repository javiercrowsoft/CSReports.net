using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSReportDll;
using CSReportGlobals;
using System.Drawing;

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

            return new RectangleF(left, top, right-left, bottom-top);
        }

        public static Rectangle newRectangle(int left, int top, int right, int bottom)
        {
            if (left < 0) left = 0;
            if (top < 0) top = 0;
            if (right < left) right = left;
            if (bottom < top) bottom = top;

            return new Rectangle(left, top, right-left, bottom-top);
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

        // fonts

        public static void redim(ref Font[] vFonts, int size)
        {
            vFonts = new Font[size];
        }

        public static void redimPreserve(ref Font[] vFonts, int size)
        {
            if (size == 0)
            {
                vFonts = new Font[0];
            }
            else
            {
                if (vFonts == null)
                {
                    vFonts = new Font[size];
                }
                else if (vFonts.Length == 0)
                {
                    vFonts = new Font[size];
                }
                else
                {
                    Font[] newArray = new Font[size];
                    Array.Copy(vFonts, newArray, vFonts.Length);
                    vFonts = newArray;
                }
            }
        }

        public static int addFontIfRequired(cReportFont font, ref Font[] m_fnt)
        {
            for(int i = 0; i < m_fnt.Length; i++) {
                if(font.getName() == m_fnt[i].Name 
                    && font.getBold() == m_fnt[i].Bold 
                    && font.getItalic() == m_fnt[i].Italic 
                    && font.getUnderline() == m_fnt[i].Underline 
                    && font.getSize() == m_fnt[i].Size 
                    && font.getStrike() == m_fnt[i].Strikeout) {
                    return i;
                }
            }

            redimPreserve(ref m_fnt, m_fnt.Length + 1);

            FontStyle fontStyle = FontStyle.Regular;
            if (font.getBold()) fontStyle = fontStyle | FontStyle.Bold;
            if (font.getItalic()) fontStyle = fontStyle | FontStyle.Italic;
            if (font.getUnderline()) fontStyle = fontStyle | FontStyle.Underline;
            if (font.getStrike()) fontStyle = fontStyle | FontStyle.Strikeout;

            Font afont = new Font(font.getName(), ((font.getSize() > 0) ? font.getSize() : 3), fontStyle);

            m_fnt[m_fnt.Length - 1] = afont;

            return m_fnt.Length - 1;
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
