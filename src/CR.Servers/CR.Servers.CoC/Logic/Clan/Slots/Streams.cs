﻿namespace CR.Servers.CoC.Logic.Clan.Slots
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using CR.Servers.CoC.Core.Network;
    using CR.Servers.CoC.Packets.Messages.Server.Alliances;
    using CR.Servers.Extensions.List;
    using Newtonsoft.Json;

    internal class Streams
    {
        internal Alliance Alliance;

        [JsonProperty] internal int Seed;
        [JsonProperty] internal ConcurrentDictionary<long, StreamEntry> Slots;

        public Streams()
        {
            this.Slots = new ConcurrentDictionary<long, StreamEntry>();
        }

        public Streams(Alliance Alliance) : this()
        {
            this.Alliance = Alliance;
        }

        internal bool AddEntry(StreamEntry StreamEntry)
        {
            StreamEntry.LowId = Interlocked.Increment(ref this.Seed);

            if (this.Slots.TryAdd(StreamEntry.StreamId, StreamEntry))
            {
                if (this.Slots.Count > 50)
                {
                    if (this.Slots.TryRemove(this.Slots.Keys.First(), out StreamEntry Removed))
                    {
                        this.RemoveEntry(Removed);
                    }
                }

                foreach (Player Player in this.Alliance.Members.Connected.Values.ToArray())
                {
                    if (Player.Connected)
                    {
                        new Alliance_Stream_Entry(Player.Level.GameMode.Device) {StreamEntry = StreamEntry}.Send();
                    }
                    else
                    {
                        this.Alliance.Members.Connected.TryRemove(Player.UserId, out _);
                    }
                }
                return true;
            }

            return false;
        }

        internal void Remove(StreamEntry StreamEntry)
        {
            if (this.Slots.TryRemove(StreamEntry.StreamId, out StreamEntry Removed))
            {
                this.RemoveEntry(Removed);
            }
        }

        internal void Remove(long StreamId)
        {
            if (this.Slots.TryRemove(StreamId, out StreamEntry Removed))
            {
                this.RemoveEntry(Removed);
            }
        }

        internal void RemoveEntry(StreamEntry StreamEntry)
        {
            foreach (Player Player in this.Alliance.Members.Connected.Values.ToArray())
            {
                if (Player.Connected)
                {
                    new Alliance_Stream_Entry_Removed(Player.Level.GameMode.Device) {MessageId = StreamEntry.StreamId}.Send();
                }
                else
                {
                    this.Alliance.Members.Connected.TryRemove(Player.UserId, out _);
                }
            }
        }

        internal StreamEntry Get(long MessageID)
        {
            if (this.Slots.ContainsKey(MessageID))
            {
                return this.Slots[MessageID];
            }

            return null;
        }

        internal void Update(StreamEntry StreamEntry)
        {
            foreach (Player Player in this.Alliance.Members.Connected.Values.ToArray())
            {
                if (Player.Connected)
                {
                    new Alliance_Stream_Entry(Player.Level.GameMode.Device) {StreamEntry = StreamEntry}.Send();
                }
                else
                {
                    this.Alliance.Members.Connected.TryRemove(Player.UserId, out _);
                }
            }
        }

        internal void Encode(List<byte> Packet, long requestorId)
        {
            StreamEntry[] Streams = this.Slots.Values.ToArray();
            Packet.AddInt(Streams.Length);

            foreach (StreamEntry Stream in Streams)
            {
                Stream.RequesterId = requestorId;
                Stream.Encode(Packet);
            }
        }
    }
}