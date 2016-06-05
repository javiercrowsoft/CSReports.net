using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CSKernelClient;

namespace CSReportWebServer
{
    public partial class fProgress : Form
    {
        public fProgress()
        {
            InitializeComponent();
        }

        public Label lbTask
        {
            get
            {
                return lb_task;
            }
        }

        public Label lbCurrPage
        {
            get
            {
                return lb_curr_page;
            }
        }

        public Label lbRecordCount
        {
            get
            {
                return lb_record_count;
            }
        }

        public Label lbCurrRecord
        {
            get
            {
                return lb_curr_record;
            }
        }

        public ProgressBar prgBar
        {
            get
            {
                return prg_bar;
            }
        }

        private void fProgress_Load(object sender, EventArgs e)
        {
            cWindow.locateFormAtTop(this);
        }
    }
}
