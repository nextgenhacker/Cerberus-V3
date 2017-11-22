﻿using CR.Servers.CoC.Files.CSV_Helpers;

namespace CR.Servers.CoC.Logic
{
    internal static class GameObjectFactory
    {
        internal static GameObject CreateGameObject(Data Data, Level Level)
        {
            switch (Data.GetDataType())
            {
                case 1:
                    return new Building(Data, Level);
                case 8:
                    return new Obstacle(Data, Level); //ClassID 3  but csv id 8
                /*case 4:
                    return new Trap(Data, Level);
                case 6:
                    return new Deco(Data, Level);*/
                case 39:
                    return new VillageObject(Data, Level); //ClassID 8  but csv id 39
                default:
                    return null;
            }
        }
    }
}