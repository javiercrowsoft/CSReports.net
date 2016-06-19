using System;
using System.IO;
using System.Windows.Forms;
using log4net;
using Newtonsoft.Json.Linq;

namespace CSReportWebServer
{
    public partial class fMain : Form
    {
        private string[] m_args;
		private static ILog m_log = LogManager.GetLogger(typeof(Host));

        public fMain(string[] args)
        {
            InitializeComponent();
            m_args = args;
        }

		private string loadTestRequest()
		{ 
		    // Open the text file using a stream reader.
			using (StreamReader sr = new StreamReader("/Users/javier/Work/Temp/request.json"))
			{
				// Read the stream to a string, and write the string to the console.
				return sr.ReadToEnd();
			}
		}

        private void cmdRegister_Click(object sender, EventArgs e)
        {
			//Main.RegisterNativeMessagingHost(new string[] { "register" });
			string r = loadTestRequest();
			safePreview(JObject.Parse(r));
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
			m_log.Info("in safePreview 01");

            //var pathAndFile = @"\\vmware-host\Shared Folders\Documents\CrowSoft\Reportes\temp\" + request["message"]["data"]["file"];
			var pathAndFile = @"/Users/javier/Documents/CrowSoft/Reportes/temp/" + request["message"]["data"]["file"];
            var report = new Report();

			m_log.Info("in safePreview 02");

            report.init(request);

			m_log.Info("in safePreview 03");

            if (report.openDocument(pathAndFile))
            {
				m_log.Info("in safePreview 04");

                report.preview();
            }

			m_log.Info("in safePreview 05");
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
