using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.OracleClient;
using CSKernelClient;
using System.Globalization;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Data.Common;

// http://msdn.microsoft.com/en-us/library/8627sbea%28VS.71%29.aspx
//
// how to get a DataSet from an OracleDataReader
//
// http://msdn.microsoft.com/en-us/library/haa3afyz%28v=vs.71%29.aspx#Y1763
public delegate void OpenRsProgress();

namespace CSDataBase
{

    public class cDataBase
    {
        private const string c_module = "cDataBase";
        private const string c_ErrorSqlInfoAdd = "@@ErrorSqlInfoAdd@@";

        private DbConnection m_ocn = null;
        private DbTransaction m_otxn = null;
        private string m_connect = "";

        private DbDataReader m_ors = null;
        private bool m_eofField = false;
        private int m_nextField = 0;
        private eFieldType m_fieldType;

        private int m_userId = 0;
        private int m_transactionLevel = 0;

        private string m_serverName = "";
        private string m_userName = "";
        private string m_password = "";

        private string m_originalStrConnect = "";

        private bool m_openRsCancel = false;
        private string m_openRsExDescript = "";

        private int m_commandTimeout = 180;
        private int m_connectionTimeout = 180;

        private int m_maxTryOpenRs = 2;
        private int m_maxTryExecute = 2;

        private string m_lastDbError = "";

        private bool m_eof = false;

        private csDatabaseEngine m_databaseEngine = csDatabaseEngine.SQL_SERVER;

        public event OpenRsProgress openRsProgress;

        public bool silent
        {
            get { return cDatabaseGlobals.Silent; }
            set { cDatabaseGlobals.Silent = value; }
        }

        public void setSilent(Boolean rhs)
        {
            cDatabaseGlobals.Silent = rhs;
        }

        public bool dbIsOpen
        {
            get
            {
                if (m_ocn == null)
                {
                    return false;
                }
                else
                {
                    return m_ocn.State == ConnectionState.Open;
                }
            }
        }

        public int commandTimeout
        {
            get { return m_commandTimeout; }
            set { m_commandTimeout = value; }
        }

        public void setCommandTimeout(int rhs)
        {
            m_commandTimeout = rhs;
        }

        public int connectionTimeout
        {
            get { return m_connectionTimeout; }
            set { m_connectionTimeout = value; }
        }

        public void setConnectionTimeout(int rhs)
        {
            m_connectionTimeout = rhs;
        }

        public bool openRsCancel
        {
            get { return m_openRsCancel; }
            set { m_openRsCancel = value; }
        }

        public string originalStrConnect
        {
            get { return m_originalStrConnect; }
            set { m_originalStrConnect = value; }
        }

        public int userId
        {
            get { return m_userId; }
            set { m_userId = value; }
        }

        public int transactionLevel
        {
            get { return m_transactionLevel; }
        }

        public DbDataReader ors
        {
            set
            {
                m_ors = value;
                m_eofField = false;
                m_nextField = 0;
            }
        }

        public bool eof
        {
            get { return m_eof; }
        }

        public bool eofField
        {
            get { return m_eofField; }
        }

        public CSOAPI.eServerVersion serverVersion
        {
            get
            {
                string ver = "";
                ver = m_ocn.ServerVersion;

                // TODO: this code is for sql server and the "Access if" don't work
                //       in this version
                //       when it runs we'll have to put the oracle numbers here
                if (ver == "06")
                {
                    return CSOAPI.eServerVersion.eVSql65;
                }
                else if (ver == "07")
                {
                    return CSOAPI.eServerVersion.eVSql70;
                }
                else if (ver == "03")
                {
                    return CSOAPI.eServerVersion.eVSAccess;
                }
                else
                {
                    return CSOAPI.eServerVersion.eVSUnknown;
                }
            }
        }

        public eFieldType fieldType
        {
            get { return m_fieldType; }
        }

        public string strConnect
        {
            get
            {
                if (m_ocn == null)
                {
                    return "";
                }
                else
                {
                    return m_ocn.ConnectionString;
                }
            }
        }

        public string dbName
        {
            get
            {
                if (m_ocn != null)
                {
                    return m_ocn.Database;
                }
                else
                {
                    return "";
                }
            }
        }

        public string serverName
        {
            get { return m_serverName; }
        }

        public string openRsExDescript
        {
            get { return m_openRsExDescript; }
            set { m_openRsExDescript = value; }
        }

        public void setOpenRsExDescript(String rhs)
        {
            m_openRsExDescript = rhs;
        }

        public int maxTryOpenRs
        {
            get { return m_maxTryOpenRs; }
            set { m_maxTryOpenRs = value; }
        }

        public int maxTryExecute
        {
            get { return m_maxTryExecute; }
            set { m_maxTryExecute = value; }
        }

        public string lastDbError
        {
            get { return m_lastDbError; }
        }

        public string password
        {
            get { return m_password; }
        }

        public bool saveSp(string sqlstmt,
                           out DbDataReader ors)
        {
            return saveSp(sqlstmt, out ors, -1, "", "", "Error", 0);
        }

