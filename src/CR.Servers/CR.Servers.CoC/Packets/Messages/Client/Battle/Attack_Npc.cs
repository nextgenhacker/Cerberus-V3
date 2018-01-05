﻿namespace CR.Servers.CoC.Packets.Messages.Client.Battle
{
    using CR.Servers.CoC.Core.Network;
    using CR.Servers.CoC.Extensions.Game;
    using CR.Servers.CoC.Extensions.Helper;
    using CR.Servers.CoC.Files;
    using CR.Servers.CoC.Files.CSV_Logic.Logic;
    using CR.Servers.CoC.Logic;
    using CR.Servers.CoC.Packets.Messages.Server.Battle;
    using CR.Servers.Extensions.Binary;

    internal class Attack_Npc : Message
    {
        internal NpcData Npc;

        public Attack_Npc(Device Device, Reader Reader) : base(Device, Reader)
        {
        }

        internal override short Type => 14134;

        internal override void Decode()
        {
            this.Npc = this.Reader.ReadData<NpcData>();
        }

        internal override void Process()
        {
            if (this.Npc != null)
            {
                // if (this.Data.SinglePlayer)
                {
                    //if (this.Npc.AlwaysUnlocked ||  this.Device.GameMode.Level.Player.NpcMapProgress.CanAttackNPC(this.Npc))
                    {
                        int Index = this.Device.GameMode.Level.Player.NpcMapProgress.FindIndex(N => N.Data == this.Npc.GlobalId);

                        if (Index < 0)
                        {
                            if (this.Npc.GlobalId == 17000000)
                            {
                                this.Device.GameMode.Level.Player.Mission_Finish(21000002);
                            }
                            else if (this.Npc.GlobalId == 17000001)
                            {
                                this.Device.GameMode.Level.Player.Mission_Finish(21000009);
                            }

                            this.Device.GameMode.Level.Player.NpcMapProgress.Add(this.Npc, 3);
                        }
                        new Npc_Data(this.Device) {NpcHome = LevelFile.Files[this.Npc.LevelFile], Npc_ID = this.Npc.GlobalId}
                            .Send();
                    }
                }
            }
        }
    }
}