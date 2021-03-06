﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Reflection;
using CSKernelClient;
using CSReportGlobals;

namespace CSReportDll
{

    public class cReportFormula
    {

        private String m_name = "";
        private String m_text = "";
        private cReportFormulasInt m_formulasInt = new cReportFormulasInt();
        private bool m_notSave;

        // when we compile a function we parse the text and extract
        // from the script all internal functions
        // every internal function is added to the collection m_FormulasInt
        // and replaced in the script by an String $$$n  
        // n is the index of the function in m_FormulasInt
        // when we run the script every occurrence of $$$n is replaced for
        // the value of their corresponding function
        // finaly if the text contains an script we evalute this with the
        // ScriptControl
        // 
        // compiled text of the function
        //
        private String m_textC = "";
        private int m_idxGroup = 0;
        private int m_idxGroup2 = -9999;
        private csRptWhenEval m_whenEval;
        private bool m_haveToEval;
        private object m_lastResult = null;

        // for debugging
        //
        private String m_controlName = "";
        private int m_sectionLineIndex = 0;
        private String m_sectionName = "";

        private Assembly m_compiledScript;

        public Assembly getCompiledScript()
        {
            return m_compiledScript;
        }

        public void setCompiledScript(Assembly value)
        {
            m_compiledScript = value;
        }

        public int getIdxGroup()
        {
            return m_idxGroup;
        }

        public void setIdxGroup(int rhs)
        {
            m_idxGroup = rhs;
        }

        public int getIdxGroup2()
        {
            return m_idxGroup2;
        }

        public void setIdxGroup2(int rhs)
        {
            m_idxGroup2 = rhs;
        }

        public csRptWhenEval getWhenEval()
        {
            return m_whenEval;
        }

        public void setWhenEval(csRptWhenEval rhs)
        {
            m_whenEval = rhs;
        }

        public String getName()
        {
            return m_name;
        }

        public void setName(String rhs)
        {
            m_name = rhs;
        }

        public String getText()
        {
            return m_text;
        }

        public void setText(String rhs)
        {
            m_text = rhs;
        }

        public String getControlName()
        {
            return m_controlName;
        }

        public void setControlName(String rhs)
        {
            m_controlName = rhs;
        }

        public String getSectionName()
        {
            return m_sectionName;
        }

        public void setSectionName(String rhs)
        {
            m_sectionName = rhs;
        }

        public int getSectionLineIndex()
        {
            return m_sectionLineIndex;
        }

        public void setSectionLineIndex(int rhs)
        {
            m_sectionLineIndex = rhs;
        }

        public cReportFormulasInt getFormulasInt()
        {
            return m_formulasInt;
        }

        public String getTextC()
        {
            return m_textC;
        }

        public void setTextC(String rhs)
        {
            m_textC = rhs;
        }

        public bool getNotSave()
        {
            return m_notSave;
        }

        public void setNotSave(bool rhs)
        {
            m_notSave = rhs;
        }

        public bool getHaveToEval()
        {
            return m_haveToEval;
        }

        public void setHaveToEval(bool rhs)
        {
            m_haveToEval = rhs;
        }

        public object getLastResult()
        {
            return m_lastResult;
        }

        public void setLastResult(object rhs)
        {
            m_lastResult = rhs;
        }

        internal bool load(CSXml.cXml xDoc, XmlNode nodeObj)
        {
            nodeObj = xDoc.getNodeFromNode(nodeObj, m_name);

            if (nodeObj != null)
            {
                m_name = xDoc.getNodeProperty(nodeObj, "Name").getValueString(eTypes.eText);
                m_text = xDoc.getNodeProperty(nodeObj, "Text").getValueString(eTypes.eText);
                m_idxGroup = xDoc.getNodeProperty(nodeObj, "idxGroup").getValueInt(eTypes.eLong);
                m_whenEval = (csRptWhenEval)xDoc.getNodeProperty(nodeObj, "WhenEval").getValueInt(eTypes.eInteger);
            }

            return true;
        }

        internal bool save(CSXml.cXml xDoc, XmlNode nodeFather)
        {
            CSXml.cXmlProperty xProperty = null;
            XmlNode nodeObj = null;
            xProperty = new CSXml.cXmlProperty();

            xProperty.setName(m_name);
            nodeObj = xDoc.addNodeToNode(nodeFather, xProperty);

            xProperty.setName("Name");
            xProperty.setValue(eTypes.eText, m_name);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            xProperty.setName("Text");
            xProperty.setValue(eTypes.eText, m_text);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            xProperty.setName("idxGroup");
            xProperty.setValue(eTypes.eLong, m_idxGroup);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            xProperty.setName("WhenEval");
            xProperty.setValue(eTypes.eInteger, m_whenEval);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            return true;
        }

    }

}
