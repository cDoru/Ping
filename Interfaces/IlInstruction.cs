using System.Reflection.Emit;

namespace PingExperiment.Interfaces
{
    public class IlInstruction
    {
        public IlInstruction(int offset, OpCode code, object operand)
        {
            Offset = offset;
            Code = code;
            Operand = operand;
        }

        public int Offset { get; private set; }
        public OpCode Code { get; private set; }
        public object Operand { get; private set; }
    }
}