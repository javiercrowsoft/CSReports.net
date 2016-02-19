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
                cmdDetails.Text = "Hide";
                this.Height = 242;
            }
            else 
            {
                cmdDetails.Text = "Details";
                this.Height = 130;
            }
        }

        public void setDetails(string details)
        {
            txError.Text = details;
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

        private void fErrors_Load(object sender, EventArgs e)
        {
            cWindow.centerForm(this);
        }
    }
}
