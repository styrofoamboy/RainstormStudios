using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RainstormStudios.Timing
{
    /// <summary>
    /// Creates, sorts, and contains a series of AosEvents objects and exposes methods for handling and controlling the events.
    /// </summary>
    public class Scheduler
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        ScheduleEventCollection
            _events;
        int
            _nextEvent;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public ScheduleEventCollection Events
        { get { return this._events; } }
        public int EventCount
        { get { return this._events.Count; } }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        /// <summary>
        /// A private constructor called internally to create a 'deep' copy of the current list object.
        /// </summary>
        /// <param name="newEvents">The variable to store the new AosEventList object into.</param>
        private Scheduler(ScheduleEventCollection newEvents)
        {
            this._events = newEvents;
            this._nextEvent = -1;
        }
        private Scheduler()
        {
            this._events = new ScheduleEventCollection();
            this._nextEvent = -1;
        }
        public Scheduler(ScheduleParamsCollection schedParams)
            : this()
        {
            this._events = new ScheduleEventCollection();
            for (int i = 0; i < schedParams.Count; i++)
            {
                // We'll set this to true is all the values are set correctly.
                bool validEvent = false;

                #region Determine if event is valid for today
                // Make sure we're between the Start and End dates before we even start parsing anything.
                if (DateTime.Now > schedParams[i].StartDate && (schedParams[i].EndDate.Ticks == 0 | schedParams[i].EndDate > DateTime.Now))
                {
                    switch (schedParams[i].EventReoccurance)
                    {
                        case EventOccurance.Daily:
                            if (schedParams[i].EventInterval == 1)
                                // This occurs every day, so set it to valid.
                                validEvent = true;
                            else
                            {
                                // Here's were we need to figure out if the current day is an
                                //   even divisor of EventInterval field, based on the StartDate
                                //   of the event.

                                // First thing we do is create a DateTime object equal to the 
                                //   StartDate of the event.
                                DateTime dt = schedParams[i].StartDate;

                                // We're ignoring the year in this equation, so if the StartDate's
                                //   DayOfYear value is greater than today's DayOfYear value, we need
                                //   to advance our DateTime object to Jan 1st.
                                if (dt.DayOfYear > DateTime.Now.DayOfYear)
                                    dt.AddDays(Convert.ToDouble(schedParams[i].EventInterval));

                                // Now, we slide the StartDate object forward in time, by the value
                                //   of the 'EventInterval' field until it is either greater than
                                //   or equal to today's 'DayOfYear' value.
                                while (dt.DayOfYear < DateTime.Now.DayOfYear)
                                    dt.AddDays(Convert.ToDouble(schedParams[i].EventInterval));

                                // If the two DateTime object's ended up with matching 'DayOfYear'
                                //   values, then we have a winner.
                                if (dt.DayOfYear == DateTime.Now.DayOfYear)
                                    validEvent = true;
                            }
                            break;
                        case EventOccurance.Weekly:
                            if (schedParams[i].EventInterval == 1)
                            {
                                WeeklyOccurance dayOfWeek = (WeeklyOccurance)Enum.Parse(typeof(WeeklyOccurance), DateTime.Now.DayOfWeek.ToString());
                                // Event fires every week, so we just have to determine if this
                                //   is one of the event's specified days.
                                if (schedParams[i].DaysOfWeek.HasFlag(dayOfWeek))
                                {
                                    // We found 'today' in the DaysOfWeek list, so add the event.
                                    validEvent = true;
                                }
                            }
                            else
                            {
                                // Again, we have to calculate if we've moved forward in time the
                                //   proper number of weeks before firing.
                                DateTime dt = schedParams[i].StartDate;
                                int sWeekOfYear = (int)System.Math.Round(Convert.ToDouble(dt.DayOfYear / 7), MidpointRounding.AwayFromZero);
                                int nWeekOfYear = (int)System.Math.Round(Convert.ToDouble(DateTime.Now.DayOfYear / 7), MidpointRounding.AwayFromZero);

                                // If the start week is higher, we've got to 'slide' foward in time till we come
                                //   back around to the first of the year.
                                if (sWeekOfYear > nWeekOfYear)
                                {
                                    while (sWeekOfYear <= 52)
                                        sWeekOfYear += schedParams[i].EventInterval;
                                    sWeekOfYear -= 52;
                                }

                                // Now, we slide the StartDate week forward in time, until it is greater
                                //   than or equal to the value of the current week.
                                while (sWeekOfYear < nWeekOfYear)
                                    sWeekOfYear += schedParams[i].EventInterval;

                                // If the two are equal, then we have a winner.
                                if (sWeekOfYear == nWeekOfYear)
                                    validEvent = true;
                            }
                            break;
                        case EventOccurance.Monthly:
                            MonthOfYear monOfYear = (MonthOfYear)Enum.Parse(typeof(MonthOfYear), DateTime.Now.ToString("MMMM"));
                            if (schedParams[i].MonthsOfYear.HasFlag(monOfYear))
                            {
                                // Valid month for this event.  Keep parsing.
                                if (schedParams[i].EventDates != null && schedParams[i].EventDates.Contains(DateTime.Now.Day))
                                {
                                    // This event only fires on specific day(s).
                                    validEvent = true;
                                }
                                else
                                {
                                    // This event fires every X occurance of a specific day of the week.
                                    WeeklyOccurance dayOfWeek = (WeeklyOccurance)Enum.Parse(typeof(WeeklyOccurance), DateTime.Now.DayOfWeek.ToString());
                                    if (schedParams[i].DaysOfWeek.HasFlag(dayOfWeek))
                                    {
                                        // Right day, now we just have to figure out if it's the correct
                                        //   occurance of that day.
                                        // TODO :: Add code to do above here.
                                        if (schedParams[i].EventInterval == 0)
                                            validEvent = true;
                                        else
                                        {
                                            int occCnt = 1;
                                            DateTime tmpDate = DateTime.Now.AddDays(-1);
                                            while (tmpDate.Day > 1)
                                            {
                                                if (tmpDate.DayOfWeek == DateTime.Now.DayOfWeek)
                                                    occCnt++;
                                                tmpDate = tmpDate.AddDays(-1);
                                            }
                                            if (occCnt == schedParams[i].EventInterval)
                                                validEvent = true;
                                        }
                                    }
                                }
                            }
                            break;
                    }
                }
                #endregion

                #region Create Event, if valid
                // If we've got a valid event schedule, we need to create the event(s).
                if (validEvent)
                {
                    if (schedParams[i].RepeatInterval.TotalSeconds > 0)
                    {
                        // Setup a repeating event.
                        // Start with the event's scheduled time to fire.
                        DateTime evtTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, schedParams[i].TimeOfDay.Hours, schedParams[i].TimeOfDay.Minutes, schedParams[i].TimeOfDay.Seconds);
                        // Then we determine the time at which the event stops repeating.
                        DateTime evtStop;
                        if (schedParams[i].RepeatUntilDuration != null && schedParams[i].RepeatUntilDuration.TotalSeconds > 0)
                            evtStop = evtTime.Add(schedParams[i].RepeatUntilDuration);
                        else if (schedParams[i].RepeatUntilTimeOfDay.TotalSeconds > 0)
                            evtStop = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, schedParams[i].RepeatUntilTimeOfDay.Hours, schedParams[i].RepeatUntilTimeOfDay.Minutes, 00);
                        else
                            evtStop = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
                        // Now we start adding events until the evtTime value exceeds the evtStop value;
                        while (evtTime.Ticks <= evtStop.Ticks)
                        {
                            string evtName = schedParams[i].TimeOfDay.ToString().Substring(0, 8);
                            this._events.Add(new ScheduleEvent(i, evtName, evtTime.TimeOfDay));
                            #region Debug Output
#if VERBOSE
                            Console.WriteLine("New Event @: {0}", evtTime);
#endif
                            #endregion
                            evtTime = evtTime.Add(schedParams[i].RepeatInterval);
                        }
                    }
                    else
                    {
                        // This is a just a single event.
                        string evtName = schedParams[i].TimeOfDay.ToString().Substring(0, 8);
                        this._events.Add(new ScheduleEvent(i, evtName, schedParams[i].TimeOfDay));
                    }
                }
                #endregion
            }

            // Once we're all done adding all valid events to the "_events" collection, we have to sort them.
            this._events.Sort(Collections.SortDirection.Ascending);

            // After that, we need to find the first event in the list who's scheduled
            //   time has not already passed.
            GetNextEvent();

            #region Debug Output
