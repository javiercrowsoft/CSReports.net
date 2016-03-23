using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using CSKernelClient;
using CSReportDll;
using CSReportGlobals;
using CSConnect;

namespace CSReportEditor
{
    public static class cGlobals
    {

        public const String C_MODULE = "CSReportEditor.cGlobals";

        public const String C_KEY_HEADER = "RH";
        public const String C_KEY_FOOTER = "RF";
        public const String C_KEY_DETAIL = "RD";
        public const String C_KEY_GROUPH = "GH";
        public const String C_KEY_GROUPF = "GF";

        public const String c_BTN_PRINT        = "PRINT";
        public const String c_BTN_PROPERTIES   = "PROPERTIES";
        public const String c_BTN_DB           = "DB";
        public const String c_BTN_SAVE         = "SAVE";
        public const String c_BTN_OPEN         = "OPEN";
        public const String c_BTN_TOOL         = "TOOL";
        public const String c_BTN_NEW          = "NEW";
        public const String c_BTN_PREV         = "PREV";

        public const String c_BTN_ALIGN_LEFT   = "ALIGN_LEFT";
        public const String c_BTN_ALIGN_CENTER = "ALIGN_CENTER";
        public const String c_BTN_ALIGN_RIGHT  = "ALIGN_RIGHT";

        public const string c_BTN_FONT_BOLD = "FONT_BOLD";
        public const string c_BTN_SEARCH = "SEARCH";

        public const String c_BTN_CTL_ALIGN_TOP        = "CTL_ALIGN_TOP";
        public const String c_BTN_CTL_ALIGN_BOTTOM     = "CTL_ALIGN_BOTTOM";
        public const String c_BTN_CTL_ALIGN_VERTICAL   = "CTL_ALIGN_VERTICAL";
        public const String c_BTN_CTL_ALIGN_HORIZONTAL = "CTL_ALIGN_HORIZONTAL";
        public const String c_BTN_CTL_ALIGN_LEFT       = "CTL_ALIGN_LEFT";
        public const String c_BTN_CTL_ALIGN_RIGHT      = "CTL_ALIGN_RIGHT";

        public const String c_BTN_CTL_WIDTH  = "CTL_WIDTH";
        public const String c_BTN_CTL_HEIGHT = "CTL_HEIGHT";

        public const String C_CONTROL_NAME = "Control";

        public const int C_TOTINRECENTLIST = 7;

        public const int C_HEIGHT_NEW_SECTION = 23;
        public const int C_HEIGHT_BAR_SECTION = 8;

        public const int C_NO_CHANGE = -32768;

        public const String C_MAIN_HEADER = "Main Header";
        public const String C_MAIN_DETAIL = "Detail";
        public const String C_MAIN_FOOTER = "Main Footer";

        public const String C_GROUP_LABEL = "Group";

		// TODO: refactor
		public const int ShiftMask = 1;

        private static cEditor m_editor = null;

        public static void setStatus()
        { 
        
        }

		public static bool showDbFields(ref string field, ref int fieldType, ref int index, cEditor editor)
		{
            fColumns fc = null;

            try {
                fc = new fColumns();

                fc.clearColumns();

                cReport report = editor.getReport();
                
                cReportConnect connect = report.getConnect();
                fc.fillColumns(connect.getDataSource(), connect.getColumns(), false);

                for (int _i = 0; _i < report.getConnectsAux().count(); _i++)
                {
                    connect = report.getConnectsAux().item(_i);
                    fc.fillColumns(connect.getDataSource(), connect.getColumns(), true);
                }

                fc.setField(field);
                fc.ShowDialog();

                if (fc.getOk())
                {
                    field = fc.getField();
                    fieldType = fc.getFieldType();
                    index = fc.getIndex();

                    return true;
                }
                else
                {
                    return false;
                }

            } catch (Exception ex) {
                cError.mngError(ex, "showDbFields", C_MODULE, "");
                return false;
            }
            finally {
                if (fc != null)
                {
                    fc.Close();
                }
            }      
		}

		public static void setEditAlignTextState(object length)
		{
            implementThisMessage("setEditAlignTextState", "(CSReportEditor cGlobals)");
		}

		public static void setEditAlignCtlState(bool b)
		{
            implementThisMessage("setEditAlignCtlState", "(CSReportEditor cGlobals)");
		}

		public static void setEditFontBoldValue(int bBold)
		{
            implementThisMessage("setEditFontBoldValue", "(CSReportEditor cGlobals)");
		}

		public static void setEditAlignValue(int align)
		{
            implementThisMessage("setEditAlignValue", "(CSReportEditor cGlobals)");
		}

