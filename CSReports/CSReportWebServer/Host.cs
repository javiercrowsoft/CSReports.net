using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CSReportWebServer.NativeMessaging;

namespace CSReportWebServer
{
    /// <summary>
    /// Native Messaging Host.
    /// </summary>
    public class Host
    {
        private static ILog log = LogManager.GetLogger(typeof(Host));

        private ManualResetEvent stop;
        private Port port;

        private fMain m_f;

        /// <summary>
        /// Creates a new instance of native messaging host.
        /// </summary>
        public Host(fMain f)
        {
            port = new Port();
            stop = new ManualResetEvent(false);
            m_f = f;
        }

        /// <summary>
        /// Starts native message processing.
        /// </summary>
        public void Run()
        {
            log.Info("host started 0.0.0.1");
            m_f.log("host started");

            throw new InvalidOperationException();

            stop.Reset();
            while (!stop.WaitOne(0))
            {
                try
                {
                    string message = port.Read();
                    log.DebugFormat("request message\n{0}", message);
                    m_f.log("request message " + message);
                    JObject request = JObject.Parse(message);
                    JObject reply = new JObject();
                    if (request["source"] != null) reply["source"] = request["destination"];
                    if (request["destination"] != null) reply["destination"] = request["source"];
                    reply["request"] = request;
                    reply["extension"] = "CSReportWebServer.Echo";
                    message = reply.ToString(Formatting.None);

                    previewReport();

                    log.DebugFormat("reply message\n{0}", message);
                    m_f.log(message);
                    port.Write(message);
                }
                catch (EndOfInputStreamException)
                {
                    log.Debug("end of input stream");
                    stop.Set();
                }
                catch (Exception ex)
                {
                    log.Error("message processing caused an exception", ex);
                    stop.Set();
                    throw ex;
                }
            }

            log.Info("host stopped");
            m_f.log("host stopped");
            m_f.Close();
        }

        /// <summary>
        /// Stops native message processing.
        /// </summary>
        public void Stop()
        {
            stop.Set();
        }

        private void previewReport()
        {
            var pathAndFile = @"\\vmware-host\Shared Folders\Documents\CrowSoft\Reportes\temp\DC_CSC_CON_014016.csr";
            var report = new Report();
            report.init();
            if (report.openDocument(pathAndFile))
            {
                report.preview();
            }
        }
    }
}
