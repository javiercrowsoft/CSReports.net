using System;
using System.Runtime.Serialization;

namespace CSReportWebServer.NativeMessaging
{
    [System.Serializable]
    public class NativeMessagingException : System.Exception
    {
        public NativeMessagingException() : base("Native messaging exception.") { }
        public NativeMessagingException(string message) : base(message) { }
        public NativeMessagingException(string message, Exception innerException) : base(message, innerException) { }
        protected NativeMessagingException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

}
