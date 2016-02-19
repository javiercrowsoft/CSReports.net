using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CSKernelClient
{
    public partial class fMsg : Form
    {
        public fMsg()
        {
            InitializeComponent();
        }
        public void setTitle(String value)
        {
            this.Text = value;
        }
        public void setIcon(CSMSGICONS icon)
        {
            switch (icon) {
                case CSMSGICONS.Error:
                    setErrorIcon();
                    break;
                case CSMSGICONS.Exclamation:
                    setErrorWarning();
                    break;
                case CSMSGICONS.Information:
                    setErrorInfo();
                    break;
            }
        }
        public void setMessage(String value)
        {
            this.txMsg.Text = value;
        }
        public void setErrorIcon()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            picIcon.Image = new Bitmap(assembly.GetManifestResourceStream(assembly.GetName().Name + ".Resources.error.png"));
        }
        public void setErrorInfo()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            picIcon.Image = new Bitmap(assembly.GetManifestResourceStream(assembly.GetName().Name + ".Resources.information.png"));
        }
        public void setErrorWarning()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            picIcon.Image = new Bitmap(assembly.GetManifestResourceStream(assembly.GetName().Name + ".Resources.warning.png"));
        }

        private void cmdOk_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void fMsg_Load(object sender, EventArgs e)
        {
            txMsg.SelectionStart = 0;
            txMsg.SelectionLength = 0;

            var height = txMsg.Lines.Length * 20;
            if (height > this.Height - 100)
            {
                this.Height = height + 100;
                txMsg.Height = height;
                cmdOk.Top = this.Height - 80;
            }            

            cWindow.centerForm(this);
        }
    }
}
