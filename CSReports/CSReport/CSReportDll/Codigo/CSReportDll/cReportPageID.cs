using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using CSKernelClient;

namespace CSReportDll
{
    public class cReportPageID
    {

        private const String C_MODULE = "cReportPageID";

        private String m_value = "";

        public String getValue()
        {
            return m_value;
        }

        public void setValue(String rhs)
        {
            m_value = rhs;
        }

        internal bool load(CSXml.cXml xDoc, XmlNode nodeObj)
        {
            m_value = xDoc.getNodeProperty(nodeObj, "Value").getValueString(eTypes.eText);
            return true;
        }

        internal bool save(CSXml.cXml xDoc, XmlNode nodeFather)
        {
            CSXml.cXmlProperty xProperty = null;
            XmlNode nodeObj = null;
            xProperty = new CSXml.cXmlProperty();

            xProperty.setName("PageID");
            nodeObj = xDoc.addNodeToNode(nodeFather, xProperty);

            xProperty.setName("Value");
            xProperty.setValue(eTypes.eText, m_value);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            return true;
        }

    }

}
