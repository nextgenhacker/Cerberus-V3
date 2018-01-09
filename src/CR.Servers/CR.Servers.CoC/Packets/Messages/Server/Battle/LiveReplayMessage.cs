﻿namespace CR.Servers.CoC.Packets.Messages.Server.Battle
{
    using System.Collections.Generic;
    using CR.Servers.CoC.Logic;
    using CR.Servers.Extensions.List;

    internal class LiveReplayMessage : Message
    {
        internal int ClientTick;
        internal string ReplayJSON;

        internal List<Command> Commands;

        public LiveReplayMessage(Device Device, string replayJSON, List<Command> commands, int clientTick) : base(Device)
        {
            this.ClientTick = clientTick;
            this.ReplayJSON = replayJSON;
            this.Commands = commands;
        }

        internal override short Type => 24118;

        internal override void Encode()
        {
            this.Data.AddString(null);
            this.Data.AddCompressed(this.ReplayJSON, false);
            this.Data.AddInt(this.ClientTick);
            this.Data.AddInt(this.Commands.Count);

            this.Commands.ForEach(Command =>
            {
                this.Data.AddInt(Command.Type);
                Command.Encode(this.Data);
            });

            this.Data.AddInt(1);
        }
    }
}