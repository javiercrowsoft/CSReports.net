using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using CSKernelClient;
using CSReportGlobals;

namespace CSReportDll
{

    public class cReportPaperInfo
    {

        private const String C_MODULE = "cReportPaperInfo";

        private float m_width = 0;
        private float m_height = 0;
        private csReportPaperType m_paperSize;
        private int m_orientation = 0;
        private int m_customHeight = 0;
        private int m_customWidth = 0;
        private String m_pagesToPrint = "";
        private int m_paperBin = 0;

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
            m_height = rhs;
        }

        public csReportPaperType getPaperSize()
        {
            return m_paperSize;
        }

        public void setPaperSize(csReportPaperType rhs)
        {
            m_paperSize = rhs;
        }

        public int getOrientation()
        {
            return m_orientation;
        }

        public void setOrientation(int rhs)
        {
            m_orientation = rhs;
        }

        public int getCustomHeight()
        {
            return m_customHeight;
        }

        public void setCustomHeight(int rhs)
        {
            m_customHeight = rhs;
        }

        public int getCustomWidth()
        {
            return m_customWidth;
        }

        public void setCustomWidth(int rhs)
        {
            m_customWidth = rhs;
        }

        public int getPaperBin()
        {
            return m_paperBin;
        }

        public void setPaperBin(int rhs)
        {
            m_paperBin = rhs;
        }

        public String getPagesToPrint()
        {
            return m_pagesToPrint;
        }

        public void setPagesToPrint(String rhs)
        {
            m_pagesToPrint = rhs;
        }

        internal bool load(CSXml.cXml xDoc, XmlNode nodeObj)
        {
            if (nodeObj != null)
            {
                m_height = xDoc.getNodeProperty(nodeObj, "Height").getValueInt(eTypes.eLong);
                m_paperSize = (csReportPaperType)xDoc.getNodeProperty(nodeObj, "PaperSize").getValueInt(eTypes.eInteger);
                m_width = xDoc.getNodeProperty(nodeObj, "Width").getValueInt(eTypes.eLong);
                m_orientation = xDoc.getNodeProperty(nodeObj, "Orientation").getValueInt(eTypes.eInteger);
                m_customWidth = xDoc.getNodeProperty(nodeObj, "CustomWidth").getValueInt(eTypes.eLong);
                m_customHeight = xDoc.getNodeProperty(nodeObj, "CustomHeight").getValueInt(eTypes.eLong);
            }

            return true;
        }

        internal bool save(CSXml.cXml xDoc, XmlNode nodeFather)
        {
            CSXml.cXmlProperty xProperty = null;
            XmlNode nodeObj = null;

            xProperty = new CSXml.cXmlProperty();

            nodeObj = nodeFather;

            xProperty.setName("Height");
            xProperty.setValue(eTypes.eLong, m_height);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            xProperty.setName("PaperSize");
            xProperty.setValue(eTypes.eInteger, m_paperSize);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            xProperty.setName("Width");
            xProperty.setValue(eTypes.eLong, m_width);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            xProperty.setName("Orientation");
            xProperty.setValue(eTypes.eInteger, m_orientation);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            xProperty.setName("CustomWidth");
            xProperty.setValue(eTypes.eLong, m_customWidth);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            xProperty.setName("CustomHeight");
            xProperty.setValue(eTypes.eLong, m_customHeight);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            return true;
        }

    }

}
