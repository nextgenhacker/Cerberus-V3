﻿namespace CR.Servers.CoC.Packets.Debugs
{
    using CR.Servers.CoC.Logic;
    using CR.Servers.Logic.Enums;

    internal class Server_Status : Debug
    {
        public Server_Status(Device Device, params string[] Parameters) : base(Device, Parameters)
        {
            // Server_Status
        }

        internal override Rank RequiredRank
        {
            get
            {
                return Rank.Player;
            }
        }

        internal override void Process()
        {
            SendChatMessage("~ Server Status ~ \n" +
                "* Online Players : " + (Program.Op + 5000) + " \n" +
                "* RAM usage of the Emulator : " + System.Diagnostics.Process.GetCurrentProcess().WorkingSet64 / (1024 * 1024) + " MB / 32768 MB \n");
        }
    }
}