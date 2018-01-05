﻿namespace CR.Servers.CoC.Packets.Messages.Server.Friend
{
    using CR.Servers.CoC.Logic;
    using CR.Servers.Extensions.List;

    internal class Friend_Online : Message
    {
        internal Friend Friend;

        public Friend_Online(Device Device) : base(Device)
        {
        }

        internal override short Type => 20206;

        internal override void Encode()
        {
            this.Data.AddLong(this.Friend.PlayerId);
            this.Data.AddVInt((int) this.Friend.GameState);
            //1 = Player online
        }
    }
}