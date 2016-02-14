using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CSKernelClient;
using CSReportDll;

namespace CSReportEditor
{
    public partial class fColumns : Form
    {

        private const String C_FIELDTYPE = "t";
        private const String C_INDEX = "i";

        private string m_field = "";
        private int m_fieldType = -1;
        private int m_fieldIndex = -1;

        private bool m_ok = false;

        public fColumns()
        {
            InitializeComponent();
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

        internal void clearColumns()
        {
            lv_columns.Items.Clear();
        }

        internal void fillColumns(string dataSource, cColumnsInfo columns)
        {
            foreach (cColumnInfo column in columns) 
            {
                var item = lv_columns.Items.Add(String.Format("{{{0}}}.{1}", dataSource, column.getName()));
                item.ImageIndex = 0;
                string info = cUtil.setInfoString("", C_INDEX, column.getPosition().ToString());
                info = cUtil.setInfoString(info, C_FIELDTYPE, column.getColumnType().ToString());
                item.Tag = info;
            }
        }

        internal void setField(string field)
        {
            m_field = field;
            foreach (ListViewItem item in lv_columns.Items)
            {
                if (item.Text == field) 
                {
                    item.Selected = true;
                    item.Focused = true;
                    lv_columns.Select();
                    break;
                }
            }
        }

        internal bool getOk()
        {
            return m_ok;
        }

        internal string getField()
        {
            return m_field;
        }

        internal int getFieldType()
        {
            return m_fieldType;
        }

        internal int getIndex()
        {
            return m_fieldIndex;
        }

        private void lv_columns_Click(object sender, EventArgs e)
        {
            if (lv_columns.SelectedItems.Count > 0)
            {
                ListViewItem item = lv_columns.SelectedItems[0];
                m_field = item.Text;
                var info = item.Tag.ToString();
                m_fieldType = cUtil.valAsInt(cUtil.getInfoString(info, C_FIELDTYPE, "-1"));
                m_fieldIndex = cUtil.valAsInt(cUtil.getInfoString(info, C_INDEX, "-1"));
            }
        }

        private void fColumns_Load(object sender, EventArgs e)
        {
            cWindow.centerForm(this);
        }
    }
}
