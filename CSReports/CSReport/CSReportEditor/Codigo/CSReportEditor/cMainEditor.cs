using System;
using System.Globalization;
using System.Collections.Generic;
using System.Windows.Forms;
using CSReportGlobals;
using CSKernelClient;
using CSReportDll;

namespace CSReportEditor
{
	public class cMainEditor {

	    private const string C_MODULE = "mPublic";
		
	    private const int NOERROR = 0;

		public DateTime CSNOFECHA = DateTime.ParseExact("01/01/1900", "dd/mm/yyyy", CultureInfo.InvariantCulture);

	    public const int C_HEIGHT_BAR_SECTION = 120;
	    public const int C_HEIGHT_NEW_SECTION = 350;

	    private const string C_KEYRECENTLIST = "Recent";

	    private const string C_CONFIG = "Interfaz";
	    private const string C_LEFTBARCOLOR = "LeftBarColor";
	    private const string C_HIDELEFTBAR = "HideLeftBar";
	    private const string C_BACKCOLOR = "BackColor";
	    private const string C_WORKFOLDER = "WorkFolder";

	    public static int gNextReport = 0;
	    private static cEditor m_editor;

        private static fToolbox m_fToolbox = null;
        private static fControls m_fControls = null;
        private static fTreeViewCtrls m_fTreeViewCtrls = null;
        private static fSearch m_fSearch = null;

		public static int gBackColor = 0;
	    public static int gLeftBarColor = 0;
	    public static bool gHideLeftBar;
	    public static String gWorkFolder = "";
	    public static bool gbFirstOpen;

        private static fMain fmain;

        public static fMain initEditor() {
            
            cRegionalCfg.init();

            if (fmain == null) {
                fmain = new fMain();
                fmain.init();
            }
            return fmain;
        }

        public static fMain getEditor() {
            return fmain;
        }

	    public static cEditor getDocActive() {
	        return m_editor;
	    }

	    public static void setDocActive(cEditor editor) {
	        m_editor = editor;
	        setMenu();
            if (editor != null)
            {
                TabPage editorTab = editor.getEditorTab();
                (editorTab.Parent as TabControl).SelectedTab = editorTab;
                
                if (m_fToolbox != null && !m_fToolbox.IsDisposed && m_fToolbox.Visible)
                {
                    if (getToolbox(editor) != null) { editor.showToolbox(); }
                }
                if (m_fControls != null && !m_fControls.IsDisposed && m_fControls.Visible)
                {
                    if (getCtrlBox(editor) != null) { editor.showControls(); }
                }
                if (m_fTreeViewCtrls != null && !m_fTreeViewCtrls.IsDisposed && m_fTreeViewCtrls.Visible)
                {
                    if (getCtrlTreeBox(editor) != null) { editor.showControlsTree(); }
                }
                fmain.showControls(editor);
                fmain.showControlsTree(editor);
                fmain.showFields(editor);
            }
	    }

	    public static void setDocInacActive(cEditor editor) {
	        if (m_editor != editor) { return; }
	        m_editor = null;
	        setMenu();
	        setEditAlignTextState(false);
	    }

	    public static void setStatus() {
	        try {
	            if (m_editor == null) {
	                setStatus("");
	            } 
	            else {
	                setStatus(pGetStatus());
	            }

	        } catch (Exception ex) {
	            cError.mngError(ex, "setStatus", C_MODULE, "");
	        }
	    }

        public static void setStatus(String status) { 
        
        }

        public static void setBarText(String text) { 
        
        }

        public static void setDisconnectedReport(bool isDisconnectedReport) { 
        
        }

        public static void setEditAlignTextState(bool status) {
            fmain.setEditAlignTextState(status);
        }

        public static void setEditAlignCtlState(bool status) {
            fmain.setEditAlignCtlState(status);
        }

        public static void setMenuAux(bool enabled) {
            fmain.setMenuAux(enabled);
        }

        public static void addToRecentList(String fileName){
            fmain.addToRecentList(fileName);
        }

	    public static void setEditFontBoldValue(int bBold) {
			// TODO: implement
	    }

