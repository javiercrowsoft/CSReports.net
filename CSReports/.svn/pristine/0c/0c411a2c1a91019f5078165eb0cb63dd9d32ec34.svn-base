using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CSMaskEdit;

namespace CSReportEditor
{
    public partial class fGroup : Form
    {
        private bool m_ok = false;

        public fGroup()
        {
            InitializeComponent();
        }

        public cMaskEdit getTxName()
        {
            return txName;
        }

        public cMaskEdit getTxDbField()
        {
            return txDbField;
        }

        public bool getAsc()
        {
            return opAsc.Checked;
        }

        public void setAsc(bool value)
        {
            opAsc.Checked = value;
        }

        public void setDesc(bool value)
        {
            opDesc.Checked = value;
        }

        public bool getPrintInNewPage()
        {
            return chkPrintInNewPage.Checked;
        }

        public void setPrintInNewPage(bool value)
        {
            chkPrintInNewPage.Checked = value;
        }

        public bool getReprintGroup()
        {
            return chkReprintGroup.Checked;
        }

        public void setReprintGroup(bool value)
        {
            chkReprintGroup.Checked = value;
        }

        public bool getGrandTotal()
        {
            return chkGrandTotal.Checked;
        }

        public void setGrandTotal(bool value)
        {
            chkGrandTotal.Checked = value;
        }

        public bool getSortByDate()
        {
            return opDate.Checked;
        }

        public void setSortByDate(bool value)
        {
            opDate.Checked = value;
        }

        public bool getSortByNumber()
        {
            return opNumber.Checked;
        }

        public void setSortByNumber(bool value)
        {
            opNumber.Checked = value;
        }

        public bool getSortByText()
        {
            return opText.Checked;
        }

        public void setSortByText(bool value)
        {
            opText.Checked = value;
        }

        public bool getOk()
        {
            return m_ok;
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
    }
}
