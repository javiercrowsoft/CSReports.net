using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Xml;
using System.Windows.Forms;
using System.Drawing;
using CSKernelClient;
using CSReportGlobals;
using CSKernelFile;
using System.Data.Common;

namespace CSReportDll
{

    public delegate void ReportDoneHandler(object sender, EventArgs e);
    public delegate void ProgressHandler(object sender, ProgressEventArgs e);
    public delegate void FindAccessFileHandler(object sender, FindAccessFileEventArgs e);

    public class cReport : IDisposable
    {        
        public event ReportDoneHandler ReportDone;
        public event ProgressHandler Progress;
        public event FindAccessFileHandler FindAccessFile;

        private class T_Group
        {
            public int first;
            public int last;
        }


        private class T_Groups
        {
            public object value;
            public int indexField;
            public bool changed;
            public bool reprintHeader;
            public bool footerMustBeClosed;
            public csRptGrpComparisonType comparisonType;
            public csRptGrpOrderType oderType;
            public Boolean grandTotalGroup;
            public T_Group[] groups;
            public int lastHPreRowEvalued;
            public int lastHPostRowEvalued;
            public int lastFPreRowEvalued;
            public int lastFPostRowEvalued;
            // to know which is the line number when we are in a group
            //
            // it is incremented only when the detail section is printed
            // it doesn't care if the details contains more than one line
            //
            public int lineNumber;
        }

        // remember mark any change could bring errors 
        // with the label WARNING and the date
        //
        // 2008-02-18 WARNING

        // additional recordset management:
        //
        // there are two types of additional recordsets:
        //
        //     - every recordset which follow the main recordset in the main connection
        //     - every recordset returned by an additional connection
        //
        // which is their porpouse:
        //
        // they bring a source of data related or not with the main recordset
        //
        // a common use is for getting the logo of company reports 
        //
        // other use is to attach to a number of rows in the main recordset 
        // some heavy data without repeat it in each row. It's very usefull 
        // with images.
        //
        // the formula getDataFromRSAd is used to access data in additional recordsets
        //
        // getDataFromRSAd parameters:
        //                    - datasource name
        //                    - index of the recordset in the datasource
        //                    - column name in the recordset
        //                    - an string with the relaction between the main and the additional recordset
        //
        // Example:    GetDataFromRsAd (lsListaPrecioExpo,2,smallimage,pr_id=pr_id)
        //
        // Nota: in this example we have a relation between two columns but the filter could contain
        //       multiples columns.
        //       the first column is for the main recordset, the second columns is for the 
        //       additional recordset

        //--------------------------------------------------------------------------------

        private const String C_MODULE = "cReport";
        private const int C_HEADERS = 1;
        private const int C_FOOTERS = 2;
        private const String C_NODERPTHEADERS = "RptHeaders";
        private const String C_NODERPTDETAILS = "RptDetails";
        private const String C_NODEGROUPS = "RptGroups";
        private const String C_NODERPTFOOTERS = "RptFooters";
        private const String C_RPTCONNECT = "RptConnect";
        private const String C_RPTCONNECTSAUX = "RptConnectsAux";
        private const String C_LAUNCHINFO = "RptLaunchInfo";
        private const String C_NODERPTFORMULAS = "RptFormulas";
        private const String C_NODERPTPAGESSETTING = "RptPagesSetting";
        private const String C_NODERPTPAGES = "RptPages";
        private const String C_NODEPAPERINFO = "PaperInfo";
        private const String C_FILEEX = "CrowSoft Report|*.csr| Archivos Xml|*.xml";
        private const String C_FILEDATAEX = "CrowSoft Report data|*.csd| Archivos Xml|*.xml";

        // every formula in a header
        //
        private const int C_IDX_GROUP_HEADER = -1000;
        // every formula in detail
        //
        private const int C_IDX_GROUP_DETAIL = 0;
        // every formula in a footer
        //
        private const int C_IDX_GROUP_FOOTER = -1001;
        // every formumal in groups
        //
        private const int C_IDX_GROUP_REPORTHEADER = -2000;
        private const int C_IDX_GROUP_REPORTFOOTER = -2001;

        private const int C_IDX_H_LAST_ROW_EVALUED = 0;
        private const int C_IDX_D_LAST_ROW_EVALUED = 1;
        private const int C_IDX_F_LAST_ROW_EVALUED = 2;

        // flag to know if we need to check in the group (m_vGroups)
        // which row was the last evaluated
        // instead of checking in m_LastRow..Evalued
        //
        private const int C_IDX_G_LAST_ROW_EVALUED = -1;

        private cReportLaunchInfo m_launchInfo;

        private cReportGroups m_groups;
        private cReportSections m_details;
        private cReportSections m_headers;
        private cReportSections m_footers;
        private cReportSections m_groupsHeaders;
        private cReportSections m_groupsFooters;
        private cReportPaperInfo m_paperInfo;
        private int m_originalHeight = 0;
        private cReportControls2 m_controls;
        private cReportFormulas m_formulas;
        private cReportFormulaTypes m_formulaTypes;
        private String m_name = "";
        private String m_path = "";
        private String m_pathDefault = "";

        private String m_descripUser = "";

        private cReportConnect m_connect;
        private cReportConnectsAux m_connectsAux;

        private cReportPageSettings m_pageSetting;
        private cReportPages m_pages;

        private cReportCompiler m_compiler;
        private int m_currenPage = 0;
        private int m_totalPages = 0;

        private bool m_reportDisconnected;

        // to sort groups
        //
        // this array contains a table with the data of every recordset
        // in the main connection
        //
        // the function pGetData() reserves a position for every recordset
        // in the additional connections
        //
        private DataTable[] m_collRows = null;

        private Dictionary<string, Image> m_images = null;
        private DataTable m_rows = null;
        private int m_recordCount = 0;
        private int[] m_vRowsIndex = null;
        private int[] m_vRowsIndexAux = null;
        private int m_iRow = 0;
        private int m_iRow2 = 0;
        private int m_iRowFormula = 0;
        private int m_lineIndex = 0;

        private int[] m_lastRowPreEvalued = null;
        private int[] m_lastRowPostEvalued = null;

        // to print groups in a new page when a group changes
        //
        private int m_idxGroupToPrintNP = 0;

        // flag to know if there are group headers to re-print in a new page
        // if it is false we can print a footer as the first line in a new page
        //
        private bool m_bExistsGrpToRePrintInNP;
        private bool m_bHaveToRePrintGroup;

        // index of the current group header
        //
        private int m_idxGroupHeader = 0;

        // index of the current group footer
        //
        private int m_idxGroupFooter = 0;

        private bool m_bPrintFooter;
        private bool m_bLastFootersWasPrinted;
        private int m_groupIndexChange = 0;

        private bool m_bEvalPreGroups;
        private bool m_bCloseFooter;
        private bool m_bOpenHeader;


        // it is incremented only when a the detail section is printed
        // it doesn't care if the details contains more than one line
        //
        // index of the current line
        //
        private int m_lineNumber = 0;

        private T_Groups[] m_vGroups;
        private bool m_firstGroup;
        private int m_groupCount = 0;

        private bool m_isForWeb;

        private String m_exportEmailAddress = "";

        public cReport()
        {
            try
            {
                m_headers = new cReportSections();
                m_details = new cReportSections();
                m_footers = new cReportSections();
                m_groups = new cReportGroups();
                m_groupsHeaders = getGroups().getGroupsHeaders();
                m_groupsFooters = getGroups().getGroupsFooters();
                m_paperInfo = new cReportPaperInfo();
                m_controls = new cReportControls2();
                m_formulas = new cReportFormulas();
                m_formulaTypes = new cReportFormulaTypes();
                m_connect = new cReportConnect();
                m_pageSetting = new cReportPageSettings();
                m_pages = new cReportPages();

                m_compiler = new cReportCompiler();

                setConnectsAux(new cReportConnectsAux());

                m_details.setCopyColl(m_controls);
                m_headers.setCopyColl(m_controls);
                m_footers.setCopyColl(m_controls);
                m_groupsHeaders.setCopyColl(m_controls);
                m_groupsFooters.setCopyColl(m_controls);

                m_details.setTypeSection(csRptTypeSection.CSRPTTPSCDETAIL);
                m_headers.setTypeSection(csRptTypeSection.CSRPTTPSCHEADER);
                m_footers.setTypeSection(csRptTypeSection.CSRPTTPSCFOOTER);
                m_groupsHeaders.setTypeSection(csRptTypeSection.CSRPTTPGROUPHEADER);
                m_groupsFooters.setTypeSection(csRptTypeSection.CSRPTTPGROUPFOOTER);

                m_details.setMainTypeSection(csRptTypeSection.CSRPTTPMAINSECTIONDETAIL);
                m_headers.setMainTypeSection(csRptTypeSection.CSRPTTPMAINSECTIONHEADER);
                m_footers.setMainTypeSection(csRptTypeSection.CSRPTTPMAINSECTIONFOOTER);
            }
            catch (Exception ex)
            {
                cError.mngError(ex, "Class_Initialize", C_MODULE, "");
            }
        }

        public String getExportEmailAddress()
        {
            return m_exportEmailAddress;
        }

        public void setExportEmailAddress(String rhs)
        {
            m_exportEmailAddress = rhs;
        }

        public bool getIsForWeb()
        {
            return m_isForWeb;
        }

        public void setIsForWeb(bool rhs)
        {
            m_isForWeb = rhs;
        }

        public cReportConnectsAux getConnectsAux()
        {
            return m_connectsAux;
        }

        public void setConnectsAux(cReportConnectsAux rhs)
        {
            m_connectsAux = rhs;
        }

        public cReportGroups getGroups()
        {
            return m_groups;
        }

        public void setGroups(cReportGroups rhs)
        {
            m_groups = rhs;
        }

        public cReportSections getDetails()
        {
            return m_details;
        }

        public void setDetails(cReportSections rhs)
        {
            m_details = rhs;
        }

        public cReportSections getHeaders()
        {
            return m_headers;
        }

        public void setHeaders(cReportSections rhs)
        {
            m_headers = rhs;
        }

        public cReportSections getFooters()
        {
            return m_footers;
        }

        public void setFooters(cReportSections rhs)
        {
            m_footers = rhs;
        }

        public cIReportGroupSections getGroupsHeaders()
        {
            return m_groupsHeaders;
        }

        public cIReportGroupSections getGroupsFooters()
        {
            return m_groupsFooters;
        }

        public cReportPaperInfo getPaperInfo()
        {
            return m_paperInfo;
        }

        public void setPaperInfo(cReportPaperInfo rhs)
        {
            m_paperInfo = rhs;
        }

        public cReportControls2 getControls()
        {
            return m_controls;
        }

        public cReportFormulas getFormulas()
        {
            return m_formulas;
        }

        public cReportFormulaTypes getFormulaTypes()
        {
            return m_formulaTypes;
        }

        public String getName()
        {
            return m_name;
        }

        public void setName(String rhs)
        {
            m_name = rhs;
        }

        public String getPath()
        {
            return m_path;
        }

        public void setPathDefault(String rhs)
        {
            m_pathDefault = rhs;
        }

        public cReportConnect getConnect()
        {
            return m_connect;
        }

        public cReportPages getPages()
        {
            return m_pages;
        }

        public cReportPageSettings getPageSetting()
        {
            return m_pageSetting;
        }

        public void setPageSetting(cReportPageSettings rhs)
        {
            m_pageSetting = rhs;
        }

        public cReportLaunchInfo getLaunchInfo()
        {
            return m_launchInfo;
        }

        public cReportCompiler getCompiler()
        {
            return m_compiler;
        }

        public bool getReportDisconnected()
        {
            return m_reportDisconnected;
        }

        public void setReportDisconnected(bool rhs)
        {
            m_reportDisconnected = rhs;
        }

        public String getDescripUser()
        {
            return m_descripUser;
        }

        public void setDescripUser(String rhs)
        {
            m_descripUser = rhs;
        }

        internal int getCurrenPage()
        {
            return m_pages.count();
        }

        internal int getTotalPages()
        {
            return m_pages.count();
        }

        public bool moveGroup(int from, int to)
        {
            if (from < 1 || from > m_groups.count())
            {
                return false;
            }
            if (to < 1 || to > m_groups.count())
            {
                return false;
            }

            if (from != to)
            {

                cReportGroup group = null;
                cReportGroups collGroups = new cReportGroups();

                for (int _i = 0; _i < m_groups.count(); _i++)
                {
                    group = m_groups.item(_i);
                    collGroups.add(group, group.getKey());
                }

                m_groups.clear();

                int index = 0;

                for (int _i = 0; _i < collGroups.count(); _i++)
                {
                    group = collGroups.item(_i);
                    index = index + 1;
                    if (index != from)
                    {
                        if (index == to)
                        {
                            cReportGroup group2 = collGroups.item(from);
                            m_groups.add2(group2, group2.getKey());
                        }
                        m_groups.add2(group, group.getKey());
                    }
                }
            }
            return true;
        }

        // this function is called by the print component every time a page is printed
        // the function add a new cReportPage object to the pages collection
        // and then set every header in the new cReportPage
        //
        public csRptNewPageResult newPage()
        {
            cReportPage page = m_pages.add(null, "");
            page.setPageNumber(m_pages.count());

            // if the user has canceled we return an error
            //
            if (!OnProgress("", m_pages.count(), 0, 0))
            {
                return csRptNewPageResult.CSRPTNPERROR;
            }

            // if it is the first page we evaluate the headers of the report
            //
            if (m_pages.count() == 1)
            {
                evalFunctions(C_IDX_GROUP_REPORTHEADER, csRptWhenEval.CSRPTEVALPRE);
            }

            // only formulas located in header sections
            //
            evalFunctions(C_IDX_GROUP_HEADER, csRptWhenEval.CSRPTEVALPRE);

            // add field from every header to the page
            //
            addFieldToNewPage(m_headers, page, C_HEADERS);

            // only formulas located in header sections
            //
            evalFunctions(C_IDX_GROUP_HEADER, csRptWhenEval.CSRPTEVALPOST);

            // if it is the first page we evaluate the headers of the report
            //
            if (m_pages.count() == 1)
            {
                evalFunctions(C_IDX_GROUP_REPORTHEADER, csRptWhenEval.CSRPTEVALPOST);
            }

            // we need to set height of headears an footers
            //
            page.setHeaderBottom(getHeightHeader());
            page.setFooterTop(getTopFooter());

            if (m_rows == null)
            {
                return csRptNewPageResult.CSRPTNPEND;
            }
            else if (m_iRow > m_vRowsIndex.Length)
            {
                return csRptNewPageResult.CSRPTNPEND;
            }

            // if there are group headers which need to be reprinted
            // in the new page
            //
            if (m_bExistsGrpToRePrintInNP)
            {
                m_bHaveToRePrintGroup = true;

                // set on the flag to know we need to re-print group headers
                //
                pMarkGroupHeadersToReprint();
            }

            return csRptNewPageResult.CSRPTNPSUCCESS;
        }

        private void pMarkGroupHeadersToReprint()
        {
            // if this is the first page we do nothing
            //
            if (m_firstGroup)
            {
                return;
            }

            for (int i = 1; i <= m_groupCount; i++)
            {
                if (m_groups.item(i).getRePrintInNewPage())
                {
                    m_vGroups[i].reprintHeader = true;
                }
            }
        }

        private bool pExistsGroupHeadersToReprint()
        {
            for (int i = 1; i <= m_groupCount; i++)
            {
                if (m_vGroups[i].reprintHeader)
                {
                    m_idxGroupHeader = i;
                    m_bOpenHeader = true;
                    return true;
                }
            }

            // there are no more groups to re-print
            //
            m_bHaveToRePrintGroup = false;
            return false;
        }

        private void pCheckExistsGroupHToReprint()
        {
            for (int i = 1; i <= m_groupCount; i++)
            {
                if (m_vGroups[i].reprintHeader)
                {
                    return;
                }
            }

            // there are no more groups to re-print
            //
            m_bHaveToRePrintGroup = false;
        }

        // this function is called by the print component every time a page is printed
        // the function set the footers in the last page of the m_pages collection
        //
        public csRptEndPageResult endPage()
        {
            // last page
            //
            cReportPage page = m_pages.item(m_pages.count());

            // only formulas located in footer sections
            //
            evalFunctions(C_IDX_GROUP_FOOTER, csRptWhenEval.CSRPTEVALPRE);

            // add field from every header to the page
            //
            addFieldToNewPage(m_footers, page, C_FOOTERS);

            // only formulas located in footer sections
            //
            evalFunctions(C_IDX_GROUP_FOOTER, csRptWhenEval.CSRPTEVALPOST);

            return csRptEndPageResult.CSRPTEPSUCCESS;
        }

        public void markGroupHeaderPrinted()
        {
            // if it took place in a re-print
            //
            if (m_vGroups[m_idxGroupHeader].reprintHeader)
            {

                m_vGroups[m_idxGroupHeader].reprintHeader = false;

                // every time we print a group
                // because was mark as needed to re-print
                // we check if we need to set off the flag
                //
                pCheckExistsGroupHToReprint();

                if (pNotPendingFooters())
                {
                    pMarkGroupHeaderPrintedAux();
                }

                // if the group has changed we need to
                // initialize it and then mark it as printed
                //
            }
            else if (m_vGroups[m_idxGroupHeader].changed)
            {
                pMarkGroupHeaderPrintedAux();
            }
        }

        private void pMarkGroupHeaderPrintedAux()
        {
            cReportSection headerSec = null;
            cReportSectionLine secLn = null;
            cReportControl ctrl = null;

            // if we have printed the group we need to set off
            // the flag which tell us the group has changed
            //
            m_vGroups[m_idxGroupHeader].changed = false;

            // if it was a group which has to be printed in a new page
            // we set off the flag because the group has been printed
            //
            if (m_idxGroupToPrintNP == m_idxGroupHeader)
            {
                m_idxGroupToPrintNP = 0;
            }

            headerSec = m_groups.item(m_idxGroupHeader).getHeader();

            // we need to initialize the variables of every formula
            // in every control located in the header section of the group
            //
            for (int _i = 0; _i < headerSec.getSectionLines().count(); _i++)
            {
                secLn = headerSec.getSectionLines().item(_i);
                for (int _j = 0; _j < secLn.getControls().count(); _j++)
                {
                    ctrl = secLn.getControls().item(_j);
                    if (ctrl.getHasFormulaHide())
                    {
                        m_compiler.initVariable(ctrl.getFormulaHide());
                    }
                    if (ctrl.getHasFormulaValue())
                    {
                        m_compiler.initVariable(ctrl.getFormulaValue());
                    }
                }
            }
        }

        public void markGroupFooterPrinted()
        {
            cReportSection footerSec = null;
            cReportControl ctrl = null;
            cReportSectionLine secLn = null;

            // if the group has been printed we set off the flag
            // used to know if it must be closed
            //
            m_vGroups[m_idxGroupFooter].footerMustBeClosed = false;

            footerSec = m_groups.item(m_idxGroupFooter).getFooter();

            // we need to initialize the variables of every formula
            // in the controls of every section lines in the footer group
            //
            for (int _i = 0; _i < footerSec.getSectionLines().count(); _i++)
            {
                secLn = footerSec.getSectionLines().item(_i);
                for (int _j = 0; _j < secLn.getControls().count(); _j++)
                {
                    ctrl = secLn.getControls().item(_j);
                    if (ctrl.getHasFormulaHide())
                    {
                        m_compiler.initVariable(ctrl.getFormulaHide());
                    }
                    if (ctrl.getHasFormulaValue())
                    {
                        m_compiler.initVariable(ctrl.getFormulaValue());
                    }
                }
            }

            if (pNotPendingFooters())
            {
                m_iRowFormula = m_iRow;
                m_iRow2 = m_iRow;
            }
        }

