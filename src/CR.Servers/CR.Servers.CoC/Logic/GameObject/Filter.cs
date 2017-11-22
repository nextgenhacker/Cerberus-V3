﻿using System.Collections.Generic;
using CR.Servers.CoC.Core;
using CR.Servers.CoC.Files.CSV_Helpers;

namespace CR.Servers.CoC.Logic
{
    internal class Filter
    {
        internal GameObjectManager GameObjectManager;

        public Filter(GameObjectManager GameObjectManager)
        {
            this.GameObjectManager = GameObjectManager;
        }

        internal List<GameObject> GetGameObjectsByData(Data Data)
        {
            if (Data.GetDataType() == 1)
            {
                List<GameObject> Result = this.GameObjectManager.GameObjects[0][this.GameObjectManager.Map].FindAll(T => T.Data == Data);

                return Result;
            }

            return null;
        }

        internal GameObject GetObstacleById(int Id)
        {
            int Class = Id / 1000000 - 500;

            if (this.GameObjectManager.GameObjects.Length > Class)
            {
                var Obstacle = this.GameObjectManager.GameObjects[Class][this.GameObjectManager.Map].Find(g => g.Id == Id);

                if (Obstacle != null)
                    return Obstacle;

                Logging.Info(this.GetType(), "GameObject id " + Id + " not exist.");
            }
            return null;
        }

        internal GameObject GetGameObjectById(int Id)
        {
            int Class = Id / 1000000 - 500;

            if (this.GameObjectManager.GameObjects.Length > Class)
            {
                int Index = Id % 1000000;

                if (this.GameObjectManager.GameObjects[Class][this.GameObjectManager.Map].Count > Index)
                    return this.GameObjectManager.GameObjects[Class][this.GameObjectManager.Map][Index];

                Logging.Info(this.GetType(), "GameObject id " + Id + " not exist.");
            }

            return null;
        }

        internal int GetGameObjectCount(Data Data, int Level = 0)
        {
            if (Data.GetDataType() == 1)
            {
                int Count = 0;

                foreach (Building GameObject in this.GameObjectManager.GameObjects[0][0])
                {
                    if (GameObject.Data == Data && GameObject.GetUpgradeLevel() >= Level)
                    {
                        ++Count;
                    }
                }

                foreach (Building GameObject in this.GameObjectManager.GameObjects[0][1])
                {
                    if (GameObject.Data == Data && GameObject.GetUpgradeLevel() >= Level)
                    {
                        ++Count;
                    }
                }

                return Count;
            }

            return 0;
        }
    }
}