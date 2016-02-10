using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Drawing;
using CSKernelClient;

namespace CSReportDll
{
    public class cReportImage
    {
        private cReportAspect m_aspect;
        private Image m_image = null;

        public cReportImage()
        {
            m_aspect = new cReportAspect();
        }

        // TODO: check if we need to free image resources
        /*
        private void class_Terminate()
        {
            m_aspect = null;
            if (m_hImage != 0) { DeleteObject(m_hImage); }
        }
         * 
         */

        public cReportAspect getAspect()
        {
            return m_aspect;
        }

        public void setAspect(cReportAspect rhs)
        {
            m_aspect = rhs;
        }

        public Image getImage()
        {
            return m_image;
        }

        public void setImage(Image rhs)
        {
            m_image = rhs;
        }

        internal bool load(CSXml.cXml xDoc, XmlNode nodeObj)
        {
            nodeObj = xDoc.getNodeFromNode(nodeObj, "Image");
            byte[] vBytes = null;
            vBytes = xDoc.getBinaryNodeProperty(nodeObj, "Data").getBinaryValue();
            //
            // an empty image is serialized as AA== which is vBytes == [0] ( yes the number zero ) and vBytes.Length == 1
            //
            if (vBytes.Length > 1)
            {
                m_image = cImage.deSerialiseBitmap(vBytes);
            }
            G.redim(ref vBytes, 0);
            return m_aspect.load(xDoc, nodeObj);
        }

        internal bool save(CSXml.cXml xDoc, XmlNode nodeFather)
        {
            CSXml.cXmlProperty xProperty = null;
            XmlNode nodeObj = null;
            object nodImage = null;

            xProperty = new CSXml.cXmlProperty();
            xProperty.setName("Image");
            nodeObj = xDoc.addNodeToNode(nodeFather, xProperty);

            byte[] vBytes = null;
            if (getImage() != null)
            {
                cImage.serialiseBitmap(getImage(), vBytes);
            }
            else
            {
                G.redim(ref vBytes, 0);
            }
            xProperty.setName("Data");
            xProperty.setBinaryValue(vBytes);

            xDoc.addBinaryPropertyToNode(nodeObj, xProperty);
            G.redim(ref vBytes, 0);

            if (!m_aspect.save(xDoc, nodeObj))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }

}
