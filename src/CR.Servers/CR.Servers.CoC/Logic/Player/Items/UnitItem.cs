﻿namespace CR.Servers.CoC.Logic
{
    using System.Collections.Generic;
    using CR.Servers.CoC.Extensions.Helper;
    using CR.Servers.Extensions.Binary;
    using CR.Servers.Extensions.List;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    internal class UnitItem : Item
    {
        [JsonProperty] internal long DonatorId;
        [JsonProperty] internal int Level;

        public UnitItem()
        {
        }

        public UnitItem(int Data, int Count, int Level) : base(Data, Count)
        {
            this.Level = Level;
        }

        public UnitItem(int Data, int Count, int Level, long DonatorId) : base(Data, Count)
        {
            this.Level = Level;
            this.DonatorId = DonatorId;
        }

        internal override int Checksum => base.Checksum + this.Level;

        internal override void Decode(Reader Reader)
        {
            base.Decode(Reader);
            this.Level = Reader.ReadInt32();
        }

        internal override void Encode(List<byte> Writer)
        {
            base.Encode(Writer);
            Writer.AddInt(this.Level);
        }

        internal override JObject Save()
        {
            JObject Json = base.Save();

            Json.Add("lvl", this.Level);

            if (this.DonatorId > 0)
            {
                Json.Add("donator_id", this.DonatorId);
            }

            return Json;
        }

        internal override void Load(JToken Token)
        {
            base.Load(Token);
            JsonHelper.GetJsonNumber(Token, "lvl", out this.Level);

            if (JsonHelper.GetJsonNumber(Token, "donator_id", out long Id))
            {
                this.DonatorId = Id;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is UnitItem Item)
            {
                return Item.Data == this.Data && Item.Level == this.Level;
            }

            return false;
        }
    }
}