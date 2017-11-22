﻿using CR.Servers.CoC.Core;
using CR.Servers.CoC.Extensions;
using CR.Servers.CoC.Extensions.Helper;
using CR.Servers.CoC.Files.CSV_Logic.Logic;
using CR.Servers.Logic.Enums;
using Newtonsoft.Json.Linq;

namespace CR.Servers.CoC.Logic
{
    internal class ResourceProductionComponent : Component
    {
        internal int ResourceMax;
        internal int ResourcePer100Hours;

        internal ResourceData ProducesResource;

        internal Timer Timer;

         internal int AvailableResources
         {
             get
             {
                 if (this.ResourcePer100Hours > 0)
                 {
                     int MaxTime = this.MaxTime;

                     if (MaxTime > 0)
                     {
                         int RemainingSeconds = this.Timer.GetRemainingSeconds(this.Parent.Level.Player.LastTick);

                         if (RemainingSeconds > 0)
                         {
                             return (int)(Extension.Pair(RemainingSeconds >> 31, this.ResourcePer100Hours) * (ulong)(MaxTime - RemainingSeconds) / 360000L);
                         }

                         return this.ResourceMax;
                     }
                 }

                 return 0;
             }
         }

         internal int MaxTime
         {
             get
             {
                 if (this.ResourcePer100Hours > 0)
                     return (int)(360000L * this.ResourceMax / this.ResourcePer100Hours);
                 return 0;
             }
         }

         internal override int Checksum => this.Timer.GetRemainingSeconds(this.Parent.Level.Player.LastTick) + this.ResourceMax + this.ResourcePer100Hours;

         internal override int Type => 5;

         public ResourceProductionComponent(GameObject GameObject) : base(GameObject)
         {
             this.Timer = new Timer();
             this.SetProduction();
         }

         internal void CollectResources()
         {
             int AvailableResources = this.AvailableResources;

             if (this.Parent.Level.State == State.LOGGED)
             {
                 if (AvailableResources > 0)
                 {
                     int AvailableStorage = this.Parent.Level.Player.GetAvailableResourceStorage(this.ProducesResource);

                     if (AvailableStorage > 0)
                     {
                         if (AvailableResources > AvailableStorage)
                         {
                             AvailableResources = AvailableStorage;
                         }

                         this.DecreaseResources(AvailableResources);
                         this.Parent.Level.Player.Resources.Add(this.ProducesResource, AvailableResources);
                     }
                 }
             }
             else
                 Logging.Error(this.GetType(), "Unable to collect the resources while the player is not in home.");
         }

         internal void DecreaseResources(int Decrease)
         {
             int AvailableResources = this.AvailableResources;
             int CollectedResources = Math.Min(AvailableResources, Decrease);

             long v4 = 360000L * (AvailableResources - CollectedResources);
             ulong v6 = Extension.Pair((int)(v4 / this.ResourcePer100Hours), 0);

             if (this.ResourcePer100Hours > 0L)
             {
                 this.Timer.StartTimer(this.Parent.Level.Player.LastTick, this.MaxTime - (int)(v4 / this.ResourcePer100Hours));
             }
         }

         internal void SetProduction()
         {
             Building Building = (Building)this.Parent;
             BuildingData BuildingData = Building.BuildingData;

             int Level = Building.GetUpgradeLevel();

             if (Level >= 0 && !Building.Locked)
             {
                 this.ProducesResource = BuildingData.ProducesResourceData;
                 this.ResourcePer100Hours = BuildingData.ResourcePer100Hours[Level];
                 this.ResourceMax = BuildingData.ResourceMax[Level];

                 this.Timer.StartTimer(this.Parent.Level.Player.LastTick, this.MaxTime);
             }
             else
             {
                 this.ProducesResource = null;
                 this.ResourcePer100Hours = 0;
                 this.ResourceMax = 0;
             }
         }

         internal override void FastForwardTime(int Secs)
         {
             this.Timer.FastForward(Secs);
         }

         internal override void Load(JToken Json)
         {
             if (JsonHelper.GetJsonNumber(Json, "res_time", out int Time))
             {
                 if (Time <= this.MaxTime && Time > -1)
                     this.Timer.StartTimer(this.Parent.Level.Player.LastTick, Time);
                 else
                     this.Timer.StartTimer(this.Parent.Level.Player.LastTick, this.MaxTime);
             }
             else
                 this.Timer.StartTimer(this.Parent.Level.Player.LastTick, this.MaxTime);

             base.Load(Json);
         }

         internal override void Save(JObject Json)
         {
             Json.Add("res_time", this.Timer.GetRemainingSeconds(this.Parent.Level.Player.LastTick));

             base.Save(Json);
         }
    }
}