        public void evalPost()
        {
            evalFunctions(C_IDX_GROUP_DETAIL, csRptWhenEval.CSRPTEVALPOST);
        }

        public void evalPreGroupHeader()
        {
            if (m_idxGroupHeader != 0)
            {
                evalFunctions(m_idxGroupHeader, csRptWhenEval.CSRPTEVALPRE);
            }
        }

        public void evalPreGroupFooter()
        {
            if (m_idxGroupHeader != 0)
            {
                int idxChildGroupFooter = 0;

                idxChildGroupFooter = pGetChildGroupFooterToClose(m_idxGroupHeader);

                // when we close a group we need to evaluate every sub-group
                //
                while (idxChildGroupFooter > m_idxGroupHeader)
                {
                    evalFunctions(idxChildGroupFooter * -1, csRptWhenEval.CSRPTEVALPRE);
                    idxChildGroupFooter = idxChildGroupFooter - 1;
                }

                // finaly we need to evaluate the group that has changed
                //
                evalFunctions(m_idxGroupHeader * -1, csRptWhenEval.CSRPTEVALPRE);
            }
        }

        public void evalPostGroupHeader()
        {
            if (m_idxGroupHeader == 0) { return; }
            evalFunctions(m_idxGroupHeader, csRptWhenEval.CSRPTEVALPOST);
        }

        public void evalPostGroupFooter()
        {
            if (m_idxGroupHeader != 0)
            {

                int idxChildGroupFooter = 0;

                idxChildGroupFooter = pGetChildGroupFooterToClose(m_idxGroupHeader);

                // when we close a group we need to evaluate every sub-group
                //
                while (idxChildGroupFooter > m_idxGroupHeader)
                {
                    evalFunctions(idxChildGroupFooter * -1, csRptWhenEval.CSRPTEVALPOST);
                    idxChildGroupFooter = idxChildGroupFooter - 1;
                }

                // finaly we need to evaluate the group that has changed
                //
                evalFunctions(m_idxGroupHeader * -1, csRptWhenEval.CSRPTEVALPOST);
            }
        }

        private int pGetChildGroupFooterToClose(int idxGroupFather)
        {
            int groupIndex = 0;
            int j = 0;
            for (j = idxGroupFather; j <= m_groupCount; j++)
            {
                if (m_vGroups[j].footerMustBeClosed)
                {
                    groupIndex = j;
                }
            }
            return groupIndex;
        }

        public void evalPre()
        {
            evalFunctions(C_IDX_GROUP_DETAIL, csRptWhenEval.CSRPTEVALPRE);
        }

        public void moveToNext()
        {
            // we move to the next group
            //
            m_iRow = m_iRow + 1;
            m_iRow2 = m_iRow;
            m_iRowFormula = m_iRow;

            // we need to move the additional recordset too
            //
            for (int indexRows = 0; indexRows < m_collRows.Length; indexRows++)
            {
                int indexRow = m_vRowsIndexAux[indexRows] + 1;
                if (m_collRows[indexRows] != null)
                {
                    if (indexRow < m_collRows.Length)
                    {
                        m_vRowsIndexAux[indexRows] = indexRow;
                    }
                }
            }
        }

        private void pExistsGroupToReprintInNP()
        {
            m_bExistsGrpToRePrintInNP = false;
            for (int i = 1; i <= m_groupCount; i++)
            {
                if (m_groups.item(i).getRePrintInNewPage())
                {
                    m_bExistsGrpToRePrintInNP = true;
                    return;
                }
            }
        }

        private bool pNotPendingFooters()
        {
            for (int i = 1; i <= m_groupCount; i++)
            {
                if (m_vGroups[i].footerMustBeClosed)
                {
                    return false;
                }
            }
            return true;
        }

        // it only returns one of the following:
        //
        //      GroupH
        //      Detail
        //      GroupF
        //      End
        //
        public csRptGetLineResult getLineType()
        {
            // if there are groups footers which need to be printed
            //
            if (m_idxGroupFooter > 0)
            {
                if (m_vGroups[m_idxGroupFooter].footerMustBeClosed)
                {
                    return csRptGetLineResult.CSRPTGLGROUPFOOTER;
                }
            }

            // if there are groups headers which need to be printed
            //
            if (m_idxGroupHeader > 0)
            {
                if (m_vGroups[m_idxGroupHeader].changed)
                {
                    return csRptGetLineResult.CSRPTGLGROUPHEADER;
                }
            }

            // if reach the end of the report and there are not groups
            // which need to be printed we have ended
            //
            if (pReportIsDone())
            {
                return csRptGetLineResult.CSRPTGLEND;
            }

            // If there are group headers:
            //
            // - which need to be printed in a new page
            // o
            // - which need to be re-printed because we are in a new page
            //
            if (m_idxGroupToPrintNP > 0 || m_bHaveToRePrintGroup)
            {
                return csRptGetLineResult.CSRPTGLVIRTUALH;
            }

            // if there are groups footers which have to be printed
            //
            if (pEvalFooterToClose2())
            {
                return csRptGetLineResult.CSRPTGLVIRTUALF;
            }

            // if there is nothing more to do we have finished
            //
            if (m_iRow > m_vRowsIndex.Length && pNotPendingFooters())
            {
                return csRptGetLineResult.CSRPTGLEND;
            }

            // if there are group headers to process
            //
            if (pGetLineAuxPrintHeader())
            {
                return csRptGetLineResult.CSRPTGLVIRTUALH;
            }

            // if we get here we are in line of the detail
            //
            return csRptGetLineResult.CSRPTGLDETAIL;
        }

        // it returns every controls of a line
        // it moves through every row in the main recordset
        //
        public csRptGetLineResult getLine(cReportPageFields fields)
        {
            // to know if we need to print in a new page
            // because a group has changed its value
            //
            bool bGetNewPage = false;

            if (fields != null)
            {
                fields.clear();
            }

            // if there are not pending calls to close or open groups
            //
            if (!(m_bCloseFooter || m_bOpenHeader))
            {

                // if there are not group headers to be re-printed in this page
                //
                if (!pExistsGroupHeadersToReprint())
                {

                    // we process the line
                    //
                    csRptGetLineResult rslt = pGetLineWork(fields, out bGetNewPage);
                    if (bGetNewPage)
                    {
                        return csRptGetLineResult.CSRPTGLNEWPAGE;
                    }
                    else
                    {
                        if (rslt == csRptGetLineResult.CSRPTGLEND || rslt == csRptGetLineResult.CSRPTGLVIRTUALF)
                        {
                            return rslt;
                        }
                    }
                }
            }

            // if we must close footers
            //
            if (m_bCloseFooter)
            {
                return pGetLineAuxGroupFooter(fields);
            }
            // if the group has changed
            //
            else if (m_bOpenHeader)
            {
                return pGetLineAuxGroupHeader(bGetNewPage, fields);
            }
            // process a details line
            //
            else
            {
                return pGetLineAuxDetail(fields);
            }
        }

        private csRptGetLineResult pGetLineWork(cReportPageFields fields, out bool bGetNewPage)
        {
            bGetNewPage = false;

            // if the user has cancel we have finished
            //
            if (pGetLineAuxReportCancel() == csRptGetLineResult.CSRPTGLEND)
            {
                return csRptGetLineResult.CSRPTGLEND;
            }

            // if we reach the end of the report and there are not groups to process
            // we have finished
            //
            csRptGetLineResult rslt = pGetLineWorkAuxReportEnd();
            if (rslt == csRptGetLineResult.CSRPTGLEND || rslt == csRptGetLineResult.CSRPTGLVIRTUALF)
            {
                return rslt;
            }

            // field collection for this line
            //
            fields = new cReportPageFields();

            // if we need to print the group in a new page
            //
            if (m_idxGroupToPrintNP > 0)
            {
                pGetLineAuxPrintGroupInNP();
            }
            // we need to process groups
            //
            else
            {
                // if the report have groups
                //
                if (m_groupCount > 0)
                {
                    // if we don't need to re-print group headers
                    //
                    if (!m_bHaveToRePrintGroup)
                    {
                        pEvalFooterToClose();
                    }

                    // if we don't need to re-print group footers
                    //
                    if (!m_bCloseFooter)
                    {
                        // if have done all the pending work we have finished
                        //
                        if (pGetLineAuxReportIsDone() == csRptGetLineResult.CSRPTGLEND)
                        {
                            return csRptGetLineResult.CSRPTGLEND;
                        }

                        // continue with the next group
                        //
                        pGetLineAuxDoGroups(bGetNewPage);
                    }
                }
            }
            return csRptGetLineResult.CSRPTGLNONE;
        }

        private void pGetLineAuxPrintGroupInNP()
        {
            m_idxGroupHeader = m_idxGroupToPrintNP;
            m_idxGroupToPrintNP = 0;
            m_bOpenHeader = true;
        }

        private bool pReportIsDone()
        {
            // if we have finished return csRptGLEnd
            //
            if (m_rows == null || m_iRow > m_recordCount)
            {
                // if there are not pending footers we have finished
                // 
                if (!m_bPrintFooter)
                {
                    return true;
                }
            }
            return false;
        }

        private csRptGetLineResult pGetLineWorkAuxReportEnd()
        {
            // if we have finished return csRptGLEnd
            //
            if (m_rows == null || m_iRow > m_recordCount)
            {
                if (m_iRow > m_recordCount)
                {
                    m_iRow2 = m_recordCount;
                }

                // if there are footer to be printed
                //
                if (m_bPrintFooter)
                {
                    // if we to eval functions before print
                    //
                    if (m_bEvalPreGroups)
                    {
                        // set this flag off to allow the next call to 
                        // getLine() -> pGetLineWork() -> pGetLineWorkAuxReportEnd()
                        // to print the footer
                        //
                        m_bEvalPreGroups = false;

                        return csRptGetLineResult.CSRPTGLVIRTUALF;
                    }
                    else
                    {
                        if (!m_bLastFootersWasPrinted)
                        {
                            // set this flag on to know we have started to
                            // close group footers
                            //
                            m_bLastFootersWasPrinted = true;

                            // we force a change in the first group to force
                            // the close of every group footer
                            //
                            m_groupIndexChange = 1;

                            // set the flag of the last group on to force this call to
                            // print it and the next footers will be printed in sucesive
                            // calls to getLine() -> pGetLineWork() -> pGetLineWorkAuxReportEnd()
                            //
                            m_vGroups[m_vGroups.Length - 1].footerMustBeClosed = true;
                        }
                    }
                }
                // if there are no more footers to be closed we have finished
                // 
                else
                {
                    reportDone();
                    return csRptGetLineResult.CSRPTGLEND;
                }
            }
            return csRptGetLineResult.CSRPTGLNONE;
        }

        private csRptGetLineResult pGetLineAuxReportCancel()
        {
            // if the user has canceled we have finished
            //
            if (!OnProgress("", 0, m_iRow, m_recordCount))
            {
                reportDone();
                return csRptGetLineResult.CSRPTGLEND;
            }
            else
            {
                return csRptGetLineResult.CSRPTGLNONE;
            }
        }

        private csRptGetLineResult pGetLineAuxReportIsDone()
        {
            // if we have printed the las footer we have finished
            //
            if (m_iRow > m_vRowsIndex.Length && pNotPendingFooters())
            {
                reportDone();
                m_bPrintFooter = false;
                return csRptGetLineResult.CSRPTGLEND;
            }
            return csRptGetLineResult.CSRPTGLNONE;
        }

        private bool pEvalFooterToClose2()
        {
            int i = 0;
            for (i = m_groupCount; i <= 1; i--)
            {
                if (m_vGroups[i].footerMustBeClosed)
                {
                    return true;
                }
            }
            return false;
        }

        private bool pEvalFooterToClose()
        {
            int i = 0;
            for (i = m_groupCount; i <= 1; i--)
            {
                if (m_vGroups[i].footerMustBeClosed)
                {
                    m_idxGroupFooter = i;

                    // we have to check only the footer or the group which has
                    // changed and its subgroups
                    //
                    if (m_idxGroupFooter > m_groupIndexChange)
                    {

                        // we need to close the footer of the group which contains it
                        //
                        m_vGroups[m_idxGroupFooter - 1].footerMustBeClosed = true;
                    }
                    m_bCloseFooter = true;
                    break;
                }
            }
            return m_bCloseFooter;
        }

        private bool pGetLineAuxPrintHeader()
        {
            // we need to evaluate groups
            //
            for (int i = 1; i <= m_groupCount; i++)
            {

                if (!m_vGroups[i].grandTotalGroup)
                {

                    if (m_vGroups[i].value == null)
                    {
                        return true;
                    }

                    int col = m_vGroups[i].indexField;
                    int row = m_vRowsIndex[m_iRow2];

                    switch (m_vGroups[i].comparisonType)
                    {
                        case csRptGrpComparisonType.CSRPTGRPTEXT:
                            String text = cReportGlobals.valVariant(m_rows.Rows[row][col]).ToString().ToLower();
                            if (m_vGroups[i].value.ToString() != text)
                            {
                                return true;
                            }
                            break;
                        case csRptGrpComparisonType.CSRPTGRPNUMBER:
                            double number = cReportGlobals.val(cReportGlobals.valVariant(m_rows.Rows[row][col]));
                            if ((double)m_vGroups[i].value != number)
                            {
                                return true;
                            }
                            break;
                        case csRptGrpComparisonType.CSRPTGRPDATE:
                            DateTime date = cReportGlobals.dateValue(cReportGlobals.valVariant(m_rows.Rows[row][col]));
                            if ((DateTime)m_vGroups[i].value != date)
                            {
                                return true;
                            }
                            break;
                    }
                }
            }
            return false;
        }

        private bool orderDateAsc(int first, int last, int orderBy)
        {
            int i = 0;
            int j = 0;
            int t = 0;
            int q = 0;
            bool bChanged = false;

            t = pEstimateLoops(last - first);
            for (i = first + 1; i <= last; i++)
            {
                bChanged = false;
                for (j = last; j <= i; j--)
                {
                    q = q + 1;
                    int row1 = m_vRowsIndex[j];
                    int row2 = m_vRowsIndex[j-1];
                    DateTime date1 = cReportGlobals.dateValue(cReportGlobals.valVariant(m_rows.Rows[row1][orderBy]));
                    DateTime date2 = cReportGlobals.dateValue(cReportGlobals.valVariant(m_rows.Rows[row2][orderBy]));
                    if (date1 < date2)
                    {
                        if (!OnProgress("", 0, q, t))
                        {
                            return false;
                        }
                        changeRow(j, j - 1);
                        bChanged = true;
                    }
                }
                if (!OnProgress("", 0, q, t))
                {
                    return false;
                }
                if (!bChanged)
                {
                    break;
                }
            }
            return true;
        }

        private bool orderDateDesc(int first, int last, int orderBy)
        {
            int i = 0;
            int j = 0;
            int t = 0;
            int q = 0;
            bool bChanged = false;

            t = pEstimateLoops(last - first);
            for (i = first + 1; i <= last; i++)
            {
                bChanged = false;
                for (j = last; j <= i; j--)
                {
                    q = q + 1;
                    int row1 = m_vRowsIndex[j];
                    int row2 = m_vRowsIndex[j - 1];
                    DateTime date1 = cReportGlobals.dateValue(cReportGlobals.valVariant(m_rows.Rows[row1][orderBy]));
                    DateTime date2 = cReportGlobals.dateValue(cReportGlobals.valVariant(m_rows.Rows[row2][orderBy]));
                    if (date1 > date2)
                    {
                        if (!OnProgress("", 0, q, t)) 
                        { 
                            return false; 
                        }
                        changeRow(j, j - 1);
                        bChanged = true;
                    }
                }
                if (!OnProgress("", 0, q, t)) 
                { 
                    return false; 
                }
                if (!bChanged) 
                { 
                    break; 
                }
            }
            return true;
        }

        private void pGetLineAuxDoGroups(bool bGetNewPage)
        { // TODO: Use of ByRef founded Private Sub pGetLineAuxDoGroups(ByRef bGetNewPage As Boolean)
            // we continue evaluating groups
            //
            for (int i = 1; i <= m_groupCount; i++)
            {

                // if the group has changed
                //
                // we only get into here where
                //
                //  - a group has changed in the previous call to 
                //    GetLine, and we have closed every GroupsFooter
                //    in previous calls to GetLine or
                //
                //  - we are in a new page and need to re-print group headers
                //
                if (m_vGroups[i].changed)
                {
                    pGroupChanged(i, bGetNewPage);
                    break;
                }
                else
                {
                    pEvalGroupChange(i);

                    if (m_vGroups[i].changed)
                    {
                        m_idxGroupHeader = i;

                        // if it is the first time we are printing groups
                        //
                        if (m_firstGroup)
                        {
                            pOpenGroupHeader(i);
                        }
                        // the first thing to do is to close footers
                        //
                        else
                        {
                            pCloseGroupFooters(i);
                        }
                        break;
                    }
                }
            }
        }

        private void pCloseGroupFooters(int i)
        {
            // save the index of the outer footer we need to close
            //
            m_groupIndexChange = i;

            m_bCloseFooter = true;
            m_idxGroupFooter = m_groupCount;

            // when a group changes we need to close from the
            // most inner group to the most outer group 
            // which is changing (m_GroupIndexChange)
            //
            int j = 0;

            for (j = m_groupIndexChange; j <= m_idxGroupFooter; j++)
            {
                m_vGroups[j].footerMustBeClosed = true;
            }
        }

        private void pOpenGroupHeader(int i)
        {
            // set this flag off to know we need to print the last footers
            //
            m_bLastFootersWasPrinted = false;
            m_vGroups[i].changed = false;
            m_idxGroupHeader = i;

            // set this flag on to know we need to close
            // the next group in a future call to getLine()
            // only if there are more group
            //
            if (i < m_groupCount)
            {
                m_vGroups[i + 1].changed = true;
            }
            m_bOpenHeader = true;
        }

        private void changeGroup(int i, object value)
        {
            m_vGroups[i].value = value;
            m_vGroups[i].changed = true;
            if (!m_firstGroup)
            {
                m_vGroups[i].footerMustBeClosed = true;
            }
            pEvalGroupChangedAux(i + 1);
        }

        private void pEvalGroupChange(int i)
        {
            if (m_vGroups[i].grandTotalGroup)
            {
                if (m_vGroups[i].value == null)
                {
                    changeGroup(i, "1");
                }
            }
            else
            {
                int col = m_vGroups[i].indexField;
                int row = m_vRowsIndex[m_iRow2];
                switch (m_vGroups[i].comparisonType)
                {
                    case csRptGrpComparisonType.CSRPTGRPTEXT:
                        String text = cReportGlobals.valVariant(m_rows.Rows[row][col]).ToString().ToLower();
                        if (m_vGroups[i].value == null)
                        {
                            changeGroup(i, text);
                        }
                        else if (m_vGroups[i].value.ToString() != text)
                        {
                            changeGroup(i, text);
                        }
                        break;
                    
                    case csRptGrpComparisonType.CSRPTGRPNUMBER:
                        double number = cReportGlobals.val(cReportGlobals.valVariant(m_rows.Rows[row][col]));
                        if (m_vGroups[i].value == null)
                        {
                            changeGroup(i, number);
                        }
                        else if ((double)m_vGroups[i].value != number)
                        {
                            changeGroup(i, number);
                        }
                        break;

                    case csRptGrpComparisonType.CSRPTGRPDATE:
                        DateTime date = cReportGlobals.dateValue(cReportGlobals.valVariant(m_rows.Rows[row][col]));
                        if (m_vGroups[i].value == null)
                        {
                            changeGroup(i, date);
                        }
                        else if ((DateTime)m_vGroups[i].value != date)
                        {
                            changeGroup(i, date);
                        }
                        break;
                }
            }
        }

        private void pEvalGroupChangedAux(int i)
        {
            for (; i <= m_groupCount; i++)
            {
                pGroupChangedAux(i);
            }
        }

