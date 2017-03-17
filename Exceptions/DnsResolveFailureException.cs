using System;
using System.Runtime.Serialization;

namespace PingExperiment.Exceptions
{
    [Serializable]
    public class DnsResolveFailureException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public DnsResolveFailureException()
        {
        }

        public DnsResolveFailureException(string message) : base(message)
        {
        }

        public DnsResolveFailureException(string message, Exception inner) : base(message, inner)
        {
        }

        protected DnsResolveFailureException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}