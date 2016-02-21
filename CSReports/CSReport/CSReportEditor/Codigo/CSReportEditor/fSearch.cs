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
    public partial class fSearch : Form
    {
        private cEditor m_editor = null;

        private enum csObjType {
            iTypeFormulaH = 1,
            iTypeFormulaV = 0,
            iTypeCtrl = 2,
            iTypeDbField = 3,
            iTypeSecG = 4,
            iTypeSec = 5,
            iTypeSecLn = 6,
            iTypeText = 7
        }


        public fSearch()
        {
            InitializeComponent();
        }

        public void clear()
        {
            lv_controls.Items.Clear();
        }

        private void cmd_search_Click(object sender, EventArgs e)
        {
            if (tx_toSearch.Text.Trim() == "")
            {
                cWindow.msgInfo("You must input some text to search");
            }
            else 
            { 
                cReport report = m_editor.getReport();
                searchInSections(report.getHeaders(), csObjType.iTypeSec);
                searchInSections(report.getGroupsHeaders(), csObjType.iTypeSecG);
                searchInSections(report.getDetails(), csObjType.iTypeSec);
                searchInSections(report.getGroupsFooters(), csObjType.iTypeSecG);
                searchInSections(report.getFooters(), csObjType.iTypeSec);
            }
        }

        private void searchInSections(cIReportGroupSections sections, csObjType objType)
        { 
            cReportSection sec;
            cReportSectionLine secLn;
            cReportControl ctrl;
            string toSearch;
  
            toSearch = tx_toSearch.Text.ToLower();

            for (int i = 0; i < sections.count(); i++)
            {
                sec = sections.item(i);
                if (sec.getName().ToLower().IndexOf(toSearch) > -1)
                {
                    pAddToSearchResult(sec.getName(), objType, objType, "S" + sec.getKey());
                }
                if (sec.getFormulaHide().getText().ToLower().IndexOf(toSearch) > -1)
                {
                    pAddToSearchResult(sec.getName(), objType, csObjType.iTypeFormulaH, "S" + sec.getKey(), sec.getFormulaHide().getText());
                }
                for (int j = 0; j < sec.getSectionLines().count(); j++)
                {
                    secLn = sec.getSectionLines().item(j);
                    if (secLn.getFormulaHide().getText().ToLower().IndexOf(toSearch) > -1)
                    {
                        pAddToSearchResult(sec.getName() + " - Line " + secLn.getIndex().ToString(),
                            csObjType.iTypeSecLn, csObjType.iTypeFormulaH, "S" + sec.getKey(), secLn.getFormulaHide().getText());
                    }
                    for (int t = 0; t < secLn.getControls().count(); t++)
                    {
                        ctrl = secLn.getControls().item(t);
                        if (ctrl.getName().ToLower().IndexOf(toSearch) > -1)
                        {
                            pAddToSearchResult(ctrl.getName(), csObjType.iTypeCtrl, csObjType.iTypeCtrl, ctrl.getKey());
                        }
                        if (ctrl.getControlType() == csRptControlType.CSRPTCTFIELD
                            || ctrl.getControlType() == csRptControlType.CSRPTCTDBIMAGE)
                        {
                            if (ctrl.getField().getName().ToLower().IndexOf(toSearch) > -1)
                            {
                                pAddToSearchResult(ctrl.getName(), csObjType.iTypeCtrl, csObjType.iTypeDbField, ctrl.getKey(), ctrl.getField().getName());
                            }
                        }
                        else
                        {
                            if (ctrl.getLabel().getText().IndexOf(toSearch) > -1)
                            {
                                pAddToSearchResult(ctrl.getName(), csObjType.iTypeCtrl, csObjType.iTypeText, ctrl.getKey(), ctrl.getLabel().getText());
                            }
                        }
                        if (ctrl.getFormulaValue().getText().ToLower().IndexOf(toSearch) > -1)
                        {
                            pAddToSearchResult(ctrl.getName(), csObjType.iTypeCtrl, csObjType.iTypeFormulaV, ctrl.getKey(), ctrl.getFormulaValue().getText());
                        }
                        if (ctrl.getFormulaHide().getText().ToLower().IndexOf(toSearch) > -1)
                        {
                            pAddToSearchResult(ctrl.getName(), csObjType.iTypeCtrl, csObjType.iTypeFormulaH, ctrl.getKey(), ctrl.getFormulaHide().getText());
                        }
                    }
                }
            }
        }

        private void pAddToSearchResult(string name, csObjType objType, csObjType objType2, string key)
        {
            pAddToSearchResult(name, objType, objType2, key, "");
        }

        private void pAddToSearchResult(string name, csObjType objType, csObjType objType2, string key, string where)
        {
            var item = lv_controls.Items.Add(name);
            item.ImageIndex = objType == objType2 ? (int)objType : (int)objType2;
            item.SubItems.Add(where);
            item.Tag = key;
        }

        public void setHandler(cEditor editor)
        {
            m_editor = editor;
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

        private void lv_controls_KeyUp(object sender, KeyEventArgs e)
        {
            selectControl();
        }

        private void lv_controls_MouseClick(object sender, MouseEventArgs e)
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

    }
}
