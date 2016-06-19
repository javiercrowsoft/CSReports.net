using System;
using System.Collections.Generic;
using System.Threading;
using System.IO;
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
		private SizeQueue<JObject> m_taskQueue = new SizeQueue<JObject>(10);

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

		private string m_reportId = "";
		private string m_webReportId = "";

		private string loadTestResponse()
		{
			// Open the text file using a stream reader.
			using (StreamReader sr = new StreamReader("/Users/javier/Work/Temp/response.json"))
			{
				// Read the stream to a string, and write the string to the console.
				return sr.ReadToEnd();
			}
		}

		private void preview()
		{
			JObject message = JObject.Parse(loadTestResponse());
			message["webReportId"] = m_webReportId;
			JObject envelope = new JObject();
			envelope["message"] = message;
			port.Write(envelope.ToString(Formatting.None));
		}

		private void reportDone()
		{
			JObject message = JObject.Parse("{ messageType: 'REPORT_DONE', reportId: '" + m_reportId + "', webReportId: '" + m_webReportId + "' }");
			JObject envelope = new JObject();
			envelope["message"] = message;
			port.Write(envelope.ToString(Formatting.None));
		}


        /// <summary>
        /// Starts native message processing.
        /// </summary>
        public void Run()
        {
            log.Info("host started 0.0.0.1");
            m_f.log("host started");

			startTaskThread();
			startMessageThread();

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
					// send response
					//
					m_webReportId = request["message"]["webReportId"].ToString();
					m_reportId = Guid.NewGuid().ToString();

					//Thread.Sleep(15000);

					//executeMessage(request);

					//reportDone();
					//preview();


					log.Info("calling executeMessage");

					m_taskQueue.Enqueue(request);

					log.Info("after calling executeMessage");

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

					port.Write(message);
                    
					// log
                    //
                    log.DebugFormat("reply message\n{0}", message);
                    m_f.log(message);

				}
				catch (EndOfInputStreamException)
                {
					stopTaskThread();
					stopMessageThread();
                    stop.Set();
					log.Debug("End of input stream");
                }
                catch (Exception ex)
                {
					stopTaskThread();
					stopMessageThread();
					stop.Set();
					log.Error("Message processing caused an exception", ex);
					throw ex;
                }
            }

            log.Info("host stopped !!!");
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
            switch (action)
            {
                case "preview":
                    previewReport(request);
                    break;
                case "debugger":
                    break;
            }
        }

        private void previewReport(JObject request)
        {
            m_f.preview(request);
        }

		//
		//
		//

		private void stopTaskThread()
		{
			bool createdNew;
			var waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset, "CF2D4313-33DE-489D-9721-6AFF69841DEB", out createdNew);

			// If the handle was already there, inform the other process to exit itself.
			// Afterwards we'll also die
			if (!createdNew)
			{
				waitHandle.Set();
			}
		}

		private void startTaskThread()
		{
			Thread workerThread = new Thread(this.processTaskQueue);
			workerThread.Start();
		}

		private void processTaskQueue()
		{
			bool createdNew;
			var waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset, "CF2D4313-33DE-489D-9721-6AFF69841DEB", out createdNew);

			// If the handle was already there, inform the other process to exit itself.
			// Afterwards we'll also die
			if (!createdNew)
			{
				log.Info("Inform other process to stop.");
				waitHandle.Set();
				log.Info("Informer exited.");

				return;
			}

			while (!waitHandle.WaitOne(TimeSpan.FromSeconds(1)))
            {
				try
				{
                    // process messages from CSReports
					//
					JObject request = m_taskQueue.Dequeue();

					while (request != null)
					{
						//
						// execute the request
						//
						executeMessage(request);

						request = m_taskQueue.Dequeue();
					}
				}
				catch (Exception ex)
				{
					log.Error("Processing queued tasks caused an exception", ex);
				}
			}
		}

		private void stopMessageThread()
		{ 
			bool createdNew;
			var waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset, "CF2D4313-33DE-489D-9721-6AFF69841DEA", out createdNew);

			// If the handle was already there, inform the other process to exit itself.
			// Afterwards we'll also die.
			if (!createdNew)
			{
				waitHandle.Set();
			}
		}

		private void startMessageThread()
		{ 
		    Thread workerThread = new Thread(this.processMessageQueue);
			workerThread.Start();
		}

		public void processMessageQueue()
		{
			bool createdNew;
			var waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset, "CF2D4313-33DE-489D-9721-6AFF69841DEA", out createdNew);

            // If the handle was already there, inform the other process to exit itself.
			// Afterwards we'll also die.
			if (!createdNew)
			{
				log.Info("Inform other process to stop.");
				waitHandle.Set();
				log.Info("Informer exited.");

				return;
			}

			while (!waitHandle.WaitOne(TimeSpan.FromSeconds(1)))
			{
				try
				{
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

						//
						// send message
						//
						port.Write(message);

						jMessage = m_messageQueue.Dequeue();
					}
				}
				catch (Exception ex)
				{
					log.Error("Processing queued messages caused an exception", ex);
				}
			}
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
