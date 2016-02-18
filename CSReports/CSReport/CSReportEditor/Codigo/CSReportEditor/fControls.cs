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
using CSReportDll;

namespace CSReportEditor
{
    public partial class fControls : Form
    {
        private cEditor m_editor;

        private const int C_CTRL_IMAGE = 1;
        private const int C_DB_IMAGE = 0;

        private cListViewColumnSorter lvwColumnSorter;

        public fControls()
        {
            InitializeComponent();
        }

		public void clear()
		{
            lv_controls.Items.Clear();
		}

		public void addCtrls(cReport report)
		{
            cGlobals.addCtrls(report, lv_controls, C_CTRL_IMAGE, C_DB_IMAGE);          
		}

        public void setHandler(cEditor editor)
        {
            m_editor = editor;
        }

        private void fControls_Load(object sender, EventArgs e)
        {
            cWindow.locateFormAtLeft(this);

            // Create an instance of a ListView column sorter and assign it 
            // to the ListView control.
            lvwColumnSorter = new cListViewColumnSorter();
            lv_controls.ListViewItemSorter = lvwColumnSorter;
            lv_controls_ColumnClick(this, new ColumnClickEventArgs(0));
        }

        private void lv_controls_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // Determine if clicked column is already the column that is being sorted.
            if (e.Column == lvwColumnSorter.SortColumn)
            {
                // Reverse the current sort direction for this column.
                if (lvwColumnSorter.Order == SortOrder.Ascending)
                {
                    lvwColumnSorter.Order = SortOrder.Descending;
                }
                else
                {
                    lvwColumnSorter.Order = SortOrder.Ascending;
                }
            }
            else
            {
                // Set the column number that is to be sorted; default to ascending.
                lvwColumnSorter.SortColumn = e.Column;
                lvwColumnSorter.Order = SortOrder.Ascending;
            }

            // Perform the sort with these new sort options.
            lv_controls.Sort();
        }

        private void lv_controls_MouseClick(object sender, MouseEventArgs e)
        {
            selectControl();
        }

        private void lv_controls_KeyUp(object sender, KeyEventArgs e)
        {
            selectControl();
        }

        private void selectControl()
        {
            if (lv_controls.SelectedItems.Count > 0)
            {
                var info = lv_controls.SelectedItems[0].Tag.ToString();
                m_editor.selectCtrl(info);
            }
        }

        private void cmd_close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cmd_edit_Click(object sender, EventArgs e)
        {
            if (lv_controls.SelectedItems.Count > 0)
            {
                var info = lv_controls.SelectedItems[0].Tag.ToString();
                m_editor.showProperties(info);
            }
        }
    }
}
