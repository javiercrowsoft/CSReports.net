using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using CSKernelClient;
using CSReportGlobals;
using System.Windows.Forms;

namespace CSReportDll
{

    public class cReportLaunchInfo
    {

        private const String C_LAUNCHINFO = "RptLaunchInfo";

        private String m_file = "";
        private object m_dataSource = null;
        private String m_sqlstmt = "";
        private String m_strConnect = "";
        private cPrinter m_printer;
        private bool m_showPrintersDialog;
        private bool m_internalPreview;
        private csRptLaunchAction m_action;
        private int m_copies = 0;
        private bool m_silent;
        private csRptFileFormat m_fileFormat;
        private int m_hWnd = 0;

        private CSIReportPrint.cIReportPrint m_objPaint;

        public int getHwnd()
        {
            return m_hWnd;
        }

        public void setHwnd(int rhs)
        {
            m_hWnd = rhs;
        }

        public String getFile()
        {
            return m_file;
        }

        public void setFile(String rhs)
        {
            m_file = rhs;
        }

        public object getDataSource()
        {
            return m_dataSource;
        }

        public void setDataSource(object rhs)
        {
            m_dataSource = rhs;
        }

        public String getSqlstmt()
        {
            return m_sqlstmt;
        }

        public void setSqlstmt(String rhs)
        {
            m_sqlstmt = rhs;
        }

        public String getStrConnect()
        {
            return m_strConnect;
        }

        public void setStrConnect(String rhs)
        {
            m_strConnect = rhs;
        }

        // System.Drawing.Printing.PrinterSettings
        //
        public cPrinter getPrinter()
        {
            return m_printer;
        }

        // System.Drawing.Printing.PrinterSettings
        //
        public void setPrinter(cPrinter rhs)
        {
            m_printer = rhs;
        }

        public csRptFileFormat getFileFormat()
        {
            return m_fileFormat;
        }

        public void setFileFormat(csRptFileFormat rhs)
        {
            m_fileFormat = rhs;
        }

        public CSIReportPrint.cIReportPrint getObjPaint()
        {
            return m_objPaint;
        }

        public void setObjPaint(CSIReportPrint.cIReportPrint rhs)
        {
            m_objPaint = rhs;
        }

        public csRptLaunchAction getAction()
        {
            return m_action;
        }

        public void setAction(csRptLaunchAction rhs)
        {
            m_action = rhs;
        }

        public bool getShowPrintersDialog()
        {
            return m_showPrintersDialog;
        }

        public void setShowPrintersDialog(bool rhs)
        {
            m_showPrintersDialog = rhs;
        }

        public bool getInternalPreview()
        {
            return m_internalPreview;
        }

        public void setInternalPreview(bool rhs)
        {
            m_internalPreview = rhs;
        }

        public int getCopies()
        {
            return m_copies;
        }

        public void setCopies(int rhs)
        {
            m_copies = rhs;
        }

        public bool getSilent()
        {
            return m_silent;
        }

        public void setSilent(bool rhs)
        {
            m_silent = rhs;
        }

        public void initPrinter(PrintDialog printDialog, String deviceName, String driverName, String port)
        {
            m_printer = cPrintAPI.getcPrint(printDialog, deviceName, driverName, port);
        }

        public void setPaperBin(String paperBin)
        {
            if (m_printer == null) { return; }

            if (paperBin.Length == 0)
            {
                int idPaperBin = 0;
                idPaperBin = cPrintAPI.printerPaperBinNameToId(m_printer.getDeviceName(), 
                                                                m_printer.getPort(), 
                                                                paperBin);
                m_printer.getPaperInfo().setPaperBin(idPaperBin);
            }
        }

        internal bool load(CSXml.cXml xDoc, XmlNode nodeObj)
        { 
            m_strConnect = xDoc.getNodeProperty(nodeObj, "StrConnect").getValueString(eTypes.eText);
            m_action = (csRptLaunchAction)xDoc.getNodeProperty(nodeObj, "Action").getValueInt(eTypes.eInteger);
            m_copies = xDoc.getNodeProperty(nodeObj, "Copies").getValueInt(eTypes.eInteger);
            m_file = xDoc.getNodeProperty(nodeObj, "File").getValueString(eTypes.eText);
            m_fileFormat = (csRptFileFormat)xDoc.getNodeProperty(nodeObj, "FileFormat").getValueInt(eTypes.eInteger);
            m_internalPreview = xDoc.getNodeProperty(nodeObj, "InternalPreview").getValueBool(eTypes.eBoolean);
            m_showPrintersDialog = xDoc.getNodeProperty(nodeObj, "ShowPrintersDialog").getValueBool(eTypes.eBoolean);
            m_silent = xDoc.getNodeProperty(nodeObj, "Silent").getValueBool(eTypes.eBoolean);
            m_sqlstmt = xDoc.getNodeProperty(nodeObj, "Sqlstmt").getValueString(eTypes.eText);

            return true;
        }

        internal bool save(CSXml.cXml xDoc, XmlNode nodeFather)
        {
            CSXml.cXmlProperty xProperty = new CSXml.cXmlProperty();
            XmlNode nodeObj = null;

            xProperty.setName(C_LAUNCHINFO);

            if (nodeFather != null)
            {
                nodeObj = xDoc.addNodeToNode(nodeFather, xProperty);
            }
            else
            {
                nodeObj = xDoc.addNode(xProperty);
            }

            xProperty.setName("Action");
            xProperty.setValue(eTypes.eInteger, m_action);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            xProperty.setName("Copies");
            xProperty.setValue(eTypes.eInteger, m_copies);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            xProperty.setName("File");
            xProperty.setValue(eTypes.eText, m_file);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            xProperty.setName("FileFormat");
            xProperty.setValue(eTypes.eInteger, m_fileFormat);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            xProperty.setName("InternalPreview");
            xProperty.setValue(eTypes.eBoolean, m_internalPreview);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            xProperty.setName("ShowPrintersDialog");
            xProperty.setValue(eTypes.eBoolean, m_showPrintersDialog);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            xProperty.setName("Silent");
            xProperty.setValue(eTypes.eBoolean, m_silent);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            xProperty.setName("Sqlstmt");
            xProperty.setValue(eTypes.eText, m_sqlstmt);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            xProperty.setName("StrConnect");
            xProperty.setValue(eTypes.eText, m_strConnect);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            return true;
        }

    }

}
