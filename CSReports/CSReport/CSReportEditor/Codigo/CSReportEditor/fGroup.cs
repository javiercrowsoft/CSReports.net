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
    public partial class fGroup : Form, cIDatabaseFieldSelector
    {
        private cEditor m_editor;
        private bool m_ok = false;
        private bool m_dbFieldChanged = false;
        private int m_index = 0;
        private int m_fieldType = 0;

        public fGroup()
        {
            InitializeComponent();
        }

        public TextBox txName
        {
            get
            {
                return tx_name;
            }
        }

        public TextBox txDbField
        {
            get
            {
                return tx_dbField;
            }
        }

        public RadioButton opDesc
        {
            get 
            {
                return op_desc;
            }
        }

        public RadioButton opAsc
        {
            get
            {
                return op_asc;
            }
        }

        public CheckBox chkPrintInNewPage
        {
            get 
            {
                return chk_printInNewPage;
            }
        }

        public CheckBox chkReprintGroup
        {
            get
            {
                return chk_reprintGroup;
            }
        }

        public CheckBox chkGrandTotal
        {
            get
            {
                return chk_grandTotal;
            }
        }

        public RadioButton opDate
        {
            get
            {
                return op_date;
            }
        }

        public RadioButton opNumber
        {
            get
            {
                return op_number;
            }
        }

        public RadioButton opText
        {
            get
            {
                return op_text;
            }
        }

        public Label lbGroup
        {
            get
            {
                return lb_group;
            }
        }

        public bool getAsc()
        {
            return op_asc.Checked;
        }

        public void setAsc(bool value)
        {
            op_asc.Checked = value;
        }

        public void setDesc(bool value)
        {
            op_desc.Checked = value;
        }

        public bool getPrintInNewPage()
        {
            return chk_printInNewPage.Checked;
        }

        public void setPrintInNewPage(bool value)
        {
            chk_printInNewPage.Checked = value;
        }

        public bool getReprintGroup()
        {
            return chk_reprintGroup.Checked;
        }

        public void setReprintGroup(bool value)
        {
            chk_reprintGroup.Checked = value;
        }

        public bool getGrandTotal()
        {
            return chk_grandTotal.Checked;
        }

        public void setGrandTotal(bool value)
        {
            chk_grandTotal.Checked = value;
        }

        public bool getSortByDate()
        {
            return op_date.Checked;
        }

        public void setSortByDate(bool value)
        {
            op_date.Checked = value;
        }

        public bool getSortByNumber()
        {
            return op_number.Checked;
        }

        public void setSortByNumber(bool value)
        {
            op_number.Checked = value;
        }

        public bool getSortByText()
        {
            return op_text.Checked;
        }

        public void setSortByText(bool value)
        {
            op_text.Checked = value;
        }

        public bool getOk()
        {
            return m_ok;
        }

		public string getDbField ()
		{
			throw new NotImplementedException ();
		}

        public int getFieldType()
        {
            return m_fieldType;
        }

        public void setFieldType(int rhs)
        {
            m_fieldType = rhs;
        }

        public int getIndex()
        {
            return m_index;
        }

        public void setIndex(int rhs)
        {
            m_index = rhs;
        }

        private void cmdOk_Click(object sender, EventArgs e)
        {
            m_ok = true;
            this.Hide();
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            m_ok = false;
            this.Hide();
        }

        public void setHandler(cEditor editor)
        {
            m_editor = editor;
        }

        private void cmd_dbField_Click(object sender, EventArgs e)
        {
            if (m_editor.showHelpDbFieldForGroup())
            {
                m_dbFieldChanged = true;
            }
        }

        private void fGroup_Load(object sender, EventArgs e)
        {
            cWindow.centerForm(this);
        }
    }
}
