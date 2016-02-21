using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using CSKernelClient;
using CSReportGlobals;

namespace CSReportDll
{

    public class cReportGroup
    {
        private const String C_HEADER = "H";
        private const String C_FOOTER = "F";

        private cReportSection m_header;
        private cReportSection m_footer;
        private int m_index = 0;

        private String m_name = "";

        private csRptGrpOrderType m_oderType;
        private csRptGrpComparisonType m_comparisonType;

        // to print in a new page when the group change
        //
        private bool m_printInNewPage;
        
        // to reprint group headers in every new page
        //
        private bool m_rePrintInNewPage;
        private bool m_grandTotalGroup;
        private String m_fieldName = "";
        private String m_key = "";

        public cReportSection getHeader()
        {
            return m_header;
        }

        public void setHeader(cReportSection rhs)
        {
            m_header = rhs;
        }

        public cReportSection getFooter()
        {
            return m_footer;
        }

        public void setFooter(cReportSection rhs)
        {
            m_footer = rhs;
        }

        public int getIndex()
        {
            return m_index;
        }

        public void setIndex(int rhs)
        {
            m_index = rhs;
        }

        public csRptGrpOrderType getOderType()
        {
            return m_oderType;
        }

        public void setOderType(csRptGrpOrderType rhs)
        {
            m_oderType = rhs;
        }

        public csRptGrpComparisonType getComparisonType()
        {
            return m_comparisonType;
        }

        public void setComparisonType(csRptGrpComparisonType rhs)
        {
            m_comparisonType = rhs;
        }

        public bool getPrintInNewPage()
        {
            return m_printInNewPage;
        }

        public void setPrintInNewPage(bool rhs)
        {
            m_printInNewPage = rhs;
        }

        public bool getRePrintInNewPage()
        {
            return m_rePrintInNewPage;
        }

        public void setRePrintInNewPage(bool rhs)
        {
            m_rePrintInNewPage = rhs;
        }

        public bool getGrandTotalGroup()
        {
            return m_grandTotalGroup;
        }

        public void setGrandTotalGroup(bool rhs)
        {
            m_grandTotalGroup = rhs;
        }

        public String getFieldName()
        {
            return m_fieldName;
        }

        public void setFieldName(String rhs)
        {
            m_fieldName = rhs;
        }

        public String getName()
        {
            return m_name;
        }

        public void setName(String rhs)
        {
            m_name = rhs;
        }

        public String getKey()
        {
            return m_key;
        }

        public void setKey(String rhs)
        {
            m_key = rhs;
        }

        internal bool load(CSXml.cXml xDoc, XmlNode nodeObj)
        {
            m_name = xDoc.getNodeProperty(nodeObj, "Name").getValueString(eTypes.eText);

            // TODO: fix me - this is Spanish - English bug we should use Index
            //
            m_index = xDoc.getNodeProperty(nodeObj, "Indice").getValueInt(eTypes.eInteger);

            m_comparisonType = (csRptGrpComparisonType)xDoc.getNodeProperty(nodeObj, "ComparisonType").getValueInt(eTypes.eInteger);
            m_fieldName = xDoc.getNodeProperty(nodeObj, "FieldName").getValueString(eTypes.eText);
            m_oderType = (csRptGrpOrderType)xDoc.getNodeProperty(nodeObj, "OderType").getValueInt(eTypes.eInteger);
            m_printInNewPage = xDoc.getNodeProperty(nodeObj, "PrintInNewPage").getValueBool(eTypes.eBoolean);
            m_rePrintInNewPage = xDoc.getNodeProperty(nodeObj, "RePrintInNewPage").getValueBool(eTypes.eBoolean);
            m_grandTotalGroup = xDoc.getNodeProperty(nodeObj, "GrandTotalGroup").getValueBool(eTypes.eBoolean);

            pSetName();

            XmlNode nodeObjAux = null;

            nodeObjAux = nodeObj;
            nodeObjAux = xDoc.getNodeFromNode(nodeObj, C_HEADER);
            nodeObjAux = xDoc.getNodeChild(nodeObjAux);
            if (!m_header.load(xDoc, nodeObjAux)) 
            { 
                return false; 
            }

            m_header.setName(m_name);

            nodeObjAux = nodeObj;
            nodeObjAux = xDoc.getNodeFromNode(nodeObj, C_FOOTER);
            nodeObjAux = xDoc.getNodeChild(nodeObjAux);
            if (!m_footer.load(xDoc, nodeObjAux)) 
            { 
                return false; 
            }

            m_footer.setName(m_name);

            return true;
        }

        private void pSetName()
        {
            if (m_name.Length == 0
                ||cUtil.subString(m_name.ToLower(), 0, 5) == "group" 
                || cUtil.subString(m_name.ToLower(), 0, 5) == "grupo" 
                || cUtil.subString(m_name.ToLower(), 0, 3) == "gh_" 
                || cUtil.subString(m_name.ToLower(), 0, 3) == "gf_" 
                || cUtil.subString(m_name.ToLower(), 0, 2) == "g_" 
                )
            {
                m_name = "G_" + m_index;
            }

        }

        internal bool save(CSXml.cXml xDoc, XmlNode nodeFather)
        {
            CSXml.cXmlProperty xProperty = null;
            XmlNode nodeObj = null;

            xProperty = new CSXml.cXmlProperty();

            xProperty.setName(m_name);
            nodeObj = xDoc.addNodeToNode(nodeFather, xProperty);

            xProperty.setName("Key");
            xProperty.setValue(eTypes.eText, m_key);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            xProperty.setName("Name");
            xProperty.setValue(eTypes.eText, m_name);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            // TODO: fix me - this is Spanish - English bug we should use Index
            //
            xProperty.setName("Indice");

            xProperty.setValue(eTypes.eInteger, m_index);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            xProperty.setName("ComparisonType");
            xProperty.setValue(eTypes.eInteger, m_comparisonType);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            xProperty.setName("FieldName");
            xProperty.setValue(eTypes.eText, m_fieldName);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            xProperty.setName("OderType");
            xProperty.setValue(eTypes.eInteger, m_oderType);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            xProperty.setName("PrintInNewPage");
            xProperty.setValue(eTypes.eBoolean, m_printInNewPage);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            xProperty.setName("RePrintInNewPage");
            xProperty.setValue(eTypes.eBoolean, m_rePrintInNewPage);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            xProperty.setName("GrandTotalGroup");
            xProperty.setValue(eTypes.eBoolean, m_grandTotalGroup);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            XmlNode nodeObjAux = null;
            nodeObjAux = nodeObj;
            xProperty.setName(C_HEADER);
            nodeObjAux = xDoc.addNodeToNode(nodeObjAux, xProperty);
            m_header.save(xDoc, nodeObjAux);

            nodeObjAux = nodeObj;
            xProperty.setName(C_FOOTER);
            nodeObjAux = xDoc.addNodeToNode(nodeObjAux, xProperty);
            m_footer.save(xDoc, nodeObjAux);

            return true;

        }

    }

}
