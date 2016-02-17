using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CSKernelClient;

namespace CSReportEditor
{
    public partial class fSimpleConnect : Form
    {
        private bool m_ok = false;

        public fSimpleConnect()
        {
            InitializeComponent();
        }

		public void setServer(string value)
		{
            tx_server.Text = value;
		}

		public void setDataBase(string value)
		{
            tx_database.Text = value;
		}

		public void setUser(string value)
		{
            tx_user.Text = value;
		}

		public void setPassword(string value)
		{
            tx_password.Text = value;
		}

		public string getUser ()
		{
            return tx_user.Text;
		}

		public void setConnectTypeToNT()
		{
            op_trustedConnection.Checked = true;
		}

		public void setConnectTypeToSQL()
		{
            op_sqlConnection.Checked = true;
		}

		public bool getOk ()
		{
            return m_ok;
		}

		public string getStrConnect ()
		{
            string strConnect;
			if(op_trustedConnection.Checked)
            {
                strConnect = "Provider=SQLOLEDB.1;";
                strConnect += "Integrated Security=SSPI;";
                strConnect += "Persist Security Info=False;";
                strConnect += "Initial Catalog=" + tx_database.Text + ";";
                strConnect += "Data Source=" + tx_server.Text + ";";
            }
            else
            {
                strConnect = "Provider=SQLOLEDB.1;";
                strConnect += "Persist Security Info=True;";
                strConnect += "Data Source=" + tx_server.Text + ";";
                strConnect += "User ID=" + tx_user.Text + ";";
                strConnect += "Password=" + tx_password.Text + ";";
                strConnect += "Initial Catalog=" + tx_database.Text + ";";
            }
            return strConnect;
		}

        private void cmd_apply_Click(object sender, EventArgs e)
        {
            if (op_sqlConnection.Checked && tx_user.Text == "")
            {
                cWindow.msgWarning("You must indicate a user");
            }
            else
            {
                m_ok = true;
                this.Close();
            }
        }

        private void cmd_cancel_Click(object sender, EventArgs e)
        {
            m_ok = false;
            this.Close();
        }

        private void op_sqlConnection_CheckedChanged(object sender, EventArgs e)
        {
            setEnabledUserAndPassword();
        }

        private void op_trustedConnection_CheckedChanged(object sender, EventArgs e)
        {
            setEnabledUserAndPassword();
        }

        private void setEnabledUserAndPassword()
        {
            tx_user.Enabled = op_sqlConnection.Checked;
            tx_password.Enabled = op_sqlConnection.Checked;
        }

        private void fSimpleConnect_Load(object sender, EventArgs e)
        {
            cWindow.centerForm(this);
        }
    }
}
