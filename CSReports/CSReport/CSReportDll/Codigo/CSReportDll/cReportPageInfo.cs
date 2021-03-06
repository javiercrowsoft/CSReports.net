﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using CSKernelClient;

namespace CSReportDll
{
    public class cReportPageInfo
    {

        private const String C_MODULE = "cReportPageInfo";

        private cReportAspect m_aspect;
        private cReportSectionLine m_sectionLine;
        private String m_name = "";
        private String m_tag = "";
        private int m_fieldType = 0;

        public cReportPageInfo()
        {
            m_aspect = new cReportAspect();
        }

        public String getName()
        {
            return m_name;
        }

        public void setName(String rhs)
        {
            m_name = rhs;
        }

        public String getTag()
        {
            return m_tag;
        }

        public void setTag(String rhs)
        {
            m_tag = rhs;
        }

        public cReportAspect getAspect()
        {
            return m_aspect;
        }

        public void setAspect(cReportAspect rhs)
        {
            m_aspect = rhs;
        }

        public cReportSectionLine getSectionLine()
        {
            return m_sectionLine;
        }

        public void setSectionLine(cReportSectionLine rhs)
        {
            m_sectionLine = rhs;
        }

        public int getFieldType()
        {
            return m_fieldType;
        }

        public void setFieldType(int rhs)
        {
            m_fieldType = rhs;
        }

        internal bool load(CSXml.cXml xDoc, XmlNode nodeObj)
        {
            XmlNode nodeObjAspect = null;
            m_name = xDoc.getNodeProperty(nodeObj, "Name").getValueString(eTypes.eText);
            m_fieldType = xDoc.getNodeProperty(nodeObj, "FieldType").getValueInt(eTypes.eInteger);

            nodeObjAspect = nodeObj;
            if (!m_aspect.load(xDoc, nodeObjAspect))
            {
                return false;
            }
            else 
            {
                return true;
            }
        }

        internal bool save(CSXml.cXml xDoc, XmlNode nodeFather)
        { 
            CSXml.cXmlProperty xProperty = null;
            XmlNode nodeObj = null;

            xProperty = new CSXml.cXmlProperty();

            xProperty.setName("PageInfo");
            nodeObj = xDoc.addNodeToNode(nodeFather, xProperty);

            xProperty.setName("Name");
            xProperty.setValue(eTypes.eText, m_name);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            xProperty.setName("FieldType");
            xProperty.setValue(eTypes.eInteger, m_fieldType);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            return m_aspect.save(xDoc, nodeObj);
        }

    }

}
