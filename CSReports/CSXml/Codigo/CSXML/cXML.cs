using System;
using System.Xml;
using System.IO;
using CSKernelClient;

namespace CSXml
{

    public class cXml : IDisposable
    {

        private const string C_MODULE = "cXml";

        private string m_name = "";
        private string m_path = "";
        private XmlDocument m_domDoc = new XmlDocument();
        private object m_commDialog = null;
        private string m_filter = "";

        public string getName()
        {
            return m_name;
        }

        public void setName(string rhs)
        {
            m_name = rhs;
        }

        public string getPath()
        {
            string _rtn = "";
            if (m_path.Substring(m_path.Length - 1) == Path.DirectorySeparatorChar.ToString())
            {
                _rtn = m_path;
            }
            else
            {
                _rtn = m_path + Path.DirectorySeparatorChar;
            }
            return _rtn;
        }

        public void setPath(string rhs)
        {
            m_path = rhs;
        }

        public string getFilter()
        {
            return m_filter;
        }

        public void setFilter(string rhs)
        {
            m_filter = rhs;
        }

        public void init(object commDialog)
        {
            m_commDialog = commDialog;
        }

        public bool openXmlWithDialog()
        {
            try
            {
                CSKernelFile.cFile file = new CSKernelFile.cFile();
                file.setFilter(m_filter);
                file.init("OpenXmlWithDialog", C_MODULE, m_commDialog);

                if (!file.open(m_name,
                                eFileMode.eRead, 
                                false, 
                                false, 
                                eFileAccess.eLockReadWrite, 
                                true, 
                                true)) 
                { 
                    return false; 
                }

                m_name = file.getName();
                m_path = file.getPath();

                file.close();

                file = null;

                return openXml();

            }
            catch (Exception ex)
            {
                cError.mngError(ex, "OpenXmlWithDialog", C_MODULE, "There was an error trying to open file: " + m_name);
                return false;
            }
        }

