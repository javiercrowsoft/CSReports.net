﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSReportGlobals;

namespace CSReportDll
{
    public class cPrintAPI
    {
        internal static bool showPrintDialog(string m_deviceName, string m_driverName, string m_port, csReportPaperType paperSize, int orientation, int fromPage, int toPage, int m_copies, int paperBin)
        {
            throw new NotImplementedException();
        }

        internal static void printerSetPaperBin(string m_deviceName, int m_oldPaperBin)
        {
            throw new NotImplementedException();
        }

        internal static int endDoc(int m_hDC)
        {
            throw new NotImplementedException();
        }

        internal static cPrinter getcPrint(string deviceName, string driverName, string port)
        {
            throw new NotImplementedException();
        }

        public static cPrinter getcPrinterFromDefaultPrinter()
        {
            throw new NotImplementedException();
        }

        internal static int printerPaperBinNameToId(string p, string p_2, string paperBin)
        {
            throw new NotImplementedException();
        }
    }
}