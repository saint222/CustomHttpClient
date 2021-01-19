using System;
using System.Runtime.Serialization;

namespace Kalyuganov.Demo.Http
{
    [Serializable]
    public class BadArgumentException : Exception
    {
        public BadArgumentException() { }        
        public BadArgumentException(string message) : base(message) { }
        public BadArgumentException(string message, Exception innerException) : base(message, innerException) { }
        protected BadArgumentException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}