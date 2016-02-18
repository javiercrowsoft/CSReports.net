using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using CSKernelClient;
using CSDataBase;

namespace CSConnect
{
    public partial class fParameters : Form
    {
        private bool m_ok = false;
        private TextBox[] m_texts = null;

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
        public string getSqlParameters()
        {
            var parameters = "";

            for (int i = 0; i < m_parameters.count(); i++)
            {
                var input = m_texts[i];
                var value = "";

                switch(input.Tag.ToString())
                {
                    case "T":
                        value = "'" + input.Text.Replace("'", "''") + "'";
                        break;
                    case "N":
                        value = getNumberSql(input.Text);
                        break;
                    case "F":
                        DateTime dt;
                        if (DateTime.TryParseExact(input.Text, "MM/dd/yyyy", null, DateTimeStyles.None, out dt)) {}
                        else if (DateTime.TryParseExact(input.Text, "dd/MM/yyyy", null, DateTimeStyles.None, out dt)) {}
                        else if (DateTime.TryParseExact(input.Text, "MM-dd-yyyy", null, DateTimeStyles.None, out dt)) { }
                        else if (DateTime.TryParseExact(input.Text, "dd-MM-yyyy", null, DateTimeStyles.None, out dt)) {}                        
                        else if (DateTime.TryParseExact(input.Text, "MM.dd.yyyy", null, DateTimeStyles.None, out dt)) {}
                        else if (DateTime.TryParseExact(input.Text, "dd.MM.yyyy", null, DateTimeStyles.None, out dt)) { }
                        else throw new Exception("Invalid date " + input.Text);
                        value =  "'" + dt.ToString(CSDataBase.cConstants.C_SQL_DATE_STRING, CultureInfo.InvariantCulture) + "'";                        
                        break;
                }
                m_parameters.item(i).setValue(value);
                parameters += value + ",";
            }

            if (parameters.Substring(parameters.Length - 1, 1) == ",")
            {
                parameters = parameters.Substring(0, parameters.Length - 1);
            }
  
            return parameters;
        }

        private void loadParameters()
        {
            int top = 20;

            m_texts = new TextBox[m_parameters.count()];

            for (int j = 0; j < m_parameters.count(); j++) 
            {
                cParameter parameter = m_parameters.getByPosition(j+1);

                System.Windows.Forms.Label label = new System.Windows.Forms.Label();
                label.AutoSize = true;
                label.Location = new System.Drawing.Point(30, top);
                label.Text = parameter.getName();

                System.Windows.Forms.TextBox input = new System.Windows.Forms.TextBox();
                input.Location = new System.Drawing.Point(150, top);
                input.Size = new System.Drawing.Size(150, 20);
                input.Text = parameter.getValue();
                input.Tag = parameter.getKey();

                switch(parameter.getColumnType())
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

                m_texts[j] = input;
                
                pnlParameters.Controls.Add(label);
                pnlParameters.Controls.Add(input);

                top += 30;
            }
        } 

    }
}
