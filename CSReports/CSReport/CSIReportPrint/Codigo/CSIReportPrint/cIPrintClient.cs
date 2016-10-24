using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSIReportPrint
{
    public interface cIPrintClient
    {
        bool printingPage(int iPage);
    }
}
