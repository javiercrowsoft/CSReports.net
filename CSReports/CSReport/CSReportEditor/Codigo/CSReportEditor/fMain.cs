using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using CSKernelClient;
using CSReportGlobals;
using CSReportDll;

namespace CSReportEditor
{
    public partial class fMain : Form
    {
        // TODO: remove me
        //static fMain instance;

        private const String C_MODULE = "fMain";

        private const string MRU_FILE = "mru.settings";

        private int m_paperSize = 0;
        private int m_paperSizeWidth = 0;
        private int m_paperSizeHeight = 0;
        private int m_orientation = 0;
        private string m_printerName = "";
        private string m_driverName = "";
        private string m_port = "";

        private bool m_wasDoubleClick = false;

        private const int C_CTRL_IMAGE = 1;
        private const int C_DB_IMAGE = 0;

        private const int C_IMG_FOLDER = 0;
        private const int C_IMG_FORMULA = 3;
        private const int C_IMG_CONTROL = 2;
        private const int C_IMG_DATBASE_FIELD = 1;

        private const String C_FIELDTYPE = "t";
        private const String C_INDEX = "i";

        private cEditor m_contextMenuEditor;

        private cListViewColumnSorter lvwColumnSorter;

        public fMain()
        {
            InitializeComponent();

            // it is the first thing we need to do
            //
            CSKernelClient.cUtil.setSepDecimal();

            cPrinter printer = cPrintAPI.getcPrinterFromDefaultPrinter();
            m_paperSize = (int)printer.getPaperInfo().getPaperSize();
            m_paperSizeHeight = Convert.ToInt32(printer.getPaperInfo().getHeight());
            m_paperSizeWidth = Convert.ToInt32(printer.getPaperInfo().getHeight());
        }

        public void init()
        {
            cEditor editor = new cEditor(this, pnEditor, pnRule, pnReport, tbpEditor);
            editor.init();
            editor.newReport(null);        
        }

        public cEditor getReportCopySource()
        {
            return null;
        }

        private cEditor createEditor() 
        {
            TabPage tab = new TabPage();
            Panel pnEditor = new Panel();
            PictureBox pnRule = new PictureBox();
            PictureBox pnReport = new PictureBox();

            pnEditor.Controls.Add(pnRule);
            pnEditor.Controls.Add(pnReport);
            tab.Controls.Add(pnEditor);
            pnEditor.Dock = DockStyle.Fill;
            tabReports.TabPages.Add(tab);
            tab.Text = "New Report";

            return new cEditor(this, pnEditor, pnRule, pnReport, tab);
        }

        private void mnuNewReport_Click(object sender, EventArgs e)
        {
            createEditor();
            cEditor editor = createEditor();
            editor.init();
        }

        private void tsbNew_Click(object sender, EventArgs e)
        {
            mnuNewReport_Click(sender, e);
        }

        public void setEditAlignTextState(bool status)
        {
            var buttons = this.tbMain.Items;

            buttons[cGlobals.c_BTN_ALIGN_CENTER].Enabled = status;
            buttons[cGlobals.c_BTN_ALIGN_LEFT].Enabled = status;
            buttons[cGlobals.c_BTN_ALIGN_RIGHT].Enabled = status;
            buttons[cGlobals.c_BTN_FONT_BOLD].Enabled = status;
        }

        public void setEditAlignCtlState(bool status) 
        {
            var buttons = this.tbMain.Items;

            buttons[cGlobals.c_BTN_CTL_ALIGN_BOTTOM].Enabled = status;
            buttons[cGlobals.c_BTN_CTL_ALIGN_TOP].Enabled = status;

            buttons[cGlobals.c_BTN_CTL_ALIGN_VERTICAL].Enabled = status;
            buttons[cGlobals.c_BTN_CTL_ALIGN_HORIZONTAL].Enabled = status;
            buttons[cGlobals.c_BTN_CTL_ALIGN_LEFT].Enabled = status;
            buttons[cGlobals.c_BTN_CTL_ALIGN_RIGHT].Enabled = status;

            buttons[cGlobals.c_BTN_CTL_HEIGHT].Enabled = status;
            buttons[cGlobals.c_BTN_CTL_WIDTH].Enabled = status;
        }

