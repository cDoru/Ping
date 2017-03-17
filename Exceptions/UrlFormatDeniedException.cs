using System;
using System.Runtime.Serialization;

namespace PingExperiment.Exceptions
{
    [Serializable]
    public class UrlFormatDeniedException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public UrlFormatDeniedException()
        {
        }

        public UrlFormatDeniedException(string message) : base(message)
        {
        }

        public UrlFormatDeniedException(string message, Exception inner) : base(message, inner)
        {
        }

        protected UrlFormatDeniedException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}