	    private static void setMenu() {
	        try {

	            if (m_editor == null) {
	                fmain.setMenuAux(false);
	                fmain.setBarText("");
	                fmain.setStatus("");
	            } 
	            else {
	                fmain.setMenuAux(true);
	                fmain.setDisconnectedReport(m_editor.getReport().getReportDisconnected());
	                fmain.setBarText(m_editor.getReport().getName());
	                fmain.setStatus(pGetStatus());
	            }
	        } catch (Exception ex) {
	            cError.mngError(ex, "SetMenu", C_MODULE, "");
	        }
	    }

        private static string pGetStatus() {
            return "";
        }

        public static fSearch getSearch()
        {
            return m_fSearch;
        }

        public static fSearch getSearch(cEditor editor)
        {
            if (m_fSearch == null || m_fSearch.IsDisposed)
            {
                m_fSearch = new fSearch();
            }
            m_fSearch.setHandler(editor);
            return m_fSearch;
        }

        public static fToolbox getToolbox()
        {
            return m_fToolbox;
        }

        public static fToolbox getToolbox(cEditor editor)
        {
            if (m_fToolbox == null || m_fToolbox.IsDisposed)
            {
                m_fToolbox = new fToolbox();
            }
            m_fToolbox.setHandler(editor);
            return m_fToolbox;
        }

        public static fControls getCtrlBox()
        {
            return m_fControls;
        }

        public static fControls getCtrlBox(cEditor editor)
        {
            if (m_fControls == null || m_fControls.IsDisposed)
            {
                m_fControls = new fControls();
            }
            m_fControls.setHandler(editor);
            return m_fControls;
        }

        public static fTreeViewCtrls getCtrlTreeBox()
        {
            return m_fTreeViewCtrls;
        }

        public static fTreeViewCtrls getCtrlTreeBox(cEditor editor)
        {
            if (m_fTreeViewCtrls == null || m_fTreeViewCtrls.IsDisposed)
            {
                m_fTreeViewCtrls = new fTreeViewCtrls();
            }
            m_fTreeViewCtrls.setHandler(editor);
            return m_fTreeViewCtrls;
        }

        public static void clearToolbox(cEditor editor)
        {
            if (m_editor == editor)
            {
                if (m_fToolbox != null && !m_fToolbox.IsDisposed && m_fToolbox.Visible)
                {
                    m_fToolbox.clear();
                }
            }
        }

        public static void showProperties(string key)
        {
            fmain.showProperties(m_editor, key);            
        }
    }

	public enum SpecialFolderIDs {
	    SFIDDESKTOP = 0x0,
	    SFIDPROGRAMS = 0x2,
	    SFIDPERSONAL = 0x5,
	    SFIDFAVORITES = 0x6,
	    SFIDSTARTUP = 0x7,
	    SFIDRECENT = 0x8,
	    SFIDSENDTO = 0x9,
	    SFIDSTARTMENU = 0xB,
	    SFIDDESKTOPDIRECTORY = 0x10,
	    SFIDNETHOOD = 0x13,
	    SFIDFONTS = 0x14,
	    SFIDTEMPLATES = 0x15,
	    SFIDCOMMON_STARTMENU = 0x16,
	    SFIDCOMMON_PROGRAMS = 0x17,
	    SFIDCOMMON_STARTUP = 0x18,
	    SFIDCOMMON_DESKTOPDIRECTORY = 0x19,
	    SFIDAPPDATA = 0x1A,
	    SFIDPRINTHOOD = 0x1B,
	    SFIDPROGRAMS_FILES = 0x26,
	    SFIDPROGRAMFILES = 0x10000,
	    SFIDCOMMONFILES = 0x10001
	}


	public enum csEAlignConst {
	    CSEALIGNTEXTLEFT = 1,
	    CSEALIGNTEXTRIGHT,
	    CSEALIGNTEXTCENTER,
	    CSEALIGNCTLLEFT,
	    CSEALIGNCTLHORIZONTAL,
	    CSEALIGNCTLRIGHT,
	    CSEALIGNCTLVERTICAL,
	    CSEALIGNCTLTOP,
	    CSEALIGNCTLBOTTOM,
	    CSEALIGNCTLWIDTH,
	    CSEALIGNCTLHEIGHT
	}

}

