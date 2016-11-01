using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSReportGlobals;

namespace CSReportDll
{
    public class cReportFormulaType
    {
        private String m_name = "";
        private String m_nameUser = "";
        private csRptFormulaType m_id = 0;
        private String m_decrip = "";
        private int m_helpContextId = 0;

        public String getName()
        {
            return m_name;
        }

        public void setName(String rhs)
        {
            m_name = rhs;
        }

        public String getNameUser()
        {
            return m_nameUser;
        }

        public void setNameUser(String rhs)
        {
            m_nameUser = rhs;
        }

        public csRptFormulaType getId()
        {
            return m_id;
        }

        public void setId(csRptFormulaType rhs)
        {
            m_id = rhs;
        }

        public String getDecrip()
        {
            return m_decrip;
        }

        public void setDecrip(String rhs)
        {
            m_decrip = rhs;
        }

        public int getHelpContextId()
        {
            return m_helpContextId;
        }

        public void setHelpContextId(csRptFormulaType rhs)
        {
            m_helpContextId = (int)rhs;
        }

        public void setHelpContextId(int rhs)
        {
            m_helpContextId = rhs;
        }

    }

}
