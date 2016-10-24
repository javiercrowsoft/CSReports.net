using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSReportGlobals;

namespace CSReportDll
{
    internal static class cReportError
    {
        private const String C_MODULE = "cReportError";

        public static String gDebugControl = "";
        public static int gDebugSectionLine = 0;
        public static String gDebugSection = "";

        public static String errGetDescript(csRptErrors rptErrCode, params String[] x)
        {
            String s = "";
            switch (rptErrCode)
            {
                case csRptErrors.LAUNCH_INFO_UNDEFINED:
                    s = "The cReport launch metodh must be called with an instance of oLaunchInfo class or the cReport init method must be called before calling launch.";
                    break;

                case csRptErrors.SINTAX_ERROR_MISSING_BRAKETS:
                    s = "Syntax error found in formula. An internal function was found in the code but the open parenthesis was missing.";
                    break;

                case csRptErrors.CSRPTERRINDEFINEDFUNCTION:
                    s = "the function $1 is not defined.";
                    break;

                case csRptErrors.CSRPTERRMISSINGPARAM:
                    s = "The parameter $1 was missing in the function call to $2.";
                    break;

                case csRptErrors.CONTROL_NOT_FOUND:
                    s = "The control $1 was not found.";
                    break;

                case csRptErrors.GROUP_NOT_FOUND:
                    s = "The group '$1' refers to field '$2' but this is not between the columns of the recordset.";
                    break;

                case csRptErrors.GROUP_NOT_FOUND_IN_MAIN_RS:
                    s = "The group '$1' refers to field '$2' but this is not between the columns of the main recordset.";
                    break;

                case csRptErrors.FIELD_NOT_FOUND:
                    s = "The control '$1' refers to field '$2' but this is not between the columns of the recordset.";
                    break;

                case csRptErrors.CSRPTERRVARNOTDEFINED:
                    s = "The variable $1 was not found in the variables collection. The variables must be declared with DeclareVar before being used by SetVar or GetVar.";
                    break;

                case csRptErrors.CSRPTERRPARAMNOTDEFINED:
                    s = "The parameter $1 was not found in the parameters collection. The parameters must be present in the main recordset.";
                    break;

                case csRptErrors.PRINTER_NOT_DEFINED:
                    s = "The printer is not defined. This could happens if your system does not have any printer or default printer is not defined.";
                    break;

                default:
                    s = "There is not a message associated to this error code.";
                    break;
            }

            int i = 0;

            for (i = 0; i < x.Length; i++)
            {
                s = s.Replace("$" + (i + 1).ToString(), x[i]);
            }

            s = s + "\n\nSection  : " + gDebugSection 
                    + "\nSec. Line: " + gDebugSectionLine 
                    + "\nControl  : " + gDebugControl + "\n";

            return s;
        }

    }
}
