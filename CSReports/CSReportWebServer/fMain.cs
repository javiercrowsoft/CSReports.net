using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CSReportWebServer
{
    public partial class fMain : Form
    {
        private string[] m_args;

        public fMain(string[] args)
        {
            InitializeComponent();
            m_args = args;
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

        delegate void PreviewCallback(JObject request);

        private void safePreview(JObject request)
        {
            var pathAndFile = @"\\vmware-host\Shared Folders\Documents\CrowSoft\Reportes\temp\" + request["message"]["data"]["file"];
            var report = new Report();
            report.init(request);
            if (report.openDocument(pathAndFile))
            {
                report.preview();
            }
        }

        public void preview(JObject request)
        {
            PreviewCallback d = new PreviewCallback(safePreview);
            this.Invoke(d, new object[] { request });
        }

        private void fMain_Load(object sender, EventArgs e)
        {
            Main.Init(m_args, this);
        }
    }
}
