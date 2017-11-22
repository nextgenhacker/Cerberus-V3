﻿using CR.Servers.CoC.Core;
using CR.Servers.CoC.Extensions;
using CR.Servers.CoC.Extensions.Game;
using CR.Servers.CoC.Extensions.Helper;
using CR.Servers.CoC.Files.CSV_Helpers;
using CR.Servers.CoC.Files.CSV_Logic.Logic;
using Newtonsoft.Json.Linq;

namespace CR.Servers.CoC.Logic
{
    internal class Building : GameObject
    {
        private int UpgradeLevel;

        internal bool Locked;
        internal bool BoostPause;

        internal Timer BoostTimer;
        internal Timer ConstructionTimer;

        internal BuildingData BuildingData => (BuildingData)this.Data;


        internal override int HeightInTiles => this.BuildingData.Height;

        internal override int WidthInTiles => this.BuildingData.Width;

        internal override int Type => 0;

        internal override int VillageType => this.BuildingData.VillageType;

        internal override int Checksum
        {
            get
            {
                int Checksum = 0;

                Checksum += base.Checksum;

                ResourceProductionComponent ResourceProductionComponent = this.ResourceProductionComponent;

                if (ResourceProductionComponent != null)
                {
                    Checksum += ResourceProductionComponent.Checksum;
                }

                ResourceStorageComponent ResourceStorageComponent = this.ResourceStorageComponent;

                if (ResourceStorageComponent != null)
                {
                    Checksum += ResourceStorageComponent.Checksum;
                }

                return Checksum;
            }
        }
        internal int RemainingConstructionTime => ConstructionTimer?.GetRemainingSeconds(this.Level.Player.LastTick) ?? 0;

        internal bool Boosted => this.BoostTimer != null;

        internal bool Constructing => this.ConstructionTimer != null;

        internal bool Gearing;

        internal bool UpgradeAvailable
        {
            get
            {
                if (!this.Constructing)
                {
                    var Data = this.BuildingData;

                    if (Data.MaxLevel > this.UpgradeLevel)
                    {
                        if (this.Level.Player.Village2)
                        {
                            return this.Level.GameObjectManager.TownHall2.GetUpgradeLevel() + 1 >=
                                   Data.TownHallLevel2[this.UpgradeLevel + 1];
                        }
                        return this.Level.GameObjectManager.TownHall.GetUpgradeLevel() + 1 >=
                               Data.TownHallLevel[this.UpgradeLevel + 1];
                    }
                }

                return false;
            }
        }

        internal UnitStorageComponent UnitStorageComponent => this.TryGetComponent(0, out Component Component) ? (UnitStorageComponent)Component : null;
        internal CombatComponent CombatComponent => this.TryGetComponent(1, out Component Component) ? (CombatComponent)Component : null;
        internal ResourceProductionComponent ResourceProductionComponent => this.TryGetComponent(5, out Component Component) ? (ResourceProductionComponent)Component : null;
        internal ResourceStorageComponent ResourceStorageComponent => this.TryGetComponent(6, out Component Component) ? (ResourceStorageComponent)Component : null;
        internal BunkerComponent BunkerComponent => this.TryGetComponent(7, out Component Component) ? (BunkerComponent)Component : null;
        internal UnitUpgradeComponent UnitUpgradeComponent => this.TryGetComponent(9, out Component Component) ? (UnitUpgradeComponent)Component : null;


        internal int GetUpgradeLevel() => this.UpgradeLevel;

        public Building(Data Data, Level Level) : base(Data, Level)
        {
            BuildingData BuildingData = this.BuildingData;


            if (BuildingData.IsTrainingHousing)
            {
                this.AddComponent(new UnitStorageComponent(this));
            }

            if (BuildingData.IsDefense)
            {
                AddComponent(new CombatComponent(this));
            }

            if (!string.IsNullOrEmpty(BuildingData.ProducesResource))
            {
                this.AddComponent(new ResourceProductionComponent(this));
            }

            if (BuildingData.CanStoreResources)
            {
                this.AddComponent(new ResourceStorageComponent(this)
                {
                    MaxArray = BuildingData.GetResourceMaxArray(0)
                });
            }

            if (BuildingData.UnitProduction[0] > 0)
            {
                this.AddComponent(new UnitProductionComponent(this));
            }       

            if (BuildingData.Bunker)
            {
                this.AddComponent(new BunkerComponent(this));
            }

            if (BuildingData.UpgradesUnits)
            {
                this.AddComponent(new UnitUpgradeComponent(this));
            }
        }

