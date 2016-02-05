using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management;

namespace CSReportDll
{
    class cPrintWMI
    {
        //
        // printer properties
        //

        //
        // used when we need more than one property
        //
        // first call getPrinterInfoFromWMI to get a printerInfo object ( ManagementObject )
        // then call this function one time for each property you need to access
        //
        public static object getPrinterInfoValueFromWMI(string propertyName, object printerInfo)
        {
            ManagementObject printer = printerInfo as ManagementObject;
            if (printer != null)
            {
                foreach (PropertyData property in printer.Properties)
                {
                    //Console.WriteLine(string.Format("{0}: {1}", property.Name, property.Value));
                    if (propertyName.Equals(property.Name))
                    {
                        return property.Value;
                    }
                }
            }
            return null;
        }

        //
        // used when we need only one property
        //
        public static object getPrinterInfoValueFromWMI(string printerName, string propertyName)
        {
            return getPrinterInfoValueFromWMI(propertyName, getPrinterInfoFromWMI(printerName));
        }

        //
        // printer configuration properties
        //

        //
        // used when we need more than one property
        //
        // first call getPrinterConfigInfoFromWMI to get a printerInfo object ( ManagementObject )
        // then call this function one time for each property you need to access
        //
        public static object getPrinterConfigInfoValueFromWMI(string propertyName, object printerInfo)
        {
            return getPrinterInfoValueFromWMI(propertyName, printerInfo);
        }

        //
        // used when we need only one property
        //
        public static object getPrinterConfigInfoValueFromWMI(string printerName, string propertyName)
        {
            return getPrinterInfoValueFromWMI(propertyName, getPrinterConfigInfoFromWMI(printerName));
        }

        //
        // printer and printer config query functions
        //

        public static object getPrinterInfoFromWMI(string printerName)
        {
            return getInfoFromWMI("Win32_Printer", printerName);
        }

        public static object getPrinterConfigInfoFromWMI(string printerName) 
        {
            return getInfoFromWMI("Win32_PrinterConfiguration", printerName);
        }

        //
        // generic query function
        //
        private static object getInfoFromWMI(string tableName, string objectName)
        {
            string query = string.Format("SELECT * from {0} WHERE Name = '{1}'", tableName, objectName);
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
            ManagementObjectCollection coll = searcher.Get();

            foreach (ManagementObject printer in coll)
            {
                return printer;
            }
            return null;
        }
    }
}
