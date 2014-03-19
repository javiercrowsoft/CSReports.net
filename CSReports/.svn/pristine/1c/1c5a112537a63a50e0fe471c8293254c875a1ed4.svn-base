using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Resources;

namespace CSKernelClient
{
    public partial class fErrors : Form
    {
        public fErrors()
        {
            InitializeComponent();
        }

        private void cmdOk_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void cmdDetails_Click(object sender, EventArgs e)
        {
            if (cmdDetails.Text == "Details")
            {
                this.Height = 242;
            }
            else 
            {
                this.Height = 150;
            }
        }

        public void setDetails(string details)
        {
            txError.Text = details;
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
    }
}
