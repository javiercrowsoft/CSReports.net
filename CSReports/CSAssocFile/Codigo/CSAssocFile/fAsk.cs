using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CSAssocFile
{
    public partial class fAsk : Form
    {

        private bool m_result = false;

        public fAsk()
        {
            InitializeComponent();
        }

        public bool result
        {
            get
            {
                return m_result;
            }
        }

        public String question
        {
            set
            {
                lbQuestion.Text = value;
            }
        }

        public String yesButton
        {
            set
            {
                cmdYes.Text = value;
            }
        }

        public String noButton
        {
            set
            {
                cmdNo.Text = value;
            }
        }

        public String dontAsk
        {
            set
            {
                chkDontAskAgain.Text = value;
            }
        }

        public bool dontAskAgain
        {
            get
            {
                return chkDontAskAgain.Checked;
            }
        }

        private void cmdYes_Click(object sender, EventArgs e)
        {
            m_result = true;
            Hide();
        }

        private void cmdNo_Click(object sender, EventArgs e)
        {
            m_result = false;
            Hide();
        }
    }
}
