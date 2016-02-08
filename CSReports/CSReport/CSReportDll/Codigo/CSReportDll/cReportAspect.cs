using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using CSKernelClient;
using CSReportGlobals;

namespace CSReportDll
{

    public class cReportAspect
    {

        private float m_left = 0;
        private float m_top = 0;
        private float m_height = 0;
        private float m_width = 0;
        private int m_backColor = (int)csColors.C_COLOR_WHITE;
        private float m_borderWidth = 0;
        private csReportBorderType m_borderType;
        private int m_borderColor = (int)csColors.C_COLOR_BLACK;
        private int m_borderColor3d = 0;
        private int m_borderColor3dShadow = 0;
        private int m_selectColor = 0;
        private cReportFont m_font = new cReportFont();
        private bool m_canGrow;
        private int m_nZOrder = 0;
        private HorizontalAlignment m_align = HorizontalAlignment.Left;
        private bool m_transparent;
        private String m_format = "";
        private String m_symbol = "";
        private bool m_isAccounting;
        private bool m_wordWrap;
        private bool m_borderRounded;
        private float m_offset = 0;

        public void setOffset(float rhs)
        {
            m_offset = rhs;
        }

        public float getOffset()
        {
            return m_offset;
        }

        public float getLeft()
        {
            return m_left;
        }

        public void setLeft(float rhs)
        {
            m_left = rhs;
        }

        public float getTop()
        {
            return m_top;
        }

        public void setTop(float rhs)
        {
            m_top = rhs;
        }

        public float getWidth()
        {
            return m_width;
        }

        public void setWidth(float rhs)
        {
            m_width = rhs;
        }

        public float getHeight()
        {
            return m_height;
        }

        public void setHeight(float rhs)
        {
            if (rhs < 1) { rhs = 1; }
            m_height = rhs;
        }

        public int getBackColor()
        {
            return m_backColor;
        }

        public void setBackColor(int rhs)
        {
            m_backColor = rhs;
        }

        public float getBorderWidth()
        {
            return m_borderWidth;
        }

        public void setBorderWidth(float rhs)
        {
            m_borderWidth = rhs;
        }

        public csReportBorderType getBorderType()
        {
            return m_borderType;
        }

        public void setBorderType(csReportBorderType rhs)
        {
            m_borderType = rhs;
        }

        public int getBorderColor()
        {
            return m_borderColor;
        }

        public void setBorderColor(int rhs)
        {
            m_borderColor = rhs;
        }

        public int getBorderColor3d()
        {
            return m_borderColor3d;
        }

        public void setBorderColor3d(int rhs)
        {
            m_borderColor3d = rhs;
        }

        public int getBorderColor3dShadow()
        {
            return m_borderColor3dShadow;
        }

        public void setBorderColor3dShadow(int rhs)
        {
            m_borderColor3dShadow = rhs;
        }

        public int getSelectColor()
        {
            return m_selectColor;
        }

        public void setSelectColor(int rhs)
        {
            m_selectColor = rhs;
        }

        public cReportFont getFont()
        {
            return m_font;
        }

        public void setFont(cReportFont rhs)
        {
            m_font = rhs;
        }

        public bool getCanGrow()
        {
            return m_canGrow;
        }

        public void setCanGrow(bool rhs)
        {
            m_canGrow = rhs;
        }

        public int getNZOrder()
        {
            return m_nZOrder;
        }

        public void setNZOrder(int rhs)
        {
            m_nZOrder = rhs;
        }

        public HorizontalAlignment getAlign()
        {
            return m_align;
        }

        public void setAlign(HorizontalAlignment rhs)
        {
            m_align = rhs;
        }

        public bool getTransparent()
        {
            return m_transparent;
        }

        public void setTransparent(bool rhs)
        {
            m_transparent = rhs;
        }

        public String getFormat()
        {
            return m_format;
        }

        public void setFormat(String rhs)
        {
            m_format = rhs;
        }

        public String getSymbol()
        {
            return m_symbol;
        }

        public void setSymbol(String rhs)
        {
            m_symbol = rhs;
        }

        public bool getIsAccounting()
        {
            return m_isAccounting;
        }

        public void setIsAccounting(bool rhs)
        {
            m_isAccounting = rhs;
        }

        public bool getWordWrap()
        {
            return m_wordWrap;
        }

        public void setWordWrap(bool rhs)
        {
            m_wordWrap = rhs;
        }

        public bool getBorderRounded()
        {
            return m_borderRounded;
        }

        public void setBorderRounded(bool rhs)
        {
            m_borderRounded = rhs;
        }

