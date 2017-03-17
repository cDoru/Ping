using System;

namespace PingExperiment.Utils
{
    public class ConstraintFailedException : Exception
    {
        public ConstraintFailedException(string msg) : base(msg) { }
        public ConstraintFailedException(string msg, params object[] arg) : base(string.Format(msg, arg)) { }
    }
}