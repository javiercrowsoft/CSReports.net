using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSReportDll
{

    public class ProgressEventArgs : EventArgs
    {
        private readonly String m_task = "";
        private readonly int m_page = 0;
        private readonly int m_currRecord = 0;
        private readonly int m_recordCount = 0;

        public ProgressEventArgs(String task, int page, int currRecord, int recordCount)
        {
            m_task = task;
            m_page = page;
            m_currRecord = currRecord;
            m_recordCount = recordCount;
        }
        public String task { get { return m_task; } }
        public int page { get { return m_page; } }
        public int currRecord { get { return m_currRecord; } }
        public int recordCount { get { return m_recordCount; } }
        public bool cancel { get; set; }
    }

}
