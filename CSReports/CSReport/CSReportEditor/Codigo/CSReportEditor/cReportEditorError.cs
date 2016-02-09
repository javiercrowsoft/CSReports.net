using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSReportEditor
{
    class cReportEditorError
    {
        public static String errGetDescript(csRptEditorErrors rptErrCode)
        {
            switch (rptErrCode)
            {
                case csRptEditorErrors.CSRPT_EDITOR_SECTION_TYPE_INVALID:
                    return "The section type of the section argument passed to function getHeightOfSectionsBellowMe is not valid.";
                default:
                    return "There is not information for this error";
            }
        }
    }

    public enum csRptEditorErrors
    {
        CSRPT_EDITOR_SECTION_TYPE_INVALID = 2001
    }
}
