﻿namespace CR.Servers.CoC.Packets.Messages.Client.Alliances
{
    using CR.Servers.CoC.Core;
    using CR.Servers.CoC.Core.Network;
    using CR.Servers.CoC.Logic;
    using CR.Servers.CoC.Logic.Clan;
    using CR.Servers.CoC.Packets.Messages.Server.Alliances;
    using CR.Servers.Extensions.Binary;

    internal class Ask_For_Alliance_Data : Message
    {
        internal int AllianceHighId;
        internal int AllianceLowId;

        public Ask_For_Alliance_Data(Device Device, Reader Reader) : base(Device, Reader)
        {
        }

        internal override short Type => 14302;

        internal override void Decode()
        {
            this.AllianceHighId = this.Reader.ReadInt32();
            this.AllianceLowId = this.Reader.ReadInt32();
        }

        internal override async void Process()
        {
            if (this.AllianceLowId > 0)
            {
                Alliance Alliance = await Resources.Clans.GetAsync(this.AllianceHighId, this.AllianceLowId);

                if (Alliance != null)
                {
                    new Alliance_Data(this.Device) {Alliance = Alliance}.Send();
                }
            }
        }
    }
}