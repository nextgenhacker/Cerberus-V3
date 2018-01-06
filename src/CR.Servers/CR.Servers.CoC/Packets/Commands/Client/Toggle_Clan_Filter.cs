﻿namespace CR.Servers.CoC.Packets.Commands.Client
{
    using CR.Servers.CoC.Logic;
    using CR.Servers.Extensions.Binary;

    internal class Toggle_Clan_Filter : Command
    {
        internal byte Unknown;

        public Toggle_Clan_Filter(Device Device, Reader Reader) : base(Device, Reader)
        {
        }

        internal override int Type => 571;

        internal override void Decode()
        {
            this.Unknown = this.Reader.ReadByte();
            base.Decode();
        }
    }
}