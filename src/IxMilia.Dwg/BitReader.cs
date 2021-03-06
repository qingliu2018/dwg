﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace IxMilia.Dwg
{
    internal class BitReader
    {
        internal int bitOffset;

        public byte[] Data { get; private set; }
        public int Offset { get; set; }

        private Stack<int> _crcStartValues = new Stack<int>();

        public BitReader(byte[] data, int offset = 0)
        {
            bitOffset = 0;
            Data = data;
            Offset = offset;
        }

        public BitReader FromOffset(int offset)
        {
            return new BitReader(Data, offset);
        }

        private byte GetCurrentByte()
        {
            if (Offset >= Data.Length)
            {
                throw new DwgReadException("Out of data");
            }

            return Data[Offset];
        }

        public void AlignToByte()
        {
            if (bitOffset != 0)
            {
                bitOffset = 0;
                Offset++;
            }
        }

        public int ReadBit()
        {
            int result = (GetCurrentByte() >> (8 - bitOffset - 1)) & 0x01;
            bitOffset++;
            if (bitOffset >= 8)
            {
                bitOffset = 0;
                Offset++;
            }

            return result;
        }

        public int ReadBits(int count)
        {
            if (count >= 32)
            {
                throw new DwgReadException("Can't read more than 32 bits at a time.");
            }

            int result = 0;
            for (int i = 0; i < count; i++)
            {
                var bit = ReadBit();
                result = (result << 1) | bit;
            }

            return result;
        }

        public byte ReadByte()
        {
            if (bitOffset == 0)
            {
                var result = GetCurrentByte();
                Offset++;
                return result;
            }
            else
            {
                var temp = (GetCurrentByte() << bitOffset);
                Offset++;
                temp |= (GetCurrentByte() >> (8 - bitOffset));
                return (byte)temp;
            }
        }

        public byte[] ReadBytes(long count)
        {
            var result = new byte[count];
            for (int i = 0; i < count; i++)
            {
                result[i] = ReadByte();
            }

            return result;
        }

        public short ReadShort()
        {
            var bytes = ReadBytes(2);
            return (short)((bytes[1] << 8) + bytes[0]);
        }

        public short ReadShortBigEndian()
        {
            var bytes = ReadBytes(2);
            return (short)((bytes[0] << 8) + bytes[1]);
        }

        public int ReadInt()
        {
            var bytes = ReadBytes(4);
            return (bytes[3] << 24) + (bytes[2] << 16) + (bytes[1] << 8) + bytes[0];
        }

        public double ReadDouble()
        {
            var bytes = ReadBytes(8);
            if (!BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }

            return BitConverter.ToDouble(bytes, 0);
        }

        public void SkipBytes(int count)
        {
            for (int i = 0; i < count; i++)
            {
                ReadByte();
            }
        }

        public string ReadStringAscii(int count)
        {
            var bytes = ReadBytes(count);
            var sb = new StringBuilder();
            foreach (var b in bytes)
            {
                sb.Append((char)b);
            }

            return sb.ToString();
        }

        public void StartCrcCheck()
        {
            _crcStartValues.Push(Offset);
        }

        public ushort ComputeCrc(ushort initialValue = 0)
        {
            if (_crcStartValues.Count == 0)
            {
                throw new InvalidOperationException($"You must call {nameof(StartCrcCheck)}() first.");
            }

            var startOffset = _crcStartValues.Pop();
            var endOffset = bitOffset == 0
                ? Offset
                : Offset + 1;

            return BitReaderExtensions.ComputeCRC(Data, startOffset, endOffset - startOffset, initialValue);
        }

        public void ValidateCrc(ushort initialValue = 0, ushort xorValue = 0, bool readCrcAsMsb = false)
        {
            AlignToByte();
            var actualCrc = ComputeCrc(initialValue);
            actualCrc ^= xorValue;
            var expectedCrc = readCrcAsMsb
                ? (ushort)ReadShortBigEndian()
                : (ushort)this.Read_RS();
            if (expectedCrc != actualCrc)
            {
                throw new DwgReadException($"Failed CRC check.  Expected {expectedCrc}, actual {actualCrc}.");
            }
        }
    }
}
