using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using CSKernelClient;
using CSReportGlobals;

namespace CSReportDll
{

    public class cReportSection : IDisposable, cIReportSection
    {

        private const String C_NODERPTSECTIONLINES = "RptSectionLines";

        private cReportSectionLines m_sectionLines = new cReportSectionLines();
        private cReportAspect m_aspect = new cReportAspect();
        private int m_index = 0;
        private int m_realIndex = 0;
        private String m_key = "";
        private String m_name = "";
        private String m_keyPaint = "";
        private cReportFormula m_formulaHide = new cReportFormula();
        private bool m_hasFormulaHide;

        public cReportSection()
        {
            m_formulaHide.setName("H");

            // when a new section is create a new line section 
            // is automatically added
            // 
            m_sectionLines.add(null, "", -1);
        }

        public cReportSectionLines getSectionLines()
        {
            return m_sectionLines;
        }

        public void setSectionLines(cReportSectionLines rhs)
        {
            m_sectionLines = rhs;
        }

        public cReportAspect getAspect()
        {
            return m_aspect;
        }

        public void setAspect(cReportAspect rhs)
        {
            m_aspect = rhs;
        }

        public int getIndex()
        {
            return m_index;
        }

        public void setIndex(int rhs)
        {
            m_index = rhs;
        }

        public int getRealIndex()
        {
            return m_realIndex;
        }

        public void setRealIndex(int rhs)
        {
            m_realIndex = rhs;
        }

        public String getKey()
        {
            return m_key;
        }

        public void setKey(String rhs)
        {
            m_key = rhs;
        }

        public csRptSectionType getTypeSection()
        {
            return m_sectionLines.getTypeSection();
        }

        public void setTypeSection(csRptSectionType rhs)
        {
            m_sectionLines.setTypeSection(rhs);
        }

        public String getName()
        {
            return m_name;
        }

        public void setName(String rhs)
        {
            m_name = rhs;
        }

        internal void setCopyColl(cReportControls2 rhs)
        {
            if (m_sectionLines != null)
            {
                m_sectionLines.setCopyColl(rhs);
            }
        }

        public String getKeyPaint()
        {
            return m_keyPaint;
        }

        public void setKeyPaint(String rhs)
        {
            m_keyPaint = rhs;
        }

        public bool getHasFormulaHide()
        {
            return m_hasFormulaHide;
        }

        public void setHasFormulaHide(bool rhs)
        {
            m_hasFormulaHide = rhs;
        }

        public cReportFormula getFormulaHide()
        {
            return m_formulaHide;
        }

        internal bool load(CSXml.cXml xDoc, XmlNode nodeObj)
        {
            XmlNode nodeObjSecLn = null;
            XmlNode nodeObjAspect = null;
            cReportSectionLine secLn = null;

            m_name = xDoc.getNodeProperty(nodeObj, "Name").getValueString(eTypes.eText);

            // TODO: fix me - this is Spanish - English bug we should use Index
            //
            m_index = xDoc.getNodeProperty(nodeObj, "Indice").getValueInt(eTypes.eInteger);

            setTypeSection((csRptSectionType)xDoc.getNodeProperty(nodeObj, "TypeSection").getValueInt(eTypes.eInteger));
            m_hasFormulaHide = xDoc.getNodeProperty(nodeObj, "HasFormulaHide").getValueBool(eTypes.eBoolean);

            nodeObjAspect = nodeObj;
            if (!m_aspect.load(xDoc, nodeObjAspect))
            {
                return false;
            }

            XmlNode nodeObjAux = nodeObj;
            if (!m_formulaHide.load(xDoc, nodeObjAux))
            {
                return false;
            }

            m_sectionLines.clear();

            nodeObj = xDoc.getNodeFromNode(nodeObj, C_NODERPTSECTIONLINES);
            if (xDoc.nodeHasChild(nodeObj))
            {
                nodeObjSecLn = xDoc.getNodeChild(nodeObj);
                while (nodeObjSecLn != null)
                {
                    String key = xDoc.getNodeProperty(nodeObjSecLn, "Key").getValueString(eTypes.eText);
                    secLn = m_sectionLines.add(null, key, -1);
                    if (!secLn.load(xDoc, nodeObjSecLn))
                    {
                        return false;
                    }
                    secLn.setSectionName(m_name);
                    nodeObjSecLn = xDoc.getNextNode(nodeObjSecLn);
                }
            }

            return true;
        }

        internal bool save(CSXml.cXml xDoc, XmlNode nodeFather)
        {
            CSXml.cXmlProperty xProperty = null;
            XmlNode nodeObj = null;

            xProperty = new CSXml.cXmlProperty();

            xProperty.setName(m_key);
            nodeObj = xDoc.addNodeToNode(nodeFather, xProperty);

            xProperty.setName("Name");
            xProperty.setValue(eTypes.eText, m_name);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            xProperty.setName("Key");
            xProperty.setValue(eTypes.eText, m_key);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            // TODO: fix me - this is Spanish - English bug we should use Index
            //
            xProperty.setName("Indice");
            xProperty.setValue(eTypes.eInteger, m_index);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            xProperty.setName("TypeSection");
            xProperty.setValue(eTypes.eInteger, getTypeSection());
            xDoc.addPropertyToNode(nodeObj, xProperty);

            xProperty.setName("HasFormulaHide");
            xProperty.setValue(eTypes.eBoolean, m_hasFormulaHide);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            if (!m_aspect.save(xDoc, nodeObj)) 
            { 
                return false; 
            }
            if (!m_formulaHide.save(xDoc, nodeObj)) 
            { 
                return false; 
            }

            xProperty.setName(C_NODERPTSECTIONLINES);
            xProperty.setValue(eTypes.eText, "");
            nodeObj = xDoc.addNodeToNode(nodeObj, xProperty);

            cReportSectionLine seccLn = null;
            for (int _i = 0; _i < m_sectionLines.count(); _i++)
            {
                seccLn = m_sectionLines.item(_i);
                seccLn.save(xDoc, nodeObj);
            }

            return true;
        }

        // Implement IDisposable.
        // Do not make this method virtual.
        // A derived class should not be able to override this method.
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        // Track whether Dispose has been called.
        private bool disposed = false;

        // Dispose(bool disposing) executes in two distinct scenarios.
        // If disposing equals true, the method has been called directly
        // or indirectly by a user's code. Managed and unmanaged resources
        // can be disposed.
        // If disposing equals false, the method has been called by the
        // runtime from inside the finalizer and you should not reference
        // other objects. Only unmanaged resources can be disposed.
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    releaseReferences();
                }

                // Note disposing has been done.
                disposed = true;

            }
        }

        // Use C# destructor syntax for finalization code.
        // This destructor will run only if the Dispose method
        // does not get called.
        // It gives your base class the opportunity to finalize.
        // Do not provide destructors in types derived from this class.
        ~cReportSection()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }

        private void releaseReferences()
        {
            if (m_sectionLines != null)
            {
                if (m_sectionLines.getCopyColl() != null)
                {
                    m_sectionLines.getCopyColl().clear();
                    m_sectionLines.setCopyColl(null);
                }
                m_sectionLines = null;
            }
            m_aspect = null;
            m_formulaHide = null;
        }

    }

}
