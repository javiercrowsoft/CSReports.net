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
    public partial class fMain : Form
    {
        static fMain instance;

        private const String C_MODULE = "fMain";

        private int m_paperSize = 0;
        private int m_paperSizeWidth = 0;
        private int m_paperSizeHeight = 0;
        private int m_orientation = 0;
        private string m_printerName = "";
        private string m_driverName = "";
        private string m_port = "";

        private cEditor m_contextMenuEditor;

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

        public PictureBox pic1() {
            return pictureBox1;
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

            if (menuItems.Count < cGlobals.C_TOTINRECENTLIST && found == false)
            {
                var menu = this.mnuFileRecentList.DropDownItems.Add("");
                menu.Visible = true;
            }

            if (!found) { j = menuItems.Count - 1; }

            for (i = j; i > 0; i--)
            {
                menuItems[i].Text = menuItems[i - 1].Text;
            }

            menuItems[0].Text = fileName;
        }

        public void loadRecentList(List<String> recentList)
        {
            int i = 0;
            String recent = "";

            for (i = 0; i < Math.Min(cGlobals.C_TOTINRECENTLIST, recentList.Count); i++)
            {
                recent = recentList[i];
                var menu = this.mnuFileRecentList.DropDownItems.Add(recent);
                menu.Visible = true;
            }

            if (this.mnuFileRecentList.DropDownItems.Count > 1)
            {
                this.mnuFileRecentList.Visible = true;
            }
        }

        public void saveRecentList()
        {
            int i = 0;

            for (i = 0; i < this.mnuFileRecentList.DropDownItems.Count; i++)
            {
                // TODO: implement
            }
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

                if(editor.openDocument()) {
                    addToRecentList(editor.getFileName());
                    saveRecentList();
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
            cPrintAPI.getDefaultPrinter(out m_printerName, out m_driverName, out m_port, out m_paperSize, out m_orientation, out m_paperSizeWidth, out m_paperSizeHeight);
            
            //
            // remove me and implement a better window position code
            //
            this.Width = 1200;
            this.Height = 900;
            cWindow.centerForm(this);
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
    }
}
