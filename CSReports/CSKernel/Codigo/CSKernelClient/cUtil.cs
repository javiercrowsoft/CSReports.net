using System;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

namespace CSKernelClient
{
    public static class cUtil
    {
        private const String C_MODULE = "cUtil";

        private static String m_sepDecimal = "";

        public static string getToken(string token, string source)
        {
            return "";
        }

        public static int tp(int twips) 
        {
            const int nTwipsPerInch = 1440;
            int dpi = getDPI();
            return (twips / nTwipsPerInch) * dpi;
        }

        public static int pt(int pixels) {
            const int nTwipsPerInch = 1440;
            int dpi = getDPI();
            return (pixels / dpi) * nTwipsPerInch;
        }

        public static int mt(int millimeters) {
            const int nTwipsPerInch = 1440;
            return (int)(mi(millimeters) * nTwipsPerInch);
        }

        public static int mp(int millimeters)
        {
            int dpi = getDPI();
            return (int)(mi(millimeters) * dpi);
        }

        public static double mi(double millimeters)
        {
            return (millimeters * .03937);
        }

        private static int getDPI()
        { 
            int currentDPI = 0;
            using (PictureBox pic = new PictureBox())
            {
                using (Graphics g =  pic.CreateGraphics())
                {
                    currentDPI = (int)g.DpiX;
                }
            }
            return currentDPI;
        }

        public static void setEmailServer(String rhs) {
            cGlobals.gEmailServer = rhs;
        }
        public static void setEmailAddress(String rhs)
        {
            cGlobals.gEmailAddress = rhs;
        }
        public static void setEmailPort(int rhs)
        {
            cGlobals.gEmailPort = rhs;
        }
        public static void setEmailUser(String rhs)
        {
            cGlobals.gEmailUser = rhs;
        }
        public static void setEmailPwd(String rhs)
        {
            cGlobals.gEmailPwd = rhs;
        }

        public static void setEmailErrDescrip(String rhs)
        {
            cGlobals.gEmailErrDescrip = rhs;
        }

        public static String getEmailServer()
        {
            return cGlobals.gEmailServer;
        }
        public static String getEmailAddress()
        {
            return cGlobals.gEmailAddress;
        }
        public static int getEmailPort()
        {
            return cGlobals.gEmailPort;
        }
        public static String getEmailUser()
        {
            return cGlobals.gEmailUser;
        }
        public static String getEmailPwd()
        {
            return cGlobals.gEmailPwd;
        }

        public static String getErrorDB()
        {
            return cGlobals.gErrorDB;
        }
        public static void setErrorDB(String rhs)
        {
            cGlobals.gErrorDB = rhs;
        }

        public static String arrayToString(int[] v) {
            int i = 0;
            String s = "";
            for (i = 0; i <= v.Length; i++)
            {
                s = s + v[i].ToString() + ",";
            }
            return removeLastColon(s);
        }

        public static String arrayToString(String[] v)
        {
            int i = 0;
            String s = "";
            for (i = 0; i <= v.Length; i++)
            {
                s = s + v[i] + ",";
            }
            return removeLastColon(s);
        }

        public static bool existsFile(String pathYName)
        {
            return File.Exists(pathYName);
        }

        public static String getGetToken(String token, String source)
        {
            int i = 0;
            String s = "";
            String c = "";
            int l = 0;

            if (token.Substring(token.Length - 1) != "=") 
            { 
                token = token + "="; 
            }

            l = source.Length;
            i = source.ToLower().IndexOf(token.ToLower(), 1);
            if (i == 0) { return ""; }
            i = i + token.Length - 1;
            
            do
            {
                i = i + 1;
                if (i > l) 
                { 
                    break; 
                }
                c = source.Substring(i, 1);
                if (c != ";")
                {
                    s = s + c;
                }
                else
                {
                    break;
                }
            } while (true);

            return s;
        }

