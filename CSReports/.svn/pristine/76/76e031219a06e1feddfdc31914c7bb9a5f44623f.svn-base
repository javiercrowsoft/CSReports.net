using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using CSKernelClient;

namespace CSReportDll
{

    public class cParameter
    {
        private const String C_MODULE = "cParameter";

        private String m_name = "";
        private CSDataBase.csDataType m_typeColumn;
        private String m_value = "";
        private int m_position = 0;
        private String m_key = "";
        private bool m_hasDefault;
        private String m_default = "";
        private bool m_isNullable;
        private int m_maxLength = 0;

        public String getKey()
        {
            return m_key;
        }

        public void setKey(String rhs)
        {
            m_key = rhs;
        }

        public String getName()
        {
            return m_name;
        }

        public void setName(String rhs)
        {
            m_name = rhs;
        }

        public CSDataBase.csDataType getTypeColumn()
        {
            return m_typeColumn;
        }

        public void setTypeColumn(CSDataBase.csDataType rhs)
        {
            m_typeColumn = rhs;
        }

        public String getValue()
        {
            return m_value;
        }

        public void setValue(String rhs)
        {
            m_value = rhs;
        }

        public int getPosition()
        {
            return m_position;
        }

        public void setPosition(int rhs)
        {
            m_position = rhs;
        }

        public bool getHasDefault()
        {
            return m_hasDefault;
        }

        public void setHasDefault(bool rhs)
        {
            m_hasDefault = rhs;
        }

        public String getDefaultValue()
        {
            return m_default;
        }

        public void setDefaultValue(String rhs)
        {
            m_default = rhs;
        }

        public bool getIsNullable()
        {
            return m_isNullable;
        }

        public void setIsNullable(bool rhs)
        {
            m_isNullable = rhs;
        }

        public int getMaxLength()
        {
            return m_maxLength;
        }

        public void setMaxLength(int rhs)
        {
            m_maxLength = rhs;
        }

        internal bool load(CSXml.cXml xDoc, XmlNode nodeObj)
        {
            m_typeColumn = (CSDataBase.csDataType)xDoc.getNodeProperty(nodeObj, "TypeColumn").getValueInt(eTypes.eInteger);
            m_value = xDoc.getNodeProperty(nodeObj, "Value").getValueString(eTypes.eText);
            m_position = xDoc.getNodeProperty(nodeObj, "Position").getValueInt(eTypes.eInteger);
            m_name = xDoc.getNodeProperty(nodeObj, "Name").getValueString(eTypes.eText);
            m_maxLength = xDoc.getNodeProperty(nodeObj, "MaxLength").getValueInt(eTypes.eInteger);
            m_key = xDoc.getNodeProperty(nodeObj, "Key").getValueString(eTypes.eText);
            m_isNullable = xDoc.getNodeProperty(nodeObj, "IsNullable").getValueBool(eTypes.eBoolean);
            m_hasDefault = xDoc.getNodeProperty(nodeObj, "HasDefault").getValueBool(eTypes.eBoolean);
            m_default = xDoc.getNodeProperty(nodeObj, "Default").getValueString(eTypes.eText);

            return true;
        }

        internal bool save(CSXml.cXml xDoc, XmlNode nodeFather)
        {
            CSXml.cXmlProperty xProperty = null;
            XmlNode nodeObj = null;
            xProperty = new CSXml.cXmlProperty();

            xProperty.setName(m_key);
            nodeObj = xDoc.addNodeToNode(nodeFather, xProperty);

            xProperty.setName("Key");
            xProperty.setValue(eTypes.eText, m_key);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            xProperty.setName("Name");
            xProperty.setValue(eTypes.eText, m_name);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            xProperty.setName("Position");
            xProperty.setValue(eTypes.eInteger, m_position);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            xProperty.setName("TypeColumn");
            xProperty.setValue(eTypes.eInteger, m_typeColumn);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            xProperty.setName("Value");
            xProperty.setValue(eTypes.eText, m_value);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            xProperty.setName("MaxLength");
            xProperty.setValue(eTypes.eInteger, m_maxLength);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            xProperty.setName("IsNullable");
            xProperty.setValue(eTypes.eBoolean, m_isNullable);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            xProperty.setName("HasDefault");
            xProperty.setValue(eTypes.eBoolean, m_hasDefault);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            xProperty.setName("Default");
            xProperty.setValue(eTypes.eText, m_default);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            return true;
        }

    }

}
