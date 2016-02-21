using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSReportScript
{
    public interface cIReportScriptType
    {
        string RunScript(cReportCompilerGlobals globals);
    }
}
