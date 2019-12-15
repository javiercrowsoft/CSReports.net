﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using CSKernelClient;
using CSReportGlobals;

namespace CSReportDll
{

    public class cReportControl
    {

        private cReportLabel m_label = new cReportLabel();
        private cReportImage m_image = new cReportImage();
        private cReportLine m_line = new cReportLine();
        private cReportField m_field = new cReportField();
        private csRptSectionType m_typeSection;
        private String m_key = "";
        private String m_keyPaint = "";
        private String m_name = "";
        private bool m_hasFormulaHide;
        private bool m_hasFormulaValue;
        private csRptControlType m_controlType;
        private cReportFormula m_formulaHide = new cReportFormula();
        private cReportFormula m_formulaValue = new cReportFormula();
        private cReportChart m_chart = new cReportChart();
        private String m_tag = "";
        private int m_exportColIdx = 0;
        private bool m_isFreeCtrl;

        // this reference tell in which section line is this control
        //
        private cReportSectionLine m_sectionLine;

        public cReportControl()
        {
            m_formulaHide.setName("H");
            m_formulaValue.setName("V");
        }

        public cReportLabel getLabel()
        {
            return m_label;
        }

        public void setLabel(cReportLabel rhs)
        {
            m_label = rhs;
        }

        public cReportImage getImage()
        {
            return m_image;
        }

        public void setImage(cReportImage rhs)
        {
            m_image = rhs;
        }

        public cReportFormula getFormulaHide()
        {
            return m_formulaHide;
        }

        public cReportFormula getFormulaValue()
        {
            return m_formulaValue;
        }

        public bool getHasFormulaValue()
        {
            return m_hasFormulaValue;
        }

        public void setHasFormulaValue(bool rhs)
        {
            m_hasFormulaValue = rhs;
        }

        public cReportLine getLine()
        {
            return m_line;
        }

        public void setLine(cReportLine rhs)
        {
            m_line = rhs;
        }

        public cReportField getField()
        {
            return m_field;
        }

        public void setField(cReportField rhs)
        {
            m_field = rhs;
        }

        public String getKey()
        {
            return m_key;
        }

        public void setKey(String rhs)
        {
            m_key = rhs;
        }

        public String getKeyPaint()
        {
            return m_keyPaint;
        }

        public void setKeyPaint(String rhs)
        {
            m_keyPaint = rhs;
        }

        public cReportChart getChart()
        {
            return m_chart;
        }

        public String getTag()
        {
            return m_tag;
        }

        public void setTag(String rhs)
        {
            m_tag = rhs;
        }

        public csRptSectionType getTypeSection()
        {
            return m_typeSection;
        }

        public void setTypeSection(csRptSectionType rhs)
        {
            m_typeSection = rhs;
        }

        public cReportSectionLine getSectionLine()
        {
            return m_sectionLine;
        }

        public void setSectionLine(cReportSectionLine rhs)
        {
            m_sectionLine = rhs;
        }

        public String getName()
        {
            return m_name;
        }

        public void setName(String rhs)
        {
            m_name = rhs;
        }

        public bool getHasFormulaHide()
        {
            return m_hasFormulaHide;
        }

        public void setHasFormulaHide(bool rhs)
        {
            m_hasFormulaHide = rhs;
        }

        public csRptControlType getControlType()
        {
            return m_controlType;
        }

        public void setControlType(csRptControlType rhs)
        {
            m_controlType = rhs;
        }

        public void setExportColIdx(int rhs)
        {
            m_exportColIdx = rhs;
        }

        public int getExportColIdx()
        {
            return m_exportColIdx;
        }

        public void setIsFreeCtrl(bool rhs)
        {
            m_isFreeCtrl = rhs;
        }

        public bool getIsFreeCtrl()
        {
            return m_isFreeCtrl;
        }

        internal bool load(CSXml.cXml xDoc, XmlNode nodeObj)
        {
            m_keyPaint = xDoc.getNodeProperty(nodeObj, "KeyPaint").getValueString(eTypes.eText);
            m_name = xDoc.getNodeProperty(nodeObj, "Name").getValueString(eTypes.eText);

            try { m_hasFormulaHide = xDoc.getNodeProperty(nodeObj, "HasFormulaHide").getValueBool(eTypes.eBoolean); }
            catch { }
            try { m_hasFormulaValue = xDoc.getNodeProperty(nodeObj, "HasFormulaValue").getValueBool(eTypes.eBoolean); }
            catch { }
            try { m_controlType = (csRptControlType)xDoc.getNodeProperty(nodeObj, "ControlType").getValueInt(eTypes.eInteger); }
            catch { }
            try { m_tag = xDoc.getNodeProperty(nodeObj, "Tag").getValueString(eTypes.eText); }
            catch { }
            try { m_exportColIdx = xDoc.getNodeProperty(nodeObj, "ExportColIdx").getValueInt(eTypes.eLong); }
            catch { }
            try { m_isFreeCtrl = xDoc.getNodeProperty(nodeObj, "IsFreeCtrl").getValueBool(eTypes.eBoolean); }
            catch { }

            try
            {
                if (!m_field.load(xDoc, nodeObj)) { return false; }
                if (!m_image.load(xDoc, nodeObj)) { return false; }
                if (!m_label.load(xDoc, nodeObj)) { return false; }
                if (!m_line.load(xDoc, nodeObj)) { return false; }
                if (!m_formulaHide.load(xDoc, nodeObj)) { return false; }
                if (!m_formulaValue.load(xDoc, nodeObj)) { return false; }
                if (!m_chart.load(xDoc, nodeObj)) { return false; }

                // TODO: remove me after all reports were migrated
                //
                if (m_label.getAspect().getFormat() == "" && m_field.getFieldType() == (int)CSDataBase.csAdoDataType.adDBTimeStamp)
                {
                    m_label.getAspect().setFormat("dd/MM/yyyy");
                }

                return true;
            }
            catch(Exception ex) 
            { 
                return false; 
            }
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

            xProperty.setName("KeyPaint");
            xProperty.setValue(eTypes.eText, m_keyPaint);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            xProperty.setName("HasFormulaHide");
            xProperty.setValue(eTypes.eBoolean, m_hasFormulaHide);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            xProperty.setName("HasFormulaValue");
            xProperty.setValue(eTypes.eBoolean, m_hasFormulaValue);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            xProperty.setName("ControlType");
            xProperty.setValue(eTypes.eInteger, m_controlType);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            xProperty.setName("Tag");
            xProperty.setValue(eTypes.eText, m_tag);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            xProperty.setName("ExportColIdx");
            xProperty.setValue(eTypes.eLong, m_exportColIdx);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            xProperty.setName("IsFreeCtrl");
            xProperty.setValue(eTypes.eBoolean, m_isFreeCtrl);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            if (!m_field.save(xDoc, nodeObj)) { return false; }
            if (!m_image.save(xDoc, nodeObj)) { return false; }
            if (!m_label.save(xDoc, nodeObj)) { return false; }
            if (!m_line.save(xDoc, nodeObj)) { return false; }
            if (!m_formulaHide.save(xDoc, nodeObj)) { return false; }
            if (!m_formulaValue.save(xDoc, nodeObj)) { return false; }
            if (!m_chart.save(xDoc, nodeObj)) { return false; }
            return true;
        }

    }

}
