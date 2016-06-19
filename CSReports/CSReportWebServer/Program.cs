using System;
using System.Windows.Forms;
using log4net;

namespace CSReportWebServer
{
    static class Program
    {
		private static ILog log = LogManager.GetLogger(typeof(Program));

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
			// configure log4net
			log4net.Config.XmlConfigurator.Configure();
            log.Info("application started 0.0.0.1");
			log.DebugFormat("command line : \"{0}\"", string.Join("\", \"", args));
			log.Info("-------");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new fMain(args));            
        }
    }
}
