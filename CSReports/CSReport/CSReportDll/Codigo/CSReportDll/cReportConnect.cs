using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Globalization;
using CSKernelClient;
using CSReportGlobals;
using CSDataBase;

namespace CSReportDll
{

    public class cReportConnect
    {

        private const String C_MODULE = "cReportConnect";

        private const String C_RPTCONNECT = "RptConnect";
        private const String C_RPTCOLUMNS = "Columns";
        private const String C_RPTPARAMETERS = "Parameters";

        private String m_strConnect = "";
        private String m_dataSource = "";
        private csDataSourceType m_dataSourceType;

        private cParameters m_parameters = new cParameters();
        private cColumnsInfo m_columns = new cColumnsInfo();

        private int m_connectionTimeout = 0;
        private int m_commandTimeout = 0;

        public int getConnectionTimeout()
        {
            return m_connectionTimeout;
        }

        public void setConnectionTimeout(int rhs)
        {
            m_connectionTimeout = rhs;
        }

        public int getCommandTimeout()
        {
            return m_commandTimeout;
        }

        public void setCommandTimeout(int rhs)
        {
            m_commandTimeout = rhs;
        }

        public String getStrConnect()
        {
            return m_strConnect;
        }

        public void setStrConnect(String rhs)
        {
            m_strConnect = rhs;
        }

        public String getDataBase()
        {
            return getXFromStrConnect(m_strConnect, "Initial Catalog=");
        }

        public String getServer()
        {
            return getXFromStrConnect(m_strConnect, "Data Source=");
        }

        public String getUser()
        {
            return getXFromStrConnect(m_strConnect, "User ID=");
        }

        public String getPassword()
        {
            return getXFromStrConnect(m_strConnect, "Password=");
        }

        public String getDataSource()
        {
            return m_dataSource;
        }

        public void setDataSource(String rhs)
        {
            m_dataSource = rhs;
        }

        public csDataSourceType getDataSourceType()
        {
            return m_dataSourceType;
        }

        public void setDataSourceType(csDataSourceType rhs)
        {
            m_dataSourceType = rhs;
        }

        public cParameters getParameters()
        {
            return m_parameters;
        }

        public void setParameters(cParameters rhs)
        {
            m_parameters = rhs;
        }

        public cColumnsInfo getColumns()
        {
            return m_columns;
        }

        public void setColumns(cColumnsInfo rhs)
        {
            m_columns = rhs;
        }

        public String getSqlParameters()
        {
            String s = "";
            cParameter param = null;
            for (int _i = 0; _i < m_parameters.count(); _i++)
            {
                param = m_parameters.item(_i);
                switch (cDatabaseGlobals.getDataTypeFromAdo((int)param.getColumnType()))
                {
                    case csDataType.CSTDWCHAR:
                        /*
                            case  csDataType.CSTDVARWCHAR:
                            case  csDataType.CSTDVARCHAR:
                            case  csDataType.CSTDLONGVARWCHAR:
                            case  csDataType.CSTDLONGVARCHAR:
                            case  csDataType.CSTDCHAR:
                         */
                        s +=  cDataBase.sqlString(param.getValue()) + ",";
                        break;
                    case csDataType.CSTDTINYINT:
                    case csDataType.CSTDUNSIGNEDTINYINT:
                    case csDataType.CSTDSMALLINT:
                    case csDataType.CSTDSINGLE:
                    case csDataType.CSTDNUMERIC:
                    case csDataType.CSTDINTEGER:
                    case csDataType.CSTDDOUBLE:
                    /*
                        case  csDataType.CSTDDECIMAL:
                        case  csDataType.CSTDCURRENCY:
                    */
                    case csDataType.CSTDBOOLEAN:
                    case csDataType.CSTDBIGINT:
                        s +=  cDataBase.sqlNumber(param.getValue()) + ",";
                        break;
                    case csDataType.CSTDDBTIMESTAMP:
                        /*
                        case  csDataType.CSTDDBTIME:
                        case  csDataType.CSTDDBDATE:
                        case  csDataType.CSTDDATE:
                        */
                        s +=  cDataBase.sqlDate(param.getValue()) + ",";                        
                        break;
                    default:
                        cWindow.msgWarning("This data type is not codified "
                                            + param.getColumnType()
                                            + ". Parameter: " + param.getName()
                                            + ". Function: sqlParameters.");
                        break;
                }
            }

            if (s.Substring(s.Length - 1) == ",")
            {
                s = s.Substring(0, s.Length - 1);
            }

            return s;
        }

