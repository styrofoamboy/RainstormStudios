using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RainstormStudios.Scheduling
{
    public class ScheduleManager
    {
        #region Global Objects
        //***************************************************************************
        // Global Variables
        // 
        const string _modName = "ScheduleManager";
        private ScheduledEventCollection events = null;
        private int nextEvent = -1;
        //***************************************************************************
        // Public Fields
        // 
        /// <summary>
        /// Gets the array of ScheduledEvent objects.
        /// </summary>
        public ScheduledEventCollection Events
        {
            get { return events; }
        }
        /// <summary>
        /// Gets the count of ScheduledEvent objects stored in the internal list.
        /// </summary>
        public long Count
        {
            get { return events.Count; }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        /// <summary>
        /// A private constructor called internally to create a 'deep' copy of the current list object.
        /// </summary>
        /// <param name="newEvents">The variable to store the new ScheduledEventList object into.</param>
        private ScheduleManager(ScheduledEventCollection newEvents)
        {
            events = newEvents;
        }
        /// <summary>
        /// Creates a list array of ScheduledEvent objects and sorts them by the times they should execute.
        /// </summary>
        /// <param name="textOut">An array of ScheduleEventParams objects used to build the ScheduledEvent objects from.</param>
        public ScheduleManager(ScheduledEventParamsCollection schedParams)
        {
            ScheduledEventCollection unsorted = new ScheduledEventCollection();
            for (int i = 0; i < schedParams.Count; i++)
            {
                // We'll set this to true if all the stats match.
                bool validEvent = false;

                // Make sure we're between the Start and End dates before we start parsing anything.
                if ((DateTime.Now > schedParams[i].StartDate) && (schedParams[i].EndDate.Ticks == 0 || schedParams[i].EndDate > DateTime.Now))
                {
                    switch (schedParams[i].EventFreq)
                    {
                        case EventFrequency.daily:
                            if (schedParams[i].EventInterval == 1)
                            {
                                // This event occurs every day, so we'll set it to valid
                                validEvent = true;
                            }
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
                        case EventFrequency.weekly:
                            if (schedParams[i].EventInterval == 1)
                            {
                                // Event fires every week, so we just have to determine if this
                                //   is one of the event's specified days.
                                if (schedParams[i].DaysOfWeek.ToLower().IndexOf(DateTime.Now.ToString("dddd").ToLower()) > -1)
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
                                int sWeekOfYear = (int)Math.Round(Convert.ToDouble(dt.DayOfYear / 7), MidpointRounding.AwayFromZero);
                                int nWeekOfYear = (int)Math.Round(Convert.ToDouble(DateTime.Now.DayOfYear / 7), MidpointRounding.AwayFromZero);
                                
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
                        case EventFrequency.monthly:
                            if (schedParams[i].MonthsOfYear.ToLower().IndexOf(Convert.ToString((MonthOfYear)DateTime.Now.Month).ToLower()) > -1)
                            {
                                // Valid month for this event.  Keep parsing.
                                if (schedParams[i].DayOfMonth > 0)
                                {
                                    // This event only fires on a specific day.
                                    if (DateTime.Now.Day == schedParams[i].DayOfMonth)
                                        validEvent = true;
                                }
                                else
                                {
                                    // This event fires every X occurance of a specific day of the week.
                                    if (DateTime.Now.DayOfWeek.ToString().ToLower() == schedParams[i].DaysOfWeek.ToLower())
                                    {
                                        // Right day, now we just have to figure out if it's the correct
                                        //   occurance of that day.
                                        // TODO :: Add code to do above here.
                                    }
                                }
                            }
                            break;
                    }
                }

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
                        if (schedParams[i].UntilDuration != null && schedParams[i].UntilDuration.TotalSeconds > 0)
                            evtStop = evtTime.Add(schedParams[i].UntilDuration);
                        else if (schedParams[i].UntilTimeOfDay.TotalSeconds > 0)
                            evtStop = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, schedParams[i].UntilTimeOfDay.Hours, schedParams[i].UntilTimeOfDay.Minutes, 00);
                        else
                            evtStop = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
                        // Now we start adding events until the evtTime value exceeds the evtStop value;
                        while (evtTime.Ticks <= evtStop.Ticks)
                        {
                            string evtName;
                            if (schedParams[i].ParentParams != null)
                                evtName = schedParams[i].ParentParams.RollupName;
                            else
                                evtName = schedParams[i].TimeOfDay.ToString().Substring(0, 8);
                            unsorted.Add(new ScheduledEvent(i, evtName, evtTime.TimeOfDay));
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
                        string evtName;
                        if (schedParams[i].ParentParams != null)
                            evtName = schedParams[i].ParentParams.RollupName + " [" + schedParams[i].TimeOfDay.ToString().Substring(0, 8) + "]";
                        else
                            evtName = schedParams[i].TimeOfDay.ToString().Substring(0, 8);
                        unsorted.Add(new ScheduledEvent(i, evtName, schedParams[i].TimeOfDay));
                    }
                }
            }

            // Once we're all done adding all valid events to the ScheduledEventList object, we
            //   have to 'sort' them into the proper order.  We use a simple 'bubble' sort
            //   to accomplish this, since there should never be so many events as to make
            //   this impractical.
            for (int r = 0; r < unsorted.Count; r++)
                for (int t = 0; t < unsorted.Count; t++)
                    if (unsorted[t].IsAfter(unsorted[r]))
                        unsorted.Move(t, r);

            // Then, we Clone the (now) sorted list into our global variable reference
            events = unsorted.Clone() as ScheduledEventCollection;

            // After that, we need to find the first event in the list who's scheduled
            //   time has not already passed.
            GetNextEvent();

            #region Debug Output
#if VERBOSE
            foreach (ScheduledEvent evnt in events)
                Console.WriteLine("{0}: {1} [{2}]", evnt.EventID, evnt.EventName, evnt.ScheduledTime.ToString().Substring(0, 8));
#endif
            #endregion
        }
        //***************************************************************************
        // Depreciated Class Constructor
        // 
        /*
            ScheduledEventList unsorted = new ScheduledEventList();
            for (int i = 0; i < RollupParams.Length; i++)
                if (RollupParams[i].EventInterval.TotalSeconds > 0)
                {
#if DEBUG
                    Console.WriteLine("Added interval event...");
                    Console.WriteLine(RollupParams[i].EventInterval.ToString());
#endif
                    DateTime evtTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 00, 00, 00);
                    for (int f = 0; f <= (24 * 60 * 60) / RollupParams[i].EventInterval.TotalSeconds; f++)
                    {
                        evtTime = evtTime.AddSeconds(aosText[i].EventInterval.TotalSeconds);
                        string evtName = RollupParams[i].ToString().PadRight(20, ' ') + " [" + evtTime.ToString("hh:mm:ss") + "]";
                        unsorted.Add(new ScheduledEvent(i, evtName, evtTime));
                    }
                }
                else
                {
                    string evtName = RollupParams[i].ToString().PadRight(20, ' ') + " [" + RollupParams[i].EventTime.ToString("hh:mm:ss") + "]";
                    unsorted.Add(new ScheduledEvent(i, evtName, aosText[i].EventTime));
                }

            for (int r = 0; r < unsorted.Count; r++)
            {
                for (int t = 0; t < unsorted.Count; t++)
                {
                    if (unsorted[t] > unsorted[r])
                    {
                        unsorted.Move(t, r);
                    }
                }
            }
            // We clone the (now) sorted list into our global variable reference
            events = unsorted.Clone() as ScheduledEventList;
            // Then we loop through until we find the first event who's time has not
            //   already passed, and set the 'nextEvent' counter to that record.
            GetNextEvent();
#if DEBUG
            foreach (ScheduledEvent evnt in events)
                Console.WriteLine("{0}:  {1} [{2}]", evnt.EventID, evnt.EventName, evnt.ScheduledTime.ToString("hh:mm:ss"));
#endif
        */
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        /// <summary>
        /// Creates a 'deep' copy of the current AosSchedule object.
        /// </summary>
        /// <returns>An AosSchedule object which is a direct copy of the current AosSchedule object.</returns>
        public override object Clone()
        {
            ScheduleManager newSchedule = new ScheduleManager(events.Clone() as ScheduledEventCollection);
            return newSchedule;
        }
        /// <summary>
        /// Returns the next ScheduledEvent object in the List scheduled be executed.
        /// </summary>
        /// <returns>An ScheduledEvent object from the List that is next in line for execution.</returns>
        public ScheduledEvent GetNextEvent()
        {
            try
            {
                if (events != null && events.Count > 0)
                {
                    for (int i = 0; i < events.Count; i++)
                        if (events[i].WaitTime.ToString().Substring(0, 1) != "-")
                        {
                            nextEvent = i;
                            break;
                        }

                    #region Debug Output
#if DEBUG
                    if (nextEvent < 0)
                        Console.WriteLine("No 'next' event found...");
                    else
                        Console.WriteLine("Found 'good' next event: " + nextEvent);
#endif
                    #endregion
                    // If no event was found with a non-negative WaitTime, return Null.
                    if (nextEvent < 0)
                        return null;
                    else
                        return events[nextEvent];
                }
                else
                {
                    Console.WriteLine("Returning null... Events = null, or its count < 1.");
                    return null;
                }
            }
            catch (Exception ex)
            {
                #region Status Output
#if DEBUG
                Console.WriteLine("Error retrieving next event... {0}", ex.ToString());
#else
                //AosCommon.WriteToLogFile("GetNextEvent(): " + ex.ToString());
                Logger.Instance.WriteToLog(new LogMessage(LogMessage.SeverityLevel.Error, _modName, "Error getting next event: " + ex.Message));
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
                if (events.Count > 0)
                {
                    for (int i = 0; i < events.Count; i++)
                        if (events[i].WaitTime.ToString().Substring(0, 1) != "-")
                        {
                            nextEvent = i;
                            break;
                        }
                    return nextEvent;
                }
                else
                    return 0;
            }
            catch (Exception ex)
            {
                //AosCommon.WriteToLogFile("GetNextEvent(): " + ex.ToString());
                Logger.Instance.WriteToLog(new LogMessage(LogMessage.SeverityLevel.Warning, "AosSchedule", "Error retrieving next event ordinal: " + ex.Message));
                return 0;
            }
        }
        /// <summary>
        /// Returns the ScheduledEvent last found by the GetNextEvent() method.
        /// </summary>
        /// <returns>The ScheduledEvent last found by the GetNextEvent() method.</returns>
        public ScheduledEvent GetCurrentEvent()
        {
            try
            {
                if (events.Count > 0)
                    return events[nextEvent];
                else
                    return null;
            }
            catch (Exception ex)
            {
                //AosCommon.WriteToLogFile("GetCurrentEvent(): " + ex.ToString());
                Logger.Instance.WriteToLog(new LogMessage(LogMessage.SeverityLevel.Warning, "AosSchedule", "Error retrieving current event: " + ex.Message));
                return null;
            }
        }
        /// <summary>
        /// Retrieves the most recent event who's scheduled time has already elapsed.
        /// </summary>
        /// <returns>The ScheduledEvent who's scheduled time most recently elapsed.</returns>
        public ScheduledEvent GetMostRecentEvent()
        {
            try
            {
                if (nextEvent > 0)
                    return events[nextEvent - 1];
                else
                    return null;
            }
            catch (Exception ex)
            {
                //AosCommon.WriteToLogFile("GetMostRecentEvent(): " + ex.ToString());
                Logger.Instance.WriteToLog(new LogMessage(LogMessage.SeverityLevel.Warning, "AosSchedule", "Error retrieving most recent event: " + ex.Message));
                return null;
            }
        }
        /// <summary>
        /// Retrieves an ScheduledEvent object from the List by searching for the Event's original ordinal position (index) in the exterior array containing the event's details.
        /// </summary>
        /// <param name="EventID">The original ordinal position of the event you want to look for.</param>
        /// <returns>An ScheduledEvent object.</returns>
        public ScheduledEvent GetEventByID(int EventID)
        {
            if (events.Count > 0)
            {
                int i;
                for (i = 0; i < events.Count; i++)
                    if (events[i].EventID == EventID)
                        break;
                return events[i];
            }
            else
                return null;
        }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        private static ScheduledEventParamsCollection ParseSchedArray(ScheduledEventParams[] schedParams)
        {
            ScheduledEventParamsCollection newParams = new ScheduledEventParamsCollection();
            if (schedParams.Length > 0)
                for (int i = 0; i < schedParams.Length; i++)
                    newParams.Add(schedParams[i]);
            return newParams;
        }
        private static ScheduledEventCollection SortEvents(ScheduledEventCollection EventList)
        {
            for (int r = 0; r < EventList.Count; r++)
                for (int t = 0; t < EventList.Count; t++)
                    if (EventList[t].IsAfter(EventList[r]))
                        EventList.SwapPos(t, r);

            return EventList;
        }
        #endregion
    }
}