        public void setMenuAux(bool enabled)
        {
            this.mnuEditAddControl.Enabled = enabled;
            this.mnuEditAddHeader.Enabled = enabled;
            this.mnuEditAddLabel.Enabled = enabled;
            this.mnuEditAddGroup.Enabled = enabled;
            this.mnuEditAddFooter.Enabled = enabled;
            this.mnuEditAddLine.Enabled = enabled;
            this.mnuEditAddSec.Enabled = enabled;
            this.mnuEditMove.Enabled = enabled;
            this.mnuDataBaseEditDataSource.Enabled = enabled;
            this.mnuPreviewReport.Enabled = enabled;
            this.mnuPrintReport.Enabled = enabled;
            this.mnuSaveReport.Enabled = enabled;
            this.mnuReportSaveAs.Enabled = enabled;
            this.mnuDataBaseSetDisconnected.Enabled = enabled;
            this.mnuEditSearch.Enabled = enabled;
            this.mnuDataBaseSQLServerConnection.Enabled = enabled;
            this.mnuDataBaseSetToMainConnect.Enabled = enabled;
            this.mnuDataBaseEditDataSource.Enabled = enabled;
            this.mnuDataBaseConnectsAuxCfg.Enabled = enabled;
            this.mnuViewGridMain.Enabled = enabled;
            this.mnuViewToolbar.Enabled = enabled;
            this.mnuViewControls.Enabled = enabled;
            this.mnuViewTreeViewCtrls.Enabled = enabled;

            var buttons = this.tbMain.Items;
            tsbPrint.Enabled = enabled;
            tsbProperties.Enabled = enabled;
            tsbDatabase.Enabled = enabled;
            tsbSave.Enabled = enabled;
            tsbControls.Enabled = enabled;
            tsbPreview.Enabled = enabled;
            tsbSearch.Enabled = enabled;
        }

        public void addToRecentList(String fileName)
        {
            int i = 0;
            int j = 0;
            bool found = false;
            var menuItems = this.mnuFileRecentList.DropDownItems;

            for (i = 0; i < menuItems.Count; i++)
            {
                if (fileName == menuItems[i].Text)
                {
                    j = i;
                    found = true;
                    break;
                }
            }

            if (menuItems.Count < cGlobals.C_TOTINRECENTLIST && !found)
            {
                var menu = this.mnuFileRecentList.DropDownItems.Add("");
                menu.Visible = true;
                menu.Click += mnuRecentClick;
            }

            if (!found) { j = menuItems.Count - 1; }

            for (i = j; i > 0; i--)
            {
                menuItems[i].Text = menuItems[i - 1].Text;
            }

            menuItems[0].Text = fileName;

            saveRecentList();
        }

        private string getMRUFileName()
        { 
            var path = System.Environment.SpecialFolder.LocalApplicationData;
            return Environment.GetFolderPath(path) + Path.DirectorySeparatorChar + MRU_FILE;
        }

        private void loadRecentListFromUserSettings()
        {
            var fileName = getMRUFileName();
            if (File.Exists(fileName))
            {
                var lines = File.ReadAllLines(fileName);
                loadRecentList(lines.ToList());
            }
        }

        private void loadRecentList(List<String> recentList)
        {
            int i = 0;
            String recent = "";

            for (i = 0; i < Math.Min(cGlobals.C_TOTINRECENTLIST, recentList.Count); i++)
            {
                recent = recentList[i];
                var menu = this.mnuFileRecentList.DropDownItems.Add(recent);
                menu.Visible = true;
                menu.Click += mnuRecentClick;
            }

            if (this.mnuFileRecentList.DropDownItems.Count > 0)
            {
                this.mnuFileRecentList.Visible = true;
            }
        }

        private void mnuRecentClick(object sender, EventArgs e)
        {
            ToolStripMenuItem mnu = (ToolStripMenuItem)sender;
            cEditor editor = createEditor();
            editor.init();
            if (editor.openDocument(mnu.Text))
            {
                addToRecentList(editor.getFileName());
            }
        }

        private void saveRecentList()
        {
            int i = 0;
            string mruList = "";

            for (i = 0; i < mnuFileRecentList.DropDownItems.Count; i++)
            {
                mruList += mnuFileRecentList.DropDownItems[i].Text + Environment.NewLine;
            }

            var fileName = getMRUFileName();
            File.WriteAllText(fileName, mruList);
        }

        public void setStatus(String status)
        {
            // TODO: implement
        }

        public void setBarText(String text)
        {
            // TODO: implement
        }

        public void setDisconnectedReport(bool isDisconnectedReport)
        {
            // TODO: implement
        }

		public void setsbPnlCtrl (string msg)
		{
            cGlobals.implementThisMessage("setsbPnlCtrl", "(fMain)");
		}

        internal void setReportCopySource(cEditor cEditor)
        {
            throw new NotImplementedException();
        }

