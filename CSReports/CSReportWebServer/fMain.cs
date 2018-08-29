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
using System.Collections.Generic;
using System.Net;
using System.IO;

namespace CSReportWebServer
{
    public partial class fMain : Form
    {
        private string[] m_args;

        private Dictionary<string, Report> m_reports = new Dictionary<string, Report>();

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

        delegate void ReportActionCallback(JObject request);

        private void safePreview(JObject request)
        {
            var fileName = request["message"]["data"]["file"];
            var reportType = request["message"]["data"]["type"].ToString();
            var url = request["message"]["data"]["url"].ToString();
            var pathAndFile = Path.GetTempPath() + fileName;
            getReportFromWebServer(url + reportType + "/" + fileName, pathAndFile);
                        
            var report = new Report();
            report.init(request, this.printDlg);
            if (report.openDocument(pathAndFile))
            {
                report.preview();
            }
            if (m_reports.ContainsKey(report.reportId))
            {
                m_reports[report.reportId] = report;
            }
            else
            {
                m_reports.Add(report.reportId, report);
            }
        }

        private void safePrint(JObject request)
        {
            var fileName = request["message"]["data"]["file"];
            var reportType = request["message"]["data"]["type"].ToString();
            var url = request["message"]["data"]["url"].ToString();
            var pathAndFile = Path.GetTempPath() + fileName;
            getReportFromWebServer(url + reportType + "/" + fileName, pathAndFile);

            var report = new Report();
            report.init(request, this.printDlg);
            if (report.openDocument(pathAndFile))
            {
                report.printReport();
            }
        }

        private void safeMoveToPage(JObject request)
        {
            var data = request["message"]["data"];
            var reportId = data["reportId"].ToString();
            var page =  int.Parse(data["page"].ToString());
            var report = m_reports[reportId];
            report.moveToPage(page);
        }

        public void preview(JObject request)
        {
            ReportActionCallback d = new ReportActionCallback(safePreview);
            this.Invoke(d, new object[] { request });
        }

        public void printReport(JObject request)
        {
            ReportActionCallback d = new ReportActionCallback(safePrint);
            this.Invoke(d, new object[] { request });
        }

        public void moveToPage(JObject request)
        {
            ReportActionCallback d = new ReportActionCallback(safeMoveToPage);
            this.Invoke(d, new object[] { request });
        }

        private void fMain_Load(object sender, EventArgs e)
        {
            Main.Init(m_args, this);
        }

        private string DownloadString(string address)
        {
            string text;
            using (var client = new WebClient())
            {
                text = client.DownloadString(address);
            }
            return text;
        }

        private void getReportFromWebServer(string url, string fileName)
        {
            var xml = DownloadString(url);
            File.WriteAllText(fileName, xml);
        }
    }
}
