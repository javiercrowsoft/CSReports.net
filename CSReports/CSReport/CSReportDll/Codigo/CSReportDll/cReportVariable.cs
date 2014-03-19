using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSReportDll
{
    public class cReportVariable
    {
        private const String C_MODULE = "cReportVariable";
        private object m_value = null;

        public object getValue()
        {
            return m_value;
        }

        public void setValue(object rhs) 
        {
            m_value = rhs;
        }

    }

}
