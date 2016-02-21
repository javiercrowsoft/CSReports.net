using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CSKernelClient;

namespace CSReportPreview
{
    public delegate void FirstPage(object sender, EventArgs e);
    public delegate void PreviousPage(object sender, EventArgs e);
    public delegate void MoveToPage(object sender, PageEventArgs e);
    public delegate void NextPage(object sender, EventArgs e);
    public delegate void LastPage(object sender, EventArgs e);

    public delegate void Print(object sender, EventArgs e);
    public delegate void ExportToPDF(object sender, EventArgs e);

    public partial class cReportPreview : UserControl
    {
        public event FirstPage FirstPage;
        public event PreviousPage PreviousPage;
        public event MoveToPage MoveToPage;
        public event NextPage NextPage;        
        public event LastPage LastPage;

        public event Print Print;
        public event ExportToPDF ExportToPDF;

        public cReportPreview()
        {
            InitializeComponent();
        }

        public PictureBox getBody()
        {
            return pnReport;
        }

        public Graphics getGraph()
        {
            return null;
        }

        public Object getParent()
        {
            return Parent;
        }

        public void setCurrPage(int page)
        {
            tsbPage.Text = page.ToString();
        }

        public void setPages(int pages)
        {
            tsbPages.Text = pages.ToString();
        }

        private void tsbFirstPage_Click(object sender, EventArgs e)
        {
            if (FirstPage != null)
            {
                FirstPage(this, EventArgs.Empty);
            }
        }

        private void tsbPreviousPage_Click(object sender, EventArgs e)
        {
            if (PreviousPage != null)
            {
                PreviousPage(this, EventArgs.Empty);
            }
        }

        private void tsbNextPage_Click(object sender, EventArgs e)
        {
            if (NextPage != null)
            {
                NextPage(this, EventArgs.Empty);
            }
        }

        private void tsbLastPage_Click(object sender, EventArgs e)
        {
            if (LastPage != null)
            {
                LastPage(this, EventArgs.Empty);
            }
        }

        private void tsbExportPDF_Click(object sender, EventArgs e)
        {
            if (ExportToPDF != null)
            {
                ExportToPDF(this, EventArgs.Empty);
            }
        }

        private void tsbPage_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                var page = cUtil.valAsInt(tsbPage.Text);
                if (page > 0) 
                {
                    if (MoveToPage != null)
                    {
                        MoveToPage(this, new PageEventArgs(page));
                    }
                }
            }
        }

        private void tsbPrint_Click(object sender, EventArgs e)
        {
            if (Print != null)
            {
                Print(this, EventArgs.Empty);
            }
        }
    }

    public class PageEventArgs : EventArgs
    {
        private readonly int m_page = -1;

        public PageEventArgs(int page)
        {
            m_page = page;
        }
        public int page { get { return m_page; } }
    }

}