        internal CSReportGlobals.csReportPaperType getPaperSize()
        {
            return (CSReportGlobals.csReportPaperType)m_paperSize;
        }

        internal int getOrientation()
        {
            return m_orientation;
        }

        private void mnuOpenReport_Click(object sender, EventArgs e)
        {
            try {
                
                cEditor editor = createEditor();

                editor.init();

                if (editor.openDocument())
                {
                    addToRecentList(editor.getFileName());
                }

            } catch (Exception ex) {
                cError.mngError(ex, "mnuOpenReport_Click", C_MODULE, "");
            }
        }

        private void tsbOpen_Click(object sender, EventArgs e)
        {
            mnuOpenReport_Click(sender, e);
        }

        //------------------------------------------------------------------------------------------------------------------

        // expose controls

        //------------------------------------------------------------------------------------------------------------------

        public OpenFileDialog openFileDialog 
        {
            get
            {
                return openFileDlg;
            }
        }

        public SaveFileDialog saveFileDialog
        {
            get
            {
                return saveFielDlg;
            }            
        }

        private void fMain_Load(object sender, EventArgs e)
        {
            cPrintAPI.getDefaultPrinter(
                out m_printerName, out m_driverName, out m_port, 
                out m_paperSize, out m_orientation, out m_paperSizeWidth, 
                out m_paperSizeHeight);
            
            //
            // remove me and implement a better window position code
            //
            this.Width = 1200;
            this.Height = 900;
            cWindow.centerForm(this);

            // Create an instance of a ListView column sorter and assign it 
            // to the ListView control.
            lvwColumnSorter = new cListViewColumnSorter();
            lv_controls.ListViewItemSorter = lvwColumnSorter;
            lv_controls_ColumnClick(this, new ColumnClickEventArgs(0));

            loadRecentListFromUserSettings();
        }

        private void cmCtrlProperties_Click(object sender, EventArgs e)
        {
            if (m_contextMenuEditor != null) 
            {
                m_contextMenuEditor.showProperties();
            }
        }

        public void showPopMenuSection(cEditor editor, bool noDelete, bool showGroups, Point p)
        {
            cmSectionDeleteSection.Enabled = !noDelete;
            cmSectionGroupProperties.Visible = showGroups;
            cmSectionMoveGroup.Visible = showGroups;
            cmSectionGroupSeparator.Visible = showGroups;

            m_contextMenuEditor = editor;

            cmnSection.Show(p);
        }

        public void showPopMenuControl(cEditor editor, bool clickInCtrl, bool pasteEnabled, Point p)
        {
            cmCtrlCopy.Enabled = clickInCtrl;
            cmCtrlCut.Enabled = clickInCtrl;
            cmCtrlDelete.Enabled = clickInCtrl;
            cmCtrlEditText.Enabled = clickInCtrl;
            cmCtrlSendBack.Enabled = clickInCtrl;
            cmCtrlBringFront.Enabled = clickInCtrl;
            cmCtrlProperties.Enabled = clickInCtrl;

            cmCtrlPaste.Enabled = pasteEnabled;
            cmCtrlPasteEx.Enabled = pasteEnabled;

            m_contextMenuEditor = editor;

            cmnControl.Show(p);
        }

        private void cmSectionSectionProperties_Click(object sender, EventArgs e)
        {
            if (m_contextMenuEditor != null)
            {
                m_contextMenuEditor.showProperties();
            }
        }

        private void cmSectionSectionLineProperties_Click(object sender, EventArgs e)
        {
            if (m_contextMenuEditor != null)
            {
                m_contextMenuEditor.showSecLnProperties();
            }
        }

        private void cmSectionGroupProperties_Click(object sender, EventArgs e)
        {
            if (m_contextMenuEditor != null)
            {
                m_contextMenuEditor.showGroupProperties();
            }
        }

        private void mnuViewTreeViewCtrls_Click(object sender, EventArgs e)
        {
            cEditor editor = cMainEditor.getDocActive();
            if (editor != null)
            {
                editor.showControlsTree();
            }
        }

        private void mnuViewControls_Click(object sender, EventArgs e)
        {
            cEditor editor = cMainEditor.getDocActive();
            if (editor != null)
            {
                editor.showControls();
            }
        }

        private void mnuViewToolbar_Click(object sender, EventArgs e)
        {
            showToolbox();
        }

        private void tsbControls_Click(object sender, EventArgs e)
        {
            showToolbox();
        }

        private void showToolbox()
        {
            cEditor editor = cMainEditor.getDocActive();
            if (editor != null)
            {
                editor.showToolbox();
            }
        }

