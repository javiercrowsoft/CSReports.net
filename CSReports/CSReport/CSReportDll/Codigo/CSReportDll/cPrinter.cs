using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSReportGlobals;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;

namespace CSReportDll
{
    public class cPrinter
    {
        private const String C_MODULE = "cPrinter";

        private String m_deviceName = "";
        private String m_driverName = "";
        private String m_port = "";
        private cReportPaperInfo m_paperInfo = new cReportPaperInfo();

        private int m_copies = 0;

        private Graphics m_graph;

        private PrintDialog m_printDialog;

        public cPrinter(PrintDialog printDialog)
        {
            m_printDialog = printDialog;
        }

        public int getCopies()
        {
            return m_copies;
        }
        
        public void setCopies(int rhs)
        {
            m_copies = rhs;
        }

        public Graphics getGraph()
        {
            return m_graph;
        }

        public void setGraph(Graphics rhs)
        {
            m_graph = rhs;
        }

        public String getDeviceName()
        {
            return m_deviceName;
        }

        public void setDeviceName(String rhs)
        {
            m_deviceName = rhs;
        }

        public String getDriverName()
        {
            return m_driverName;
        }

        public void setDriverName(String rhs)
        {
            m_driverName = rhs;
        }

        public String getPort()
        {
            return m_port;
        }

        public void setPort(String rhs)
        {
            m_port = rhs;
        }

        public cReportPaperInfo getPaperInfo()
        {
            return m_paperInfo;
        }

        public void setPaperInfo(cReportPaperInfo rhs)
        {
            m_paperInfo = rhs;
        }

        public bool showDialog(int pages)
        {
            csReportPaperType paperSize = 0;
            int orientation = 0;
            int fromPage = 0;
            int toPage = 0;
            int paperBin = 0;

            paperSize = m_paperInfo.getPaperSize();
            orientation = m_paperInfo.getOrientation();

            fromPage = 1;
            toPage = pages;

            if (cPrintAPI.showPrintDialog(
                    m_printDialog,
                    ref m_deviceName,
                    ref m_driverName,
                    ref m_port,
                    ref paperSize,
                    ref orientation,
                    ref fromPage,
                    ref toPage,
                    ref m_copies,
                    ref paperBin))
            {
                m_paperInfo.setPaperSize(paperSize);
                m_paperInfo.setOrientation(orientation);
                m_paperInfo.setPagesToPrint(fromPage.ToString() + "-" + toPage.ToString());
                m_paperInfo.setPaperBin(paperBin);

                return true;
            }
            else
            {
                return false;
            }
        }

        private PaperSize getPaperSize(csReportPaperType paperSize)
        {
            PaperSize size = new PaperSize();

            switch (paperSize) {
                case csReportPaperType.CSRPTPAPERTYPEA4:
                    size.RawKind = (int)PaperKind.A4;
                    break;
                case csReportPaperType.CSRPTPAPERTYPEA3:
                    size.RawKind = (int)PaperKind.A3;
                    break;
                case csReportPaperType.CSRPTPAPERTYPELETTER:
                    size.RawKind = (int)PaperKind.Letter;
                    break;
                case csReportPaperType.CSRPTPAPERTYPELEGAL:
                    size.RawKind = (int)PaperKind.Legal;
                    break;
            }
            return size;
        }

        public bool starDoc(PrintDocument printDoc, String title, csReportPaperType paperSize, int orientation)
        {
            printDoc.DefaultPageSettings.Landscape = (orientation == (int)csRptPageOrientation.LANDSCAPE);
            printDoc.DefaultPageSettings.PaperSize = getPaperSize(paperSize);

            return true;
        }
    }

}
