using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CSKernelClient
{
    public static class cWindow
    {
        private static String m_title = "Message";

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
            msgWarning(msg, title, "");
        }

        public static void msgWarning(String msg)
        {
            msgWarning(msg, "@@@@@", "");
        }

        public static bool ask(String msg, MessageBoxDefaultButton defaultButton) 
        {
            return ask(msg, defaultButton, "@@@@@");
        }

        public static bool ask(String msg, MessageBoxDefaultButton defaultButton, String Title) 
        {
            return MessageBox.Show(msg, "", MessageBoxButtons.YesNo, MessageBoxIcon.Question, defaultButton) == DialogResult.Yes;
        }

        public static void msgInfo(String msg)
        {
            msgInfo(msg, "@@@@@");
        }

        public static void msgInfo(String msg, String title)
        {
            pMsgAux(msg, CSMSGICONS.Information, title, "");
        }

        private static void pMsgAux(String msg, CSMSGICONS icon, String title, String details) 
        {
            if (title == "@@@@@") { title = m_title; }
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

        public static void locateFormAtTop(Form form)
        {
            var top = (Screen.FromControl(form).Bounds.Height - form.Height) / 2 - 200;
            form.Left = (Screen.FromControl(form).Bounds.Width - form.Width) / 2;
            form.Top = top;
        }
    }
}
