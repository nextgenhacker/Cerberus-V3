﻿namespace CR.Servers.CoC.Packets.Messages.Server.Alliances
{
    using CR.Servers.CoC.Logic;
    using CR.Servers.CoC.Packets.Enums;
    using CR.Servers.Extensions.List;

    internal class CreateAllianceFailedMessage : Message
    {
        internal AllianceErrorReason Error;

        public CreateAllianceFailedMessage(Device Device) : base(Device)
        {
        }

        internal override short Type
        {
            get
            {
                return 24332;
            }
        }

        internal override void Encode()
        {
            this.Data.AddInt((int) this.Error);
        }
    }
}