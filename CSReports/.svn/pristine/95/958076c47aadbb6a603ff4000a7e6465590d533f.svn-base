using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using CSKernelClient;
using CSReportGlobals;

namespace CSReportDll
{

    public class cReportSectionLine : IDisposable
    {

        private const String C_NODERPTCONTROLS = "RptControls";

        private cReportControls m_controls = new cReportControls();
        private cReportAspect m_aspect = new cReportAspect();
        private int m_index = 0;
        private int m_realIndex = 0;
        private String m_key = "";
        private String m_keyPaint = "";
        private cReportFormula m_formulaHide = new cReportFormula();
        private bool m_hasFormulaHide;

        // it is the name of the control which have the id of the line
        // it is used by cReportLinkServer
        // when a user makes double clic over a line in a preview report
        // window the showDetails() event of cReportLinkServer will be raised
        // a listener for this event could use this property to know which
        // control contains the id of the record expressed in the line selected
        // by the user.
        //
        private String m_idField = "";

        // for debugging
        //
        private String m_sectionName = "";

        public cReportSectionLine() 
        {
            m_controls.setSectionLine(this);
            m_formulaHide.setName("H");
        }

        public String getKeyPaint()
        {
            return m_keyPaint;
        }

        public void setKeyPaint(String rhs)
        {
            m_keyPaint = rhs;
        }

        public cReportControls getControls()
        {
            return m_controls;
        }

        public void setControls(cReportControls rhs)
        {
            m_controls = rhs;
        }

        public String getIdField()
        {
            return m_idField;
        }

        public void setIdField(String rhs)
        {
            m_idField = rhs;
        }

        public String getKey()
        {
            return m_key;
        }

        public void setKey(String rhs)
        {
            m_key = rhs;
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

        public int getrealIndex()
        {
            return m_realIndex;
        }

        public void setrealIndex(int rhs)
        {
            m_realIndex = rhs;
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

        public csRptTypeSection getTypeSection()
        {
            return m_controls.getTypeSection();
        }

        public void setTypeSection(csRptTypeSection rhs)
        {
            m_controls.setTypeSection(rhs);
        }

        public String getSectionName()
        {
            return m_sectionName;
        }

        public void setSectionName(String rhs)
        {
            m_sectionName = rhs;
        }

        public void setCopyColl(cReportControls2 rhs)
        {
            if (m_controls != null)
                m_controls.setCopyColl(rhs);
        }

        internal bool load(CSXml.cXml xDoc, XmlNode nodeObj) 
        {
            XmlNode nodeObjCtrls = null;
            XmlNode nodeObjCtrl = null;
            XmlNode nodeObjAspect = null;

            cReportControl ctrl = null;

            m_index = xDoc.getNodeProperty(nodeObj, "Index").getValueInt(eTypes.eInteger);
            m_idField = xDoc.getNodeProperty(nodeObj, "IdField").getValueString(eTypes.eText);
            m_hasFormulaHide = xDoc.getNodeProperty(nodeObj, "HasFormulaHide").getValueBool(eTypes.eBoolean);

            nodeObjAspect = nodeObj;

            XmlNode nodeObjAux = nodeObj;
            if (!m_formulaHide.load(xDoc, nodeObjAux)) 
            { 
                return false; 
            }

            if (!m_aspect.load(xDoc, nodeObjAspect)) 
            { 
                return false; 
            }

            nodeObjCtrls = xDoc.getNodeFromNode(nodeObj, C_NODERPTCONTROLS);

            if (xDoc.nodeHasChild(nodeObjCtrls)) 
            {
                nodeObjCtrl = xDoc.getNodeChild(nodeObjCtrls);

                while (nodeObjCtrl != null) {
                    String key = xDoc.getNodeProperty(nodeObjCtrl, "Key").getValueString(eTypes.eText);
                    ctrl = m_controls.add(null, key);
                    if (!ctrl.load(xDoc, nodeObjCtrl)) 
                    { 
                        return false; 
                    }
                    nodeObjCtrl = xDoc.getNextNode(nodeObjCtrl);
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

            xProperty.setName("Key");
            xProperty.setValue(eTypes.eText, m_key);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            xProperty.setName("Index");
            xProperty.setValue(eTypes.eInteger, m_index);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            xProperty.setName("IdField");
            xProperty.setValue(eTypes.eText, m_idField);
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

            xProperty.setName(C_NODERPTCONTROLS);
            nodeObj = xDoc.addNodeToNode(nodeObj, xProperty);

            cReportControl ctrl = null;
            for (int _i = 0; _i < m_controls.count(); _i++)
            {
                ctrl = m_controls.item(_i);
                ctrl.save(xDoc, nodeObj);
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
        ~cReportSectionLine()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }

        private void releaseReferences()
        {
            setCopyColl(null);

            if (m_controls != null)
            {
                if (m_controls.getCopyColl() != null)
                {
                    m_controls.getCopyColl().clear();
                    m_controls.setCopyColl(null);
                }
                m_controls.clear();
                m_controls = null;
            }

            m_aspect = null;
            m_formulaHide = null;
        }
    }

}
