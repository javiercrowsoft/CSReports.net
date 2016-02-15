using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using CSKernelClient;
using CSReportGlobals;
using CSReportDll;

namespace CSReportEditor
{
    public partial class fTreeViewCtrls : Form
    {
        private cEditor m_editor;

        private const int C_IMG_FOLDER = 0;
        private const int C_IMG_FORMULA = 3;
        private const int C_IMG_CONTROL = 2;
        private const int C_IMG_DATBASE_FIELD = 1;

        private String m_formulaName = "";

        public fTreeViewCtrls()
        {
            InitializeComponent();
        }

        public string getFormulaName()
        {
            return m_formulaName;
        }

        public void clear()
        {
            tv_controls.Nodes.Clear();
        }

        public void addCtrls()
        {
            TreeNode nodeGroup;
            cReport report = m_editor.getReport();
            TreeNode nodeRoot = tv_controls.Nodes.Add(report.getName());
            nodeRoot.ImageIndex = C_IMG_FOLDER;

            nodeGroup = nodeRoot.Nodes.Add("Headers");
            nodeGroup.ImageIndex = C_IMG_FOLDER;
            pAddCtrlsAux(report.getHeaders(), nodeGroup);

            nodeGroup = nodeRoot.Nodes.Add("Group Header");
            nodeGroup.ImageIndex = C_IMG_FOLDER;
            pAddCtrlsAux(report.getGroupsHeaders(), nodeGroup);

            nodeGroup = nodeRoot.Nodes.Add("Details");
            nodeGroup.ImageIndex = C_IMG_FOLDER;
            pAddCtrlsAux(report.getDetails(), nodeGroup);

            nodeGroup = nodeRoot.Nodes.Add("Group Footer");
            nodeGroup.ImageIndex = C_IMG_FOLDER;
            pAddCtrlsAux(report.getGroupsFooters(), nodeGroup);

            nodeGroup = nodeRoot.Nodes.Add("Footers");
            nodeGroup.ImageIndex = C_IMG_FOLDER;
            pAddCtrlsAux(report.getFooters(), nodeGroup);

            lbTitle.Text = "Report definition: " + report.getName();

            nodeRoot.ExpandAll();
        }

        private void pAddCtrlsAux(cIReportGroupSections sections, TreeNode father)
        {
            TreeNode nodeSec;
            TreeNode nodeSecLn;
            TreeNode nodeCtrl;
            TreeNode item;
            string text;
            bool bComplexF = false; ;

            cReportSection sec;
            cReportSectionLine secLn;
            cReportControl ctrl;

            for (int i = 0; i < sections.count(); i++)
            {
                sec = sections.item(i);
                nodeSec = father.Nodes.Add(sec.getName());
                nodeSec.Tag = "S" + sec.getKey();

                if (sec.getFormulaHide().getText() != "")
                {
                    if (sec.getFormulaHide().getText() == "0")
                    {
                        text = "Hidden";
                        bComplexF = false; ;
                    }
                    else
                    {
                        text = "Visibility formula";
                        bComplexF = true;
                    }
                    item = nodeSec.Nodes.Add(text);
                    item.ImageIndex = C_IMG_FORMULA;
                    item.SelectedImageIndex = C_IMG_FORMULA;
                    if (!sec.getHasFormulaHide())
                    {
                        item.ForeColor = Color.Red;
                    }

                    if (bComplexF)
                    {
                        item.Tag = "@FH=" + sec.getFormulaHide().getText();
                    }
                }

                for (int j = 0; j < sec.getSectionLines().count(); j++)
                {
                    secLn = sec.getSectionLines().item(j);
                    nodeSecLn = nodeSec.Nodes.Add("Line " + secLn.getIndex());
                    nodeSecLn.ImageIndex = C_IMG_FOLDER;
                    nodeSecLn.Tag = "L" + secLn.getKey();

                    if (secLn.getFormulaHide().getText() != "")
                    {
                        if (secLn.getFormulaHide().getText() == "0")
                        {
                            text = "Hidden";
                            bComplexF = false;
                        }
                        else
                        {
                            text = "Visibility formula";
                            bComplexF = true;
                        }
                        item = nodeSecLn.Nodes.Add(text);
                        item.ImageIndex = C_IMG_FORMULA;
                        item.SelectedImageIndex = C_IMG_FORMULA;
                        if (!secLn.getHasFormulaHide())
                        {
                            item.ForeColor = Color.Red;
                        }
                        if (bComplexF)
                        {
                            item.Tag = "@FH=" + secLn.getFormulaHide().getText();
                        }
                    }
                    for (int t = 0; t < secLn.getControls().count(); t++)
                    {
                        ctrl = secLn.getControls().item(t);
                        nodeCtrl = nodeSecLn.Nodes.Add(ctrl.getName());
                        nodeCtrl.ImageIndex = C_IMG_CONTROL;
                        nodeCtrl.SelectedImageIndex = C_IMG_CONTROL;
                        nodeCtrl.Tag = ctrl.getKey();
                        nodeCtrl.BackColor = cColor.colorFromRGB(ctrl.getLabel().getAspect().getBackColor());
                        nodeCtrl.ForeColor = cColor.colorFromRGB(ctrl.getLabel().getAspect().getFont().getForeColor());

                        if (ctrl.getControlType() == csRptControlType.CSRPTCTFIELD)
                        {
                            item = nodeCtrl.Nodes.Add(ctrl.getField().getName());
                            item.ImageIndex = C_IMG_DATBASE_FIELD;
                            item.SelectedImageIndex = C_IMG_DATBASE_FIELD;
                        }

                        if (ctrl.getFormulaHide().getText() != "")
                        {
                            if (ctrl.getFormulaHide().getText() == "0")
                            {
                                text = "hidden";
                                bComplexF = false;
                            }
                            else
                            {
                                text = "Visibility formula";
                                bComplexF = true;
                            }

                            item = nodeCtrl.Nodes.Add(text);
                            item.ImageIndex = C_IMG_FORMULA;
                            item.SelectedImageIndex = C_IMG_FORMULA;
                            if (!ctrl.getHasFormulaHide())
                            {
                                item.ForeColor = Color.Red;
                            }
                            if (bComplexF)
                            {
                                item.Tag = "@FH=" + ctrl.getFormulaHide().getText();
                            }
                        }

                        if (ctrl.getFormulaValue().getText() != "")
                        {
                            item = nodeCtrl.Nodes.Add("Value formula");
                            item.ImageIndex = C_IMG_FORMULA;
                            item.SelectedImageIndex = C_IMG_FORMULA;
                            if (!ctrl.getHasFormulaValue())
                            {
                                item.ForeColor = Color.Red;
                            }
                            item.Tag = "@FV=" + ctrl.getFormulaValue().getText();
                        }
                    }
                }
            }
            father.ExpandAll();
        }

