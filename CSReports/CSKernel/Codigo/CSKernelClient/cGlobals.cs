using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSKernelClient
{
    public static class cGlobals
    {
        public static String gAppName = "";
        public static String gAppPath = "";
        public static String gDefaultHelpFile = "";

        public static String gErrorDB = "";

        // to send emails with errors to crowsoft
        //
        public static String gEmailServer = "";
        public static String gEmailAddress = "";
        public static int gEmailPort = 0;
        public static String gEmailUser = "";
        public static String gEmailPwd = "";

        public static String gEmailErrDescrip = "";

        public static bool G_FormResult;
        
        // used for fEditar to return the result.
        public static String G_InputValue = "";

        public static bool gNoChangeMouseCursor;
    }
}