		public static void setParametersAux(cConnect connect, cReportConnect rptConnect)
		{
            rptConnect.getColumns().clear();
            rptConnect.getParameters().clear();

            for (int i = 0; i < connect.getColumnsInfo().count(); i++)
            {
                CSConnect.cColumnInfo colInfo = connect.getColumnsInfo().item(i);
                CSReportDll.cColumnInfo rptColInfo = new CSReportDll.cColumnInfo();

                rptColInfo.setName(colInfo.getName());
                rptColInfo.setPosition(colInfo.getPosition());
                rptColInfo.setColumnType(colInfo.getColumnType());
                rptConnect.getColumns().add(rptColInfo, "");
            }

            for (int i = 0; i < connect.getParameters().count(); i++)
            {
                CSConnect.cParameter parameter = connect.getParameters().item(i);
                CSReportDll.cParameter rptParameter = new CSReportDll.cParameter();

                rptParameter.setName(parameter.getName());
                rptParameter.setPosition(parameter.getPosition());
                rptParameter.setColumnType(parameter.getColumnType());
                rptParameter.setValue(parameter.getValue());
                rptConnect.getParameters().add(rptParameter, "");
            }        
        }

        public static void moveGroup(cReportGroup group, cEditor editor)
        {
            throw new NotImplementedException();
        }

        public static string getDataSourceStr(string dataSource)
        {
            return "{" + dataSource + "}.";
        }

        internal static void createStandarSections(cReport report, Rectangle tr)
        {
            report.getHeaders().add(null, C_KEY_HEADER);
            report.getFooters().add(null, C_KEY_FOOTER);
            report.getDetails().add(null, C_KEY_DETAIL);

            // 
            // main header
            //
            cReportSection sec = report.getHeaders().item(C_KEY_HEADER);
            sec.setName("Main header");

            cReportAspect aspect = sec.getAspect();
            aspect.setTop(0);
            aspect.setHeight(tr.height * 0.25f);
            aspect.setWidth(tr.width);
            cReportSectionLine secLn = sec.getSectionLines().item(0);
            secLn.setSectionName("Main header");
            aspect = secLn.getAspect();
            aspect.setTop(0);
            aspect.setHeight(tr.height * 0.25f);
            aspect.setWidth(tr.width);

            // 
            // detail
            //
            sec = report.getDetails().item(C_KEY_DETAIL);
            sec.setName("Detail");

            aspect = sec.getAspect();
            aspect.setTop(tr.height * 0.25f);
            aspect.setHeight(tr.height * 0.25f);
            aspect.setWidth(tr.width);
            secLn = sec.getSectionLines().item(0);
            secLn.setSectionName("Detail");
            aspect = secLn.getAspect();
            aspect.setTop(tr.height * 0.25f);
            aspect.setHeight(tr.height * 0.25f);
            aspect.setWidth(tr.width);

            // 
            // main footer
            //
            sec = report.getFooters().item(C_KEY_FOOTER);
            sec.setName("Main footer");

            aspect = sec.getAspect();
            aspect.setTop(tr.height * 0.75f);
            aspect.setHeight(tr.height * 0.25f);
            aspect.setWidth(tr.width);
            secLn = sec.getSectionLines().item(0);
            secLn.setSectionName("Main footer");
            aspect = secLn.getAspect();
            aspect.setTop(tr.height * 0.75f);
            aspect.setHeight(tr.height * 0.25f);
            aspect.setWidth(tr.width);
        }

        internal static void clearCtrlBox(cEditor editor)
        {
            throw new NotImplementedException();
        }

        public static void implementThisMessage(string functionName, string moduleName) 
        {
            //Console.WriteLine(String.Format("Implement this: public static void {0} {1}", functionName, moduleName));
        }

        public static void addCtrls(cReport report, ListView lv_controls, int C_CTRL_IMAGE, int C_DB_IMAGE)
        {
            lv_controls.Items.Clear();

            for (int i = 0; i < report.getControls().count(); i++)
            {
                var ctrl = report.getControls().item(i);
                var ctrlName = ctrl.getName();
                var ctrlInfo = "";
                var ctrlField = "";

                switch (ctrl.getControlType())
                {
                    case csRptControlType.CSRPTCTFIELD:
                        ctrlField = ctrl.getField().getName();
                        break;
                    case csRptControlType.CSRPTCTDBIMAGE:
                        ctrlInfo = ctrl.getField().getName();
                        break;
                    case csRptControlType.CSRPTCTIMAGE:
                        ctrlInfo = " (Image)";
                        break;
                    case csRptControlType.CSRPTCTLABEL:
                        ctrlInfo = ctrl.getLabel().getText();
                        break;
                }

                if (ctrlInfo.Length > 0)
                {
                    ctrlName += " (" + ctrlInfo + ")";
                }

                var item = lv_controls.Items.Add(ctrlName, C_CTRL_IMAGE);
                item.Tag = ctrl.getKey();
                item.SubItems.Add("");
                item.SubItems.Add("");
                item.SubItems.Add("");

                if (ctrl.getHasFormulaValue()) item.SubItems[1].Text = "*";
                if (ctrl.getHasFormulaHide()) item.SubItems[2].Text = "*";

                if (ctrlField.Length > 0)
                {
                    item.SubItems[3].Text = ctrlField;
                    item.SubItems[3].ForeColor = Color.Blue;
                    item.ImageIndex = C_DB_IMAGE;
                }
                if (ctrl.getName().Length > 4 && ctrl.getName().Substring(0, 4) == "lnk_")
                {
                    item.ForeColor = Color.Red;
                }
            }
        }

