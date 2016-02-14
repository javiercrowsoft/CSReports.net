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
    public partial class fSecProperties : Form
    {
        private cEditor m_editor;

        private bool m_ok = false;

        private bool m_formulaHideChanged;
        private bool m_setFormulaHideChanged;
        private String m_formulaHide = "";

        private String m_formulaName = "";

        public fSecProperties()
        {
            InitializeComponent();
        }

        public void setHandler(cEditor editor)
        {
            m_editor = editor;
        }

		public string getFormulaName ()
		{
			return m_formulaName;
		}

        public String getFormulaHide()
        {
            return m_formulaHide;
        }

        public void setFormulaHide(String rhs)
        {
            m_formulaHide = rhs;
        }

        public bool getFormulaHideChanged()
        {
            return m_formulaHideChanged;
        }

        public void setFormulaHideChanged(bool rhs)
        {
            m_formulaHideChanged = rhs;
        }

        public bool getSetFormulaHideChanged()
        {
            return m_setFormulaHideChanged;
        }

        public void setSetFormulaHideChanged(bool rhs)
        {
            m_setFormulaHideChanged = rhs;
        }

        //------------------------------------------------------------------------------------------------------------------

        // expose controls

        //------------------------------------------------------------------------------------------------------------------

        public System.Windows.Forms.Label lbSectionName
        {
            get
            {
                return lb_section;
            }
        }
        public System.Windows.Forms.TextBox txName
        {
            get
            {
                return tx_name;
            }
        }
        public System.Windows.Forms.Label lbFormulaHide
        {
            get
            {
                return lb_formulaHide;
            }
        }
        public System.Windows.Forms.CheckBox chkFormulaHide
        {
            get
            {
                return chk_formulaHide;
            }
        }
        public System.Windows.Forms.Button cmdFormulaHide
        {
            get
            {
                return cmd_formulaHide;
            }
        }

        internal bool getOk()
        {
            return m_ok;
        }

        private void fSecProperties_Load(object sender, EventArgs e)
        {
            cWindow.centerForm(this);
            lb_formulaHide.Text = m_formulaHide;
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

        private void cmd_formulaHide_Click(object sender, EventArgs e)
        {
            m_formulaName = "Ocultar";
            if (m_editor.showEditFormula(ref m_formulaHide))
            {
                m_formulaHideChanged = true;
                lb_formulaHide.Text = m_formulaHide;
            }
        }
    }
}
