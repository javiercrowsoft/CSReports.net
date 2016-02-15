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
    public partial class fToolbox : Form
    {
        private cEditor m_editor;

        private const int C_CTRL_IMAGE = 0;
        private const int C_LABEL_IMAGE = 1;
        private const int C_FORMULA_FOLDER_IMAGE = 2;
        private const int C_FORMULA_IMAGE = 3;

        private const string C_CONTROL_NAME = "C";
        private const string C_FORMULA_NAME = "F";

        private const string C_FIELD_INDEX = "FC";
        private const string C_FIELD_TYPE = "FT";

        public fToolbox()
        {
            InitializeComponent();
        }

        public void clear()
        {
            lv_controls.Items.Clear();
            lv_labels.Items.Clear();
            lv_formulas.Items.Clear();
        }

        internal void addLbFormula(string controlName)
        {
            lv_formulas.Items.Add(controlName, C_FORMULA_FOLDER_IMAGE);
        }

        internal void addFormula(string name, string controlName, string formulaName)
        {
            var item = lv_formulas.Items.Add(name, C_FORMULA_IMAGE);
            var info = "";
            info = cUtil.setInfoString(info, C_CONTROL_NAME, controlName);
            info = cUtil.setInfoString(info, C_FORMULA_NAME, formulaName);
            item.Tag = info;
        }

        internal void addField(string name, int fieldType, int fieldIndex)
        {
            var item = lv_controls.Items.Add(name, C_CTRL_IMAGE);
            var info = "";
            info = cUtil.setInfoString(info, C_FIELD_INDEX, fieldType.ToString());
            info = cUtil.setInfoString(info, C_FIELD_TYPE, fieldIndex.ToString());
            item.Tag = info;
        }

        internal void addLabels(string name)
        {
            lv_labels.Items.Add(name, C_LABEL_IMAGE);
        }

        public void setHandler(cEditor editor)
        {
            m_editor = editor;
        }

    }
}
