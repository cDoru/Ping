using System;

namespace PingExperiment.IOC.RuntimeChecks
{
    internal class BigEndianByteReader
    {
        public BigEndianByteReader(byte[] data)
        {
            this.data = data;
            position = 0;
        }

        private byte[] data;
        private int position;

        public int Position
        {
            get { return position; }
        }

        public bool Eof
        {
            get { return position >= data.Length; }
        }

        public sbyte ReadSByte()
        {
            return (sbyte)data[position++];
        }

        public byte ReadByte()
        {
            return (byte)data[position++];
        }

        public int ReadInt16()
        {
            return ((data[position++] | (data[position++] << 8)));
        }

        public ushort ReadUInt16()
        {
            return (ushort)((data[position++] | (data[position++] << 8)));
        }

        public int ReadInt32()
        {
            return (((data[position++] | (data[position++] << 8)) | (data[position++] << 0x10)) | (data[position++] << 0x18));
        }

        public ulong ReadInt64()
        {
            return (ulong)(((data[position++] | (data[position++] << 8)) | (data[position++] << 0x10)) | (data[position++] << 0x18) | 
                           (data[position++] << 0x20) | (data[position++] << 0x28) | (data[position++] << 0x30) | (data[position++] << 0x38));
        }

        public double ReadDouble()
        {
            var result = BitConverter.ToDouble(data, position);
            position += 8;
            return result;
        }

        public float ReadSingle()
        {
            var result = BitConverter.ToSingle(data, position);
            position += 4;
            return result;
        }
    }
}