        private void pGroupChangedAux(int i)
        {
            int col = m_vGroups[i].indexField;
            int row = m_vRowsIndex[m_iRow2];
            switch (m_vGroups[i].comparisonType)
            {
                case csRptGrpComparisonType.CSRPTGRPTEXT:
                    m_vGroups[i].value = cReportGlobals.valVariant(m_rows.Rows[row][col]).ToString().ToLower();
                    break;
                case csRptGrpComparisonType.CSRPTGRPNUMBER:
                    m_vGroups[i].value = cReportGlobals.val(cReportGlobals.valVariant(m_rows.Rows[row][col]));
                    break;
                case csRptGrpComparisonType.CSRPTGRPDATE:
                    m_vGroups[i].value = cReportGlobals.dateValue(cReportGlobals.valVariant(m_rows.Rows[row][col]));
                    break;
            }
        }

        private void pGroupChanged(int i, bool bGetNewPage)
        { // TODO: Use of ByRef founded Private Sub pGroupChanged(ByVal i As Integer, ByRef bGetNewPage As Boolean)
            m_idxGroupHeader = i;
            pGroupChangedAux(i);

            bGetNewPage = m_groups.item(i).getPrintInNewPage() && !m_firstGroup;
            m_idxGroupHeader = i;

            if (bGetNewPage)
            {
                // setting it to any value but zero we mean that this group
                // must be printed in a new page
                //
                m_idxGroupToPrintNP = i;
            }
            else
            {
                m_idxGroupToPrintNP = 0;
            }

            // set this flag on to open this group in a future
            // call to getLine(). only if there are more groups
            //
            if (i < m_groupCount)
            {
                m_vGroups[i + 1].changed = true;
            }
            m_bOpenHeader = true;
        }

        private csRptGetLineResult pGetLineAuxGroupFooter(cReportPageFields fields)
        { // TODO: Use of ByRef founded Private Function pGetLineAuxGroupFooter(ByRef Fields As cReportPageFields) As csRptGetLineResult
            cReportSection footerSec = null;
            cReportControl ctrl = null;
            cReportSectionLine secLn = null;

            // if we need to evaluate functions which must run
            // before printing
            //
            if (m_bEvalPreGroups)
            {
                // when we are evaluating this kind of formulas we must use
                // the previous row because here we are closing groups
                // which means the current row doesn't belong to the
                // group we are closing
                //
                // NOTE: whe we have done whit printing the footers
                // we need to set m_iRowFormula and m_iRow2 to their 
                // original values
                //
                m_iRowFormula = m_iRow - 1;
                m_iRow2 = m_iRow - 1;

                // to force the next call to getLine() to close the footer
                //
                m_bEvalPreGroups = false;

                return csRptGetLineResult.CSRPTGLVIRTUALF;
            }
            else
            {

                // if there are more footers to be printed this
                // flag will be turn on in the next call to getLine()
                //
                m_bCloseFooter = false;

                // to force the next call to return CSRPTGLVIRTUALF
                //
                m_bEvalPreGroups = true;

                footerSec = m_groups.item(m_idxGroupFooter).getFooter();

                getLineAux(footerSec, fields);

                return csRptGetLineResult.CSRPTGLGROUPFOOTER;
            }
        }

        private csRptGetLineResult pGetLineAuxGroupHeader(bool bGetNewPage, cReportPageFields fields)
        { // TODO: Use of ByRef founded Private Function pGetLineAuxGroupHeader(ByVal bGetNewPage As Boolean, ByRef Fields As cReportPageFields) As csRptGetLineResult
            cReportSection headerSec = null;

            if (bGetNewPage && !m_firstGroup)
            {
                // in the deatil and group headers the row for formulas
                // is the current row
                //
                m_iRowFormula = m_iRow;

                return csRptGetLineResult.CSRPTGLNEWPAGE;
            }
            else
            {

                // if we need to evaluate the functions which must
                // run before printing
                //
                if (m_bEvalPreGroups)
                {
                    // if we are not reprinting group headers
                    //
                    if (!m_bHaveToRePrintGroup)
                    {
                        // in the detail and group headers the row for formulas
                        // is the current row
                        //
                        m_iRowFormula = m_iRow;
                    }
                    // to force the next call to getLine() to print the footer
                    //
                    m_bEvalPreGroups = false;

                    return csRptGetLineResult.CSRPTGLVIRTUALH;
                }
                else
                {

                    m_bOpenHeader = false;

                    // to force the next call to getLine() to return CSRPTGLVIRTUALF
                    //
                    m_bEvalPreGroups = true;
                    headerSec = m_groups.item(m_idxGroupHeader).getHeader();
                    getLineAux(headerSec, fields);

                    // set this flag on to indicate we have footers to close
                    //
                    m_bPrintFooter = true;

                    // we return a group line
                    //
                    return csRptGetLineResult.CSRPTGLGROUPHEADER;
                }
            }
        }

        private csRptGetLineResult pGetLineAuxDetail(cReportPageFields fields)
        { // TODO: Use of ByRef founded Private Function pGetLineAuxDetail(ByRef Fields As cReportPageFields) As csRptGetLineResult
            m_firstGroup = false;

            getLineAux(m_details.item(1), fields);

            // we return a detail line
            //
            return csRptGetLineResult.CSRPTGLDETAIL;
        }

        private void getLineAux(cReportSection sec, cReportPageFields fields)
        {
            // for every control in every section line of sec
            // we need to create a new cPageField
            //
            cReportPageField field = null;
            cReportSectionLine secLn = null;
            cReportControl ctrl = null;
            bool isVisible = false;
            int indexCtrl = 0;

            // this indexes are used to
            //
            // indicate in which data source is this field
            //
            int indexRows = 0;
            //
            // in which row is this field
            //
            int indexRow = 0;
            //
            // in which column is this field
            //
            int indexField = 0;

            if (sec.getHasFormulaHide())
            {
                isVisible = cReportGlobals.val(m_compiler.resultFunction(sec.getFormulaHide())) != 0;
            }
            else
            {
                isVisible = true;
            }

            if (isVisible)
            {
                // for every section line in sec
                //
                for (int _i = 0; _i < sec.getSectionLines().count(); _i++)
                {
                    secLn = sec.getSectionLines().item(_i);
                    m_lineIndex++;

                    if (secLn.getHasFormulaHide())
                    {
                        m_compiler.evalFunction(secLn.getFormulaHide());
                        isVisible = cReportGlobals.val(m_compiler.resultFunction(secLn.getFormulaHide())) != 0;
                    }
                    else
                    {
                        isVisible = true;
                    }

                    if (isVisible)
                    {
                        // for every control in the section line
                        //
                        int[] collByLeft = secLn.getControls().getCollByLeft();
                        for (indexCtrl = 1; indexCtrl <= collByLeft.Length; indexCtrl++)
                        {
                            ctrl = secLn.getControls().item(collByLeft[indexCtrl]);

                            // add a new field to the collection
                            //
                            field = fields.add(null, "");
                            field.setIndexLine(m_lineIndex);

                            if (ctrl.getHasFormulaValue())
                            {
                                field.setValue(
                                    cReportGlobals.format(
                                        m_compiler.resultFunction(ctrl.getFormulaValue()),
                                        ctrl.getLabel().getAspect().getFormat()));
                            }
                            else
                            {
                                cReportLabel w_label = null;
                                switch (ctrl.getControlType())
                                {
                                    case csRptControlType.CSRPTCTFIELD:

                                        pGetIndexRows(indexRows, indexRow, indexField, ctrl);

                                        if (m_collRows[indexRows] != null)
                                        {
                                            // it looks ugly, dont think you?
                                            //
                                            // maybe this help a litle:
                                            //
                                            //    m_vCollRows(IndexRows)    a matrix with the data 
                                            //                              contained in the datasource
                                            //                              referd by this control
                                            //
                                            //    (IndexField, IndexRow)    a cell in this matrix
                                            //
                                            object value = m_collRows[indexRows].Rows[indexRow][indexField];
                                            field.setValue(
                                                cReportGlobals.format(
                                                    cReportGlobals.valVariant(value),
                                                    ctrl.getLabel().getAspect().getFormat()));
                                        }
                                        break;

                                    case csRptControlType.CSRPTCTLABEL:
                                        w_label = ctrl.getLabel();
                                        field.setValue(cReportGlobals.format(w_label.getText(), w_label.getAspect().getFormat()));
                                        break;

                                    case csRptControlType.CSRPTCTIMAGE:
                                        w_label = ctrl.getLabel();
                                        field.setValue(cReportGlobals.format(w_label.getText(), w_label.getAspect().getFormat()));
                                        field.setImage(ctrl.getImage().getImage());
                                        break;

                                    case csRptControlType.CSRPTCTDBIMAGE:
                                        pGetIndexRows(indexRows, indexRow, indexField, ctrl);
                                        if (m_collRows[indexRows] != null)
                                        {
                                            field.setImage(pGetImage(indexRows, indexField, indexRow));
                                        }
                                        break;

                                    case csRptControlType.CSRPTCTCHART:
                                        pGetIndexRows(indexRows, indexRow, indexField, ctrl);
                                        field.setImage(pGetChartImage(indexRows, indexField, indexRow, ctrl));
                                        break;
                                }
                            }

                            if (ctrl.getHasFormulaHide())
                            {
                                field.setVisible(cReportGlobals.val(m_compiler.resultFunction(ctrl.getFormulaHide())) != 0);
                            }
                            else
                            {
                                field.setVisible(true);
                            }

                            // set a reference to the definition of this field
                            //
                            field.setInfo(m_pageSetting.item(ctrl.getKey()));
                        }
                    }
                }
            }
        }

        // indexRows     define the datasource
        // indexRow      define the row in the datasource
        //
        private void pGetIndexRows(int indexRows, int indexRow, int indexField, cReportControl ctrl)
        { // TODO: Use of ByRef founded Private Sub pGetIndexRows(ByRef IndexRows As Long, ByRef IndexRow As Long, ByRef IndexField As Long, ByRef ctrl As cReportControl)
            // the datasource index have an offset of 1000 between each other
            //
            indexRows = (int)(ctrl.getField().getIndex() / 1000);
            indexField = ctrl.getField().getIndex() - (indexRows * 1000);

            if (indexRows == 0)
            {
                indexRow = m_vRowsIndex[m_iRow2];
            }
            else
            {
                indexRow = m_vRowsIndexAux[indexRows];
            }
        }

        public bool init(cReportLaunchInfo oLaunchInfo)
        {
            try
            {
                setLaunchInfo(oLaunchInfo);
                return true;
            }
            catch (Exception ex)
            {
                cError.mngError(ex, "Init", C_MODULE, "");
                return false;
            }
        }

        // run report
        //
		public bool launch()
		{
			return launch(null);
		}
        public bool launch(cReportLaunchInfo oLaunchInfo)
        { // TODO: Use of ByRef founded Public Function Launch(Optional ByRef oLaunchInfo As cReportLaunchInfo = Nothing) As Boolean
            try
            {
                int errNumber = 0;
                String errSource = "";
                String errDescription = "";
                String errHelpfile = "";
                int errHelpcontext = 0;

                List<object[]> recordsets = null;

                m_compiler.setReport(this);
                m_compiler.initGlobalObject();

                if (oLaunchInfo == null)
                {
                    if (m_launchInfo == null)
                    {
                        throw new ReportLaunchInfoNoDefined(
                            C_MODULE,
                            cReportError.errGetDescript(
                                            csRptErrors.LAUNCH_INFO_UNDEFINED));
                    }
                }
                else
                {
                    setLaunchInfo(oLaunchInfo);
                }

                if (m_launchInfo.getPrinter() == null)
                {
                    throw new ReportLaunchInfoNoDefined(
                        C_MODULE,
                        cReportError.errGetDescript(
                                        csRptErrors.PRINTER_NOT_DEFINED));
                }

                if (!OnProgress("Preparando el reporte"))
                {
                    return false;
                }

                // we need to sort all controls using the zorder property
                //
                sortCollection();

                if (!OnProgress("Compiling report ..."))
                {
                    return false;
                }

                // compile report
                //
                if (!compileReport())
                {
                    return false;
                }

                // we need to sort all controls by his aspect.left property
                //
                pSortControlsByLeft();

                if (!OnProgress("Querying database"))
                {
                    return false;
                }

                recordsets = new List<object[]>();

                G.redim(ref m_collRows, 0);

                // get the main recordset
                //
                if (!pGetData(ref m_rows, m_connect, true, recordsets))
                {
                    return false;
                }

                // the first element contains the main recordset
                //
                m_collRows[0] = m_rows;

                pInitImages();

                // get additional recordsets
                //
                if (!pGetDataAux(recordsets))
                {
                    return false;
                }

                if (!initGroups(m_rows, pGetMainDataSource(recordsets)))
                {
                    return false;
                }

                if (!OnProgress("Initializing report"))
                {
                    return false;
                }

                if (!initControls(recordsets))
                {
                    return false;
                }

                // create the definition of this report
                //
                if (!createPageSetting())
                {
                    return false;
                }

                m_pages.clear();
                m_lineIndex = 0;

                // globals initialization
                //
                m_bPrintFooter = false;
                m_bLastFootersWasPrinted = false;
                m_groupIndexChange = 0;
                m_iRow2 = 0;
                m_iRowFormula = 0;
                pSetGroupFormulaHeaders();
                pSetGroupsInCtrlFormulaHide();
                pSetIndexColInGroupFormulas(recordsets);
                pInitRowFormulas();

                // check if there are groups which need to be reprinted when the page change
                //
                pExistsGroupToReprintInNP();

                // to force the evaluate of the groups in the first page
                //
                m_bEvalPreGroups = true;
                m_bCloseFooter = false;
                m_bOpenHeader = false;

                cReportFormula formula = null;
                for (int _i = 0; _i < m_formulas.count(); _i++)
                {
                    formula = m_formulas.item(_i);
                    formula.setHaveToEval(true);
                }

                // launch the report
                //
                m_launchInfo.getObjPaint().setReport(this);
                if (!m_launchInfo.getObjPaint().makeReport())
                {
                    return false;
                }

                switch (m_launchInfo.getAction())
                {
                    case csRptLaunchAction.CSRPTLAUNCHPRINTER:
                        if (!m_launchInfo.getObjPaint().printReport())
                        {
                            return false;
                        }
                        break;
                    case csRptLaunchAction.CSRPTLAUNCHFILE:
                        if (!m_launchInfo.getObjPaint().makeXml())
                        {
                            return false;
                        }
                        break;
                    case csRptLaunchAction.CSRPTLAUNCHPREVIEW:
                        if (!m_launchInfo.getObjPaint().previewReport())
                        {
                            return false;
                        }
                        break;
                }

                return true;

            }
            catch (Exception ex)
            {
                m_compiler.setReport(null);

                // if we haven't printed to preview
                // we need to clear the references 
                // between cReport and cReportLaunchInfo
                //
                if (m_launchInfo.getAction() != csRptLaunchAction.CSRPTLAUNCHPREVIEW)
                {
                    m_launchInfo.getObjPaint().setReport(null);
                    m_launchInfo.setObjPaint(null);
                }

                throw new ReportException(csRptErrors.ERROR_WHEN_RUNNING_REPORT,
                                          C_MODULE,
                                          "Error when running report.\n\n"
                                          + "Info: " + ex.Message + "\n"
                                          + "Source: " + ex.Source + "\n"
                                          + "Stack trace: " + ex.StackTrace + "\n"
                                          + "Description: " + ex.ToString()
                                          );
            }
        }

        public bool loadSilent(String fileName)
        {

            try
            {
                CSXml.cXml docXml = null;
                docXml = new CSXml.cXml();

                CSKernelFile.cFile f = null;
                f = new CSKernelFile.cFile();

                m_path = cFile.getPath(fileName);
                m_name = cFile.getFileName(fileName);

                docXml.init(null);
                docXml.setFilter(C_FILEEX);
                docXml.setName(m_name);
                docXml.setPath(m_path);

                if (!docXml.openXml())
                {
                    return false;
                }

                m_path = docXml.getPath();
                m_name = docXml.getName();
                CSXml.cXmlProperty property = docXml.getNodeProperty(docXml.getRootNode(), "ReportDisconnected");
                m_reportDisconnected = property.getValueBool(eTypes.eBoolean);

                return nLoad(docXml);
            }
            catch (Exception ex)
            {
                cError.mngError(ex, "LoadSilent", C_MODULE, "");
                return false;
            }
        }

        public bool load(object commDialog)
        { // TODO: Use of ByRef founded Public Function Load(ByRef CommDialog As Object) As Boolean
            try
            {
                CSXml.cXml docXml = null;
                docXml = new CSXml.cXml();

                docXml.init(commDialog);
                docXml.setFilter(C_FILEEX);

                if (m_name != "")
                {
                    docXml.setName(m_name);
                }
                else
                {
                    docXml.setPath(m_pathDefault + "\\*." + C_FILEEX);
                }

                docXml.setPath(m_path);

                if (!docXml.openXmlWithDialog())
                {
                    return false;
                }

                m_path = docXml.getPath();
                m_name = docXml.getName();
                CSXml.cXmlProperty property = docXml.getNodeProperty(docXml.getRootNode(), "ReportDisconnected");
                m_reportDisconnected = property.getValueBool(eTypes.eBoolean);

                return nLoad(docXml);
            }
            catch (Exception ex)
            {
                cError.mngError(ex, "Load", C_MODULE, "");
                return false;
            }
        }

        public bool save(object commDialog, bool withDialog)
        { // TODO: Use of ByRef founded Public Function Save(ByRef CommDialog As Object, Optional ByVal WithDialog As Boolean = True) As Boolean
            CSXml.cXml docXml = null;
            docXml = new CSXml.cXml();

            docXml.init(commDialog);
            docXml.setFilter(C_FILEEX);
            docXml.setName(m_name);
            docXml.setPath(m_path);

            if (withDialog)
            {
                if (!docXml.newXmlWithDialog())
                {
                    return false;
                }
            }
            else
            {
                if (!docXml.newXml())
                {
                    return false;
                }
            }

            m_name = docXml.getName();
            m_path = docXml.getPath();

            CSXml.cXmlProperty xProperty = null;
            xProperty = new CSXml.cXmlProperty();

            xProperty.setName("RptName");
            xProperty.setValue(eTypes.eText, m_name);
            docXml.addProperty(xProperty);

            xProperty.setName("ReportDisconnected");
            xProperty.setValue(eTypes.eBoolean, m_reportDisconnected);
            docXml.addProperty(xProperty);

            // sections
            //
            cReportSection sec = null;
            XmlNode nodeObj = null;

            if (!m_connect.save(docXml, null))
            {
                return false;
            }
            if (!m_connectsAux.save(docXml, null))
            {
                return false;
            }
            if (!m_launchInfo.save(docXml, null))
            {
                return false;
            }

            xProperty.setName(C_NODERPTHEADERS);
            xProperty.setValue(eTypes.eText, "");
            nodeObj = docXml.addNode(xProperty);

            for (int _i = 0; _i < m_headers.count(); _i++)
            {
                sec = m_headers.item(_i);
                sec.save(docXml, nodeObj);
            }

            xProperty.setName(C_NODERPTDETAILS);
            xProperty.setValue(eTypes.eText, "");
            nodeObj = docXml.addNode(xProperty);

            for (int _i = 0; _i < m_details.count(); _i++)
            {
                sec = m_details.item(_i);
                sec.save(docXml, nodeObj);
            }

            xProperty.setName(C_NODERPTFOOTERS);
            xProperty.setValue(eTypes.eText, "");
            nodeObj = docXml.addNode(xProperty);

            for (int _i = 0; _i < m_footers.count(); _i++)
            {
                sec = m_footers.item(_i);
                sec.save(docXml, nodeObj);
            }

            xProperty.setName(C_NODEGROUPS);
            xProperty.setValue(eTypes.eText, "");
            nodeObj = docXml.addNode(xProperty);

            cReportGroup group = null;

            for (int _i = 0; _i < m_groups.count(); _i++)
            {
                group = m_groups.item(_i);
                group.save(docXml, nodeObj);
            }

            xProperty.setName(C_NODERPTFORMULAS);
            xProperty.setValue(eTypes.eText, "");
            nodeObj = docXml.addNode(xProperty);

            cReportFormula formula = null;
            for (int _i = 0; _i < m_formulas.count(); _i++)
            {
                formula = m_formulas.item(_i);
                if (!formula.getNotSave())
                {
                    formula.save(docXml, nodeObj);
                }
            }

            xProperty.setName(C_NODEPAPERINFO);
            xProperty.setValue(eTypes.eText, "");
            nodeObj = docXml.addNode(xProperty);
            m_paperInfo.save(docXml, nodeObj);

            if (!docXml.save())
            {
                return false;
            }

            if (!docXml.openXml())
            {
                return false;
            }

            if (!nLoad(docXml))
            {
                return false;
            }

            return true;
        }