        public static void addCtrls(
            cReport report, TreeView tv_controls,
            int C_IMG_FOLDER, int C_IMG_FORMULA, 
            int C_IMG_CONTROL, int C_IMG_DATBASE_FIELD)
        {
            tv_controls.Nodes.Clear();

            TreeNode nodeGroup;
            TreeNode nodeRoot = tv_controls.Nodes.Add(report.getName());
            nodeRoot.ImageIndex = C_IMG_FOLDER;

            nodeGroup = nodeRoot.Nodes.Add("Headers");
            nodeGroup.ImageIndex = C_IMG_FOLDER;
            pAddCtrlsAux(report.getHeaders(), nodeGroup, C_IMG_FOLDER, C_IMG_FORMULA, C_IMG_CONTROL, C_IMG_DATBASE_FIELD);

            nodeGroup = nodeRoot.Nodes.Add("Group Header");
            nodeGroup.ImageIndex = C_IMG_FOLDER;
            pAddCtrlsAux(report.getGroupsHeaders(), nodeGroup, C_IMG_FOLDER, C_IMG_FORMULA, C_IMG_CONTROL, C_IMG_DATBASE_FIELD);

            nodeGroup = nodeRoot.Nodes.Add("Details");
            nodeGroup.ImageIndex = C_IMG_FOLDER;
            pAddCtrlsAux(report.getDetails(), nodeGroup, C_IMG_FOLDER, C_IMG_FORMULA, C_IMG_CONTROL, C_IMG_DATBASE_FIELD);

            nodeGroup = nodeRoot.Nodes.Add("Group Footer");
            nodeGroup.ImageIndex = C_IMG_FOLDER;
            pAddCtrlsAux(report.getGroupsFooters(), nodeGroup, C_IMG_FOLDER, C_IMG_FORMULA, C_IMG_CONTROL, C_IMG_DATBASE_FIELD);

            nodeGroup = nodeRoot.Nodes.Add("Footers");
            nodeGroup.ImageIndex = C_IMG_FOLDER;
            pAddCtrlsAux(report.getFooters(), nodeGroup, C_IMG_FOLDER, C_IMG_FORMULA, C_IMG_CONTROL, C_IMG_DATBASE_FIELD);
            
            nodeRoot.ExpandAll();
        }

        private static void pAddCtrlsAux(
            cIReportGroupSections sections, TreeNode father,
            int C_IMG_FOLDER, int C_IMG_FORMULA, int C_IMG_CONTROL, int C_IMG_DATBASE_FIELD)
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
                        nodeCtrl = nodeSecLn.Nodes.Add(
                            ctrl.getName() 
                            + (ctrl.getLabel().getText() != "" 
                                ? " - " + ctrl.getLabel().getText() 
                                : "")
                            );
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

        public static void fillColumns(
            string dataSource, CSReportDll.cColumnsInfo columns, ListView lv_columns,
            string C_INDEX, string C_FIELDTYPE, bool add)
        {
            if (!add) lv_columns.Items.Clear();

            foreach (CSReportDll.cColumnInfo column in columns)
            {
                var item = lv_columns.Items.Add(String.Format("{{{0}}}.{1}", dataSource, column.getName()));
                item.ImageIndex = 0;
                string info = cUtil.setInfoString("", C_INDEX, column.getPosition().ToString());
                info = cUtil.setInfoString(info, C_FIELDTYPE, column.getColumnType().ToString());
                item.Tag = info;
            }
        }
    }

    public class Rectangle
    {
        public long height;
        public long width;

        public Rectangle(RectangleF rect)
        {
            height = (long)rect.Height;
            width = (long)rect.Width;
        }
    }

    public interface cIDatabaseFieldSelector 
    {
        int getFieldType();
        void setFieldType(int rhs);
        int getIndex();
        void setIndex(int rhs);
        System.Windows.Forms.TextBox txDbField { get; }
    }

    public enum csRptEditorMoveType {
        CSRPTEDMOVTHORIZONTAL,
        CSRPTEDMOVTVERTICAL,
        CSRPTEDMOVTALL,
        CSRPTEDMOVLEFT,
        CSRPTEDMOVRIGHT,
        CSRPTEDMOVUP,
        CSRPTEDMOVDOWN,
        CSRPTEDMOVLEFTDOWN,
        CSRPTEDMOVLEFTUP,
        CSRPTEDMOVRIGHTDOWN,
        CSRPTEDMOVRIGHTUP,
        CSRPTEDMOVTNONE
    }

    public enum csRptEditCtrlType {
        none,
        label,
        field,
        formula,
        image,
        chart,
        lineLabel
    }
}
