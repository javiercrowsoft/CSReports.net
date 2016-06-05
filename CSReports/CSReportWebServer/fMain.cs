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

        delegate void LogCallback(string message);

        private void safeLog(string message)
        {
            var i = lvLog.Items.Add(DateTime.Now.ToString("h:mm:ss tt"));
            i.SubItems.Add(message);
        }

        public void log(string message)
        {
            LogCallback d = new LogCallback(safeLog);
            this.Invoke(d, new object[] { message });
        }

        delegate void CloseCallback();

        public void close() {
            CloseCallback d = new CloseCallback(this.Close);
            this.Invoke(d);
        }

        delegate void PreviewCallback();

        private void safePreview()
        {
            var pathAndFile = @"\\vmware-host\Shared Folders\Documents\CrowSoft\Reportes\temp\DC_CSC_CON_014016.csr";
            var report = new Report();
            report.init();
            if (report.openDocument(pathAndFile))
            {
                report.preview();
            }
        }

        public void preview()
        {
            PreviewCallback d = new PreviewCallback(safePreview);
            this.Invoke(d);
        }

        private void cmdTest_Click(object sender, EventArgs e)
        {
            preview();
        }
    }
}