        public bool loadSilentData(String fileName)
        {
            CSXml.cXml docXml = null;
            docXml = new CSXml.cXml();

            m_path = CSKernelFile.cFile.getPath(fileName);
            m_name = CSKernelFile.cFile.getFileName(fileName);

            docXml.init(null);
            docXml.setFilter(C_FILEDATAEX);
            docXml.setName(m_name);
            docXml.setPath(m_path);

            if (!docXml.openXml())
            {
                return false;
            }

            m_path = docXml.getPath();
            m_name = docXml.getName();

            CSXml.cXmlProperty property = docXml.getNodeProperty(docXml.getRootNode(), "ReportDisconnected");
            m_reportDisconnected = property.getValueBool(eTypes.eBoolean);

            return nLoadData(docXml);
        }

        public bool loadData(object commDialog)
        { // TODO: Use of ByRef founded Public Function LoadData(ByRef CommDialog As Object) As Boolean
            CSXml.cXml docXml = null;
            docXml = new CSXml.cXml();

            docXml.init(commDialog);
            docXml.setFilter(C_FILEDATAEX);
            docXml.setName(m_name);
            docXml.setPath(m_path);

            if (!docXml.openXmlWithDialog())
            {
                return false;
            }

            m_path = docXml.getPath();
            m_name = docXml.getName();
            CSXml.cXmlProperty property = docXml.getNodeProperty(docXml.getRootNode(), "ReportDisconnected");
            m_reportDisconnected = property.getValueBool(eTypes.eBoolean);

            return nLoadData(docXml);
        }

        public bool saveData(object commDialog, bool withDialog)
        { // TODO: Use of ByRef founded Public Function SaveData(ByRef CommDialog As Object, Optional ByVal WithDialog As Boolean = True) As Boolean
            CSXml.cXml docXml = null;
            docXml = new CSXml.cXml();

            docXml.init(commDialog);
            docXml.setFilter(C_FILEDATAEX);
            docXml.setName(getFileName(m_name) + "-data.csd");
            docXml.setPath(m_path);

            if (withDialog)
            {
                if (!docXml.newXmlWithDialog())
                {
                    return false;
                }
            }
            else
            {
                if (!docXml.newXml())
                {
                    return false;
                }
            }

            Application.DoEvents();

            cMouseWait mouse = new cMouseWait();
            String dataName = "";
            String dataPath = "";

            dataName = docXml.getName();
            dataPath = docXml.getPath();

            CSXml.cXmlProperty xProperty = null;
            xProperty = new CSXml.cXmlProperty();

            xProperty.setName("RptName");
            xProperty.setValue(eTypes.eText, dataName);
            docXml.addProperty(xProperty);

            // Configuracion de paginas
            XmlNode nodeObj = null;
            XmlNode nodeObjAux = null;

            // Paginas
            cReportPage page = null;

            xProperty.setName(C_NODERPTPAGES);
            xProperty.setValue(eTypes.eText, "");
            nodeObj = docXml.addNode(xProperty);

            for (int _i = 0; _i < m_pages.count(); _i++)
            {
                page = m_pages.item(_i);
                page.save(docXml, nodeObj);
                if (!saveDataForWeb(page, dataName, dataPath))
                {
                    return false;
                }
            }

            if (!docXml.save())
            {
                return false;
            }

            if (!docXml.openXml())
            {
                return false;
            }

            if (!nLoadData(docXml))
            {
                return false;
            }

            mouse.Dispose();

            return true;
        }

        private bool saveDataForWeb(cReportPage page, String dataName, String dataPath)
        { // TODO: Use of ByRef founded Private Function SaveDataForWeb(ByRef Page As cReportPage, ByVal DataName As String, ByVal DataPath As String) As Boolean
            CSXml.cXml docXml = null;
            docXml = new CSXml.cXml();

            docXml.init(null);
            docXml.setFilter("xml");
            docXml.setName(getFileName(dataName) + "-1.xml");
            docXml.setPath(dataPath);

            if (!docXml.newXml())
            {
                return false;
            }

            dataName = docXml.getName();

            CSXml.cXmlProperty xProperty = null;
            xProperty = new CSXml.cXmlProperty();

            xProperty.setName("Page_" + page.getPageNumber().ToString());
            xProperty.setValue(eTypes.eText, dataName);
            docXml.addProperty(xProperty);

            XmlNode nodeObj = null;

            xProperty.setName("Page");
            xProperty.setValue(eTypes.eText, "");
            nodeObj = docXml.addNode(xProperty);

            page.saveForWeb(docXml, nodeObj);

            return docXml.save();
        }

        internal object getValueFromRs(int colIndex)
        {
            return m_rows.Rows[m_vRowsIndex[m_iRow2]][colIndex];
        }

        internal String getValueString(String controlName)
        {
            return (String)getValue(controlName, false);
        }

        internal object getValue(String controlName)
        {
            return getValue(controlName, false);
        }

        internal object getValue(String controlName, bool notFormat)
        {
            cReportControl ctrl = null;
            bool found = false;
            int iRow = 0;

            if (m_iRowFormula > m_vRowsIndex.Length)
            {
                iRow = m_vRowsIndex.Length;
            }
            else
            {
                iRow = m_iRowFormula;
            }

            for (int _i = 0; _i < m_controls.count(); _i++)
            {
                ctrl = m_controls.item(_i);
                if (ctrl.getName().ToUpper() == controlName.ToUpper())
                {
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                throw new ReportException(csRptErrors.CONTROL_NOT_FOUND,
                                          C_MODULE,
                                          cReportError.errGetDescript(csRptErrors.CONTROL_NOT_FOUND, controlName)
                                          );
            }

            switch (ctrl.getControlType())
            {
                case csRptControlType.CSRPTCTFIELD:

                    // this indexes 
                    // current datasource
                    //
                    int indexRows = 0;
                    // current row in the active datasource
                    //
                    int indexRow = 0;
                    int indexField = 0;

                    // the datasource index have an offset of 1000 between each other
                    //
                    indexRows = (int)(ctrl.getField().getIndex() / 1000);
                    indexField = ctrl.getField().getIndex() - (indexRows * 1000);

                    if (indexRows == 0)
                    {
                        indexRow = m_vRowsIndex[iRow];
                    }
                    else
                    {
                        indexRow = m_vRowsIndexAux[indexRows];
                    }

                    if (m_collRows[indexRows] != null)
                    {
                        // it looks ugly, dont think you?
                        //
                        // maybe this help a litle:
                        //
                        //    m_vCollRows(IndexRows)    a matrix with the data 
                        //                              contained in the datasource
                        //                              referd by this control
                        //
                        //    (IndexField, IndexRow)    a cell in this matrix
                        //
                        object value = m_collRows[indexRows].Rows[indexRow][indexField];
                        if (ctrl.getLabel().getAspect().getFormat() != "" && notFormat == false)
                        {
                            return cReportGlobals.format(
                                        cReportGlobals.valVariant(value),
                                        ctrl.getLabel().getAspect().getFormat());

                            // this is the same
                        }
                        else
                        {
                            return cReportGlobals.valVariant(value);
                        }
                    }
                    else
                    {
                        return null;
                    }

                case csRptControlType.CSRPTCTLABEL:
                case csRptControlType.CSRPTCTIMAGE:
                    if (ctrl.getHasFormulaValue())
                    {
                        if (ctrl.getFormulaValue().getHaveToEval())
                        {
                            object value = m_compiler.resultFunction(ctrl.getFormulaValue());
                            if (ctrl.getLabel().getAspect().getFormat() != "" && notFormat == false)
                            {
                                return cReportGlobals.format(value, ctrl.getLabel().getAspect().getFormat());
                            }
                            else
                            {
                                return value;
                            }
                        }
                        else
                        {
                            object value = ctrl.getFormulaValue().getLastResult();
                            if (ctrl.getLabel().getAspect().getFormat() != "" && notFormat == false)
                            {
                                return cReportGlobals.format(value, ctrl.getLabel().getAspect().getFormat());
                            }
                            else
                            {
                                return value;
                            }
                        }
                    }
                    else
                    {
                        return ctrl.getLabel().getText();
                    }
                default:
                    return null;
            }
        }

        private bool initControls(List<object[]> recordsets)
        { // TODO: Use of ByRef founded Private Function InitControls(ByRef Recordsets As Collection) As Boolean
            cReportControl ctrl = null;
            cReportChartSerie serie = null;
            int idx = 0;

            for (int _i = 0; _i < m_controls.count(); _i++)
            {
                ctrl = m_controls.item(_i);
                if (ctrl.getControlType() == csRptControlType.CSRPTCTFIELD
                    || ctrl.getControlType() == csRptControlType.CSRPTCTDBIMAGE)
                {
                    idx = ctrl.getField().getIndex();
                    if (!pInitCtrls(ctrl, idx, recordsets, ctrl.getField().getName()))
                    {
                        return false;
                    }
                    ctrl.getField().setIndex(idx);
                }
                else if (ctrl.getControlType() == csRptControlType.CSRPTCTCHART)
                {
                    if (ctrl.getChart().getGroupFieldName() != "")
                    {
                        idx = -1;
                        pInitCtrls(ctrl, idx, recordsets, ctrl.getChart().getGroupFieldName());
                        ctrl.getChart().setGroupFieldIndex(idx);
                    }
                    else
                    {
                        ctrl.getChart().setGroupFieldIndex(-1);
                    }

                    for (int _j = 0; _j < ctrl.getChart().getSeries().count(); _j++)
                    {
                        serie = ctrl.getChart().getSeries().item(_j);
                        idx = serie.getValueIndex();
                        if (!pInitCtrls(ctrl, idx, recordsets, serie.getValueFieldName()))
                        {
                            return false;
                        }
                        serie.setValueIndex(idx);
                        idx = serie.getLabelIndex();
                        if (!pInitCtrls(ctrl, idx, recordsets, serie.getLabelFieldName()))
                        {
                            return false;
                        }
                        serie.setLabelIndex(idx);
                    }
                    ctrl.getChart().setChartCreated(false);
                }
            }
            return true;
        }

        private bool pInitCtrls(cReportControl ctrl, int idx, List<object[]> recordsets, String fieldName)
        { // TODO: Use of ByRef founded Private Function pInitCtrls(ByRef ctrl As cReportControl, ByRef Idx As Long, ByRef Recordsets As Collection, ByVal FieldName As String) As Boolean
            bool found = false;
            int j = 0;
            // index of the group which contains the control
            //
            int k = 0;
            bool bIsDBImage = false;
            object[] varRs = null;
            DataTable rs = null;
            String dataSource = "";

            bIsDBImage = false;

            found = false;
            k = 0;
            for (int _i = 0; _i < recordsets.Count; _i++)
            {
                varRs = recordsets[_i];
                dataSource = pGetDataSource(fieldName);
                String rsDataSource = (String)varRs[1];
                if (rsDataSource.ToUpper() == dataSource.ToUpper() || dataSource == "")
                {
                    rs = (DataTable)varRs[0];

                    for (j = 0; j < rs.Columns.Count; j++)
                    {
                        if (rs.Columns[j].ColumnName.ToUpper() == cReportGlobals.getRealName(fieldName).ToUpper())
                        {
                            // TODO: we need to check what is the value of rs.Columns[j].DataType
                            //       when the column contains a binary field (tipicaly used for images)
                            //
                            System.TypeCode typeCode = System.Type.GetTypeCode(rs.Columns[j].DataType);
                            bIsDBImage = typeCode == System.TypeCode.Object;
                            found = true;
                            break;
                        }
                    }
                }
                if (found)
                {
                    break;
                }
                k = k + 1000;
            }

            if (found)
            {
                idx = j + k;
                if (bIsDBImage)
                {
                    ctrl.setControlType(csRptControlType.CSRPTCTDBIMAGE);
                }
            }
            else
            {
                throw new ReportException(csRptErrors.FIELD_NOT_FOUND,
                                            C_MODULE,
                                            cReportError.errGetDescript(csRptErrors.FIELD_NOT_FOUND, ctrl.getName(), fieldName)
                                            );
            }
            return true;
        }

        private String pGetDataSource(String name)
        {
            int n = 0;
            n = name.IndexOf("}.", 1);
            if (n == 0) 
            { 
                return ""; 
            }
            n = n - 2;
            return name.Substring(2, n);
        }

        private void pInitImages()
        {
            pDestroyImages();
            m_images = new Dictionary<string, Image>();
        }
        
        private void pDestroyImages() {
            if (m_images != null)
            {                
                foreach (KeyValuePair<String, Image> item in m_images)
                {
                    item.Value.Dispose();
                }
                m_images = null;
            }
        }

        private Image pGetChartImage(int indexRows, int indexField, int indexRow, cReportControl ctrl)
        { // TODO: Use of ByRef founded Private Function pGetChartImage(ByVal IndexRows As Long, ByVal IndexField As Long, ByVal IndexRow As Long, ByRef ctrl As cReportControl) As Long
            if (ctrl.getChart().getChartCreated())
            {
                return ctrl.getChart().getImage();
            }
            else
            {
                if (ctrl.getChart().make(m_collRows[indexRows].Rows, ctrl.getLabel().getAspect().getFormat(), false, ""))
                {
                    return ctrl.getChart().getImage();
                }
                else
                {
                    return null;
                }
            }
        }
        
        // the params are used to create a key 
        // to use as an id for every image contained 
        // in the report
        //
        private Image pGetImage(int indexRows, int indexField, int indexRow)
        {
            String key = "";
            Image image = null;
            String fileInTMP = "";

            key = "k" + indexRows.ToString() + indexField.ToString() + indexRow.ToString();
            try
            {
                image = m_images[key];
            }
            catch
            {
                // we are optimistic. if I don't get a picture
                // we return null but don't complaint
                //
                byte[] bytes = null;

                // it looks ugly, dont think you?
                //
                // maybe this help a litle:
                //
                //    m_vCollRows(IndexRows)    a matrix with the data 
                //                              contained in the datasource
                //                              referd by this control
                //
                //    (IndexField, IndexRow)    a cell in this matrix
                //
                object value = m_collRows[indexRows].Rows[indexRow][indexField];
                bytes = (byte[])value;

                fileInTMP = pGetFileImageInTMP(bytes);

                if (fileInTMP != "")
                {
                    try
                    {
                        image = Image.FromFile(fileInTMP);
                        m_images.Add(key, image);
                    }
                    catch
                    {
                        // we don't care
                    }
                }
            }
            return image;
        }

        private String pGetFileImageInTMP(byte[] bytes)
        {
            String fileName = fileName = "~csrptImage";
            fileName = cUtil.getValidPath(System.Environment.GetEnvironmentVariable("TEMP")) + fileName;

            CSKernelFile.cFileEx fileEx = null;
            fileEx = new CSKernelFile.cFileEx();
            if (!fileEx.fileDelete(fileName)) { return ""; }

            CSKernelFile.cFile file = new CSKernelFile.cFile();
            if (!file.open(fileName, eFileMode.eBinaryWrite, true, true, eFileAccess.eLockWrite, false, false))
            {
                return "";
            }

            if (!file.binaryWrite(bytes))
            {
                return "";
            }

            file.close();

            return fileName;
        }

        public void setLaunchInfo(cReportLaunchInfo oLaunchInfo)
        {
            m_launchInfo = new cReportLaunchInfo();
            // copy from oLaunchInfo to m_LaunchInfo
            //
            m_launchInfo.setAction(oLaunchInfo.getAction());
            m_launchInfo.setStrConnect(oLaunchInfo.getStrConnect());
            m_launchInfo.setCopies(oLaunchInfo.getCopies());

            m_launchInfo.setObjPaint(oLaunchInfo.getObjPaint());
            m_launchInfo.setDataSource(oLaunchInfo.getDataSource());

            m_launchInfo.setFile(oLaunchInfo.getFile());
            m_launchInfo.setFileFormat(oLaunchInfo.getFileFormat());
            m_launchInfo.setInternalPreview(oLaunchInfo.getInternalPreview());
            m_launchInfo.setShowPrintersDialog(oLaunchInfo.getShowPrintersDialog());
            m_launchInfo.setSilent(oLaunchInfo.getSilent());
            m_launchInfo.setSqlstmt(oLaunchInfo.getSqlstmt());
            m_launchInfo.setPrinter(oLaunchInfo.getPrinter());

            // if the printer is not defined
            //
            if (m_launchInfo.getPrinter() == null)
            {
                // we use the default printer of the OS
                //
                m_launchInfo.setPrinter(cPrintAPI.getcPrinterFromDefaultPrinter());
            }

            // if we have a reference to a printer
            //
            if (m_launchInfo.getPrinter() != null)
            {
                // TODO: check the values of paperType after calling
                //       getcPrinterFromDefaultPrinter() because
                //       the constants used by the OS could be differents
                //       from the used by CSReport. in the original vb6 version
                //       it was the case
                //

                // finaly we copy into m_PaperInfo the definicion found in LaunchInfo.
                //
                // when the report is called without define a printer
                // whe assign a defult printer and asign m_PaperInfo 
                // to m_LaunchInfo.Printer.PaperInfo, so sometimes we
                // don't need to do that
                //
                if (!object.ReferenceEquals(m_paperInfo, m_launchInfo.getPrinter().getPaperInfo()))
                {
                    m_paperInfo.setHeight(m_launchInfo.getPrinter().getPaperInfo().getHeight());
                    m_paperInfo.setWidth(m_launchInfo.getPrinter().getPaperInfo().getWidth());
                }
            }
        }

