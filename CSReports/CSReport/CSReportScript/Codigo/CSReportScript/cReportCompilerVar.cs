using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSReportScript
{
    public class cReportCompilerVar
    {

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
