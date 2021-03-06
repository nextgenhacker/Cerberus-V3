﻿using System;
using Magic.ClashOfClans.Files.CSV_Helpers;
using Magic.ClashOfClans.Files.CSV_Reader;

namespace Magic.ClashOfClans.Files.CSV_Logic
{
    internal class Experience_Levels : Data
    {
        public Experience_Levels(Row _Row, DataTable _DataTable) : base(_Row, _DataTable)
        {
            Load(_Row);
        }
        public string Name { get; set; }
        public int ExpPoints { get; set; }
    }
}
