using System;

namespace PingExperiment.Attributes
{
    [AttributeUsage(AttributeTargets.GenericParameter)]
    public class IsInterfaceAttribute : ConstraintAttribute
    {
        public override bool Check(Type genericType)
        {
            return genericType.IsInterface;
        }

        public override string ToString()
        {
            return "Generic type is not an interface";
        }
    }
}