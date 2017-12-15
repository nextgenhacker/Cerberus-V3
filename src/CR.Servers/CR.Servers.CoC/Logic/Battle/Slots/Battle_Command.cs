﻿using System.Collections.Generic;
using CR.Servers.CoC.Packets;
using Newtonsoft.Json.Linq;

namespace CR.Servers.CoC.Logic.Battle.Slots
{
    public class Battle_Command : List<Command>
    {
        internal JArray Save()
        {
            JArray Array = new JArray();

            this.ForEach(Item =>
            {
                Array.Add(Item.Save());
            });

            return Array;
        }
    }
}