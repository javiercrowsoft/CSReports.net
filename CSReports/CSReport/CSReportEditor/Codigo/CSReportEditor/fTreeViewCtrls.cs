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
    public partial class fTreeViewCtrls : Form
    {
        private cEditor m_editor;

        private const int C_IMG_FOLDER = 0;
        private const int C_IMG_FORMULA = 3;
        private const int C_IMG_CONTROL = 2;
        private const int C_IMG_DATBASE_FIELD = 1;

        public fTreeViewCtrls()
        {
            InitializeComponent();
        }

		public void clear ()
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
            bool bComplexF = false;;
  
            cReportSection sec;
            cReportSectionLine secLn;
            cReportControl ctrl;

            for(int i = 0; i < sections.count(); i++)
            {
                sec = sections.item(i);
                nodeSec = father.Nodes.Add(sec.getName());
                nodeSec.Tag = "S" + sec.getKey();
    
                if (sec.getFormulaHide().getText() != "") 
                {
                    if (sec.getFormulaHide().getText() == "0") 
                    {
                        text = "Hidden";
                        bComplexF = false;;
                    }
                    else 
                    {
                        text = "Visibility formula";
                        bComplexF = true;
                    }
                    item = nodeSec.Nodes.Add(text);
                    item.ImageIndex = C_IMG_FORMULA;
                    item.SelectedImageIndex = C_IMG_FORMULA;
                    if (! sec.getHasFormulaHide())
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
                    nodeSecLn.Tag = "S" + secLn.getKey();
      
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
                        if (! secLn.getHasFormulaHide())
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
                            if(ctrl.getFormulaHide().getText() == "0") 
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
    }
}
