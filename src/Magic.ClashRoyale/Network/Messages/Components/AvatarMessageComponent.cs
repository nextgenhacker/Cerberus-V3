﻿namespace Magic.Network.Messages.Components
{
    public class AvatarMessageComponent : MessageComponent
    {
        public int Unknown1;
        public int Unknown2;

        public long UserId;
        public long HomeId;
        public long UserId2;

        public string Username;

        public int Unknown3;
        public int Unknown4;
        public int Unknown5;
        public int Unknown6;

        public int Unknown7;

        public int Unknown8;
        public int Unknown9;
        public int Unknown10;

        public int Unknown11;

        public int Unknown12;

        public int Unknown13;
        public Slot[] Resources;

        public int Unknown14;

        public Slot[] Achievements;
        public Slot[] CompletedAchievements;
        public Slot[] Stats; // Like some side achievements type of resources.
        public Slot[] Characters;

        public int Unknown15;

        public int Unknown16;
        public int Unknown17;
        public int ExpPoints;
        public int ExpLevel;
        public int Unknown18;

        public int Unknown19;
        public ClanMessageComponent Clan;

        public int Unknown20;
        public int Unknown21;
        public int Unknown22;
        public int Unknown23;
        public int Unknown24;
        public int Unknown25;
        public int Unknown26;
        public int Unknown27;

        public override void ReadMessageComponent(MessageReader reader)
        {
            ThrowIfReaderNull(reader);

            Unknown1 = reader.ReadRRInt32(); // 0
            Unknown2 = reader.ReadRRInt32(); // 0

            UserId = reader.ReadRRInt64();
            HomeId = reader.ReadRRInt64();
            UserId2 = reader.ReadRRInt64(); // Another one?

            Username = reader.ReadString();

            Unknown3 = reader.ReadRRInt32(); // 0
            Unknown4 = reader.ReadRRInt32(); // 2
            Unknown5 = reader.ReadRRInt32(); // 315
            Unknown6 = reader.ReadRRInt32(); // 0

            Unknown7 = reader.ReadInt32(); // 0

            Unknown8 = reader.ReadRRInt32(); // 0
            Unknown9 = reader.ReadRRInt32(); // 0
            Unknown10 = reader.ReadRRInt32(); // 30

            Unknown11 = reader.ReadInt32(); // 0

            Unknown12 = reader.ReadRRInt32(); // 0

            Unknown13 = reader.ReadRRInt32(); // 7

            //TODO: Write function to decode slots.
            var count1 = reader.ReadRRInt32();
            Resources = new Slot[count1];
            for (int i = 0; i < count1; i++)
            {
                var resource = new Slot();
                resource.ReadSlot(reader);

                Resources[i] = resource;
            }

            // Could be another slot array.
            Unknown14 = reader.ReadRRInt32(); // 0

            var count2 = reader.ReadRRInt32();
            Achievements = new Slot[count2];
            for (int i = 0; i < count2; i++)
            {
                var achievement = new Slot();
                achievement.ReadSlot(reader);

                Achievements[i] = achievement;
            }

            var count3 = reader.ReadRRInt32();
            CompletedAchievements = new Slot[count3];
            for (int i = 0; i < count3; i++)
            {
                var achievement = new Slot();
                achievement.ReadSlot(reader);

                CompletedAchievements[i] = achievement;
            }

            var count4 = reader.ReadRRInt32();
            Stats = new Slot[count4];
            for (int i = 0; i < count4; i++)
            {
                var stat = new Slot();
                stat.ReadSlot(reader);

                Stats[i] = stat;
            }

            var count5 = reader.ReadRRInt32();
            Characters = new Slot[count5];
            for (int i = 0; i < count5; i++)
            {
                var character = new Slot();
                character.ReadSlot(reader);

                Characters[i] = character;
            }

            // Could be another slot array.
            Unknown15 = reader.ReadRRInt32(); // 0

            Unknown16 = reader.ReadRRInt32(); // Gems?
            Unknown17 = reader.ReadRRInt32(); // Gems?

            ExpPoints = reader.ReadRRInt32();
            ExpLevel = reader.ReadRRInt32();

            Unknown18 = reader.ReadRRInt32(); // 0

            // 9 When in clan.
            Unknown19 = reader.ReadRRInt32();
            if (Unknown19 > 0)
            {
                Clan = new ClanMessageComponent();
                Clan.ReadMessageComponent(reader);
            }

            Unknown20 = reader.ReadRRInt32(); // 1
            Unknown21 = reader.ReadRRInt32(); // 16
            Unknown22 = reader.ReadRRInt32(); // 0
            Unknown23 = reader.ReadRRInt32(); // 0
            Unknown24 = reader.ReadRRInt32(); // 11
            Unknown25 = reader.ReadRRInt32(); // 5
            Unknown26 = reader.ReadRRInt32(); // 3
            Unknown27 = reader.ReadRRInt32(); // 8
        }

        public override void WriteMessageComponent(MessageWriter writer)
        {
            ThrowIfWriterNull(writer);


        }

        // Temp stuff.
        public class Slot
        {
            public int Type;
            public int Id;
            public int Value;

            public void ReadSlot(MessageReader reader)
            {
                Type = reader.ReadRRInt32();
                Id = reader.ReadRRInt32();
                Value = reader.ReadRRInt32();
            }
        }
    }
}
