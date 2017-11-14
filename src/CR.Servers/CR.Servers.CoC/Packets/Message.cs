﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using CR.Servers.CoC.Core;
using CR.Servers.CoC.Logic;
using CR.Servers.Extensions;
using CR.Servers.Extensions.Binary;
using CR.Servers.Extensions.List;

namespace CR.Servers.CoC.Packets
{
    internal abstract class Message
    {
        internal int Length;
        internal short Version;

        internal int Offset;

        internal abstract short Type { get; }

        internal Device Device;

        internal Reader Reader;
        internal List<byte> Data;

        internal Message(Device Device)
        {
            this.Device = Device;
            this.Data = new List<byte>();
        }

        internal Message(Device Device, Reader Reader)
        {
            this.Device = Device;
            this.Reader = Reader;
        }

        internal byte[] ToBytes
        {
            get
            {
                List<byte> Packet = new List<byte>();

                Packet.AddShort(this.Type);
                Packet.AddUInt24((uint)this.Length);
                Packet.AddShort(this.Version);
                Packet.AddRange(this.Data.ToArray());

                return Packet.ToArray();
            }
        }

        internal virtual void Decode()
        {
            //Trace.WriteLine("[*] " + this.GetType().Name + " : " + "Decoding.");
        }

        internal virtual void Encode()
        {
            //Trace.WriteLine("[*] " + this.GetType().Name + " : " + "Encoding.");
        }

        internal virtual void Process()
        {
            //Trace.WriteLine("[*] " + this.GetType().Name + " : " + "Processing.");
        }

        internal virtual void Encrypt()
        {
            byte[] Packet = this.Data.ToArray();

            Packet =  this.Device.SendEncrypter.Encrypt(Packet);

            this.Data.Clear();
            this.Data.AddRange(Packet);

            this.Length = this.Data.Count;
        }

        internal void ShowBuffer()
        {
            Logging.Info(this.GetType(), BitConverter.ToString(this.Reader.ReadBytes((int)(this.Reader.BaseStream.Length - this.Reader.BaseStream.Position))));
        }

        internal void ShowValues()
        {
            foreach (FieldInfo Field in this.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                if (Field != null)
                {
                    Logging.Info(this.GetType(), ConsolePad.Padding(Field.Name) + " : " + ConsolePad.Padding(!string.IsNullOrEmpty(Field.Name) ? (Field.GetValue(this) != null ? Field.GetValue(this).ToString() : "(null)") : "(null)", 40));
                }
            }
        }

        internal void Log()
        {
            File.AppendAllText(Directory.GetCurrentDirectory() + "\\Logs\\" + this.GetType().Name + ".log", BitConverter.ToString(this.Reader.ReadBytes((int)(this.Reader.BaseStream.Length - this.Reader.BaseStream.Position))) + Environment.NewLine);
        }
    }
}
