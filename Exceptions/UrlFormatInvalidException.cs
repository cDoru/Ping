using System;
using System.Runtime.Serialization;

namespace PingExperiment.Exceptions
{
    [Serializable]
    public class UrlFormatInvalidException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public UrlFormatInvalidException()
        {
        }

        public UrlFormatInvalidException(string message) : base(message)
        {
        }

        public UrlFormatInvalidException(string message, Exception inner) : base(message, inner)
        {
        }

        protected UrlFormatInvalidException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}