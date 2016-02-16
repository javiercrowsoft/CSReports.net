using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSReportGlobals;

namespace CSConnect
{
    public class cConnect
    {
        private cParameters m_parameters = new cParameters();

        private string m_strConnect = "";
        private string m_dataSource = "";
        private csDataSourceType m_dataSourceType;

		public cParameters getParameters()
		{
            return m_parameters;
		}

        public bool fillParameters(string dataSource)
        {
            return true;
        }

		public bool getDataSourceColumnsInfo(string str, csDataSourceType csDataSourceType)
		{
            string sqlstmt;

            if(m_dataSourceType == csDataSourceType.CDDTPROCEDURE)
            {
                if(! fillParameters(m_dataSource))
                {
                    return false;
                }

                fParameters f = new fParameters();
                f.setParameters(m_parameters);
                f.ShowDialog();
                if (f.getOk())
                {
                    sqlstmt = "[" + m_dataSource + "] " + f.getSqlParameters();
                }
                else
                {
                    return false;
                }
            }
            else
            {
                sqlstmt = "select * from [" + m_dataSource + "]";
            }

            return fillColumns(sqlstmt);
		}

        private bool fillColumns(string sqlstmt)
        {
            return true;
        }

		public void setStrConnect(string strConnect)
		{
			m_strConnect = strConnect;
		}

		public void setDataSource(string dataSource)
		{
			m_dataSource = dataSource;
		}

		public void setDataSourceType(csDataSourceType dataSourceType)
		{
            m_dataSourceType = dataSourceType;
		}

		public bool showOpenConnection()
		{
			throw new NotImplementedException ();
		}

		public string getDataSource()
		{
            return m_dataSource;
		}

		public csDataSourceType getDataSourceType ()
		{
            return m_dataSourceType;
		}
    }
}
