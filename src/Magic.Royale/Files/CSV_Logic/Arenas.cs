﻿using Magic.Royale.Files.CSV_Helpers;
using Magic.Royale.Files.CSV_Reader;

namespace Magic.Royale.Files.CSV_Logic
{
    internal class Arenas : Data
    {
        public Arenas(Row row, DataTable dt) : base(row, dt)
        {
            Load(row);
        }

        public string Name { get; set; }
        public string TID { get; set; }
        public string SubtitleTID { get; set; }
        public int Arena { get; set; }
        public string ChestArena { get; set; }
        public string TvArena { get; set; }
        public bool IsInUse { get; set; }
        public bool TrainingCamp { get; set; }
        public bool PVEArena { get; set; }
        public int TrophyLimit { get; set; }
        public int DemoteTrophyLimit { get; set; }
        public int SeasonTrophyReset { get; set; }
        public int ChestRewardMultiplier { get; set; }
        public int ChestShopPriceMultiplier { get; set; }
        public int RequestSize { get; set; }
        public int MaxDonationCountCommon { get; set; }
        public int MaxDonationCountRare { get; set; }
        public int MaxDonationCountEpic { get; set; }
        public string IconSWF { get; set; }
        public string IconExportName { get; set; }
        public string MainMenuIconExportName { get; set; }
        public string SmallIconExportName { get; set; }
        public int MatchmakingMinTrophyDelta { get; set; }
        public int MatchmakingMaxTrophyDelta { get; set; }
        public int MatchmakingMaxSeconds { get; set; }
        public string PvpLocation { get; set; }
        public string TeamVsTeamLocation { get; set; }
        public int DailyDonationCapacityLimit { get; set; }
        public int BattleRewardGold { get; set; }
        public string ReleaseDate { get; set; }

    }
}
