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
            var report = m_editor.getReport();
            cGlobals.addCtrls(report, tv_controls, C_IMG_FOLDER, C_IMG_FORMULA, C_IMG_CONTROL, C_IMG_DATBASE_FIELD);
            lbTitle.Text = "Report definition: " + report.getName();
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
                    else if (infoType == "S" || infoType == "L")
                    {
                        m_editor.selectSection(info.Substring(1));
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