        public double getGroupTotal(int colIndex, int indexGroup)
        {
            int iRow = 0;
            double rtn = 0;
            int i = 0;

            if (indexGroup == -1)
            {
                for (iRow = 0; iRow <= m_recordCount; iRow++)
                {
                    rtn = rtn + (double)cReportGlobals.valVariant(m_rows.Rows[m_vRowsIndex[iRow]][colIndex]);
                }
            }
            else
            {
                if (m_vGroups[indexGroup].grandTotalGroup)
                {
                    for (iRow = 0; iRow <= m_recordCount; iRow++)
                    {
                        rtn = rtn + (double)cReportGlobals.valVariant(m_rows.Rows[m_vRowsIndex[iRow]][colIndex]);
                    }
                }
                else
                {
                    for (iRow = m_iRow; iRow <= m_recordCount; iRow++)
                    {
                        for (i = 1; i <= indexGroup; i++)
                        {
                            switch (m_vGroups[i].comparisonType)
                            {
                                case csRptGrpComparisonType.CSRPTGRPTEXT:

                                    if (m_vGroups[i].value == null)
                                    {
                                        return rtn;
                                    }
                                    else 
                                    {
                                        object value = m_rows.Rows[m_vRowsIndex[iRow]][m_vGroups[i].indexField];
                                        String text = (String)cReportGlobals.valVariant(value);
                                        if ((String)m_vGroups[i].value != text.ToLower())
                                        {
                                            return rtn;
                                        }
                                    }

                                    if (i == indexGroup)
                                    {
                                        object value = m_rows.Rows[colIndex][m_vRowsIndex[iRow]];
                                        rtn = rtn + (double)cReportGlobals.valVariant(value);
                                    }
                                    break;

                                case csRptGrpComparisonType.CSRPTGRPNUMBER:

                                    if (m_vGroups[i].value == null)
                                    {
                                        return rtn;
                                    }
                                    else
                                    {
                                        object value = m_rows.Rows[m_vRowsIndex[iRow]][m_vGroups[i].indexField];
                                        double number = cReportGlobals.val(cReportGlobals.valVariant(value));
                                        if ((double)m_vGroups[i].value != number)
                                        {
                                            return rtn;
                                        }
                                    }

                                    if (i == indexGroup)
                                    {
                                        object value = m_rows.Rows[m_vRowsIndex[iRow]][colIndex];
                                        rtn = rtn + (double)cReportGlobals.valVariant(value);
                                    }
                                    break;

                                case csRptGrpComparisonType.CSRPTGRPDATE:

                                    if (m_vGroups[i].value == null)
                                    {
                                        return rtn;
                                    }
                                    else
                                    {
                                        object value = m_rows.Rows[m_vRowsIndex[iRow]][m_vGroups[i].indexField];
                                        DateTime date = cReportGlobals.dateValue(cReportGlobals.valVariant(value));
                                        if ((DateTime)m_vGroups[i].value != date)
                                        {
                                            return rtn;
                                        }
                                    }

                                    if (i == indexGroup)
                                    {
                                        object value = m_rows.Rows[m_vRowsIndex[iRow]][colIndex];
                                        rtn = rtn + (double)cReportGlobals.valVariant(value);
                                    }
                                    break;
                            }
                        }
                    }
                }
            }
            return rtn;
        }

        public double getGroupMax(int colIndex, int indexGroup)
        {
            int iRow = 0;
            double rtn = 0;
            int i = 0;

            rtn = (double)cReportGlobals.valVariant(m_rows.Rows[m_vRowsIndex[iRow]][colIndex]);

            if (indexGroup == -1)
            {
                for (iRow = 0; iRow <= m_recordCount; iRow++)
                {
                    double value = (double)cReportGlobals.valVariant(m_rows.Rows[m_vRowsIndex[iRow]][colIndex]);
                    if (rtn < value)
                    {
                        rtn = value;
                    }
                }
            }
            else
            {
                if (m_vGroups[indexGroup].grandTotalGroup)
                {
                    for (iRow = 0; iRow <= m_recordCount; iRow++)
                    {
                        double value = (double)cReportGlobals.valVariant(m_rows.Rows[m_vRowsIndex[iRow]][colIndex]);
                        if (rtn < value)
                        {
                            rtn = value;
                        }
                    }
                }
                else
                {
                    for (iRow = m_iRow; iRow <= m_recordCount; iRow++)
                    {
                        for (i = 1; i <= indexGroup; i++)
                        {
                            switch (m_vGroups[i].comparisonType)
                            {
                                case csRptGrpComparisonType.CSRPTGRPTEXT:

                                    if (m_vGroups[i].value == null)
                                    {
                                        return rtn;
                                    }
                                    else
                                    {
                                        object value = m_rows.Rows[m_vRowsIndex[iRow]][m_vGroups[i].indexField];
                                        String text = (String)cReportGlobals.valVariant(value);
                                        if ((String)m_vGroups[i].value != text.ToLower())
                                        {
                                            return rtn;
                                        }
                                    }
                                    if (i == indexGroup)
                                    {
                                        double value = (double)cReportGlobals.valVariant(m_rows.Rows[m_vRowsIndex[iRow]][colIndex]);
                                        if (rtn < value)
                                        {
                                            rtn = value;
                                        }
                                    }
                                    break;

                                case csRptGrpComparisonType.CSRPTGRPNUMBER:

                                    if (m_vGroups[i].value == null)
                                    {
                                        return rtn;
                                    }
                                    else
                                    {
                                        object value = m_rows.Rows[m_vRowsIndex[iRow]][m_vGroups[i].indexField];
                                        double number = cReportGlobals.val(cReportGlobals.valVariant(value));
                                        if ((double)m_vGroups[i].value != number)
                                        {
                                            return rtn;
                                        }
                                    }
                                    if (i == indexGroup)
                                    {
                                        double value = (double)cReportGlobals.valVariant(m_rows.Rows[m_vRowsIndex[iRow]][colIndex]);
                                        if (rtn < value)
                                        {
                                            rtn = value;
                                        }
                                    }
                                    break;

                                case csRptGrpComparisonType.CSRPTGRPDATE:

                                    if (m_vGroups[i].value == null)
                                    {
                                        return rtn;
                                    }
                                    else
                                    {
                                        object value = m_rows.Rows[m_vRowsIndex[iRow]][m_vGroups[i].indexField];
                                        DateTime date = cReportGlobals.dateValue(cReportGlobals.valVariant(value));
                                        if ((DateTime)m_vGroups[i].value != date)
                                        {
                                            return rtn;
                                        }
                                    }
                                    if (i == indexGroup)
                                    {
                                        double value = (double)cReportGlobals.valVariant(m_rows.Rows[m_vRowsIndex[iRow]][colIndex]);
                                        if (rtn < value)
                                        {
                                            rtn = value;
                                        }
                                    }
                                    break;
                            }
                        }
                    }
                }
            }
            return rtn;
        }

        public double getGroupMin(int colIndex, int indexGroup)
        {
            int iRow = 0;
            double rtn = 0;
            int i = 0;

            rtn = (double)cReportGlobals.valVariant(m_rows.Rows[m_vRowsIndex[iRow]][colIndex]);

            if (indexGroup == -1)
            {
                for (iRow = 0; iRow <= m_recordCount; iRow++)
                {
                    double value = (double)cReportGlobals.valVariant(m_rows.Rows[m_vRowsIndex[iRow]][colIndex]);
                    if (rtn > value)
                    {
                        rtn = value;
                    }
                }
            }
            else
            {
                if (m_vGroups[indexGroup].grandTotalGroup)
                {
                    for (iRow = 0; iRow <= m_recordCount; iRow++)
                    {
                        double value = (double)cReportGlobals.valVariant(m_rows.Rows[m_vRowsIndex[iRow]][colIndex]);
                        if (rtn > value)
                        {
                            rtn = value;
                        }
                    }
                }
                else
                {
                    for (iRow = m_iRow; iRow <= m_recordCount; iRow++)
                    {
                        for (i = 1; i <= indexGroup; i++)
                        {
                            switch (m_vGroups[i].comparisonType)
                            {
                                case csRptGrpComparisonType.CSRPTGRPTEXT:

                                    if (m_vGroups[i].value == null)
                                    {
                                        return rtn;
                                    }
                                    else
                                    {
                                        object value = m_rows.Rows[m_vRowsIndex[iRow]][m_vGroups[i].indexField];
                                        String text = (String)cReportGlobals.valVariant(value);
                                        if ((String)m_vGroups[i].value != text.ToLower())
                                        {
                                            return rtn;
                                        }
                                    }
                                    if (i == indexGroup)
                                    {
                                        double value = (double)cReportGlobals.valVariant(m_rows.Rows[m_vRowsIndex[iRow]][colIndex]);
                                        if (rtn > value)
                                        {
                                            rtn = value;
                                        }
                                    }
                                    break;

                                case csRptGrpComparisonType.CSRPTGRPNUMBER:

                                    if (m_vGroups[i].value == null)
                                    {
                                        return rtn;
                                    }
                                    else
                                    {
                                        object value = m_rows.Rows[m_vRowsIndex[iRow]][m_vGroups[i].indexField];
                                        double number = cReportGlobals.val(cReportGlobals.valVariant(value));
                                        if ((double)m_vGroups[i].value != number)
                                        {
                                            return rtn;
                                        }
                                    }
                                    if (i == indexGroup)
                                    {
                                        double value = (double)cReportGlobals.valVariant(m_rows.Rows[m_vRowsIndex[iRow]][colIndex]);
                                        if (rtn > value)
                                        {
                                            rtn = value;
                                        }
                                    }
                                    break;

                                case csRptGrpComparisonType.CSRPTGRPDATE:

                                    if (m_vGroups[i].value == null)
                                    {
                                        return rtn;
                                    }
                                    else
                                    {
                                        object value = m_rows.Rows[m_vRowsIndex[iRow]][m_vGroups[i].indexField];
                                        DateTime date = cReportGlobals.dateValue(cReportGlobals.valVariant(value));
                                        if ((DateTime)m_vGroups[i].value != date)
                                        {
                                            return rtn;
                                        }
                                    }
                                    if (i == indexGroup)
                                    {
                                        double value = (double)cReportGlobals.valVariant(m_rows.Rows[m_vRowsIndex[iRow]][colIndex]);
                                        if (rtn > value)
                                        {
                                            rtn = value;
                                        }
                                    }
                                    break;
                            }
                        }
                    }
                }
            }
            return rtn;
        }

        public double getGroupAverage(int colIndex, int indexGroup)
        {
            int iRow = 0;
            double rtn = 0;
            int i = 0;
            int count = 0;

            if (indexGroup == -1)
            {
                for (iRow = 0; iRow <= m_recordCount; iRow++)
                {
                    rtn = rtn + (double)cReportGlobals.valVariant(m_rows.Rows[m_vRowsIndex[iRow]][colIndex]);
                    count = count + 1;
                }
            }
            else
            {
                if (m_vGroups[indexGroup].grandTotalGroup)
                {

                    for (iRow = 0; iRow <= m_recordCount; iRow++)
                    {
                        rtn = rtn + (double)cReportGlobals.valVariant(m_rows.Rows[m_vRowsIndex[iRow]][colIndex]);
                        count = count + 1;
                    }

                }
                else
                {
                    for (iRow = m_iRow; iRow <= m_recordCount; iRow++)
                    {
                        for (i = 1; i <= indexGroup; i++)
                        {
                            switch (m_vGroups[i].comparisonType)
                            {
                                case csRptGrpComparisonType.CSRPTGRPTEXT:

                                    if (m_vGroups[i].value == null)
                                    {
                                        return rtn;
                                    }
                                    else
                                    {
                                        object value = m_rows.Rows[m_vRowsIndex[iRow]][m_vGroups[i].indexField];
                                        String text = (String)cReportGlobals.valVariant(value);
                                        if (m_vGroups[i].value != text.ToLower())
                                        {
                                            return rtn;
                                        }
                                    }
                                    if (i == indexGroup)
                                    {
                                        rtn = rtn + (double)cReportGlobals.valVariant(m_rows.Rows[m_vRowsIndex[iRow]][colIndex]);
                                        count = count + 1;
                                    }
                                    break;

                                case csRptGrpComparisonType.CSRPTGRPNUMBER:

                                    if (m_vGroups[i].value == null)
                                    {
                                        return rtn;
                                    }
                                    else
                                    {
                                        object value = m_rows.Rows[m_vRowsIndex[iRow]][m_vGroups[i].indexField];
                                        double number = cReportGlobals.val(cReportGlobals.valVariant(value));
                                        if ((double)m_vGroups[i].value != number)
                                        {
                                            return rtn;
                                        }
                                    }
                                    if (i == indexGroup)
                                    {
                                        rtn = rtn + (double)cReportGlobals.valVariant(m_rows.Rows[m_vRowsIndex[iRow]][colIndex]);
                                        count = count + 1;
                                    }
                                    break;

                                case csRptGrpComparisonType.CSRPTGRPDATE:

                                    if (m_vGroups[i].value == null)
                                    {
                                        return rtn;
                                    }
                                    else
                                    {
                                        object value = m_rows.Rows[m_vRowsIndex[iRow]][m_vGroups[i].indexField];
                                        DateTime date = cReportGlobals.dateValue(cReportGlobals.valVariant(value));
                                        if ((DateTime)m_vGroups[i].value != date)
                                        {
                                            return rtn;
                                        }
                                    }
                                    if (i == indexGroup)
                                    {
                                        rtn = rtn + (double)cReportGlobals.valVariant(m_rows.Rows[m_vRowsIndex[iRow]][colIndex]);
                                        count = count + 1;
                                    }
                                    break;
                            }
                        }
                    }
                }
            }
            return cUtil.divideByZero(rtn, count);
        }

        public object getGroupLineNumber(int indexGroup)
        {
            if (indexGroup == -1)
            {
                return m_lineNumber;
            }
            else
            {
                return m_vGroups[indexGroup].lineNumber;
            }
        }

        public double getGroupCount(int colIndex, int indexGroup)
        {
            int iRow = 0;
            double rtn = 0;
            int i = 0;

            if (indexGroup == -1)
            {
                rtn = m_recordCount;
            }
            else
            {
                if (m_vGroups[indexGroup].grandTotalGroup)
                {
                    rtn = m_recordCount;
                }
                else
                {
                    for (iRow = m_iRow; iRow <= m_recordCount; iRow++)
                    {
                        for (i = 1; i <= indexGroup; i++)
                        {
                            switch (m_vGroups[i].comparisonType)
                            {
                                case csRptGrpComparisonType.CSRPTGRPTEXT:

                                    if (m_vGroups[i].value == null)
                                    {
                                        return rtn;
                                    }
                                    else
                                    {
                                        object value = m_rows.Rows[m_vRowsIndex[iRow]][m_vGroups[i].indexField];
                                        String text = (String)cReportGlobals.valVariant(value);
                                        if ((String)m_vGroups[i].value != text.ToLower())
                                        {
                                            return rtn;
                                        }
                                    }
                                    if (i == indexGroup)
                                    {
                                        rtn = rtn + 1;
                                    }
                                    break;

                                case csRptGrpComparisonType.CSRPTGRPNUMBER:

                                    if (m_vGroups[i].value == null)
                                    {
                                        return rtn;
                                    }
                                    else
                                    {
                                        object value = m_rows.Rows[m_vRowsIndex[iRow]][m_vGroups[i].indexField];
                                        double number = cReportGlobals.val(cReportGlobals.valVariant(value));
                                        if ((double)m_vGroups[i].value != number)
                                        {
                                            return rtn;
                                        }
                                    }
                                    if (i == indexGroup)
                                    {
                                        rtn = rtn + 1;
                                    }
                                    break;

                                case csRptGrpComparisonType.CSRPTGRPDATE:

                                    if (m_vGroups[i].value == null)
                                    {
                                        return rtn;
                                    }
                                    else
                                    {
                                        object value = m_rows.Rows[m_vRowsIndex[iRow]][m_vGroups[i].indexField];
                                        DateTime date = cReportGlobals.dateValue(cReportGlobals.valVariant(value));
                                        if ((DateTime)m_vGroups[i].value != date)
                                        {
                                            return rtn;
                                        }
                                    }
                                    if (i == indexGroup)
                                    {
                                        rtn = rtn + 1;
                                    }
                                    break;
                            }
                        }
                    }
                }
            }
            return rtn;
        }

        private void addGroup(int i, int j, object value)
        {
            // set the upper bound of the last group
            //
            m_vGroups[i + 1].groups[m_vGroups[i + 1].groups.Length - 1].last = j - 1;
            // add a new group
            //
            redimPreserve(ref m_vGroups[i + 1].groups, m_vGroups[i + 1].groups.Length + 1);
            m_vGroups[i + 1].groups[m_vGroups[i + 1].groups.Length - 1].first = j;
            m_vGroups[i + 1].value = value;       
        }

        private bool initGroups(DataTable rs, String mainDataSource)
        { // TODO: Use of ByRef founded Private Function InitGroups(ByRef rs As ADODB.Recordset, ByVal MainDataSource As String) As Boolean
            m_groupCount = m_groups.count();
            m_firstGroup = true;

            if (m_groupCount == 0 || m_rows == null)
            {
                m_vGroups = null;
                return true;
            }
            else
            {
                m_vGroups = new T_Groups[m_groupCount];
            }

            if (!OnProgress("Ordenando el reporte", 0, 0, 0))
            {
                return false;
            }

            int k = 0;
            int i = 0;
            int j = 0;
            bool found = false;
            String fieldName = "";
            String dataSource = "";

            // we need to check every group is in the main recordset
            //
            for (i = 0; i < m_groupCount; i++)
            {
                m_vGroups[i].value = null;
                found = false;
                fieldName = m_groups.item(i).getFieldName();
                dataSource = pGetDataSource(fieldName).ToUpper();
                fieldName = cReportGlobals.getRealName(fieldName).ToUpper();

                // the column must be in the main recordset
                //
                if (mainDataSource.ToUpper() != dataSource && dataSource != "")
                {
                    cReportGroup w_item = m_groups.item(i);
                    throw new ReportException(csRptErrors.GROUP_NOT_FOUND_IN_MAIN_RS,
                                                C_MODULE,
                                                cReportError.errGetDescript(
                                                                csRptErrors.GROUP_NOT_FOUND,
                                                                w_item.getName(),
                                                                w_item.getFieldName())
                                                );
                }
                m_vGroups[i].grandTotalGroup = m_groups.item(i).getGrandTotalGroup();

                if (!m_vGroups[i].grandTotalGroup)
                {
                    for (j = 0; j < rs.Columns.Count; j++)
                    {
                        if (rs.Columns[j].ColumnName.ToUpper() == fieldName)
                        {
                            found = true;
                            break;
                        }
                    }
                    if (found)
                    {
                        m_vGroups[i].indexField = j;
                    }
                    else
                    {
                        cReportGroup w_item = m_groups.item(i);
                        throw new ReportException(csRptErrors.GROUP_NOT_FOUND_IN_MAIN_RS,
                                                    C_MODULE,
                                                    cReportError.errGetDescript(
                                                                    csRptErrors.GROUP_NOT_FOUND,
                                                                    w_item.getName(),
                                                                    w_item.getFieldName())
                                                    );
                    }
                }
                m_vGroups[i].comparisonType = m_groups.item(i).getComparisonType();
                m_vGroups[i].oderType = m_groups.item(i).getOderType();

                m_vGroups[i].groups = null;
            }

            int recordCount = 0;
            double percent = 0;
            int q = 0;

            m_vGroups[1].groups = new T_Group[0];
            recordCount = m_vRowsIndex.Length;
            m_vGroups[0].groups[0].first = 0;
            m_vGroups[0].groups[0].last = recordCount;
            recordCount = m_groupCount * recordCount;

            // we need to sort the data
            //
            for (i = 0; i < m_groupCount; i++)
            {
                for (j = 0; j < m_vGroups[i].groups.Length; j++)
                {
                    if (!m_vGroups[i].grandTotalGroup)
                    {
                        if (m_vGroups[i].oderType == csRptGrpOrderType.CSRPTGRPASC)
                        {
                            switch (m_vGroups[i].comparisonType)
                            {
                                case csRptGrpComparisonType.CSRPTGRPTEXT:
                                    if (!orderTextAsc(m_vGroups[i].groups[j].first,
                                                        m_vGroups[i].groups[j].last,
                                                        m_vGroups[i].indexField))
                                    {
                                        return false;
                                    }
                                    break;

                                case csRptGrpComparisonType.CSRPTGRPNUMBER:
                                    if (!orderNumberAsc(m_vGroups[i].groups[j].first,
                                                        m_vGroups[i].groups[j].last,
                                                        m_vGroups[i].indexField))
                                    {
                                        return false;
                                    }
                                    break;

                                case csRptGrpComparisonType.CSRPTGRPDATE:
                                    if (!orderDateAsc(m_vGroups[i].groups[j].first,
                                                        m_vGroups[i].groups[j].last,
                                                        m_vGroups[i].indexField))
                                    {
                                        return false;
                                    }
                                    break;
                            }
                        }
                        else
                        {
                            switch (m_vGroups[i].comparisonType)
                            {
                                case csRptGrpComparisonType.CSRPTGRPTEXT:
                                    if (!orderTextDesc(m_vGroups[i].groups[j].first,
                                                        m_vGroups[i].groups[j].last,
                                                        m_vGroups[i].indexField))
                                    {
                                        return false;
                                    }
                                    break;

                                case csRptGrpComparisonType.CSRPTGRPNUMBER:
                                    if (!orderNumberDesc(m_vGroups[i].groups[j].first,
                                                            m_vGroups[i].groups[j].last,
                                                            m_vGroups[i].indexField))
                                    {
                                        return false;
                                    }
                                    break;

                                case csRptGrpComparisonType.CSRPTGRPDATE:
                                    if (!orderDateDesc(m_vGroups[i].groups[j].first,
                                                        m_vGroups[i].groups[j].last,
                                                        m_vGroups[i].indexField))
                                    {
                                        return false;
                                    }
                                    break;
                            }
                        }
                    }
                }

                // after sorting we need to prepare the next group
                //
                if (i < m_groupCount)
                {
                    for (k = 0; k < m_vGroups[i].groups.Length; k++)
                    {
                        // if it is a total group the next group
                        // is from the first to the last row in 
                        // the main recordset
                        // Si es un grupo de totales el proximo grupo
                        // first (position: 0)
                        // last  (position: m_vGroups[0].groups[0].last)
                        //
                        if (m_vGroups[i].grandTotalGroup)
                        {
                            int t = i + 1;
                            int r = m_vGroups[t].groups.Length - 1;
                            m_vGroups[t].groups[r].last = 0;

                            // add a group item
                            //
                            redimPreserve(ref m_vGroups[t].groups, r + 2);
                            r = m_vGroups[t].groups.Length - 1;

                            // set the values of the new group item
                            //
                            m_vGroups[t].groups[t].first = 0;
                            m_vGroups[t].groups[t].last = m_vGroups[0].groups[0].last;
                            m_vGroups[t].value = null;
                        }
                        else
                        {
                            for (j = m_vGroups[i].groups[k].first; j <= m_vGroups[i].groups[k].last; j++)
                            {
                                q = q + 1;
                                if (!OnProgress("", 0, q, recordCount))
                                {
                                    return false;
                                }

                                object value = cReportGlobals.valVariant(m_rows.Rows[m_vRowsIndex[j]][m_vGroups[i].indexField]);
                                if (m_vGroups[i + 1].value == null)
                                {
                                    addGroup(i, j, value);
                                }
                                else
                                {
                                    switch (m_vGroups[i].comparisonType)
                                    {
                                        case csRptGrpComparisonType.CSRPTGRPTEXT:

                                            String text1 = (String)m_vGroups[i + 1].value;
                                            String text2 = (String)value;
                                            if (text1.ToLower() != text2.ToLower())
                                            {
                                                addGroup(i, j, value);
                                            }
                                            break;

                                        case csRptGrpComparisonType.CSRPTGRPNUMBER:

                                            double number1 = cReportGlobals.val(m_vGroups[i + 1].value);
                                            double number2 = cReportGlobals.val(value);
                                            if (number1 != number2)
                                            {
                                                addGroup(i, j, value);
                                            }
                                            break;

                                        case csRptGrpComparisonType.CSRPTGRPDATE:

                                            DateTime date1 = (DateTime)m_vGroups[i + 1].value;
                                            DateTime date2 = (DateTime)value;
                                            if (date1 != date2)
                                            {
                                                addGroup(i, j, value);
                                            }
                                            break;
                                    }
                                }
                            }
                            m_vGroups[i + 1].groups[m_vGroups[i + 1].groups.Length - 1].last = j - 1;
                            m_vGroups[i + 1].value = null;
                        }
                    }
                }
            }
            return true;
        }

