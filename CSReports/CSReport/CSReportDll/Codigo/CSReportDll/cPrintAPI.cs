﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Printing;
using CSKernelClient;
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

        internal static cPrinter getcPrint(string deviceName, string driverName, string port, int orientation, int paperSize, int width, int height)
        {
            cPrinter o = new cPrinter();

            o.setDeviceName(deviceName);
            o.setDriverName(driverName);
            o.setPort(port);

            cReportPaperInfo paperInfo = o.getPaperInfo();

            paperInfo.setOrientation(orientation);
            paperInfo.setPaperSize((csReportPaperType)paperSize);

            if (width == 0 || height == 0)
            {
                getSizeFromPaperSize((csReportPaperType)paperSize, orientation, out width, out height);
            }

            paperInfo.setWidth(width);
            paperInfo.setHeight(height);
            return o;
        }

        internal static cPrinter getcPrint(string deviceName, string driverName, string port)
        {
            object printerConfigInfo = cPrintWMI.getPrinterConfigInfoFromWMI(deviceName);

            int paperSize = getPaperSizeFromSizeName(cPrintWMI.getPrinterConfigInfoValueFromWMI("PaperSize", printerConfigInfo, "A4") as string);
            int orientation = (int)cPrintWMI.getPrinterConfigInfoValueFromWMI("Orientation", printerConfigInfo, 1);

            int width = (int)cPrintWMI.getPrinterConfigInfoValueFromWMI("PaperWidth", printerConfigInfo, 210);
            int height = (int)cPrintWMI.getPrinterConfigInfoValueFromWMI("PaperLength", printerConfigInfo, 297);

            return getcPrint(deviceName, driverName, port, orientation, paperSize, width, height);
        }

        public static cPrinter getcPrinterFromDefaultPrinter()
        {
            String deviceName = "";
            String driverName = "";
            String port = "";
            int paperSize = 0;
            int orientation = 0;
            int width = 0;
            int height = 0;

            getDefaultPrinter(out deviceName, out driverName, out port, out paperSize, out orientation, out width, out height);

            if (deviceName != "")
            {
                return getcPrint(deviceName, driverName, port, orientation, paperSize, width, height);
            }
            else
            {
                return null;
            }
        }

        internal static int printerPaperBinNameToId(string p, string p_2, string paperBin)
        {
            throw new NotImplementedException();
        }

        public static void getDefaultPrinter(
            out String deviceName, out String driverName, out String port,
            out int paperSize, out int orientation, out int width, out int height)
        {
            PrinterSettings settings = new PrinterSettings();

            deviceName = settings.PrinterName;

            object printerInfo = cPrintWMI.getPrinterInfoFromWMI(settings.PrinterName);

            driverName = cPrintWMI.getPrinterInfoValueFromWMI("DriverName", printerInfo, "") as string;
            port = cPrintWMI.getPrinterInfoValueFromWMI("PortName", printerInfo, "") as string;

            object printerConfigInfo = cPrintWMI.getPrinterConfigInfoFromWMI(settings.PrinterName);

            paperSize = getPaperSizeFromSizeName(cPrintWMI.getPrinterConfigInfoValueFromWMI("PaperSize", printerConfigInfo, "A4") as string);
            orientation = Convert.ToInt32(cPrintWMI.getPrinterConfigInfoValueFromWMI("Orientation", printerConfigInfo, 1));

            width = cUtil.valAsInt(cPrintWMI.getPrinterConfigInfoValueFromWMI("PaperWidth", printerConfigInfo, 210));
            height = cUtil.valAsInt(cPrintWMI.getPrinterConfigInfoValueFromWMI("PaperLength", printerConfigInfo, 297));

            if (width == 0 || height == 0 || paperSize == 99)
            {
                if (paperSize == 99 /*UNKNOWN*/) paperSize = 1; /*LETTER*/

                getSizeFromPaperSize((csReportPaperType)paperSize, orientation, out width, out height);
            }
        }

        private static void getSizeFromPaperSize(csReportPaperType paperSize, int orientation, out int width, out int height)
        {
            switch (paperSize)
            {
                case csReportPaperType.CSRPTPAPERTYPELETTER:
                    height = 279;
                    width = 216;
                    break;

                case csReportPaperType.CSRPTPAPERTYPELEGAL:
                    height = 356;
                    width = 216;
                    break;

                case csReportPaperType.CSRPTPAPERTYPEA4:
                    height = 297;
                    width = 210;
                    break;

                case csReportPaperType.CSRPTPAPERTYPEA3:
                    height = 420;
                    width = 297;
                    break;

                default:
                    height = 0;
                    width = 0;
                    break;
            }

            if (orientation == (int)csRptPageOrientation.LANDSCAPE)
            {
                int tmp = 0;
                tmp = height;
                height = width;
                width = tmp;
            }
        }

        private static int getPaperSizeFromSizeName(string sizeName) 
        {
            int size;
            switch (sizeName.ToLower()) { 
                case "a4":
                    size = (int)csReportPaperType.CSRPTPAPERTYPEA4;
                    break;
                case "letter":
                    size = (int)csReportPaperType.CSRPTPAPERTYPELETTER;
                    break;
                case "legal":
                    size = (int)csReportPaperType.CSRPTPAPERTYPELEGAL;
                    break;
                default:
                    size = (int)csReportPaperType.CSRPTPAPERUSER;
                    break;
            }
            return size;
        }
  
    }
}