        public bool openXml()
        {
            try
            {
                string file = "";
                m_domDoc = new XmlDocument();
                file = getPath() + m_name;

                CSKernelFile.cFileEx fileEx = new CSKernelFile.cFileEx();
                if (fileEx.fileExists(file))
                {
                    if (!loadXml(file))
                    {
                        return false;
                    }
                }
                else
                {
                    cWindow.msgWarning("The file;;" + file + ";;doesnt exists.");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                cError.mngError(ex, "OpenXml", C_MODULE, "There was an error trying to open the file: " + m_name);
                return false;
            }
        }

        public bool newXmlWithDialog()
        {
            try
            {
                string msg = "";
                CSKernelFile.cFile file = new CSKernelFile.cFile();

                file.init("NewXmlWithDialog", C_MODULE, m_commDialog);
                file.setFilter(m_filter);

                bool bExists = false;
                bool bReadonly = false;

                if (!file.save(m_name, out bExists, out bReadonly, "")) 
                { 
                    return false; 
                }

                if (bExists && bReadonly)
                {
                    msg = "There is already a file with this name and it is read only. Do you want to replace this file?";
                }
                else if (bExists)
                {
                    if (m_name != file.getName())
                    {
                        msg = "There is already a file with this name. Do you want to replace this file?";
                    }
                }

                if (msg != "")
                {
                    if (!cWindow.ask(msg, VbMsgBoxResult.vbNo, "Saving")) 
                    { 
                        return false; 
                    }
                }

                m_name = file.getName();
                m_path = file.getPath();

                file = null;

                return newXml();
            }
            catch (Exception ex)
            {
                cError.mngError(ex, "NewXmlWithDialog", C_MODULE, "There was an error trying to create the file: " + m_name);
                return false;
            }
        }

        public bool newXml()
        {
            try
            {
                m_domDoc = new XmlDocument();
                XmlNode node = m_domDoc.CreateNode(XmlNodeType.Element, "Root", "");
                m_domDoc.AppendChild(node);

                return true;
            }
            catch (Exception ex)
            {
                cError.mngError(ex, "NewXml", C_MODULE, "There was an error trying to create the file: " + m_name);
                return false;
            }
        }

        public bool saveWithDialog()
        {
            try
            {
                CSKernelFile.cFile file = new CSKernelFile.cFile();

                if (!file.open(m_name, eFileMode.eWrite, false, false, eFileAccess.eLockWrite, false, false)) 
                { 
                    return false; 
                }

                m_name = file.getName();
                m_path = file.getPath();

                file = null;

                return save();
            }
            catch (Exception ex)
            {
                cError.mngError(ex, "SaveWithDialog", C_MODULE, "There was an error trying to save the file: " + m_name);
                return false;
            }            
        }

        public void setNodeText(XmlNode node, string text)
        {
            node.Value = text;
        }

        public bool save()
        {
            try
            {
                m_domDoc.Save(getPath() + m_name);
                return true;
            }
            catch (Exception ex)
            {
                cError.mngError(ex, "Save", C_MODULE, "There was an error trying to save the file: " + m_name);
                return false;
            }
        }

        public bool addProperty(cXmlProperty xProperty)
        { 
            return addPropertyToNodeByTag("Root", xProperty);
        }

        public bool addPropertyToNodeByTag(string nodeTag, cXmlProperty xProperty)
        {
            XmlNodeList w_element = m_domDoc.GetElementsByTagName(nodeTag);
            return addPropertyToNode(w_element.Item(0), xProperty);
        }

        public bool addPropertyToNode(XmlNode node, cXmlProperty xProperty)
        {
            XmlAttribute attr = m_domDoc.CreateAttribute(xProperty.getName());
            attr.Value = xProperty.getValueString(eTypes.eVariant);
            node.Attributes.Append(attr);
            return true;
        }

        public bool addBinaryPropertyToNode(XmlNode node, cXmlProperty xProperty)
        {
            XmlAttribute attr = m_domDoc.CreateAttribute(xProperty.getName());
            attr.Value = Convert.ToBase64String(xProperty.getBinaryValue());
            node.Attributes.Append(attr);
            return true;
        }

        public XmlNode addNode(cXmlProperty xProperty)
        {
            return addNodeToNodeByTag("Root", xProperty);
        }

        public XmlNode addNodeToNodeByTag(string nodeTag, cXmlProperty xProperty)
        {
            XmlNodeList w_element = m_domDoc.GetElementsByTagName(nodeTag);
            return addNodeToNode(w_element[0], xProperty);
        }

        public XmlNode addNodeToNode(XmlNode nodeFather, cXmlProperty xProperty)
        {
            XmlNode node = m_domDoc.CreateNode(XmlNodeType.Element, xProperty.getName(), "");
            nodeFather.AppendChild(node);
            return node;
        }

        public XmlNode getRootNode()
        {
            if (m_domDoc.GetElementsByTagName("Root").Count > 0)
            {
                return m_domDoc.GetElementsByTagName("Root")[0];
            }
            else
            {
                return null;
            }
        }

        public XmlNode getNode(string nodeTag)
        {
            if (m_domDoc.GetElementsByTagName(nodeTag).Count > 0)
            {
                return m_domDoc.GetElementsByTagName(nodeTag)[0];
            }
            else
            {
                return null;
            }
        }

        public XmlNode getNodeFromNode(XmlNode node, string nodeTag)
        {
            return node.SelectSingleNode(nodeTag);
        }

        public XmlNode getNodeChild(XmlNode node)
        {
            if (nodeHasChild(node))
            {
                return node.ChildNodes[0];
            }
            else
            {
                return null;
            }
        }

        public XmlNode getNextNode(XmlNode node)
        { 
            return node.NextSibling;
        }

        public cXmlProperty getNodeValue(XmlNode node)
        {
            cXmlProperty o = null;
            o = new cXmlProperty();
            o.setValue(eTypes.eText, node.Name);
            return o;
        }

        public cXmlProperty getNodeProperty(XmlNode node, string propertyName)
        {
            cXmlProperty o = new cXmlProperty();
            string txt = "";

            if (node.Attributes[propertyName] != null)
            {
                txt = node.Attributes[propertyName].Value;
            }

            // TODO: remove after testing
            //txt = txt.Replace("\n", "\\n");
            o.setValue(eTypes.eVariant, txt);
            return o;
        }

        public cXmlProperty getBinaryNodeProperty(XmlNode node, string propertyName)
        {
            XmlAttribute attr = null;
            cXmlProperty o = new cXmlProperty();
            byte[] vBuffer = null;

            XmlElement element = (XmlElement)node;
            attr = element.GetAttributeNode(propertyName);
            if (attr != null)
            {
                vBuffer = System.Convert.FromBase64String(attr.Value);
            }
            else
            {
                G.redim(ref vBuffer, 0);
            }

            o.setBinaryValue(vBuffer);
            return o;
        }

        public bool nodeHasChild(XmlNode node)
        {
            return node.ChildNodes.Count > 0;
        }

        private bool loadXml(string file)
        {
            try
            {
                m_domDoc.Load(file);
                return true;
            }
            catch (Exception ex)
            {
                cWindow.msgWarning("Open file has failded.;;" 
                                    + file 
                                    + ";;Error: " 
                                    + ex.Message);
                return false;
            }
        }

        public void Dispose()
        {
            m_domDoc = null;
            m_commDialog = null;
        }
    }
}