#if VERBOSE
            foreach (AosEvent evnt in events)
                Console.WriteLine("{0}: {1} [{2}]", evnt.EventID, evnt.EventName, evnt.ScheduledTime.ToString().Substring(0, 8));
#endif
            #endregion

        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        /// <summary>
        /// Creates a 'deep' copy of the current AosSchedule object.
        /// </summary>
        /// <returns>An AosSchedule object which is a direct copy of the current AosSchedule object.</returns>
        public object Clone()
        {
            Scheduler newSchedule = new Scheduler(this._events.Clone() as ScheduleEventCollection);
            return newSchedule;
        }
        /// <summary>
        /// Returns the next AosEvent object in the List scheduled be executed.
        /// </summary>
        /// <returns>An AosEvent object from the List that is next in line for execution.</returns>
        public ScheduleEvent GetNextEvent()
        {
            try
            {
                if (this._events != null && this._events.Count > 0)
                {
                    for (int i = 0; i < this._events.Count; i++)
                        if (this._events[i].WaitTime.ToString().Substring(0, 1) != "-")
                        {
                            this._nextEvent = i;
                            break;
                        }

                    #region Debug Output
#if DEBUG
                    if (this._nextEvent < 0)
                        Console.WriteLine("No 'next' event found...");
                    else
                        Console.WriteLine("Found 'good' next event: " + this._nextEvent);
#endif
                    #endregion
                    // If no event was found with a non-negative WaitTime, return Null.
                    if (this._nextEvent < 0)
                        return null;
                    else
                        return this._events[this._nextEvent];
                }
                else
                {
#if VERBOSE
                    Console.WriteLine("Returning null... Events = null, or its count < 1.");
#endif
                    return null;
                }
            }
            catch (Exception ex)
            {
                #region Status Output
#if DEBUG
                Console.WriteLine("Error retrieving next event... {0}", ex.ToString());
#else
                Logger.Instance.WriteToLog(new LogMessage(SeverityLevel.Error, "RainstormStudios.Scheduler", "Error getting next event: " + ex.Message));
#endif
                #endregion
                return null;
            }
        }
        /// <summary>
        /// Returns the ordinal position of the next event in the list to be fired.
        /// </summary>
        /// <returns>An integer value of the List's ordinal position for the next event scheduled to fire.</returns>
        public int GetNextEventOrdinal()
        {
            try
            {
                if (this._events.Count > 0)
                {
                    for (int i = 0; i < this._events.Count; i++)
                        if (this._events[i].WaitTime.ToString().Substring(0, 1) != "-")
                        {
                            this._nextEvent = i;
                            break;
                        }
                    return this._nextEvent;
                }
                else
                    return 0;
            }
            catch (Exception ex)
            {
                //AosCommon.WriteToLogFile("GetNextEvent(): " + ex.ToString());
                Logger.Instance.WriteToLog(new LogMessage(SeverityLevel.Warning, "RainstormStudios.Scheduler", "Error retrieving next event ordinal: " + ex.Message));
                return 0;
            }
        }
        /// <summary>
        /// Returns the AosEvent last found by the GetNextEvent() method.
        /// </summary>
        /// <returns>The AosEvent last found by the GetNextEvent() method.</returns>
        public ScheduleEvent GetCurrentEvent()
        {
            try
            {
                if (this._events.Count > 0)
                    return this._events[this._nextEvent];
                else
                    return null;
            }
            catch (Exception ex)
            {
                //AosCommon.WriteToLogFile("GetCurrentEvent(): " + ex.ToString());
                Logger.Instance.WriteToLog(new LogMessage(SeverityLevel.Warning, "RainstormStudios.Scheduler", "Error retrieving current event: " + ex.Message));
                return null;
            }
        }
        /// <summary>
        /// Retrieves the most recent event who's scheduled time has already elapsed.
        /// </summary>
        /// <returns>The AosEvent who's scheduled time most recently elapsed.</returns>
        public ScheduleEvent GetMostRecentEvent()
        {
            try
            {
                if (this._nextEvent > 0)
                    return this._events[this._nextEvent - 1];
                else
                    return null;
            }
            catch (Exception ex)
            {
                //AosCommon.WriteToLogFile("GetMostRecentEvent(): " + ex.ToString());
                Logger.Instance.WriteToLog(new LogMessage(SeverityLevel.Warning, "RainstormStudios.Scheduler", "Error retrieving most recent event: " + ex.Message));
                return null;
            }
        }
        /// <summary>
        /// Retrieves an AosEvent object from the List by searching for the Event's original ordinal position (index) in the exterior array containing the event's details.
        /// </summary>
        /// <param name="EventID">The original ordinal position of the event you want to look for.</param>
        /// <returns>An AosEvent object.</returns>
        public ScheduleEvent GetEventByID(int EventID)
        {
            if (this._events.Count > 0)
            {
                int i;
                for (i = 0; i < this._events.Count; i++)
                    if (this._events[i].EventID == EventID)
                        break;
                return this._events[i];
            }
            else
                return null;
        }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        private ScheduleParamsCollection ParseSchedArray(ScheduleParams[] schedParams)
        {
            ScheduleParamsCollection newParams = new ScheduleParamsCollection();
            if (schedParams.Length > 0)
                for (int i = 0; i < schedParams.Length; i++)
                    newParams.Add(schedParams[i]);
            return newParams;
        }
        #endregion
    }
}
