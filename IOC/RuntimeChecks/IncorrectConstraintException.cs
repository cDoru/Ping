using System;

namespace PingExperiment.IOC.RuntimeChecks
{
    public class IncorrectConstraintException : Exception
    {
        public IncorrectConstraintException(string msg, params object[] arg) : base(string.Format(msg, arg)) { }
    }
}