        internal void FinishConstruction()
        {

            BuildingData Data = this.BuildingData;
            if (this.Gearing)
            {           
                this.Gearing = false;

                this.CombatComponent.GearUp = 1;

                if (CombatComponent.AltAttackMode)
                {
                    CombatComponent.AttackMode = true;
                    CombatComponent.AttackModeDraft = true;
                }
            }
            else
            {
                if (this.UpgradeLevel + 1 > Data.MaxLevel)
                {
                    Logging.Error(this.GetType(),
                        "Unable to upgrade the building because the level is out of range! - " + Data.Name + ".");
                    this.SetUpgradeLevel(Data.MaxLevel);
                }
                else
                    this.SetUpgradeLevel(this.UpgradeLevel + 1);
            }

            if (this.VillageType == 0)
            {
                this.Level.WorkerManager.DeallocateWorker(this);
            }
            else
            {
                this.Level.WorkerManagerV2.DeallocateWorker(this);
            }

            if (!Data.IsTroopHousingV2)
            {
                this.Level.Player.AddExperience(GamePlayUtil.TimeToXp(Data.GetBuildTime(this.UpgradeLevel)));
            }
            else
            {
                var troopHousing = Level.GameObjectManager.Filter.GetGameObjectCount(this.BuildingData);

                if (troopHousing >= 0)
                {
                    int Time = Globals.TroopHousingV2BuildTimeSeconds.Length == troopHousing ? Globals.TroopHousingV2BuildTimeSeconds[troopHousing - 1] : Globals.TroopHousingV2BuildTimeSeconds[troopHousing];
                    this.Level.Player.AddExperience(GamePlayUtil.TimeToXp(Time));
                }
                else
                {
                    Logging.Error(this.GetType(), "TroopHousingV2 count is below zero when trying to get build time");
                }
            }

            this.ConstructionTimer = null;

        }

        internal void StartUpgrade()
        {
            int Time = this.BuildingData.GetBuildTime(this.UpgradeLevel + 1);

            if (!this.Constructing)
            {
                if (this.VillageType == 0)
                {
                    this.Level.WorkerManager.AllocateWorker(this);
                }
                else
                {
                    this.Level.WorkerManagerV2.AllocateWorker(this);
                }

                if (Time <= 0)
                {
                    this.FinishConstruction();
                }
                else
                {
                    this.ResourceProductionComponent?.CollectResources();

                    this.ConstructionTimer = new Timer();
                    this.ConstructionTimer.StartTimer(this.Level.Player.LastTick, Time);
                }
            }
        }

        internal void StartGearing()
        {
            int Time = this.BuildingData.GetGearUpTime(1);
            if (this.CombatComponent.GearUp != 1)
            {
                if (!this.Constructing)
                {
                    if (this.VillageType == 0)
                    {
                        this.Level.WorkerManager.AllocateWorker(this);
                    }
                    else
                    {
                        this.Level.WorkerManagerV2.AllocateWorker(this);
                    }

                    this.Gearing = true;

                    if (Time <= 0)
                    {
                        this.FinishConstruction();
                    }
                    else
                    {
                        this.ResourceProductionComponent?.CollectResources();

                        this.ConstructionTimer = new Timer();
                        this.ConstructionTimer.StartTimer(this.Level.Player.LastTick, Time);
                    }
                }
            }
            else
            {
                Logging.Error(this.GetType(),  "Unable to gear up the building because the buidling is already geared up! - " + Data.Name + ".");
            }
        }

        internal void SpeedUpConstruction()
        {
            if (this.Level.Player != null)
            {
                if (this.Constructing)
                {
                    int Cost = GamePlayUtil.GetSpeedUpCost(this.RemainingConstructionTime, this.BuildingData.VillageType, 100);

                    if (this.Level.Player.HasEnoughDiamonds(Cost))
                    {
                        this.Level.Player.UseDiamonds(Cost);
                        this.FinishConstruction();
                    }
                }
            }
        }

        internal void SetUpgradeLevel(int UpgradeLevel)
        {
            this.UpgradeLevel = UpgradeLevel;

            ResourceProductionComponent ResourceProductionComponent = this.ResourceProductionComponent;

            ResourceProductionComponent?.SetProduction();

            ResourceStorageComponent ResourceStorageComponent = this.ResourceStorageComponent;

            if (ResourceStorageComponent != null)
            {
                if (UpgradeLevel > -1)
                {
                    ResourceStorageComponent.SetMaxArray(this.BuildingData.GetResourceMaxArray(UpgradeLevel));
                    this.Level.ComponentManager.RefreshResourceCaps();
                }
            }
            
            UnitStorageComponent UnitStorageComponent = this.UnitStorageComponent;

            if (UnitStorageComponent != null)
            {
                if (UpgradeLevel > -1)
                {
                    UnitStorageComponent.SetStorage();
                }
            }
        }