        private int pEstimateLoops(int n)
        {
            int q = 0;
            for (q = n - 1; q <= 1; q--)
            {
                n = n + q;
            }
            return n;
        }

        private bool orderNumberAsc(int first, int last, int orderBy)
        {
            int i = 0;
            int j = 0;
            int t = 0;
            int q = 0;
            bool bChanged = false;

            t = pEstimateLoops(last - first);
            for (i = first + 1; i <= last; i++)
            {
                bChanged = false;
                for (j = last; j <= i; j--)
                {
                    q = q + 1;
                    double value1 = cReportGlobals.val(m_rows.Rows[m_vRowsIndex[j]][orderBy]);
                    double value2 = cReportGlobals.val(m_rows.Rows[m_vRowsIndex[j - 1]][orderBy]);
                    if (value1 < value2)
                    {
                        if (!OnProgress("", 0, q, t)) 
                        { 
                            return false; 
                        }
                        changeRow(j, j - 1);
                        bChanged = true;
                    }
                }
                if (!OnProgress("", 0, q, t)) 
                { 
                    return false; 
                }
                if (!bChanged) 
                { 
                    break; 
                }
            }
            return true;
        }

        private bool orderNumberDesc(int first, int last, int orderBy)
        {
            int i = 0;
            int j = 0;
            int t = 0;
            int q = 0;
            bool bChanged = false;

            t = pEstimateLoops(last - first);
            for (i = first + 1; i <= last; i++)
            {
                bChanged = false;
                for (j = last; j <= i; j--)
                {
                    q = q + 1;
                    double number1 = cReportGlobals.val(m_rows.Rows[m_vRowsIndex[j]][orderBy]);
                    double number2 = cReportGlobals.val(m_rows.Rows[m_vRowsIndex[j - 1]][orderBy]);
                    if (number1 > number2)
                    {
                        if (!OnProgress("", 0, q, t))
                        {
                            return false;
                        }
                        changeRow(j, j - 1);
                        bChanged = true;
                    }
                }
                if (!OnProgress("", 0, q, t))
                {
                    return false;
                }
                if (!bChanged)
                {
                    break;
                }
            }
            return true;
        }

        private bool orderTextAsc(int first, int last, int orderBy)
        {
            int i = 0;
            int j = 0;
            int t = 0;
            int q = 0;
            bool bChanged = false;

            t = pEstimateLoops(last - first);
            for (i = first + 1; i <= last; i++)
            {
                bChanged = false;
                for (j = last; j <= i; j--)
                {
                    q = q + 1;
                    String text1 = (String)cReportGlobals.valVariant(m_rows.Rows[m_vRowsIndex[j]][orderBy]);
                    String text2 = (String)cReportGlobals.valVariant(m_rows.Rows[m_vRowsIndex[j - 1]][orderBy]);
                    if (String.Compare(text1.ToLower(), 
                                        text2.ToLower(), 
                                        StringComparison.CurrentCulture) < 0)
                    {
                        if (!OnProgress("", 0, q, t)) 
                        { 
                            return false; 
                        }
                        changeRow(j, j - 1);
                        bChanged = true;
                    }
                }
                if (!OnProgress("", 0, q, t)) 
                { 
                    return false; 
                }
                if (!bChanged) 
                { 
                    break; 
                }
            }
            return true;
        }

        private bool orderTextDesc(int first, int last, int orderBy)
        {
            int i = 0;
            int j = 0;
            int t = 0;
            int q = 0;
            bool bChanged = false;

            t = pEstimateLoops(last - first);
            for (i = first + 1; i <= last; i++)
            {
                bChanged = false;
                for (j = last; j <= i; j--)
                {
                    q = q + 1;
                    String text1 = (String)cReportGlobals.valVariant(m_rows.Rows[m_vRowsIndex[j]][orderBy]);
                    String text2 = (String)cReportGlobals.valVariant(m_rows.Rows[m_vRowsIndex[j - 1]][orderBy]);
                    if (String.Compare(text1.ToLower(),
                                        text2.ToLower(),
                                        StringComparison.CurrentCulture) > 0)
                    {
                        if (!OnProgress("", 0, q, t))
                        {
                            return false;
                        }
                        changeRow(j, j - 1);
                        bChanged = true;
                    }
                }
                if (!OnProgress("", 0, q, t))
                {
                    return false;
                }
                if (!bChanged)
                {
                    break;
                }
            }
            return true;
        }

        private void changeRow(int i, int j)
        {
            int q = m_vRowsIndex[j];
            m_vRowsIndex[j] = m_vRowsIndex[i];
            m_vRowsIndex[i] = q;
        }
        
        private bool evalFunctions(int idxGroup, csRptWhenEval whenEval)
        {
            cReportFormula formula = null;
            bool bHaveToEvalRow = false;
            int idxRowEvalued = 0;
            int recordCount = 0;

            if (m_rows != null)
            {
                recordCount = m_vRowsIndex.Length;
            }

            // if the row to be evaluated is valid
            //
            if (m_iRowFormula <= recordCount)
            {
                switch (idxGroup)
                {
                    case C_IDX_GROUP_HEADER:
                    case C_IDX_GROUP_REPORTHEADER:
                        idxRowEvalued = C_IDX_H_LAST_ROW_EVALUED;
                        break;

                    case C_IDX_GROUP_DETAIL:
                        idxRowEvalued = C_IDX_D_LAST_ROW_EVALUED;
                        break;

                    case C_IDX_GROUP_FOOTER:
                    case C_IDX_GROUP_REPORTFOOTER:
                        idxRowEvalued = C_IDX_F_LAST_ROW_EVALUED;
                        break;

                    // groups headers o footers
                    default:
                        idxRowEvalued = C_IDX_G_LAST_ROW_EVALUED;
                        break;
                }

                // evaluate functions before printing
                //
                if (whenEval == csRptWhenEval.CSRPTEVALPRE)
                {
                    if (idxRowEvalued == C_IDX_G_LAST_ROW_EVALUED)
                    {
                        // if it is a footer
                        //
                        if (idxGroup < 0)
                        {
                            bHaveToEvalRow = m_vGroups[idxGroup * -1].lastFPreRowEvalued < m_iRowFormula;
                        }
                        else
                        {
                            bHaveToEvalRow = m_vGroups[idxGroup].lastHPreRowEvalued < m_iRowFormula;
                        }
                    }
                    else
                    {
                        bHaveToEvalRow = m_lastRowPreEvalued[idxRowEvalued] < m_iRowFormula;
                    }

                }
                // evaluate function after printing
                //
                else
                {
                    if (idxRowEvalued == C_IDX_G_LAST_ROW_EVALUED)
                    {
                        // if it is a footer
                        //
                        if (idxGroup < 0)
                        {
                            bHaveToEvalRow = m_vGroups[idxGroup * -1].lastFPostRowEvalued < m_iRowFormula;
                        }
                        else
                        {
                            bHaveToEvalRow = m_vGroups[idxGroup].lastHPostRowEvalued < m_iRowFormula;
                        }
                    }
                    else
                    {
                        bHaveToEvalRow = m_lastRowPostEvalued[idxRowEvalued] < m_iRowFormula;
                    }
                }

                // if we need to evaluate
                //
                if (bHaveToEvalRow)
                {
                    for (int _i = 0; _i < m_formulas.count(); _i++)
                    {
                        formula = m_formulas.item(_i);
                        if (formula.getWhenEval() == whenEval
                            && (idxGroup == formula.getIdxGroup()
                                    || formula.getIdxGroup2() == idxGroup))
                        {
                            formula.setHaveToEval(true);
                        }
                    }
                    for (int _i = 0; _i < m_formulas.count(); _i++)
                    {
                        formula = m_formulas.item(_i);
                        if (formula.getWhenEval() == whenEval
                            && (idxGroup == formula.getIdxGroup()
                                || formula.getIdxGroup2() == idxGroup))
                        {
                            if (formula.getIdxGroup2() == idxGroup)
                            {
                                m_compiler.evalFunctionGroup(formula);
                            }
                            else
                            {
                                m_compiler.evalFunction(formula);
                            }
                        }
                    }

                    // update the last evaluated row
                    //

                    // evaluate before printing
                    //
                    if (whenEval == csRptWhenEval.CSRPTEVALPRE)
                    {
                        if (idxRowEvalued == C_IDX_G_LAST_ROW_EVALUED)
                        {
                            // if it is a footer
                            //
                            if (idxGroup < 0)
                            {
                                m_vGroups[idxGroup * -1].lastFPreRowEvalued = m_iRowFormula;
                            }
                            else
                            {
                                m_vGroups[idxGroup].lastHPreRowEvalued = m_iRowFormula;
                            }
                        }
                        else
                        {
                            m_lastRowPreEvalued[idxRowEvalued] = m_iRowFormula;
                        }
                    }
                    // evaluate after printing
                    //
                    else
                    {
                        if (idxRowEvalued == C_IDX_G_LAST_ROW_EVALUED)
                        {
                            // if it is a footer
                            //
                            if (idxGroup < 0)
                            {
                                m_vGroups[idxGroup * -1].lastFPostRowEvalued = m_iRowFormula;
                            }
                            else
                            {
                                m_vGroups[idxGroup].lastHPostRowEvalued = m_iRowFormula;
                            }
                        }
                        else
                        {
                            m_lastRowPostEvalued[idxRowEvalued] = m_iRowFormula;
                        }
                    }
                }
            }
            return true;
        }

        // all the formulas which are in headers are compile
        // only one time for page. to do this we set the idxGroup
        // of the formula to -2000
        //
        private void pSetGroupFormulaHeaders()
        {
            pSetGroupFormulaHF(m_headers, C_IDX_GROUP_HEADER);

            // the main header is -2000
            //
            if (m_headers.item(1).getHasFormulaHide())
            {
                m_headers.item(1).getFormulaHide().setIdxGroup(C_IDX_GROUP_REPORTHEADER);
            }

            cReportSectionLine secLn = null;
            cReportControl ctrl = null;

            for (int _i = 0; _i < m_headers.item(1).getSectionLines().count(); _i++)
            {
                secLn = m_headers.item(1).getSectionLines().item(_i);
                for (int _j = 0; _j < secLn.getControls().count(); _j++)
                {
                    ctrl = secLn.getControls().item(_j);
                    if (ctrl.getHasFormulaHide())
                    {
                        ctrl.getFormulaHide().setIdxGroup(C_IDX_GROUP_REPORTHEADER);
                    }
                    if (ctrl.getHasFormulaValue())
                    {
                        ctrl.getFormulaValue().setIdxGroup(C_IDX_GROUP_REPORTHEADER);
                    }
                }
            }
        }

        private void pSetGroupsInCtrlFormulaHide()
        {
            for (int _i = 0; _i < m_groups.count(); _i++)
            {
                cReportGroup group = m_groups.item(_i);
                pSetGroupsInCtrlFormulaHideAux(group.getHeader().getSectionLines(), group.getIndex());
                pSetGroupsInCtrlFormulaHideAux(group.getFooter().getSectionLines(), group.getIndex());
            }
        }

        private void pSetGroupsInCtrlFormulaHideAux(cReportSectionLines scls, int idxGrop)
        { // TODO: Use of ByRef founded Private Sub pSetGroupsInCtrlFormulaHideAux(ByRef Scls As cReportSectionLines, ByVal IdxGrop As Integer)
            cReportSectionLine scl = null;
            cReportControl ctrl = null;

            for (int _i = 0; _i < scls.count(); _i++)
            {
                scl = scls.item(_i);
                for (int _j = 0; _j < scl.getControls().count(); _j++)
                {
                    ctrl = scl.getControls().item(_j);
                    if (ctrl.getHasFormulaHide())
                    {
                        if (ctrl.getFormulaHide().getIdxGroup() == 0)
                        {
                            ctrl.getFormulaHide().setIdxGroup(idxGrop);
                        }
                    }
                }
            }
        }

        private void pSetGroupFormulaHF(cReportSections sections, int idxGrop)
        { // TODO: Use of ByRef founded Private Sub pSetGroupFormulaHF(ByRef Sections As cReportSections, ByVal IdxGrop As Integer)
            cReportSection sec = null;
            cReportSectionLine secLn = null;
            cReportControl ctrl = null;

            for (int _i = 0; _i < sections.count(); _i++)
            {
                sec = sections.item(_i);
                for (int _j = 0; _j < sec.getSectionLines().count(); _j++)
                {
                    secLn = sec.getSectionLines().item(_j);
                    for (int _k = 0; _k < secLn.getControls().count(); _k++)
                    {
                        ctrl = secLn.getControls().item(_k);
                        if (ctrl.getHasFormulaHide())
                        {
                            if (ctrl.getFormulaHide().getIdxGroup() == 0)
                            {
                                ctrl.getFormulaHide().setIdxGroup(idxGrop);
                            }
                        }
                        if (ctrl.getHasFormulaValue())
                        {
                            if (ctrl.getFormulaValue().getIdxGroup() == 0)
                            {
                                ctrl.getFormulaValue().setIdxGroup(idxGrop);
                            }
                        }
                    }
                }
            }
        }

        private bool compileReport()
        {
            cReportControl ctrl = null;

            for (int _i = 0; _i < m_controls.count(); _i++)
            {
                ctrl = m_controls.item(_i);
                if (ctrl.getHasFormulaHide())
                {
                    if (!m_compiler.checkSyntax(ctrl.getFormulaHide())) 
                    { 
                        return false; 
                    }
                    // to have debug info
                    //
                    ctrl.getFormulaHide().setSectionName(ctrl.getSectionLine().getSectionName());
                    ctrl.getFormulaHide().setSectionLineIndex(ctrl.getSectionLine().getIndex());
                    ctrl.getFormulaHide().setControlName(ctrl.getName());

                    // add the formula to the formulas collection
                    //
                    addFormula(ctrl.getFormulaHide(), ctrl.getName() + "_" + "H");
                }
                if (ctrl.getHasFormulaValue())
                {
                    if (!m_compiler.checkSyntax(ctrl.getFormulaValue())) 
                    { 
                        return false; 
                    }

                    // to have debug info
                    //
                    ctrl.getFormulaValue().setSectionName(ctrl.getSectionLine().getSectionName());
                    ctrl.getFormulaValue().setSectionLineIndex(ctrl.getSectionLine().getIndex());
                    ctrl.getFormulaValue().setControlName(ctrl.getName());

                    // add the formula to the formulas collection
                    //
                    addFormula(ctrl.getFormulaValue(), ctrl.getName() + "_" + "V");
                }
            }

            if (!pAddFormulasInSection(m_headers)) { return false; }
            if (!pAddFormulasInSection(m_groupsHeaders)) { return false; }
            if (!pAddFormulasInSection(m_groupsFooters)) { return false; }
            if (!pAddFormulasInSection(m_details)) { return false; }
            if (!pAddFormulasInSection(m_footers)) { return false; }

            cReportFormula formula = null;

            for (int _i = 0; _i < m_formulas.count(); _i++)
            {
                formula = m_formulas.item(_i);
                m_compiler.initVariable(formula);
            }

            pSetIndexGroupInFormulaGroups(m_headers);
            pSetIndexGroupInFormulaGroups(m_groupsHeaders);
            pSetIndexGroupInFormulaGroups(m_groupsFooters);
            pSetIndexGroupInFormulaGroups(m_details);
            pSetIndexGroupInFormulaGroups(m_footers);

            m_compiler.clearVariables();

            return true;
        }

        private void pSetIndexGroupInFormulaGroups(cReportSections sections)
        { // TODO: Use of ByRef founded Private Function pSetIndexGroupInFormulaGroups(ByRef Sections As cReportSections) As Boolean
            cReportSection sec = null;
            cReportSectionLine secLn = null;
            cReportControl ctrl = null;

            for (int _i = 0; _i < sections.count(); _i++)
            {
                sec = sections.item(_i);
                if (sec.getHasFormulaHide())
                {
                    pSetFormulaIndexGroup(sec.getFormulaHide(), sec);
                }
                for (int _j = 0; _j < sec.getSectionLines().count(); _j++)
                {
                    secLn = sec.getSectionLines().item(_j);
                    if (secLn.getHasFormulaHide())
                    {
                        pSetFormulaIndexGroup(secLn.getFormulaHide(), sec);
                    }
                    for (int _k = 0; _k < secLn.getControls().count(); _k++)
                    {
                        ctrl = secLn.getControls().item(_k);
                        if (ctrl.getHasFormulaHide())
                        {
                            pSetFormulaIndexGroup(ctrl.getFormulaHide(), sec);
                        }
                        if (ctrl.getHasFormulaValue())
                        {
                            pSetFormulaIndexGroup(ctrl.getFormulaValue(), sec);
                        }
                    }
                }
            }
        }