        private void tsbSearch_Click(object sender, EventArgs e)
        {
            search();
        }

        private void mnuEditSearch_Click(object sender, EventArgs e)
        {
            search();
        }

        private void search() 
        {
            cEditor editor = cMainEditor.getDocActive();
            if (editor != null)
            {
                editor.search();
            }
        }

        private void tsbProperties_Click(object sender, EventArgs e)
        {
            cEditor editor = cMainEditor.getDocActive();
            if (editor != null)
            {
                editor.showProperties();
            }
        }

        private void mnuDataBaseSQLServerConnection_Click(object sender, EventArgs e)
        {
            cEditor editor = cMainEditor.getDocActive();
            if (editor != null)
            {
                editor.setSimpleConnection();
            }
        }

        private void mnuDataBaseConnectConfig_Click(object sender, EventArgs e)
        {
            cEditor editor = cMainEditor.getDocActive();
            if (editor != null)
            {
                editor.editConnectionString();
            }
        }

        private void mnuDataBaseEditDataSource_Click(object sender, EventArgs e)
        {
            cEditor editor = cMainEditor.getDocActive();
            if (editor != null)
            {
                editor.editDataSource();
            }
        }

        private void mnuExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void mnuParametersSettings_Click(object sender, EventArgs e)
        {
            cEditor editor = cMainEditor.getDocActive();
            if (editor != null)
            {
                editor.setParameters();
            }
        }

        public void showControls(cEditor editor)
        {
            cGlobals.addCtrls(editor.getReport(), lv_controls, C_CTRL_IMAGE, C_DB_IMAGE);
        }

        public void showControlsTree(cEditor editor)
        {
            m_wasDoubleClick = false;
            cGlobals.addCtrls(editor.getReport(), tv_controls, C_IMG_FOLDER, C_IMG_FORMULA, C_IMG_CONTROL, C_IMG_DATBASE_FIELD);
        }

        public void showProperties(cEditor editor, string key)
        {
            lv_properties.Items.Clear();
            if (editor != null)
            {
                setObjectDescription(getControlOrSection(editor, key));
            }
        }

        private object getControlOrSection(cEditor editor, string key)
        {
            if (key.Length > 1)
            {
                if (key.Substring(0, 1) == "S")
                {
                    return editor.getSectionOrSectionLineFromKey(key.Substring(1));
                }
                else 
                {
                    return editor.getReport().getControls().item(key);
                }
            }
            else 
            {
                return null;
            }
        }

        private void setObjectDescription(object anObject)
        {
            setObjectDescription(anObject, 0);
        }

