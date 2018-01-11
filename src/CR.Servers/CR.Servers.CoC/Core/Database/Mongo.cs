﻿namespace CR.Servers.CoC.Core.Database
{
    using CR.Servers.CoC.Core.Database.Models.Mongo;
    using MongoDB.Driver;

    internal class Mongo
    {
        private static IMongoClient Client;
        private static IMongoDatabase Database;

        internal static IMongoCollection<Clans> Clans;
        internal static IMongoCollection<Battles> Battles;
        internal static IMongoCollection<Players> Players;

        internal static int ClanSeed
        {
            get
            {
                Clans last = Mongo.Clans.Find(T => T.HighId == Constants.ServerId).Sort(Builders<Clans>.Sort.Descending(T => T.LowId)).Limit(1).SingleOrDefault();

                return last?.LowId ?? 0;
            }
        }

        internal static int BattleSeed
        {
            get
            {
                Battles last = Mongo.Battles.Find(T => T.HighId == Constants.ServerId).Sort(Builders<Battles>.Sort.Descending(T => T.LowId)).Limit(1).SingleOrDefault();

                return last?.LowId ?? 0;
            }
        }

        internal static int PlayerSeed
        {
            get
            {
                Players last = Mongo.Players.Find(T => T.HighId == Constants.ServerId).Sort(Builders<Players>.Sort.Descending(T => T.LowId)).Limit(1).SingleOrDefault();

                return last?.LowId ?? 0;
            }
        }

        internal static void Initialize()
        {
            Mongo.Client = new MongoClient("mongodb://127.0.0.1:27017");
            Mongo.Database = Mongo.Client.GetDatabase("ClashOfClans");

            Mongo.Clans = Mongo.Database.GetCollection<Clans>("Clans");
            Mongo.Battles = Mongo.Database.GetCollection<Battles>("Battles");
            Mongo.Players = Mongo.Database.GetCollection<Players>("Players");
        }
    }
}