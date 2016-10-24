using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSReportDll
{
    public class cReportFormulaParameter
    {
        private const String C_MODULE = "cReportFormulaParameter";

        private String m_value = "";

        public String getValue()
        {
            return m_value;
        }

        public void setValue(String rhs)
        {
            m_value = rhs;
        }

    }

}