        private void pSetFormulaIndexGroup(cReportFormula formula, cReportSection sec)
        { // TODO: Use of ByRef founded Private Function pSetFormulaIndexGroup(ByRef Formula As cReportFormula, ByRef Sec As cReportSection) As Boolean
            cReportFormulaInt fint = null;
            int indexGroup = 0;

            for (int _i = 0; _i < formula.getFormulasInt().count(); _i++)
            {
                fint = formula.getFormulasInt().item(_i);

                if (pIsGroupFormula((int)fint.getFormulaType()))
                {
                    if (fint.getFormulaType() == csRptFormulaType.CSRPTGROUPPERCENT)
                    {
                        formula.setIdxGroup2(0);
                        indexGroup = (int)cReportGlobals.val(fint.getParameters().item(3).getValue());
                    }
                    else
                    {
                        indexGroup = (int)cReportGlobals.val(fint.getParameters().item(2).getValue());
                    }
                    if (fint.getParameters().item(cReportGlobals.C_KEYINDEXGROUP) == null)
                    {
                        fint.getParameters().add2("", cReportGlobals.C_KEYINDEXGROUP);
                    }
                    if (indexGroup == -1)
                    {
                        if (sec.getTypeSection() == csRptTypeSection.CSRPTTPGROUPHEADER
                            || sec.getTypeSection() == csRptTypeSection.CSRPTTPGROUPFOOTER)
                        {
                            // index of the group
                            //
                            fint.getParameters().item(cReportGlobals.C_KEYINDEXGROUP).setValue(sec.getIndex().ToString());
                            formula.setIdxGroup(sec.getIndex());
                        }
                        else if (sec.getTypeSection() == csRptTypeSection.CSRPTTPMAINSECTIONDETAIL)
                        {
                            // index of the most internal group
                            //
                            fint.getParameters().item(cReportGlobals.C_KEYINDEXGROUP).setValue(m_groups.count().ToString());
                            formula.setIdxGroup(m_groups.count());
                        }
                        else
                        {
                            fint.getParameters().item(cReportGlobals.C_KEYINDEXGROUP).setValue("0");
                            formula.setIdxGroup(0);
                        }
                    }
                    else
                    {
                        fint.getParameters().item(cReportGlobals.C_KEYINDEXGROUP).setValue(indexGroup.ToString());
                        formula.setIdxGroup(indexGroup);
                    }
                }
            }
        }

        private bool pIsGroupFormula(int formulaType)
        {
            switch (formulaType)
            {
                case (int)csRptFormulaType.CSRPTGROUPTOTAL:
                case (int)csRptFormulaType.CSRPTGROUPMAX:
                case (int)csRptFormulaType.CSRPTGROUPMIN:
                case (int)csRptFormulaType.CSRPTGROUPAVERAGE:
                case (int)csRptFormulaType.CSRPTGROUPPERCENT:
                case (int)csRptFormulaType.CSRPTGROUPCOUNT:
                case (int)csRptFormulaType.CSRPTGROUPLINENUMBER:

                    return true;

                default:

                    return false;
            }
        }

        private bool pAddFormulasInSection(cReportSections sections)
        { // TODO: Use of ByRef founded Private Function pAddFormulasInSection(ByRef Sections As cReportSections) As Boolean
            cReportSection sec = null;
            cReportSectionLine secLn = null;

            for (int _i = 0; _i < sections.count(); _i++)
            {
                sec = sections.item(_i);
                if (sec.getHasFormulaHide())
                {
                    if (!m_compiler.checkSyntax(sec.getFormulaHide())) 
                    { 
                        return false; 
                    }
                    // to have debug info
                    //
                    sec.getFormulaHide().setSectionName(sec.getName());

                    // add the formula to the formulas collection
                    //
                    addFormula(sec.getFormulaHide(), sec.getName() + "_" + "H");
                }
                for (int _j = 0; _j < sec.getSectionLines().count(); _j++)
                {
                    secLn = sec.getSectionLines().item(_j);
                    if (secLn.getHasFormulaHide())
                    {
                        if (!m_compiler.checkSyntax(secLn.getFormulaHide())) 
                        { 
                            return false; 
                        }
                        // to have debug info
                        //
                        secLn.getFormulaHide().setSectionName(secLn.getSectionName());
                        secLn.getFormulaHide().setSectionLineIndex(secLn.getIndex());

                        // add the formula to the formulas collection
                        //
                        addFormula(secLn.getFormulaHide(), sec.getName() 
                                    + "_R_" + secLn.getIndex().ToString() + "_" + "H");
                    }
                }
            }
            return true;
        }

        private void addFormula(cReportFormula formula, String name)
        { // TODO: Use of ByRef founded Private Sub AddFormula(ByRef Formula As cReportFormula, ByVal Name As String)
            if (m_formulas.item(name) == null)
            {
                m_formulas.add2(formula, name);
            }
        }

        private float getHeightHeader()
        {
            cReportSection sec = null;
            float height = 0;
            bool isVisible = false;

            for (int _i = 0; _i < m_headers.count(); _i++)
            {
                sec = m_headers.item(_i);
                if (sec.getHasFormulaHide())
                {
                    isVisible = cReportGlobals.val(m_compiler.resultFunction(sec.getFormulaHide())) != 0;
                }
                else
                {
                    isVisible = true;
                }

                if (isVisible) 
                { 
                    height = height + sec.getAspect().getHeight(); 
                }
            }
            return height;
        }

        private float getTopFooter()
        {
            int offset = 0;

            cReportPaperInfo w_paperInfo = m_launchInfo.getPrinter().getPaperInfo();
            if (w_paperInfo.getPaperSize() == csReportPaperType.CSRPTPAPERUSER)
            {
                offset = m_paperInfo.getCustomHeight() - w_paperInfo.getCustomHeight();
            }

            cReportAspect w_aspect = m_footers.item(0).getAspect();
            return w_aspect.getTop() - offset;
        }

