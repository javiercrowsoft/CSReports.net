using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace CSKernelClient
{
    class KernelException : System.Exception
    {
        public String className { get; set; }

        public KernelException() : base() { }

        public KernelException(String module, String message) : base(message) 
        {
            className = module;
        }

        // A constructor is needed for serialization when an
        // exception propagates from a remoting server to the client. 
        protected KernelException(SerializationInfo info, StreamingContext context) { }
        
        public override string ToString()
        {
            return base.ToString() + "\n\nClass Name:" + className;
        }
    }
}
