using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CSKernelClient
{
    public static class cWindow
    {
        private static String m_title;

        public static void setTitle(String value) 
        {
            m_title = value;
        }

        public static void msgError(String msg, String title, String details)
        {
            pMsgAux(msg, CSMSGICONS.Error, title, details);
        }

        public static void msgError(String msg, String title)
        {
            if (title == "@@@@@") { title = m_title; }
            msgError(msg, title, "");
        }

        public static void msgError(String msg)
        {
            msgError(msg, "@@@@@", "");
        }

        public static void msgWarning(String msg, String title, String details)
        {
            pMsgAux(msg, CSMSGICONS.Exclamation, title, details);
        }

        public static void msgWarning(String msg, String title)
        {
            if (title == "@@@@@") { title = m_title; }
            msgWarning(msg, title, "");
        }

        public static void msgWarning(String msg)
        {
            msgWarning(msg, "@@@@@", "");
        }

        public static bool ask(String msg, VbMsgBoxResult defaultButton) 
        {
            return ask(msg, defaultButton, "@@@@@");
        }

        public static bool ask(String msg, VbMsgBoxResult defaultButton, String Title) 
        {
            return false;
        }

        public static void msgInfo(String msg)
        {
            msgInfo(msg, "@@@@@");
        }

        public static void msgInfo(String msg, String title)
        {
        }

        private static void pMsgAux(String msg, CSMSGICONS icon, String title, String details) 
        {
            fMsg fmsg = new fMsg();
            fmsg.setIcon(icon);
            fmsg.setMessage(msg);
            fmsg.setTitle(title);
            fmsg.ShowDialog();
        }

        public static void centerForm(Form form) 
        {
            form.Left = (Screen.FromControl(form).Bounds.Width - form.Width) / 2;
            form.Top = (Screen.FromControl(form).Bounds.Height - form.Height) / 2;
        }

        public static void locateFormAtLeft(Form form)
        {
            form.Left = 100;
            form.Top = (Screen.FromControl(form).Bounds.Height - form.Height) / 2;
        }
    }
}
