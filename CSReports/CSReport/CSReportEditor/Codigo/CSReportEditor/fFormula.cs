using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CSKernelClient;
using CSReportGlobals;

namespace CSReportEditor
{
    public partial class fFormula : Form
    {

        private const String C_KEY_SYSFUNCTIONS = "FS";
        private const String C_KEY_SYSVARS = "VS";
        private const String C_KEY_SYSLABELS = "VL";
        private const String C_KEY_SYSDBFIELDS = "VC";

        private const String C_FUNID = "I";
        private const String C_FUNDESCRIP = "D";
        private const String C_FUNNAME = "N";
        private const String C_HELPCONTEXTID = "H";
        private const String C_ISDBFIELDORLABEL = "FL";

        private const int C_FOLDER_INDEX = 0;
        private const int C_DATABSE_INDEX = 1;
        private const int C_LABEL_INDEX = 2;
        private const int C_FORMULA_INDEX = 3;

        private bool m_ok = false;

        public fFormula()
        {
            InitializeComponent();
        }

		public void createTree()
		{
            tv_formulas.Nodes.Add(C_KEY_SYSFUNCTIONS, "Internal functions", C_FOLDER_INDEX);
            var item = tv_formulas.Nodes.Add(C_KEY_SYSVARS, "Internal variables", C_FOLDER_INDEX);
            item.Nodes.Add(C_KEY_SYSDBFIELDS, "Database fields");
            item.Nodes.Add(C_KEY_SYSLABELS, "Labels");
		}

		public void addFormula(csRptFormulaType formulaType, string name, string nameUser, string descrip, int helpContextId)
		{
            var item = tv_formulas.Nodes[C_KEY_SYSFUNCTIONS].Nodes.Add(nameUser);
            item.ImageIndex = C_FORMULA_INDEX;
            item.SelectedImageIndex = item.ImageIndex;

            string info = "";
            info += cUtil.setInfoString(info, C_FUNID, formulaType.ToString());
            info += cUtil.setInfoString(info, C_FUNDESCRIP, descrip);
            info += cUtil.setInfoString(info, C_FUNNAME, name);
            info += cUtil.setInfoString(info, C_HELPCONTEXTID, helpContextId.ToString());

            item.Tag = info;
		}

		public void addDBField(string name, string descrip)
		{
            addAux(name, descrip, C_KEY_SYSDBFIELDS, C_DATABSE_INDEX);
		}

		public void addLabel(string name)
		{
            addAux(name, "", C_KEY_SYSLABELS, C_LABEL_INDEX);
		}

		public void setFormula(string formula)
		{
			tx_formula.Text = formula;
		}

		public void expandTree ()
		{
            tv_formulas.Nodes[0].ExpandAll();
            tv_formulas.Nodes[1].ExpandAll();
		}

		public bool getOk()
		{
            return m_ok;
		}

		public string getFormula()
		{
			return tx_formula.Text;
		}

        private void addAux(String name, String descrip, String key, int image) {
            var father = tv_formulas.Nodes[C_KEY_SYSVARS].Nodes[key];
            var item = father.Nodes.Add(name);
            item.ImageIndex = image;
            item.SelectedImageIndex = item.ImageIndex;

            if (descrip != "") 
            {
                item.Text = descrip + " ( "+ name + " )";
            }
            
            var info = "";
            info += cUtil.setInfoString(info, C_FUNDESCRIP, descrip);
            info += cUtil.setInfoString(info, C_FUNNAME, name);
            info += cUtil.setInfoString(info, C_ISDBFIELDORLABEL, "1");

            item.Tag = info;
        }

        private void fFormula_Load(object sender, EventArgs e)
        {
            cWindow.centerForm(this);
        }
    }
}
