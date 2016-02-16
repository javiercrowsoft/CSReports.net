using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CSKernelClient
{
    public partial class fInput : Form
    {
        private bool m_ok = false;

        public fInput()
        {
            InitializeComponent();
        }

        public bool getOk()
        {
            return m_ok;
        }

        public void setTitle(string title)
        {
            lb_title.Text = title;
        }

        public void setDescrip(string descrip)
        {
            lb_descrip.Text = descrip;
        }

        public void setText(string text)
        {
            tx_server.Text = text;
        }

        public string getText()
        {
            return tx_server.Text;
        }

        private void cmd_apply_Click(object sender, EventArgs e)
        {
            m_ok = true;
            this.Close();
        }

        private void cmd_cancel_Click(object sender, EventArgs e)
        {
            m_ok = false;
            this.Close();
        }

        private void fInput_Load(object sender, EventArgs e)
        {
            cWindow.centerForm(this);
        }

    }
}