        private void addFieldToNewPage(cReportSections sections, cReportPage page, int where)
        { // TODO: Use of ByRef founded Private Sub AddFieldToNewPage(ByRef Sections As cReportSections, ByRef Page As cReportPage, ByVal Where As Long)
            cReportPageField field = null;
            cReportSection sec = null;
            cReportSectionLine secline = null;
            cReportControl ctrl = null;
            bool isVisible = false;
            int indexCtrl = 0;
            float offset = 0;
            int recordCount = 0;

            if (m_rows != null)
            {
                recordCount = m_vRowsIndex.Length;
            }

            // this indexes means
            //
            // in which datasource is this control
            //
            int indexRows = 0;
            // in which row of the datasource is the control
            //
            int indexRow = 0;
            int indexField = 0;

            for (int _i = 0; _i < sections.count(); _i++)
            {
                sec = sections.item(_i);
                m_lineIndex = m_lineIndex + 1;

                if (sec.getHasFormulaHide())
                {
                    isVisible = cReportGlobals.val(m_compiler.resultFunction(sec.getFormulaHide())) != 0;
                }
                else
                {
                    isVisible = true;
                }
                if (isVisible)
                {
                    for (int _j = 0; _j < sec.getSectionLines().count(); _j++)
                    {
                        secline = sec.getSectionLines().item(_j);
                        if (secline.getHasFormulaHide())
                        {
                            isVisible = cReportGlobals.val(m_compiler.resultFunction(secline.getFormulaHide())) != 0;
                        }
                        else
                        {
                            isVisible = true;
                        }
                        if (isVisible)
                        {
                            // For Each Ctrl In Secline.Controls
                            //
                            for (indexCtrl = 1; indexCtrl <= secline.getControls().getCollByLeft().Length; indexCtrl++)
                            {
                                ctrl = secline.getControls().item(secline.getControls().getCollByLeft()[indexCtrl]);

                                if (where == C_HEADERS)
                                {
                                    field = page.getHeader().add(null, "");
                                }
                                else if (where == C_FOOTERS)
                                {
                                    field = page.getFooter().add(null, "");
                                }

                                field.setIndexLine(m_lineIndex);

                                if (ctrl.getHasFormulaValue())
                                {
                                    field.setValue(
                                        cReportGlobals.format(
                                            m_compiler.resultFunction(ctrl.getFormulaValue()), 
                                            ctrl.getLabel().getAspect().getFormat()));
                                }
                                else
                                {
                                    switch (ctrl.getControlType())
                                    {
                                        case csRptControlType.CSRPTCTFIELD:

                                            pGetIndexRows(indexRows, indexRow, indexField, ctrl);

                                            if (m_collRows[indexRows] != null)
                                            {
                                                // it looks ugly, dont think you?
                                                //
                                                // maybe this help a litle:
                                                //
                                                //    m_vCollRows(IndexRows)    a matrix with the data 
                                                //                              contained in the datasource
                                                //                              referd by this control
                                                //
                                                //    (IndexField, IndexRow)    a cell in this matrix
                                                //
                                                object value = m_collRows[indexRows].Rows[indexRow][indexField];
                                                field.setValue(
                                                    cReportGlobals.format(
                                                        cReportGlobals.valVariant(value),
                                                        ctrl.getLabel().getAspect().getFormat()));
                                            }
                                            break;

                                        case csRptControlType.CSRPTCTLABEL:
                                            field.setValue(
                                                cReportGlobals.format(
                                                    ctrl.getLabel().getText(), 
                                                    ctrl.getLabel().getAspect().getFormat()));
                                            break;

                                        case csRptControlType.CSRPTCTIMAGE:
                                            field.setValue(
                                                cReportGlobals.format(
                                                    ctrl.getLabel().getText(), 
                                                    ctrl.getLabel().getAspect().getFormat()));
                                            field.setImage(ctrl.getImage().getImage());
                                            break;

                                        case csRptControlType.CSRPTCTDBIMAGE:

                                            pGetIndexRows(indexRows, indexRow, indexField, ctrl);

                                            if (m_collRows[indexRows] != null)
                                            {
                                                field.setImage(pGetImage(indexRows, indexField, indexRow));
                                            }
                                            break;

                                        case csRptControlType.CSRPTCTCHART:

                                            pGetIndexRows(indexRows, indexRow, indexField, ctrl);
                                            field.setImage(pGetChartImage(indexRows, indexField, indexRow, ctrl));
                                            break;
                                    }
                                }

                                field.setInfo(m_pageSetting.item(ctrl.getKey()));
                                field.setTop(field.getInfo().getAspect().getTop() + offset);

                                if (ctrl.getHasFormulaHide())
                                {
                                    field.setVisible(
                                        cReportGlobals.val(m_compiler.resultFunction(ctrl.getFormulaHide())) != 0);
                                }
                                else
                                {
                                    field.setVisible(true);
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (where == C_HEADERS)
                    {
                        offset = offset - sec.getAspect().getHeight();
                    }
                    else if (where == C_FOOTERS)
                    {
                        offset = offset + sec.getAspect().getHeight();
                    }
                }
            }
        }

        private bool createPageSetting()
        {
            // clear the collection
            //
            m_pageSetting.clear();

            m_pageSetting.setHeight(m_launchInfo.getPrinter().getPaperInfo().getHeight());

            cReportSection sec = null;
            cReportSectionLine secline = null;
            cReportControl ctrl = null;

            // headers
            //
            for (int _i = 0; _i < m_headers.count(); _i++)
            {
                sec = m_headers.item(_i);
                for (int _j = 0; _j < sec.getSectionLines().count(); _j++)
                {
                    secline = sec.getSectionLines().item(_j);
                    for (int _k = 0; _k < secline.getControls().count(); _k++)
                    {
                        ctrl = secline.getControls().item(_k);
                        cReportPageInfo pageInfo = m_pageSetting.add(secline, null, ctrl.getKey());
                        pageInfo.setAspect(ctrl.getLabel().getAspect());
                        pageInfo.setName(ctrl.getName());
                        pageInfo.setFieldType(ctrl.getField().getFieldType());
                        pageInfo.setTag(ctrl.getTag());
                    }
                }
            }
            // detail
            //
            for (int _i = 0; _i < m_details.count(); _i++)
            {
                sec = m_details.item(_i);
                for (int _j = 0; _j < sec.getSectionLines().count(); _j++)
                {
                    secline = sec.getSectionLines().item(_j);
                    for (int _k = 0; _k < secline.getControls().count(); _k++)
                    {
                        ctrl = secline.getControls().item(_k);
                        cReportPageInfo pageInfo = m_pageSetting.add(secline, null, ctrl.getKey());
                        pageInfo.setAspect(ctrl.getLabel().getAspect());
                        pageInfo.setName(ctrl.getName());
                        pageInfo.setFieldType(ctrl.getField().getFieldType());
                        pageInfo.setTag(ctrl.getTag());
                    }
                }
            }
            // footers
            //
            int offset = 0;

            cReportPaperInfo w_paperInfo = m_launchInfo.getPrinter().getPaperInfo();
            if (w_paperInfo.getPaperSize() == csReportPaperType.CSRPTPAPERUSER)
            {
                offset = m_originalHeight - w_paperInfo.getCustomHeight();
            }
            for (int _i = 0; _i < m_footers.count(); _i++)
            {
                sec = m_footers.item(_i);
                for (int _j = 0; _j < sec.getSectionLines().count(); _j++)
                {
                    secline = sec.getSectionLines().item(_j);
                    for (int _k = 0; _k < secline.getControls().count(); _k++)
                    {
                        ctrl = secline.getControls().item(_k);
                        cReportPageInfo pageInfo = m_pageSetting.add(secline, null, ctrl.getKey());
                        pageInfo.setAspect(ctrl.getLabel().getAspect());
                        cReportAspect aspect = pageInfo.getAspect();
                        aspect.setTop(aspect.getTop() - offset);
                        pageInfo.setName(ctrl.getName());
                        pageInfo.setFieldType(ctrl.getField().getFieldType());
                        pageInfo.setTag(ctrl.getTag());
                    }
                }
            }
            // groups
            //
            for (int _i = 0; _i < m_groups.count(); _i++)
            {
                cReportGroup grp = m_groups.item(_i);
                // header
                //
                for (int _j = 0; _j < grp.getHeader().getSectionLines().count(); _j++)
                {
                    secline = grp.getHeader().getSectionLines().item(_j);
                    for (int _k = 0; _k < secline.getControls().count(); _k++)
                    {
                        ctrl = secline.getControls().item(_k);
                        cReportPageInfo pageInfo = m_pageSetting.add(secline, null, ctrl.getKey());
                        pageInfo.setAspect(ctrl.getLabel().getAspect());
                        pageInfo.setName(ctrl.getName());
                        pageInfo.setFieldType(ctrl.getField().getFieldType());
                        pageInfo.setTag(ctrl.getTag());
                    }
                }
                // footer
                //
                for (int _j = 0; _j < grp.getFooter().getSectionLines().count(); _j++)
                {
                    secline = grp.getFooter().getSectionLines().item(_j);
                    for (int _k = 0; _k < secline.getControls().count(); _k++)
                    {
                        ctrl = secline.getControls().item(_k);
                        cReportPageInfo pageInfo = m_pageSetting.add(secline, null, ctrl.getKey());
                        pageInfo.setAspect(ctrl.getLabel().getAspect());
                        pageInfo.setName(ctrl.getName());
                        pageInfo.setFieldType(ctrl.getField().getFieldType());
                        pageInfo.setTag(ctrl.getTag());
                    }
                }
            }
            return true;
        }

        private bool pGetDataAux(List<object[]> recordsets)
        { // TODO: Use of ByRef founded Private Function pGetDataAux(ByRef Recordsets As Collection) As Boolean
            for (int _i = 0; _i < m_connectsAux.count(); _i++)
            {
                cReportConnect connect = m_connectsAux.item(_i);
                G.redimPreserve(ref m_collRows, m_collRows.Length + 1);
                if (!pGetData(ref m_collRows[m_collRows.Length - 1], connect, false, recordsets))
                {
                    return false;
                }
            }
            G.redim(ref m_vRowsIndexAux, m_collRows.Length);
            return true;
        }

        private bool pGetData(
            ref DataTable vRows,
            cReportConnect connect,
            bool createIndexVector,
            List<object[]> recordsets)
        {
            String strConnect = "";
            bool saveInReport = false;
            CSDataBase.cDataBase cn = null;
            object[] varRs = null;
            DataTable rsAux = null;
            DbDataReader dr = null;

            // if we get an string connection
            //
            if (m_launchInfo.getStrConnect().Trim() != "")
            {
                strConnect = m_launchInfo.getStrConnect();
            }
            // if m_launchInfo.getStrConnect() is empty we will use
            // the connection of the connect object
            // 
            else
            {
                strConnect = connect.getStrConnect();
                saveInReport = true;
            }
            if (!getReportDisconnected())
            {
                if (strConnect.Trim() == "")
                {
                    cWindow.msgWarning("The connection settings were not defined."
                                        + "Both the LaunchInfo and the Connect object have their "
                                        + "strConnect property empty. Whitout this connection string "
                                        + "it will be imposible to open the connection to the database.",
                                        "CSReportEditor");
                    return false;
                }

                cn = new CSDataBase.cDataBase();

                if (m_isForWeb)
                {
                    cn.setSilent(true);
                }
                if (connect.getCommandTimeout() > 0)
                {
                    cn.setCommandTimeout(connect.getCommandTimeout());
                }
                if (connect.getConnectionTimeout() > 0)
                {
                    cn.setConnectionTimeout(connect.getConnectionTimeout());
                }

                // open the connection
                //
                if (!cn.initDb("", "", "", "", strConnect))
                {
                    if (!resumeDBAccessMissing(strConnect, saveInReport, cn))
                    {
                        return false;
                    }
                }

                // we need to prepare the first sentence
                //
                String sqlstmt = "";

                // if it was a select
                //
                if (m_launchInfo.getSqlstmt().Trim() != "")
                {
                    sqlstmt = m_launchInfo.getSqlstmt();
                }
                else
                {
                    if (connect.getDataSourceType() == csDataSourceType.CDDTPROCEDURE)
                    {
                        sqlstmt = "exec [" + connect.getDataSource() + "] " + connect.getSqlParameters();
                    }
                    else if (connect.getDataSourceType() == csDataSourceType.CSDTTABLE)
                    {
                        sqlstmt = "select * from [" + connect.getDataSource() + "]";
                    }
                    else
                    {
                        sqlstmt = connect.getDataSource();
                    }
                }

                // open the recordset
                //
                cn.setOpenRsExDescript(m_descripUser);

                if (!cn.loadDataTable(true,
                                        false,
                                        false,
                                        sqlstmt,
                                        out vRows,
                                        out dr,
                                        "GetData",
                                        C_MODULE,
                                        ""))
                {
                    return false;
                }

                if (vRows.Rows.Count == 0)
                {
                    vRows = null;
                    if (createIndexVector)
                    {
                        G.redim(ref m_vRowsIndex, 0);
                    }
                }
                else
                {
                    if (createIndexVector)
                    {
                        G.redim(ref m_vRowsIndex, vRows.Rows.Count);
                        int k = 0;
                        for (k = 0; k <= m_vRowsIndex.Length; k++)
                        {
                            m_vRowsIndex[k] = k;
                        }
                    }
                }

                G.redim(ref varRs, 1);
                varRs[0] = vRows;
                varRs[1] = connect.getDataSource();
                recordsets.Add(varRs);

                // we need to load every recordset from every data source
                // in the recordset collection (this code suport multiples
                // recordset in the same reader)
                //
                while (dr.NextResult())
                {
                    rsAux = new DataTable();
                    rsAux.Load(dr);
                    G.redimPreserve(ref m_collRows, m_collRows.Length + 1);
                    G.redim(ref varRs, 1);
                    varRs[0] = rsAux;
                    varRs[1] = connect.getDataSource();
                    recordsets.Add(varRs);
                }

                cn.closeDb();
            }
            else
            {
                vRows = null;
                if (createIndexVector)
                {
                    G.redim(ref m_vRowsIndex, 0);
                }
            }
            if (m_rows != null)
            {
                m_recordCount = m_vRowsIndex.Length;
            }
            else
            {
                m_recordCount = 0;
            }
            m_iRow = 0;
            m_idxGroupHeader = 0;
            m_idxGroupFooter = 0;

            return true;
        }

        private void pInitRowFormulas()
        {
            int i = 0;

            G.redim(ref m_lastRowPreEvalued, 2);
            G.redim(ref m_lastRowPostEvalued, 2);

            for (i = 0; i <= 2; i++)
            {
                m_lastRowPreEvalued[i] = -1;
                m_lastRowPostEvalued[i] = -1;
            }

            for (i = 0; i <= m_groupCount; i++)
            {
                // headers
                //
                m_vGroups[i].lastHPreRowEvalued = -1;
                m_vGroups[i].lastHPostRowEvalued = -1;

                // footers
                //
                m_vGroups[i].lastFPreRowEvalued = -1;
                m_vGroups[i].lastFPostRowEvalued = -1;
            }
        }

        private bool nLoad(CSXml.cXml docXml)
        {
            pDestroyCrossRef(m_headers);
            pDestroyCrossRef(m_details);
            pDestroyCrossRef(m_footers);
            pDestroyCrossRef(m_groups.getGroupsHeaders());
            pDestroyCrossRef(m_groups.getGroupsFooters());

            m_headers.clear();
            m_groups.clear();
            m_details.clear();
            m_footers.clear();
            m_controls.clear();
            m_formulas.clear();
            m_connect.getColumns().clear();
            m_connect.getParameters().clear();

            m_details.setCopyColl(m_controls);
            m_headers.setCopyColl(m_controls);
            m_footers.setCopyColl(m_controls);
            m_groupsHeaders.setCopyColl(m_controls);
            m_groupsFooters.setCopyColl(m_controls);

            if (!loadAux(docXml, m_headers, C_NODERPTHEADERS)) { return false; }
            if (!loadAux(docXml, m_details, C_NODERPTDETAILS)) { return false; }
            if (!loadAux(docXml, m_footers, C_NODERPTFOOTERS)) { return false; }

            if (!loadGroups(docXml)) { return false; }

            pFixGroupIndex();

            if (!loadConnect(docXml)) { return false; }
            if (!loadConnectsAux(docXml)) { return false; }
            if (!loadLaunchInfo(docXml)) { return false; }

            loadPaperInfo(docXml);

            sortCollection();

            m_originalHeight = m_paperInfo.getCustomHeight();

            return true;
        }

        private void pFixGroupIndex()
        {
            int idx = 0;
            for (int _i = 0; _i < m_groups.count(); _i++)
            {
                cReportGroup group = m_groups.item(_i);
                idx = idx + 1;
                group.setIndex(idx);
            }
        }

        private void loadPaperInfo(CSXml.cXml docXml)
        {
            XmlNode nodeObj = null;
            nodeObj = docXml.getRootNode();
            nodeObj = docXml.getNodeFromNode(nodeObj, C_NODEPAPERINFO);
            if (!m_paperInfo.load(docXml, nodeObj)) { return; }
        }

        private void sortCollection()
        {
            sortCollectionAux(m_headers);
            sortCollectionAux(m_details);
            sortCollectionAux(m_footers);
            sortCollectionAux(m_groupsFooters);
            sortCollectionAux(m_groupsHeaders);
        }

        private void sortCollectionAux(cReportSections col)
        {
            cReportSection sec = null;
            cReportSectionLine secLn = null;

            for (int _i = 0; _i < col.count(); _i++)
            {
                sec = col.item(_i);
                for (int _j = 0; _j < sec.getSectionLines().count(); _j++)
                {
                    secLn = sec.getSectionLines().item(_j);
                    secLn.setControls(getControlsInZOrder(secLn.getControls()));
                }
            }
        }

        private bool loadAux(CSXml.cXml docXml, cReportSections sections, String keySection)
        {
            XmlNode nodeObj = null;
            XmlNode nodeObjAux = null;
            XmlNode nodeObjSec = null;

            nodeObj = docXml.getRootNode();
            nodeObj = docXml.getNodeFromNode(nodeObj, keySection);

            if (docXml.nodeHasChild(nodeObj))
            {
                nodeObjSec = docXml.getNodeChild(nodeObj);

                while (nodeObjSec != null)
                {
                    nodeObjAux = nodeObjSec;
                    String key = docXml.getNodeProperty(nodeObjAux, "Key").getValueString(eTypes.eText);
                    cReportSection sec = sections.add(null, key);
                    if (!sec.load(docXml, nodeObjAux)) 
                    { 
                        return false; 
                    }
                    nodeObjSec = docXml.getNextNode(nodeObjSec);
                }
            }
            return true;
        }

        private bool loadFormulas(CSXml.cXml docXml)
        {
            XmlNode nodeObj = null;
            XmlNode nodeObjAux = null;
            XmlNode nodeObjSec = null;

            nodeObj = docXml.getRootNode();
            nodeObj = docXml.getNodeFromNode(nodeObj, C_NODERPTFORMULAS);

            if (docXml.nodeHasChild(nodeObj))
            {
                nodeObjSec = docXml.getNodeChild(nodeObj);
                while (nodeObjSec != null)
                {
                    nodeObjAux = nodeObjSec;
                    String name = docXml.getNodeProperty(nodeObjAux, "Name").getValueString(eTypes.eText);
                    cReportFormula formula = m_formulas.add(name);
                    if (!formula.load(docXml, nodeObjAux)) 
                    { 
                        return false; 
                    }
                    nodeObjSec = docXml.getNextNode(nodeObjSec);
                }
            }
            return true;
        }

        private bool loadConnect(CSXml.cXml docXml)
        {
            XmlNode nodeObj = docXml.getRootNode();
            nodeObj = docXml.getNodeFromNode(nodeObj, C_RPTCONNECT);
            return m_connect.load(docXml, nodeObj);
        }

        private bool loadConnectsAux(CSXml.cXml docXml)
        {
            XmlNode nodeObj = docXml.getRootNode();
            nodeObj = docXml.getNodeFromNode(nodeObj, C_RPTCONNECTSAUX);
            return m_connectsAux.load(docXml, nodeObj);
        }

        private bool loadGroups(CSXml.cXml docXml)
        {
            XmlNode nodeObj = null;
            XmlNode nodeObjAux = null;
            XmlNode nodeObjGroup = null;

            nodeObj = docXml.getRootNode();
            nodeObj = docXml.getNodeFromNode(nodeObj, C_NODEGROUPS);

            if (docXml.nodeHasChild(nodeObj))
            {
                nodeObjGroup = docXml.getNodeChild(nodeObj);
                while (nodeObjGroup != null)
                {
                    nodeObjAux = nodeObjGroup;
                    String key = docXml.getNodeProperty(nodeObjAux, "Key").getValueString(eTypes.eText);
                    cReportGroup group = getGroups().add(null, key);
                    if (!group.load(docXml, nodeObjAux)) 
                    { 
                        return false; 
                    }
                    nodeObjGroup = docXml.getNextNode(nodeObjGroup);
                }
            }
            return true;
        }

        private bool loadLaunchInfo(CSXml.cXml docXml)
        {
            XmlNode nodeObj = docXml.getRootNode();
            nodeObj = docXml.getNodeFromNode(nodeObj, C_LAUNCHINFO);
            return m_launchInfo.load(docXml, nodeObj);
        }

        private String getFileName(String fileNameWithExt) {
            return CSKernelFile.cFile.getFileWithoutExt(fileNameWithExt);
        }

        private bool nLoadData(CSXml.cXml docXml)
        {
            XmlNode nodeObj = null;
            XmlNode nodeObjAux = null;
            XmlNode nodeObjSec = null;

            m_pages.clear();
            nodeObj = docXml.getRootNode();
            nodeObj = docXml.getNodeFromNode(nodeObj, C_NODERPTPAGES);

            if (docXml.nodeHasChild(nodeObj))
            {
                nodeObjSec = docXml.getNodeChild(nodeObj);
                while (nodeObjSec != null)
                {
                    nodeObjAux = nodeObjSec;
                    cReportPage page = m_pages.add(null);
                    if (!page.load(docXml, nodeObjAux)) 
                    { 
                        return false; 
                    }
                    nodeObjSec = docXml.getNextNode(nodeObjSec);
                }
            }
            return true;
        }

        protected virtual void OnReportDone()
        {
            if (ReportDone != null)
            {
                ReportDone(this, new EventArgs());
            }
        }

        protected virtual bool OnProgress(String task)
        {
            return OnProgress(task, 0, 0, 0);
        }
        protected virtual bool OnProgress(String task, int page, int currRecord, int recordCount)
        {
            bool cancel = false;
            if (Progress != null)
            {
                ProgressEventArgs e = new ProgressEventArgs(task, page, currRecord, recordCount);
                Progress(this, e);
                cancel = e.cancel;
            }
            return !cancel;
        }

        private bool resumeDBAccessMissing(String connectString, bool saveInReport, CSDataBase.cDataBase cn)
        { // TODO: Use of ByRef founded Private Function ResumeDBAccessMissing(ByVal StrConnect As String, ByVal SaveInReport As Boolean, ByRef cn As cDataBase) As Boolean
            try
            {
                // if the database is not access we do nothing
                //
                if (connectString.ToLower().IndexOf("PROVIDER=Microsoft.Jet.OLEDB.4.0;".ToLower()) == 0)
                {
                    return false;
                }

                // get the datasource's name
                //
                String fileName = "";
                fileName = getToken(connectString, "Data Source");

                // ask to the user if he wan to search for the database file
                //
                CommonDialog commDialog = null;
                if (FindAccessFile != null)
                {
                    FindAccessFileEventArgs e = new FindAccessFileEventArgs(fileName);
                    FindAccessFile(this, e);
                    if (e.cancel)
                    {
                        return false;
                    }
                    commDialog = e.commonDialog;
                }

                CSKernelFile.cFile file = new CSKernelFile.cFile();

                file.filter = "Access files|*.mdb";
                file.init("ResumeDBAccessMissing", C_MODULE, commDialog);

                if (!file.open(m_pathDefault + "\\" + file,
                                CSKernelClient.eFileMode.eRead,
                                false,
                                false,
                                eFileAccess.eShared,
                                true,
                                true))
                {
                    return false;
                }

                fileName = file.fullName;

                file.close();

                connectString = "PROVIDER=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileName;

                if (!cn.initDb(connectString))
                {
                    return false;
                }

                // save the new location 
                //
                if (saveInReport)
                {
                    m_connect.setStrConnect(connectString);
                }
                return true;

            }
            catch (Exception ex)
            {
                cError.mngError(ex, "ResumeDBAccessMissing", C_MODULE, "");
                return false;
            }
        }

        private String getToken(String source, String token)
        {
            token = token.Trim();
            if (token.Substring(token.Length - 1) != "=") 
            { 
                token = token + "="; 
            }
            int p = source.IndexOf(token);
            if (p < 0) 
            { 
                return ""; 
            }
            int p2 = p2 = p + 1;
            p = source.IndexOf(";", p2);
            if (p < 0) 
            { 
                p = source.Length + 1; 
            }
            p2 = p2 + token.Length - 1;
            p = p - p2;
            return source.Substring(p2, p);
        }

        private void pSortControlsByLeft()
        {
            pSortControlsByLeftAux1(m_headers);
            pSortControlsByLeftAux1(m_groupsHeaders);
            pSortControlsByLeftAux1(m_details);
            pSortControlsByLeftAux1(m_groupsFooters);
            pSortControlsByLeftAux1(m_footers);
        }

        private void pSortControlsByLeftAux1(cReportSections sections)
        {
            cReportSection sec = null;
            cReportSectionLine secLn = null;

            for (int _i = 0; _i < sections.count(); _i++)
            {
                sec = sections.item(_i);
                for (int _j = 0; _j < sec.getSectionLines().count(); _j++)
                {
                    secLn = sec.getSectionLines().item(_j);
                    secLn.getControls().orderCollByLeft();
                }
            }
        }
        // public functions
        public void Dispose()
        {
            m_rows = null;
            m_collRows = null;
            m_vRowsIndexAux = null;
            m_vGroups = null;
            m_vRowsIndex = null;
            m_lastRowPreEvalued = null;
            m_lastRowPostEvalued = null;

            m_controls.clear();
            m_controls = null;

            pDestroyCrossRef(m_headers);
            pDestroyCrossRef(m_details);
            pDestroyCrossRef(m_footers);
            pDestroyCrossRef(m_groups.getGroupsHeaders());
            pDestroyCrossRef(m_groups.getGroupsFooters());

            m_headers.clear();
            m_details.clear();
            m_footers.clear();
            m_groupsHeaders.clear();
            m_groupsFooters.clear();

            m_details.setCopyColl(null);
            m_headers.setCopyColl(null);
            m_footers.setCopyColl(null);
            m_groupsHeaders.setCopyColl(null);
            m_groupsFooters.setCopyColl(null);

            m_headers = null;
            m_details = null;
            m_footers = null;
            m_groupsHeaders = null;
            m_groupsFooters = null;

            m_paperInfo = null;

            m_formulas.clear();
            m_formulas = null;

            m_formulaTypes.clear();
            m_formulaTypes = null;

            m_connect = null;

            m_pages.clear();
            m_pages = null;

            m_pageSetting.clear();
            m_pageSetting = null;

            m_compiler = null;
            m_launchInfo = null;

            m_connectsAux.clear();
            m_connectsAux = null;

            pDestroyImages();
            m_images = null;
        }

        private void pDestroyCrossRef(cReportSections secs)
        {
            cReportSection sec = null;
            cReportSectionLine secl = null;

            for (int _i = 0; _i < secs.count(); _i++)
            {
                sec = secs.item(_i);
                for (int _j = 0; _j < sec.getSectionLines().count(); _j++)
                {
                    secl = sec.getSectionLines().item(_j);
                    secl.getControls().setSectionLine(null);

                    if (secl.getControls().getCopyColl() != null)
                    {
                        secl.getControls().getCopyColl().clear();
                    }
                    secl.getControls().setCopyColl(null);
                    secl.getControls().clear();
                    secl.setControls(null);
                }
                sec.setCopyColl(null);
            }
            secs.setCopyColl(null);
        }

        private String pGetMainDataSource(List<object[]> recordsets)
        {
            if (recordsets.Count > 0)
            {
                return (String)recordsets[0][1];
            }
            else 
            {
                return "";
            }
        }

        private void pSetIndexColInGroupFormulas(List<object[]> recordsets)
        {
            pSetIndexColInGroupFormulasAux(m_headers, recordsets);
            pSetIndexColInGroupFormulasAux(m_groupsHeaders, recordsets);
            pSetIndexColInGroupFormulasAux(m_groupsFooters, recordsets);
            pSetIndexColInGroupFormulasAux(m_details, recordsets);
            pSetIndexColInGroupFormulasAux(m_footers, recordsets);
        }

        private void pSetIndexColInGroupFormulasAux(cReportSections sections, List<object[]> recordsets)
        {
            cReportSection sec = null;
            cReportSectionLine secLn = null;
            cReportControl ctrl = null;

            for (int _i = 0; _i < sections.count(); _i++)
            {
                sec = sections.item(_i);
                if (sec.getHasFormulaHide())
                {
                    pSetIndexColInGroupFormula(sec.getFormulaHide(), recordsets);
                }
                for (int _j = 0; _j < sec.getSectionLines().count(); _j++)
                {
                    secLn = sec.getSectionLines().item(_j);
                    if (secLn.getHasFormulaHide())
                    {
                        pSetIndexColInGroupFormula(secLn.getFormulaHide(), recordsets);
                    }
                    for (int _k = 0; _k < secLn.getControls().count(); _k++)
                    {
                        ctrl = secLn.getControls().item(_k);
                        if (ctrl.getHasFormulaHide())
                        {
                            pSetIndexColInGroupFormula(ctrl.getFormulaHide(), recordsets);
                        }
                        if (ctrl.getHasFormulaValue())
                        {
                            pSetIndexColInGroupFormula(ctrl.getFormulaValue(), recordsets);
                        }
                    }
                }
            }
        }

        private void pSetIndexColInGroupFormula(cReportFormula formula, List<object[]> recordsets)
        {
            cReportFormulaInt fint = null;
            String colName = "";
            DataTable rs = null;

            if (!m_reportDisconnected)
            {
                rs = (DataTable)recordsets[0][0];

                for (int _i = 0; _i < formula.getFormulasInt().count(); _i++)
                {
                    fint = formula.getFormulasInt().item(_i);

                    if (pIsGroupFormula((int)fint.getFormulaType()))
                    {
                        colName = fint.getParameters().item(0).getValue();
                        pSetColIndexInGroupFormulaAux(rs, fint, colName, cReportGlobals.C_KEYINDEXCOL);

                        if (fint.getFormulaType() == csRptFormulaType.CSRPTGROUPPERCENT)
                        {
                            colName = fint.getParameters().item(1).getValue();
                            pSetColIndexInGroupFormulaAux(rs, fint, colName, cReportGlobals.C_KEYINDEXCOL2);
                        }
                    }
                }
            }
        }

        private void pSetColIndexInGroupFormulaAux(
            DataTable rs, 
            cReportFormulaInt fint, 
            String colName, 
            String keyParam)
        {
            for (int i = 0; i < rs.Columns.Count; i++)
            {
                if (colName.ToLower() == rs.Columns[i].ColumnName.ToLower())
                {
                    if (fint.getParameters().item(keyParam) == null)
                    {
                        fint.getParameters().add2("", keyParam);
                    }
                    fint.getParameters().item(keyParam).setValue(i.ToString());
                    break;
                }
            }
        }

        private void redimPreserve(ref T_Group[] groups, int size)
        {
            if (size == 0)
            {
                groups = null;
            }
            else
            {
                if (groups == null)
                {
                    groups = new T_Group[size];
                }
                else if (groups.Length == 0)
                {
                    groups = new T_Group[size];
                }
                else
                {
                    T_Group[] newArray = new T_Group[size];
                    Array.Copy(groups, newArray, groups.Length);
                    groups = newArray;
                }
            }
        }

        private cReportControls getControlsInZOrder(cReportControls col)
        {
            int i = 0;
            cReportControl ctrl = null;
            cReportControls ctrls = null;

            ctrls = new cReportControls();
            ctrls.setCopyColl(col.getCopyColl());
            ctrls.setTypeSection(col.getTypeSection());
            ctrls.setSectionLine(col.getSectionLine());

            // we load a new collection ordered by zorder
            //
            while (col.count() > 0)
            {
                // we search the lower zorder in this collection
                //
                i = 32767;
                for (int _i = 0; _i < col.count(); _i++)
                {
                    ctrl = col.item(_i);
                    if (ctrl.getLabel().getAspect().getNZOrder() < i)
                    {
                        i = ctrl.getLabel().getAspect().getNZOrder();
                    }
                }

                for (int _i = 0; _i < col.count(); _i++)
                {
                    ctrl = col.item(_i);
                    if (ctrl.getLabel().getAspect().getNZOrder() == i)
                    {
                        col.remove(ctrl.getKey());
                        ctrls.add(ctrl, ctrl.getKey());
                        break;
                    }
                }
                i = i + 1;
            }
            return ctrls;
        }

        private void reportDone()
        {
            ReportDone.Invoke(this, null);
        }
    
    }
}