        internal override void FastForwardTime(int Seconds)
        {
            if (this.Constructing)
            {
                this.ConstructionTimer.FastForward(Seconds);
            }

            if (this.Boosted)
            {
                this.BoostTimer.FastForward(Seconds);
            }

            base.FastForwardTime(Seconds);
        }


        internal override void Tick()
        {
            if (this.Constructing)
            {
                if (this.ConstructionTimer.GetRemainingSeconds(this.Level.Player.LastTick) <= 0)
                {
                    this.FinishConstruction();
                }
            }

            if (this.Boosted)
            {
                if (this.BoostTimer.GetRemainingSeconds(this.Level.Player.LastTick) <= 0)
                {
                    this.BoostTimer = null;
                }
            }

            base.Tick();
        }

        internal override void Load(JToken Json)
        {
            BuildingData Data = this.BuildingData;

            if (Data.Locked)
            {
                JsonHelper.GetJsonBoolean(Json, "locked", out this.Locked);
            }

            if (JsonHelper.GetJsonNumber(Json, "const_t", out int ConstructionTime) && JsonHelper.GetJsonNumber(Json, "const_t_end", out int ConstructionTimeEnd))
            {
                if (ConstructionTime > -1)
                {
                    var startTime = (int) TimeUtils.ToUnixTimestamp(this.Level.Player.LastTick);
                    var duration = ConstructionTimeEnd - startTime;
                    if (duration < 0)
                        duration = 0;
                    //ConstructionTime = Math.Min(ConstructionTime, Data.GetBuildTime(this.UpgradeLevel + 1));

                    this.ConstructionTimer = new Timer();
                    this.ConstructionTimer.StartTimer(this.Level.Player.LastTick, duration);

                    if (this.VillageType == 0)
                        this.Level.WorkerManager.AllocateWorker(this);
                    else
                        this.Level.WorkerManagerV2.AllocateWorker(this);
                }
            }

            if (JsonHelper.GetJsonNumber(Json, "boost_t", out int BoostTime) && JsonHelper.GetJsonNumber(Json, "boost_t_end", out int BoostTimeEnd))
            {
                if (BoostTime > -1)
                {
                    var startTime = (int)TimeUtils.ToUnixTimestamp(this.Level.Player.LastTick);
                    var duration = BoostTimeEnd - startTime;
                    if (duration < 0)
                        duration = 0;
                    this.BoostTimer = new Timer();
                    this.BoostTimer.StartTimer(this.Level.Player.LastTick, duration);
                }
            }

            if (JsonHelper.GetJsonBoolean(Json, "gearing", out bool Gearing))
                this.Gearing = Gearing;


            JsonHelper.GetJsonBoolean(Json, "boost_pause", out this.BoostPause);

            if (JsonHelper.GetJsonNumber(Json, "lvl", out int Level))
            {
                if (Level < -1)
                {
                    Logging.Error(this.GetType(),
                        "An error has been throwed when the loading of building - Load an illegal upgrade level. Level : " +
                        Level);
                    this.SetUpgradeLevel(0);
                }
                else if (Level > Data.MaxLevel)
                {
                    Logging.Error(this.GetType(),
                        $"An error has been throwed when the loading of building - Loaded upgrade level {Level + 1} is over max! (max = {Data.MaxLevel + 1}) id {this.Id} data id {Data.GlobalId}");
                    this.SetUpgradeLevel(Data.MaxLevel);
                }
                else
                    this.SetUpgradeLevel(Level);
            }

            base.Load(Json);
        }

        internal override void Save(JObject Json)
        {
            Json.Add("lvl", this.UpgradeLevel);

            if (this.Locked)
                Json.Add("locked", this.Locked);

            if (Gearing)
            {
                Json.Add("gearing", this.Gearing);
            }

            if (this.ConstructionTimer != null)
            {
                Json.Add("const_t", this.ConstructionTimer.GetRemainingSeconds(this.Level.Player.LastTick));
                Json.Add("const_t_end", this.ConstructionTimer.EndTime);
            }

            if (this.BoostTimer != null)
            {
                Json.Add("boost_t", this.BoostTimer.GetRemainingSeconds(this.Level.Player.LastTick));
                Json.Add("boost_t_end", this.BoostTimer.EndTime);
            }


            if (this.BoostPause)
                Json.Add("boost_pause", this.BoostPause);

            base.Save(Json);
        }
    }
}
