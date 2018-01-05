﻿namespace CR.Servers.CoC.Packets.Messages.Client.Battle
{
    using CR.Servers.CoC.Core;
    using CR.Servers.CoC.Logic;
    using CR.Servers.Extensions.Binary;

    internal class Watch_1vs1_Live : Message
    {
        internal int HighId;
        internal int LowId;

        public Watch_1vs1_Live(Device Device, Reader Reader) : base(Device, Reader)
        {
        }

        internal override short Type => 15110;

        internal override void Decode()
        {
            this.HighId = this.Reader.ReadInt32();
            this.LowId = this.Reader.ReadInt32();
        }

        internal override void Process()
        {
            Player Player = Resources.Accounts.LoadAccount(this.HighId, this.LowId).Player;

            if (Player != null)
            {
                if (Player.Level?.BattleManager != null)
                {
                    Player.Level.BattleManager.AddSpectator(this.Device.GameMode.Level);
                }
            }
        }
    }
}