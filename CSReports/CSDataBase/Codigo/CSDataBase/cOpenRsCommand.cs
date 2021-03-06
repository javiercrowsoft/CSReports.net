﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Este código fue generado por una herramienta.
//     Versión del motor en tiempo de ejecución:2.0.50727.3603
//
//     Los cambios en este archivo podrían causar un comportamiento incorrecto y se perderán si
//     se vuelve a generar el código.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Data.Common;
using System.Data.OracleClient;
using CSKernelClient;

namespace CSDataBase
{


    public class cOpenRsCommand
    {
        private const string c_module = "cDataBase";

        private delegate DbDataReader delegateAsyncOpenRsEx(string sqlstmt);

        private delegateAsyncOpenRsEx m_invoke = null;
        private DbDataReader m_ors = null;
        private string m_sqlstmt = "";
        private bool m_done = false;

        public bool done
        {
            get { return m_done; }
        }

        public bool success
        {
            get { return m_ors != null; }
        }

        public DbDataReader ors
        {
            get { return m_ors; }
        }

        public void getExecuteCommand(cDataBase db, string sqlstmt)
        {
            m_sqlstmt = sqlstmt;
            m_invoke = new delegateAsyncOpenRsEx(db.asyncOpenRsEx);
        }

        public void execute()
        {
            try
            {
                m_invoke.BeginInvoke(m_sqlstmt, this.callBack, null);
            }
            catch (Exception ex)
            {
                cError.mngError(ex, "execute", c_module, "");
            }
        }

        private void callBack(IAsyncResult ar)
        {
            try
            {
                m_ors = m_invoke.EndInvoke(ar);
                m_done = true;
            }
            catch (Exception ex)
            {
                cError.mngError(ex, "callBack", c_module, "");
            }
        }

        public cOpenRsCommand()
        {
        }
    }
}
