using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using CSReportGlobals;

namespace CSReportDll
{
    class ReportException : System.Exception
    {
        public csRptErrors errorCode { get; set; }
        public String className { get; set; }

        public ReportException() : base() { }

        public ReportException(csRptErrors code, String module, String message) : base(message) 
        {
            errorCode = code;
            className = module;
        }
        
        // A constructor is needed for serialization when an
        // exception propagates from a remoting server to the client. 
        protected ReportException(SerializationInfo info, StreamingContext context) { }
        
        public override string ToString()
        {
            return base.ToString() + "\n\nCode:" + errorCode.ToString();
        }
    }
}