        private void setObjectDescription(object anObject, int n)
        {
            if (anObject == null) return;

            var tabs = new String(' ', n*2);
            var methods = getMethods(anObject);
            foreach (var m in methods)
            {
                if (m.IsPublic
                    && m.Name.Length > 3
                    && m.Name.Substring(0, 3) == "get"
                    && m.Name.Substring(0, 4) != "get_"
                    && m.GetParameters().Length == 0
                    && m.Name != "getSectionLine"
                    )
                {
                    var item = lv_properties.Items.Add(tabs + m.Name.Substring(3));
                    item.ImageIndex = C_IMG_CONTROL;
                    item.SubItems.Add(getValue(m.Invoke(anObject, null), n));
                    if (item.SubItems[1].Text == "...") item.ImageIndex = C_IMG_FOLDER;
                }
            }
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
                    setObjectDescription(value, n + 1);
                    return "...";
                }
            }
        }

        private static MethodInfo[] getMethods(object obj)
        {
            return obj.GetType().GetMethods();
        }

        public void showFields(cEditor editor)
        {
            var connect = editor.getReport().getConnect();
            cGlobals.fillColumns(
                connect.getDataSource(), 
                connect.getColumns(), lv_fields, C_INDEX, C_FIELDTYPE);
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
            cEditor editor = cMainEditor.getDocActive();

            if (lv_controls.SelectedItems.Count > 0 && editor != null)
            {
                var info = lv_controls.SelectedItems[0].Tag.ToString();
                editor.selectCtrl(info);
            }
        }

        private void tv_controls_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            selectControl(e.Node);
        }

        private void selectControl(TreeNode node)
        {
            cEditor editor = cMainEditor.getDocActive();

            if (node != null && node.Tag != null && editor != null)
            {
                var info = node.Tag.ToString();
                if (info.Length > 0)
                {
                    var infoType = info.Substring(0, 1);
                    if ("@SL".IndexOf(infoType) == -1)
                    {
                        editor.selectCtrl(info);
                    }
                    else if (infoType == "S" || infoType == "L")
                    {
                        editor.selectSection(info.Substring(1));
                    }
                }
            }
        }

        private void tv_controls_KeyUp(object sender, KeyEventArgs e)
        {
            selectControl(tv_controls.SelectedNode);
        }

        private void tv_controls_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            cEditor editor = cMainEditor.getDocActive();

            if (tv_controls.SelectedNode != null && editor != null)
            {
                if (tv_controls.SelectedNode.Tag != null)
                {
                    var info = tv_controls.SelectedNode.Tag.ToString();
                    if (info.Length > 0)
                    {
                        var infoType = info.Substring(0, 1);
                        if ("@".IndexOf(infoType) == -1)
                        {
                            editor.showProperties(info);
                        }
                    }
                }
            }
        }

        private void tv_controls_MouseDown(object sender, MouseEventArgs e)
        {
            m_wasDoubleClick = e.Clicks > 1;
        }

        private void tv_controls_BeforeCollapse(object sender, TreeViewCancelEventArgs e)
        {
            if (m_wasDoubleClick == true && e.Action == TreeViewAction.Collapse)
                e.Cancel = true;
        }

        private void tv_controls_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            if (m_wasDoubleClick == true && e.Action == TreeViewAction.Expand)
                e.Cancel = true;
        }

        private void lv_controls_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            cEditor editor = cMainEditor.getDocActive();

            if (lv_controls.SelectedItems.Count > 0 && editor != null)
            {
                var info = lv_controls.SelectedItems[0].Tag.ToString();
                editor.showProperties(info);
            }
        }

        private void mnuPreviewReport_Click(object sender, EventArgs e)
        {
            previewReport();
        }

        private void tsbPreview_Click(object sender, EventArgs e)
        {
            previewReport();
        }

        private void previewReport()
        {
            cEditor editor = cMainEditor.getDocActive();
            if (editor != null)
            {
                editor.preview();
            }        
        }


        private void tabReports_MouseClick(object sender, MouseEventArgs e)
        {
            for (int i = 0; i < tabReports.TabCount; ++i)
            {
                var rect = tabReports.GetTabRect(i);
                var xRect = new System.Drawing.Rectangle(rect.Left + rect.Width - 18, rect.Top, 18, rect.Height);
                if (xRect.Contains(e.Location))
                {
                    cEditor editor = (cEditor)tabReports.TabPages[i].Tag;
                    if (editor.close())
                    {
                        tabReports.TabPages.RemoveAt(i);
                    }
                }
            }
        }

        private void mnuEditAddHeader_Click(object sender, EventArgs e)
        {
            cEditor editor = cMainEditor.getDocActive();
            if (editor != null)
            {
                editor.addSection(csRptSectionType.HEADER);
            }
        }

        private void mnuEditAddGroup_Click(object sender, EventArgs e)
        {
            cEditor editor = cMainEditor.getDocActive();
            if (editor != null)
            {
                editor.addGroup();
            }
        }

        private void mnuEditAddFooter_Click(object sender, EventArgs e)
        {
            cEditor editor = cMainEditor.getDocActive();
            if (editor != null)
            {
                editor.addSection(csRptSectionType.FOOTER);
            }
        }

        private void mnuEditAddLabel_Click(object sender, EventArgs e)
        {
            cEditor editor = cMainEditor.getDocActive();
            if (editor != null)
            {
                editor.addLabel();
            }
        }

        private void mnuEditAddLine_Click(object sender, EventArgs e)
        {
            cEditor editor = cMainEditor.getDocActive();
            if (editor != null)
            {
                editor.addLabel();
            }
        }

        private void mnuEditAddControl_Click(object sender, EventArgs e)
        {
            cEditor editor = cMainEditor.getDocActive();
            if (editor != null)
            {
                editor.addDBField();
            }
        }

        private void mnuEditAddImage_Click(object sender, EventArgs e)
        {
            cEditor editor = cMainEditor.getDocActive();
            if (editor != null)
            {
                editor.addImage();
            }
        }

        private void mnuEditAddChart_Click(object sender, EventArgs e)
        {
            cEditor editor = cMainEditor.getDocActive();
            if (editor != null)
            {
                editor.addChart();
            }
        }

        private void mnuHelpAbout_Click(object sender, EventArgs e)
        {
            cWindow.msgInfo(
                System.Reflection.Assembly.GetExecutingAssembly().GetName().Name 
                + " - Version " 
                + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version
                + "\r\n\r\nhttps://github.com/javiercrowsoft/CSReports.net");
        }
    }
}
