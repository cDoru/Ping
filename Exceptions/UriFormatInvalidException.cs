using System;
using System.Runtime.Serialization;

namespace PingExperiment.Exceptions
{
    [Serializable]
    public class UriFormatInvalidException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public UriFormatInvalidException()
        {
        }

        public UriFormatInvalidException(string message) : base(message)
        {
        }

        public UriFormatInvalidException(string message, Exception inner) : base(message, inner)
        {
        }

        protected UriFormatInvalidException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}