using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CSReportEditor
{
    public partial class fSecProperties : Form
    {
        private bool m_formulaHideChanged;
        private bool m_setFormulaHideChanged;
        private String m_formulaHide = "";

        public fSecProperties()
        {
            InitializeComponent();
        }

		public string getFormulaName ()
		{
			throw new NotImplementedException ();
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

        public System.Windows.Forms.Label lbSecLn
        {
            get
            {
                return lb_secLn;
            }
        }
        public System.Windows.Forms.Label lbControl
        {
            get
            {
                return lb_control;
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
            throw new NotImplementedException();
        }
    }
}
