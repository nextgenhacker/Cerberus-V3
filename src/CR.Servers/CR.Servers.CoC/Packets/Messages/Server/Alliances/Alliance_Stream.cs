﻿namespace CR.Servers.CoC.Packets.Messages.Server.Alliances
{
    using CR.Servers.CoC.Logic;
    using CR.Servers.CoC.Logic.Clan;
    using CR.Servers.Extensions.List;

    internal class Alliance_Stream : Message
    {
        internal Alliance Alliance;

        public Alliance_Stream(Device Device) : base(Device)
        {
        }

        internal override short Type => 24311;

        internal override void Encode()
        {
            this.Data.AddInt(0);
            this.Alliance.Streams.Encode(this.Data, this.Device.GameMode.Level.Player.UserId);
        }
    }
}