        internal bool load(CSXml.cXml xDoc, XmlNode nodeObj)
        {
            XmlNode nodeObjAux = null;
            XmlNode nodeObjAux2 = null;

            m_dataSource = xDoc.getNodeProperty(nodeObj, "DataSource").getValueString(eTypes.eText);
            m_dataSourceType = (csDataSourceType)xDoc.getNodeProperty(nodeObj, "DataSourceType").getValueInt(eTypes.eInteger);
            m_strConnect = xDoc.getNodeProperty(nodeObj, "StrConnect").getValueString(eTypes.eText);

            nodeObjAux2 = xDoc.getNodeFromNode(nodeObj, C_RPTCOLUMNS);

            if (xDoc.nodeHasChild(nodeObjAux2))
            {
                nodeObjAux = xDoc.getNodeChild(nodeObjAux2);
                while (nodeObjAux != null)
                {
                    String key = xDoc.getNodeProperty(nodeObjAux, "Key").getValueString(eTypes.eText);
                    if (!m_columns.add(null, key).load(xDoc, nodeObjAux))
                    {
                        return false;
                    }
                    nodeObjAux = xDoc.getNextNode(nodeObjAux);
                }
            }

            nodeObjAux2 = xDoc.getNodeFromNode(nodeObj, C_RPTPARAMETERS);

            if (xDoc.nodeHasChild(nodeObjAux2))
            {
                nodeObjAux = xDoc.getNodeChild(nodeObjAux2);
                while (nodeObjAux != null)
                {
                    String key = xDoc.getNodeProperty(nodeObjAux, "Key").getValueString(eTypes.eText);
                    if (!m_parameters.add(null, key).load(xDoc, nodeObjAux))
                    {
                        return false;
                    }
                    nodeObjAux = xDoc.getNextNode(nodeObjAux);
                }
            }

            return true;
        }

        internal bool save(CSXml.cXml xDoc, XmlNode nodeFather)
        {
            CSXml.cXmlProperty xProperty = null;
            XmlNode nodeObj = null;
            XmlNode nodeObjAux = null;
            xProperty = new CSXml.cXmlProperty();

            xProperty.setName(C_RPTCONNECT);

            if (nodeFather != null)
            {
                nodeObj = xDoc.addNodeToNode(nodeFather, xProperty);
            }
            else
            {
                nodeObj = xDoc.addNode(xProperty);
            }

            xProperty.setName("DataSource");
            xProperty.setValue(eTypes.eText, m_dataSource);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            xProperty.setName("DataSourceType");
            xProperty.setValue(eTypes.eInteger, m_dataSourceType);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            xProperty.setName("StrConnect");
            xProperty.setValue(eTypes.eText, m_strConnect);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            nodeObjAux = nodeObj;

            xProperty.setName(C_RPTCOLUMNS);
            nodeObj = xDoc.addNodeToNode(nodeObj, xProperty);

            cColumnInfo col = null;
            for (int _i = 0; _i < m_columns.count(); _i++)
            {
                col = m_columns.item(_i);
                if (!col.save(xDoc, nodeObj))
                {
                    return false;
                }
            }

            nodeObj = nodeObjAux;

            xProperty.setName(C_RPTPARAMETERS);
            nodeObj = xDoc.addNodeToNode(nodeObj, xProperty);

            cParameter param = null;
            for (int _i = 0; _i < m_parameters.count(); _i++)
            {
                param = m_parameters.item(_i);
                if (!param.save(xDoc, nodeObj))
                {
                    return false;
                }
            }

            return true;
        }

        private String getXFromStrConnect(String strConnect, String x)
        {
            int i = 0;
            int p = 0;

            if (x.Substring(x.Length - 1) != "=")
            {
                x = x + "=";
            }
            i = strConnect.IndexOf(x, 0);
            if (i > 0)
            {
                p = strConnect.IndexOf(";", i);
                if (p == 0)
                {
                    p = strConnect.Length + 1;
                }
                i = i + x.Length;
                return strConnect.Substring(i, p - i);
            }
            else
            {
                return "";
            }
        }

    }

}
