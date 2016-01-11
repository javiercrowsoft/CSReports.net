﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSReportGlobals;
using System.Drawing;

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

        private bool m_bRestorePaperBin;
        private int m_oldPaperBin = 0;

        private Graphics m_graph;

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

            if (cPrintAPI.showPrintDialog(m_deviceName,
                                            m_driverName,
                                            m_port,
                                            paperSize,
                                            orientation,
                                            fromPage,
                                            toPage,
                                            m_copies,
                                            paperBin))
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

        public bool starDoc(String title, csReportPaperType paperSize, int paperOrient)
        {
            // TODO: print using .Net
            /*
            m_hDC = CreateDC(m_driverName, m_deviceName, 0, 0);

            if (m_paperInfo != null)
            {
                if (m_paperInfo.getPaperBin())
                {

                    m_oldPaperBin = mPrintAPI.printerSetPaperBin(m_deviceName, m_paperInfo.getPaperBin());
                    m_bRestorePaperBin = true;

                }
            }

            mPrintAPI.printerSetSizeAndOrient(m_deviceName, paperSize, paperOrient, m_hDC);

            DOCINFO di = null;
            di.cbSize = di.Length;
            di.lpszDocName = title;

            return mPrintAPI.StartDoc(m_hDC, di) == 0;
            */
            return false;
        }

        public bool endDoc() {
            bool _rtn = false;

            // TODO: print using .Net
            /*
            if (m_hDC != 0)
            {
                _rtn = cPrintAPI.endDoc(m_hDC) != 0;
                // TODO: print using .Net
                // DeleteDC(m_hDC);
                m_hDC = 0;
            }
            else
            {
                _rtn = true;
            }
            */
            if (m_bRestorePaperBin)
            {
                m_bRestorePaperBin = false;
                cPrintAPI.printerSetPaperBin(m_deviceName, m_oldPaperBin);
            }

            return _rtn;
        }

        public bool starPage()
        {
            // TODO: print using .Net
            //return mPrintAPI.StartPage(m_hDC) != 0;
            return false;
        }

        public bool endPage()
        {
            // TODO: print using .Net
            // return mPrintAPI.cReport.endPage(m_hDC) != 0;
            return false;
        }

    }

}