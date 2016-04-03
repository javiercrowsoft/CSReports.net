using System;
using System.Windows.Forms;
using CSKernelClient;
using CSReportGlobals;

namespace CSReportEditor
{
    public partial class fPageSetup : Form
    {
        private bool m_ok = false;
        private int m_customHeight;
        private int m_customWidth;
        private int m_orientation = 1;
        private csReportPaperType m_paperSize = csReportPaperType.CSRPTPAPERTYPEA4;

        public fPageSetup()
        {
            InitializeComponent();
        }

        public void initDialog(csReportPaperType paperSize, int customHeight, int customWidth, int orientation)
        {
            m_customHeight = customHeight;
            m_customWidth = customWidth;
            m_orientation = orientation;
            m_paperSize = paperSize;
        }

        public void setCustomHeight(int rhs)
        {
            m_customHeight = rhs;
        }

        public void setCustomWidth(int rhs)
        {
            m_customWidth = rhs;
        }

        public void setOrientation(int rhs)
        {
            m_orientation = rhs;
        }

        public csReportPaperType getPaperSize()
        {
            return m_paperSize;
        }

        public int getCustomHeight()
        {
            return m_customHeight;
        }

        public int getCustomWidth()
        {
            return m_customWidth;
        }

        public int getOrientation()
        {
            return m_orientation;
        }

        public bool getOk()
        {
            return m_ok;
        }

        private void op_portrait_CheckedChanged(object sender, EventArgs e)
        {
            pic_landscape.Visible = false;
            pic_portrait.Visible = true;
        }

        private void op_landscape_CheckedChanged(object sender, EventArgs e)
        {
            pic_portrait.Visible = false;
            pic_landscape.Visible = true;
        }

        private void cmd_cancel_Click(object sender, EventArgs e)
        {
            m_ok = false;
            this.Hide();
        }

        private void cmd_apply_Click(object sender, EventArgs e)
        {
            m_ok = true;
            m_customHeight = (int)cUtil.val(tx_height.Text);
            m_customWidth = (int)cUtil.val(tx_width.Text);
            m_paperSize = (csReportPaperType)cUtil.listID(cb_paperSize);
            m_orientation = op_landscape.Checked ? (int)csRptPageOrientation.LANDSCAPE : (int)csRptPageOrientation.PORTRAIT;
            this.Hide();
        }

        private void fPageSetup_Load(object sender, EventArgs e)
        {
            cUtil.listAdd(cb_paperSize, "Letter", (int)csReportPaperType.CSRPTPAPERTYPELETTER);
            cUtil.listAdd(cb_paperSize, "A4", (int)csReportPaperType.CSRPTPAPERTYPEA4);
            cUtil.listAdd(cb_paperSize, "Legal", (int)csReportPaperType.CSRPTPAPERTYPELEGAL);
            cUtil.listAdd(cb_paperSize, "A3", (int)csReportPaperType.CSRPTPAPERTYPEA3);
            cUtil.listAdd(cb_paperSize, "User", (int)csReportPaperType.CSRPTPAPERUSER);
            cUtil.listSetListIndexForId(cb_paperSize, (int)m_paperSize);
            tx_height.Text = m_customHeight.ToString();
            tx_width.Text = m_customWidth.ToString();
            if (m_orientation == (int)csRptPageOrientation.LANDSCAPE)
            {
                op_landscape.Checked = true;
            }
            else
            {
                op_portrait.Checked = true;
            }
            cWindow.centerForm(this);
        }

        private void cb_paperSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            var enabled = cUtil.listID(cb_paperSize) == (int)csReportPaperType.CSRPTPAPERUSER;
            tx_height.Enabled = enabled;
            tx_width.Enabled = enabled;
        }
    }
}
