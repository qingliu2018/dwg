﻿using System;
using System.IO;
using Xunit;

namespace IxMilia.Dwg.Test
{
    public class BitWriterTests
    {
        private static BitWriter Writer()
        {
            var stream = new MemoryStream();
            return new BitWriter(stream);
        }

        private static byte[] Bytes(BitWriter writer)
        {
            writer.Flush();
            writer.BaseStream.Seek(0, SeekOrigin.Begin);
            var bytes = new byte[writer.BaseStream.Length];
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = (byte)writer.BaseStream.ReadByte();
            }

            return bytes;
        }

        private static byte[] Bytes(params int[] values)
        {
            var bytes = new byte[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] < 0 || values[i] > 255)
                {
                    // the int data type is only a convenience
                    throw new NotSupportedException("Byte values must be between [0, 255].");
                }

                bytes[i] = (byte)values[i];
            }

            return bytes;
        }

        [Fact]
        public void BitProgression()
        {
            var writer = Writer();
            writer.WriteBit(1);
            writer.WriteBit(1);
            writer.WriteBit(0);
            writer.WriteBit(0);
            writer.WriteBit(1);
            writer.WriteBit(1);
            writer.WriteBit(0);
            writer.WriteBit(1);
            Assert.Equal(Bytes(0b11001101), Bytes(writer));
        }

        [Fact]
        public void ByteProgression()
        {
            var writer = Writer();
            for (int i = 0; i < 8; i++)
            {
                writer.WriteBit(0);
            }

            for (int i = 0; i < 8; i++)
            {
                writer.WriteBit(1);
            }

            Assert.Equal(Bytes(0b00000000, 0b11111111), Bytes(writer));
        }

        [Fact]
        public void WriteBits()
        {
            Assert.Equal(Bytes(0b10000000), Bytes(Writer().WriteBits(0b11110000, 1)));
            Assert.Equal(Bytes(0b11000000), Bytes(Writer().WriteBits(0b11110000, 2)));
            Assert.Equal(Bytes(0b11100000), Bytes(Writer().WriteBits(0b11110000, 3)));
            Assert.Equal(Bytes(0b11110000), Bytes(Writer().WriteBits(0b11110000, 4)));
            Assert.Equal(Bytes(0b11110000), Bytes(Writer().WriteBits(0b11110000, 5)));
            Assert.Equal(Bytes(0b11110000), Bytes(Writer().WriteBits(0b11110000, 6)));
            Assert.Equal(Bytes(0b11110000), Bytes(Writer().WriteBits(0b11110000, 7)));
            Assert.Equal(Bytes(0b11110000), Bytes(Writer().WriteBits(0b11110000, 8)));
        }

        [Fact]
        public void Write_BS()
        {
            Assert.Equal(Bytes(0b00010101, 0b10000000, 0b01000000), Bytes(Writer().Write_BS(342)));
            //                   ||AAAAAA    AABBBBBB    BB------

            Assert.Equal(Bytes(0b01001010, 0b10000000), Bytes(Writer().Write_BS(42)));
            //                   ||AAAAAA    AA------

            Assert.Equal(Bytes(0b10000000), Bytes(Writer().Write_BS(0)));
            //                   ||------

            Assert.Equal(Bytes(0b11000000), Bytes(Writer().Write_BS(256)));
            //                   ||------
        }

        [Fact]
        public void Write_BL()
        {
            Assert.Equal(Bytes(0b00000000, 0b01000000, 0b01000000, 0b00000000, 0b00000000), Bytes(Writer().Write_BL(257)));
            //                   ||AAAAAA    AABBBBBB    BBCCCCCC    CCDDDDDD    DD------

            Assert.Equal(Bytes(0b10000000), Bytes(Writer().Write_BL(0)));
            //                   ||------

            Assert.Equal(Bytes(0b01000011, 0b11000000), Bytes(Writer().Write_BL(15)));
            //                   ||AAAAAA    AA------
        }

        [Fact]
        public void Write_BD()
        {
            Assert.Equal(Bytes(0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00001101, 0b11010000, 0b00000000), Bytes(Writer().Write_BD(23.0)));
            //                   ||ssssss    ssssssss    ssssssss    ssssssss    ssssssss    ssssssss    ssssssee    eeeeeeee    e^------

            Assert.Equal(Bytes(0b01000000), Bytes(Writer().Write_BD(1.0)));
            //                       ||------

            Assert.Equal(Bytes(0b10000000), Bytes(Writer().Write_BD(0.0)));
            //                   ||------
        }

        [Fact]
        public void Write_MC()
        {
            Assert.Equal(Bytes(0b10000010, 0b00100100), Bytes(Writer().Write_MC(4610)));
            Assert.Equal(Bytes(0b11101001, 0b10010111, 0b11100110, 0b00110101), Bytes(Writer().Write_MC(112823273)));
            Assert.Equal(Bytes(0b10000101, 0b01001011), Bytes(Writer().Write_MC(-1413)));
        }

        [Fact]
        public void Write_MS()
        {
            Assert.Equal(Bytes(0b00110001, 0b11110100, 0b10001101, 0b00000000), Bytes(Writer().Write_MS(4650033)));
        }

        [Fact]
        public void Write_H()
        {
            Assert.Equal(Bytes(0b01010010, 0b00000101, 0b11100111), Bytes(Writer().Write_H(new DwgHandleReference(DwgHandleReferenceCode.SoftOwner, 0x05E7))));
        }
    }
}