using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CSMaskEdit
{
    public partial class cMaskEdit : UserControl
    {
        public cMaskEdit()
        {
            InitializeComponent();
        }

        private void cMaskEdit_SizeChanged(object sender, EventArgs e)
        {
            try
            {
                cmdButton.Left = this.ClientSize.Width - cmdButton.Width;
                cmdButton.Height = this.ClientSize.Height;
                txText.Width = this.ClientSize.Width - cmdButton.Width;
                txText.Height = this.ClientSize.Height;
            }
            catch
            { }
        }

    }
}
