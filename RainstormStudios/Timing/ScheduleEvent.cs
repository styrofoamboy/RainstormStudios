using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RainstormStudios.Timing
{
    /// <summary>
    /// A class that contains information about an event (who's details are stored in a seperate array) such
    /// as Name, Event Time, and the oridinal position in the originating array where it's details are stored.
    /// </summary>
    [Author("Michael Unfried")]
    public class ScheduleEvent : ICloneable
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        string
            _evtName;
        int?
            _intID;
        DateTime
            _dtSched;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        /// <summary>
        /// Gets or sets a <see cref="T:System.String"/> value containing the display name for this event.
        /// </summary>
        public string EventName
        {
            get { return this._evtName; }
            set { this._evtName = value; }
        }
        /// <summary>
        /// Gets a longer version of the Event's name, formated for on-screen display or log output, and includes the
        /// name of the event followed by the time it's scheduled to occur in brackets.
        /// </summary>
        public string LongEventDesc
        {
            get
            {
                string output = (this._intID.HasValue ? this._intID.ToString() : "0") + ": " + this.EventName + "\t(Time till event: ";
                if (this.WaitTime.ToString().Substring(0, 1) == "-")
                    output += this.WaitTime.ToString().Substring(0, 9);
                else
                    output += this.WaitTime.ToString().Substring(0, 8);
                output += ")";
                return output;
            }
        }
        /// <summary>
        /// Gets a <see cref="T:System.Int32"/> value indicating the ordinal position of the event in its parent collection.
        /// </summary>
        public int? EventID
        {
            get { return this._intID; }
            internal set { this._intID = value; }
        }
        /// <summary>
        /// Gets or sets a DateTime value indicating at what time of day this event is intended to occur.
        /// </summary>
        public TimeSpan ScheduledTime
        {
            get { return this._dtSched.TimeOfDay; }
            set { this._dtSched = rsDateTime.GetDateTimeFromTimeSpan(value); }
        }
        /// <summary>
        /// Gets a <see cref="T:System.TimeSpan"/> object indicating the amount of time between now and the next scheduled execution of this event.
        /// </summary>
        public TimeSpan WaitTime
        {
            get { return this._dtSched.TimeOfDay.Subtract(DateTime.Now.TimeOfDay); }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        private ScheduleEvent()
        { }
        /// <summary>
        /// Contains information about an event (who's details are stored in a seperate array) such
        /// as Name, Event Time, and the oridinal position in the originating array where the details are stored.
        /// </summary>
        /// <param name="eventID">The ordinal position (index) within the original array.</param>
        /// <param name="eventName">A string value containing a name used to refer to this query.</param>
        /// <param name="schedule">A DateTime object containing the time of day at which this event is supposed to fire.</param>
        public ScheduleEvent(int eventID, string eventName, TimeSpan schedule)
            : this()
        {
            this._intID = eventID;
            this._evtName = EventName;
            this._dtSched = rsDateTime.GetDateTimeFromTimeSpan(schedule);
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
        public object Clone()
        {
            return MemberwiseClone();
        }
        #endregion

        #region Operator Overloads
        //***************************************************************************
        // Operator Overloads
        // 
        public static bool operator ==(ScheduleEvent val1, ScheduleEvent val2)
        { return val1.WaitTime.Ticks == val2.WaitTime.Ticks; }
        public static bool operator !=(ScheduleEvent val1, ScheduleEvent val2)
        { return !(val1 == val2); }
        public static bool operator >(ScheduleEvent val1, ScheduleEvent val2)
        { return val1.WaitTime.Ticks > val2.WaitTime.Ticks; }
        public static bool operator >=(ScheduleEvent val1, ScheduleEvent val2)
        { return val1.WaitTime.Ticks >= val2.WaitTime.Ticks; }
        public static bool operator <(ScheduleEvent val1, ScheduleEvent val2)
        { return val1.WaitTime.Ticks < val2.WaitTime.Ticks; }
        public static bool operator <=(ScheduleEvent val1, ScheduleEvent val2)
        { return val1.WaitTime.Ticks <= val2.WaitTime.Ticks; }
        /// <summary>
        /// Determines whether or not the specied AosEvent is equivalent to this instance.
        /// </summary>
        /// <param name="obj">A popuiated AosEvent object to comare against this instance.</param>
        /// <returns>A bool value indicating equialence.</returns>
        public override bool Equals(object obj)
        {
            ScheduleEvent evt2 = (obj as ScheduleEvent);
            if (evt2 == null)
                return false;

            else if (!this._dtSched.Equals(evt2._dtSched))
                return false;

            else if (!this._intID.Equals(evt2._intID))
                return false;

            else if (!this._evtName.Equals(evt2._evtName))
                return false;

            else
                return true;
        }
        public override int GetHashCode()
        {
            return this.LongEventDesc.GetHashCode();
        }
        #endregion
    }
    /// <summary>
    /// A custom class used to store a list of Events.
    /// </summary>
    [Author("Michael Unfried")]
    public class ScheduleEventCollection : Collections.ObjectCollectionBase<ScheduleEvent>, ICloneable
    {
        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public new void Add(ScheduleEvent val)
        { base.Add(val, (val.EventID != null ? val.EventID.Value.ToString() : string.Empty)); }
        public new void Sort(Collections.SortDirection dir)
        { base.Sort(dir, new ScheduleEventComparer()); }
        /// <summary>
        /// Creates a shallow clone of the current Event object.
        /// </summary>
        /// <returns>An object of type 'object'.</returns>
        public object Clone()
        {
            return MemberwiseClone();
        }
        #endregion
    }
    [Author("Michael Unfried")]
    public sealed class ScheduleEventComparer : System.Collections.IComparer
    {
        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public int Compare(object x, object y)
        {
            ScheduleEvent val1 = (x as ScheduleEvent);
            ScheduleEvent val2 = (y as ScheduleEvent);

            if (val1 == null)
            {
                if (val2 == null)
                    // If val1 is null and val2 is null, they're equal.
                    return 0;

                else
                    // If val1 is null and val2 is not null, val2 is greater.
                    return -1;
            }
            else
            {
                // If val1 is not null...
                if (val2 == null)
                    // If val2 is null, val1 is greater.
                    return 1;

                else
                    // If neither value is null, compare their wait times.
                    if (val1.WaitTime.Ticks > val2.WaitTime.Ticks)
                        // val1 is greater.
                        return 1;
                    else if (val1.WaitTime.Ticks < val2.WaitTime.Ticks)
                        // val2 is greater.
                        return -1;
                    else
                        // They are equal.
                        return 0;
            }
        }
        #endregion
    }
}
