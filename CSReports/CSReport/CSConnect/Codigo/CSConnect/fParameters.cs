using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CSKernelClient;
using CSDataBase;

namespace CSConnect
{
    public partial class fParameters : Form
    {
        private bool m_ok = false;

        private cParameters m_parameters;

        public fParameters()
        {
            InitializeComponent();
        }

        public bool getOk()
        {
            return m_ok;
        }

        private void cmd_apply_Click(object sender, EventArgs e)
        {
            m_ok = true;
            this.Hide();
        }

        private void cmd_cancel_Click(object sender, EventArgs e)
        {
            m_ok = false;
            this.Hide();
        }

        private void fParameters_Load(object sender, EventArgs e)
        {
            cWindow.centerForm(this);
            loadParameters();
        }

        internal void setParameters(cParameters value)
        {
            m_parameters = value;
        }

        internal string getSqlParameters()
        {
            return "";
        }

        private void loadParameters()
        {
            int top = 20;

            for (int j = 0; j < m_parameters.count(); j++) 
            {
                cParameter parameter = m_parameters.item(j);

                System.Windows.Forms.Label label = new System.Windows.Forms.Label();
                label.AutoSize = true;
                label.Location = new System.Drawing.Point(30, top);
                label.Text = parameter.getName();

                System.Windows.Forms.TextBox input = new System.Windows.Forms.TextBox();
                input.Location = new System.Drawing.Point(150, top);
                input.Size = new System.Drawing.Size(150, 20);
                input.Text = parameter.getValue();
                input.Tag = parameter.getKey();

                switch(parameter.getTypeColumn())
                {
                    case csDataType.CSTDLONGVARCHAR:
                    case csDataType.CSTDCHAR:
                        input.Tag = "T";
                        break;
                    case csDataType.CSTDBIGINT:
                    case csDataType.CSTDBINARY:
                    case csDataType.CSTDINTEGER:
                    case csDataType.CSTDSMALLINT:
                    case csDataType.CSTDTINYINT:
                    case csDataType.CSTDUNSIGNEDTINYINT:
                        input.Tag = "N";
                        break;
                    case csDataType.CSTDBOOLEAN:
                        input.Tag = "N";
                        break;
                    case csDataType.CSTDSINGLE:
                    case csDataType.CSTDDECIMAL:
                    case csDataType.CSTDDOUBLE:
                        input.Tag = "N";
                        break;
                    case csDataType.CSTDDBTIME:
                        input.Tag = "F";
                        break;
                }
                
                pnlParameters.Controls.Add(label);
                pnlParameters.Controls.Add(input);

                top += 30;
            }
        } 

    }
}
