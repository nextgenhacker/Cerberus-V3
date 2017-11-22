﻿using CR.Servers.CoC.Extensions;
using System;

namespace CR.Servers.CoC.Logic
{
    internal class Timer
    {
        internal int Seconds;
        internal DateTime StartTime;
        internal int EndTime;

        internal bool Started => this.EndTime != 0;


        internal DateTime GetStartTime => StartTime;
        internal DateTime GetEndTime => TimeUtils.FromUnixTimestamp(EndTime);

        internal Timer()
        {
            StartTime = new DateTime();
            EndTime = 0;
            Seconds = 0;
        }
        

        internal void StartTimer(DateTime time, int durationSeconds)
        {
            StartTime = time;
            EndTime = (int)TimeUtils.ToUnixTimestamp(time) + durationSeconds;
            Seconds = durationSeconds;
        }

        internal void StopTimer()
        {
            this.EndTime = 0;
            this.Seconds = 0;
        }

        internal void FastForward(int seconds)
        {
            Seconds -= seconds;
            EndTime -= seconds;
        }

        internal void IncreaseTimer(int seconds)
        {
            Seconds += seconds;
            EndTime += seconds;
        }

        internal int GetRemainingSeconds(DateTime time)
        {
            int result = Seconds - (int)time.Subtract(StartTime).TotalSeconds;
            if (result <= 0)
            {
                result = 0;
            }
            return result;
        }


        internal int GetRemainingSeconds(DateTime time, bool boost, DateTime boostEndTime = default(DateTime), float multiplier = 0f)
        {
            int result;
            if (!boost)
            {
                result = Seconds - (int)time.Subtract(StartTime).TotalSeconds;
            }
            else
            {
                if (boostEndTime >= time)
                    result = Seconds - (int)(time.Subtract(StartTime).TotalSeconds * multiplier);
                else
                {
                    var boostedTime = (float)time.Subtract(StartTime).TotalSeconds - (float)(time - boostEndTime).TotalSeconds;
                    var notBoostedTime = (float)time.Subtract(StartTime).TotalSeconds - boostedTime;

                    result = Seconds - (int)(boostedTime * multiplier + notBoostedTime);
                }
            }
            if (result <= 0)
                result = 0;
            return result;
        }
    }
}