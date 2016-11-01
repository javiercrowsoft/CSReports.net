using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using CSReportDll;
using CSReportGlobals;

namespace CSReportPaint
{
    public class cReportPaintObject
    {

        private const String C_MODULE = "cReportPaintObject";

        private cReportAspect m_aspect = new cReportAspect();
        private String m_key = "";
        private String m_text = "";
        private csRptPaintObjType m_paintType;
        private String m_tag = "";
        private csRptSectionType m_rptType;
        private String m_rptKeySec = "";
        private Image m_image = null;
        private int m_indexField = 0;

        private bool m_isSection;

        private float m_heightSec = 0;
        private float m_heightSecLine = 0;
        private String m_textLine = "";

        public Image getImage()
        {
            return m_image;
        }

        public void setImage(Image rhs)
        {
            m_image = rhs;
        }

        public cReportAspect getAspect()
        {
            return m_aspect;
        }

        public void setAspect(cReportAspect rhs)
        {
            m_aspect = rhs;
        }

        public String getKey()
        {
            return m_key;
        }

        public void setKey(String rhs)
        {
            m_key = rhs;
        }

        public String getText()
        {
            return m_text;
        }

        public void setText(String rhs)
        {
            m_text = rhs;
        }

        public csRptPaintObjType getPaintType()
        {
            return m_paintType;
        }

        public void setPaintType(csRptPaintObjType rhs)
        {
            m_paintType = rhs;
        }

        public csRptSectionType getRptType()
        {
            return m_rptType;
        }

        public void setRptType(csRptSectionType rhs)
        {
            m_rptType = rhs;
        }

        public String getTag()
        {
            return m_tag;
        }

        public void setTag(String rhs)
        {
            m_tag = rhs;
        }

        public String getRptKeySec()
        {
            return m_rptKeySec;
        }

        public void setRptKeySec(String rhs)
        {
            m_rptKeySec = rhs;
        }

        public int getIndexField()
        {
            return m_indexField;
        }

        public void setIndexField(int rhs)
        {
            m_indexField = rhs;
        }

        public float getHeightSec()
        {
            return m_heightSec;
        }

        public void setHeightSec(float rhs)
        {
            m_heightSec = rhs;
        }

        public float getHeightSecLine()
        {
            return m_heightSecLine;
        }

        public void setHeightSecLine(float rhs)
        {
            m_heightSecLine = rhs;
        }

        public String getTextLine()
        {
            return m_textLine;
        }

        public void setTextLine(String rhs)
        {
            m_textLine = rhs;
        }

        public bool getIsSection()
        {
            return m_isSection;
        }

        public void setIsSection(bool rhs)
        {
            m_isSection = rhs;
        }

    }

}
