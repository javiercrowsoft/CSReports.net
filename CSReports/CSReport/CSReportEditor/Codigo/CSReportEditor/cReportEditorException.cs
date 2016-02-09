using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace CSReportEditor
{
    class ReportEditorException : System.Exception
    {
        public csRptEditorErrors errorCode { get; set; }
        public String className { get; set; }

        public ReportEditorException() : base() { }

        public ReportEditorException(csRptEditorErrors code, String module, String message)
            : base(message)
        {
            errorCode = code;
            className = module;
        }

        // A constructor is needed for serialization when an
        // exception propagates from a remoting server to the client. 
        protected ReportEditorException(SerializationInfo info, StreamingContext context) { }

        public override string ToString()
        {
            return base.ToString() + "\n\nCode:" + errorCode.ToString();
        }
    }
}
