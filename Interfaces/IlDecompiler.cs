using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using PingExperiment.IOC;
using PingExperiment.Utils;

namespace PingExperiment.Interfaces
{
    internal static class IlDecompiler
    {
        static IlDecompiler()
        {
            // Initialize our cheat tables
            singleByteOpcodes = new OpCode[0x100];
            multiByteOpcodes = new OpCode[0x100];

            var infoArray1 = typeof(OpCodes).GetFields();
            for (var num1 = 0; num1 < infoArray1.Length; num1++)
            {
                var info1 = infoArray1[num1];
                if (info1.FieldType == typeof(OpCode))
                {
                    var code1 = (OpCode)info1.GetValue(null);
                    var num2 = (ushort)code1.Value;
                    if (num2 < 0x100)
                    {
                        singleByteOpcodes[(int)num2] = code1;
                    }
                    else
                    {
                        if ((num2 & 0xff00) != 0xfe00)
                        {
                            throw new Exception("Invalid opcode: " + num2);
                        }
                        multiByteOpcodes[num2 & 0xff] = code1;
                    }
                }
            }
        }

        private static OpCode[] singleByteOpcodes;
        private static OpCode[] multiByteOpcodes;

        public static IEnumerable<IlInstruction> Decompile(MethodBase mi, byte[] ildata)
        {
            var module = mi.Module;

            var reader = new BigEndianByteReader(ildata);
            while (!reader.Eof)
            {
                var code = OpCodes.Nop;

                var offset = reader.Position;
                ushort b = reader.ReadByte();
                if (b != 0xfe)
                {
                    code = singleByteOpcodes[b];
                }
                else
                {
                    b = reader.ReadByte();
                    code = multiByteOpcodes[b];
                    b |= (ushort)(0xfe00);
                }

                object operand = null;
                switch (code.OperandType)
                {
                    case OperandType.InlineBrTarget:
                        operand = reader.ReadInt32() + reader.Position;
                        break;
                    case OperandType.InlineField:
                        if (mi is ConstructorInfo)
                        {
                            operand = module.ResolveField(reader.ReadInt32(), mi.DeclaringType.GetGenericArguments(), Type.EmptyTypes);
                        }
                        else
                        {
                            operand = module.ResolveField(reader.ReadInt32(), mi.DeclaringType.GetGenericArguments(), mi.GetGenericArguments());
                        }
                        break;
                    case OperandType.InlineI:
                        operand = reader.ReadInt32();
                        break;
                    case OperandType.InlineI8:
                        operand = reader.ReadInt64();
                        break;
                    case OperandType.InlineMethod:
                        try
                        {
                            if (mi is ConstructorInfo)
                            {
                                operand = module.ResolveMember(reader.ReadInt32(), mi.DeclaringType.GetGenericArguments(), Type.EmptyTypes);
                            }
                            else
                            {
                                operand = module.ResolveMember(reader.ReadInt32(), mi.DeclaringType.GetGenericArguments(), mi.GetGenericArguments());
                            }
                        }
                        catch
                        {
                            operand = null;
                        }
                        break;
                    case OperandType.InlineNone:
                        break;
                    case OperandType.InlineR:
                        operand = reader.ReadDouble();
                        break;
                    case OperandType.InlineSig:
                        operand = module.ResolveSignature(reader.ReadInt32());
                        break;
                    case OperandType.InlineString:
                        operand = module.ResolveString(reader.ReadInt32());
                        break;
                    case OperandType.InlineSwitch:
                        var count = reader.ReadInt32();
                        var targetOffsets = new int[count];
                        for (var i = 0; i < count; ++i)
                        {
                            targetOffsets[i] = reader.ReadInt32();
                        }
                        var pos = reader.Position;
                        for (var i = 0; i < count; ++i)
                        {
                            targetOffsets[i] += pos;
                        }
                        operand = targetOffsets;
                        break;
                    case OperandType.InlineTok:
                    case OperandType.InlineType:
                        try
                        {
                            if (mi is ConstructorInfo)
                            {
                                operand = module.ResolveMember(reader.ReadInt32(), mi.DeclaringType.GetGenericArguments(), Type.EmptyTypes);
                            }
                            else
                            {
                                operand = module.ResolveMember(reader.ReadInt32(), mi.DeclaringType.GetGenericArguments(), mi.GetGenericArguments());
                            }
                        }
                        catch
                        {
                            operand = null;
                        }
                        break;
                    case OperandType.InlineVar:
                        operand = reader.ReadUInt16();
                        break;
                    case OperandType.ShortInlineBrTarget:
                        operand = reader.ReadSByte() + reader.Position;
                        break;
                    case OperandType.ShortInlineI:
                        operand = reader.ReadSByte();
                        break;
                    case OperandType.ShortInlineR:
                        operand = reader.ReadSingle();
                        break;
                    case OperandType.ShortInlineVar:
                        operand = reader.ReadByte();
                        break;

                    default:
                        throw new Exception("Unknown instruction operand; cannot continue. Operand type: " + code.OperandType);
                }

                yield return new IlInstruction(offset, code, operand);
            }
        }
    }
}