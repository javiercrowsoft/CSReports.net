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
            picIcon.Image = new Bitmap(System.Reflection.Assembly.GetEntryAssembly().GetManifestResourceStream("Status-dialog-error-icon.png"));

        }
        public void setErrorInfo()
        {
            picIcon.Image = new Bitmap(System.Reflection.Assembly.GetEntryAssembly().GetManifestResourceStream("Status-dialog-information-icon.png"));

        }
        public void setErrorWarning()
        {
            picIcon.Image = new Bitmap(System.Reflection.Assembly.GetEntryAssembly().GetManifestResourceStream("Warning-icon.png"));

        }

        private void cmdOk_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
