﻿using System.Collections.Generic;
using System.Linq;
using Magic.Royale.Extensions.List;
using Magic.Royale.Files;
using Magic.Royale.Files.CSV_Logic;
using Magic.Royale.Logic.Structure.Slots.Items;
using Resource = Magic.Royale.Logic.Enums.Resource;

namespace Magic.Royale.Logic.Structure.Slots
{
    internal class Resources : List<Slot>
    {
        internal Resources()
        {
            // Resources.
        }

        internal Resources(bool Initialize)
        {
            if (Initialize)
                this.Initialize();
        }

        internal int Gems => Get(Resource.Diamonds);

        internal int Get(int Gl_ID)
        {
            var i = FindIndex(R => R.Data == Gl_ID);

            if (i > -1)
                return this[i].Count;

            return 0;
        }

        internal int Get(Resource Global)
        {
            return Get(3000000 + (int) Global);
        }

        internal void Set(int Global, int Count)
        {
            var i = FindIndex(R => R.Data == Global);

            if (i > -1)
                this[i].Count = Count;
            else
                Add(new Slot(Global, Count));
        }

        internal void Set(Resource Resource, int Count)
        {
            Set(3000000 + (int) Resource, Count);
        }

        internal void Plus(int Global, int Count)
        {
            var i = FindIndex(R => R.Data == Global);

            if (i > -1)
                this[i].Count += Count;
            else Add(new Slot(Global, Count));
        }

        internal void Plus(Resource Resource, int Count)
        {
            Plus(3000000 + (int) Resource, Count);
        }

        internal bool Minus(int Global, int Count)
        {
            var i = FindIndex(R => R.Data == Global);

            if (i > -1)
                if (this[i].Count >= Count)
                {
                    this[i].Count -= Count;
                    return true;
                }

            return false;
        }

        internal void Minus(Resource _Resource, int _Value)
        {
            var Index = FindIndex(T => T.Data == 3000000 + (int) _Resource);

            if (Index > -1)
                this[Index].Count -= _Value;
        }

        internal byte[] ToBytes
        {
            get
            {
                var Packet = new List<byte>();

                Packet.AddInt(Count - 1);
                foreach (var Resource in this.Skip(1))
                {
                    Packet.AddInt(Resource.Data);
                    Packet.AddInt(Resource.Count);
                }

                return Packet.ToArray();
            }
        }

        /* [Obsolete]
         internal void ResourceChangeHelper(int GlobalID, int count)
         {
             int current = this.Get(GlobalID);
             int newResourceValue = Math.Max(current + count, 0);
             if (count >= 1)
             {
                 int resourceCap = this.Player.Resources_Cap.Get(GlobalID);
                 if (current < resourceCap)
                 {
                     if (newResourceValue > resourceCap)
                     {
                         newResourceValue = resourceCap;
                     }
                 }
             }
             this.Plus(GlobalID, count);
         }


         [Obsolete]
         internal void ResourceChangeHelper(Enums.Resource resource, int count)
         {
             int current = this.Get(resource);
             int newResourceValue = Math.Max(current + count, 0);
            if (count >= 1)
             {
                 int resourceCap = this.Player.Resources_Cap.Get(resource);
                 if (resourceCap > current)
                 {
                     if (newResourceValue > resourceCap)
                     {
                         newResourceValue = resourceCap;
                     }
                 }
             }
             this.Plus(resource, count);
         }*/

        internal void Initialize()
        {

        }
    }
}