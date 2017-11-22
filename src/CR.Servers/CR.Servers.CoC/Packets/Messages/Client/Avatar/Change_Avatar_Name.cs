﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CR.Servers.CoC.Core;
using CR.Servers.CoC.Logic;
using CR.Servers.CoC.Packets.Commands.Server;
using CR.Servers.Extensions.Binary;

namespace CR.Servers.CoC.Packets.Messages.Client.Avatar
{
    internal class Change_Avatar_Name : Message
    {
        internal override short Type => 10212;

        public Change_Avatar_Name(Device Device, Reader Reader) : base(Device, Reader)
        {

        }

        internal string AvatarName;
        internal bool NameSetByUser;

        internal override void Decode()
        {
            this.AvatarName = this.Reader.ReadString();
            this.NameSetByUser = this.Reader.ReadBoolean();
        }
        internal override void Process()
        {
            if (!this.Device.GameMode.CommandManager.ChangeNameOnGoing)
            {
                if (!string.IsNullOrEmpty(this.AvatarName))
                {
                    if (!this.Device.GameMode.Level.Player.NameSetByUser == this.NameSetByUser)
                    {
                        if (this.AvatarName.Length <= 16)
                        {
                            this.AvatarName = Resources.Regex.Replace(this.AvatarName, " ");

                            if (this.AvatarName.StartsWith(" "))
                            {
                                this.AvatarName = this.AvatarName.Remove(0, 1);
                            }

                            if (this.AvatarName.Length >= 2)
                            {
                                this.Device.GameMode.CommandManager.ChangeNameOnGoing = true;
                                this.Device.GameMode.CommandManager.AddCommand(new Name_Change_Callback(this.Device) {AvatarName = this.AvatarName, ChangeNameCount = this.Device.GameMode.Level.Player.ChangeNameCount });
                            }
                        }
                    }
                }
            }
        }
    }
}