        public static void setSepDecimal()
        {
            float n;
            float.TryParse("1.000", out n);
            if (n == 1)
            {
                m_sepDecimal = ".";
            }
            else
            {
                float.TryParse("1,000", out n);
                if (n == 1)
                {
                    m_sepDecimal = ",";
                }
            }
            if (m_sepDecimal == "") 
            {
                throw new KernelException(
                    C_MODULE,
                    "The decimal symbol could not be determined." 
                    + "Check in 'control panel/reginal settings' "
                    + "that the values in the Numbers tab match the values "
                    + "in the Currency tab for field 'decimal symbol' and "
                    + "'Digit grouping symbol'. ");
            }
        }
        
        public static String getSepDecimal()
        {
            return m_sepDecimal;
        }

        public static String getValidPath(String path) {
            if (path.Substring(path.Length - 1) != "\\") { path = path + "\\"; }
            return path;
        }

        //--------------------------------------------------------------------------------------------------------------------
        public static void listAdd(ComboBox list, String value)
        {
            listAdd_(list, value);
        }
        public static void listAdd(ComboBox list, String value, int id)
        {
            listAdd_(list, value, id);
        }
        public static long listID(ComboBox list)
        {
            return listID_(list);
        }
        public static long listItemData(ComboBox list, int index)
        {
            return listItemData_(list, index);
        }
        public static void listSetListIndex(ComboBox list, int idx)
        {
            listSetListIndex_(list, idx);
        }
        public static void listSetListIndexForId(ComboBox list, int id)
        {
            listSetListIndexForId_(list, id);
        }
        public static void listSetListIndexForText(ComboBox list, String text)
        {
            listSetListIndexForText_(list, text);
        }
        public static void listChangeTextForSelected(ComboBox list, String value)
        {
            listChangeTextForSelected_(list, value);
        }
        public static void listChangeText(ComboBox list, int idx, String value)
        {
            listChangeText_(list, idx, value);
        }
        public static int listGetIndexFromItemData(ComboBox list, int valueItemData)
        {
            return listGetIndexFromItemData_(list, valueItemData);
        }

        private static void listAdd_(ComboBox list, String value)
        {
            list.Items.Add(value);
        }
        private static void listAdd_(ComboBox list, String value, int id)
        {
            list.Items.Add(new ListValueWithId(value, id));
        }
        private static long listID_(ComboBox list)
        {
            if (list.SelectedIndex == -1) { return 0; }
            return ((ListValueWithId)list.SelectedItem).Id;
        }
        private static long listItemData_(ComboBox list, int index)
        {
            long _rtn = 0;
            
            if (index < list.Items.Count)
            {
                if (index == -1)
                {
                    _rtn = listID_(list);
                }
                else
                {
                    _rtn = ((ListValueWithId)list.Items[index]).Id;
                }
            }
            return _rtn;
        }
        private static void listSetListIndex_(ComboBox list, int idx)
        {
            if (list.Items.Count < 1) { return; }
            if (list.Items.Count > idx) { list.SelectedIndex = idx; }
        }
        private static void listSetListIndexForId_(ComboBox list, int id)
        {
            int i = 0;
            for (i = 0; i < list.Items.Count; i++)
            {
                if (((ListValueWithId)list.Items[i]).Id == id)
                {
                    list.SelectedIndex = i;
                    break;
                }
            }
        }
        private static void listSetListIndexForText_(ComboBox list, String text)
        {
            int i = 0;
            for (i = 0; i <= list.Items.Count; i++)
            {
                if (list.Items[i].ToString() == text)
                {
                    list.SelectedIndex = i;
                    break;
                }
            }
        }
        private static void listChangeTextForSelected_(ComboBox list, String value)
        {
            listChangeText_(list, list.SelectedIndex, value);
        }
        private static void listChangeText_(ComboBox list, int idx, String value)
        {
            if (idx < list.Items.Count && idx > -1)
            {
                object item = list.Items[idx];
                if (item is ListValueWithId)
                {
                    ((ListValueWithId)item).Text = value;
                }
                else
                {
                    list.Items[idx] = value;
                }
            }
        }
        private static int listGetIndexFromItemData_(ComboBox list, int valueItemData)
        {
            for (int i = 0; i < list.Items.Count; i++)
            {

                if (list.Items[i] is ListValueWithId && ((ListValueWithId)list.Items[i]).Id == valueItemData)
                {
                    return i;
                }
            }

            return -1;
        }

