using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CSKernelClient;
using CSReportGlobals;
using CSReportDll;
using CSReportPaint;
using CSDataBase;

namespace CSReportWebServer
{
    class Report
    {
        private const String C_MODULE = "Report";

        private string m_database;
        private cReport m_report;

        private fProgress m_fProgress;
        private bool m_cancelPrinting = false;

        // we modify the report data source so it uses the CSReportWebServer instead of a real sql engine (SqlServer, PostgreSQL or Oracle)
        //
        public void init(JObject request)
        {
            m_database = Guid.NewGuid().ToString();
            m_report = new cReport();

            m_report.setDatabaseEngine(csDatabaseEngine.CSREPORT_WEB);

            m_report.Progress += reportProgress;
            m_report.ReportDone += reportDone;

            cReportLaunchInfo oLaunchInfo = new cReportLaunchInfo();

            oLaunchInfo.setPrinter(cPrintAPI.getcPrinterFromDefaultPrinter());
            oLaunchInfo.setObjPaint(new cReportPrint());

            registerDataSource(request);

            if (!m_report.init(oLaunchInfo)) { return; }

            m_report.setPathDefault(Application.StartupPath);
        }

        public bool openDocument(String fileName)
        {
            cMouseWait mouse = new cMouseWait();
            try
            {
                if (!m_report.loadSilent(fileName))
                {
                    return false;
                }

                m_report.getLaunchInfo().setStrConnect(m_database);

                Application.DoEvents();
                
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                mouse.Dispose();
            }
        }

        public void preview()
        {
            m_report.getLaunchInfo().setAction(csRptLaunchAction.CSRPTLAUNCHPREVIEW);
            launchReport();
        }

        private void registerDataSource(JObject request)
        {
            var dataSources = request["message"]["data"]["data"];
            foreach (var dataSource in dataSources)
            {
                cJSONDataSource ds = new cJSONDataSource(dataSource["name"].ToString(), dataSource["data"] as JObject);
                cJSONServer.registerDataSource(ds, m_database + "." + ds.getName());
            }
        }

        private void reportDone(object sender, EventArgs e)
        {
            closeProgressDlg();
        }

        private void reportProgress(object sender, ProgressEventArgs e)
        {

            String task = e.task;
            int page = e.page;
            int currRecord = e.currRecord;
            int recordCount = e.recordCount;

            if (m_cancelPrinting)
            {
                if (cWindow.ask("Confirm you want to cancel the execution of this report?", MessageBoxDefaultButton.Button2))
                {
                    e.cancel = true;
                    closeProgressDlg();
                    return;
                }
                else {
                    m_cancelPrinting = false;
                }
            }

            if (m_fProgress == null) { return; }

            if (page > 0) { m_fProgress.lbCurrPage.Text = page.ToString(); }
            if (task != "") { m_fProgress.lbTask.Text = task; }
            if (currRecord > 0) { m_fProgress.lbCurrRecord.Text = currRecord.ToString(); }
            if (recordCount > 0 && cUtil.val(m_fProgress.lbRecordCount.Text) != recordCount)
            {
                m_fProgress.lbRecordCount.Text = recordCount.ToString();
            }

            double percent = 0;
            if (recordCount > 0 && currRecord > 0)
            {
                percent = Convert.ToDouble(currRecord) / recordCount;
                var value = Convert.ToInt32(percent * 100);
                if (value > 100) value = 100;
                m_fProgress.prgBar.Value = value;
            }

            Application.DoEvents();
        }

        private void launchReport()
        {
            cMouseWait mouse = new cMouseWait();
            try
            {
                showProgressDlg();

                m_report.getLaunchInfo().getPrinter().setPaperInfo(m_report.getPaperInfo());
                m_report.getLaunchInfo().setObjPaint(new cReportPrint());
                // TODO: remove this
                m_report.getLaunchInfo().setHwnd(0);
                m_report.getLaunchInfo().setShowPrintersDialog(true);
                m_report.launch();

            }
            catch (Exception ex)
            {
                cError.mngError(ex, "launchReport", C_MODULE, "");
            }
            finally
            {
                mouse.Dispose();
                closeProgressDlg();
            }
        }

        private void showProgressDlg()
        {
            m_cancelPrinting = false;
            if (m_fProgress == null)
            {
                m_fProgress = new fProgress();
                // TODO: add event for m_report_Progress
            }
            m_fProgress.Show();
            m_fProgress.BringToFront();
        }

        private void closeProgressDlg()
        {
            if (m_fProgress != null && !m_fProgress.IsDisposed)
            {
                m_fProgress.Close();
            }
            m_fProgress = null;
        }
    }
}
