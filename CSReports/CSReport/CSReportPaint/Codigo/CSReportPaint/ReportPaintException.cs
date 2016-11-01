using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace CSReportPaint
{
    class ReportPaintException : System.Exception
    {
        public csRptPaintErrors errorCode { get; set; }
        public String className { get; set; }

        public ReportPaintException() : base() { }

        public ReportPaintException(csRptPaintErrors code, String module, String message) : base(message) 
        {
            errorCode = code;
            className = module;
        }

        // A constructor is needed for serialization when an
        // exception propagates from a remoting server to the client. 
        protected ReportPaintException(SerializationInfo info, StreamingContext context) { }
        
        public override string ToString()
        {
            return base.ToString() + "\n\nCode:" + errorCode.ToString();
        }
    }
}

