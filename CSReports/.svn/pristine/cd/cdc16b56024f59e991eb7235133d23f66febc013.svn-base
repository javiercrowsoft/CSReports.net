using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using CSKernelClient;
using CSReportGlobals;

namespace CSReportDll
{
    public class cReportFont
    {

        private int m_foreColor = (int)csColors.C_COLOR_BLACK;
        private float m_size = 8;
        private String m_name = "Tahoma";
        private bool m_underline;
        private bool m_bold;
        private bool m_italic;
        private bool m_strike;

        public int getForeColor()
        {
            return m_foreColor;
        }

        public void setForeColor(int rhs)
        {
            m_foreColor = rhs;
        }

        public float getSize()
        {
            return m_size;
        }

        public void setSize(float rhs)
        {
            m_size = rhs;
        }

        public String getName()
        {
            return m_name;
        }

        public void setName(String rhs)
        {
            m_name = rhs;
        }

        public bool getUnderline()
        {
            return m_underline;
        }

        public void setUnderLine(bool rhs)
        {
            m_underline = rhs;
        }

        public bool getBold()
        {
            return m_bold;
        }

        public void setBold(bool rhs)
        {
            m_bold = rhs;
        }

        public bool getItalic()
        {
            return m_italic;
        }

        public void setItalic(bool rhs)
        {
            m_italic = rhs;
        }

        public bool getStrike()
        {
            return m_strike;
        }

        public void setStrike(bool rhs)
        {
            m_strike = rhs;
        }

        internal bool load(CSXml.cXml xDoc, XmlNode nodeObj)
        {
            nodeObj = xDoc.getNodeFromNode(nodeObj, "Font");
            m_bold = xDoc.getNodeProperty(nodeObj, "Bold").getValueBool(eTypes.eBoolean);
            m_foreColor = xDoc.getNodeProperty(nodeObj, "ForeColor").getValueInt(eTypes.eLong);
            m_italic = xDoc.getNodeProperty(nodeObj, "Italic").getValueBool(eTypes.eBoolean);
            m_name = xDoc.getNodeProperty(nodeObj, "Name").getValueString(eTypes.eText);
            m_size = xDoc.getNodeProperty(nodeObj, "Size").getValueInt(eTypes.eInteger);
            m_underline = xDoc.getNodeProperty(nodeObj, "UnderLine").getValueBool(eTypes.eBoolean);
            m_strike = xDoc.getNodeProperty(nodeObj, "Strike").getValueBool(eTypes.eBoolean);

            return true;
        }

        internal bool save(CSXml.cXml xDoc, XmlNode nodeFather)
        {
            CSXml.cXmlProperty xProperty = null;
            XmlNode nodeObj = null;
            xProperty = new CSXml.cXmlProperty();

            xProperty.setName("Font");
            nodeObj = xDoc.addNodeToNode(nodeFather, xProperty);

            xProperty.setName("ForeColor");
            xProperty.setValue(eTypes.eLong, m_foreColor);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            xProperty.setName("Bold");
            xProperty.setValue(eTypes.eBoolean, m_bold);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            xProperty.setName("Italic");
            xProperty.setValue(eTypes.eBoolean, m_italic);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            xProperty.setName("Name");
            xProperty.setValue(eTypes.eText, m_name);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            xProperty.setName("Size");
            xProperty.setValue(eTypes.eInteger, m_size);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            xProperty.setName("UnderLine");
            xProperty.setValue(eTypes.eBoolean, m_underline);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            xProperty.setName("Strike");
            xProperty.setValue(eTypes.eBoolean, m_strike);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            return true;
        }

    }

}
