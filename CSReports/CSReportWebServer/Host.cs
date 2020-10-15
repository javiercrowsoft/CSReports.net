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
        private SizeQueue<JObject> m_messageQueue;
        private Dictionary<String, String> m_partialMessages = new Dictionary<string, string>();

        private const string C_EXTENSION_NAME = "CSReportWebServer.Echo";

        /// <summary>
        /// Creates a new instance of native messaging host.
        /// </summary>
        public Host(fMain f, SizeQueue<JObject> messageQueue)
        {
            port = new Port();
            stop = new ManualResetEvent(false);
            m_f = f;
            m_messageQueue = messageQueue;
        }

        /// <summary>
        /// Starts native message processing.
        /// </summary>
        public void Run()
        {
            log.Info("host started 0.0.0.1");
            m_f.log("host started");

            stop.Reset();
            while (!stop.WaitOne(0))
            {
                // process messages from Chrome
                //
                try
                {
                    //
                    // read a message
                    //
                    string message = port.Read();

                    // log
                    //
                    log.DebugFormat("request message\n{0}", message);
                    m_f.log("request message " + message);

                    JObject request = JObject.Parse(message);

                    //
                    // execute the request
                    //
                    executeMessage(request);

                    //
                    // prepare a response
                    //
                    JObject reply = new JObject();
                    if (request["source"] != null) reply["source"] = request["destination"];
                    if (request["destination"] != null) reply["destination"] = request["source"];

                    reply["message"] = new JObject();
                    reply["message"]["id"] = request["id"];

                    // identify service
                    //
                    reply["extension"] = C_EXTENSION_NAME;

                    message = reply.ToString(Formatting.None);                    

                    // log
                    //
                    log.DebugFormat("reply message\n{0}", message);
                    m_f.log(message);

                    //
                    // send response
                    //
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
                    //stop.Set();
                    //throw ex;
                }

                // process messages from CSReports
                //
                JObject jMessage = m_messageQueue.Dequeue();

                while (jMessage != null)
                {
                    // identify service
                    //
                    jMessage["extension"] = C_EXTENSION_NAME;

                    string message = jMessage.ToString(Formatting.None);

                    // log
                    //
                    log.DebugFormat("reply message\n{0}", message);
                    m_f.log(message);

                    //
                    // send message
                    //
                    port.Write(message);

                    jMessage = m_messageQueue.Dequeue();
                }
            }

            log.Info("host stopped");
            m_f.log("host stopped");
            m_f.close();
        }

        /// <summary>
        /// Stops native message processing.
        /// </summary>
        public void Stop()
        {
            stop.Set();
        }

        private void executeMessage(JObject request)
        {
            var action = request["message"]["action"].ToString();

            var id = request["id"].ToString();

            if (action.StartsWith("__PARTIAL_MESSAGE__"))
            {
                var p = "";
                if (m_partialMessages.ContainsKey(id))
                {
                    p = m_partialMessages[id];
                }
                p += request["message"]["data"];
                m_partialMessages[id] = p;
            }
            else
            {
                if (m_partialMessages.ContainsKey(id))
                {
                    request["message"]["data"] = JObject.Parse((m_partialMessages[id] + request["message"]["data"]).ToString());
                }
                else {
                    request["message"]["data"] = JObject.Parse(request["message"]["data"].ToString());
                }
                switch (action)
                {
                    case "preview":
                        previewReport(request);
                        break;
                    case "print":
                        printReport(request);
                        break;
                    case "moveToPage":
                        moveToPage(request);
                        break;
                    case "debugger":
                        break;
                }
            }
        }

        private void previewReport(JObject request)
        {
            m_f.preview(request);
        }

        private void printReport(JObject request)
        {
            m_f.printReport(request);
        }

        private void moveToPage(JObject request)
        {
            m_f.moveToPage(request);
        }
    }

    public class SizeQueue<T>
    {
        private readonly Queue<T> queue = new Queue<T>();
        private readonly int maxSize;
        public SizeQueue(int maxSize) { this.maxSize = maxSize; }

        public void Enqueue(T item)
        {
            lock (queue)
            {
                if (queue.Count < maxSize) // yes, I am a bad person :P
                {
                    queue.Enqueue(item);
                }
            }
        }
        public T Dequeue()
        {
            lock (queue)
            {
                T item = default(T);
                if (queue.Count > 0)
                {
                    item = queue.Dequeue();
                }
                return item;
            }
        }
    }
}
