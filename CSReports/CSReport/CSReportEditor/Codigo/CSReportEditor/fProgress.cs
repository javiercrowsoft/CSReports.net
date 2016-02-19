using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CSKernelClient;

namespace CSReportEditor
{
    public partial class fProgress : Form
    {
        public fProgress()
        {
            InitializeComponent();
        }

        private void fProgress_Load(object sender, EventArgs e)
        {
            cWindow.centerForm(this);
        }
    }
}
