using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace CSReportDll
{
    public class cReportLine
    {

        private cReportAspect m_aspect = new cReportAspect();

        public cReportAspect getAspect()
        {
            return m_aspect;
        }

        public void setAspect(cReportAspect rhs)
        {
            m_aspect = rhs;
        }

        internal bool load(CSXml.cXml xDoc, XmlNode nodeObj)
        { 
            nodeObj = xDoc.getNodeFromNode(nodeObj, "Line");
            return m_aspect.load(xDoc, nodeObj);
        }

        internal bool save(CSXml.cXml xDoc, XmlNode nodeFather)
        { 
            CSXml.cXmlProperty xProperty = null;
            XmlNode nodeObj = null;

            xProperty = new CSXml.cXmlProperty();

            xProperty.setName("Line");
            nodeObj = xDoc.addNodeToNode(nodeFather, xProperty);

            return m_aspect.save(xDoc, nodeObj);
        }

    }

}