        public bool saveSp(string sqlstmt,
                           out DbDataReader ors,
                           int timeout,
                           string function,
                           string module,
                           string title,
                           eErrorLevel level)
        {

            int oldCommandTimeout = m_commandTimeout;
            if (timeout != -1)
            {
                m_commandTimeout = timeout;
            }
            bool rtn = openRs(sqlstmt, out ors, function, module, title, level);
            m_commandTimeout = oldCommandTimeout;
            return rtn;
        }

        // list of sql schema names
        // https://msdn.microsoft.com/en-us/library/ms254969(v=vs.110).aspx
        //
        // how to create restriction array
        // https://msdn.microsoft.com/en-us/library/ms136366(v=vs.110).aspx
        //
        public DataTable openSchema()
        {
            return m_ocn.GetSchema();
        }

        public DataTable openSchema(string collectionName)
        {
            return m_ocn.GetSchema(collectionName);
        }

        public DataTable openSchema(string collectionName, string[] restrinctionValues)
        {
            return m_ocn.GetSchema(collectionName, restrinctionValues);
        }

        public bool initDb(string connect)
        {
            return initDbEx("", "", "", "", connect, false);
        }

        public bool initDb(string nameDb,
                           string server,
                           string user,
                           string password,
                           string connect)
        {
            return initDbEx(nameDb, server, user, password, connect, false);
        }

        public bool initDbEx(string nameDb,
                             string server,
                             string user,
                             string password,
                             string connect,
                             bool useOleDb)
        {
            cMouseWait mouseWait = new cMouseWait();
            try
            {
                closeDb();
                if (m_ocn == null)
                {
                    m_ocn = createConnection();
                }
                m_originalStrConnect = connect;
                if (connect == "")
                {
                    connect = string.Format("Data Source={0};User Id={1};Password={2};Integrated Security=no;",
                                            server,
                                            user,
                                            password);
                    m_serverName = server;
                    m_userName = user;
                    m_password = password;
                }
                else
                {
                    m_serverName = cUtil.getToken(connect, "Data Source=");
                    m_userName = cUtil.getToken(connect, "User=");
                    m_password = cUtil.getToken(connect, "Password=");
                }
                m_connect = connect;
                pConnect();
                return true;
            }
            catch (Exception ex)
            {
                cError.mngError(ex, "initDbEx", c_module, "");
                return false;
            }
            finally
            {
                mouseWait.Dispose();
            }
        }

        public bool addNew(DataTable dt, out DataRow dr)
        {
            dr = null;
            try
            {
                dr = dt.NewRow();
                return true;
            }
            catch (Exception ex)
            {
                cError.mngError(ex, "addNew", c_module, "");
                return false;
            }
        }

        public bool update(DataRow dr, DataTable dt, DbDataAdapter da)
        {
            try
            {
                dt.Rows.Add(dr);
                da.Update(dt);
                return true;
            }
            catch (Exception ex)
            {
                cError.mngError(ex, "update", c_module, "");
                return false;
            }
        }

        public bool delete(DataRow dr, DataTable dt, DbDataAdapter da)
        {
            try
            {
                dt.Rows.Remove(dr);
                da.Update(dt);
                return true;
            }
            catch (Exception ex)
            {
                cError.mngError(ex, "delete", c_module, "");
                return false;
            }
        }

        public bool openRsEx(bool showWindowCancel,
                             bool raiseProgressEvent,
                             bool showModal,
                             string sqlstmt,
                             out DbDataReader ors)
        {
            return openRsEx(showWindowCancel,
                            raiseProgressEvent,
                            showModal,
                            sqlstmt,
                            out ors,
                            "",
                            "",
                            "");
        }

        public bool loadDataTables(bool showWindowCancel,
                                     bool raiseProgressEvent,
                                     bool showModal,
                                     string sqlstmt,
                                     out List<DataTable> dt,
                                     string function,
                                     string module,
                                     string title)
        {
            DbDataReader ors;
            if (openRsEx(showWindowCancel,
                            raiseProgressEvent,
                            showModal,
                            sqlstmt,
                            out ors,
                            function,
                            module,
                            title))
            {
                dt = new List<DataTable>();
                var o = new DataTable();
                o.Load(ors);
                dt.Add(o);
                return true;
            }
            else
            {
                dt = null;
                return false;
            }
        }
        public bool loadDataTable(bool showWindowCancel,
                                     bool raiseProgressEvent,
                                     bool showModal,
                                     string sqlstmt,
                                     out DataTable dt,
                                     out DbDataReader dr,
                                     string function,
                                     string module,
                                     string title)
        {
            DbDataReader ors;
            if (openRsEx(showWindowCancel,
                            raiseProgressEvent,
                            showModal,
                            sqlstmt,
                            out ors,
                            function,
                            module,
                            title))
            {
                dr = ors;
                dt = new DataTable();
                dt.Load(ors);
                return true;
            }
            else
            {
                dt = null;
                dr = null;
                return false;
            }
        }

