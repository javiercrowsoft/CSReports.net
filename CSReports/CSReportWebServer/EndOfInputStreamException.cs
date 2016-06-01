using System;
using System.Runtime.Serialization;

namespace CSReportWebServer.NativeMessaging
{
    [System.Serializable]
    public class EndOfInputStreamException : NativeMessagingException
    {
        public EndOfInputStreamException() : base("End of input stream exception.") { }
        public EndOfInputStreamException(string message) : base(message) { }
        public EndOfInputStreamException(string message, Exception innerException) : base(message, innerException) { }
        protected EndOfInputStreamException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
