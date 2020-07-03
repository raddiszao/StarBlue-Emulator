﻿using StarBlue.Communication.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace StarBlue.Communication.Packets.Outgoing
{
    public class ServerPacket : IServerPacket
    {
        private readonly Encoding _encoding = Encoding.Default;

        private readonly List<byte> _body = new List<byte>();

        public ServerPacket(int id)
        {
            Id = id;
            WriteShort(id);
        }

        public int Id { get; }

        public byte[] GetBytes()
        {
            var final = new List<byte>();
            final.AddRange(BitConverter.GetBytes(_body.Count)); // packet len
            final.Reverse();
            final.AddRange(_body); // Add Packet
            return final.ToArray();
        }

        public void WriteByte(byte b)
        {
            _body.Add(b);
        }

        public void WriteByte(int b)
        {
            _body.Add((byte)b);
        }

        public void WriteBytes(byte[] b, bool isInt) // d
        {
            if (isInt)
            {
                for (int i = (b.Length - 1); i > -1; i--)
                {
                    _body.Add(b[i]);
                }
            }
            else
            {
                _body.AddRange(b);
            }
        }

        public void WriteDouble(double d) // d
        {
            string raw = Math.Round(d, 1).ToString(CultureInfo.CurrentCulture);

            if (raw.Length == 1)
            {
                raw += ".0";
            }

            WriteString(raw.Replace(',', '.'));
        }

        public void WriteString(string s) // d
        {
            WriteShort(s.Length);
            WriteBytes(_encoding.GetBytes(s), false);
        }

        public void WriteShort(int s) // d
        {
            var i = (Int16)s;
            WriteBytes(BitConverter.GetBytes(i), true);
        }

        public void WriteUnsignedShort(UInt16 s) // d
        {
            var i = s;
            WriteBytes(BitConverter.GetBytes(i), true);
        }

        public void WriteInteger(int i) // d
        {
            WriteBytes(BitConverter.GetBytes(i), true);
        }

        public void WriteBoolean(bool b) // d
        {
            WriteBytes(new[] { (byte)(b ? 1 : 0) }, false);
        }
    }
}