        public void setHandler(cEditor editor)
        {
            m_editor = editor;
        }

        private void fTreeViewCtrls_Load(object sender, EventArgs e)
        {
            cWindow.locateFormAtLeft(this);
        }

        private void tv_formulas_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            selectAndShowInfo(e.Node); 
        }

        private void selectAndShowInfo(TreeNode node)
        {
            if (node != null && node.Tag != null)
            {
                var info = node.Tag.ToString();
                if (info.Length > 0)
                {
                    var infoType = info.Substring(0, 1);
                    if (infoType == "@")
                    {
                        tx_descrip.Text = info.Substring(4);
                    }
                    else if (infoType == "S")
                    {

                    }
                    else if (infoType == "L")
                    {

                    }
                    else
                    {
                        tx_descrip.Text = getObjectDescription(getControl(info));
                        m_editor.selectCtrl(info);
                    }
                }
            }        
        }

        private void tv_formulas_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.Tag != null)
            {
                var info = e.Node.Tag.ToString();
                if (info.Length > 0)
                {
                    var infoType = info.Substring(0, 4);
                    if (infoType == "@FH=")
                    {
                        m_formulaName = "Hide";
                        string formula = info.Substring(4);
                        if (m_editor.showEditFormula(ref formula))
                        {
                            e.Node.Tag = "@FH=" + formula;
                        }
                    }
                    else if (infoType == "@FV=")
                    {
                        m_formulaName = "Value";
                        string formula = info.Substring(4);
                        if (m_editor.showEditFormula(ref formula))
                        {
                            e.Node.Tag = "@FV=" + formula;
                        }
                    }
                }
            }
        }

        private void tv_controls_KeyUp(object sender, KeyEventArgs e)
        {
            selectAndShowInfo(tv_controls.SelectedNode);
        }

        // Get property array
        private cReportControl getControl(string key)
        {
            return m_editor.getReport().getControls().item(key);
        }

        private string getObjectDescription(object anObject)
        {
            return getObjectDescription(anObject, 0);
        }

        private string getObjectDescription(object anObject, int n)
        {
            var descrip = "";
            var tabs = new String('\t', n);
            var methods = getMethods(anObject);
            foreach (var m in methods)
            {
                if (m.IsPublic 
                    && m.Name.Length > 3
                    && m.Name.Substring(0,3) == "get"
                    && m.Name.Substring(0, 4) != "get_"
                    && m.GetParameters().Length == 0
                    && m.Name != "getSectionLine"
                    )
                {
                    descrip += tabs + m.Name.Substring(3) + ": " + getValue(m.Invoke(anObject, null), n) + "\r\n";
                }
            }

            return descrip;
        }

        private string getValue(object value, int n)
        {
            if (n > 10) return "";

            if (value == null)
            {
                return "NULL";
            }
            else
            {
                var t = value.GetType();
                if (t.IsPrimitive || t == typeof(Decimal) || t == typeof(String))
                {
                    return value.ToString();
                }
                else
                {
                    return "\r\n" + getObjectDescription(value, n + 1);
                }
            }
        }

        private static MethodInfo[] getMethods(object obj)
        {
            return obj.GetType().GetMethods();
        }

        private void cmd_edit_Click(object sender, EventArgs e)
        {
            if (tv_controls.SelectedNode != null)
            {
                if (tv_controls.SelectedNode.Tag != null)
                {
                    var info = tv_controls.SelectedNode.Tag.ToString();
                    if (info.Length > 0)
                    {
                        var infoType = info.Substring(0, 1);
                        if (infoType == "@")
                        {
                            tx_descrip.Text = info.Substring(4);
                        }
                        else if (infoType == "S")
                        {

                        }
                        else if (infoType == "L")
                        {

                        }
                        else
                        {
                            m_editor.showProperties(info);
                        }
                    }
                }
            }            
        }

        private void cmd_close_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
