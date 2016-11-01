using System;
using System.Windows.Forms;

namespace CSKernelClient
{


    public class cMouseWait : IDisposable
    {
        private Cursor m_lastCursor = null;

        public void Dispose()
        {
            if (m_lastCursor != null)
            {
                Cursor.Current = m_lastCursor;
            }
        }

        public cMouseWait()
        {
            m_lastCursor = Cursor.Current;
            Cursor.Current = Cursors.WaitCursor;
        }
    }
}
