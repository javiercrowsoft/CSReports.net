using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using CSKernelClient;
using CSDataBase;
using CSReportGlobals;
using CSReportDll;
using CSReportPaint;

namespace CSReportEditor
{
    public class cEditor
    {
        private fMain m_fmain = null;
        private Panel m_editor = null;
        private PictureBox m_picRule = null;
        private PictureBox m_picReport = null;
        private TabPage m_editorTab = null;
        private String m_reportFullPath = "";
		private String m_name = "";

        public cEditor(fMain fmain, Panel editor, PictureBox rule, PictureBox report, TabPage editorTab) {
            m_fmain = fmain;
            m_editor = editor;
            m_editor.AutoScroll = true;
        
            m_picRule = rule;
            m_picRule.SetBounds(cUtil.mp(1), cUtil.mp(1), cUtil.mp(50), cUtil.mp(297));
            m_picRule.BackColor = Color.PeachPuff;

            m_picReport = report;
            m_picReport.SetBounds(cUtil.mp(50) + cUtil.mp(1), cUtil.mp(1), cUtil.mp(210), cUtil.mp(297));
            m_picReport.BackColor = Color.Beige;

            m_picReport.Paint += new PaintEventHandler(m_picReport_Paint);
            m_picRule.Paint += new PaintEventHandler(m_picRule_Paint);

            // mouse events
            //
            m_picReport.MouseDown += new MouseEventHandler(m_picReport_MouseDown);
            m_picReport.MouseUp += new MouseEventHandler(m_picReport_MouseUp);
            m_picReport.MouseMove += new MouseEventHandler(m_picReport_MouseMove);

            // mouse well
            //
            // se me cae un lagrimon :(
            //
            m_picReport.Click += (s, e) => { editor.Focus(); };

            // tab
            //
            m_editorTab = editorTab;

            m_editorTab.Enter += (s, e) => { cMainEditor.setDocActive(this); };

            m_editorTab.Tag = this;
        }
        
        private cEditor() {}

        public bool close() 
        {
            if (!saveChanges())
            {
                return false;
            }
            else { 
            
                // TODO: dispose all objects
                return true;
            }
        }

        private const String C_MODULE = "cEditor";
        private const int C_TOPBODY = 10;
        private const int C_LEFTBODY = 0;
        private const int C_MIN_HEIGHT_SECTION = 3;
        private const String C_SECTIONLINE = "Line ";

        private const int C_NOMOVE = -1111111;

        private cReport m_report;
        private CSReportPaint.cReportPaint m_paint;
        private String m_keyMoving = "";
        private csRptEditorMoveType m_moveType;
        private String m_keySizing = "";
        private bool m_mouseButtonPress = false;
        private float m_offX = 0;
        private float m_offY = 0;
        private String m_keyObj = "";
        private String m_keyFocus = "";
        private bool m_moving = false;
        private bool m_opening = false;
        private float m_offSet = 0;

        // the first SectionLine from where we need 
        // to modify the top after moving sections. 
        // It is used only in footers.
        private int m_indexSecLnMoved = 0;

        // it is used in MoveSection to calculate
        // the positions after adding new SectionLines.
        //
        // good explanation is found in addSectionLine
        //
        private float m_newSecLineOffSet = 0;

        private bool m_bMoveVertical = false;
        private bool m_bMoveHorizontal = false;
        private bool m_bNoMove = false;

        private String[] m_vSelectedKeys = new String[0];
        private String[] m_vCopyKeys = new String[0];

        private fProgress m_fProgress;
        private bool m_cancelPrinting = false;

        private int m_formIndex = 0;

        private fProperties m_fProperties;
        private fSecProperties m_fSecProperties;
        private fFormula m_fFormula;
        private fGroup m_fGroup;
        private fConnectsAux m_fConnectsAux;

        // names
        private int m_nextNameCtrl = 0;
        private bool m_showingProperties = false;
        private bool m_dataHasChanged = false;

        // to add new controls
        private bool m_copyControls = false;
        private bool m_copyControlsFromOtherReport = false;
        private bool m_bCopyWithoutMoving = false;

        private bool m_draging = false;
        private String m_controlName = "";
        private csRptEditCtrlType m_controlType;
        private String m_fieldName = "";
        private int m_fieldType = 0;
        private int m_fieldIndex = 0;
        private String m_formulaText = "";

        private int m_x = 0;
        private int m_y = 0;
        private bool m_keyboardMove = false;

        private int m_keyboardMoveStep = 0;

        private bool m_inMouseDown = false;

        private CSReportPaint.csETypeGrid m_typeGrid;

        public TabPage getEditorTab() {
            return m_editorTab;
        }

        public String getVCopyKeys(int idx) {
            return m_vCopyKeys[idx];
        }

        public int getVCopyKeysCount() {
            return m_vCopyKeys.Length;
        }

        public CSReportPaint.cReportPaint getPaint() {
            return m_paint;
        }

        public void setKeyboardMoveStep(int rhs) {
            m_keyboardMoveStep = rhs;
        }
        public bool getBMoveNoMove() {
            return m_bNoMove;
        }
        public bool getBMoveVertical() {
            return m_bMoveVertical;
        }
        public bool getBMoveHorizontal() {
            return m_bMoveHorizontal;
        }

        public csReportPaperType getPaperSize() {
            if (m_report == null) { return 0; }
            return m_report.getPaperInfo().getPaperSize();
        }

        public int getOrientation() {
            if (m_report == null) { return 0; }
            return m_report.getPaperInfo().getOrientation();
        }

        public int getCopies() {
            if (m_report == null) { return 0; }
            return m_report.getLaunchInfo().getCopies();
        }

        public void setPaperSize(csReportPaperType rhs) {
            if (m_report == null) { return; }
            m_report.getPaperInfo().setPaperSize(rhs);
        }

        public void setOrientation(int rhs) {
            if (m_report == null) { return; }
            m_report.getPaperInfo().setOrientation(rhs);
        }

        public void setCopies(int rhs) {
            if (m_report == null) { return; }
            m_report.getLaunchInfo().setCopies(rhs);
        }

        public void setCustomHeight(int rhs) {
            if (m_report == null) { return; }
            m_report.getPaperInfo().setCustomHeight(rhs);
        }

        public void setCustomWidth(int rhs) {
            if (m_report == null) { return; }
            m_report.getPaperInfo().setCustomWidth(rhs);
        }

        public int getCustomHeight() {
            if (m_report == null) { return 0; }
            return m_report.getPaperInfo().getCustomHeight();
        }

        public int getCustomWidth() {
            if (m_report == null) { return 0; }
            return m_report.getPaperInfo().getCustomWidth();
        }

        public String getFileName() {
            return m_report.getPath() + m_report.getName();
        }

        public bool getShowingProperties() {
            return m_showingProperties;
        }
        
        public void setShowingProperties(bool rhs) {
            m_showingProperties = rhs;
        }

        public fGroup getFGroup() {
            return m_fGroup;
        }
        
        public void setFGroup(fGroup rhs) {
            m_fGroup = rhs;
        }
        
        public cReport getReport() {
            return m_report;
        }

        public bool getDataHasChanged() {
            return m_dataHasChanged;
        }

        public void setDataHasChanged(bool rhs) {
            m_dataHasChanged = rhs;
        }

        public void search() {
            try
            {
                fSearch f = cMainEditor.getSearch(this);
                f.clear();
                if (!f.Visible)
                {
                    f.Show(m_fmain);
                }
            }
            catch (Exception ex)
            {
                cError.mngError(ex, "showControls", C_MODULE, "");
            }
        }

        public void moveVertical() {
			// TODO: reimplement
			//form_KeyUp(Keys.F11, false);
        }

        public void moveHorizontal() {
			// TODO: reimplement
			//form_KeyUp(Keys.F12, false);
        }

        public void moveNoMove() {
			// TODO: reimplement
			//form_KeyUp(Keys.F9, false);
        }

        public void moveAll() {
			// TODO: reimplement
			//form_KeyUp(Keys.F8, false);
        }

        public void showGrid(CSReportPaint.csETypeGrid typeGrid) {
            m_typeGrid = typeGrid;
            m_paint.initGrid(m_picReport.CreateGraphics(), typeGrid);
        }

        public void showConnectsAux() {
            try {
                m_fConnectsAux = new fConnectsAux();

                /* TODO: this code must to be moved to fConnectsAux constructor
                 * 
                
                __TYPE_NOT_FOUND w___TYPE_NOT_FOUND = m_fConnectsAux.lvColumns;
                    w___TYPE_NOT_FOUND.ListItems.fToolbox.clear();
                    w___TYPE_NOT_FOUND.ColumnHeaders.fToolbox.clear();
                    w___TYPE_NOT_FOUND.ColumnHeaders.Add(, , "DataSource", 2500);
                    w___TYPE_NOT_FOUND.ColumnHeaders.Add(, , "StrConnect", 5000);
                */
				for (int _i = 0; _i < m_report.getConnectsAux().count(); _i++) {
					pAddConnectAuxToListView(m_report.getConnectsAux().item(_i));
                }                
                m_fConnectsAux.ShowDialog();

            } catch (Exception ex) {
                cError.mngError(ex, "showConnectsAux", C_MODULE, "");
                m_fConnectsAux.Close();
                m_fConnectsAux = null;
            }
        }

        private void pAddConnectAuxToListView(cReportConnect connect) {
			m_fConnectsAux.addConnect(connect.getDataSource(), connect.getStrConnect());
        }
		// TODO: reimplement
		/*
        private void form_KeyUp(Keys keyCode, bool ctrlKey) {
            // if we are in edit mode we do nothing
            //
            if (TxEdit.Visible) { return; }

            switch (keyCode) {

                case Keys.F2:
                    editText();

                    break;
                case Keys.Delete:
                    deleteObj();

                    break;
                case Keys.Escape:
                    endDraging();

                    break;
                case Keys.F11:
                    m_bMoveVertical = true;
                    m_bMoveHorizontal = false;
                    cGlobals.setStatus();

                    break;
                case Keys.F12:
                    m_bMoveHorizontal = true;
                    m_bMoveVertical = false;
                    cGlobals.setStatus();

                    break;
                case Keys.F8:
                    m_bMoveHorizontal = false;
                    m_bMoveVertical = false;
                    cGlobals.setStatus();

                    break;
                case Keys.F9:
                    m_bNoMove = !m_bNoMove;
                    cGlobals.setStatus();

                    break;
                case Keys.F4:
                    showProperties();

                    break;
                case Keys.C:
                    if (ctrlKey) {
                        copy();
                    }

                    break;
                case Keys.V:
                    if (ctrlKey) {
                        paste(false);
                    }

                    break;
            }

            Application.DoEvents();
        }
		*/

        // TODO: this functionality must to be moved to fConnectsAux
        //
        private void m_fConnectsAux_AddConnect() {
            try {

                cReportConnect rptConnect = null;
                rptConnect = new cReportConnect();

                if (!configConnection(rptConnect)) { return; }

                m_report.getConnectsAux().add(rptConnect);

                pAddConnectAuxToListView(rptConnect);

            } catch (Exception ex) {
                cError.mngError(ex, "m_fConnectsAux_AddConnect", C_MODULE, "");        
            }
        }

        // TODO: this functionality must to be moved to fConnectsAux
        //
        private void m_fConnectsAux_DeleteConnect() {
			/*
            try {
                int index = 0;

                if (m_fConnectsAux.lvColumns.SelectedItem == null) {
                    cWindow.msgWarning("Select one connection", "Additional connections");
                    return;
                }

                // TODO: this functionality must to be refactored to separate the
                //       UI code from the business code
                //
                index = m_fConnectsAux.lvColumns.SelectedItem.index;

                m_report.getConnectsAux().remove(index);

                m_fConnectsAux.lvColumns.ListItems.Remove(index);

            } catch (Exception ex) {
                cError.mngError(ex, "m_fConnectsAux_DeleteConnect", C_MODULE, "");
            }
            */
        }

        // TODO: this functionality must to be moved to fConnectsAux
        //
        private void m_fConnectsAux_EditConnect() {
			/*
            try {
                int index = 0;

                if (m_fConnectsAux.lvColumns.SelectedItem == null) {
                    cWindow.msgWarning("Select one connection", "Additional Connections");
                    return;
                }

                index = m_fConnectsAux.lvColumns.SelectedItem.index;

                if (!configConnection(m_report.getConnectsAux(index))) { return; }

                //TODO:** can't found type for with block
                //With m_fConnectsAux.lvColumns.SelectedItem
                __TYPE_NOT_FOUND w___TYPE_NOT_FOUND = m_fConnectsAux.lvColumns.SelectedItem;
                    w___TYPE_NOT_FOUND.Text = m_report.getConnectsAux(index).DataSource;
                    w___TYPE_NOT_FOUND.SubItems(1) = m_report.getConnectsAux(index).strConnect;
                // {end with: w___TYPE_NOT_FOUND}

            } catch (Exception ex) {
                cError.mngError(ex, "m_fConnectsAux_EditConnect", C_MODULE, "");
            }
            */
        }

        private void m_fSearch_EditCtrl(String ctrlKey) {
            try {

                selectCtrl(ctrlKey);
                showProperties();

            } catch (Exception ex) {
                cError.mngError(ex, "m_fSearch_EditCtrl", C_MODULE, "");
            }
        }

        private void m_fSearch_SetFocusSec(String secKey) {
            try {

                pSelectSection(secKey);

            } catch (Exception ex) {
                cError.mngError(ex, "m_fSearch_SetFocusSec", C_MODULE, "");
            }
        }

        private void editCtrl(String ctrlKey) {
            try {

                selectCtrl(ctrlKey);
                showProperties();
            } catch (Exception ex) {
                cError.mngError(ex, "editCtrl", C_MODULE, "");
            }
        }

        public void editSection(String secKey) {
            try {

                bool bIsSecLn = false;

				pSelectSection(secKey, out bIsSecLn);

                if (bIsSecLn) {
                    showSecLnProperties();
                } 
                else {
                    showProperties();
                }
            } catch (Exception ex) {
                cError.mngError(ex, "editCtrl", C_MODULE, "");
            }
        }

        public void setFocusCtrl(String ctrlKey) {
            try {

                selectCtrl(ctrlKey);

            } catch (Exception ex) {
                cError.mngError(ex, "setFocusCtrl", C_MODULE, "");
            }
        }

        public object getSectionOrSectionLineFromKey(string key)
        {
            object sec = getSectionOrSectionLineFromKey(key, m_report.getHeaders());
            if (sec == null) 
            {
                sec = getSectionOrSectionLineFromKey(key, m_report.getGroupsHeaders());
                if (sec == null)
                {
                    sec = getSectionOrSectionLineFromKey(key, m_report.getDetails());
                    if (sec == null)
                    {
                        sec = getSectionOrSectionLineFromKey(key, m_report.getGroupsFooters());
                        if (sec == null)
                        {
                            sec = getSectionOrSectionLineFromKey(key, m_report.getFooters());
                        }
                    }
                }
            }
            
            return sec;
        }

        private object getSectionOrSectionLineFromKey(string key, cIReportGroupSections sections)
        {
            for (int i = 0; i < sections.count(); i++)
            {
                var sec = sections.item(i);
                if (sec.getKey() == key)
                {
                    return sec;
                }
                else {
                    var secLn = sec.getSectionLines().item(key);
                    if (secLn != null)
                    {
                        return secLn;
                    }
                }
            }
            return null;
        }

        public void selectSection(String secKey)
        {
            try {

                pSelectSection(secKey);

            } catch (Exception ex) {
                cError.mngError(ex, "setelectSection", C_MODULE, "");
            }
        }

        public void selectCtrl(String ctrlKey) {
            bool bWasRemoved = false;
            String sKey = "";

			G.redim(ref m_vSelectedKeys, 0);
            sKey = getReport().getControls().item(ctrlKey).getKeyPaint();
            pAddToSelected(sKey, false, out bWasRemoved);

            if (bWasRemoved) { sKey = ""; }

            m_keyFocus = sKey;
            m_keyObj = sKey;
            m_paint.setFocus(m_keyFocus, m_picReport.CreateGraphics(), true);
            cMainEditor.showProperties(ctrlKey);
        }

		private void pSelectSection(String secKey) 
		{
			bool bIsSecLn = false;
			pSelectSection(secKey, out bIsSecLn);
		}