        /* 
        //--------------------------------------------------------------------------------------------------------------------
        public void setNodeForId(Object tree, int id) { // TODO: Use of ByRef founded Public Sub SetNodeForId(ByRef Tree As Object, ByVal Id As Long)
            mUtil.setNodeForId_(tree, id);
        }
        //--------------------------------------------------------------------------------------------------------------------
        public bool getPropertyFromParent(Object retValue, Object o, String propiedad) { // TODO: Use of ByRef founded Public Function GetPropertyFromParent(ByRef retValue As Variant, ByVal o As Object, ByVal propiedad As String) As Boolean
            return mUtil.getPropertyFromParent_(retValue, o, propiedad);
        }
        public bool getWindowState(Object retValue, Object o) { // TODO: Use of ByRef founded Public Function GetWindowState(ByRef retValue As Variant, ByVal o As Object) As Boolean
            return mUtil.getWindowState_(retValue, o);
        }
        
        //--------------------------------------------------------------------------------------------------------------------
        public String setInfoString(String fuente, String clave, String value) {
            return mUtil.setInfoString_(fuente, clave, value);
        }
        public String getInfoString(String fuente, String clave, String defaultValue) {
            return mUtil.getInfoString_(fuente, clave, defaultValue);
        }
        */          
        //--------------------------------------------------------------------------------------------------------------------
        public static bool getInput(out string value, String descrip) {
            // TODO: implement
			value = "";
            return false;
        }
        /*
        public bool getInputEx(String value, String descrip) { // TODO: Use of ByRef founded Public Function GetInputEx(ByRef Value As String, Optional ByVal Descrip As String) As Boolean
            return mUtil.getInputEx_(value, descrip);
        }
         */ 
        public static String removeLastColon(String list) {
            list = list.Trim();
            if (list.Substring(list.Length - 1) == ",")
            {
                return list.Substring(1, list.Length - 1);
            }
            else
            {
                return list;
            }
        }
        /*
        public void sleep(int dwMilliseconds) {
            SubSleep(dwMilliseconds);
        }
        */
        public static void setFocusControl(Control ctl)
        {
            ctl.Select();
        }

        public static String getComputerName()
        {
            return System.Environment.MachineName;
        }

        /*
        public void showHelp(int hwnd, String helpFileFullName, String helpFile, int contextId) {
            mUtil.showHelp_(hwnd, helpFileFullName, helpFile, contextId);
        }

        public void sendEmailToCrowSoft(String subject, String body) {
            String text = "";

            if (!getInputEx(text, "Write your comments")) { return; }

            body = text + "\\n" + "\\n" + body + "\\n" + "\\n" + "Send by " + cGlobals.gEmailErrDescrip;
            mUtil.sendEmailToCrowSoft_(subject, body);
        }
        */
        public static double divideByZero(double x1, double x2)
        {
            if (x2 != 0)
            {
                return x1 / x2;
            }
            else
            {
                return 0;
            }
        }
    }

    public class ListValueWithId {
        private string value;
        private int id;

        public ListValueWithId(string value, int id) 
        {
            this.value = value;
            this.id = id;
        }

        public override string ToString()
        {
            return value;
        }

        public int Id 
        {
            get
            {
                return id;
            }
        }

        public string Text
        {
            set
            {
                Text = value;
            }
        }
    }
}