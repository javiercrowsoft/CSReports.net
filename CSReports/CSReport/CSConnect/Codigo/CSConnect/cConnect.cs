using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using CSReportGlobals;
using CSDataBase;

namespace CSConnect
{
    public class cConnect
    {
        private cParameters m_parameters = new cParameters();
        private cColumnsInfo m_columnsInfo = new cColumnsInfo();

        private string m_strConnect = "";
        private string m_dataSource = "";
        private csDataSourceType m_dataSourceType;

		public cParameters getParameters()
		{
            return m_parameters;
		}

        public cColumnsInfo getColumnsInfo()
        {
            return m_columnsInfo;
        }

        public bool fillParameters(string dataSource)
        {
            cDataBase db = new cDataBase(csDatabaseEngine.SQL_SERVER);
            if (db.initDb(m_strConnect))
            {
                string[] restrictions = new string[4];
                restrictions[2] = dataSource;
                DataTable dt = db.openSchema("ProcedureParameters", restrictions);

                if (m_parameters == null) m_parameters = new cParameters();

                cParameters parameters = new cParameters();

                foreach (DataRow row in dt.Rows)
                {
                    if (row["parameter_mode"].ToString() != "OUT") 
                    {
                        cParameter p = null;
                        bool found = false;
                        for (var i = 0; i < m_parameters.count(); i++)
                        { 
                            p = m_parameters.item(i);
                            if (p.getName() == row["parameter_name"].ToString())
                            {
                                found = true;
                                break;
                            }
                        }
                        if (!found) p = null;
                        p = parameters.add(p, "");
                        p.setName(row["parameter_name"].ToString());
                        p.setPosition((int)row["ordinal_position"]);
                        p.setColumnType(cDatabaseGlobals.getDataTypeFromString(row["data_type"].ToString()));
                    }
                }

                m_parameters = parameters;
                
                return true;
            }

            return false;
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
            var db = new cDataBase(csDatabaseEngine.SQL_SERVER);
            if (db.initDb(m_strConnect))
            {
                DbDataReader rs;
                if (db.openRs(sqlstmt, out rs, "fillColumns", "cConnect", "Update columns's definition", CSKernelClient.eErrorLevel.eErrorInformation))
                {
                    for (int i = 0; i < rs.FieldCount; i++)
                    {
                        var column = new cColumnInfo();
                        column.setName(rs.GetName(i));
                        column.setPosition(i);
                        column.setColumnType((csDataType)System.Type.GetTypeCode((rs.GetFieldType(i))));
                        m_columnsInfo.add(column, "");
                    }
                }
                else
                {
                    return false;
                }
            }
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
