using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RainstormStudios.Timing
{
    /// <summary>
    /// Used by the <see cref="T:RainstormStudios.Timing.Schedule"/> to determine the frequency with which an event occurs.
    /// </summary>
    public enum EventOccurance
    {
        Once = 0,
        Daily,
        Weekly,
        Monthly,
        Hourly
    }
    /// <summary>
    /// Used by the <see cref="T:RainstormStudios.Timing.Schedule"/> class to determine on which months an event should trigger.
    /// </summary>
    [Flags]
    public enum MonthOfYear : uint
    {
        January = 1 << 0,
        Febuary = 1 << 1,
        March = 1 << 2,
        April = 1 << 3,
        May = 1 << 4,
        June = 1 << 5,
        July = 1 << 6,
        August = 1 << 7,
        September = 1 << 8,
        October = 1 << 9,
        November = 1 << 10,
        December = 1 << 11
    }
    [Flags]
    public enum EventOccuranceType : uint
    {
        First = 1 << 0,
        Second = 1 << 1,
        Third = 1 << 2,
        Fourth = 1 << 3,
        Fith = 1 << 4,
        Last = 1 << 5
    }
    public enum WeeklyOccurance : uint
    {
        Sunday = 1 << 0,
        Monday = 1 << 1,
        Tuesday = 1 << 2,
        Wednesday = 1 << 3,
        Thursday = 1 << 4,
        Friday = 1 << 5,
        Saturday = 1 << 6
    }
}
