﻿using Magic.ClashOfClans.Files.CSV_Reader;
using System.Collections.Generic;
using System.IO;
using Magic.ClashOfClans.Core;
using Magic.ClashOfClans.Logic.Enums;

namespace Magic.ClashOfClans.Files
{
    internal static class CSV
    {
        internal static readonly Dictionary<int, string> Gamefiles = new Dictionary<int, string>();

        internal static Gamefiles Tables;
        internal static void Initialize()
        {
            Gamefiles.Add((int)Gamefile.Buildings, @"Gamefiles/logic/buildings.csv");
            Gamefiles.Add((int)Gamefile.Resources, @"Gamefiles/logic/resources.csv");
            Gamefiles.Add((int)Gamefile.Characters, @"Gamefiles/logic/characters.csv");
            Gamefiles.Add((int)Gamefile.Obstacles, @"Gamefiles/logic/obstacles.csv");
            Gamefiles.Add((int)Gamefile.Experience_Levels, @"Gamefiles/logic/experience_levels.csv");
            Gamefiles.Add((int)Gamefile.Traps, @"Gamefiles/logic/traps.csv");
            Gamefiles.Add((int)Gamefile.Globals, @"Gamefiles/logic/globals.csv");
            Gamefiles.Add((int)Gamefile.Npcs, @"Gamefiles/logic/npcs.csv");
            Gamefiles.Add((int)Gamefile.Decos, @"Gamefiles/logic/decos.csv");
            Gamefiles.Add((int)Gamefile.Missions, @"Gamefiles/logic/missions.csv");
            Gamefiles.Add((int)Gamefile.Spells, @"Gamefiles/logic/spells.csv");
            Gamefiles.Add((int)Gamefile.Heroes, @"Gamefiles/logic/heroes.csv");
            Gamefiles.Add((int)Gamefile.Leagues, @"Gamefiles/logic/leagues.csv");
            Gamefiles.Add((int)Gamefile.Variables, @"Gamefiles/logic/variables.csv");
            Gamefiles.Add((int)Gamefile.Village_Objects, @"Gamefiles/logic/village_objects.csv");
            Tables = new Gamefiles();

            foreach (var File in Gamefiles)
            {
                if (new FileInfo(File.Value).Exists)
                {
                    Tables.Initialize(new Table(File.Value), File.Key);
                }
                else
                {
                    throw new FileNotFoundException($"{File.Value} does not exist!");
                }
            }
            Logger.SayInfo(Gamefiles.Count + " CSV Files loaded and stored in memory.\n");
        }
    }
}