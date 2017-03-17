using System;

namespace PingExperiment.Attributes
{
    public abstract class ConstraintAttribute : Attribute
    {
        public ConstraintAttribute() {}

        public abstract bool Check(Type generic);
    }
}