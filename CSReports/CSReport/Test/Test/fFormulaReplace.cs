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
    public partial class fFormulaReplace : Form
    {
        public fFormulaReplace()
        {
            InitializeComponent();
        }

        //------------------------------------------------------------------------------------------------------------------

        // expose controls

        //------------------------------------------------------------------------------------------------------------------

        public System.Windows.Forms.TextBox txCurrFormula
        {
            get
            {
                return tx_currFormula;
            }
        }
        public System.Windows.Forms.TextBox txNewFormula
        {
            get
            {
                return tx_newFormula;
            }
        }

        internal bool getOk()
        {
            throw new NotImplementedException();
        }
    }
}
