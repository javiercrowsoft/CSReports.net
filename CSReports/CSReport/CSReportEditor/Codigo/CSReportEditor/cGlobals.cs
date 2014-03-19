﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSKernelClient;
using CSReportDll;
using CSReportGlobals;

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

        public static void setStatus()
        { 
        
        }

        public static bool isNumberField(int fieldType)
        {
            return false;
        }

    }

    public enum CSRptEditorMoveType {
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
        CSRPTEDITNONE,
        CSRPTEDITLABEL,
        CSRPTEDITFIELD,
        CSRPTEDITFORMULA,
        CSRPTEDITIMAGE,
        CSRPTEDITCHART
    }

    public enum csESectionLineTypes
    {
        C_KEY_SECLN_HEADER = 1000,
        C_KEY_SECLN_DETAIL = 1001,
        C_KEY_SECLN_FOOTER = 1002,
        C_KEY_SECLN_GROUPH = 1003,
        C_KEY_SECLN_GROUPF = 1004
    }

}
