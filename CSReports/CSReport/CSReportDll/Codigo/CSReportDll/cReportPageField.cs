using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Drawing;
using CSKernelClient;

namespace CSReportDll
{
    public class cReportPageField
    {

        private const String C_MODULE = "cReportPageField";

        private String m_value = "";
        private cReportPageInfo m_info;
        private bool m_visible;
        private cReportPageID m_objectID;
        private int m_indexLine = 0;
        private float m_top = 0;
        private float m_height = 0;
        private float m_width = 0;
        private Image m_image = null;

        public String getValue()
        {
            return m_value;
        }

        public void setValue(String rhs)
        {
            m_value = rhs;
        }

        public cReportPageInfo getInfo()
        {
            return m_info;
        }

        public void setInfo(cReportPageInfo rhs)
        {
            m_info = rhs;
        }

        public bool getVisible()
        {
            return m_visible;
        }

        public void setVisible(bool rhs)
        {
            m_visible = rhs;
        }

        public cReportPageID getObjectID()
        {
            return m_objectID;
        }

        public void setObjectID(cReportPageID rhs)
        {
            m_objectID = rhs;
        }

        public float getTop()
        {
            return m_top;
        }

        public void setTop(float rhs)
        {
            m_top = rhs;
        }

        public float getHeight()
        {
            return m_height;
        }

        public void setHeight(float rhs)
        {
            m_height = rhs;
        }

        public float getWidth()
        {
            return m_width;
        }

        public void setWidth(float rhs)
        {
            m_width = rhs;
        }

        public Image getImage()
        {
            return m_image;
        }

        public void setImage(Image rhs)
        {
            m_image = rhs;
        }

        public int getIndexLine()
        {
            return m_indexLine;
        }

        public void setIndexLine(int rhs)
        {
            m_indexLine = rhs;
        }

        internal bool load(CSXml.cXml xDoc, XmlNode nodeObj)
        {
            m_objectID = new cReportPageID();
            m_info = new cReportPageInfo();

            m_value = xDoc.getNodeProperty(nodeObj, "Value").getValueString(eTypes.eText);
            m_visible = xDoc.getNodeProperty(nodeObj, "Visible").getValueBool(eTypes.eBoolean);
            m_top = xDoc.getNodeProperty(nodeObj, "Top").getValueInt(eTypes.eLong);
            m_height = xDoc.getNodeProperty(nodeObj, "Height").getValueInt(eTypes.eLong);
            m_width = xDoc.getNodeProperty(nodeObj, "Width").getValueInt(eTypes.eLong);

            XmlNode nodeObjAux = null;
            nodeObjAux = nodeObj;
            if (!m_objectID.load(xDoc, nodeObjAux)) 
            { 
                return false; 
            }
            nodeObjAux = nodeObj;
            if (!m_info.load(xDoc, nodeObjAux)) 
            { 
                return false; 
            }

            return true;
        }

        internal bool save(CSXml.cXml xDoc, XmlNode nodeFather)
        {
            CSXml.cXmlProperty xProperty = null;
            XmlNode nodeObj = null;
            xProperty = new CSXml.cXmlProperty();

            xProperty.setName("Field");
            nodeObj = xDoc.addNodeToNode(nodeFather, xProperty);

            xProperty.setName("Value");
            xProperty.setValue(eTypes.eText, m_value);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            xProperty.setName("Visible");
            xProperty.setValue(eTypes.eBoolean, m_visible);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            xProperty.setName("Top");
            xProperty.setValue(eTypes.eLong, m_top);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            xProperty.setName("Height");
            xProperty.setValue(eTypes.eLong, m_height);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            xProperty.setName("Width");
            xProperty.setValue(eTypes.eLong, m_width);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            if (m_objectID != null)
            {
                if (!m_objectID.save(xDoc, nodeObj))
                {
                    return false;
                }
            }
            if (!m_info.save(xDoc, nodeObj)) 
            { 
                return false; 
            }

            return true;  
        }

        public bool saveForWeb(CSXml.cXml xDoc, XmlNode nodeFather)
        {
            CSXml.cXmlProperty xProperty = null;
            XmlNode nodeObj = null;
            xProperty = new CSXml.cXmlProperty();

            xProperty.setName("Field");
            nodeObj = xDoc.addNodeToNode(nodeFather, xProperty);

            xProperty.setName("Value");
            xProperty.setValue(eTypes.eText, m_value);
            nodeObj = xDoc.addNodeToNode(nodeObj, xProperty);
            xDoc.setNodeText(nodeObj, m_value);

            return true;
        }

    }

}
