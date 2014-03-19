using System;


namespace CSKernelClient
{


    public static class cError
    {
        private static String m_lastErrorDescription = "";
        private static String m_lastErrorInfoAdd = "";
        private static String m_lastErrorModule = "";
        private static String m_lastErrorNumber = "";
        private static String m_lastErrorLine = "";
        private static String m_lastErrorFunction = "";
        private static Boolean m_silent = false;

        public static void mngError(Exception ex,
                             string function,
                             string module,
                             string infoAdd)
        {
            mngError(ex, function, module, infoAdd, "", eErrorLevel.eErrorWarning, eErrorType.eErrorVba, null);
        }

        public static void mngError(Exception ex,
                             string function,
                             string module,
                             string infoAdd,
                             string title,
                             eErrorLevel level,
                             eErrorType varType,
                             object connection)
        {
            // TODO: implement function
            fErrors f = new fErrors();
            f.setErrorIcon();
            f.setDetails(ex.Message);
            f.ShowDialog();
        }

        public static String getLastErrorDescription() 
        {
            return m_lastErrorDescription;
        }

        public static String getLastErrorInfoAdd() 
        {
            return m_lastErrorInfoAdd;
        }

        public static String getLastErrorModule() 
        {
            return m_lastErrorModule;
        }

        public static String getLastErrorNumber() 
        {
            return m_lastErrorNumber;
        }

        public static String getLastErrorLine() 
        {
            return m_lastErrorLine;
        }

        public static String getLastErrorFunction() 
        {
            return m_lastErrorFunction;
        }

        public static void setSilent(bool rhs)
        {
            m_silent = rhs;
        }
    }
}