        internal bool load(CSXml.cXml xDoc, XmlNode nodeObj)
        {
            nodeObj = xDoc.getNodeFromNode(nodeObj, "Aspect");

            // we don't care if some property is missing

            try { m_align = (HorizontalAlignment)xDoc.getNodeProperty(nodeObj, "Align").getValueInt(eTypes.eInteger); }
            catch { }
            try { m_backColor = xDoc.getNodeProperty(nodeObj, "BackColor").getValueInt(eTypes.eLong); }
            catch { }
            try { m_borderColor = xDoc.getNodeProperty(nodeObj, "BorderColor").getValueInt(eTypes.eLong); }
            catch { }
            try { m_borderColor3d = xDoc.getNodeProperty(nodeObj, "BorderColor3D").getValueInt(eTypes.eLong); }
            catch { }
            try { m_borderColor3dShadow = xDoc.getNodeProperty(nodeObj, "BorderColor3DShadow").getValueInt(eTypes.eLong); }
            catch { }
            try { m_borderType = (csReportBorderType)xDoc.getNodeProperty(nodeObj, "BorderType").getValueInt(eTypes.eInteger); }
            catch { }
            try { m_borderWidth = xDoc.getNodeProperty(nodeObj, "BorderWidth").getValueInt(eTypes.eLong); }
            catch { }
            try { m_height = xDoc.getNodeProperty(nodeObj, "Height").getValueInt(eTypes.eLong); }
            catch { }
            try { m_canGrow = xDoc.getNodeProperty(nodeObj, "CanGrow").getValueBool(eTypes.eBoolean); }
            catch { }
            try { m_left = xDoc.getNodeProperty(nodeObj, "Left").getValueInt(eTypes.eLong); }
            catch { }
            try { m_nZOrder = xDoc.getNodeProperty(nodeObj, "nZOrder").getValueInt(eTypes.eInteger); }
            catch { }
            try { m_selectColor = xDoc.getNodeProperty(nodeObj, "SelectColor").getValueInt(eTypes.eLong); }
            catch { }
            try { m_top = xDoc.getNodeProperty(nodeObj, "Top").getValueInt(eTypes.eLong); }
            catch { }
            try { m_width = xDoc.getNodeProperty(nodeObj, "Width").getValueInt(eTypes.eLong); }
            catch { }
            try { m_transparent = xDoc.getNodeProperty(nodeObj, "Transparent").getValueBool(eTypes.eBoolean); }
            catch { }
            try { m_format = xDoc.getNodeProperty(nodeObj, "Format").getValueString(eTypes.eText); }
            catch { }
            try { m_symbol = xDoc.getNodeProperty(nodeObj, "Symbol").getValueString(eTypes.eText); }
            catch { }
            try { m_isAccounting = xDoc.getNodeProperty(nodeObj, "IsAccounting").getValueBool(eTypes.eBoolean); }
            catch { }
            try { m_wordWrap = xDoc.getNodeProperty(nodeObj, "WordWrap").getValueBool(eTypes.eBoolean); }
            catch { }
            try { m_borderRounded = xDoc.getNodeProperty(nodeObj, "BorderRounded").getValueBool(eTypes.eBoolean); }
            catch { }

            twipsToPixels();

            return m_font.load(xDoc, nodeObj);
        }

        internal bool save(CSXml.cXml xDoc, XmlNode nodeFather)
        {
            pixelsToTwips();

            CSXml.cXmlProperty xProperty = null;
            XmlNode nodeObj = null;
            xProperty = new CSXml.cXmlProperty();

            xProperty.setName("Aspect");
            nodeObj = xDoc.addNodeToNode(nodeFather, xProperty);

            xProperty.setName("Align");
            xProperty.setValue(eTypes.eInteger, m_align);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            xProperty.setName("BackColor");
            xProperty.setValue(eTypes.eLong, m_backColor);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            xProperty.setName("BorderColor");
            xProperty.setValue(eTypes.eLong, m_borderColor);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            xProperty.setName("BorderColor3D");
            xProperty.setValue(eTypes.eLong, m_borderColor3d);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            xProperty.setName("BorderColor3DShadow");
            xProperty.setValue(eTypes.eLong, m_borderColor3dShadow);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            xProperty.setName("BorderType");
            xProperty.setValue(eTypes.eInteger, m_borderType);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            xProperty.setName("BorderWidth");
            xProperty.setValue(eTypes.eLong, m_borderWidth);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            xProperty.setName("CanGrow");
            xProperty.setValue(eTypes.eBoolean, m_canGrow);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            xProperty.setName("Height");
            xProperty.setValue(eTypes.eLong, m_height);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            xProperty.setName("Format");
            xProperty.setValue(eTypes.eText, m_format);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            xProperty.setName("Left");
            xProperty.setValue(eTypes.eLong, m_left);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            xProperty.setName("nZOrder");
            xProperty.setValue(eTypes.eInteger, m_nZOrder);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            xProperty.setName("SelectColor");
            xProperty.setValue(eTypes.eLong, m_selectColor);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            xProperty.setName("Top");
            xProperty.setValue(eTypes.eLong, m_top);

            xDoc.addPropertyToNode(nodeObj, xProperty);

            xProperty.setName("Width");
            xProperty.setValue(eTypes.eLong, m_width);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            xProperty.setName("Transparent");
            xProperty.setValue(eTypes.eBoolean, m_transparent);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            xProperty.setName("Symbol");
            xProperty.setValue(eTypes.eText, m_symbol);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            xProperty.setName("IsAccounting");
            xProperty.setValue(eTypes.eBoolean, m_isAccounting);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            xProperty.setName("WordWrap");
            xProperty.setValue(eTypes.eBoolean, m_wordWrap);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            xProperty.setName("BorderRounded");
            xProperty.setValue(eTypes.eBoolean, m_borderRounded);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            twipsToPixels();

            return !m_font.save(xDoc, nodeObj);
        }

        private void twipsToPixels() 
        {
            m_height = cUtil.tp(Convert.ToInt32(m_height));
            m_left = cUtil.tp(Convert.ToInt32(m_left));
            m_top = cUtil.tp(Convert.ToInt32(m_top));
            m_width = cUtil.tp(Convert.ToInt32(m_width));
        }

        private void pixelsToTwips() 
        {
            m_height = cUtil.pt(Convert.ToInt32(m_height));
            m_left = cUtil.pt(Convert.ToInt32(m_left));
            m_top = cUtil.pt(Convert.ToInt32(m_top));
            m_width = cUtil.pt(Convert.ToInt32(m_width));        
        }
    }

}
