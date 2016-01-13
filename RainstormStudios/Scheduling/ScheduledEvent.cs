using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RainstormStudios.Scheduling
{
    /// <summary>
    /// A class that contains information about an event (who's details are stored in a seperate array) such
    /// as Name, Event Time, and the oridinal position in the originating array where it's details are stored.
    /// </summary>
    public class ScheduledEvent : ICloneable
    {
        #region Global Objects
        //***************************************************************************
        // Global Variables
        // 
        private string strName = "";
        private int intId = 0;
        private DateTime dateSched;
        //***************************************************************************
        // Public Fields
        // 
        /// <summary>
        /// Gets or sets the name used to identify this event.
        /// </summary>
        public string EventName
        {
            get { return strName; }
            set { strName = value; }
        }
        /// <summary>
        /// Gets a longer version of the Event's name, formated for on-screen display or log output, and includes the
        /// name of the event followed by the time it's scheduled to fire in brackets.
        /// </summary>
        public string LongName
        {
            get
            {
                string output = intId.ToString() + ": " + strName + "\t(Time till event: ";
                if (this.WaitTime.ToString().Substring(0, 1) == "-")
                    output += this.WaitTime.ToString().Substring(0, 9);
                else
                    output += this.WaitTime.ToString().Substring(0, 8);
                output += ")";
                return output;
            }
        }
        /// <summary>
        /// Gets or sets the ordinal position (index) of the event's details in the original array object.
        /// </summary>
        public int EventID
        {
            get { return intId; }
            set { intId = value; }
        }
        /// <summary>
        /// Gets or sets a DateTime value indicating when this event is intended to fire.
        /// </summary>
        public DateTime ScheduledDate
        {
            get { return this.dateSched; }
            set { this.dateSched = value; }
        }
        /// <summary>
        /// Gets a TimeSpan object set for the amount of time between now, and the next scheduled execution of this event.
        /// </summary>
        public TimeSpan WaitTime
        {
            get { return dateSched.TimeOfDay.Subtract(DateTime.Now.TimeOfDay); }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        /// <summary>
        /// Contains information about an event (who's details are stored in a seperate array) such
        /// as Name, Event Time, and the oridinal position in the originating array where the details are stored.
        /// </summary>
        private ScheduledEvent()
        { }
        /// <summary>
        /// Contains information about an event (who's details are stored in a seperate array) such
        /// as Name, Event Time, and the oridinal position in the originating array where the details are stored.
        /// </summary>
        /// <param name="EventID">The ordinal position (index) within the original array.</param>
        /// <param name="EventName">A string value containing a name used to refer to this query.</param>
        /// <param name="Schedule">A DateTime object containing the time of day at which this event is supposed to fire.</param>
        public ScheduledEvent(int EventID, string EventName, TimeSpan Schedule)
        {
            intId = EventID;
            strName = EventName;
            dateSched = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Schedule.Hours, Schedule.Minutes, Schedule.Seconds);
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        /// <summary>
        /// Creates a shallow clone of the current Event object.
        /// </summary>
        /// <returns>An object of type 'object'.</returns>
        public override object Clone()
        {
            return MemberwiseClone();
        }
        /// <summary>
        /// Determines if the specified event is scheduled to fire before this event instance.
        /// </summary>
        /// <param name="thisEvent">The event to compare against.</param>
        /// <returns>A bool value indicating 'true' if the current instance is further away in time than the specified instance.</returns>
        public bool IsAfter(ScheduledEvent thisEvent)
        {
            return (this.WaitTime.Ticks > thisEvent.WaitTime.Ticks);
        }
        /// <summary>
        /// Determines if the specified event is scheduled to fire after this event instance.
        /// </summary>
        /// <param name="thisEvent">The event to compare against.</param>
        /// <returns>A bool value indicating 'true' if the current instance is sooner in time than the specified instance.</returns>
        public bool IsBefore(ScheduledEvent thisEvent)
        {
            return (this.WaitTime.Ticks > thisEvent.WaitTime.Ticks);
        }
        #endregion

        #region Operator Overloads
        //***************************************************************************
        // Operator Overloads
        // 
        public static bool operator ==(ScheduledEvent Event1, ScheduledEvent Event2)
        {
            return Event1.Equals(Event2);
        }
        public static bool operator !=(ScheduledEvent Event1, ScheduledEvent Event2)
        {
            return !(Event1 == Event2);
        }
        public static bool operator >(ScheduledEvent Event1, ScheduledEvent Event2)
        {
            return Event1.IsAfter(Event2);
        }
        public static bool operator >=(ScheduledEvent Event1, ScheduledEvent Event2)
        {
            return (Event1.IsAfter(Event2) || Event1.dateSched == Event2.dateSched);
        }
        public static bool operator <(ScheduledEvent Event1, ScheduledEvent Event2)
        {
            return Event1.IsBefore(Event2);
        }
        public static bool operator <=(ScheduledEvent Event1, ScheduledEvent Event2)
        {
            return (Event1.IsBefore(Event2) || Event1.dateSched == Event2.dateSched);
        }
        /// <summary>
        /// Determines whether or not the specied ScheduledEvent is equivalent to this instance.
        /// </summary>
        /// <param name="obj">A populated ScheduledEvent object to compare against this instance.</param>
        /// <returns>A bool value indicating equivalence.</returns.
        public override bool Equals(object obj)
        {
            return this == (ScheduledEvent)obj;
        }
        public override int GetHashCode()
        {
            return this.EventName.GetHashCode();
        }
        #endregion
    }
    public class ScheduledEventCollection : RainstormStudios.Collections.ObjectCollectionBase<ScheduledEvent>
    {
        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public string Add(ScheduledEvent val)
        {
            return base.Add(val, string.Empty);
        }
        public void Add(ScheduledEvent val, string key)
        {
            base.Add(val, key);
        }
        public ScheduledEventCollection Clone()
        {
            ScheduledEventCollection newCol = new ScheduledEventCollection();
            for (int i = 0; i < this.Count; i++)
                newCol.Add((ScheduledEvent)this[i].Clone(), this.GetKey(i));
            return newCol;
        }
        public void Sort()
        { this.Sort(SortDirection.Ascending); }
        public void Sort(SortDirection dir)
        {
            base.Sort(dir, new ScheduledEventSorter());
        }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        internal virtual void SwapPos(int idx1, int idx2)
        {
            ScheduledEvent idx1Val = this[idx1];
            ScheduledEvent idx2Val = this[idx2];

            this[idx1] = idx2Val;
            this[idx2] = idx1Val;
        }
        internal virtual void SwapPos(ScheduledEvent val1, ScheduledEvent val2)
        {
            int idx1 = this.IndexOf(val1);
            int idx2 = this.IndexOf(val2);

            this.Move(idx1, idx2);
        }
        #endregion
    }
    class ScheduledEventSorter : IComparer<ScheduledEvent>
    {
    }
}