        private void pSelectSection(String secKey, out bool bIsSecLn) {
            bool bWasRemoved = false;
            String sKey = "";

            bIsSecLn = false;

			G.redim(ref m_vSelectedKeys, 0);

			if (m_report.getHeaders().item(secKey) != null) {
                sKey = m_report.getHeaders().item(secKey).getKeyPaint();
            } 
			else if (m_report.getGroupsHeaders().item(secKey) != null) {
                sKey = m_report.getGroupsHeaders().item(secKey).getKeyPaint();
            } 
			else if (m_report.getDetails().item(secKey) != null) {
                sKey = m_report.getDetails().item(secKey).getKeyPaint();
            } 
			else if (m_report.getGroupsFooters().item(secKey) != null) {
                sKey = m_report.getGroupsFooters().item(secKey).getKeyPaint();
            } 
			else if (m_report.getFooters().item(secKey) != null) {
                sKey = m_report.getFooters().item(secKey).getKeyPaint();
            } 
            else {
                cReportSectionLine secLn = null;
                cReportSection sec = null;

                bIsSecLn = true;

				secLn = pGetSecLnFromKey(secKey, m_report.getHeaders(), out sec);
                if (secLn != null) {
                    sKey = secLn.getKeyPaint();
                    if (sKey == "") {
                        sKey = sec.getKeyPaint();
                    }
                } 
                else {
					secLn = pGetSecLnFromKey(secKey, m_report.getGroupsHeaders(), out sec);
                    if (secLn != null) {
                        sKey = secLn.getKeyPaint();
                        if (sKey == "") {
                            sKey = sec.getKeyPaint();
                        }
                    } 
                    else {
						secLn = pGetSecLnFromKey(secKey, m_report.getDetails(), out sec);
                        if (secLn != null) {
                            sKey = secLn.getKeyPaint();
                            if (sKey == "") {
                                sKey = sec.getKeyPaint();
                            }
                        } 
                        else {
							secLn = pGetSecLnFromKey(secKey, m_report.getGroupsFooters(), out sec);
                            if (secLn != null) {
                                sKey = secLn.getKeyPaint();
                                if (sKey == "") {
                                    sKey = sec.getKeyPaint();
                                }
                            } 
                            else {
								secLn = pGetSecLnFromKey(secKey, m_report.getFooters(), out sec);
                                if (secLn != null) {
                                    sKey = secLn.getKeyPaint();
                                    if (sKey == "") {
                                        sKey = sec.getKeyPaint();
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (sKey == "") { return; }

            pAddToSelected(sKey, false, out bWasRemoved);
            if (bWasRemoved) { sKey = ""; }

            m_keyFocus = sKey;
            m_keyObj = sKey;
            m_paint.setFocus(m_keyFocus, m_picReport.CreateGraphics(), true);
            cMainEditor.showProperties("S" + secKey);
        }

        private cReportSectionLine pGetSecLnFromKey(
            String secKey, 
			cIReportGroupSections sections, 
            out cReportSection rtnSec) 
        { 
            cReportSection sec = null;
			rtnSec = null;
			for (int _i = 0; _i < sections.count(); _i++) {
				sec = sections.item(_i);
				if (sec.getSectionLines().item(secKey) != null) {
                    rtnSec = sec;
                    return sec.getSectionLines().item(secKey);
                }
            }
			return null;
        }

        public bool checkSyntax(String code) { 
            cReportFormula f = null;
            
            f = new cReportFormula();
            
            if (m_fProperties != null) 
            {
                f.setName(m_fProperties.getFormulaName());
            }
            else if (m_fSecProperties != null)
            {
                f.setName(m_fSecProperties.getFormulaName());
            }
            else 
            {
                f.setName(cMainEditor.getCtrlTreeBox().getFormulaName());
            }

            f.setText(code);
			
            return m_report.getCompiler().checkSyntax(f);
        }

		public bool showHelpChartField(TextBox ctrl, int idx) {
            int nIndex = 0;
            int nFieldType = 0;
            String sField = "";

            sField = ctrl.Text;
            nFieldType = m_fProperties.getChartFieldType(idx);
            nIndex = m_fProperties.getChartIndex(idx);

            if (cGlobals.showDbFields(ref sField, ref nFieldType, ref nIndex, this))
            {
                ctrl.Text = sField;
                m_fProperties.setChartFieldType(idx, nFieldType);
                m_fProperties.setChartIndex(idx, nIndex);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool showHelpChartGroupField() {
            int nIndex = 0;
            int nFieldType = 0;
            String sField = "";

			sField = m_fProperties.getDbFieldGroupValue();
            nFieldType = m_fProperties.getChartGroupFieldType();
            nIndex = m_fProperties.getChartGroupIndex();

			if(cGlobals.showDbFields(ref sField, ref nFieldType, ref nIndex, this)) 
            {
                m_fProperties.setDbFieldGroupValue(sField);
                m_fProperties.setChartGroupFieldType(nFieldType);
                m_fProperties.setChartGroupIndex(nIndex);
                return true;
            }
            else
            {
                return false;
            }
        }

		public bool showEditFormula(ref String formula) {

            try
            {
                cReportFormulaType f = null;
                cReportControl c = null;

                if (m_fFormula == null)
                {
                    m_fFormula = new fFormula();
                    // TODO: set event handlers for fFormula
                }

                // TODO: this functionality has to be moved to fFormula
                //

                // Load formulas in the tree
                m_fFormula.createTree();

                for (int _i = 0; _i < m_report.getFormulaTypes().count(); _i++)
                {
                    f = m_report.getFormulaTypes().item(_i);
                    m_fFormula.addFormula(f.getId(), f.getName(), f.getNameUser(), f.getDecrip(), f.getHelpContextId());
                }

                for (int _i = 0; _i < m_report.getControls().count(); _i++)
                {
                    c = m_report.getControls().item(_i);
                    if (c.getControlType() == csRptControlType.CSRPTCTFIELD)
                    {
                        m_fFormula.addDBField(c.getName(), c.getField().getName());
                    }
                    else if (c.getControlType() == csRptControlType.CSRPTCTLABEL)
                    {
                        m_fFormula.addLabel(c.getName());
                    }
                }

                m_fFormula.setFormula(formula);

                m_fFormula.setHandler(this);

                m_fFormula.expandTree();

                //
                // TODO: end functionality to move 

                m_fFormula.ShowDialog();

                if (m_fFormula.getOk())
                {
                    formula = m_fFormula.getFormula();
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                cError.mngError(ex, "showEditFormula", C_MODULE, "");
                return false;
            }
            finally 
            {
                m_fFormula.Hide();
                m_fFormula = null;
            }
		}

        private void m_fSecProperties_UnloadForm() {
            m_fSecProperties = null;
        }

        private void m_fToolBox_AddControl(
            String controlName, 
            csRptEditCtrlType controlType, 
            String fieldName, 
            String formulaText, 
            int fieldType, 
            int fieldIndex) 
        {
            beginDraging();
            m_controlName = controlName;
            m_controlType = controlType;
            m_fieldName = fieldName;
            m_formulaText = formulaText;
            m_fieldIndex = fieldIndex;
            m_fieldType = fieldType;
        }

        private void m_fTreeCtrls_UpdateFormulaHide(String ctrlKey, String formula) {
            m_report.getControls().item(ctrlKey).getFormulaHide().setText(formula);
        }

        private void m_fTreeCtrls_UpdateFormulaValue(String ctrlKey, String formula) {
            m_report.getControls().item(ctrlKey).getFormulaValue().setText(formula);
        }

        private void m_fTreeCtrls_UpdateSectionFormulaHide(String secKey, String formula) {

			if (m_report.getHeaders().item(secKey) != null) {
                m_report.getHeaders().item(secKey).getFormulaHide().setText(formula);
            } 
			else if (m_report.getGroupsHeaders().item(secKey) != null) {
                m_report.getGroupsHeaders().item(secKey).getFormulaHide().setText(formula);
            } 
			else if (m_report.getDetails().item(secKey) != null) {
                m_report.getDetails().item(secKey).getFormulaHide().setText(formula);
            } 
			else if (m_report.getGroupsFooters().item(secKey) != null) {
                m_report.getGroupsFooters().item(secKey).getFormulaHide().setText(formula);
            } 
			else if (m_report.getFooters().item(secKey) != null) {
                m_report.getFooters().item(secKey).getFormulaHide().setText(formula);
            } 
            else {
                cReportSectionLine secLn = null;
                cReportSection sec = null;

				secLn = pGetSecLnFromKey(secKey, m_report.getHeaders(), out sec);
                if (secLn != null) {
                    secLn.getFormulaHide().setText(formula);
                } 
                else {
					secLn = pGetSecLnFromKey(secKey, m_report.getGroupsHeaders(), out sec);
                    if (secLn != null) {
                        secLn.getFormulaHide().setText(formula);
                    } 
                    else {
						secLn = pGetSecLnFromKey(secKey, m_report.getDetails(), out sec);
                        if (secLn != null) {
                            secLn.getFormulaHide().setText(formula);
                        } 
                        else {
							secLn = pGetSecLnFromKey(secKey, m_report.getGroupsFooters(), out sec);
                            if (secLn != null) {
                                secLn.getFormulaHide().setText(formula);
                            } 
                            else {
								secLn = pGetSecLnFromKey(secKey, m_report.getFooters(), out sec);
                                if (secLn != null) {
                                    secLn.getFormulaHide().setText(formula);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void m_picReport_KeyDown(int keyCode, int shift) {
			cReportAspect aspect = null;
            try {

                // only process arrow keys 
                switch (keyCode) {
				case (int)Keys.Up:
                        break;
				case (int)Keys.Down:
                        break;
				case (int)Keys.Left:
                        break;
				case (int)Keys.Right:
                        break;
                default:
                        return;
                }

                int x = 0;
                int y = 0;

				if (m_vSelectedKeys.Length < 1) { return; }

                if (!m_keyboardMove) {
                    aspect = m_paint.getPaintObject(m_vSelectedKeys[1]).getAspect();
					y = Convert.ToInt32(aspect.getTop());
					x = Convert.ToInt32(aspect.getLeft());
                } 
                else {
                    y = m_y;
                    x = m_x;
                }

                // resize
                //
				if (Control.ModifierKeys == Keys.Shift) {

                    if (m_keySizing == "") {
                        m_keySizing = m_paint.getPaintObject(m_vSelectedKeys[1]).getKey();
                    }

                    if (!m_keyboardMove) {

                        aspect = m_paint.getPaintObject(m_vSelectedKeys[1]).getAspect();
						y += Convert.ToInt32(aspect.getHeight());
						x += Convert.ToInt32(aspect.getWidth());

                        pSetMovingFromKeyboard(x, y);

                        if (m_keySizing == "") {
                            m_keySizing = m_paint.getPaintObject(m_vSelectedKeys[1]).getKey();
                        }

                        switch (keyCode) {

						case (int)Keys.Down:
						case (int)Keys.Up:
                                m_keyMoving = "";
                                m_moveType = csRptEditorMoveType.CSRPTEDMOVDOWN;
                                break;
						case (int)Keys.Right:
						case (int)Keys.Left:
                                m_keyMoving = "";
                                m_moveType = csRptEditorMoveType.CSRPTEDMOVRIGHT;
                                break;
                        }
                    }

                    switch (keyCode) {
					case (int)Keys.Up:
                            y = y - m_keyboardMoveStep;
                            break;
					case (int)Keys.Down:
                            y = y + m_keyboardMoveStep;
                            break;
					case (int)Keys.Left:
                            x = x - m_keyboardMoveStep;
                            break;
					case (int)Keys.Right:
                            x = x + m_keyboardMoveStep;
                            break;
                    }

                    // move
                    //
                } 
                else {

                    if (!m_keyboardMove) {
                        pSetMovingFromKeyboard(x, y);
                    }

                    if (m_keyMoving == "") {
                        m_keyMoving = m_paint.getPaintObject(m_vSelectedKeys[1]).getKey();
                    }

                    switch (keyCode) {
					case (int)Keys.Up:
                            y = y - m_keyboardMoveStep;
                            break;
					case (int)Keys.Down:
                            y = y + m_keyboardMoveStep;
                            break;
					case (int)Keys.Left:
                            x = x - m_keyboardMoveStep;
                            break;
					case (int)Keys.Right:
                            x = x + m_keyboardMoveStep;
                            break;
                    }
                }

                m_picReport_MouseMove(this, new MouseEventArgs(MouseButtons.Left, 0, x, y, 0));
                m_x = x;
                m_y = y;

                m_keyboardMove = true;

            } catch (Exception ex) {
                cError.mngError(ex, "m_picReport_KeyDown", C_MODULE, "");
            }
        }

        private void pSetMovingFromKeyboard(float x, float y) {

            m_keyMoving = m_keyFocus;

            CSReportPaint.cReportPaintObject po = m_paint.getPaintObject(m_keyMoving);

			switch (po.getTag()) {
                case cGlobals.C_KEY_DETAIL:
                case cGlobals.C_KEY_FOOTER:
                case cGlobals.C_KEY_HEADER:
                    m_moveType = csRptEditorMoveType.CSRPTEDMOVTVERTICAL;
					m_picReport.Cursor = Cursors.SizeNS;
                    break;
				default:
					if (po.getRptType() == csRptTypeSection.DETAIL 
						|| po.getRptType() == csRptTypeSection.HEADER
                        || po.getRptType() == csRptTypeSection.GROUP_HEADER 
						|| po.getRptType() == csRptTypeSection.GROUP_FOOTER 
						|| po.getRptType() == csRptTypeSection.FOOTER) {

						m_picReport.Cursor = Cursors.SizeNS;
                        m_moveType = csRptEditorMoveType.CSRPTEDMOVTVERTICAL;

                    } 
					else if (po.getRptType() == csRptTypeSection.SECLN_HEADER 
						|| po.getRptType() == csRptTypeSection.SECLN_DETAIL 
						|| po.getRptType() == csRptTypeSection.SECLN_FOOTER 
						|| po.getRptType() == csRptTypeSection.SECLN_GROUPH 
						|| po.getRptType() == csRptTypeSection.SECLN_GROUPF) {

					    m_picReport.Cursor = Cursors.SizeNS;
                        m_moveType = csRptEditorMoveType.CSRPTEDMOVTVERTICAL;

                    } 
                    else {
                        m_moveType = csRptEditorMoveType.CSRPTEDMOVTALL;
						m_picReport.Cursor = Cursors.SizeNS;
                    }
                    break;
            }

            cReportAspect aspect = m_paint.getPaintObject(m_keyMoving).getAspect();
			m_offX = x - aspect.getLeft();
			m_offY = y - (aspect.getTop() - aspect.getOffset());

            m_keyObj = m_keyMoving;

			cGlobals.setEditAlignTextState(m_vSelectedKeys.Length);
			cGlobals.setEditAlignCtlState(m_vSelectedKeys.Length > 1);
            pSetEditAlignValue();
            pSetFontBoldValue();

        }

        private void m_picReport_KeyUp(int keyCode, bool ctrlKey) {
            if (m_keyboardMove) {
                m_keyboardMove = false;
                m_picReport_MouseUp(this, new MouseEventArgs(MouseButtons.Left, 0, m_x, m_y, 0));
            }
        }

        private void m_picReport_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (m_paint == null) return;

            MouseButtons button = e.Button;
            bool ctrlKey = Control.ModifierKeys.HasFlag(Keys.Control); 
            int x = e.X;
            int y = e.Y;

            try
            {

                String sKey = "";
                bool bClearSelected = false;
                String lastKeyMoving = "";
                String lastKeyObj = "";

                // to avoid reentrancy
                if (m_opening) { return; }

                m_inMouseDown = true;

                if (m_draging)
                {
                    addControlEnd(x, y);
                    endDraging();
                }

                endEditText(false);

                bClearSelected = pClearSelected(button, ctrlKey, x, y);

                if (button == MouseButtons.Left)
                {

                    lastKeyObj = m_keyObj;
                    m_keyObj = "";

                    sKey = m_keyMoving != "" ? m_keyMoving : m_keySizing;

                    // to force focus in the header
                    if (sKey == "")
                    {
                        m_paint.pointIsInObject(x, y, ref sKey);

                        if (sKey != "")
                        {

                            CSReportPaint.cReportPaintObject po = m_paint.getPaintObject(sKey);
                            lastKeyMoving = m_keyMoving;
                            m_keyMoving = sKey;

                            switch (po.getTag())
                            {
                                case cGlobals.C_KEY_DETAIL:
                                case cGlobals.C_KEY_FOOTER:
                                case cGlobals.C_KEY_HEADER:

                                    // only if no controls are selected
                                    //
                                    if (ctrlKey)
                                    {

                                        if (m_vSelectedKeys.Length > 0)
                                            return;
                                        if (m_vSelectedKeys[0].Length > 0)
                                            return;
                                        m_keyMoving = lastKeyMoving;
                                        m_keyObj = lastKeyObj;
                                        return;
                                    }

                                    m_moveType = csRptEditorMoveType.CSRPTEDMOVTVERTICAL;
                                    m_picReport.Cursor = Cursors.SizeNS;
                                    break;

                                default:
                                    if (po.getRptType() == csRptTypeSection.DETAIL
                                        || po.getRptType() == csRptTypeSection.HEADER
                                        || po.getRptType() == csRptTypeSection.GROUP_HEADER
                                        || po.getRptType() == csRptTypeSection.GROUP_FOOTER
                                        || po.getRptType() == csRptTypeSection.FOOTER)
                                    {

                                        // only if no controls are selected
                                        //
                                        if (ctrlKey)
                                        {

                                            if (m_vSelectedKeys.Length > 0)
                                                return;
                                            if (m_vSelectedKeys[0].Length > 0)
                                                return;
                                            m_keyMoving = lastKeyMoving;
                                            m_keyObj = lastKeyObj;
                                            return;
                                        }

                                        m_picReport.Cursor = Cursors.SizeNS;
                                        m_moveType = csRptEditorMoveType.CSRPTEDMOVTVERTICAL;

                                    }
                                    else if (po.getRptType() == csRptTypeSection.SECLN_HEADER
                                        || po.getRptType() == csRptTypeSection.SECLN_DETAIL
                                        || po.getRptType() == csRptTypeSection.SECLN_FOOTER
                                        || po.getRptType() == csRptTypeSection.SECLN_GROUPH
                                        || po.getRptType() == csRptTypeSection.SECLN_GROUPF)
                                    {

                                        // only if no controls are selected
                                        //
                                        if (ctrlKey)
                                        {
                                            if (m_vSelectedKeys.Length > 0)
                                                return;
                                            if (m_vSelectedKeys[0].Length > 0)
                                                return;
                                            m_keyMoving = lastKeyMoving;
                                            m_keyObj = lastKeyObj;
                                            return;
                                        }

                                        m_picReport.Cursor = Cursors.SizeNS;
                                        m_moveType = csRptEditorMoveType.CSRPTEDMOVTVERTICAL;

                                    }
                                    else
                                    {
                                        m_moveType = csRptEditorMoveType.CSRPTEDMOVTALL;
                                        m_picReport.Cursor = Cursors.SizeAll;
                                    }
                                    break;
                            }
                        }
                    }

                    bool bWasRemoved = false;
                    pAddToSelected(m_keyMoving, ctrlKey, out bWasRemoved);

                    if (bWasRemoved) { sKey = ""; }

                    if (sKey != "")
                    {
                        cReportAspect aspect = m_paint.getPaintObject(sKey).getAspect();
                        m_offX = x - aspect.getLeft();
                        m_offY = y - (aspect.getTop() - aspect.getOffset());
                    }

                    m_keyFocus = sKey;
                    m_keyObj = sKey;
                    m_paint.setFocus(m_keyFocus, m_picReport.CreateGraphics(), bClearSelected);

                    CSReportPaint.cReportPaintObject poSelected = m_paint.getPaintObject(sKey);
                    if (poSelected != null)
                    {
                        cMainEditor.showProperties(
                            poSelected.getIsSection()
                            ? "S" + poSelected.getTag() 
                            : poSelected.getTag());
                    }                    
                }
                else if (button == MouseButtons.Right)
                {
                    m_keySizing = "";
                    m_keyMoving = "";
                    m_keyObj = "";

                    if (m_paint.pointIsInObject(x, y, ref sKey))
                    {
                        m_keyObj = sKey;

                        bClearSelected = pSetSelectForRightBttn();

                        m_keyFocus = sKey;
                        m_paint.setFocus(m_keyFocus, m_picReport.CreateGraphics(), bClearSelected);

                        CSReportPaint.cReportPaintObject po = m_paint.getPaintObject(sKey);

                        if (m_paint.paintObjIsSection(sKey))
                        {

                            bool noDelete = false;

                            switch (po.getTag())
                            {
                                // this sections can not be moved
                                case cGlobals.C_KEY_HEADER:
                                case cGlobals.C_KEY_DETAIL:
                                case cGlobals.C_KEY_FOOTER:
                                    noDelete = true;
                                    break;

                                default:
                                    noDelete = false;
                                    break;
                            }

                            bool isGroup = false;
                            bool isSecLn = false;

                            pGetSection(out isGroup, out isSecLn);

                            if (isSecLn) { noDelete = true; }

                            showPopMenuSection(noDelete, isGroup, x, y);

                            cMainEditor.showProperties("S" + po.getTag());
                        }
                        else
                        {
                            showPopMenuControl(true, x, y);
                            
                            cMainEditor.showProperties(po.getTag());
                        }
                    }
                    else
                    {
                        showPopMenuControl(false, x, y);
                    }
                }

                cGlobals.setEditAlignTextState(m_vSelectedKeys.Length);
                cGlobals.setEditAlignCtlState(m_vSelectedKeys.Length > 1);
                pSetEditAlignValue();
                pSetFontBoldValue();

            }
            catch (Exception ex)
            {
                cError.mngError(ex, "m_picReport_MouseDown", C_MODULE, "");
            }
            finally 
            {
                m_inMouseDown = false;
            }
        }

        public void setFontBold() {
            int bBold = -2;
            bool bBoldValue = false;

            for (int i = 0; i < m_vSelectedKeys.Length; i++) {
                cReportFont font = m_paint.getPaintObject(m_vSelectedKeys[i]).getAspect().getFont();

                if (bBold == -2) {
					bBold = font.getBold() ? -1 : 0;
                } 
				else if (bBold != (font.getBold() ? -1 : 0)) {
                    bBold = -2;
                    break;
                }                
            }

            if (bBold == -2) {
                bBoldValue = true;
            } 
            else {
                bBoldValue = bBold == 0;
            }

            CSReportPaint.cReportPaintObject paintObject = null;
            cReportControl rptCtrl = null;

            for (int i = 0; i < m_vSelectedKeys.Length; i++) {

                paintObject = m_paint.getPaintObject(m_vSelectedKeys[i]);
                rptCtrl = m_report.getControls().item(paintObject.getTag());
                rptCtrl.getLabel().getAspect().getFont().setBold(bBoldValue);
                paintObject.getAspect().getFont().setBold(bBoldValue);
            }

            m_dataHasChanged = true;
            refreshAll();
            pSetFontBoldValue();
        }

        public void pSetFontBoldValue() {
            int bBold = -2;

            for (int i = 0; i < m_vSelectedKeys.Length; i++) {
                cReportFont font = m_paint.getPaintObject(m_vSelectedKeys[i]).getAspect().getFont();

                if (bBold == -2) {
					bBold = font.getBold() ? -1 : 0;
                } 
				else if (bBold != (font.getBold() ? -1 : 0)) {
                    bBold = -2;
                    break;
                }
            }

			cGlobals.setEditFontBoldValue(bBold);
        }

		public void controlsAlign(CSReportGlobals.csECtlAlignConst align) {
            CSReportPaint.cReportPaintObject paintObject = null;
            cReportControl rptCtrl = null;

			float top = 0;
			float left = 0;

			float newTop = 0;
			float newLeft = 0;
			float height = 0;
			float width = 0;
			cReportAspect aspect;

            switch (align) {

				case csECtlAlignConst.csECtlAlignHeight:
				case csECtlAlignConst.csECtlAlignWidth:

                    aspect = m_paint.getPaintObject(m_vSelectedKeys[1]).getAspect();
				    height = aspect.getHeight();
                    width = aspect.getWidth();
                    break;

				case csECtlAlignConst.csECtlAlignVertical:
				case csECtlAlignConst.csECtlAlignHorizontal:

                    aspect = m_paint.getPaintObject(m_vSelectedKeys[1]).getAspect();
                    newTop = aspect.getTop();
                    newLeft = aspect.getLeft();
                    break;

                default:

                    switch (align) {
						case csECtlAlignConst.csECtlAlignLeft:
                            newLeft = 100000;
                            break;
						case csECtlAlignConst.csECtlAlignRight:
                            newLeft = 0;
                            break;
						case csECtlAlignConst.csECtlAlignTop:
                            newTop = 100000;
                            break;
						case csECtlAlignConst.csECtlAlignBottom:
                            newTop = 0;
                            break;
                    }

                    for (int i = 0; i < m_vSelectedKeys.Length; i++) {

                        aspect = m_paint.getPaintObject(m_vSelectedKeys[i]).getAspect();
                        top = aspect.getTop();
                        left = aspect.getLeft();

                        switch (align) {
							case csECtlAlignConst.csECtlAlignLeft:
                                if (left < newLeft) { newLeft = left; }
                                break;
							case csECtlAlignConst.csECtlAlignRight:
                                if (left > newLeft) { newLeft = left; }
                                break;
							case csECtlAlignConst.csECtlAlignTop:
                                if (top < newTop) { newTop = top; }
                                break;
							case csECtlAlignConst.csECtlAlignBottom:
                                if (top > newTop) { newTop = top; }
                                break;
                        }
                    }

                    break;
            }

            for (int i = 0; i < m_vSelectedKeys.Length; i++) {

                paintObject = m_paint.getPaintObject(m_vSelectedKeys[i]);
                rptCtrl = m_report.getControls().item(paintObject.getTag());

                switch (align) {

					case csECtlAlignConst.csECtlAlignHeight:
                        rptCtrl.getLabel().getAspect().setHeight(height);
                        paintObject.getAspect().setHeight(height);
                        break;

					case csECtlAlignConst.csECtlAlignWidth:
                        rptCtrl.getLabel().getAspect().setWidth(width);
                        paintObject.getAspect().setWidth(width);
                        break;

					case csECtlAlignConst.csECtlAlignLeft:
					case csECtlAlignConst.csECtlAlignRight:
					case csECtlAlignConst.csECtlAlignHorizontal:
                        rptCtrl.getLabel().getAspect().setLeft(newLeft);
                        paintObject.getAspect().setLeft(newLeft);
                        break;

					case csECtlAlignConst.csECtlAlignTop:
					case csECtlAlignConst.csECtlAlignBottom:
					case csECtlAlignConst.csECtlAlignVertical:
                        rptCtrl.getLabel().getAspect().setTop(newTop);
                        paintObject.getAspect().setTop(newTop);
                        break;
                }
            }

            m_dataHasChanged = true;
            refreshAll();
        }

		public void textAlign(CSReportGlobals.HorizontalAlignment align) {
            CSReportPaint.cReportPaintObject paintObject = null;
            cReportControl rptCtrl = null;

            for (int i = 0; i < m_vSelectedKeys.Length; i++) {

                paintObject = m_paint.getPaintObject(m_vSelectedKeys[i]);
                rptCtrl = m_report.getControls().item(paintObject.getTag());

				rptCtrl.getLabel().getAspect().setAlign(align);
				paintObject.getAspect().setAlign(align);
            }

            m_dataHasChanged = true;
            refreshAll();
            pSetEditAlignValue();
        }

        private void pSetEditAlignValue() {
            int align = -1;

            for (int i = 0; i < m_vSelectedKeys.Length; i++) {
                CSReportDll.cReportAspect aspect = m_paint.getPaintObject(m_vSelectedKeys[i]).getAspect();

                if (align == -1) {
					align = (int)aspect.getAlign();
                } 
				else if (align != (int)aspect.getAlign()) {
                    align = -2;
                    break;
                }
            }
			cGlobals.setEditAlignValue(align);
        }

        private void pAddToSelected(String sKey, bool ctrlKey, out bool bWasRemoved) {

			bWasRemoved = false;
            if (sKey == "") { return; }

            bWasRemoved = false;

            if (ctrlKey) {

                for (int i = 0; i < m_vSelectedKeys.Length; i++) {

                    if (m_vSelectedKeys[i] == sKey) {
                        pRemoveFromSelected(sKey);
                        bWasRemoved = true;
                        return;
                    }
                }
            } 
            else {
                if (pAllreadySelected(sKey)) { return; }
            }

			G.redimPreserve(ref m_vSelectedKeys, m_vSelectedKeys.Length + 1);
			m_vSelectedKeys[m_vSelectedKeys.Length - 1] = sKey;
        }

        private bool pAllreadySelected(String sKey) {
            if (sKey == "") {
                return true;
            }

            for (int i = 0; i < m_vSelectedKeys.Length; i++) {
                if (m_vSelectedKeys[i] == sKey) {
                    return true;
                }
            }
            return false;
        }

        private void pRemoveFromSelected(String sKey) {
            int i;

            for (i = 0; i < m_vSelectedKeys.Length; i++) {
                if (m_vSelectedKeys[i] == sKey) {
                    break;
                }
            }

            if (i > m_vSelectedKeys.Length) { return; }
            for (i = i + 1; i < m_vSelectedKeys.Length; i++) {
                m_vSelectedKeys[i - 1] = m_vSelectedKeys[i];
            }
            if (m_vSelectedKeys.Length > 0) {
				G.redimPreserve(ref m_vSelectedKeys, m_vSelectedKeys.Length - 1);
            } 
            else {
				G.redim(ref m_vSelectedKeys, 0);
            }

            m_paint.removeFromSelected(sKey, m_picReport.CreateGraphics());
        }

        private bool pClearSelected(MouseButtons button, bool ctrlKey, float x, float y) {
            String sKey = "";

            if (!ctrlKey && button != MouseButtons.Right) {
				m_paint.pointIsInObject(x, y, ref sKey);
                for (int i = 0; i < m_vSelectedKeys.Length; i++) {
                    if (m_vSelectedKeys[i] == sKey) {
                        return false;
                    }
                }
				G.redim(ref m_vSelectedKeys, 0);
                return true;
            }
            return false;
        }

        private void pShowMoveAll(float x, float y) {
            int i = 0;
			float offsetTop = 0;
			float offsetLeft = 0;
			float firstLeft = 0;
			float firstTop = 0;
            bool clear = false;
			float offSet2 = 0;

            if (m_vSelectedKeys.Length == 0) { return; }

            cReportAspect aspect = m_paint.getPaintObject(m_keyMoving).getAspect();
			firstLeft = aspect.getLeft();
			firstTop = aspect.getTop();

            clear = true;

            for (i = m_vSelectedKeys.Length-1; i > -1; i--) {

                aspect = m_paint.getPaintObject(m_vSelectedKeys[i]).getAspect();
                offsetLeft = pGetOffsetLeftFromControls(firstLeft, aspect.getLeft());
                offsetTop = pGetOffsetTopFromControls(firstTop, aspect.getTop());
				offSet2 = aspect.getOffset();

                if (m_bMoveHorizontal) {
                    m_paint.moveObjToXYEx(m_keyMoving, 
                                            x - m_offX + offsetLeft, 
                                            firstTop - offSet2 + offsetTop,
                                            m_picReport.CreateGraphics(), 
                                            clear);
                } 
                else if (m_bMoveVertical) {
                    m_paint.moveObjToXYEx(m_keyMoving, 
                                            firstLeft + offsetLeft, 
                                            y - m_offY + offsetTop,
                                            m_picReport.CreateGraphics(), 
                                            clear);
                } 
                else {
                    m_paint.moveObjToXYEx(m_keyMoving, 
                                            x - m_offX + offsetLeft, 
                                            y - m_offY + offsetTop,
                                            m_picReport.CreateGraphics(), 
                                            clear);
                }

                if (clear) { clear = false; }
            }
        }

        private void m_picReport_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (m_paint == null) return;

            MouseButtons button = e.Button;
            int x = e.X;
            int y = e.Y;

            String sKey = "";
			csRptPaintRegionType rgnTp = csRptPaintRegionType.CRPTPNTRGNTYPEBODY;

            if (m_draging) { return; }

            if (m_inMouseDown) { return; }

            if (button == MouseButtons.Left) {

                m_paint.beginMove();

                if (m_keyMoving != "") {

                    switch (m_moveType) {
                        case csRptEditorMoveType.CSRPTEDMOVTALL:
                            pShowMoveAll(x, y);
                            break;
                        case csRptEditorMoveType.CSRPTEDMOVTHORIZONTAL:
                            m_paint.moveHorizontal(m_keyMoving, x, m_picReport.CreateGraphics());
                            break;
                        case csRptEditorMoveType.CSRPTEDMOVTVERTICAL:
                            m_paint.moveVertical(m_keyMoving, y, m_picReport.CreateGraphics());
                            break;
                    }

                    m_moving = true;

                } 
                else if (m_keySizing != "") {
                    switch (m_moveType) {
                        case csRptEditorMoveType.CSRPTEDMOVDOWN:
                            m_paint.resize(m_picReport.CreateGraphics(), m_keySizing, cGlobals.C_NO_CHANGE, cGlobals.C_NO_CHANGE, cGlobals.C_NO_CHANGE, y);
                            break;
                        case csRptEditorMoveType.CSRPTEDMOVLEFT:
                            m_paint.resize(m_picReport.CreateGraphics(), m_keySizing, x, cGlobals.C_NO_CHANGE, cGlobals.C_NO_CHANGE, cGlobals.C_NO_CHANGE);
                            break;
                        case csRptEditorMoveType.CSRPTEDMOVRIGHT:
                            m_paint.resize(m_picReport.CreateGraphics(), m_keySizing, cGlobals.C_NO_CHANGE, cGlobals.C_NO_CHANGE, x, cGlobals.C_NO_CHANGE);
                            break;
                        case csRptEditorMoveType.CSRPTEDMOVUP:
                            m_paint.resize(m_picReport.CreateGraphics(), m_keySizing, cGlobals.C_NO_CHANGE, y, cGlobals.C_NO_CHANGE, cGlobals.C_NO_CHANGE);
                            break;
                        case csRptEditorMoveType.CSRPTEDMOVLEFTDOWN:
                            m_paint.resize(m_picReport.CreateGraphics(), m_keySizing, x, cGlobals.C_NO_CHANGE, cGlobals.C_NO_CHANGE, y);
                            break;
                        case csRptEditorMoveType.CSRPTEDMOVLEFTUP:
                            m_paint.resize(m_picReport.CreateGraphics(), m_keySizing, x, y, cGlobals.C_NO_CHANGE, cGlobals.C_NO_CHANGE);
                            break;
                        case csRptEditorMoveType.CSRPTEDMOVRIGHTDOWN:
                            m_paint.resize(m_picReport.CreateGraphics(), m_keySizing, cGlobals.C_NO_CHANGE, cGlobals.C_NO_CHANGE, x, y);
                            break;
                        case csRptEditorMoveType.CSRPTEDMOVRIGHTUP:
                            m_paint.resize(m_picReport.CreateGraphics(), m_keySizing, cGlobals.C_NO_CHANGE, y, x, cGlobals.C_NO_CHANGE);
                            break;
                    }
                    m_moving = true;
                } 
                else {
                    m_moving = false;
                }
            } 
            else {
                if (m_keyFocus != "") {
                    sKey = m_keyFocus;
					if (m_paint.pointIsInThisObject(x, y, ref m_keyFocus, ref rgnTp)) {
                        CSReportPaint.cReportPaintObject po = m_paint.getPaintObject(sKey);

                        cReportControl ctrl = m_report.getControls().item(po.getTag());
                        pSetSbPnlCtrl(
							ctrl.getName(), 
							ctrl.getControlType(), 
							ctrl.getFormulaHide().getText(), 
							ctrl.getFormulaValue().getText(), 
							ctrl.getHasFormulaHide(), 
							ctrl.getHasFormulaValue(), 
							ctrl.getField().getName());

						if (po.getPaintType() == csRptPaintObjType.CSRPTPAINTOBJLINE) {
                            m_keyMoving = sKey;
                            m_keySizing = "";
                            m_picReport.Cursor = Cursors.SizeNS;
                        } 
                        else {
                            switch (po.getTag()) {
                                case cGlobals.C_KEY_DETAIL:
                                case cGlobals.C_KEY_FOOTER:
                                case cGlobals.C_KEY_HEADER:                                        
                                    m_keyMoving = sKey;
                                    m_keySizing = "";
                                    m_picReport.Cursor = Cursors.SizeNS;
                                    m_moveType = csRptEditorMoveType.CSRPTEDMOVTVERTICAL;
                                    break;

                                default:

								if (po.getRptType() == csRptTypeSection.DETAIL 
									|| po.getRptType() == csRptTypeSection.HEADER 
									|| po.getRptType() == csRptTypeSection.GROUP_HEADER 
                                        || po.getRptType() == csRptTypeSection.GROUP_FOOTER 
									|| po.getRptType() == csRptTypeSection.FOOTER) {

                                        m_keyMoving = sKey;
                                        m_keySizing = "";
                                        m_picReport.Cursor = Cursors.SizeNS;
                                        m_moveType = csRptEditorMoveType.CSRPTEDMOVTVERTICAL;
                                    } 
                                    else {

                                        switch (rgnTp) {
									case csRptPaintRegionType.CRPTPNTRGNTYPEBODY:
                                                m_picReport.Cursor = Cursors.SizeAll;
                                                m_keyMoving = sKey;
                                                m_keySizing = "";
                                                m_moveType = csRptEditorMoveType.CSRPTEDMOVTALL;
                                                break;

                                            case csRptPaintRegionType.CRPTPNTRGNTYPEDOWN:
                                                m_picReport.Cursor = Cursors.SizeNS;
                                                m_keySizing = sKey;
                                                m_keyMoving = "";
                                                m_moveType = csRptEditorMoveType.CSRPTEDMOVDOWN;
                                                break;

                                            case csRptPaintRegionType.CRPTPNTRGNTYPEUP:
                                                m_picReport.Cursor = Cursors.SizeNS;
                                                m_keySizing = sKey;
                                                m_keyMoving = "";
                                                m_moveType = csRptEditorMoveType.CSRPTEDMOVUP;
                                                break;

                                            case csRptPaintRegionType.CRPTPNTRGNTYPELEFT:
										        m_picReport.Cursor = Cursors.SizeWE;
                                                m_keySizing = sKey;
                                                m_keyMoving = "";
                                                m_moveType = csRptEditorMoveType.CSRPTEDMOVLEFT;
                                                break;

                                            case csRptPaintRegionType.CRPTPNTRGNTYPERIGHT:
                                                m_picReport.Cursor = Cursors.SizeWE;
                                                m_keySizing = sKey;
                                                m_keyMoving = "";
                                                m_moveType = csRptEditorMoveType.CSRPTEDMOVRIGHT;
                                                break;

                                            case csRptPaintRegionType.CRPTPNTRGNTYPELEFTDOWN:
                                                m_picReport.Cursor = Cursors.SizeNESW;
                                                m_keySizing = sKey;
                                                m_keyMoving = "";
                                                m_moveType = csRptEditorMoveType.CSRPTEDMOVLEFTDOWN;
                                                break;

                                            case csRptPaintRegionType.CRPTPNTRGNTYPERIGHTUP:
                                                m_picReport.Cursor = Cursors.SizeNESW;
                                                m_keySizing = sKey;
                                                m_keyMoving = "";
                                                m_moveType = csRptEditorMoveType.CSRPTEDMOVRIGHTUP;
                                                break;

                                            case csRptPaintRegionType.CRPTPNTRGNTYPERIGHTDOWN:
                                                m_picReport.Cursor = Cursors.SizeNWSE;
                                                m_keySizing = sKey;
                                                m_keyMoving = "";
                                                m_moveType = csRptEditorMoveType.CSRPTEDMOVRIGHTDOWN;
                                                break;

                                            case csRptPaintRegionType.CRPTPNTRGNTYPELEFTUP:
                                                m_picReport.Cursor = Cursors.SizeNWSE;
                                                m_keySizing = sKey;
                                                m_keyMoving = "";
                                                m_moveType = csRptEditorMoveType.CSRPTEDMOVLEFTUP;
                                                break;

                                            default:
                                                m_keySizing = "";
                                                m_keyMoving = "";
                                                break;
                                        }
                                    }
                                    break;
                            }
                        }
                    } 
                    else {
                        pSetSbPnlCtrl("");
                        m_picReport.Cursor = Cursors.Default;
                        m_keySizing = "";
                        m_keyMoving = "";
                    }
                }

				if (m_paint.pointIsInObject(x, y, ref sKey, ref rgnTp)) {
                    CSReportPaint.cReportPaintObject po = m_paint.getPaintObject(sKey);
                    if (po.getRptType() == csRptTypeSection.CONTROL) {
                        cReportControl rptCtrl = null;
                        rptCtrl = m_report.getControls().item(po.getTag());
                        if (rptCtrl != null) {
                            pSetSbPnlCtrl(rptCtrl.getName(), 
                                            rptCtrl.getControlType(), 
                                            rptCtrl.getFormulaHide().getText(), 
                                            rptCtrl.getFormulaValue().getText(), 
                                            rptCtrl.getHasFormulaHide(), 
                                            rptCtrl.getHasFormulaValue(), 
                                            rptCtrl.getField().getName());
                        }
                    } 
                    else {
                        pSetSbPnlCtrl("");
                    }
                } 
                else {
                    pSetSbPnlCtrl("");
                }
            }
        }

		private void pSetSbPnlCtrl(String ctrlName)
		{
			pSetSbPnlCtrl (ctrlName, csRptControlType.CSRPTCTLABEL, "", "", false, false, "");
		}

        private void pSetSbPnlCtrl(
            String ctrlName, 
            csRptControlType ctrlType, 
            String formulaHide, 
            String formulaValue, 
            bool hasFormulaHide, 
            bool hasFormulaValue, 
            String fieldName) 
        {

            String msg = "";
            String strCtlType = "";

            switch (ctrlType) {
                case csRptControlType.CSRPTCTDBIMAGE:
                    strCtlType = "DbImage";
                    break;
                case csRptControlType.CSRPTCTFIELD:
                    strCtlType = "Field";
                    break;
                case csRptControlType.CSRPTCTIMAGE:
                    strCtlType = "Image";
                    break;
                case csRptControlType.CSRPTCTLABEL:
                    strCtlType = "Label";
                    break;
            }

            if (ctrlName != "") {
                msg = "Ctl:[" + ctrlName 
                    + "]Tipo:[" + strCtlType 
					+ "]F.Hide:[" + cUtil.subString(formulaHide, 1, 100) 
                    + "]Activa[" + ((bool) hasFormulaHide).ToString()
                    + "]F.Value:[" + cUtil.subString(formulaValue, 1, 100) 
                    + "]Activa[" + ((bool) hasFormulaValue).ToString() 
                    + "]Field:[" + fieldName + "]";
            }
            m_fmain.setsbPnlCtrl(msg);
        }

        private void m_picReport_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (m_paint == null) return;

            MouseButtons button = e.Button;
            int x = e.X;
            int y = e.Y;

            // to avoid reentrancy
            if (m_opening) { return; }

            //----------------------------------------------------
            // MOVING
            //----------------------------------------------------

            if (m_moving) {
                if (m_keyMoving != "") {
                    switch (m_moveType) {
                        case csRptEditorMoveType.CSRPTEDMOVTALL:
                            if (m_bMoveVertical) {
                                pMoveAll(C_NOMOVE, y);
                            } 
                            else if (m_bMoveHorizontal) {
                                pMoveAll(x, C_NOMOVE);
                            } 
                            else {
                                pMoveAll(x, y);
                            }
                            break;

                        case csRptEditorMoveType.CSRPTEDMOVTHORIZONTAL:
                            pMoveHorizontal(x);
                            break;

                        case csRptEditorMoveType.CSRPTEDMOVTVERTICAL:
                            pMoveVertical(x, y);
                            break;
                    }

                //----------------------------------------------------
                // SIZING
                //----------------------------------------------------
                } 
                else if (m_keySizing != "") {
                    pResizeControl(x, y);
                }

                refreshBody();
                m_moving = false;
                refreshRule();
            }

            m_keySizing = "";
            m_keyMoving = "";
        }

        private void m_picReport_Paint(object sender, PaintEventArgs e)
        {
            if (m_paint != null)
            {
                m_paint.paintPicture(e.Graphics, false);                
            }            
        }

        private void m_picRule_Paint(object sender, PaintEventArgs e)
        {
            if (m_paint != null)
            {
                CSReportPaint.cReportPaintObjects ps = m_paint.getPaintSections();
                for (int i = 0; i < ps.count(); i++)
                {
                    m_paint.drawRule(ps.getNextKeyForZOrder(i), e.Graphics);
                }
            }
        }

        public void setParameters() {
            CSConnect.cConnect connect = new CSConnect.cConnect();
            cParameter param = null;

			for (int _i = 0; _i < m_report.getConnect().getParameters().count(); _i++) {
				param = m_report.getConnect().getParameters().item(_i);
				CSConnect.cParameter connectParam = connect.getParameters().add(null, "");
				connectParam.setName(param.getName());
				connectParam.setValue(param.getValue());
            }

			if (m_report.getConnect().getDataSource() == "") {
                cWindow.msgWarning("Before editting the parameter info you must define a connection");
                return;
            }

			connect.setStrConnect(m_report.getConnect().getStrConnect());
			connect.setDataSource(m_report.getConnect().getDataSource());
			connect.setDataSourceType(m_report.getConnect().getDataSourceType());

			if (!connect.getDataSourceColumnsInfo(m_report.getConnect().getDataSource(), 
				m_report.getConnect().getDataSourceType())) 
            	return;

			cGlobals.setParametersAux(connect, m_report.getConnect());
        }

        public void setSimpleConnection() {
            fSimpleConnect f = new fSimpleConnect();
            try {

                String strConnect = "";
				strConnect = m_report.getConnect().getStrConnect();
                f.setServer(cUtil.getToken("Data Source", strConnect));
                f.setDataBase(cUtil.getToken("Initial Catalog", strConnect));
                f.setUser(cUtil.getToken("User ID", strConnect));
                f.setPassword(cUtil.getToken("Password", strConnect));
                if (f.getUser() == "") {
                    f.setConnectTypeToNT();
                } 
                else {
                    f.setConnectTypeToSQL();
                }
                f.ShowDialog();

                if (!f.getOk()) { 
                    f.Close();
                }
                else {
					m_report.getConnect().setStrConnect(f.getStrConnect());
                }

            } catch (Exception ex) {
                cError.mngError(ex, "configConnection", C_MODULE, "");
                f.Close();
            }
        }

        public bool configConnection(cReportConnect rptConnect) {
            try {

                CSConnect.cConnect connect = new CSConnect.cConnect();

				if (!connect.showOpenConnection())
					return false;

                refreshAll();

                if (!connect.getDataSourceColumnsInfo(
					connect.getDataSource(), 
					connect.getDataSourceType())) {
                    return false;
                }

                if (rptConnect == null) {
					cGlobals.setParametersAux(connect, m_report.getConnect());
                } 
                else {
                    cGlobals.setParametersAux(connect, rptConnect);
                }

                if (cMainEditor.getToolbox(this) != null) { showToolbox(); }

                return true;

            } catch (Exception ex) {
                cError.mngError(ex, "configConnection", C_MODULE, "");
                return false;
            }
        }

        public void setAllConnectToMainConnect() {
            try {

                cReportConnect connect = null;
				for (int _i = 0; _i < m_report.getConnectsAux().count(); _i++) {
					connect = m_report.getConnectsAux().item(_i);
                    connect.setStrConnect(m_report.getConnect().getStrConnect());
                }

            } catch (Exception ex) {
                cError.mngError(ex, "setAllConnectToMainConnect", C_MODULE, "");
            }
        }

        public void deleteObj(bool bDelSectionLine) {
            cReportSection sec = null;
            cReportSections secs = null;
            cReportSectionLine secLn = null;
            cReportControl ctrl = null;
            CSReportPaint.cReportPaintObject paintObj = null;

            bool isGroupFooter = false;
            bool isGroupHeader = false;
            bool isSecLn = false;

            if (m_keyFocus == "") { return; }

            cReportGroup group = null;
            cReportSection secG = null;
            if (m_paint.paintObjIsSection(m_keyFocus)) {
                if (m_paint.getPaintSections().item(m_keyFocus) == null) { return; }

                CSReportPaint.cReportPaintObject po = m_paint.getPaintSections().item(m_keyFocus);

                // first we check it is not a section line
                //
                sec = pGetSection(out isSecLn, out secLn, false, out isGroupHeader, out isGroupFooter);
                if (!isSecLn) {

                    // check it is not the last section line in this section
                    //
                    if (bDelSectionLine) {

                        sec = pGetSection(out isSecLn, out secLn, true, out isGroupHeader, out isGroupFooter);
                    }
					if (!pCanDeleteSection(out secs, sec, po.getTag())) { return; }
                }

                String what = "";

                if (isSecLn) {
                    what = "the section line";
                } 
                else {
                    what = "the section";
                }

                if (!cWindow.ask("Are yuo sure you want to delete "
                            + what + " and all the controls it contains? ", MessageBoxDefaultButton.Button2))
                {
                    return;
                }

                if (isSecLn) {

					for (int _i = 0; _i < secLn.getControls().count(); _i++) {
						ctrl = secLn.getControls().item(_i);
                        for (int i = 0; i < m_paint.getPaintObjects().count(); i++) {
                            paintObj = m_paint.getPaintObjects().item(i);
                            if (paintObj.getTag() == ctrl.getKey()) {
                                m_paint.getPaintObjects().remove(paintObj.getKey());
                                break;
                            }
                        }
                    }

                    secLn.getControls().clear();

                    // at least one section line has to be in the section
                    //
                    if (sec.getSectionLines().count() > 1) {
                        sec.getSectionLines().remove(secLn.getKey());
                    }

                } 
                else {

					for (int _i = 0; _i < sec.getSectionLines().count(); _i++) {
						secLn = sec.getSectionLines().item(_i);
						for (int _j = 0; _j < secLn.getControls().count(); _j++) {
                            ctrl = secLn.getControls().item(_j);
                            for (int i = 0; i < m_paint.getPaintObjects().count(); i++) {
                                paintObj = m_paint.getPaintObjects().item(i);
                                if (paintObj.getTag() == ctrl.getKey()) {
                                    m_paint.getPaintObjects().remove(paintObj.getKey());
                                    break;
                                }
                            }
                        }
                    }

                    // if this is a group section we need to delete the header and the footer
                    // 

                    if (isGroupFooter || isGroupHeader) {
                        if (isGroupHeader) {
							for (int _i = 0; _i < m_report.getGroups().count(); _i++) {
								group = m_report.getGroups().item(_i);
                                if (group.getHeader().getKey() == sec.getKey()) { break; }
                            }
                            secG = group.getFooter();
                        } 
                        else if (isGroupFooter) {
							for (int _i = 0; _i < m_report.getGroups().count(); _i++) {
								group = m_report.getGroups().item(_i);
                                if (group.getFooter().getKey() == sec.getKey()) { break; }
                            }
                            secG = group.getHeader();
                        }

						for (int _i = 0; _i < secG.getSectionLines().count(); _i++) {
							secLn = secG.getSectionLines().item(_i);
							for (int _j = 0; _j < secLn.getControls().count(); _j++) {
                                ctrl = secLn.getControls().item(_j);
                                for (int i = 0; i < m_paint.getPaintObjects().count(); i++) {
                                    paintObj = m_paint.getPaintObjects().item(i);
                                    if (paintObj.getTag() == ctrl.getKey()) {
                                        m_paint.getPaintObjects().remove(paintObj.getKey());
                                        break;
                                    }
                                }
                            }
                        }

                        for (int i = 0; i < m_paint.getPaintSections().count(); i++) {
                            paintObj = m_paint.getPaintSections().item(i);
                            if (paintObj.getTag() == secG.getKey()) {
                                m_paint.getPaintSections().remove(paintObj.getKey());
                                break;
                            }
                        }

                        m_report.getGroups().remove(group.getIndex());

                    } 
                    else {
                        secs.remove(sec.getKey());
                    }

                }

                bool bDeletePaintObj = false;

                bDeletePaintObj = true;
                if (isSecLn) {
                    bDeletePaintObj = sec.getKeyPaint() != m_keyFocus;
                }

                if (bDeletePaintObj) {

                    m_paint.getPaintSections().remove(m_keyFocus);

                    // if I have deleted the last section line in this 
                    // section I need to delete the paint object
                    // asociated with the current last section line
                    // and then to asociate this section line with
                    // the paint object of the section
                } 
                else {
					cReportSectionLines secLns = sec.getSectionLines();
					m_paint.getPaintSections().remove(secLns.item(secLns.count() - 1).getKeyPaint());
					secLns.item(secLns.count() - 1).setKeyPaint(sec.getKeyPaint());
                }

                pResetKeysFocus();
				G.redim(ref m_vSelectedKeys, 0);
            } 
            else {
                paintObj = m_paint.getPaintObjects().item(m_keyFocus);
                if (paintObj == null) { return; }

                if (!cWindow.ask("Confirm you want to delete the control? ", MessageBoxDefaultButton.Button2)) { return; }

                for (int i = 0; i < m_vSelectedKeys.Length; i++) {
                    paintObj = m_paint.getPaintObjects().item(m_vSelectedKeys[i]);
                    ctrl = m_report.getControls().item(paintObj.getTag());

                    m_paint.getPaintObjects().remove(paintObj.getKey());
                    if (ctrl == null) { return; }
                    ctrl.getSectionLine().getControls().remove(ctrl.getKey());
                }

                pResetKeysFocus();
				G.redim(ref m_vSelectedKeys, 0);
            }

            refreshAll();
        }

        private bool pCanDeleteSection(
            out cReportSections secs, 
            cReportSection sec, 
            String tag) 
        { 
            cReportSection secAux = null;
            
            // header
            //
            secAux = m_report.getHeaders().item(tag);
            secs = null;

            if (secAux != null) {
                if (secAux.Equals(sec) || sec == null) {
                    if (secAux.getTypeSection() == csRptTypeSection.MAIN_HEADER) {
                        cWindow.msgInfo("The main header can't be deleted");
                        return false;
                    }
                    secs = m_report.getHeaders();
                }
            } 
            // if we don't find the section yet
            //
            if (secs == null) {

                // footers
                //
                secAux = m_report.getFooters().item(tag);
                if (secAux != null) {
                    if (secAux.Equals(sec) || sec == null) {
                        if (secAux.getTypeSection() == csRptTypeSection.MAIN_FOOTER) {
                            cWindow.msgInfo("The main footer can't be deleted");
                            return false;
                        }
                        secs = m_report.getFooters();
                    }
                } 
                // if we don't find the section yet
                //
                if (secs == null) {

                    // check for groups
                    //
                    secAux = m_report.getGroupsHeaders().item(tag);
                    if (secAux != null) {
                        if (!((secAux.Equals(sec) || sec == null))) {

                            secAux = m_report.getGroupsFooters().item(tag);
                            if (secAux != null) {
                                if (!((secAux.Equals(sec) || sec == null))) {

                                    // finally the detail section can't be deleted
                                    //
                                    cWindow.msgInfo("The detail section can't be deleted");
                                    return false;
                                }
                            }
                        }
                    }
                }
            }

            return true;
        }

        private void pResetKeysFocus() {
            m_keyFocus = "";
            m_keyMoving = "";
            m_keySizing = "";
            m_picReport.Cursor = Cursors.Default;
        }

        public void addDBField() {
            String sField = "";
            int nIndex = 0;
            int nFieldType = 0;

			if (!cGlobals.showDbFields(ref sField, ref nFieldType, ref nIndex, this))
				return;

            beginDraging();
            m_controlName = "";
            m_controlType = csRptEditCtrlType.CSRPTEDITFIELD;
            m_fieldName = sField;
            m_formulaText = "";
            m_fieldIndex = nIndex;
            m_fieldType = nFieldType;
        }

        public void addLabel() {
            pAddLabelAux(csRptEditCtrlType.CSRPTEDITLABEL);
        }

        public void addImage() {
            pAddLabelAux(csRptEditCtrlType.CSRPTEDITIMAGE);
        }

        public void addChart() {
            pAddLabelAux(csRptEditCtrlType.CSRPTEDITCHART);
        }

        public void pAddLabelAux(csRptEditCtrlType ctrlType) {
            beginDraging();
            m_controlName = "";
            m_controlType = ctrlType;
            m_fieldName = "";
            m_formulaText = "";
            m_fieldIndex = 0;
            m_fieldType = 0;
        }

		private bool addControlEnd(float left, float top) {

            m_draging = false;

            if (m_controlType == csRptEditCtrlType.CSRPTEDITNONE) {
                return true;
            }

            m_dataHasChanged = true;

			float originalLeft = 0;
			float originalTop = 0;
            cReportControl copyCtrl = null;
            cReportControl movedCtrl = null;
			float firstCtrlLeft = 0;
			float offSet = 0;

            if (m_copyControls) {

                if (m_vCopyKeys.Length == 0) { return false; }

                originalLeft = left;
                originalTop = top;

                String keyPaint = m_vCopyKeys[m_vCopyKeys.Length - 1];
				String keyCtrl = m_paint.getPaintObjects().item(keyPaint).getTag();
				movedCtrl = m_report.getControls().item(keyCtrl);
                firstCtrlLeft = movedCtrl.getLabel().getAspect().getLeft();

                for (int i = m_vCopyKeys.Length-1; i > -1; i--) {

                    keyPaint = m_vCopyKeys[i];
					keyCtrl = m_paint.getPaintObjects().item(keyPaint).getTag();
					copyCtrl = m_report.getControls().item(keyCtrl);

                    // starting with the first control we move the left
                    // of every control if reach the right margin 
                    // move down a line and restart
                    //
                    offSet = pGetOffsetLeftFromControls(firstCtrlLeft, copyCtrl.getLabel().getAspect().getLeft());
                    left = originalLeft + offSet;

                    if (m_bCopyWithoutMoving) {

                        top = copyCtrl.getLabel().getAspect().getTop();
                        left = copyCtrl.getLabel().getAspect().getLeft();

                    }

                    if (left - 400 > m_picReport.Width) {
                        left = originalLeft + (offSet % originalLeft);
                        top += 100;
                    }

                    if (top > m_picReport.Height) {
                        top = m_picReport.Height - 100;
                    }

                    pAddControlEndAux(left, top, copyCtrl);

                }
                m_copyControls = false;

            } 
            else if (m_copyControlsFromOtherReport) {

                if (m_fmain.getReportCopySource() == null) { return false; }

                originalLeft = left;
                originalTop = top;

                cEditor editor = m_fmain.getReportCopySource();
                String keyPaint = editor.getVCopyKeys(editor.getVCopyKeysCount());
				String keyCtrl = editor.getPaint().getPaintObjects().item(keyPaint).getTag();
				movedCtrl = editor.getReport().getControls().item(keyCtrl);
                firstCtrlLeft = movedCtrl.getLabel().getAspect().getLeft();

                for (int i = editor.getVCopyKeysCount()-1; i > -1; i--) {

                    keyPaint = editor.getVCopyKeys(i);
					keyCtrl = editor.getPaint().getPaintObjects().item(keyPaint).getTag();
					copyCtrl = editor.getReport().getControls().item(keyCtrl);

                    // starting with the first control we move the left
                    // of every control if reach the right margin 
                    // move down a line and restart
                    //
                    offSet = pGetOffsetLeftFromControls(firstCtrlLeft, copyCtrl.getLabel().getAspect().getLeft());
                    left = originalLeft + offSet;

                    if (m_bCopyWithoutMoving) {

                        top = copyCtrl.getLabel().getAspect().getTop();
                        left = copyCtrl.getLabel().getAspect().getLeft();

                    }

                    if (left - 400 > m_picReport.Width) {
                        left = originalLeft + (offSet % originalLeft);
                        top = top + 100;
                    }

                    if (top > m_picReport.Height) {
                        top = m_picReport.Height - 100;
                    }

                    pAddControlEndAux(left, top, copyCtrl);
                }

                m_copyControlsFromOtherReport = false;

            } 
            else {
                pAddControlEndAux(left, top, null);
            }

            refreshBody();

            return true;
        }

		private float pGetOffsetLeftFromControls(float leftCtrl1, float leftCtrl2) {
            return leftCtrl2 - leftCtrl1;
        }

		private float pGetOffsetTopFromControls(float topCtrl1, float topCtrl2) {
            return topCtrl2 - topCtrl1;
        }

		private void pAddControlEndAux(float left, float top, cReportControl baseControl) {
            cReportControl ctrl = null;

            // first we add a control in the main header
            // after the user complete the add operation
            // we would move the control to the desired
            // section line
            //
			ctrl = m_report.getHeaders().item(cGlobals.C_KEY_HEADER).getSectionLines().item(0).getControls().add();

            // later we will set the properties related to the type of the control
            //
            m_nextNameCtrl = m_nextNameCtrl + 1;
            ctrl.setName(cGlobals.C_CONTROL_NAME + m_nextNameCtrl);

            if (baseControl == null) {
                pSetNewControlProperties(ctrl);
            } 
            else {
                pCopyControl(baseControl, ctrl);
            }

            pSetNewControlPosition(ctrl, left, top);
        }

        private void pCopyFont(cReportFont fromFont, cReportFont toFont) {
            toFont.setBold(fromFont.getBold());
            toFont.setForeColor(fromFont.getForeColor());
            toFont.setItalic(fromFont.getItalic());
            toFont.setName(fromFont.getName());
            toFont.setSize(fromFont.getSize());
            toFont.setStrike(fromFont.getStrike());
			toFont.setUnderline(fromFont.getUnderline());
        }

        /* TODO: it must be removed
        private void pCopyFontPaint(cReportFont fromFont, cReportFont toFont) { 
            toFont.setBold(fromFont.getBold());
            toFont.setForeColor(fromFont.getForeColor());
            toFont.setItalic(fromFont.getItalic());
            toFont.setName(fromFont.getName());
            toFont.setSize(fromFont.getSize());
            toFont.setStrike(fromFont.getStrike());
            toFont.setUnderline(fromFont.getUnderLine());
        }
         */

        private void pCopyChart(cReportChart fromChart, cReportChart toChart) {
            toChart.setChartTitle(fromChart.getChartTitle());
            toChart.setChartType(fromChart.getChartType());
            toChart.setDiameter(fromChart.getDiameter());
            toChart.setFormat(fromChart.getFormat());
            toChart.setGridLines(fromChart.getGridLines());
            toChart.setOutlineBars(fromChart.getOutlineBars());
            toChart.setShowValues(fromChart.getShowValues());
            toChart.setThickness(fromChart.getThickness());
            toChart.setTop(fromChart.getTop());
            toChart.setGroupFieldName(fromChart.getGroupFieldName());
            toChart.setGroupFieldIndex(fromChart.getGroupFieldIndex());
            toChart.setGroupValue(fromChart.getGroupValue());
            toChart.setSort(fromChart.getSort());

            cReportChartSerie fromSerie = null;

			for (int _i = 0; _i < fromChart.getSeries().count(); _i++) {
                fromSerie = fromChart.getSeries().item(_i);
                cReportChartSerie serie = toChart.getSeries().add();
                serie.setColor(fromSerie.getColor());
                serie.setLabelFieldName(fromSerie.getLabelFieldName());
				serie.setColor((csColors)fromSerie.getLabelIndex());
                serie.setValueFieldName(fromSerie.getValueFieldName());
                serie.setValueIndex(fromSerie.getValueIndex());
            }
        }

        private void pCopyAspect(cReportAspect fromAspect, cReportAspect toAspect) {
            toAspect.setAlign(fromAspect.getAlign());
            toAspect.setBackColor(fromAspect.getBackColor());
            toAspect.setBorderColor(fromAspect.getBorderColor());
            toAspect.setBorderColor3d(fromAspect.getBorderColor3d());
            toAspect.setBorderColor3dShadow(fromAspect.getBorderColor3dShadow());
            toAspect.setBorderType(fromAspect.getBorderType());
            toAspect.setBorderWidth(fromAspect.getBorderWidth());
            toAspect.setCanGrow(fromAspect.getCanGrow());
            toAspect.setFormat(fromAspect.getFormat());
            toAspect.setHeight(fromAspect.getHeight());
            toAspect.setIsAccounting(fromAspect.getIsAccounting());
            toAspect.setLeft(fromAspect.getLeft());
            toAspect.setNZOrder(fromAspect.getNZOrder());
            toAspect.setSelectColor(fromAspect.getSelectColor());
            toAspect.setSymbol(fromAspect.getSymbol());
            toAspect.setTop(fromAspect.getTop());
            toAspect.setTransparent(fromAspect.getTransparent());
            toAspect.setWidth(fromAspect.getWidth());
            toAspect.setWordWrap(fromAspect.getWordWrap());

            pCopyFont(fromAspect.getFont(), toAspect.getFont());
        }

		// TODO: this function shouldn't be needed
		//
        private void pCopyAspectToPaint(cReportAspect fromAspect, cReportAspect toAspect) { 
            toAspect.setAlign(fromAspect.getAlign());
            toAspect.setBackColor(fromAspect.getBackColor());
            toAspect.setBorderColor(fromAspect.getBorderColor());
            toAspect.setBorderColor3d(fromAspect.getBorderColor3d());
            toAspect.setBorderColor3dShadow(fromAspect.getBorderColor3dShadow());
            toAspect.setBorderType(fromAspect.getBorderType());
            toAspect.setBorderWidth(fromAspect.getBorderWidth());
            toAspect.setCanGrow(fromAspect.getCanGrow());
            toAspect.setFormat(fromAspect.getFormat());
            toAspect.setHeight(fromAspect.getHeight());
            toAspect.setIsAccounting(fromAspect.getIsAccounting());
            toAspect.setLeft(fromAspect.getLeft());
            toAspect.setNZOrder(fromAspect.getNZOrder());
            toAspect.setSelectColor(fromAspect.getSelectColor());
            toAspect.setSymbol(fromAspect.getSymbol());
            toAspect.setTop(fromAspect.getTop());
            toAspect.setTransparent(fromAspect.getTransparent());
            toAspect.setWidth(fromAspect.getWidth());
            toAspect.setWordWrap(fromAspect.getWordWrap());

            pCopyFontPaint(fromAspect.getFont(), toAspect.getFont());
        }

		private void pCopyFontPaint(cReportFont fromFont, cReportFont toFont)
		{
			toFont.setBold(fromFont.getBold());
			toFont.setForeColor(fromFont.getForeColor());
			toFont.setItalic(fromFont.getItalic());
			toFont.setName(fromFont.getName());
			toFont.setSize(fromFont.getSize());
			toFont.setStrike(fromFont.getStrike());
			toFont.setUnderline(fromFont.getUnderline());
		}

        private void pCopyControl(cReportControl fromCtrl, cReportControl toCtrl) { 
            toCtrl.setControlType(fromCtrl.getControlType());

            cReportField field = toCtrl.getField();
            field.setFieldType(fromCtrl.getField().getFieldType());
            field.setIndex(fromCtrl.getField().getIndex());
            field.setName(fromCtrl.getField().getName());

            toCtrl.getFormulaHide().setName(fromCtrl.getFormulaHide().getName());
            toCtrl.getFormulaHide().setText(fromCtrl.getFormulaHide().getText());
            toCtrl.getFormulaValue().setName(fromCtrl.getFormulaValue().getName());
            toCtrl.getFormulaValue().setText(fromCtrl.getFormulaValue().getText());

            toCtrl.setHasFormulaHide(fromCtrl.getHasFormulaHide());
            toCtrl.setHasFormulaValue(fromCtrl.getHasFormulaValue());

            pCopyAspect(fromCtrl.getImage().getAspect(), toCtrl.getImage().getAspect());

            cReportLabel label = toCtrl.getLabel();
            pCopyAspect(fromCtrl.getLabel().getAspect(), label.getAspect());
            label.setCanGrow(fromCtrl.getLabel().getCanGrow());
            label.setText(fromCtrl.getLabel().getText());

            pCopyAspect(fromCtrl.getLine().getAspect(), toCtrl.getLine().getAspect());
            pCopyChart(fromCtrl.getChart(), toCtrl.getChart());
        }

        private void pSetNewControlProperties(cReportControl ctrl) { 
			cReportAspect aspect = null;

            ctrl.getLabel().getAspect().setAlign(CSReportGlobals.HorizontalAlignment.Left);

            switch (m_controlType) {
                case csRptEditCtrlType.CSRPTEDITFIELD:
                    ctrl.setControlType(csRptControlType.CSRPTCTFIELD);
                    ctrl.getLabel().setText(m_fieldName);
                    cReportField field = ctrl.getField();
                    field.setIndex(m_fieldIndex);
                    field.setName(m_fieldName);
                    field.setFieldType(m_fieldType);

                    if (cDatabaseGlobals.isNumberField(m_fieldType))
                    {
                        aspect = ctrl.getLabel().getAspect();
					    aspect.setAlign(CSReportGlobals.HorizontalAlignment.Right);
                        aspect.setFormat("#0.00;-#0.00");
                    }
                    break;

                case csRptEditCtrlType.CSRPTEDITFORMULA:
                    ctrl.setControlType(csRptControlType.CSRPTCTLABEL);
                    ctrl.getFormulaValue().setText(m_formulaText + "(" + m_controlName + ")");
                    ctrl.setHasFormulaValue(true);
                    ctrl.getLabel().getAspect().setFormat("0.00;-0.00");
                    ctrl.getLabel().getAspect().getFont().setBold(true);
                    ctrl.getLabel().setText(ctrl.getFormulaValue().getText());
                    ctrl.getLabel().getAspect().setAlign(CSReportGlobals.HorizontalAlignment.Right);
                    break;

                case csRptEditCtrlType.CSRPTEDITLABEL:
                    ctrl.setControlType(csRptControlType.CSRPTCTLABEL);
                    ctrl.getLabel().setText(m_fieldName);
                    ctrl.getLabel().getAspect().getFont().setBold(true);

                    break;
                case csRptEditCtrlType.CSRPTEDITIMAGE:
                    ctrl.setControlType(csRptControlType.CSRPTCTIMAGE);
                    ctrl.getLabel().setText(m_fieldName);

                    break;
                case csRptEditCtrlType.CSRPTEDITCHART:
                    ctrl.setControlType(csRptControlType.CSRPTCTCHART);
                    ctrl.getLabel().setText(m_fieldName);
                    break;
            }

            const int ctrl_height = 285;
            const int ctrl_width = 2000;

			aspect = ctrl.getLabel().getAspect();
            aspect.setWidth(ctrl_width);
            aspect.setHeight(ctrl_height);
            aspect.setTransparent(true);
        }

		private void pSetNewControlPosition(cReportControl ctrl, float left, float top) {
            cReportAspect aspect = ctrl.getLabel().getAspect();
            aspect.setLeft(left);
            aspect.setTop(top);

            cReportPaintObject paintObj = null;
			csRptPaintObjType paintType = csRptPaintObjType.CSRPTPAINTOBJBOX;

            if (ctrl.getControlType() == csRptControlType.CSRPTCTIMAGE 
                || ctrl.getControlType() == csRptControlType.CSRPTCTCHART) {
                paintType = CSReportPaint.csRptPaintObjType.CSRPTPAINTOBJIMAGE;
            }

            paintObj = m_paint.getNewObject(paintType);

            aspect = ctrl.getLabel().getAspect();

			pCopyAspectToPaint(aspect, paintObj.getAspect());

            aspect.setLeft(left);
            aspect.setTop(top);

            paintObj.setText(ctrl.getLabel().getText());

            paintObj.setRptType(csRptTypeSection.CONTROL);

            paintObj.setTag(ctrl.getKey());
            ctrl.setKeyPaint(paintObj.getKey());

            // position the control in the desired section line
            //
            moveControl(paintObj.getKey());

            m_paint.drawObject(paintObj.getKey(), m_picReport.CreateGraphics());
        }
        
        public void addGroup() {
			pShowGroupProperties(null);
            refreshAll();
        }

        private cReportGroup pGetGroup(String key) {
            cReportGroup group = null;

			for (int _i = 0; _i < m_report.getGroups().count(); _i++) {
				group = m_report.getGroups().item(_i);
                if (group.getHeader().getKey() == key) { break; }
                if (group.getFooter().getKey() == key) { break; }
            }

            return group;
        }
        
        public void addSectionLine() {
            cReportSection sec = null;
			cReportAspect aspect = null;
            bool isGroup = false;

			sec = pGetSection(out isGroup);

            if (sec == null) { return; }

            switch (sec.getTypeSection()) {

                // in footers we add from top
                // it means that the first section line is the last one
                //
                case csRptTypeSection.FOOTER:
                case csRptTypeSection.MAIN_FOOTER:

                    aspect = sec.getSectionLines().add(null, "", 1).getAspect();
                    aspect.setHeight(cGlobals.C_HEIGHT_NEW_SECTION);
                    aspect.setWidth(sec.getAspect().getWidth());

                    // for new sections the top is the top of the previous section
                    // plus cGlobals.C_HEIGHT_NEW_SECTION
                    //
				    aspect.setTop(sec.getSectionLines().item(1).getAspect().getTop() - cGlobals.C_HEIGHT_NEW_SECTION);
                    break;

                default:

                    // because the height of the sections is calculated
                    // in pChangeHeightSection which is called by moveSection
                    // which is called by pAddSectionLinesAux, and on this
                    // function, the height of the section line is set with
                    // the result of substract to the height of the section
                    // the sum of every section line except the height of the
                    // last one section line, if we don't modify the height
                    // of the section the new section line will have an height
                    // of zero (actually the minimum height is 1 pixel).
                    //
                    // for this reazon we change the height of the section
                    // but this will fail because the moveSection function
                    // get the height of the section line from the difference
                    // between the height of the section and the new height
                    // which results of moving the section.
                    //
                    // To solve the problem we have this member variable 
                    // which is used to instruct moveSection to add 
                    // to the section height the size of the new section line
                    //
                    m_newSecLineOffSet = cGlobals.C_HEIGHT_NEW_SECTION;

                    aspect = sec.getSectionLines().add().getAspect();
                    aspect.setHeight(cGlobals.C_HEIGHT_NEW_SECTION);
                    aspect.setWidth(sec.getAspect().getWidth());

                    break;
            }

            aspect = sec.getAspect();
            aspect.setHeight(aspect.getHeight() + cGlobals.C_HEIGHT_NEW_SECTION);

            pAddSectionLinesAux(sec);

            // we reset this variable to zero
            //
            m_newSecLineOffSet = 0;
        }

        private void pAddSectionLinesAux(cReportSection sec) 
        {
			csRptTypeSection typeSecLn = csRptTypeSection.CONTROL;
			cReportAspect aspect = null;
            float maxBottom = 0;
            float minBottom = 0;
            int index = 0;
			float y = 0;

            switch (sec.getTypeSection()) {
                case csRptTypeSection.HEADER:
                case csRptTypeSection.MAIN_HEADER:

                    pMoveHeader(sec.getKey(), out minBottom, out maxBottom);
                    aspect = sec.getAspect();
                    y = aspect.getHeight() + aspect.getTop();
                    typeSecLn = csRptTypeSection.SECLN_HEADER;
                    index = sec.getSectionLines().count() - 1;
                    break;

                case csRptTypeSection.DETAIL:
                case csRptTypeSection.MAIN_DETAIL:

                    pMoveDetails(sec.getKey(), out minBottom, out maxBottom);
                    aspect = sec.getAspect();
                    y = aspect.getHeight() + aspect.getTop();
                    typeSecLn = csRptTypeSection.SECLN_DETAIL;
                    index = sec.getSectionLines().count() - 1;
                    break;

                case csRptTypeSection.GROUP_HEADER:

                    pMoveGroupHeader(sec.getKey(), out minBottom, out maxBottom);
                    aspect = sec.getAspect();
                    y = aspect.getHeight() + aspect.getTop();
                    typeSecLn = csRptTypeSection.SECLN_GROUPH;
                    index = sec.getSectionLines().count() - 1;
                    break;

                case csRptTypeSection.GROUP_FOOTER:

                    pMoveGroupFooter(sec.getKey(), out minBottom, out maxBottom);
                    aspect = sec.getAspect();
                    y = aspect.getHeight() + aspect.getTop();
                    typeSecLn = csRptTypeSection.SECLN_GROUPF;
                    index = sec.getSectionLines().count() - 1;
                    break;

                case csRptTypeSection.FOOTER:
                case csRptTypeSection.MAIN_FOOTER:

                    aspect = sec.getAspect();
                    aspect.setTop(aspect.getTop() - cGlobals.C_HEIGHT_NEW_SECTION);
                    pMoveFooter(sec.getKey(), out minBottom, out maxBottom);
                    m_offY = 0;
                    y = aspect.getHeight() + aspect.getTop() - m_offSet - cGlobals.C_HEIGHT_BAR_SECTION;
                    typeSecLn = csRptTypeSection.SECLN_FOOTER;
                    index = 1;
                    break;
            }
			// we add a paint object to all sectionlines except the last one 
            // the last sectionline uses the paint object of the section
            //
			cReportSectionLine secL = sec.getSectionLines().item(index);
			secL.setKeyPaint(
				paintSection(secL.getAspect(), 
								secL.getKey(), 
                                sec.getTypeSection(), 
								C_SECTIONLINE + (sec.getSectionLines().count() - 1).ToString(), 
								true));

			// section line
            CSReportPaint.cReportPaintObject po = m_paint.getPaintSections().item(secL.getKeyPaint());
			po.setRptType(typeSecLn);
			po.setRptKeySec(sec.getKey());

			// section
            po = m_paint.getPaintSections().item(sec.getKeyPaint());
			po.setTextLine(C_SECTIONLINE + (sec.getSectionLines().count() - 1).ToString());

            moveSection(m_paint.getPaintSections().item(m_keyFocus), 0, y, minBottom, maxBottom, sec, false);

            refreshBody();
            refreshRule();
        }
        
        public void addSection(csRptTypeSection typeSection) {

			if (!m_editor.Visible)
				return;

            cReportSection rptSection = null;
            cReportSection topSec = null;
			cReportAspect w_aspect = null;
			cReportAspect aspect = null;
            CSReportPaint.cReportPaintObject paintObj = null;

            float maxBottom = 0;
            float minBottom = 0;
            float y = 0;

            switch (typeSection) {
                case csRptTypeSection.HEADER:
                    cReportSections w_headers = m_report.getHeaders();
				    rptSection = w_headers.add();
                    rptSection.setName("H_" + rptSection.getIndex().ToString());
				    aspect = w_headers.item(w_headers.count() - 2).getAspect();
                    rptSection.getAspect().setWidth(aspect.getWidth());
                    rptSection.getAspect().setHeight(0);
                    rptSection.getAspect().setTop(aspect.getTop() + aspect.getHeight());

                    rptSection.setKeyPaint(paintSection(rptSection.getAspect(), 
                                                        rptSection.getKey(), 
                                                        csRptTypeSection.HEADER, 
                                                        rptSection.getName(), 
                                                        false));

                    w_aspect = rptSection.getAspect();
                    moveSection(m_paint.getPaintObject(rptSection.getKeyPaint()), 
                                0, 
                                w_aspect.getTop(), 
                                w_aspect.getTop() + cGlobals.C_HEIGHT_NEW_SECTION, 
                                w_aspect.getTop() + rptSection.getAspect().getHeight(), 
                                rptSection, 
                                true);
                    break;

                case csRptTypeSection.DETAIL:
                    break;

                case csRptTypeSection.GROUP_HEADER:

                    cIReportGroupSections w_groupsHeaders = m_report.getGroupsHeaders();
				    rptSection = w_groupsHeaders.item(w_groupsHeaders.count()-1);
                    rptSection.setName("GH_" + rptSection.getIndex().ToString());

                    // the first group is next to the last header
                    //
					if (w_groupsHeaders.count() == 1) {
                        topSec = m_report.getHeaders().item(m_report.getHeaders().count()-1);
                    } 
                    else {
						topSec = w_groupsHeaders.item(w_groupsHeaders.count() - 1);
                    }

				    w_aspect = topSec.getAspect();
                    rptSection.getAspect().setWidth(w_aspect.getWidth());
                    rptSection.getAspect().setHeight(0);
                    rptSection.getAspect().setTop(w_aspect.getTop() + w_aspect.getHeight());

                    rptSection.setKeyPaint(paintSection(rptSection.getAspect(), 
                                                        rptSection.getKey(), 
                                                        csRptTypeSection.GROUP_HEADER, 
                                                        rptSection.getName(), 
                                                        false));

					w_aspect = rptSection.getAspect();
                    moveSection(m_paint.getPaintObject(rptSection.getKeyPaint()), 
                                0, 
                                w_aspect.getTop() + cGlobals.C_HEIGHT_NEW_SECTION, 
                                w_aspect.getTop(), 
                                w_aspect.getTop() + cGlobals.C_HEIGHT_NEW_SECTION, 
                                rptSection, 
                                true);
                    break;

                case csRptTypeSection.GROUP_FOOTER:

                    cIReportGroupSections w_groupsFooters = m_report.getGroupsFooters();
                    rptSection = w_groupsFooters.item(0);
                    rptSection.setName("GF_" + rptSection.getIndex().ToString());

                    // all group footers are added to the top so at the
                    // beginning they are next to the detail section
                    //

                    topSec = m_report.getDetails().item(m_report.getDetails().count()-1);

					w_aspect = topSec.getAspect();
                    rptSection.getAspect().setWidth(w_aspect.getWidth());
                    rptSection.getAspect().setHeight(cGlobals.C_HEIGHT_NEW_SECTION);
                    rptSection.getAspect().setTop(w_aspect.getTop() + w_aspect.getHeight());

                    rptSection.setKeyPaint(paintSection(rptSection.getAspect(), 
                                                        rptSection.getKey(), 
                                                        csRptTypeSection.GROUP_FOOTER, 
                                                        rptSection.getName(), 
                                                        false));

                    paintObj = m_paint.getPaintObject(rptSection.getKeyPaint());
                    pMoveGroupFooter(rptSection.getKey(), out minBottom, out maxBottom);
                    
                    m_offY = 0;

					w_aspect = rptSection.getAspect();
                    y = w_aspect.getHeight() + w_aspect.getTop() - cGlobals.C_HEIGHT_BAR_SECTION;

                    moveSection(paintObj, 0, y, minBottom, maxBottom, rptSection, true);
                    break;

                case csRptTypeSection.FOOTER:
                    cReportSections w_footers = m_report.getFooters();

                    // all footers are added to the beginning of the collection
                    // 
                    rptSection = w_footers.add(null, "" , 1);
                    rptSection.setName("F_" + rptSection.getIndex().ToString());

                    aspect = w_footers.item(2).getAspect();
                    rptSection.getAspect().setWidth(aspect.getWidth());
                    rptSection.getAspect().setHeight(cGlobals.C_HEIGHT_NEW_SECTION);
                    rptSection.getAspect().setTop(aspect.getTop());

                    rptSection.setKeyPaint(paintSection(rptSection.getAspect(), 
                                                        rptSection.getKey(), 
                                                        csRptTypeSection.FOOTER, 
                                                        rptSection.getName(), 
                                                        false));

                    paintObj = m_paint.getPaintObject(rptSection.getKeyPaint());
                    pMoveFooter(rptSection.getKey(), out minBottom, out maxBottom);

                    m_offY = 0;

                    w_aspect = rptSection.getAspect();
                    y = w_aspect.getHeight() + w_aspect.getTop() - m_offSet - cGlobals.C_HEIGHT_BAR_SECTION;

                    moveSection(paintObj, 0, y, minBottom, maxBottom, rptSection, true);
                    break;
            }

            // every section we add has a section line
            // and we need to set his width
            //
			aspect = rptSection.getSectionLines().item(0).getAspect();
            aspect.setWidth(rptSection.getAspect().getWidth());

            refreshBody();
            refreshRule();
        }

        public void bringToFront() {
			m_paint.getPaintObjects().zorder(m_keyObj, true);
            refreshBody();
            m_dataHasChanged = true;
        }

        public void sendToBack() {
            m_paint.getPaintObjects().sendToBack(m_keyObj);
            refreshBody();
            m_dataHasChanged = true;
        }

        public void preview() {
            m_report.getLaunchInfo().setAction(csRptLaunchAction.CSRPTLAUNCHPREVIEW);
            launchReport();
        }

        public void printReport() {
            m_report.getLaunchInfo().setAction(csRptLaunchAction.CSRPTLAUNCHPRINTER);
            launchReport();
        }

        private void launchReport() {
            cMouseWait mouse = new cMouseWait(); 
            try {
                setZOrder();
                showProgressDlg();

                m_report.getLaunchInfo().getPrinter().setPaperInfo(m_report.getPaperInfo());
                m_report.getLaunchInfo().setObjPaint(new CSReportPaint.cReportPrint());
				// TODO: remove this
				m_report.getLaunchInfo().setHwnd(0);
                m_report.getLaunchInfo().setShowPrintersDialog(true);
                m_report.launch();

            } catch (Exception ex) {
                cError.mngError(ex, "launchReport", C_MODULE, "");
            }
            finally {
                mouse.Dispose();
                closeProgressDlg();
            }
        }

        public bool saveDocument(bool saveAs) {
            cMouseWait mouse = new cMouseWait(); 
            try {
                bool isNew = false;

                isNew = m_report.getName() == "";

                if (isNew) {
					m_report.setName(m_name);
                }

                if (saveAs) {
                    isNew = true;
                }

                setZOrder();

                pValidateSectionAspect();

                if (!m_report.save(m_fmain.saveFileDialog, isNew))
                reLoadReport();
                return true;

            } catch (Exception ex) {
                cError.mngError(ex, "saveDocument", C_MODULE, "");
                return false;
            }
            finally {
                mouse.Dispose();
            }
        }

        private void setZOrder() {
            cReportControl ctrl = null;
            for (int _i = 0; _i < m_report.getControls().count(); _i++) {
                ctrl = m_report.getControls().item(_i);
                ctrl.getLabel().getAspect().setNZOrder(m_paint.getPaintObjects().getZOrderForKey(ctrl.getKeyPaint()));
            }
        }

        public void newReport(cReport report) {

            if (report != null) {

                m_report = report;
                reLoadReport();
                pValidateSectionAspect();
                reLoadReport();

            } 
            else {

                m_report.setName("New report");

                m_paint.createPicture(m_picReport.CreateGraphics());
                refreshRule();

            }

			cMainEditor.setDocActive(this);
        }

        public bool openDocument()
        {
            return openDocument("");
        }

        public bool openDocument(String fileName) {
            cMouseWait mouse = new cMouseWait();
            try {

                // to avoid reentrancy
                m_opening = true;

                if (fileName == "") {

                    pSetInitDir();

                    if (!m_report.load(m_fmain.openFileDialog)) {

                        if (m_report.getName() == "")  
                            return false;
                    }

                } 
                else {

                    if (!m_report.loadSilent(fileName)) {
                        return false;
                    }
                }

                reLoadReport(); 

                Application.DoEvents();

                cMainEditor.setDocActive(this);

                m_opening = false;

                // Testing
                //

                //m_paint.initGrid(m_picReport.CreateGraphics(), m_typeGrid);

                //var bmp = m_paint.getBitmap();
/*
                var g = Graphics.FromImage(bmp);
                var graph = m_picReport.CreateGraphics();

                System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, (int)graph.VisibleClipBounds.Width, (int)graph.VisibleClipBounds.Height + 56); // TODO check why 56 ???
                Brush brush = new SolidBrush(Color.Red);
                g.FillRectangle(brush, rect);
                brush.Dispose();

                var b = new SolidBrush(Color.Black);
                g.FillRectangle(b, new RectangleF(0,0,10,10));
                g.FillRectangle(b, new RectangleF(100,100, 10, 10));

                Brush bg = new System.Drawing.Drawing2D.HatchBrush(
                                            System.Drawing.Drawing2D.HatchStyle.DottedGrid,
                                            Color.FromArgb(0xC0C0C0),
                                            Color.White);

                g.FillRectangle(bg, new RectangleF(200, 100, 100, 100));

                                

                b.Dispose();
                bg.Dispose();
                graph.Dispose();
                g.Dispose();
 */ 
                //m_picReport.Image = bmp;

                return true;
            }
            catch (Exception ex) {
                return false;
            }
            finally {
                mouse.Dispose();
            }            
        }

        public bool saveChanges() {
            csAskEditResult rslt;

            if (m_dataHasChanged) {

                rslt = askEdit("Do you want to save changes to " + m_reportFullPath + "?", "CSReportEditor");

                switch (rslt) {
                    case csAskEditResult.CSASKRSLTYES:
						if (!saveDocument(false)) 
                            return false; 
                        break;

                    case csAskEditResult.CSASKRSLTCANCEL:
                        return false;
                }
            }

            m_dataHasChanged = false;
            return true;
        }

        private csAskEditResult askEdit(String msg, String title) {
            
			DialogResult rslt = MessageBox.Show(
                                    	msg, title, 
                                    	MessageBoxButtons.YesNoCancel, 
										MessageBoxIcon.Question,
										MessageBoxDefaultButton.Button3);
            switch (rslt) {
                case DialogResult.Yes:
                    return csAskEditResult.CSASKRSLTYES;
                case DialogResult.No:
                    return csAskEditResult.CSASKRSLTNO;
                default:
                    return csAskEditResult.CSASKRSLTCANCEL;
            }
        }

        public bool showHelpDbField() { 
            return pShowHelpDbField(m_fProperties);
        }

        public bool showHelpDbFieldForGroup() { 
            return pShowHelpDbField(m_fGroup);
        }

        private bool pShowHelpDbField(cIDatabaseFieldSelector f) { 
            int nIndex = 0;
            int nFieldType = 0;
            String sField = "";

            sField = f.txDbField.Text;
            nFieldType = f.getFieldType();
            nIndex = f.getIndex();

            if (cGlobals.showDbFields(ref sField, ref nFieldType, ref nIndex, this))
            {
                f.txDbField.Text = sField;
                f.setFieldType(nFieldType);
                f.setIndex(nIndex);
                
                if(f is fProperties) {
                    (f as fProperties).txText.Text = sField;
                }
                return true;
            }
            else 
            {
                return false;
            }
        }

        public void showGroupProperties() {
            cReportSection sec = null;
            cReportGroup group = null;
            bool isGroup = false;

            sec = pGetSection(out isGroup);

            if (sec == null) { return; }

            if (!isGroup) { return; }

            for (int _i = 0; _i < m_report.getGroups().count(); _i++) {
                group = m_report.getGroups().item(_i);
                if (group.getHeader().getKey() == sec.getKey()) { break; }
                if (group.getFooter().getKey() == sec.getKey()) { break; }
            }

            pShowGroupProperties(group);

            refreshAll();
        }

        private void pShowGroupProperties(cReportGroup group) 
        {
    
            try {

                bool isNew = false;

                m_showingProperties = true;

                if (m_fGroup == null) { m_fGroup = new fGroup(); }

                m_fGroup.setHandler(this);

                if (group == null) { isNew = true; }

                if (isNew) {
                    m_fGroup.txName.Text = "Group" + m_report.getGroups().count() + 1;
                } 
                else {
                    m_fGroup.txName.Text = group.getName();
                    m_fGroup.txDbField.Text = group.getFieldName();

                    if (group.getOderType() == csRptGrpOrderType.CSRPTGRPASC) {
                      m_fGroup.opAsc.Checked = true;
                    } 
                    else {
                      m_fGroup.opDesc.Checked = true;
                    }

                    m_fGroup.chkPrintInNewPage.Checked = group.getPrintInNewPage();
                    m_fGroup.chkReprintGroup.Checked = group.getRePrintInNewPage();
                    m_fGroup.chkGrandTotal.Checked = group.getGrandTotalGroup();

                    switch (group.getComparisonType()) {
                      case  csRptGrpComparisonType.CSRPTGRPDATE:
                        m_fGroup.opDate.Checked = true;
                        break;

                      case  csRptGrpComparisonType.CSRPTGRPNUMBER:
                        m_fGroup.opNumber.Checked = true;
                        break;

                      case  csRptGrpComparisonType.CSRPTGRPTEXT:
                        m_fGroup.opText.Checked = true;
                        break;
                    }
                }

                m_fGroup.lbGroup.Text = "Group: " + m_fGroup.txName.Text;

                m_fGroup.ShowDialog();

                if (m_fGroup.getOk())
                {

                    if (isNew)
                    {
                        group = m_report.getGroups().add(null, "");
                    }

                    group.setName(m_fGroup.txName.Text);
                    group.setFieldName(m_fGroup.txDbField.Text);

                    group.setIndex(m_report.getGroups().Count);
                    group.setOderType(m_fGroup.opAsc.Checked ? csRptGrpOrderType.CSRPTGRPASC : csRptGrpOrderType.CSRPTGRPDESC);

                    group.setPrintInNewPage(m_fGroup.chkPrintInNewPage.Checked);
                    group.setRePrintInNewPage(m_fGroup.chkReprintGroup.Checked);
                    group.setGrandTotalGroup(m_fGroup.chkGrandTotal.Checked);

                    if (m_fGroup.opDate.Checked)
                    {
                        group.setComparisonType(csRptGrpComparisonType.CSRPTGRPDATE);
                    }
                    else if (m_fGroup.opNumber.Checked)
                    {
                        group.setComparisonType(csRptGrpComparisonType.CSRPTGRPNUMBER);
                    }
                    else if (m_fGroup.opText.Checked)
                    {
                        group.setComparisonType(csRptGrpComparisonType.CSRPTGRPTEXT);
                    }

                    if (isNew)
                    {
                        addSection(csRptTypeSection.GROUP_HEADER);
                        addSection(csRptTypeSection.GROUP_FOOTER);
                    }

                    m_dataHasChanged = true;
                }
      
            } catch (Exception ex) {
                cError.mngError(ex, "showGroupProperties", C_MODULE, "");
            }
            finally {
                m_showingProperties = false;
                if (m_fGroup != null)
                {
                    m_fGroup.Close();
                    m_fGroup = null;
                }                    
            }      
        }

        public void moveGroup() {
            cReportSection sec = null;
            cReportGroup group = null;
            bool isGroup = false;

            sec = pGetSection(out isGroup);

            if (sec == null) { return; }

            if (!isGroup) { return; }

            for (int _i = 0; _i < m_report.getGroups().count(); _i++) {
                group = m_report.getGroups().item(_i);
                if (group.getHeader().getKey() == sec.getKey()) { break; }
                if (group.getFooter().getKey() == sec.getKey()) { break; }
            }

            cGlobals.moveGroup(group, this);

            G.redim(ref m_vSelectedKeys, 0);
            refreshReport();
        }

        public void showSectionProperties() {
            cReportSection sec = null;
            bool isGroup = false;

            sec = pGetSection(out isGroup);

            if (sec == null) { return; }

            pShowSecProperties(sec);

            refreshAll();
        }

        public void showSecLnProperties() {
            cReportSection sec = null;
            cReportSectionLine secLn = null;
            bool isSecLn = false;

            sec = pGetSection(out isSecLn, out secLn, true);

            if (sec == null) { return; }
            if (secLn == null) { return; }
            if (!isSecLn) { return; }

            pShowSecProperties(secLn, sec.getName() + " - line " + secLn.getRealIndex().ToString());

            refreshAll();
        }

        private void pShowSecProperties(cIReportSection sec)
        {
            pShowSecProperties(sec, "");
        }

        private void pShowSecProperties(cIReportSection sec, String secLnName) { 
            try {

                m_showingProperties = true;

                if (m_fSecProperties == null) { 
                    m_fSecProperties = new fSecProperties();
                }

                m_fSecProperties.setHandler(this);

                m_fSecProperties.chkFormulaHide.Checked = sec.getHasFormulaHide();
                m_fSecProperties.setFormulaHide(sec.getFormulaHide().getText());
                
                if (sec is cReportSectionLine) {
                    m_fSecProperties.txName.Enabled = false;
                }

                m_fSecProperties.txName.Text = sec is cReportSectionLine ? secLnName : sec.getName();

                m_fSecProperties.lbSectionName.Text = "Section: " + (sec is cReportSectionLine ? secLnName : sec.getName());

                m_fSecProperties.ShowDialog();

                if (m_fSecProperties.getOk()) {
                    if (m_fSecProperties.getSetFormulaHideChanged()) { sec.setHasFormulaHide(m_fSecProperties.chkFormulaHide.Checked); }
                    if (m_fSecProperties.getFormulaHideChanged()) { sec.getFormulaHide().setText(m_fSecProperties.getFormulaHide()); }
                    if (sec is cReportSection) { sec.setName(m_fSecProperties.txName.Text); }
                }

            } catch (Exception ex) {
                cError.mngError(ex, "pShowSecProperties", C_MODULE, "");
            }
            finally {
                m_fSecProperties.Close();
                m_showingProperties = false;
                m_fSecProperties = null;
            }
        }

        // ReturnSecLn is flag to state that the caller wants to get
        // the section line asociated with the separator of the section
        // remember that the last section line don't have a separator 
        // but share it with the section.
        //
        private cReportSection pGetSection(
            out bool isGroup)
        {
            bool isSecLn;
            bool isGroupHeader;
            bool isGroupFooter;
            cReportSectionLine secLn;
            return pGetSection(out isGroup, out isSecLn, out secLn, false, out isGroupHeader, out isGroupFooter);
        }

		private cReportSection pGetSection(
			out bool isGroup, 
			out bool isSecLn) 
		{
            bool isGroupHeader;
            bool isGroupFooter;
            cReportSectionLine secLn;
            return pGetSection(out isGroup, out isSecLn, out secLn, false, out isGroupHeader, out isGroupFooter);
		}

        private cReportSection pGetSection(
            out bool isSecLn,
            out cReportSectionLine secLn,
            bool returnSecLn)
        {
            bool isGroupHeader;
            bool isGroupFooter;

            return pGetSection(out isSecLn, out secLn, returnSecLn, out isGroupHeader, out isGroupFooter);
        }

        private cReportSection pGetSection(
            out bool isSecLn,
            out cReportSectionLine secLn,
            bool returnSecLn,
            out bool isGroupHeader,
            out bool isGroupFooter)
        { 
            bool isGroup;
            return pGetSection(out isGroup, out isSecLn, out secLn, returnSecLn, out isGroupHeader, out isGroupFooter);
        }

        private cReportSection pGetSection(
            out bool isGroup, 
            out bool isSecLn, 
			out cReportSectionLine secLn,
            bool returnSecLn,
            out bool isGroupHeader,
            out bool isGroupFooter) 
        {

            cReportSection sec = null;

            isGroup = false;
            isSecLn = false;
            secLn = null;
            isGroupFooter = false;
            isGroupHeader = false;

			if (m_keyFocus == "") { return null; }

            // get the section and show his properties
            //
			if (!m_paint.paintObjIsSection(m_keyFocus)) { return null; }

			CSReportPaint.cReportPaintObject paintObj = m_paint.getPaintSections().item(m_keyFocus);

            // nothing to do
            //
			if (paintObj == null) { return null; }

            sec = m_report.getHeaders().item(paintObj.getTag());
            if (sec != null) {

                // it's a header
            } 
            else {
                sec = m_report.getFooters().item(paintObj.getTag());
                if (sec != null) {

                    // it's a footer
                } 
                else {

                    // check if it is a group
                    //
                    sec = m_report.getGroupsHeaders().item(paintObj.getTag());
                    if (sec != null) {

                        // it's a group
                        //
                        isGroup = true;
                        isGroupHeader = true;

                    } 
                    else {
                        sec = m_report.getGroupsFooters().item(paintObj.getTag());
                        if (sec != null) {

                            // it's a group
                            //
                            isGroup = true;
                            isGroupFooter = true;

                        } 
                        else {
                            // check if it is a detail
                            //
                            sec = m_report.getDetails().item(paintObj.getTag());
                            if (sec != null) {

                                // it's a detail

                                // it's a line
                            } 
                            else {
                                isSecLn = true;
                                switch (paintObj.getRptType()) {
                                    case csRptTypeSection.SECLN_HEADER:
                                        sec = m_report.getHeaders().item(paintObj.getRptKeySec());
                                        break;
                                    case csRptTypeSection.SECLN_DETAIL:
                                        sec = m_report.getDetails().item(paintObj.getRptKeySec());
                                        break;
                                    case csRptTypeSection.SECLN_FOOTER:
                                        sec = m_report.getFooters().item(paintObj.getRptKeySec());
                                        break;
                                    case csRptTypeSection.SECLN_GROUPH:
                                        sec = m_report.getGroupsHeaders().item(paintObj.getRptKeySec());
                                        break;
                                    case csRptTypeSection.SECLN_GROUPF:
                                        sec = m_report.getGroupsFooters().item(paintObj.getRptKeySec());
                                        break;
                                }
								secLn = sec.getSectionLines().item(paintObj.getTag());
                            }
                        }
                    }
                }
            }

            // if the caller wants a section line and the separator
            // is owned by a section (isSecLn == false) we return
            // the last section line of the section asociated to the separator
            //
            if (returnSecLn && !isSecLn) {
				secLn = sec.getSectionLines().item(sec.getSectionLines().count()-1);
                isSecLn = true;
            }

            return sec;
        }

        public void showProperties(string key)
        {
            if ("SL".IndexOf(cUtil.subString(key, 0, 1)) != -1)
            {
                bool bIsSecLn = false;
                pSelectSection(key.Substring(1), out bIsSecLn);

                if (bIsSecLn)
                {
                    showSecLnProperties();
                }
                else
                {
                    showProperties();
                }
            }
            else
            {
                selectCtrl(key);
                showProperties();
            }                        
        }

        public void showProperties() {
            if (m_keyFocus == "") { return; }

            cMouseWait mouse = new cMouseWait();

            if (m_paint.paintObjIsSection(m_keyFocus)) {
                showSectionProperties();
            } 
            else {
                m_keyObj = m_keyFocus;
                showCtrlProperties();
            }

            refreshAll();
        }

        private void showCtrlProperties() {
            try {

                CSReportPaint.cReportPaintObject paintObject = null;
                cReportControl rptCtrl = null;
				cReportAspect w_aspect = null;
				cReportFont w_font = null;
                bool bMultiSelect = false;
                
                m_showingProperties = true;

                if (m_fProperties == null) { 
                    m_fProperties = new fProperties();
                }

                m_fProperties.setHandler(this);

                paintObject = m_paint.getPaintObject(m_keyObj);
                if (paintObject == null) { return; }

                m_fProperties.txText.Text = paintObject.getText();
                rptCtrl = m_report.getControls().item(paintObject.getTag());

                if (rptCtrl.getControlType() != csRptControlType.CSRPTCTIMAGE)
                {
                    m_fProperties.hideTabImage();
                }
                else {
                    m_fProperties.picImage.Image = rptCtrl.getImage().getImage();
                }

                if (rptCtrl.getControlType() != csRptControlType.CSRPTCTCHART) {
                    m_fProperties.hideTabChart();
                } 
                else {

                    cUtil.listSetListIndexForId(m_fProperties.cbType, (int)rptCtrl.getChart().getChartType());
                    cUtil.listSetListIndexForId(m_fProperties.cbFormatType, (int)rptCtrl.getChart().getFormat());
                    cUtil.listSetListIndexForId(m_fProperties.cbChartSize, (int)rptCtrl.getChart().getDiameter());
                    cUtil.listSetListIndexForId(m_fProperties.cbChartThickness, (int)rptCtrl.getChart().getThickness());
                    cUtil.listSetListIndexForId(m_fProperties.cbLinesType, (int)rptCtrl.getChart().getGridLines());

                    m_fProperties.txChartTop.Text = rptCtrl.getChart().getTop().ToString();
                    m_fProperties.txDbFieldGroupValue.Text = rptCtrl.getChart().getGroupFieldName();
                    m_fProperties.setChartGroupIndex(rptCtrl.getChart().getGroupFieldIndex());
                    m_fProperties.txChartGroupValue.Text = rptCtrl.getChart().getGroupValue();
                    m_fProperties.chkShowOutlines.Checked = rptCtrl.getChart().getOutlineBars();
                    m_fProperties.chkShowBarValues.Checked = rptCtrl.getChart().getShowValues();
                    m_fProperties.chkSort.Checked = rptCtrl.getChart().getSort();
                    m_fProperties.txText.Text = rptCtrl.getChart().getChartTitle();

                    if (rptCtrl.getChart().getSeries().count() > 0) {
                        m_fProperties.txDbFieldLbl1.Text = rptCtrl.getChart().getSeries().item(0).getLabelFieldName();
                        m_fProperties.txDbFieldVal1.Text = rptCtrl.getChart().getSeries().item(0).getValueFieldName();

                        m_fProperties.setChartIndex(0, rptCtrl.getChart().getSeries().item(0).getLabelIndex());
                        m_fProperties.setChartIndex(1, rptCtrl.getChart().getSeries().item(0).getValueIndex());

                        cUtil.listSetListIndexForId(m_fProperties.cbColorSerie1, (int)rptCtrl.getChart().getSeries().item(0).getColor());

                        if (rptCtrl.getChart().getSeries().count() > 1) {
                            m_fProperties.txDbFieldLbl2.Text = rptCtrl.getChart().getSeries().item(1).getLabelFieldName();
                            m_fProperties.txDbFieldVal2.Text = rptCtrl.getChart().getSeries().item(1).getValueFieldName();

                            m_fProperties.setChartIndex(2, rptCtrl.getChart().getSeries().item(1).getLabelIndex());
                            m_fProperties.setChartIndex(3, rptCtrl.getChart().getSeries().item(1).getValueIndex());

                            cUtil.listSetListIndexForId(m_fProperties.cbColorSerie2, (int)rptCtrl.getChart().getSeries().item(1).getColor());
                        }
                    }
                }

                if (rptCtrl.getControlType() == csRptControlType.CSRPTCTFIELD 
                    || rptCtrl.getControlType() == csRptControlType.CSRPTCTDBIMAGE) {
                    m_fProperties.txText.Enabled = false;
                    cReportField w_field = rptCtrl.getField();
                    m_fProperties.txText.Text = w_field.getName();
                    m_fProperties.txDbField.Text = w_field.getName();
                    m_fProperties.setFieldType(w_field.getFieldType());
                    m_fProperties.setIndex(w_field.getIndex());
                } 
                else {
                    m_fProperties.hideTabField();
                    m_fProperties.txText.Enabled = true;
                }

                m_fProperties.txName.Text = rptCtrl.getName();
                m_fProperties.lbControl.Text = rptCtrl.getName();
                m_fProperties.chkFormulaHide.Checked = rptCtrl.getHasFormulaHide();
                m_fProperties.chkFormulaValue.Checked = rptCtrl.getHasFormulaValue();

                m_fProperties.txExportColIdx.Text = rptCtrl.getExportColIdx().ToString();
                m_fProperties.chkIsFreeCtrl.Checked = rptCtrl.getIsFreeCtrl();

                m_fProperties.txTag.Text = rptCtrl.getTag();
                m_fProperties.setFormulaHide(rptCtrl.getFormulaHide().getText());
                m_fProperties.setFormulaValue(rptCtrl.getFormulaValue().getText());
                m_fProperties.txIdxGroup.Text = rptCtrl.getFormulaValue().getIdxGroup().ToString();
                m_fProperties.opBeforePrint.Checked = rptCtrl.getFormulaValue().getWhenEval() == csRptWhenEval.CSRPTEVALPRE;
                m_fProperties.opAfterPrint.Checked = rptCtrl.getFormulaValue().getWhenEval() == csRptWhenEval.CSRPTEVALPOST;

                w_aspect = rptCtrl.getLabel().getAspect();
                m_fProperties.chkCanGrow.Checked = w_aspect.getCanGrow();
                m_fProperties.txFormat.Text = w_aspect.getFormat();
                m_fProperties.txSymbol.Text = w_aspect.getSymbol();
                m_fProperties.setIsAccounting(w_aspect.getIsAccounting());
                m_fProperties.chkWordWrap.Checked = w_aspect.getWordWrap();

                cUtil.listSetListIndexForId(m_fProperties.cbAlign, (int)w_aspect.getAlign());

                m_fProperties.txBorderColor.Text = w_aspect.getBorderColor().ToString();
                m_fProperties.txBorder3D.Text = w_aspect.getBorderColor3d().ToString();
                m_fProperties.txBorderShadow.Text = w_aspect.getBorderColor3dShadow().ToString();
                m_fProperties.chkBorderRounded.Checked = w_aspect.getBorderRounded();
                m_fProperties.txBorderWidth.Text = w_aspect.getBorderWidth().ToString();

                cUtil.listSetListIndexForId(m_fProperties.cbBorderType, (int)w_aspect.getBorderType());

                w_font = w_aspect.getFont();
                m_fProperties.txFont.Text = w_font.getName();
                m_fProperties.txForeColor.Text = w_font.getForeColor().ToString();
                m_fProperties.shForeColor.BackColor = cColor.colorFromRGB(w_font.getForeColor());
                m_fProperties.txFontSize.Text = w_font.getSize().ToString();
                m_fProperties.chkFontBold.Checked = w_font.getBold();
                m_fProperties.chkFontItalic.Checked = w_font.getItalic();
                m_fProperties.chkFontUnderline.Checked = w_font.getUnderline();
                m_fProperties.chkFontStrike.Checked = w_font.getStrike();

                w_aspect = paintObject.getAspect();
                m_fProperties.txLeft.Text = w_aspect.getLeft().ToString();
                m_fProperties.txTop.Text = w_aspect.getTop().ToString();
                m_fProperties.txWidth.Text = w_aspect.getWidth().ToString();
                m_fProperties.txHeight.Text = w_aspect.getHeight().ToString();
                m_fProperties.txBackColor.Text = w_aspect.getBackColor().ToString();
                m_fProperties.shBackColor.BackColor = cColor.colorFromRGB(w_aspect.getBackColor());
                m_fProperties.chkTransparent.Checked = w_aspect.getTransparent();

                bMultiSelect = m_vSelectedKeys.Length > 1;

                m_fProperties.resetChangedFlags();

                m_fProperties.ShowDialog();

                if (!m_fProperties.getOk()) { return; }

                for (int i = 0; i < m_vSelectedKeys.Length; i++) {

                    paintObject = m_paint.getPaintObject(m_vSelectedKeys[i]);
                    rptCtrl = m_report.getControls().item(paintObject.getTag());

                    if (!bMultiSelect) {
                        if (rptCtrl.getName() != m_fProperties.txName.Text) {
                            if (rptCtrl.getName() != "") {
                                if (cWindow.ask("You have changed the name of this control.;;Do you want to update all references to this control in all formulas of this report?", MessageBoxDefaultButton.Button2))
                                {
                                    pUpdateFormulas(rptCtrl.getName(), m_fProperties.txName.Text);
                                }
                            }
                        }
                        rptCtrl.setName(m_fProperties.txName.Text);
                    }

                    if (m_fProperties.getTextChanged()) { rptCtrl.getLabel().setText(m_fProperties.txText.Text); }
                    if (m_fProperties.getTagChanged()) { rptCtrl.setTag(m_fProperties.txTag.Text); }
                    if (m_fProperties.getSetFormulaHideChanged()) { rptCtrl.setHasFormulaHide(m_fProperties.chkFormulaHide.Checked); }
                    if (m_fProperties.getSetFormulaValueChanged()) { rptCtrl.setHasFormulaValue(m_fProperties.chkFormulaValue.Checked); }
                    if (m_fProperties.getFormulaHideChanged()) { rptCtrl.getFormulaHide().setText(m_fProperties.getFormulaHide()); }
                    if (m_fProperties.getFormulaValueChanged()) { rptCtrl.getFormulaValue().setText(m_fProperties.getFormulaValue()); }
                    if (m_fProperties.getIdxGroupChanged()) { rptCtrl.getFormulaValue().setIdxGroup(cUtil.valAsInt(m_fProperties.txIdxGroup.Text)); }
                    if (m_fProperties.getWhenEvalChanged()) { rptCtrl.getFormulaValue().setWhenEval(m_fProperties.opAfterPrint.Checked ? csRptWhenEval.CSRPTEVALPOST : csRptWhenEval.CSRPTEVALPRE); }

                    if (m_fProperties.getExportColIdxChanged()) { rptCtrl.setExportColIdx(cUtil.valAsInt(m_fProperties.txExportColIdx.Text)); }
                    if (m_fProperties.getIsFreeCtrlChanged()) { rptCtrl.setIsFreeCtrl(m_fProperties.chkIsFreeCtrl.Checked); }

                    if (rptCtrl.getControlType() == csRptControlType.CSRPTCTFIELD || rptCtrl.getControlType() == csRptControlType.CSRPTCTDBIMAGE) {

                        cReportField w_field = rptCtrl.getField();
                        if (m_fProperties.getDbFieldChanged()) {
                            w_field.setFieldType(m_fProperties.getFieldType());
                            w_field.setIndex(m_fProperties.getIndex());
                            w_field.setName(m_fProperties.txDbField.Text);
                        }
                    }

                    if (m_fProperties.getPictureChanged()) {
                        rptCtrl.getImage().setImage(new Bitmap(m_fProperties.picImage.Image));
                    }

                    if (rptCtrl.getControlType() == csRptControlType.CSRPTCTCHART) {

                        if (rptCtrl.getChart().getSeries().count() < 1) { 
                            rptCtrl.getChart().getSeries().add(); 
                        }

                        if (m_fProperties.getChartTypeChanged()) {
                            rptCtrl.getChart().setChartType((csRptChartType)cUtil.listID(m_fProperties.cbType));
                        }
                        if (m_fProperties.getChartFormatTypeChanged()) {
                            rptCtrl.getChart().setFormat((csRptChartFormat)cUtil.listID(m_fProperties.cbFormatType));
                        }
                        if (m_fProperties.getChartSizeChanged()) {
                            rptCtrl.getChart().setDiameter((csRptChartPieDiameter)cUtil.listID(m_fProperties.cbChartSize));
                        }
                        if (m_fProperties.getChartThicknessChanged()) {
                            rptCtrl.getChart().setThickness((csRptChartPieThickness)cUtil.listID(m_fProperties.cbChartThickness));
                        }
                        if (m_fProperties.getChartLinesTypeChanged()) {
                            rptCtrl.getChart().setGridLines((csRptChartLineStyle)cUtil.listID(m_fProperties.cbLinesType));
                        }

                        if (m_fProperties.getChartShowLinesChanged()) {
                            rptCtrl.getChart().setOutlineBars(m_fProperties.chkShowOutlines.Checked);
                        }
                        if (m_fProperties.getChartShowValuesChanged()) {
                            rptCtrl.getChart().setShowValues(m_fProperties.chkShowBarValues.Checked);
                        }

                        if (m_fProperties.getTextChanged()) {
                            rptCtrl.getChart().setChartTitle(m_fProperties.txText.Text);
                        }

                        if (m_fProperties.getChartTopChanged()) {
                            rptCtrl.getChart().setTop(cUtil.valAsInt(m_fProperties.txChartTop.Text));
                        }

                        if (m_fProperties.getChartSortChanged()) {
                            rptCtrl.getChart().setSort(m_fProperties.chkSort.Checked);
                        }

                        if (m_fProperties.getChartGroupValueChanged()) {
                            rptCtrl.getChart().setGroupValue(m_fProperties.txChartGroupValue.Text);
                        }

                        if (m_fProperties.getChartFieldGroupChanged()) {
                            rptCtrl.getChart().setGroupFieldName(m_fProperties.txDbFieldGroupValue.Text);
                            rptCtrl.getChart().setGroupFieldIndex(m_fProperties.getChartGroupIndex());
                        }

                        if (m_fProperties.getChartFieldLbl1Changed()) {
                            rptCtrl.getChart().getSeries().item(0).setLabelFieldName(m_fProperties.txDbFieldLbl1.Text);
                            rptCtrl.getChart().getSeries().item(0).setLabelIndex(m_fProperties.getChartIndex(0));
                        }
                        if (m_fProperties.getChartFieldVal1Changed()) {
                            rptCtrl.getChart().getSeries().item(0).setValueFieldName(m_fProperties.txDbFieldVal1.Text);
                            rptCtrl.getChart().getSeries().item(0).setValueIndex(m_fProperties.getChartIndex(1));
                        }

                        if (m_fProperties.getChartColorSerie1Changed()) {
                            rptCtrl.getChart().getSeries().item(0).setColor((csColors)cUtil.listID(m_fProperties.cbColorSerie1));
                        }

                        if (m_fProperties.getChartFieldLbl2Changed() || m_fProperties.getChartFieldVal2Changed()) {
                            if (rptCtrl.getChart().getSeries().count() < 2) { 
                                rptCtrl.getChart().getSeries().add(); 
                            }
                        }

                        if (m_fProperties.txDbFieldLbl2.Text == "" || m_fProperties.txDbFieldVal2.Text == "") {
                            if (rptCtrl.getChart().getSeries().count() > 1) { rptCtrl.getChart().getSeries().remove(1); }
                        }

                        if (rptCtrl.getChart().getSeries().count() > 1) {

                            if (m_fProperties.getChartFieldLbl2Changed()) {
                                rptCtrl.getChart().getSeries().item(1).setLabelFieldName(m_fProperties.txDbFieldLbl2.Text);
                                rptCtrl.getChart().getSeries().item(1).setLabelIndex(m_fProperties.getChartIndex(2));
                            }
                            if (m_fProperties.getChartFieldVal2Changed()) {
                                rptCtrl.getChart().getSeries().item(1).setValueFieldName(m_fProperties.txDbFieldVal2.Text);
                                rptCtrl.getChart().getSeries().item(1).setValueIndex(m_fProperties.getChartIndex(3));
                            }

                            if (m_fProperties.getChartColorSerie2Changed()) {
                                rptCtrl.getChart().getSeries().item(1).setColor((csColors)cUtil.listID(m_fProperties.cbColorSerie2));
                            }
                        }
                    }

                    if (m_fProperties.getTextChanged()) { paintObject.setText(m_fProperties.txText.Text); }

                    w_aspect = rptCtrl.getLabel().getAspect();
                    if (m_fProperties.getLeftChanged()) { w_aspect.setLeft((float)cUtil.val(m_fProperties.txLeft.Text)); }
                    if (m_fProperties.getTopChanged()) { w_aspect.setTop((float)cUtil.val(m_fProperties.txTop.Text)); }
                    if (m_fProperties.getWidthChanged()) { w_aspect.setWidth((float)cUtil.val(m_fProperties.txWidth.Text)); }
                    if (m_fProperties.getHeightChanged()) { w_aspect.setHeight((float)cUtil.val(m_fProperties.txHeight.Text)); }
                    if (m_fProperties.getBackColorChanged()) { w_aspect.setBackColor(cUtil.valAsInt(m_fProperties.txBackColor.Text)); }
                    if (m_fProperties.getTransparentChanged()) { w_aspect.setTransparent(m_fProperties.chkTransparent.Checked); }
                    if (m_fProperties.getAlignChanged()) { w_aspect.setAlign((CSReportGlobals.HorizontalAlignment)cUtil.listID(m_fProperties.cbAlign)); }
                    if (m_fProperties.getFormatChanged()) { w_aspect.setFormat(m_fProperties.txFormat.Text); }
                    if (m_fProperties.getSymbolChanged()) {
                        w_aspect.setSymbol(m_fProperties.txSymbol.Text);
                        w_aspect.setIsAccounting(m_fProperties.getIsAccounting());
                    }
                    if (m_fProperties.getWordWrapChanged()) { w_aspect.setWordWrap(m_fProperties.chkWordWrap.Checked); }
                    if (m_fProperties.getCanGrowChanged()) { w_aspect.setCanGrow(m_fProperties.chkCanGrow.Checked); }

                    if (m_fProperties.getBorderColorChanged()) { w_aspect.setBorderColor(cUtil.valAsInt(m_fProperties.txBorderColor.Text)); }
                    if (m_fProperties.getBorder3DChanged()) { w_aspect.setBorderColor3d(cUtil.valAsInt(m_fProperties.txBorder3D.Text)); }
                    if (m_fProperties.getBorder3DShadowChanged()) { w_aspect.setBorderColor3dShadow(cUtil.valAsInt(m_fProperties.txBorderShadow.Text)); }
                    if (m_fProperties.getBorderRoundedChanged()) { w_aspect.setBorderRounded(m_fProperties.chkBorderRounded.Checked); }
                    if (m_fProperties.getBorderWidthChanged()) { w_aspect.setBorderWidth(cUtil.valAsInt(m_fProperties.txBorderWidth.Text)); }
                    if (m_fProperties.getBorderTypeChanged()) { w_aspect.setBorderType((csReportBorderType)cUtil.listID(m_fProperties.cbBorderType)); }

                    w_font = w_aspect.getFont();
                    if (m_fProperties.getFontChanged()) { w_font.setName(m_fProperties.txFont.Text); }
                    if (m_fProperties.getForeColorChanged()) { w_font.setForeColor(cUtil.valAsInt(m_fProperties.txForeColor.Text)); }
                    if (m_fProperties.getFontSizeChanged()) { w_font.setSize((float)cUtil.val(m_fProperties.txFontSize.Text)); }
                    if (m_fProperties.getBoldChanged()) { w_font.setBold(m_fProperties.chkFontBold.Checked); }
                    if (m_fProperties.getItalicChanged()) { w_font.setItalic(m_fProperties.chkFontItalic.Checked); }
                    if (m_fProperties.getUnderlineChanged()) { w_font.setUnderline(m_fProperties.chkFontUnderline.Checked); }
                    if (m_fProperties.getStrikeChanged()) { w_font.setStrike(m_fProperties.chkFontStrike.Checked); }

                    if (m_fProperties.getPictureChanged()) {
                        paintObject.setImage(rptCtrl.getImage().getImage());
                    }

                    //
                    // TODO: check if we can refactor this now we have a better class hierarchy
                    //

                    w_aspect = paintObject.getAspect();
                    if (m_fProperties.getLeftChanged()) { w_aspect.setLeft((float)cUtil.val(m_fProperties.txLeft.Text)); }
                    if (m_fProperties.getTopChanged()) { w_aspect.setTop((float)cUtil.val(m_fProperties.txTop.Text)); }
                    if (m_fProperties.getWidthChanged()) { w_aspect.setWidth((float)cUtil.val(m_fProperties.txWidth.Text)); }
                    if (m_fProperties.getHeightChanged()) { w_aspect.setHeight((float)cUtil.val(m_fProperties.txHeight.Text)); }
                    if (m_fProperties.getBackColorChanged()) { w_aspect.setBackColor(cUtil.valAsInt(m_fProperties.txBackColor.Text)); }
                    if (m_fProperties.getTransparentChanged()) { w_aspect.setTransparent(m_fProperties.chkTransparent.Checked); }
                    if (m_fProperties.getAlignChanged()) { w_aspect.setAlign((CSReportGlobals.HorizontalAlignment)cUtil.listID(m_fProperties.cbAlign)); }
                    if (m_fProperties.getFormatChanged()) { w_aspect.setFormat(m_fProperties.txFormat.Text); }
                    if (m_fProperties.getSymbolChanged()) { w_aspect.setSymbol(m_fProperties.txSymbol.Text); }
                    if (m_fProperties.getWordWrapChanged()) { w_aspect.setWordWrap(m_fProperties.chkWordWrap.Checked); }

                    if (m_fProperties.getBorderTypeChanged()) { w_aspect.setBorderType((csReportBorderType)cUtil.listID(m_fProperties.cbBorderType)); }

                    if (w_aspect.getBorderType() == csReportBorderType.CSRPTBSNONE) {
                        w_aspect.setBorderColor(Color.Black.ToArgb());
                        w_aspect.setBorderWidth(1);
                        w_aspect.setBorderRounded(false);
                        w_aspect.setBorderType(csReportBorderType.CSRPTBSFIXED);
                    } 
                    else {
                        if (m_fProperties.getBorderColorChanged()) { w_aspect.setBorderColor(cUtil.valAsInt(m_fProperties.txBorderColor.Text)); }
                        if (m_fProperties.getBorder3DChanged()) { w_aspect.setBorderColor3d(cUtil.valAsInt(m_fProperties.txBorder3D.Text)); }
                        if (m_fProperties.getBorder3DShadowChanged()) { w_aspect.setBorderColor3dShadow(cUtil.valAsInt(m_fProperties.txBorderShadow.Text)); }
                        if (m_fProperties.getBorderRoundedChanged()) { w_aspect.setBorderRounded(m_fProperties.chkBorderRounded.Checked); }
                        if (m_fProperties.getBorderWidthChanged()) { w_aspect.setBorderWidth(cUtil.valAsInt(m_fProperties.txBorderWidth.Text)); }
                    }

                    w_font = w_aspect.getFont();
                    if (m_fProperties.getFontChanged()) { w_font.setName(m_fProperties.txFont.Text); }
                    if (m_fProperties.getForeColorChanged()) { w_font.setForeColor(cUtil.valAsInt(m_fProperties.txForeColor.Text)); }
                    if (m_fProperties.getFontSizeChanged()) { w_font.setSize((float)cUtil.val(m_fProperties.txFontSize.Text)); }
                    if (m_fProperties.getBoldChanged()) { w_font.setBold(m_fProperties.chkFontBold.Checked); }
                    if (m_fProperties.getItalicChanged()) { w_font.setItalic(m_fProperties.chkFontItalic.Checked); }
                    if (m_fProperties.getUnderlineChanged()) { w_font.setUnderline(m_fProperties.chkFontUnderline.Checked); }
                    if (m_fProperties.getStrikeChanged()) { w_font.setStrike(m_fProperties.chkFontStrike.Checked); }
                }

                m_dataHasChanged = true;

            } catch (Exception ex) {
                cError.mngError(ex, "pShowCtrlProperties", C_MODULE, "");
            }
            finally {
                m_fProperties.Hide();
                m_showingProperties = false;
                m_fProperties = null;
                m_paint.endMove(m_picReport.CreateGraphics());
            }
        }

        private void beginDraging() {
            /* TODO: implement me
            m_picReport.SetFocus;
            m_draging = true;
            m_picReport.Cursor = Cursors.Custom;
            m_picReport.MouseIcon = LoadPicture(App.Path + "\\move32x32.cur");
             */ 
        }

        private void endDraging() {
            m_draging = false;
            m_controlType = csRptEditCtrlType.CSRPTEDITNONE;
            m_picReport.Cursor = Cursors.Default;
        }

        public void showToolbox() {

            fToolbox f = cMainEditor.getToolbox(this);

            f.clear();

            pAddColumnsToToolbox(m_report.getConnect().getDataSource(), m_report.getConnect().getColumns(), f);

            for (int _i = 0; _i < m_report.getConnectsAux().count(); _i++) {
                cReportConnect connect = m_report.getConnectsAux().item(_i);
                pAddColumnsToToolbox(connect.getDataSource(), connect.getColumns(), f);
            }

            for (int _i = 0; _i < m_report.getControls().count(); _i++) {
                cReportControl ctrl = m_report.getControls().item(_i);
                if (cDatabaseGlobals.isNumberField(ctrl.getField().getFieldType()))
                {
                    f.addLbFormula(ctrl.getField().getName());

                    // TODO: refactor this to a better way to suggest the 
                    //       list of formulas applicable to the type of 
                    //       the database field
                    //
                    f.addFormula("Sum", ctrl.getName(), "_Sum");
                    f.addFormula("Maximum", ctrl.getName(), "_Max");
                    f.addFormula("Minimum", ctrl.getName(), "_Min");
                    f.addFormula("Averagge", ctrl.getName(), "_Average");
                }
            }
            if (!f.Visible)
            {
                f.Show(m_fmain);
            }
        }

        public void pAddColumnsToToolbox(String dataSource, cColumnsInfo columns, fToolbox f) {
            for (int _i = 0; _i < columns.count(); _i++) {
                cColumnInfo col = columns.item(_i);
                f.addField(
                    cGlobals.getDataSourceStr(dataSource) + col.getName(), 
                    (int)col.getColumnType(), 
                    col.getPosition());
                f.addLabels(col.getName());
            }
        }

        public void copy() {
            try {
                if (m_vSelectedKeys.Length == 0) { return; }

                G.redim(ref m_vCopyKeys, m_vSelectedKeys.Length);

                for (int i = 0; i < m_vSelectedKeys.Length; i++) {
                    m_vCopyKeys[i] = m_vSelectedKeys[i];
                }

				m_fmain.setReportCopySource(this);

            } catch (Exception ex) {
                cError.mngError(ex, "Copy", C_MODULE, "");
            }
        }

        public void paste(bool bDontMove) {
            try {

                m_bCopyWithoutMoving = bDontMove;

                if (m_vCopyKeys.Length == 0) {

					if (m_fmain.getReportCopySource() == null) { return; }

                    m_copyControlsFromOtherReport = true;

                } 
                else {

                    m_copyControls = true;

                }

                addLabel();

            } catch (Exception ex) {
                cError.mngError(ex, "Paste", C_MODULE, "");
            }
        }

        public void editText() {
            try {

                const int c_margen = 1;

                String sText = "";
                cReportAspect paintObjAspect = null;
                cReportControl ctrl = null;

                if (m_keyObj == "") { return; }

                cReportPaintObject w_getPaintObject = m_paint.getPaintObject(m_keyObj);
                paintObjAspect = w_getPaintObject.getAspect();
                sText = w_getPaintObject.getText();
                ctrl = m_report.getControls().item(w_getPaintObject.getTag());
                if (paintObjAspect == null) { return; }

                /* TODO: implement me
                TxEdit.Text = sText;
                TxEdit.Left = paintObjAspect.getLeft() + c_margen;
                TxEdit.Top = paintObjAspect.getTop() + c_margen - paintObjAspect.getOffset();
                TxEdit.Width = paintObjAspect.getWidth() - c_margen * 2;
                TxEdit.Height = paintObjAspect.getHeight() - c_margen * 2;
                TxEdit.Visible = true;
                TxEdit.ZOrder;
                TxEdit.SetFocus;
                TxEdit.FontName = paintObjAspect.getFont().getName();
                TxEdit.FontSize = paintObjAspect.getFont().getSize();
                TxEdit.FontBold = paintObjAspect.getFont().getBold();
                TxEdit.ForeColor = paintObjAspect.getFont().getForeColor();
                TxEdit.BackColor = paintObjAspect.getBackColor();
                */
            } catch (Exception ex) {
                cError.mngError(ex, "EditText", C_MODULE, "");
            }
        }

        private void endEditText(bool descartar) {
            /* TODO: implement me
            if (!TxEdit.Visible) { return; }

            TxEdit.Visible = false;

            if (descartar) { return; }

            m_dataHasChanged = true;

            CSReportPaint.cReportPaintObject paintObjAspect = null;
            paintObjAspect = m_paint.getPaintObject(m_keyObj);
            if (paintObjAspect == null) { return; }

            String sKeyRpt = "";
            sKeyRpt = paintObjAspect.getTag();

            paintObjAspect.setText(TxEdit.Text);

            m_report.getControls().item(sKeyRpt).getLabel().setText(paintObjAspect.getText());
            refreshBody();
             */ 
        }

        private String paintSection(cReportAspect aspect, 
                                    String sKey, 
			                        csRptTypeSection rptType, 
                                    String text, 
                                    bool isSecLn) 
        { 

            CSReportPaint.cReportPaintObject paintObj = null;
            paintObj = m_paint.getNewSection(CSReportPaint.csRptPaintObjType.CSRPTPAINTOBJBOX);

            cReportAspect w_aspect = paintObj.getAspect();

            // we only draw the bottom line of the sections
            //
            w_aspect.setLeft(0);
            w_aspect.setTop(aspect.getTop() + aspect.getHeight() - cGlobals.C_HEIGHT_BAR_SECTION);
            w_aspect.setWidth(aspect.getWidth());
            w_aspect.setHeight(cGlobals.C_HEIGHT_BAR_SECTION);

            if (isSecLn) {
                w_aspect.setBackColor(0xffcc99);
                w_aspect.setBorderColor(Color.Red.ToArgb());
            } 
            else {
                const int innerColor = 0x99ccff;

                if (rptType == csRptTypeSection.GROUP_FOOTER 
                    || rptType == csRptTypeSection.GROUP_HEADER) {
                    w_aspect.setBackColor(innerColor);
                    w_aspect.setBorderColor(0xC0C000);
                } 
                else {
                    w_aspect.setBackColor(innerColor);
                    w_aspect.setBorderColor(0x0066cc);
                }
            }

            if (rptType == csRptTypeSection.MAIN_FOOTER 
                || rptType == csRptTypeSection.FOOTER) {
                w_aspect.setOffset(m_offSet);
            }

            paintObj.setIsSection(!isSecLn);

            paintObj.setRptType(rptType);
            paintObj.setTag(sKey);

            paintObj.setText(text);

            return paintObj.getKey();
        }

        private bool getLineRegionForControl(String sKeyPaintObj, 
                                                out cReportSectionLine rptSecLine, 
                                                bool isFreeCtrl) 
        { 
            cReportSection rptSection = null;
	
            rptSecLine = null;

            if (!getRegionForControl(sKeyPaintObj, out rptSection, isFreeCtrl)) { return false; }

            float w1 = 0;
            float w2 = 0;

            float y = 0;

            cReportSectionLine rtnSecLine = null;

            cReportAspect w_aspect = m_paint.getPaintObject(sKeyPaintObj).getAspect();
            if (isFreeCtrl) {
                y = w_aspect.getTop() + w_aspect.getOffset();
            } 
            else {
                y = w_aspect.getTop() + w_aspect.getHeight() / 2 + w_aspect.getOffset();
            }

            for (int _i = 0; _i < rptSection.getSectionLines().count(); _i++) {
                cReportSectionLine rptSL = rptSection.getSectionLines().item(_i);
                w_aspect = rptSL.getAspect();
                w1 = w_aspect.getTop();
                w2 = w_aspect.getTop() + w_aspect.getHeight();
                if (isFreeCtrl) {
                    //
                    // if the control is a free control
                    // this function will return the last sectionLine which
                    // has a bottom bigger than the top of the control
                    //
                    if (w1 <= y) {
                        rtnSecLine = rptSL;
                    }
                } 
                else {
                    //
                    // if the control is not a free control
                    // this function will return the section line
                    // which contains the control
                    //
                    if (w1 <= y && w2 >= y) {
                        rtnSecLine = rptSL;
                        break;
                    }
                }
            }

            //
            // if the control is not a free contrl and there wasn't a
            // section line which contained the top of the control
            // (I think that can't be posible but anyways)
            // this function will return false and rptSecLine will be null
            //

            if (rtnSecLine != null) {
                rptSecLine = rtnSecLine;
                return true;
            }
            else {
                return false;
            }
        }

        private bool getRegionForControl(String sKeyPaintObj, out cReportSection rptSection, bool isFreeCtrl) 
        { 
            float x = 0;
            float y = 0;

            cReportAspect w_aspect = m_paint.getPaintObject(sKeyPaintObj).getAspect();
                
            // Headers
            //
            x = w_aspect.getLeft();
            if (isFreeCtrl) {
                y = w_aspect.getTop();
            } 
            else {
                y = w_aspect.getTop() + w_aspect.getHeight() / 2;
            }

            if (getRegionForControlAux(m_report.getHeaders(), x, y, out rptSection, isFreeCtrl)) {
                w_aspect.setOffset(0);
                return true;
            }

            // Groups Headers
            //
            if (getRegionForControlAux(m_report.getGroupsHeaders(), x, y, out rptSection, isFreeCtrl)) {
                w_aspect.setOffset(0);
                return true;
            }

            // Details
            //
            if (getRegionForControlAux(m_report.getDetails(), x, y, out rptSection, isFreeCtrl)) {
                w_aspect.setOffset(0);
                return true;
            }

            // Groups Footers
            //
            if (getRegionForControlAux(m_report.getGroupsFooters(), x, y, out rptSection, isFreeCtrl)) {
                w_aspect.setOffset(0);
                return true;
            }

            y = y + m_offSet;

            // Footers
            //
            if (getRegionForControlAux(m_report.getFooters(), x, y, out rptSection, isFreeCtrl)) {
                w_aspect.setOffset(m_offSet);
                return true;
            }

            return false;
        }

        private bool getRegionForControlAux(cIReportGroupSections rptSections, 
                                            float x, 
                                            float y, 
                                            out cReportSection rptSection, 
                                            bool isFreeCtrl) 
        {
            float y1 = 0;
            float y2 = 0;
            cReportSection rtnSec = null;

            rptSection = null;

            for (int _i = 0; _i < rptSections.count(); _i++) {
                
                cReportSection rptSec = rptSections.item(_i);
                cReportAspect w_aspect = rptSec.getAspect();

                y1 = w_aspect.getTop();
                y2 = w_aspect.getTop() + w_aspect.getHeight();

                if (isFreeCtrl) {
                    if (y1 <= y) {
                        rtnSec = rptSec;
                    }
                } 
                else {
                    if (y1 <= y && y2 >= y) {
                        rtnSec = rptSec;
                        break; 
                    }
                }
            }

            if (rtnSec != null) {
                rptSection = rtnSec;
                return true;
            }
            else {
                return false;
            }
        }

        private void pChangeTopSection(cReportSection rptSec, 
                                        float offSetTopSection, 
                                        bool bChangeTop, 
                                        bool bZeroOffset) 
        { 
            float newTopCtrl = 0;
            float offSet = 0;
            float bottom = 0;
            float secTop = 0;
            float secLnHeigt = 0;
            float offSecLn = 0;
            cReportPaintObject paintSec;

            cReportAspect secAspect = rptSec.getAspect();
            secAspect.setTop(secAspect.getTop() + offSetTopSection);
            offSet = rptSec.getSectionLines().item(0).getAspect().getTop() - secAspect.getTop();
            secTop = secAspect.getTop();

            for (int _i = 0; _i < rptSec.getSectionLines().count(); _i++) {

                cReportSectionLine rptSecLine = rptSec.getSectionLines().item(_i);

                cReportAspect secLineAspect = rptSecLine.getAspect();

                // footers grow to top
                //
                if (rptSec.getTypeSection() == csRptTypeSection.MAIN_FOOTER 
                    || rptSec.getTypeSection() == csRptTypeSection.FOOTER) {

                    if (bChangeTop) {

                        if (bZeroOffset) {
                            offSet = 0;
                        }

                    } 
                    else {

                        if (rptSecLine.getRealIndex() >= m_indexSecLnMoved && m_indexSecLnMoved > 0) {

                            bChangeTop = true;
                        }

                    }

                    // every other section grow to bottom
                    //
                } 
                else {
                    offSecLn = (secTop + secLnHeigt) - secLineAspect.getTop();

                    if (offSetTopSection != 0) {
                        offSecLn = 0;
                    }
                }

                secLineAspect.setTop(secTop + secLnHeigt);
                secLnHeigt = secLnHeigt + secLineAspect.getHeight();

                if (rptSecLine.getKeyPaint() != "") {
                    paintSec = m_paint.getPaintSections().item(rptSecLine.getKeyPaint());
                    paintSec.getAspect().setTop(secLineAspect.getTop() + secLineAspect.getHeight() - cGlobals.C_HEIGHT_BAR_SECTION);
                } 
                else {
                    paintSec = m_paint.getPaintSections().item(rptSec.getKeyPaint());
                }
                if (paintSec != null) {
                    paintSec.setHeightSecLine(secLineAspect.getHeight());
                }

                for (int _j = 0; _j < rptSecLine.getControls().count(); _j++) {
                    cReportControl rptCtrl = rptSecLine.getControls().item(_j);

                    cReportAspect ctrLabelAspect = rptCtrl.getLabel().getAspect();

                    if (rptCtrl.getIsFreeCtrl()) {
                        newTopCtrl = (ctrLabelAspect.getTop() - offSet) + offSecLn;
                    } 
                    else {
                        newTopCtrl = (ctrLabelAspect.getTop() + ctrLabelAspect.getHeight() - offSet) + offSecLn;
                    }

                    bottom = secLineAspect.getTop() + secLineAspect.getHeight();

                    if (newTopCtrl > bottom) {
                        newTopCtrl = bottom - ctrLabelAspect.getHeight();
                    } 
                    else {
                        newTopCtrl = (ctrLabelAspect.getTop() - offSet) + offSecLn;
                    }

                    if (newTopCtrl < secLineAspect.getTop()) { newTopCtrl = secLineAspect.getTop(); }

                    ctrLabelAspect.setTop(newTopCtrl);
                    if (m_paint.getPaintObject(rptCtrl.getKeyPaint()) != null) {
                        m_paint.getPaintObject(rptCtrl.getKeyPaint()).getAspect().setTop(ctrLabelAspect.getTop());
                    }
                }
            }

            // when a group is added the first to get here is the header
            // and the footer has not have a section yet
            //
            if (rptSec.getKeyPaint() == "") { return; }

            cReportAspect w_aspect = rptSec.getAspect();
            
            // we only draw the bottom line of the sections
            //
            paintSec = m_paint.getPaintSections().item(rptSec.getKeyPaint());

            if (paintSec != null) {
                paintSec.getAspect().setTop(w_aspect.getTop() 
                                            + w_aspect.getHeight() 
                                            - cGlobals.C_HEIGHT_BAR_SECTION);
                paintSec.setHeightSec(w_aspect.getHeight());
            }
        }

        private void moveSection(CSReportPaint.cReportPaintObject paintObj, 
                                    float x, 
                                    float y, 
                                    float minBottom, 
                                    float maxBottom, 
                                    cReportSection secToMove, 
                                    bool isNew) 
        { 
            float oldHeight = 0;

            m_dataHasChanged = true;

            cReportAspect w_aspect = paintObj.getAspect();

            // if Y is contained by the allowed range everything is ok
            //
            if (y >= minBottom && y <= maxBottom) {
                w_aspect.setTop(y - m_offY);

                // because the top has been set to real dimensions
                // of the screen we must move to the offset
                // of its section
                //
                w_aspect.setTop(w_aspect.getTop() + w_aspect.getOffset());
            } 
            else {
                // if we have moved to top
                //
                if (y < minBottom) {
                    w_aspect.setTop(minBottom);

                    // because the top has been set to real dimensions
                    // of the screen we must move to the offset
                    // of his section
                    //
                    w_aspect.setTop(w_aspect.getTop() + w_aspect.getOffset());
                } 
                else {
                    w_aspect.setTop(maxBottom);
                }
            }

            // TODO: remove after more testing - aligning the sections has an undesired result: the last section line is shrinked after five resize actions
            //
            // m_paint.alingToGrid(paintObj.getKey());

            if (isNew) {
                oldHeight = 0;
            } 
            else {
                oldHeight = secToMove.getAspect().getHeight();
            }

            // for the detail section and every other section which is over the detail
            // we only change the height, for all sections bellow the detail we need to
            // change the height and top becasuse wen we strech a section it needs to move
            // to the bottom of the report
            //
            secToMove.getAspect().setHeight(w_aspect.getTop() 
                                            + cGlobals.C_HEIGHT_BAR_SECTION 
                                            - secToMove.getAspect().getTop());

            // every section bellow this section needs to update its top
            //
            float offsetTop = 0;

            w_aspect = secToMove.getAspect();

            offsetTop = oldHeight - (w_aspect.getHeight() + m_newSecLineOffSet);

            switch (secToMove.getTypeSection()) {

                    // if the section is a footer we move to bottom
                    // (OJO: footer sections, no group footers)
                    //
                case  csRptTypeSection.FOOTER:
                case csRptTypeSection.MAIN_FOOTER:

                    w_aspect.setTop(w_aspect.getTop() + offsetTop);

                    // OJO: this has to be after we have changed the top of the section
                    //      to allow the paint object to reflect the change
                    //
                    // we move the controls of this section
                    //
                    pChangeHeightSection(secToMove, oldHeight);

                    // move the section
                    //
                    pChangeBottomSections(secToMove, offsetTop);

                    // for headers, group headers, group footers and the detail section we move to top
                    //
                    break;
                default:

                    // move all controls in this section
                    //
                    pChangeHeightSection(secToMove, oldHeight);

                    offsetTop = offsetTop * -1;

                    pChangeTopSections(secToMove, offsetTop);
                    break;
            }

            // finaly we need to update the offset of every section,
            // apply it to every object paint in m_Paint
            //
            float pageHeight = 0;
            cReportPaperInfo w_paperInfo = m_report.getPaperInfo();
            pGetOffSet(CSReportPaint.cGlobals.getRectFromPaperSize(
                                                        m_report.getPaperInfo(), 
                                                        w_paperInfo.getPaperSize(), 
                                                        w_paperInfo.getOrientation()).Height, 
                                                        pageHeight);
            pRefreshOffSetInPaintObjs();
            m_paint.setGridHeight(pageHeight);
        }

        private void pChangeBottomSections(cReportSection secToMove, float offsetTop) { 

            cReportSection sec = null;
            bool bChangeTop = false;

            if (secToMove.getTypeSection() == csRptTypeSection.FOOTER 
                || secToMove.getTypeSection() == csRptTypeSection.MAIN_FOOTER 
                || bChangeTop) {

                for (int i = m_report.getFooters().count()-1; i > -1; i--) {
                    sec = m_report.getFooters().item(i);

                    if (bChangeTop) {
                        pChangeTopSection(sec, offsetTop, bChangeTop, false);
                    }

                    if (sec == secToMove) {
                        bChangeTop = true;
                    }
                }
            }
        }

        private void pChangeTopSections(cReportSection secToMove, float offsetTop) { 

            cReportSection sec = null;
            bool bChangeTop = false;

            if (secToMove.getTypeSection() == csRptTypeSection.HEADER 
                || secToMove.getTypeSection() == csRptTypeSection.MAIN_HEADER) {

                for (int _i = 0; _i < m_report.getHeaders().count(); _i++) {
                    sec = m_report.getHeaders().item(_i);
                    if (bChangeTop) {
                        pChangeTopSection(sec, offsetTop, bChangeTop, false);
                    }

                    if (sec == secToMove) {
                        bChangeTop = true;
                    }
                }
            }

            if (secToMove.getTypeSection() == csRptTypeSection.GROUP_HEADER || bChangeTop) {

                for (int _i = 0; _i < m_report.getGroupsHeaders().count(); _i++) {
                    sec = m_report.getGroupsHeaders().item(_i);
                    if (bChangeTop) {
                        pChangeTopSection(sec, offsetTop, bChangeTop, false);
                    }

                    if (sec == secToMove) {
                        bChangeTop = true;
                    }
                }
            }

            if (secToMove.getTypeSection() == csRptTypeSection.MAIN_DETAIL 
                || secToMove.getTypeSection() == csRptTypeSection.DETAIL || bChangeTop) {

                for (int _i = 0; _i < m_report.getDetails().count(); _i++) {
                    sec = m_report.getDetails().item(_i);
                    if (bChangeTop) {
                        pChangeTopSection(sec, offsetTop, bChangeTop, false);
                    }

                    if (sec == secToMove) {
                        bChangeTop = true;
                    }
                }
            }

            if (secToMove.getTypeSection() == csRptTypeSection.GROUP_FOOTER || bChangeTop) {

                for (int _i = 0; _i < m_report.getGroupsFooters().count(); _i++) {
                    sec = m_report.getGroupsFooters().item(_i);
                    if (bChangeTop) {
                        pChangeTopSection(sec, offsetTop, bChangeTop, false);
                    }

                    if (sec == secToMove) {
                        bChangeTop = true;
                    }
                }
            }
        }

        private void pChangeHeightSection(cReportSection sec, float oldSecHeight) {
            float heightLines = 0;
            cReportAspect w_aspect;

            // Update section line
            //
            for (int i = 0; i < sec.getSectionLines().count() - 1; i++) {
                w_aspect = sec.getSectionLines().item(i).getAspect();
                heightLines = heightLines + w_aspect.getHeight();
            }

            // for the last section line the height is the rest
            //
            cReportSectionLines w_sectionLines = sec.getSectionLines();
            w_aspect = w_sectionLines.item(w_sectionLines.count()-1).getAspect();
            w_aspect.setHeight(sec.getAspect().getHeight() - heightLines);

            pChangeTopSection(sec, 0, false, true);
        }

        private void reLoadReport() {

            cReportPaintObject paintSec = null;

            m_paint = null;

            m_keyMoving = "";
            m_keySizing = "";
            m_keyObj = "";
            m_keyFocus = "";
            m_moveType = csRptEditorMoveType.CSRPTEDMOVTNONE;

            m_paint = new CSReportPaint.cReportPaint();

            cReportPaperInfo w_paperInfo = m_report.getPaperInfo();
            m_paint.setGridHeight(
                    pSetSizePics(CSReportPaint.cGlobals.getRectFromPaperSize(
                                                                m_report.getPaperInfo(), 
                                                                w_paperInfo.getPaperSize(), 
                                                                w_paperInfo.getOrientation()).Height));

            m_paint.initGrid(m_picReport.CreateGraphics(), m_typeGrid);

            if (m_report.getName() != "") {
                m_editorTab.Text = m_report.getName() + "   [X]";
            }

            cReportSection sec = null;

            for (int _i = 0; _i < m_report.getHeaders().count(); _i++) {
                sec = m_report.getHeaders().item(_i);
                sec.setKeyPaint(paintSection(sec.getAspect(), 
                                                sec.getKey(), 
                                                sec.getTypeSection(), 
                                                sec.getName(), 
                                                false));
                paintSec = m_paint.getPaintSections().item(sec.getKeyPaint());
                paintSec.setHeightSec(sec.getAspect().getHeight());
                pAddPaintSetcionForSecLn(sec, csRptTypeSection.SECLN_HEADER);
            }

            for (int _i = 0; _i < m_report.getGroupsHeaders().count(); _i++) {
                sec = m_report.getGroupsHeaders().item(_i);
                sec.setKeyPaint(paintSection(sec.getAspect(), 
                                                sec.getKey(), 
                                                sec.getTypeSection(), 
                                                sec.getName(), 
                                                false));
                paintSec = m_paint.getPaintSections().item(sec.getKeyPaint());
                paintSec.setHeightSec(sec.getAspect().getHeight());
                pAddPaintSetcionForSecLn(sec, csRptTypeSection.SECLN_GROUPH);
            }

            for (int _i = 0; _i < m_report.getDetails().count(); _i++) {
                sec = m_report.getDetails().item(_i);
                sec.setKeyPaint(paintSection(sec.getAspect(), 
                                                sec.getKey(), 
                                                sec.getTypeSection(), 
                                                sec.getName(), 
                                                false));
                paintSec = m_paint.getPaintSections().item(sec.getKeyPaint());
                paintSec.setHeightSec(sec.getAspect().getHeight());
                pAddPaintSetcionForSecLn(sec, csRptTypeSection.SECLN_DETAIL);
            }

            for (int _i = 0; _i < m_report.getGroupsFooters().count(); _i++) {
                sec = m_report.getGroupsFooters().item(_i);
                sec.setKeyPaint(paintSection(sec.getAspect(), 
                                                sec.getKey(), 
                                                sec.getTypeSection(), 
                                                sec.getName(), 
                                                false));
                paintSec = m_paint.getPaintSections().item(sec.getKeyPaint());
                paintSec.setHeightSec(sec.getAspect().getHeight());
                pAddPaintSetcionForSecLn(sec, csRptTypeSection.SECLN_GROUPF);
            }

            for (int _i = 0; _i < m_report.getFooters().count(); _i++) {
                sec = m_report.getFooters().item(_i);
                sec.setKeyPaint(paintSection(sec.getAspect(), 
                                                sec.getKey(), 
                                                sec.getTypeSection(), 
                                                sec.getName(), 
                                                false));
                paintSec = m_paint.getPaintSections().item(sec.getKeyPaint());
                paintSec.setHeightSec(sec.getAspect().getHeight());
                pAddPaintSetcionForSecLn(sec, csRptTypeSection.SECLN_FOOTER);
            }

            CSReportPaint.csRptPaintObjType paintType;

            for (int _i = 0; _i < m_report.getControls().count(); _i++) {

                cReportControl rptCtrl = m_report.getControls().item(_i);
                refreshNextNameCtrl(rptCtrl.getName());
                cReportAspect ctrlAspect = rptCtrl.getLabel().getAspect();

                if (rptCtrl.getControlType() == csRptControlType.CSRPTCTIMAGE 
                    || rptCtrl.getControlType() == csRptControlType.CSRPTCTCHART) {
                    paintType = CSReportPaint.csRptPaintObjType.CSRPTPAINTOBJIMAGE;
                } 
                else {
                    paintType = CSReportPaint.csRptPaintObjType.CSRPTPAINTOBJBOX;
                }

                CSReportPaint.cReportPaintObject paintObj = m_paint.getNewObject(paintType);

                // for old reports
                //
                ctrlAspect.setTransparent(ctrlAspect.getBackColor() == Color.White.ToArgb());

                paintObj.setImage(rptCtrl.getImage().getImage());

                cReportAspect w_aspect = paintObj.getAspect();
                w_aspect.setLeft(ctrlAspect.getLeft());
                w_aspect.setTop(ctrlAspect.getTop());
                w_aspect.setWidth(ctrlAspect.getWidth());
                w_aspect.setHeight(ctrlAspect.getHeight());
                w_aspect.setBackColor(ctrlAspect.getBackColor());
                w_aspect.setTransparent(ctrlAspect.getTransparent());
                w_aspect.setAlign(ctrlAspect.getAlign());
                w_aspect.setWordWrap(ctrlAspect.getWordWrap());

                if (ctrlAspect.getBorderType() == csReportBorderType.CSRPTBSNONE) {
                    w_aspect.setBorderColor(Color.Black.ToArgb());
                    w_aspect.setBorderWidth(1);
                    w_aspect.setBorderRounded(false);
                    w_aspect.setBorderType(csReportBorderType.CSRPTBSFIXED);
                } 
                else {
                    w_aspect.setBorderType(ctrlAspect.getBorderType());
                    w_aspect.setBorderColor(ctrlAspect.getBorderColor());
                    w_aspect.setBorderColor3d(ctrlAspect.getBorderColor3d());
                    w_aspect.setBorderColor3dShadow(ctrlAspect.getBorderColor3dShadow());
                    w_aspect.setBorderRounded(ctrlAspect.getBorderRounded());
                    w_aspect.setBorderWidth(ctrlAspect.getBorderWidth());
                }

                switch (rptCtrl.getSectionLine().getTypeSection()) {
                    case  csRptTypeSection.FOOTER:
                    case  csRptTypeSection.MAIN_FOOTER:
                        w_aspect.setOffset(m_offSet);
                        break;
                }

                cReportFont w_font = w_aspect.getFont();
                w_font.setName(ctrlAspect.getFont().getName());
                w_font.setForeColor(ctrlAspect.getFont().getForeColor());
                w_font.setSize(ctrlAspect.getFont().getSize());
                w_font.setBold(ctrlAspect.getFont().getBold());
                w_font.setItalic(ctrlAspect.getFont().getItalic());
                w_font.setUnderline(ctrlAspect.getFont().getUnderline());
                w_font.setStrike(ctrlAspect.getFont().getStrike());

                paintObj.setText(rptCtrl.getLabel().getText());
                paintObj.setRptType(csRptTypeSection.CONTROL);
                paintObj.setTag(rptCtrl.getKey());
                rptCtrl.setKeyPaint(paintObj.getKey());
            }

            m_dataHasChanged = false;

            m_paint.createPicture(m_picReport.CreateGraphics());

            m_picRule.Refresh();
        }

        private void pAddPaintSetcionForSecLn(
            cReportSection sec, 
			csRptTypeSection typeSecLn) 
        { 
            cReportPaintObject paintSec = null;

            if (sec.getSectionLines().count() > 1) {

                for (int i = 0; i < sec.getSectionLines().count() - 1; i++) {
                    cReportSectionLine secLine = sec.getSectionLines().item(i);
                    secLine.setKeyPaint(
                        paintSection(
                            secLine.getAspect(), 
                            secLine.getKey(), 
                            sec.getTypeSection(), 
                            C_SECTIONLINE + i.ToString(), 
                            true));

                    // we set the height of every section line
                    //
                    paintSec = m_paint.getPaintSections().item(secLine.getKeyPaint());
                    paintSec.setHeightSecLine(secLine.getAspect().getHeight());
                    paintSec.setRptType(typeSecLn);
                    paintSec.setRptKeySec(sec.getKey());
                }

                // if there is more than one section we use
                // textLine to show the name of the last line
                //
                CSReportPaint.cReportPaintObject po = m_paint.getPaintSections().item(sec.getKeyPaint());
                po.setTextLine(C_SECTIONLINE + (sec.getSectionLines().count() - 1).ToString());
            }

            // we set the height of the last section line
            //
            paintSec = m_paint.getPaintSections().item(sec.getKeyPaint());

            cReportSectionLines secLines = sec.getSectionLines();
            paintSec.setHeightSecLine(secLines.item(secLines.count() - 1).getAspect().getHeight());
        }

        private void refreshNextNameCtrl(String nameCtrl) {
            int x = 0;
            if (cUtil.subString(nameCtrl, 0, cGlobals.C_CONTROL_NAME.Length).ToUpper() == cGlobals.C_CONTROL_NAME.ToUpper())
            {
                x = cUtil.valAsInt(nameCtrl.Substring(cGlobals.C_CONTROL_NAME.Length + 1));
                if (x > m_nextNameCtrl) {
                    m_nextNameCtrl = x + 1;
                }
            }
        }

        private void moveControl(String sKeyPaintObj) {
            cReportSectionLine rptSecLine = null;
            cReportControl rptCtrl = null;
            cReportAspect rptSecLineAspect = null;
            cReportAspect objPaintAspect = null;

            m_paint.alingToGrid(sKeyPaintObj);

            rptCtrl = m_report.getControls().item(m_paint.getPaintObject(sKeyPaintObj).getTag());

            objPaintAspect = m_paint.getPaintObject(sKeyPaintObj).getAspect();

            if (rptCtrl == null) { return; }

            cReportAspect w_aspect = rptCtrl.getLabel().getAspect();
            w_aspect.setTop(objPaintAspect.getTop() + objPaintAspect.getOffset());
            w_aspect.setHeight(objPaintAspect.getHeight());
            w_aspect.setWidth(objPaintAspect.getWidth());
            w_aspect.setLeft(objPaintAspect.getLeft());

            if (getLineRegionForControl(sKeyPaintObj, out rptSecLine, rptCtrl.getIsFreeCtrl())) {

                if (!(rptSecLine == rptCtrl.getSectionLine())) {
                    rptCtrl.getSectionLine().getControls().remove(rptCtrl.getKey());
                    rptSecLine.getControls().add(rptCtrl, rptCtrl.getKey());
                }

                // we need to check the control is between the limits of the section
                // in which it is contained
                //
                rptSecLineAspect = rptCtrl.getSectionLine().getAspect();

                w_aspect = rptCtrl.getLabel().getAspect();

                w_aspect.setTop(objPaintAspect.getTop() + objPaintAspect.getOffset());

                if (!rptCtrl.getIsFreeCtrl()) {
                    if (w_aspect.getTop() + w_aspect.getHeight() 
                        > rptSecLineAspect.getTop() + rptSecLineAspect.getHeight()) {
                        w_aspect.setTop(rptSecLineAspect.getTop() 
                                        + rptSecLineAspect.getHeight() 
                                        - w_aspect.getHeight());
                    }
                }

                if (w_aspect.getTop() < rptSecLineAspect.getTop()) {
                    w_aspect.setTop(rptSecLineAspect.getTop());
                }

                objPaintAspect.setTop(w_aspect.getTop());
            }
        }

        private void showPopMenuSection(bool noDelete, bool showGroups, int x, int y) {
            m_fmain.showPopMenuSection(this, noDelete, showGroups, m_picReport.PointToScreen(new Point(x, y)));
        }

        private void showPopMenuControl(bool clickInCtrl, int x, int y) {

            bool pasteEnabled = false;

            if (m_vCopyKeys.Length > 0) {
                pasteEnabled = true;
            } 
            else if (!(m_fmain.getReportCopySource() == null)) {
                pasteEnabled = m_fmain.getReportCopySource().getVCopyKeysCount() > 0;
            }

            m_fmain.showPopMenuControl(this, clickInCtrl, pasteEnabled, m_picReport.PointToScreen(new Point(x, y)));
        }

        private void m_fGroup_UnloadForm() {
            m_fGroup = null;
        }

        public void destroyPropertiesFormReference() {
            m_fProperties = null;
        }

        private void refreshBody() {
            try {

                m_paint.endMove(m_picReport.CreateGraphics());

            } catch (Exception ex) {
                cError.mngError(ex, "ShowConnectsAux", C_MODULE, "");
            }
        }

        private void refreshRule() {
            m_picRule.Refresh();
        }

        public void refreshReport() {

            cReportPaperInfo w_paperInfo = m_report.getPaperInfo();
            m_paint.setGridHeight(pSetSizePics(
                                        CSReportPaint.cGlobals.getRectFromPaperSize(
                                                                    m_report.getPaperInfo(), 
                                                                    w_paperInfo.getPaperSize(), 
                                                                    w_paperInfo.getOrientation()).Height));
            pValidateSectionAspect();
            reLoadReport();
        }

        // TODO: remove me if not needed
        public void refreshPostion() {
        }

        public void refreshAll() {
            refreshBody();
            refreshRule();
        }

        private void reportDone(object sender, EventArgs e)
        {
            closeProgressDlg();
        }

        private void reportProgress(object sender, ProgressEventArgs e) 
        { 

            String task = e.task; 
            int page = e.page; 
            int currRecord = e.currRecord; 
            int recordCount = e.recordCount; 

            if (m_cancelPrinting) {
                if (cWindow.ask("Confirm you want to cancel the execution of this report?", MessageBoxDefaultButton.Button2)) {
                    e.cancel = true;
                    closeProgressDlg();
                    return;
                } 
                else {
                    m_cancelPrinting = false;
                }
            }

            if (m_fProgress == null) { return; }

            if (page > 0) { m_fProgress.lbCurrPage.Text = page.ToString(); }
            if (task != "") { m_fProgress.lbTask.Text = task; }
            if (currRecord > 0) { m_fProgress.lbCurrRecord.Text = currRecord.ToString(); }
            if (recordCount > 0 && cUtil.val(m_fProgress.lbRecordCount.Text) != recordCount) { 
                m_fProgress.lbRecordCount.Text = recordCount.ToString(); 
            }

            double percent = 0;
            if (recordCount > 0 && currRecord > 0) {
                percent = Convert.ToDouble(currRecord) / recordCount;
                m_fProgress.prgBar.Value = Convert.ToInt32(percent * 100);
            }

            Application.DoEvents();
        }

        private void closeProgressDlg() {
            if (m_fProgress != null && !m_fProgress.IsDisposed)
            {
                m_fProgress.Close();
            }
            m_fProgress = null;
        }

        private void showProgressDlg() {
            m_cancelPrinting = false;
            if (m_fProgress == null) { 
                m_fProgress = new fProgress();
                // TODO: add event for m_report_Progress
            }
            m_fProgress.Show();
            m_fProgress.BringToFront();
        }

        private void m_fProgress_Cancel() {
            m_cancelPrinting = true;
        }

        /* TODO: implement me
        private void m_report_FindFileAccess(
            out bool answer, 
            object commDialog, 
            String file) 
        { 
            String msg = "";
            msg = "The " + file + " could not be found. ¿Do you want to find it?";
            if (!cWindow.ask(msg, VbMsgBoxResult.vbYes)) { return; }

            commDialog = m_fmain.cmDialog;
            answer = true;
            m_fProgress.BringToFront();
            m_dataHasChanged = true;
        }
         */ 

        /* TODO: implement me
        private void txEdit_KeyPress(int keyAscii) {
            if (keyAscii == vbKeyEscape) {
                endEditText(keyAscii == vbKeyEscape);
                keyAscii = 0;
            }
        }
         */

        private int pGetLeftBody() {
            if (cMainEditor.gHideLeftBar) {
                return C_LEFTBODY;
            } 
            else {
                return m_picRule.Width + C_LEFTBODY;
            }
        }

        private float pSetSizePics(float realPageHeight) {
            float pageHeight = 0;

            cReportPaperInfo w_paperInfo = m_report.getPaperInfo();
            m_picReport.Width = (int)CSReportPaint.cGlobals.getRectFromPaperSize(
                                                                m_report.getPaperInfo(), 
                                                                w_paperInfo.getPaperSize(), 
                                                                w_paperInfo.getOrientation()).Width;
            pGetOffSet(realPageHeight, pageHeight);

            if (pageHeight > realPageHeight) { realPageHeight = pageHeight; }

            m_picReport.Height = (int)realPageHeight;
            m_picRule.Height = (int)(realPageHeight + C_TOPBODY * 2);

            return pageHeight;
        }

        private void pMoveAll(float x, float y) {
            cReportAspect rptCtrlAspect = null;
            CSReportPaint.cReportPaintObject paintObj = null;

            m_dataHasChanged = true;

            if (m_bNoMove) { return; }

            int i = 0;
            float offsetTop = 0;
            float offsetLeft = 0;
            float firstLeft = 0;
            float firstTop = 0;
            float firstOffSet = 0;

            if (m_vSelectedKeys.Length == 0) { return; }

            paintObj = m_paint.getPaintObject(m_keyMoving);

            cReportAspect w_aspect = paintObj.getAspect();
            firstLeft = w_aspect.getLeft();
            firstTop = w_aspect.getTop();
            firstOffSet = w_aspect.getOffset();

            for (i = m_vSelectedKeys.Length-1; i > -1; i--) {

                paintObj = m_paint.getPaintObject(m_vSelectedKeys[i]);

                offsetLeft = pGetOffsetLeftFromControls(firstLeft, 
                                                        paintObj.getAspect().getLeft());

                offsetTop = pGetOffsetTopFromControls(firstTop - firstOffSet, 
                                                        paintObj.getAspect().getTop() 
                                                        - paintObj.getAspect().getOffset());

                w_aspect = paintObj.getAspect();

                if (x != C_NOMOVE) {
                    w_aspect.setLeft(x - m_offX + offsetLeft);
                }

                if (y != C_NOMOVE) {
                    w_aspect.setTop(y - m_offY + offsetTop);
                } 
                else {

                    // we get rid off the offset because the primitive
                    // add it to the top but we don't allow vertical
                    // moves so Y must to remain constant
                    //
                    w_aspect.setTop(w_aspect.getTop() - paintObj.getAspect().getOffset());
                }

                // only controls move in all directions
                // 
                if (paintObj.getRptType() == csRptTypeSection.CONTROL) {
                    rptCtrlAspect = m_report.getControls().item(paintObj.getTag()).getLabel().getAspect();
                    rptCtrlAspect.setLeft(w_aspect.getLeft());
                    rptCtrlAspect.setTop(w_aspect.getTop());
                    rptCtrlAspect.setWidth(w_aspect.getWidth());
                    rptCtrlAspect.setHeight(w_aspect.getHeight());
                }

                moveControl(m_vSelectedKeys[i]);
            }
        }

        private void pMoveHorizontal(float x) {
            m_dataHasChanged = true;
            m_paint.getPaintObject(m_keyMoving).getAspect().setLeft(x - m_offX);
        }

        private void pMoveVertical(float x, float y) {
            String sKeySection = "";
            csRptTypeSection rptType;

            float maxBottom = 0;
            float minBottom = 0;

            float maxBottomSectionLine = 0;

            cReportSection rptSec = null;
            CSReportPaint.cReportPaintObject paintObj = null;
            bool isSecLn = false;

            m_indexSecLnMoved = -1;

            paintObj = m_paint.getPaintObject(m_keyMoving);
            cReportAspect w_aspect = paintObj.getAspect();

            sKeySection = paintObj.getTag();

            // sections can only be move verticaly
            // always is the bottom of the section which is moved
            // every time we move a section the height change
            //
            rptType = paintObj.getRptType();

            switch (rptType) {

                    //---------------------
                    // HEADER
                    //---------------------

                case csRptTypeSection.MAIN_HEADER:
                case csRptTypeSection.HEADER:

                    rptSec = pMoveHeader(sKeySection, out minBottom, out maxBottom);

                    //---------------------
                    // GROUP HEADER
                    //---------------------

                    break;
                
                case  csRptTypeSection.GROUP_HEADER:

                    rptSec = pMoveGroupHeader(sKeySection, out minBottom, out maxBottom);

                    //---------------------
                    // DETAIL
                    //---------------------

                    break;
                
                case  csRptTypeSection.MAIN_DETAIL:
                case  csRptTypeSection.DETAIL:

                    rptSec = pMoveDetails(sKeySection, out minBottom, out maxBottom);

                    //---------------------
                    // GROUP FOOTER
                    //---------------------

                    break;
                
                case  csRptTypeSection.GROUP_FOOTER:

                    rptSec = pMoveGroupFooter(sKeySection, out minBottom, out maxBottom);

                    //---------------------
                    // FOOTER
                    //---------------------

                    break;
                
                case  csRptTypeSection.MAIN_FOOTER:
                case  csRptTypeSection.FOOTER:

                    rptSec = pMoveFooter(sKeySection, out minBottom, out maxBottom);

                    //---------------------
                    // Section Lines
                    //---------------------
                    break;

                case  csRptTypeSection.SECLN_HEADER:
                    sKeySection = paintObj.getRptKeySec();
                    rptSec = pMoveHeader(sKeySection, out minBottom, out maxBottom);
                    isSecLn = true;
                    break;

                case  csRptTypeSection.SECLN_GROUPH:
                    sKeySection = paintObj.getRptKeySec();
                    rptSec = pMoveGroupHeader(sKeySection, out minBottom, out maxBottom, true, paintObj.getTag(), out maxBottomSectionLine);
                    isSecLn = true;
                    break;

                case  csRptTypeSection.SECLN_DETAIL:
                    sKeySection = paintObj.getRptKeySec();
                    rptSec = pMoveDetails(sKeySection, out minBottom, out maxBottom, true, paintObj.getTag(), out maxBottomSectionLine);
                    isSecLn = true;
                    break;

                case  csRptTypeSection.SECLN_GROUPF:
                    sKeySection = paintObj.getRptKeySec();
                    rptSec = pMoveGroupFooter(sKeySection, out minBottom, out maxBottom, true, paintObj.getTag(), out maxBottomSectionLine);
                    isSecLn = true;
                    break;

                case  csRptTypeSection.SECLN_FOOTER:
                    sKeySection = paintObj.getRptKeySec();
                    rptSec = pMoveFooter(sKeySection, out minBottom, out maxBottom, true, paintObj.getTag(), out maxBottomSectionLine);
                    isSecLn = true;
                    m_indexSecLnMoved = rptSec.getSectionLines().item(paintObj.getTag()).getRealIndex();
                    break;
            }

            if (isSecLn) {
                minBottom = pGetMinBottomForSecLn(rptSec, paintObj.getTag(), minBottom);
                pChangeSecLnHeight(paintObj, 
                                    y, 
                                    minBottom,
                                    maxBottomSectionLine, 
                                    rptSec.getSectionLines().item(paintObj.getTag()));

                y = rptSec.getAspect().getTop() 
                    - paintObj.getAspect().getOffset() 
                    + pGetSecHeigthFromSecLines(rptSec) 
                    - cGlobals.C_HEIGHT_BAR_SECTION;

                m_offY = 0;
                paintObj = m_paint.getPaintSections().item(rptSec.getKeyPaint());
            }

            moveSection(paintObj, x, y, minBottom, maxBottom, rptSec, false);
        }

        private float pGetSecHeigthFromSecLines(cReportSection sec) {
            float rtn = 0;

            for (int _i = 0; _i < sec.getSectionLines().count(); _i++) {
                cReportSectionLine secLn = sec.getSectionLines().item(_i);
                rtn = rtn + secLn.getAspect().getHeight();
            }

            return rtn;
        }

        private float pGetMinBottomForSecLn(
            cReportSection sec, 
            String secLnKey, 
            float minBottom) 
        {
            for (int _i = 0; _i < sec.getSectionLines().count(); _i++) {
                cReportSectionLine secLn = sec.getSectionLines().item(_i);
                if (secLn.getKey() == secLnKey) { break; }
                minBottom = minBottom + secLn.getAspect().getHeight();
            }
            return minBottom;
        }

        private void pChangeSecLnHeight(
            CSReportPaint.cReportPaintObject paintObj, 
            float y, 
            float minBottom, 
            float maxBottom, 
            cReportSectionLine secLn) 
        { 
            cReportAspect w_aspect = paintObj.getAspect();

            // if Y is contained between the range allowed everything is ok
            //
            if (y >= minBottom && y <= maxBottom) {
                w_aspect.setTop(y - m_offY);
            } 
            else {
                // if it have been moved upward
                //
                if (y < minBottom) {
                    w_aspect.setTop(minBottom);

                } 
                // if it have been moved downward
                //
                else {
                    w_aspect.setTop(maxBottom);
                }
            }

            // because the top has been setted to the real dimensions
            // of the screen now we need to move it the offset
            // of its section
            //
            w_aspect.setTop(w_aspect.getTop() + w_aspect.getOffset());

            // TODO: remove after more testing - aligning the sections has an undesired result: the last section line is shrinked after five resize actions
            //
            // m_paint.alingToGrid(paintObj.getKey());

            // the section line height has been changed
            //
            secLn.getAspect().setHeight(w_aspect.getTop() 
                                        + cGlobals.C_HEIGHT_BAR_SECTION 
                                        - secLn.getAspect().getTop());
        }

        private void pResizeControl(float x, float y) {
            float height = 0;
            float width = 0;
            float left = 0;
            float top = 0;

            if (m_vSelectedKeys.Length == 0) { return; }

            m_dataHasChanged = true;

            // first we need to modify the control which has its size changed
            //
            cReportPaintObject w_getPaintObject = m_paint.getPaintObject(m_keySizing);
            cReportAspect w_aspect = w_getPaintObject.getAspect();

            // orginal size to know how much it has changed
            //
            height = w_aspect.getHeight();
            width = w_aspect.getWidth();
            left = w_aspect.getLeft();
            top = w_aspect.getTop();

            switch (m_moveType) {
                case  csRptEditorMoveType.CSRPTEDMOVDOWN:
                    w_aspect.setHeight(y - (w_aspect.getTop() - w_aspect.getOffset()));
                    break;
                case  csRptEditorMoveType.CSRPTEDMOVLEFT:
                    w_aspect.setWidth(w_aspect.getWidth() + w_aspect.getLeft() - x);
                    w_aspect.setLeft(x);
                    break;
                case  csRptEditorMoveType.CSRPTEDMOVRIGHT:
                    w_aspect.setWidth(x - w_aspect.getLeft());
                    break;
                case  csRptEditorMoveType.CSRPTEDMOVUP:
                    w_aspect.setHeight(w_aspect.getHeight() + (w_aspect.getTop() - w_aspect.getOffset()) - y);
                    w_aspect.setTop(y + w_aspect.getOffset());
                    break;
                case  csRptEditorMoveType.CSRPTEDMOVLEFTDOWN:
                    w_aspect.setHeight(y - (w_aspect.getTop() - w_aspect.getOffset()));
                    w_aspect.setWidth(w_aspect.getWidth() + w_aspect.getLeft() - x);
                    w_aspect.setLeft(x);
                    break;
                case  csRptEditorMoveType.CSRPTEDMOVLEFTUP:
                    w_aspect.setHeight(w_aspect.getHeight() + (w_aspect.getTop() - w_aspect.getOffset()) - y);
                    w_aspect.setTop(y + w_aspect.getOffset());
                    w_aspect.setWidth(w_aspect.getWidth() + w_aspect.getLeft() - x);
                    w_aspect.setLeft(x);
                    break;
                case  csRptEditorMoveType.CSRPTEDMOVRIGHTDOWN:
                    w_aspect.setWidth(x - w_aspect.getLeft());
                    w_aspect.setHeight(y - (w_aspect.getTop() - w_aspect.getOffset()));
                    break;
                case  csRptEditorMoveType.CSRPTEDMOVRIGHTUP:
                    w_aspect.setHeight(w_aspect.getHeight() + (w_aspect.getTop() - w_aspect.getOffset()) - y);
                    w_aspect.setTop(y + w_aspect.getOffset());
                    w_aspect.setWidth(x - w_aspect.getLeft());
                    break;
            }

            top = w_aspect.getTop() - top;
            left = w_aspect.getLeft() - left;
            width = w_aspect.getWidth() - width;
            height = w_aspect.getHeight() - height;

            pMoveControlAfterResize(w_getPaintObject.getAspect(), true);

            for (int i = 0; i < m_vSelectedKeys.Length; i++) {

                if (m_keySizing != m_vSelectedKeys[i]) {

                    w_getPaintObject = m_paint.getPaintObject(m_vSelectedKeys[i]);
                    w_aspect = w_getPaintObject.getAspect();

                    w_aspect.setHeight(w_aspect.getHeight() + height);
                    w_aspect.setTop(w_aspect.getTop() + top);
                    w_aspect.setWidth(w_aspect.getWidth() + width);
                    w_aspect.setLeft(w_aspect.getLeft() + left);

                    pMoveControlAfterResize(w_getPaintObject.getAspect(), false);
                }
            }
        }

        private void pMoveControlAfterResize(cReportAspect aspect, bool bSizing) { 
            const int C_MIN_WIDTH = 1;
            const int C_MIN_HEIGHT = 1;

            cReportAspect rptCtrlAspect = null;

            if (m_paint.getPaintObject(m_keySizing).getRptType() == csRptTypeSection.CONTROL) {
                rptCtrlAspect = m_report.getControls().item(m_paint.getPaintObject(m_keySizing).getTag()).getLabel().getAspect();
                rptCtrlAspect.setLeft(aspect.getLeft());
                if (!bSizing) {
                    rptCtrlAspect.setTop(aspect.getTop() + aspect.getOffset());
                } 
                else {
                    rptCtrlAspect.setTop(aspect.getTop());
                }
                rptCtrlAspect.setWidth(aspect.getWidth());
                rptCtrlAspect.setHeight(aspect.getHeight());
            }

            switch (m_moveType) {
                case  csRptEditorMoveType.CSRPTEDMOVDOWN:
                    m_paint.alingObjBottomToGrid(m_keySizing);
                    break;
                case  csRptEditorMoveType.CSRPTEDMOVLEFT:
                    m_paint.alingObjLeftToGrid(m_keySizing);
                    break;
                case  csRptEditorMoveType.CSRPTEDMOVRIGHT:
                    m_paint.alingObjRightToGrid(m_keySizing);
                    break;
                case  csRptEditorMoveType.CSRPTEDMOVUP:
                    m_paint.alingObjTopToGrid(m_keySizing);
                    break;
                case  csRptEditorMoveType.CSRPTEDMOVLEFTDOWN:
                    m_paint.alingObjLeftBottomToGrid(m_keySizing);
                    break;
                case  csRptEditorMoveType.CSRPTEDMOVLEFTUP:
                    m_paint.alingObjLeftTopToGrid(m_keySizing);
                    break;
                case  csRptEditorMoveType.CSRPTEDMOVRIGHTDOWN:
                    m_paint.alingObjRightBottomToGrid(m_keySizing);
                    break;
                case  csRptEditorMoveType.CSRPTEDMOVRIGHTUP:
                    m_paint.alingObjRightTopToGrid(m_keySizing);
                    break;
            }

            // Validations

            // Width can't be lower than C_MIN_WIDTH
            //
            if (aspect.getWidth() < C_MIN_WIDTH) { aspect.setWidth(C_MIN_WIDTH); }
            
            // Height can't be lower than C_MIN_HEIGHT
            //
            if (aspect.getHeight() < C_MIN_HEIGHT) { aspect.setHeight(C_MIN_HEIGHT); }
        }

        private cReportSection pMoveHeader(
            String sKeySection,
            out float minBottom,
            out float maxBottom)
        {
            float dummy;
            return pMoveHeader(sKeySection, out minBottom, out maxBottom, false, "", out dummy);
        }

        private cReportSection pMoveHeader(
            String sKeySection, 
            out float minBottom, 
            out float maxBottom,
            bool isForSectionLine,
            string secLnKey,
            out float maxBottomSectionLine) 
        {
            int index = 0;
            cReportSection rptSec = null;

            rptSec = m_report.getHeaders().item(sKeySection);

            index = rptSec.getRealIndex();

            //-----------
            // MinBottom
            //-----------
            if (index == 0) {
                minBottom = C_MIN_HEIGHT_SECTION;
            } 
            else {
                // bottom of previous header + C_Min_Height_Section
                cReportAspect w_aspect = m_report.getHeaders().item(index - 1).getAspect();
                minBottom = w_aspect.getTop() + w_aspect.getHeight() + C_MIN_HEIGHT_SECTION;
            }

            if (!isForSectionLine) {
                minBottom = pGetMinBottomWithSecLn(rptSec.getSectionLines(), minBottom);
            }

            maxBottomSectionLine = m_picReport.Height - getHeightOfSectionsBellowMe(rptSec, secLnKey);
            maxBottom = m_picReport.Height - getHeightOfSectionsBellowMe(rptSec, "");

            return rptSec;
        }

        private cReportSection pMoveGroupHeader(
            String sKeySection,
            out float minBottom,
            out float maxBottom)
        {
            float dummy;
            return pMoveGroupHeader(sKeySection, out minBottom, out maxBottom, false, "", out dummy);
        }

        private cReportSection pMoveGroupHeader(
            String sKeySection, 
            out float minBottom, 
            out float maxBottom,
            bool isForSectionLine,
            string secLnKey,
            out float maxBottomSectionLine) 
        {
            int index = 0;
            cReportSection rptSec = null;

            rptSec = m_report.getGroupsHeaders().item(sKeySection);

            index = rptSec.getRealIndex();

            //-----------
            // MinBottom
            //-----------
            if (index == 0) {
                // bottom of previous header + C_Min_Height_Section
                cReportSections w_headers = m_report.getHeaders();
                cReportAspect w_aspect = w_headers.item(w_headers.count()-1).getAspect();
                minBottom = w_aspect.getHeight() + w_aspect.getTop() + C_MIN_HEIGHT_SECTION;
            } 
            else {
                // bottom of previous group header + C_Min_Height_Section
                cReportAspect w_aspect = m_report.getGroupsHeaders().item(index - 1).getAspect();
                minBottom = w_aspect.getHeight() + w_aspect.getTop() + C_MIN_HEIGHT_SECTION;
            }

            if (!isForSectionLine) {
                minBottom = pGetMinBottomWithSecLn(rptSec.getSectionLines(), minBottom);
            }

            maxBottomSectionLine = m_picReport.Height - getHeightOfSectionsBellowMe(rptSec, secLnKey);
            maxBottom = m_picReport.Height - getHeightOfSectionsBellowMe(rptSec, "");

            return rptSec;
        }

        private float getHeightOfSectionsBellowMe(cReportSection section, String secLnKey) 
        {
            float height = 0;

            if ( ! String.IsNullOrEmpty(secLnKey) )
            {
                bool add = false;
                for (int _i = 0; _i < section.getSectionLines().count(); _i++)
                {
                    cReportSectionLine secLn = section.getSectionLines().item(_i);
                    if (add)
                    {
                        height += secLn.getAspect().getHeight();
                    }
                    else if (secLn.getKey() == secLnKey)
                    {
                        add = true;
                    }
                }
            }

            csRptTypeSection rptType = section.getTypeSection();

            switch (rptType) { 
                
                case csRptTypeSection.HEADER:
                case csRptTypeSection.MAIN_HEADER:
                    height += getHeightFromSections(m_report.getHeaders(), section);
                    height += getHeightFromSections(m_report.getGroupsHeaders(), null);
                    height += getHeightFromSections(m_report.getDetails(), null);
                    height += getHeightFromSections(m_report.getGroupsFooters(), null);
                    height += getHeightFromSections(m_report.getFooters(), null);
                    break;
                
                case csRptTypeSection.GROUP_HEADER:
                    height += getHeightFromSections(m_report.getGroupsHeaders(), section);
                    height += getHeightFromSections(m_report.getDetails(), null);
                    height += getHeightFromSections(m_report.getGroupsFooters(), null);
                    height += getHeightFromSections(m_report.getFooters(), null);
                    break;
                
                case csRptTypeSection.DETAIL:
                case csRptTypeSection.MAIN_DETAIL:
                    height += getHeightFromSections(m_report.getDetails(), section);
                    height += getHeightFromSections(m_report.getGroupsFooters(), null);
                    height += getHeightFromSections(m_report.getFooters(), null);
                    break;
                
                case csRptTypeSection.GROUP_FOOTER:
                    height += getHeightFromSections(m_report.getGroupsFooters(), section);
                    height += getHeightFromSections(m_report.getFooters(), null);
                    break;
                
                case csRptTypeSection.FOOTER:
                case csRptTypeSection.MAIN_FOOTER:
                    height += getHeightFromSections(m_report.getFooters(), section);
                    break;

                default:
                    throw new ReportEditorException(
                        csRptEditorErrors.CSRPT_EDITOR_SECTION_TYPE_INVALID,
                        C_MODULE,
                        cReportEditorError.errGetDescript(
                                        csRptEditorErrors.CSRPT_EDITOR_SECTION_TYPE_INVALID)); 
            }           

            return height;   
        }

        private float getHeightFromSections(cIReportGroupSections sections, cReportSection section)
        {
            bool add = section == null;
            float height = 0;
            for (int _i = 0; _i < sections.count(); _i++)
            {
                cReportSection sec = sections.item(_i);
                if (add) 
                {
                    height += sec.getAspect().getHeight();
                }
                else if (section == sec)
                {
                    add = true;
                }
            }
            return height;
        }

        private float getAllHeadersAndGroupsAndDetailsHeight()
        {
            cReportSection sec = null;

            float height = 0;

            for (int _i = 0; _i < m_report.getHeaders().count(); _i++)
            {
                sec = m_report.getHeaders().item(_i);
                height = height + sec.getAspect().getHeight();
            }

            for (int _i = 0; _i < m_report.getGroupsHeaders().count(); _i++)
            {
                sec = m_report.getGroupsHeaders().item(_i);
                height = height + sec.getAspect().getHeight();
            }

            for (int _i = 0; _i < m_report.getDetails().count(); _i++)
            {
                sec = m_report.getDetails().item(_i);
                height = height + sec.getAspect().getHeight();
            }            

            return height;
        }

        private cReportSection pMoveDetails(
            String sKeySection,
            out float minBottom,
            out float maxBottom)
        {
            float dummy;
            return pMoveDetails(sKeySection, out minBottom, out maxBottom, false, "", out dummy);
        }

        private cReportSection pMoveDetails(
            String sKeySection, 
            out float minBottom, 
            out float maxBottom, 
            bool isForSectionLine,
            string secLnKey,
            out float maxBottomSectionLine) 
        { 
            int index = 0;
            cReportSection rptSec = null;

            rptSec = m_report.getDetails().item(sKeySection);

            index = rptSec.getRealIndex();

            //-----------
            // MinBottom
            //-----------

            if (index == 0) {
                // if there are groups
                //
                if (m_report.getGroupsHeaders().count() > 0) {
                    // top of the last group header + C_Min_Height_Section
                    cIReportGroupSections w_groupsHeaders = m_report.getGroupsHeaders();
                    cReportAspect w_aspect = w_groupsHeaders.item(w_groupsHeaders.count()-1).getAspect();
                    minBottom = w_aspect.getHeight() + w_aspect.getTop() + C_MIN_HEIGHT_SECTION;
                } 
                else {
                    // top of the last header + C_Min_Height_Section
                    cReportSections w_headers = m_report.getHeaders();
                    cReportAspect w_aspect = w_headers.item(w_headers.count()-1).getAspect();
                    minBottom = w_aspect.getHeight() + w_aspect.getTop() + C_MIN_HEIGHT_SECTION;
                }
            } 
            else {
                // top of the previous detail + C_Min_Height_Section
                //
                cReportAspect w_aspect = m_report.getDetails().item(index - 1).getAspect();
                minBottom = w_aspect.getHeight() + w_aspect.getTop() + C_MIN_HEIGHT_SECTION;
            }

            if (!isForSectionLine) {
                minBottom = pGetMinBottomWithSecLn(rptSec.getSectionLines(), minBottom);
            }

            maxBottomSectionLine = m_picReport.Height - getHeightOfSectionsBellowMe(rptSec, secLnKey);
            maxBottom = m_picReport.Height - getHeightOfSectionsBellowMe(rptSec, "");

            return rptSec;
        }

        private cReportSection pMoveGroupFooter(
            String sKeySection,
            out float minBottom,
            out float maxBottom)
        {
            float dummy;
            return pMoveGroupFooter(sKeySection, out minBottom, out maxBottom, false, "", out dummy);
        }

        private cReportSection pMoveGroupFooter(
            String sKeySection, 
            out float minBottom, 
            out float maxBottom,
            bool isForSectionLine,
            string secLnKey,
            out float maxBottomSectionLine) 
        {
            int index = 0;
            cReportSection rptSec = null;

            rptSec = m_report.getGroupsFooters().item(sKeySection);

            index = rptSec.getRealIndex();

            //-----------
            // MinBottom
            //-----------
            if (index == 0) {
                // bottom of the last detail + C_Min_Height_Section
                //
                cReportSections w_details = m_report.getDetails();
                cReportAspect w_aspect = w_details.item(w_details.count()-1).getAspect();
                minBottom = w_aspect.getHeight() + w_aspect.getTop() + C_MIN_HEIGHT_SECTION;
            } 
            else {
                // bottom of the previous group footer + C_Min_Height_Section
                //
                cReportAspect w_aspect = m_report.getGroupsFooters().item(index - 1).getAspect();
                minBottom = w_aspect.getHeight() + w_aspect.getTop() + C_MIN_HEIGHT_SECTION;
            }

            if (!isForSectionLine) {
                minBottom = pGetMinBottomWithSecLn(rptSec.getSectionLines(), minBottom);
            }

            maxBottomSectionLine = m_picReport.Height - getHeightOfSectionsBellowMe(rptSec, secLnKey);
            maxBottom = m_picReport.Height - getHeightOfSectionsBellowMe(rptSec, "");

            return rptSec;
        }

        private cReportSection pMoveFooter(
            String sKeySection,
            out float minBottom,
            out float maxBottom)
        {
            float dummy;
            return pMoveFooter(sKeySection, out minBottom, out maxBottom, false, "", out dummy);
        }

        private cReportSection pMoveFooter(
            String sKeySection,
            out float minBottom,
            out float maxBottom,
            bool isForSectionLine,
            string secLnKey,
            out float maxBottomSectionLine) 
        {

            int index = 0;
            cReportSection rptSec = null;

            rptSec = m_report.getFooters().item(sKeySection);

            index = rptSec.getRealIndex();

            //-----------
            // MinBottom
            //-----------
            if (index == 0) {
                
                // if there are groups
                //
                if (m_report.getGroupsFooters().count() > 0) {
                    
                    // the bottom of the last group footer
                    //
                    cIReportGroupSections w_groupsFooters = m_report.getGroupsFooters();
                    cReportAspect w_aspect = w_groupsFooters.item(w_groupsFooters.count()-1).getAspect();
                    minBottom = w_aspect.getHeight() + w_aspect.getTop() + C_MIN_HEIGHT_SECTION;
                } 
                else {
                    // bottom of the last detail
                    //
                    cReportSections w_details = m_report.getDetails();
                    cReportAspect w_aspect = w_details.item(w_details.count()-1).getAspect();
                    minBottom = w_aspect.getHeight() + w_aspect.getTop() + C_MIN_HEIGHT_SECTION;
                }
            } 
            else {
                // bottom of the previous footer
                //
                cReportAspect w_aspect = m_report.getFooters().item(index - 1).getAspect();
                minBottom = w_aspect.getHeight() + w_aspect.getTop() - m_offSet + C_MIN_HEIGHT_SECTION;
            }

            if (!isForSectionLine) {
                minBottom = pGetMinBottomWithSecLn(rptSec.getSectionLines(), minBottom);
            }

            maxBottomSectionLine = m_picReport.Height - getHeightOfSectionsBellowMe(rptSec, secLnKey);
            maxBottom = m_picReport.Height - getHeightOfSectionsBellowMe(rptSec, "");

            return rptSec;
        }

        private float pGetMinBottomWithSecLn(cReportSectionLines secLns, float minBottom) { 
            for (int i = 0; i < secLns.count() - 1; i++) {
                minBottom = minBottom + secLns.item(i).getAspect().getHeight();
            }

            return minBottom;
        }

        private void pGetOffSet(float realPageHeight, float rtnPageHeight) {
            cReportSection sec = null;

            rtnPageHeight = 0;

            for (int _i = 0; _i < m_report.getHeaders().count(); _i++) {
                sec = m_report.getHeaders().item(_i);
                rtnPageHeight = rtnPageHeight + sec.getAspect().getHeight();
            }

            for (int _i = 0; _i < m_report.getGroupsHeaders().count(); _i++) {
                sec = m_report.getGroupsHeaders().item(_i);
                rtnPageHeight = rtnPageHeight + sec.getAspect().getHeight();
            }

            for (int _i = 0; _i < m_report.getDetails().count(); _i++) {
                sec = m_report.getDetails().item(_i);
                rtnPageHeight = rtnPageHeight + sec.getAspect().getHeight();
            }

            for (int _i = 0; _i < m_report.getGroupsFooters().count(); _i++) {
                sec = m_report.getGroupsFooters().item(_i);
                rtnPageHeight = rtnPageHeight + sec.getAspect().getHeight();
            }

            for (int _i = 0; _i < m_report.getFooters().count(); _i++) {
                sec = m_report.getFooters().item(_i);
                rtnPageHeight = rtnPageHeight + sec.getAspect().getHeight();
            }

            m_offSet = realPageHeight - rtnPageHeight;

            if (m_offSet < 0) { m_offSet = 0; }
        }

        private void pRefreshOffSetInPaintObjs() {
            cReportSection sec = null;
            cReportSectionLine secLines = null;
            cReportControl ctl = null;

            cReportPaintObjects w_paintSections = m_paint.getPaintSections();
                for (int _i = 0; _i < m_report.getFooters().count(); _i++) {
                    sec = m_report.getFooters().item(_i);
                    w_paintSections.item(sec.getKeyPaint()).getAspect().setOffset(m_offSet);
                    for (int _j = 0; _j < sec.getSectionLines().count(); _j++) {
                        secLines = sec.getSectionLines().item(_j);
                        if (secLines.getKeyPaint() != "") {
                            w_paintSections.item(secLines.getKeyPaint()).getAspect().setOffset(m_offSet);
                        }
                        for (int _k = 0; _k < secLines.getControls().count(); _k++) {
                            ctl = secLines.getControls().item(_k);
                            CSReportPaint.cReportPaintObject po;
                            po = m_paint.getPaintObjects().item(ctl.getKeyPaint());
                            po.getAspect().setOffset(m_offSet);
                        }
                    }
                }
        }

        // if the click was over a control which is not part of the
        // selected controls collection we clear the selected collection
        // and add the control which was clicked to the selected collection
        //
        private bool pSetSelectForRightBttn() {
            for (int i = 0; i < m_vSelectedKeys.Length; i++) {
                if (m_vSelectedKeys[i] == m_keyObj) { return false; }
            }

            G.redim(ref m_vSelectedKeys, 1);
            m_vSelectedKeys[0] = m_keyObj;

            return true;
        }

        private void pValidateSectionAspect() {
            cReportSection sec = null;
            float top = 0;
            int i = 0;

            for (int _i = 0; _i < m_report.getHeaders().count(); _i++) {
                sec = m_report.getHeaders().item(_i);
                top = pValidateSectionAspecAux(top, sec);
            }

            for (int _i = 0; _i < m_report.getGroupsHeaders().count(); _i++) {
                sec = m_report.getGroupsHeaders().item(_i);
                top = pValidateSectionAspecAux(top, sec);
            }

            for (int _i = 0; _i < m_report.getDetails().count(); _i++) {
                sec = m_report.getDetails().item(_i);
                top = pValidateSectionAspecAux(top, sec);
            }

            for (int _i = 0; _i < m_report.getGroupsFooters().count(); _i++) {
                sec = m_report.getGroupsFooters().item(_i);
                top = pValidateSectionAspecAux(top, sec);
            }

            cReportPaperInfo w_paperInfo = m_report.getPaperInfo();
            top = CSReportPaint.cGlobals.getRectFromPaperSize(m_report.getPaperInfo(), 
                                                    w_paperInfo.getPaperSize(), 
                                                    w_paperInfo.getOrientation()).Height;

            for (i = m_report.getFooters().count()-1; i > -1; i--) {
                sec = m_report.getFooters().item(i);
                top = top - sec.getAspect().getHeight();
                pValidateSectionAspecAux(top, sec);
            }
        }

        private float pValidateSectionAspecAux(float top, cReportSection sec) {
            cReportSectionLine secLn = null;
            float topLn = 0;
            float secLnHeight = 0;
            float width = 0;

            cReportPaperInfo w_paperInfo = m_report.getPaperInfo();
            width = CSReportPaint.cGlobals.getRectFromPaperSize(
                                                    m_report.getPaperInfo(), 
                                                    w_paperInfo.getPaperSize(), 
                                                    w_paperInfo.getOrientation()).Width;
            topLn = top;

            cReportAspect w_aspect;

            for (int i = 0; i < sec.getSectionLines().count() - 1; i++) {
                secLn = sec.getSectionLines().item(i);
                w_aspect = secLn.getAspect();
                w_aspect.setTop(topLn);
                w_aspect.setWidth(width);
                if (w_aspect.getHeight() < C_MIN_HEIGHT_SECTION) {
                    w_aspect.setHeight(C_MIN_HEIGHT_SECTION);
                }
                topLn = topLn + w_aspect.getHeight();
                secLnHeight = secLnHeight + w_aspect.getHeight();
            }

            cReportSectionLines w_sectionLines = sec.getSectionLines();
            secLn = w_sectionLines.item(w_sectionLines.count()-1);

            w_aspect = secLn.getAspect();
            w_aspect.setTop(topLn);
            w_aspect.setHeight(sec.getAspect().getHeight() - secLnHeight);
            if (w_aspect.getHeight() < C_MIN_HEIGHT_SECTION) {
                w_aspect.setHeight(C_MIN_HEIGHT_SECTION);
            }
            secLnHeight = secLnHeight + w_aspect.getHeight();

            w_aspect = sec.getAspect();
            w_aspect.setHeight(secLnHeight);
            if (w_aspect.getHeight() < C_MIN_HEIGHT_SECTION) {
                w_aspect.setHeight(C_MIN_HEIGHT_SECTION);
            }
            w_aspect.setWidth(width);
            w_aspect.setTop(top);
            topLn = top;
            top = top + w_aspect.getHeight();

            pChangeTopSection(sec, 0, false, false);
            return top;
        }

        public void showControls() {
            try {
                fControls f = cMainEditor.getCtrlBox(this);
                f.clear();
                f.addCtrls(m_report);
                if (!f.Visible)
                {
                    f.Show(m_fmain);
                }
            } catch (Exception ex) {
                cError.mngError(ex, "showControls", C_MODULE, "");
            }
        }

        public void showControlsTree() {
            try {
                fTreeViewCtrls f = cMainEditor.getCtrlTreeBox(this);
                f.clear();
                f.addCtrls();
                if (!f.Visible)
                {
                    f.Show(m_fmain);
                }
            } catch (Exception ex) {
                cError.mngError(ex, "showControlsTree", C_MODULE, "");
            }
        }

        private void pSetInitDir() {
            if (cMainEditor.gbFirstOpen) {
                cMainEditor.gbFirstOpen = false;
                // TODO: implement me
                // m_fmain.cmDialog.InitDir = cGlobals.gWorkFolder;
            }
        }

        private void form_Load() {
            G.redim(ref m_vSelectedKeys, 0);
            G.redim(ref m_vCopyKeys, 0);
            m_copyControls = false;
            m_copyControlsFromOtherReport = false;
            m_typeGrid = csETypeGrid.CSEGRIDPOINTS;
            m_keyboardMoveStep = 50;
        }

        /* TODO: implement me
        private void form_QueryUnload(int cancel, int unloadMode) {
            cancel = !saveChanges();
            if (cancel) { cGlobals.setDocActive(this); }
        }
         */

        /* TODO: implement me
        private void form_Unload(int cancel) {
            if (m_fmain.getReportCopySource() == this) {
                m_fmain.setReportCopySource(null);
            }
            if (fSearch.fSearch.getFReport() == this) {
                fSearch.fSearch.setFReport(null);
            }
            m_report = null;
            m_paint = null;
            m_fToolBox = null;
            m_fControls = null;
            m_fTreeCtrls = null;
            m_fConnectsAux = null;
            m_fProperties = null;
            m_fFormula = null;
            m_fGroup = null;
            m_fProgress.Hide();
            m_fProgress = null;
            cGlobals.setDocInacActive(this);
            G.redim(ref m_vSelectedKeys, 0);
            G.redim(ref m_vCopyKeys, 0);
        }
         */

        public void init() {
            m_showingProperties = false;

            cReportLaunchInfo oLaunchInfo = null;
            m_report = new cReport();

            // TODO: event handler for
            //
            /*
                        m_report_Done();
                        m_report_Progress(task, page, currRecord, recordCount, cancel,);
                        m_report_FindFileAccess(answer, commDialog, file,);
            */

            m_report.Progress += reportProgress;
            m_report.ReportDone += reportDone;

            oLaunchInfo = new cReportLaunchInfo();

            m_report.getPaperInfo().setPaperSize(m_fmain.getPaperSize());
            m_report.getPaperInfo().setOrientation(m_fmain.getOrientation());

            oLaunchInfo.setPrinter(cPrintAPI.getcPrinterFromDefaultPrinter());
            oLaunchInfo.setObjPaint(new CSReportPaint.cReportPrint());
            if (!m_report.init(oLaunchInfo)) { return; }

            CSKernelFile.cFile file = new CSKernelFile.cFile();
            m_report.setPathDefault(Application.StartupPath);

            m_picReport.Top = C_TOPBODY;
            m_picRule.Left = 0;
            m_picReport.Left = pGetLeftBody();

            m_keyMoving = "";
            m_keySizing = "";
            m_keyObj = "";
            m_keyFocus = "";
            m_nextNameCtrl = 0;

            m_paint = new CSReportPaint.cReportPaint();

            Rectangle tR = null;
            cReportPaperInfo w_paperInfo = m_report.getPaperInfo();
            tR = new Rectangle(CSReportPaint.cGlobals.getRectFromPaperSize(
                                                m_report.getPaperInfo(), 
                                                w_paperInfo.getPaperSize(), 
                                                w_paperInfo.getOrientation()));
            
            cGlobals.createStandarSections(m_report, tR);

            reLoadReport();

            m_dataHasChanged = false;
        }

        private void pUpdateFormulas(String currentName, String newName) {
            cReportControl rptCtrl = null;

            for (int i = 0; i < m_report.getControls().count(); i++) {

                rptCtrl = m_report.getControls().item(i);

                cReportFormula w_formulaHide = rptCtrl.getFormulaHide();
                if (w_formulaHide.getText() != "") {
                    if (w_formulaHide.getText().IndexOf(currentName, 1) != 0) {
                        w_formulaHide.setText(pReplaceInFormula(w_formulaHide.getText(), currentName, newName));
                    }
                }

                cReportFormula w_formulaValue = rptCtrl.getFormulaValue();
                if (w_formulaValue.getText() != "") {
                    if (w_formulaValue.getText().IndexOf(currentName, 1) != 0) {
                        w_formulaValue.setText(pReplaceInFormula(w_formulaValue.getText(), currentName, newName));
                    }
                }
            }
        }

        private String pReplaceInFormula(String formulaText, String currentName, String newName) {
            String _rtn = "";

            // if it isn't an internal function we give the user
            // a chance to cancel the changes
            //
            if (cUtil.subString(formulaText, 0, 1).Trim() != "_")
            {
                fFormulaReplace fReplace = null;
                fReplace = new fFormulaReplace();
                fReplace.txCurrFormula.Text = formulaText;
                fReplace.txNewFormula.Text = formulaText.Replace(currentName, newName);
                fReplace.ShowDialog();
                if (fReplace.getOk()) {
                    _rtn = fReplace.txNewFormula.Text;
                } 
                else {
                    _rtn = formulaText;
                }
                fReplace.Hide();
            } 
            else {

                _rtn = formulaText.Replace(currentName, newName);
            }
            return _rtn;
        }

        private void form_Deactivate() {
            cMainEditor.setDocInacActive(this);
            cMainEditor.clearToolbox(this);
        }

        public void editConnectionString()
        {
            string stringConnection = m_report.getConnect().getStrConnect();
            if (cUtil.getInput(ref stringConnection, "You can modify the string connection of this report", "String connection"))
            {
                m_report.getConnect().setStrConnect(stringConnection);
            }
        }

        public void editDataSource()
        {
            string dataSource = m_report.getConnect().getDataSource();
            if (cUtil.getInput(ref dataSource, "You can modify the data source of this report", "Data Source"))
            {
                m_report.getConnect().setDataSource(dataSource);
            }
        }
    }

    enum csAskEditResult {
        CSASKRSLTYES = 1,
        CSASKRSLTNO = 2,
        CSASKRSLTCANCEL = 3
    }
}