        public bool openRsEx(bool showWindowCancel,
                             bool raiseProgressEvent,
                             bool showModal,
                             string sqlstmt,
                             out DbDataReader ors,
                             string function,
                             string module,
                             string title)
        {
            bool cancelDialogShowed = false;
            fCancelQuery f = null;
            ors = null;
            try
            {
                // create a command to execute the query
                cOpenRsCommand cmd = new cOpenRsCommand();
                cmd.getExecuteCommand(this, sqlstmt);

                // execute in asynchronous mode
                cmd.execute();

                int seconds = 0;
                bool queryCanceled = false;

                // wait until the query finish
                while (!cmd.done)
                {
                    // cancel dialog
                    if (showWindowCancel && seconds > 200)
                    {
                        // show the cancel dialog
                        if (!cancelDialogShowed)
                        {
                            f = new fCancelQuery();
                            if (m_openRsExDescript != "")
                            {
                                f.descript = "Getting data for: " + m_openRsExDescript;
                            }
                            f.Show();
                            cancelDialogShowed = true;
                        }
                    }

                    // events
                    if (raiseProgressEvent)
                    {
                        if (openRsProgress != null)
                        {
                            openRsProgress();
                        }
                    }
                    Application.DoEvents();
                    if (cancelDialogShowed)
                    {
                        if (f.cancel)
                        {
                            queryCanceled = true;
                            break;
                        }
                    }
                    System.Threading.Thread.Sleep(100);
                    seconds += 100;
                }
                // hide cancel dialog
                if (showWindowCancel && cancelDialogShowed)
                {
                    f.Hide();
                }

                if (queryCanceled)
                {
                    pReconnect();
                    return false;
                }
                else
                {
                    ors = cmd.ors;
                    return cmd.success;
                }
            }
            catch (Exception ex)
            {
                cError.mngError(ex, "openRsEx", c_module, "");
                return false;
            }
            finally
            {
                f = null;
            }
        }

        public DbDataReader asyncOpenRsEx(string sqlstmt)
        {
            DbCommand ocmd = createCommand(sqlstmt);
            return ocmd.ExecuteReader(CommandBehavior.Default);
        }

        public bool openRs(string sqlstmt,
                           out DbDataReader ors,
                           string function,
                           string module,
                           string title,
                           eErrorLevel level)
        {
            int tryCount = 0;
            ors = null;

            while (tryCount < m_maxTryOpenRs)
            {
                if (pOpenRs(sqlstmt,
                            out ors,
                            function,
                            module,
                            title,
                            level,
                            tryCount == m_maxTryOpenRs))
                {
                    return true;
                }
            }
            return false;
        }

        private bool pOpenRs(string sqlstmt,
                             out DbDataReader ors,
                             string function,
                             string module,
                             string title,
                             eErrorLevel level,
                             bool showError)
        {
            ors = null;
            try
            {
                DbCommand ocmd = createCommand(sqlstmt);
                ors = ocmd.ExecuteReader();
                return true;
            }
            catch (Exception ex)
            {
                if (showError)
                {
                    cError.mngError(ex, "openRs for " + module + "." + function, c_module, "sentencia: " + sqlstmt);
                }
                return false;
            }
        }

        public bool disconnectRecordset(string sqlstmt, out DataTable dt)
        {
            dt = null;
            try
            {
                DbCommand ocmd = createCommand(sqlstmt);
                DbDataAdapter oda = new OracleDataAdapter(ocmd as OracleCommand);
                dt = new DataTable();
                oda.Fill(dt);
                return true;
            }
            catch (Exception ex)
            {
                cError.mngError(ex, "disconnectRecordset", "", "sentencia: " + sqlstmt);
                return false;
            }
        }

        public bool existsInRecordset(DataTable dt,
                                      string field,
                                      string val,
                                      out bool founded)
        {
            return existsInRecordset(dt, field, val, out founded,
                                     "", "", "", eErrorLevel.eErrorInformation);
        }

        public bool existsInRecordset(DataTable dt,
                                      string field,
                                      string val,
                                      out bool founded,
                                      string function,
                                      string module,
                                      string title,
                                      eErrorLevel level)
        {
            string filter = field + " = " + val;
            founded = false;

            try
            {
                if (dt.Rows.Count == 0)
                {
                    return false;
                }
                else
                {
                    DataRow[] vdr = dt.Select(filter);
                    founded = vdr.Length > 0;
                    return true;
                }
            }
            catch (Exception ex)
            {
                cError.mngError(ex, "existsInRecordset", module, "filter: " + filter);
                return false;
            }
        }

        public bool existsInRecord(DataRow dr,
                                   DataColumn[] columns,
                                   string val,
                                   out bool founded)
        {
            return existsInRecord(dr, columns, val, out founded,
                                  "", "", "", eErrorLevel.eErrorInformation);
        }

        public bool existsInRecord(DataRow dr,
                                   DataColumn[] columns,
                                   string val,
                                   out bool founded,
                                   string function,
                                   string module,
                                   string title,
                                   eErrorLevel level)
        {
            return pExistsInRecord(dr, columns, val, out founded, true,
                                   function, module, title, level);
        }

