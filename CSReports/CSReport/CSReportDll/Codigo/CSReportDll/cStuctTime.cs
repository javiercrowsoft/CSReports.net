using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSReportDll
{
    public class cStructTime
    {

        private int m_hour = 0;
        private int m_minute = 0;
        private int m_second = 0;

        public int getHour()
        {
            return m_hour;
        }

        public void setHour(int rhs)
        {
            m_hour = rhs;
        }

        public int getMinute()
        {
            return m_minute;
        }

        public void setMinute(int rhs)
        {
            m_minute = rhs;
        }

        public int getSecond()
        {
            return m_second;
        }

        public void setSecond(int rhs)
        {
            m_second = rhs;
        }

    }

}
