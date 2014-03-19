using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using CSKernelClient;

namespace CSReportDll
{
    public class cReportLabel
    {

        private cReportAspect m_aspect;
        private String m_text = "";
        private bool m_canGrow;

        public cReportLabel()
        {
            m_aspect = new cReportAspect();
        }

        public cReportAspect getAspect()
        {
            return m_aspect;
        }

        public void setAspect(cReportAspect rhs)
        {
            m_aspect = rhs;
        }

        public String getText()
        {
            return m_text;
        }

        public void setText(String rhs)
        {
            m_text = rhs;
        }

        public bool getCanGrow()
        {
            return m_canGrow;
        }

        public void setCanGrow(bool rhs)
        {
            m_canGrow = rhs;
        }

        internal bool load(CSXml.cXml xDoc, XmlNode nodeObj)
        { 
            nodeObj = xDoc.getNodeFromNode(nodeObj, "Label");
            m_text = xDoc.getNodeProperty(nodeObj, "Text").getValueString(eTypes.eText);
            return m_aspect.load(xDoc, nodeObj);
        }

        internal bool save(CSXml.cXml xDoc, XmlNode nodeFather)
        { 
            CSXml.cXmlProperty xProperty = null;
            XmlNode nodeObj = null;
            xProperty = new CSXml.cXmlProperty();

            xProperty.setName("Label");
            nodeObj = xDoc.addNodeToNode(nodeFather, xProperty);

            xProperty.setName("Text");
            xProperty.setValue(eTypes.eText, m_text);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            xProperty.setName("CanGrow");
            xProperty.setValue(eTypes.eBoolean, m_canGrow);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            if (!m_aspect.save(xDoc, nodeObj))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

    }

}