        public bool existsInRecordEx(DataRow dr,
                                     DataColumn[] columns,
                                     string val,
                                     out bool founded,
                                     bool like,
                                     string function,
                                     string module,
                                     string title,
                                     eErrorLevel level)
        {
            return pExistsInRecord(dr, columns, val, out founded, like,
                                   function, module, title, level);
        }

        private bool pExistsInRecord(DataRow dr,
                                    DataColumn[] columns,
                                    string val,
                                    out bool founded,
                                    bool like,
                                    string function,
                                    string module,
                                    string title,
                                    eErrorLevel level)
        {
            string filter = "";
            founded = false;
            val = val.ToLower();

            try
            {
                foreach (DataColumn col in columns)
                {
                    System.TypeCode typeCode = System.Type.GetTypeCode(col.GetType()); ;
                    switch (typeCode)
                    {
                        case System.TypeCode.Char:
                        case System.TypeCode.String:
                            if (like)
                            {
                                founded = dr[col.ColumnName].ToString().ToLower().Contains(val);
                            }
                            else
                            {
                                founded = dr[col.ColumnName].ToString().ToLower() == val;
                            }
                            break;
                        case System.TypeCode.Decimal:
                        case System.TypeCode.Double:
                        case System.TypeCode.Int16:
                        case System.TypeCode.Int32:
                        case System.TypeCode.Int64:
                        case System.TypeCode.Single:
                        case System.TypeCode.UInt16:
                        case System.TypeCode.UInt32:
                        case System.TypeCode.UInt64:
                            int ival;
                            if (int.TryParse(val, out ival))
                            {
                                founded = (int)dr[col.ColumnName] == ival;
                            }
                            break;
                        case System.TypeCode.DateTime:
                            break;
                        case System.TypeCode.Boolean:
                            founded = false;
                            break;
                        default:
                            founded = false;
                            break;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                cError.mngError(ex, "existsInRecord", module, "filter: " + filter);
                return false;
            }
        }

        public bool execute(string sqlstmt)
        {
            return execute(sqlstmt, "", "", "", eErrorLevel.eErrorInformation);
        }

        public bool execute(string sqlstmt,
                            string function,
                            string module,
                            string title,
                            eErrorLevel level)
        {
            int tryCount = 0;
            ors = null;

            while (tryCount < m_maxTryExecute)
            {
                if (pExecute(sqlstmt,
                             function,
                             module,
                             title,
                             level,
                             tryCount == m_maxTryExecute))
                {
                    return true;
                }
            }
            return false;
        }

        private bool pExecute(string sqlstmt,
                              string function,
                              string module,
                              string title,
                              eErrorLevel level,
                              bool showError)
        {
            try
            {
                DbCommand ocmd = createCommand(sqlstmt);
                ocmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                if (showError)
                {
                    cError.mngError(ex, "pExecute for " + module + "." + function, c_module, "sentencia: " + sqlstmt);
                }
                return false;
            }
        }

        public static string sqlString(string val)
        {
            return "'" + val.Replace("'", "''") + "'";
        }

        public static string sqlDate(string val)
        {
            DateTime dt;
            if (DateTime.TryParseExact(val, "MM/dd/yyyy", null, DateTimeStyles.None, out dt)) { }
            else if (DateTime.TryParseExact(val, "dd/MM/yyyy", null, DateTimeStyles.None, out dt)) { }
            else if (DateTime.TryParseExact(val, "MM-dd-yyyy", null, DateTimeStyles.None, out dt)) { }
            else if (DateTime.TryParseExact(val, "dd-MM-yyyy", null, DateTimeStyles.None, out dt)) { }
            else if (DateTime.TryParseExact(val, "MM.dd.yyyy", null, DateTimeStyles.None, out dt)) { }
            else if (DateTime.TryParseExact(val, "dd.MM.yyyy", null, DateTimeStyles.None, out dt)) { }
            
            else if (DateTime.TryParseExact(val, "M/dd/yyyy", null, DateTimeStyles.None, out dt)) { }
            else if (DateTime.TryParseExact(val, "dd/M/yyyy", null, DateTimeStyles.None, out dt)) { }
            else if (DateTime.TryParseExact(val, "M-dd-yyyy", null, DateTimeStyles.None, out dt)) { }
            else if (DateTime.TryParseExact(val, "dd-M-yyyy", null, DateTimeStyles.None, out dt)) { }
            else if (DateTime.TryParseExact(val, "M.dd.yyyy", null, DateTimeStyles.None, out dt)) { }
            else if (DateTime.TryParseExact(val, "dd.M.yyyy", null, DateTimeStyles.None, out dt)) { }
            
            else if (DateTime.TryParseExact(val, "M/d/yyyy", null, DateTimeStyles.None, out dt)) { }
            else if (DateTime.TryParseExact(val, "d/M/yyyy", null, DateTimeStyles.None, out dt)) { }
            else if (DateTime.TryParseExact(val, "M-d-yyyy", null, DateTimeStyles.None, out dt)) { }
            else if (DateTime.TryParseExact(val, "d-M-yyyy", null, DateTimeStyles.None, out dt)) { }
            else if (DateTime.TryParseExact(val, "M.d.yyyy", null, DateTimeStyles.None, out dt)) { }
            else if (DateTime.TryParseExact(val, "d.M.yyyy", null, DateTimeStyles.None, out dt)) { }

            else throw new Exception("Invalid date " + val);
            return "'" + dt.ToString(cConstants.C_SQL_DATE_STRING, CultureInfo.InvariantCulture) + "'";
        }

        /* TODO: remove me
        private string getNumberSql(string number)
        {
            if (! G.isNumeric(number)) 
            {
                return "0";
            }
            else
            {
                var s = cUtil.val(number).ToString(new String('#', 27) + "0." + new String('#', 28), CultureInfo.InvariantCulture);
                s = s.Replace(",", ".");
                if (s.Substring(s.Length - 1, 0) == ".")
                {
                    s = s.Substring(0, s.Length - 1);
                }
                return s;
            }

        } 
        private string getNumberSql(string number)
        {
            if (!G.isNumeric(number))
            {
                return "0";
            }
            else
            {
                var s = cUtil.val(number).ToString(new String('#', 27) + "0." + new String('#', 28), CultureInfo.InvariantCulture);
                s = s.Replace(",", ".");
                if (s.Substring(s.Length - 1, 0) == ".")
                {
                    s = s.Substring(0, s.Length - 1);
                }
                return s;
            }

        }
         
         */
        public static string sqlNumber(string number)
        {
            if (!G.isNumeric(number))
            {
                return "0";
            }
            else
            {
                var s = cUtil.val(number).ToString(new String('#', 27) + "0." + new String('#', 28), CultureInfo.InvariantCulture);
                s = s.Replace(",", ".");
                if (s.Substring(s.Length - 1, 0) == ".")
                {
                    s = s.Substring(0, s.Length - 1);
                }
                return s;
            }
        }

        /* TODO: remove me 
        public static string sqlNumber(string val)
        {
            double ival;
            NumberStyles style = (NumberStyles)
                                    (
                                        (int)NumberStyles.AllowCurrencySymbol
                                        + (int)NumberStyles.AllowDecimalPoint
                                        + (int)NumberStyles.AllowThousands
                                        + (int)NumberStyles.AllowLeadingSign
                                        + (int)NumberStyles.AllowLeadingWhite
                                        + (int)NumberStyles.AllowTrailingSign
                                        + (int)NumberStyles.AllowParentheses
                                    );
            if (!double.TryParse(val, style, null, out ival))
            {
                return "0";
            }
            else
            {
                val = String.Format("{0:0.000000}", ival);
                int i = val.IndexOf(cRegionalCfg.decimalPoint);
                if (i != -1)
                {
                    return val.Substring(0, i) + "." + val.Substring(i + 1);
                }
                else
                {
                    return val;
                }
            }
        }
        */

        public void closeDb()
        {
            try
            {
                if (m_transactionLevel > 0)
                {
                    rollbackTransaction();
                }
                m_transactionLevel = 0;
                m_userName = "";
                m_serverName = "";

                if (m_ocn != null)
                {
                    if (m_ocn.State != ConnectionState.Closed)
                    {
                        m_ocn.Close();
                    }

                    m_ocn.Dispose();
                    m_ocn = null;
                }
                m_ocn = createConnection();
            }
            catch (Exception ex)
            {
                cError.mngError(ex, "closeDb", c_module, "");
            }
        }

        public bool beginTransaction()
        {
            try
            {
                if (m_transactionLevel <= 0)
                {
                    m_otxn = m_ocn.BeginTransaction();
                    m_transactionLevel = 1;
                }
                else
                {
                    m_transactionLevel++;
                }
                return true;
            }
            catch (Exception ex)
            {
                cError.mngError(ex, "commitTransaction", c_module, "");
                return false;
            }
        }

        public bool commitTransaction()
        {
            if (m_transactionLevel <= 0)
            {
                m_transactionLevel = 0;
                return false;
            }
            try
            {
                if (m_transactionLevel == 1)
                {
                    m_otxn.Commit();
                    m_transactionLevel = 0;
                }
                else
                {
                    m_transactionLevel--;
                }
                return true;
            }
            catch (Exception ex)
            {
                cError.mngError(ex, "commitTransaction", c_module, "");
                return false;
            }
        }

        public void rollbackTransaction()
        {
            try
            {
                if (m_ocn != null)
                {
                    if (m_otxn != null)
                    {
                        m_otxn.Rollback();
                    }
                }
                m_transactionLevel = 0;
            }
            catch (Exception ex)
            {
                cError.mngError(ex, "rollbackTransaction", c_module, "");
            }
        }

        public bool getData(string table,
                            string fieldId,
                            string id,
                            string field,
                            out string data)
        {
            return getData(table, fieldId, id, field, out data, "", "", "", eErrorLevel.eErrorInformation);
        }

        public bool getData(string table,
                            string fieldId,
                            string id,
                            string field,
                            out int data)
        {
            return getData(table, fieldId, id, field, out data, "", "", "", eErrorLevel.eErrorInformation);
        }

        public bool getData(string table,
                            string fieldId,
                            string id,
                            string field,
                            out double data)
        {
            return getData(table, fieldId, id, field, out data, "", "", "", eErrorLevel.eErrorInformation);
        }

        public bool getData(string table,
                            string fieldId,
                            string id,
                            string field,
                            out DateTime data)
        {
            return getData(table, fieldId, id, field, out data, "", "", "", eErrorLevel.eErrorInformation);
        }

        public bool getData(string table,
                            string fieldId,
                            string id,
                            string field,
                            out string data,
                            string function,
                            string module,
                            string title,
                            eErrorLevel level)
        {
            DbDataReader ors;

            data = "";

            if (pGetData(table, fieldId, id, field, out ors,
                         function, module, title, level))
            {
                if (ors.Read())
                {
                    data = ors.GetString(0);
                }
                return true;
            }
            else return false;
        }

        public bool getData(string table,
                            string fieldId,
                            string id,
                            string field,
                            out int data,
                            string function,
                            string module,
                            string title,
                            eErrorLevel level)
        {
            DbDataReader ors;

            data = 0;

            if (pGetData(table, fieldId, id, field, out ors,
                         function, module, title, level))
            {
                if (ors.Read())
                {
                    data = (int)ors.GetInt32(0);
                }
                return true;
            }
            else return false;
        }

        public bool getData(string table,
                            string fieldId,
                            string id,
                            string field,
                            out double data,
                            string function,
                            string module,
                            string title,
                            eErrorLevel level)
        {
            DbDataReader ors;

            data = 0;

            if (pGetData(table, fieldId, id, field, out ors,
                         function, module, title, level))
            {
                if (ors.Read())
                {
                    data = ors.GetDouble(0);
                }
                return true;
            }
            else return false;
        }

        public bool getData(string table,
                            string fieldId,
                            string id,
                            string field,
                            out DateTime data,
                            string function,
                            string module,
                            string title,
                            eErrorLevel level)
        {
            DbDataReader ors;

            data = cConstants.C_NO_DATE;

            if (pGetData(table, fieldId, id, field, out ors,
                         function, module, title, level))
            {
                if (ors.Read())
                {
                    data = ors.GetDateTime(0);
                }
                return true;
            }
            else return false;
        }

        private bool pGetData(string table,
                              string fieldId,
                              string id,
                              string field,
                              out DbDataReader ors,
                              string function,
                              string module,
                              string title,
                              eErrorLevel level)
        {
            string sqlstmt;
            ors = null;
            sqlstmt = "select " + field + " from " + table + " where " + fieldId + " = " + id;

            return openRs(sqlstmt, out ors, function, module, title, level);
        }

        public bool getNewId(string table,
                             string fieldId,
                             out int id,
                             string function,
                             string module,
                             string title,
                             eErrorLevel level)
        {
            cMouseWait mouseWait = new cMouseWait();
            id = cConstants.C_NO_ID;
            try
            {
                if (pGetNewId(table, fieldId, out id, false,
                               function, module, title, level))
                {
                    return true;
                }
                else
                {
                    if (pReconnectTry())
                    {
                        return pGetNewId(table, fieldId, out id, true,
                                         function, module, title, level);
                    }
                    else return false;
                }
            }
            catch (Exception ex)
            {
                cError.mngError(ex, "getNewId for " + module + "." + function, c_module, "");
                return false;
            }
            finally
            {
                mouseWait.Dispose();
            }

        }

        private bool pGetNewId(string table,
                               string fieldId,
                               out int id,
                               bool showError,
                               string function,
                               string module,
                               string title,
                               eErrorLevel level)
        {
            id = 0;
            try
            {

                string sqlstmt;
                DbDataReader ors;
                DbCommand ocmd;

                sqlstmt = "sp_dbgetnewid "
                                + sqlString(table) + ","
                                + sqlString(fieldId) + ",0";

                ocmd = createCommand(sqlstmt);
                ors = ocmd.ExecuteReader();
                if (ors.Read())
                {
                    id = (int)ors.GetInt32(0);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                cError.mngError(ex, "pGetNewId for " + module + "." + function, c_module, "");
                return false;
            }
        }

        //------------------------------
        // sql statment parser functions

        public string getSelect(string sqlstmt)
        {
            int i = sqlstmt.ToUpper().IndexOf("FROM");
            if (i >= 0)
            {
                return sqlstmt.Substring(0, i).Trim();
            }
            else
            {
                return sqlstmt;
            }
        }

        public string getFrom(string sqlstmt)
        {
            int i = sqlstmt.ToUpper().IndexOf("FROM");
            if (i >= 0)
            {
                sqlstmt = sqlstmt.Substring(i);
                i = sqlstmt.ToUpper().IndexOf("WHERE");
                if (i >= 0)
                {
                    return sqlstmt.Substring(0, i).Trim();
                }
                else
                {
                    i = sqlstmt.ToUpper().IndexOf("GROUP BY");
                    if (i >= 0)
                    {
                        return sqlstmt.Substring(0, i).Trim();
                    }
                    else
                    {
                        i = sqlstmt.ToUpper().IndexOf("ORDER BY");
                        if (i >= 0)
                        {
                            return sqlstmt.Substring(0, i).Trim();
                        }
                        else
                        {
                            return sqlstmt;
                        }
                    }
                }
            }
            else
            {
                return "";
            }
        }

        public string getWhere(string sqlstmt)
        {
            int i = sqlstmt.ToUpper().IndexOf("WHERE");
            if (i >= 0)
            {
                sqlstmt = sqlstmt.Substring(i);
                i = sqlstmt.ToUpper().IndexOf("GROUP BY");
                if (i >= 0)
                {
                    return sqlstmt.Substring(0, i).Trim();
                }
                else
                {
                    i = sqlstmt.ToUpper().IndexOf("ORDER BY");
                    if (i >= 0)
                    {
                        return sqlstmt.Substring(0, i).Trim();
                    }
                    else
                    {
                        return sqlstmt;
                    }
                }
            }
            else
            {
                return "";
            }
        }

        public string getGroup(string sqlstmt)
        {
            int i = sqlstmt.ToUpper().IndexOf("GROUP BY");
            if (i >= 0)
            {
                sqlstmt = sqlstmt.Substring(i);
                i = sqlstmt.ToUpper().IndexOf("ORDER BY");
                if (i >= 0)
                {
                    return sqlstmt.Substring(0, i).Trim();
                }
                else
                {
                    return sqlstmt;
                }
            }
            else
            {
                return "";
            }
        }

        public string getOrder(string sqlstmt)
        {
            int i = sqlstmt.ToUpper().IndexOf("ORDER BY");
            if (i >= 0)
            {
                return sqlstmt.Substring(i).Trim();
            }
            else
            {
                return "";
            }
        }

        public string getSearchSqlstmt(string sqlstmt, string toSearch)
        {
            try
            {
                string sqlSelect = getSelect(sqlstmt);
                string sqlFrom = getFrom(sqlstmt);
                string sqlWhere = getWhere(sqlstmt);
                string sqlGroup = getGroup(sqlstmt);
                string sqlOrder = getOrder(sqlstmt);
                string filter = "";

                toSearch = toSearch.Trim();

                if (toSearch != "")
                {
                    if (sqlWhere == "")
                    {
                        filter = " where (";
                    }
                    else
                    {
                        filter = " and (";
                    }

                    int i = 1;
                    string column;

                    column = pGetColumnFromStatement(sqlSelect, i);
                    while (column != "")
                    {
                        filter = filter + pGetStmtForColumn(column, toSearch, sqlSelect) + " or ";
                        i++;
                        column = pGetColumnFromStatement(sqlSelect, i);
                    }

                    //extrat last 'or' and put a parentheses
                    filter = filter.Substring(0, filter.Length - 3) + ")";
                }
                return sqlSelect + " " + sqlFrom + " " + sqlWhere + " " + filter + " " + sqlOrder + sqlGroup;
            }
            catch (Exception ex)
            {
                cError.mngError(ex, "getSearchSqlstmt", c_module, "");
                return "";
            }
        }

        private string pGetColumnFromStatement(string sqlstmt, int i)
        {
            int k;
            int q = 0;
            for (int p = 1; p <= i; p++)
            {
                int h = sqlstmt.IndexOf("=", q, StringComparison.Ordinal);
                int r = sqlstmt.IndexOf(",", q, StringComparison.Ordinal);

                if (h < r)
                {
                    k = h;
                }
                else
                {
                    k = r;
                }

                if (k < 0)
                {

                    if (h > r)
                    {
                        k = h;
                    }
                    else
                    {
                        k = r;
                    }

                    if (k < 0)
                    {
                        // if the index column where I am positioned is not lower than
                        // the index column required, the column required doesn't exists.
                        if (p < i)
                        {
                            return "";
                        }

                        break;
                    }
                }
                if (p == i)
                {
                    sqlstmt = sqlstmt.Substring(0, k);
                }
                else
                {
                    if (k == h)
                    {
                        if (r < 0)
                        {
                            return "";
                        }
                        k = r;
                    }
                    sqlstmt = sqlstmt.Substring(k + 1);
                }
            }

            sqlstmt = sqlstmt.Trim();
            q = sqlstmt.Length - 1;
            string c = sqlstmt.Substring(q, 1);
            if (c == "]")
            {
                do
                {
                    c = sqlstmt.Substring(q, 1);
                    if (c == "[")
                    {
                        break;
                    }
                    q--;
                } while (q >= 0);
            }
            else
            {
                do
                {
                    c = sqlstmt.Substring(q, 1);
                    if (c == " ")
                    {
                        break;
                    }
                    q--;
                } while (q >= 0);
            }
            q = q < 0 ? 0 : q;
            string field = sqlstmt.Substring(q).ToLower();
            if (String.CompareOrdinal(field, "select") == 0
                || String.CompareOrdinal(field, "distinct") == 0)
            {
                return "";
            }
            else
            {
                return field.Trim();
            }
        }

        private string pGetStmtForColumn(string column, string toSearch, string sqlstmt)
        {
            string realName = pGetColNameFromColExpression(column, sqlstmt);
            return "charindex('" + toSearch + "', convert(varchar(4000)," + realName + ")) > 0";
        }

        private string pGetColNameFromColExpression(string column, string sqlstmt)
        {
            string retval = "";
            string sep = "";
            string sqlSelect = "";
            string toSearch = "";

            sqlSelect = getSelect(sqlstmt).ToLower();
            toSearch = column.ToLower().Trim();

            int i = sqlSelect.IndexOf(toSearch);
            if (i >= 0)
            {
                retval = sqlSelect.Substring(i + toSearch.Length).Trim().Replace("'", " ").Trim();
                if (retval.Substring(0, 1) == "=")
                {
                    retval = retval.Substring(1);
                    sep = ",";
                    int q = retval.IndexOf(",");
                    int t = retval.IndexOf(" ");
                    if (q < 0)
                    {
                        sep = " ";
                    }
                    else if (t >= 0)
                    {
                        if (q > t)
                        {
                            sep = " ";
                        }
                    }
                    int w = retval.IndexOf(sep);
                    if (w >= 0)
                    {
                        retval = retval.Substring(0, w).Trim();
                    }
                }
                else
                {
                    retval = column;
                }
            }
            else
            {
                retval = column;
            }
            return retval;
        }

        private void pReconnect()
        {
            closeDb();
            pConnect();
        }

        private bool pReconnectTry()
        {
            closeDb();
            try
            {
                pConnect();
                return true;
            }
            catch (Exception ex)
            {
                cError.mngError(ex, "pReconnectTry", c_module, "");
                return false;
            }
        }

        private void pConnect()
        {
            m_ocn.ConnectionString = translateFromAdoIfNeeded(m_connect);
            m_ocn.Open();
        }

        public cDataBase(csDatabaseEngine databaseEngine)
        {
            m_databaseEngine = databaseEngine;
        }

        private DbConnection createConnection()
        {
            switch (m_databaseEngine)
            { 
                case csDatabaseEngine.SQL_SERVER:
                    return new SqlConnection();
                case csDatabaseEngine.POSTGRESQL:
                    throw new NotImplementedException();
                case csDatabaseEngine.ORACLE:
                    return new OracleConnection();
            }
            throw new Exception("The database engine is not supported " + m_databaseEngine.ToString());
        }

        private DbCommand createCommand(string sqlstmt)
        {
            DbCommand ocmd = null;
            
            switch (m_databaseEngine)
            {
                case csDatabaseEngine.SQL_SERVER:
                    ocmd = new SqlCommand(sqlstmt, m_ocn as SqlConnection);
                    break;
                case csDatabaseEngine.POSTGRESQL:
                    throw new NotImplementedException();
                case csDatabaseEngine.ORACLE:
                    ocmd = new OracleCommand(sqlstmt, m_ocn as OracleConnection);
                    break;
            }
            
            if(ocmd == null)
                throw new Exception("The database engine is not supported " + m_databaseEngine.ToString());
            
            ocmd.CommandTimeout = m_commandTimeout;
            ocmd.CommandType = CommandType.Text;
            
            return ocmd;
        }

        /*
            
         * // .NET DataProvider -- Standard Connection with username and password

            SqlConnection conn = new SqlConnection();
            conn.ConnectionString =
            "Data Source=ServerName;" +
            "Initial Catalog=DataBaseName;" +
            "User id=UserName;" +
            "Password=Secret;";
            conn.Open();

         * // .NET DataProvider -- Trusted Connection
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString =
            "Data Source=ServerName;" +
            "Initial Catalog=DataBaseName;" +
            "Integrated Security=SSPI;";
            conn.Open();
         
         * ADO
         * "Provider=SQLOLEDB.1;Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=cairo;Data Source=daimaku;"
         * 
         */
        private string translateFromAdoIfNeeded(string strConnect)
        {
            if (m_databaseEngine == csDatabaseEngine.SQL_SERVER)
            {

                if (strConnect.IndexOf("Provider=") > -1)
                {
                    var dataSource = cUtil.getToken("Data Source", strConnect);
                    var initialCatalog = cUtil.getToken("Initial Catalog", strConnect);
                    var trusted = cUtil.getToken("Integrated Security", strConnect);
                    var userId = cUtil.getToken("User ID", strConnect);
                    var password = cUtil.getToken("Password", strConnect);
                    if (trusted == "SSPI")
                    {
                        strConnect = String.Format("Data Source={0};Initial Catalog={1};Integrated Security=SSPI;", dataSource, initialCatalog);
                    }
                    else
                    {
                        strConnect = String.Format("Data Source={0};Initial Catalog={1};User id={2};Password={3};", dataSource, initialCatalog, userId, password);
                    }
                }
            }
            return strConnect;
        }

    }

    public enum csDatabaseEngine
    { 
        SQL_SERVER = 1,
        POSTGRESQL = 2,
        ORACLE = 3
    }
}
