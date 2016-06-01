using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CSReportWebServer
{
    public partial class fMain : Form
    {
        public fMain(string[] args)
        {
            InitializeComponent();
            Main.Init(args, this);
        }

        private void cmdRegister_Click(object sender, EventArgs e)
        {
            Main.RegisterNativeMessagingHost(new string[] { "register" });
        }

        public void log(String message)
        {
            var i = lvLog.Items.Add(DateTime.Now.ToString("h:mm:ss tt"));
            i.SubItems.Add(message);
        }        
    }
}
