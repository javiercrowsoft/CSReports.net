using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSReportDll
{
    public interface cIReportSection
    {
        cReportFormula getFormulaHide();

        bool getHasFormulaHide();

        string getName();

        void setName(string name);

        void setHasFormulaHide(bool value);
    }
}
