using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RainstormStudios.Scheduling
{
    public class ScheduledEventParams:ICloneable
    {
        #region Global Objects
        //***************************************************************************
        // Global Variables
        // 
        protected EventFrequency eFreq;
        protected DateTime sDate = DateTime.Now;
        protected DateTime eDate;
        protected TimeSpan evtTime = DateTime.Now.TimeOfDay;
        protected TimeSpan repInt;
        protected TimeSpan repEndTime;
        protected TimeSpan repEndDur;
        protected MonthlyOccurance monOcc;
        protected int dayMonth = 0;
        protected int evtInt = 0;
        protected string dayWeek = "";
        protected string monYear = "";
        protected AosRollupParams myOwner = null;
        //***************************************************************************
        // Public Fields
        // 
        /// <summary>
        /// Gets or sets a value of the type AllOneSystem.Collections.EventFrequency enumeration which defines how often this event executes.
        /// </summary>
        public EventFrequency EventFreq
        {
            get { return eFreq; }
            set { eFreq = value; }
        }
        /// <summary>
        /// Gets or sets a Date/Time value indicating the very first date and time that this event is allowed to begin populating itself into the schedule.
        /// </summary>
        public DateTime StartDate
        {
            get { return sDate; }
            set { sDate = value; }
        }
        /// <summary>
        /// Gets or sets a Date/Time value indicating the when the event should stop populating itself into the schedule.
        /// </summary>
        public DateTime EndDate
        {
            get { return eDate; }
            set { eDate = value; }
        }
        /// <summary>
        /// Gets or sets the exact time of day at which the event should fire.  If a repeat interval is set, this is the time of day when the repeated events begin.
        /// </summary>
        public TimeSpan TimeOfDay
        {
            get { return evtTime; }
            set { evtTime = value; }
        }
        /// <summary>
        /// Gets or sets the length of time to wait before re-executing this event after each successful execution.
        /// </summary>
        public TimeSpan RepeatInterval
        {
            get { return repInt; }
            set { repInt = value; }
        }
        /// <summary>
        /// An event with a valid RepeatInterval will continuously fire at the specified interval until this specific time of day (or midnight).  Either 'UntilDuration' OR 'UntilTimeOfDay' should be set.  Not both.
        /// </summary>
        public TimeSpan UntilTimeOfDay
        {
            get { return repEndTime; }
            set { repEndTime = value; }
        }
        /// <summary>
        /// An event with a valid RepeatInterval will continuously fire at the specified interval for this length of time (or until midnight).  Either 'UntilDuration' OR 'UntilTimeOfDay' should be set.  Not both.
        /// </summary>
        public TimeSpan UntilDuration
        {
            get { return repEndDur; }
            set { repEndDur = value; }
        }
        /// <summary>
        /// Specifies which occurance of a day in the month, as defined by the AllOneSystem.Collections.MonthlyOccurance enumeration, this event should fire on.
        /// </summary>
        public MonthlyOccurance MonOccurance
        {
            get { return monOcc; }
            set { monOcc = value; }
        }
        /// <summary>
        /// Gets or sets an integer value indicating the date on which the event should fire.
        /// </summary>
        public int DayOfMonth
        {
            get { return dayMonth; }
            set { dayMonth = value; }
        }
        /// <summary>
        /// Gets or sets an integer value indicating how often the event should execute with it's scope.  For daily events, 1 = every day, 2 = every other day, etc.
        /// </summary>
        public int EventInterval
        {
            get { return evtInt; }
            set { evtInt = value; }
        }
        /// <summary>
        /// Gets or sets a string value defining which days of the week this event should fire on.  The format is a series of string values, seperated by comas.  One value for each day the event is scheduled on, with the name spelled in full.
        /// </summary>
        public string DaysOfWeek
        {
            get { return dayWeek; }
            set { dayWeek = value; }
        }
        /// <summary>
        /// Gets or sets a string value containing a comma-separated list of indicating which months out of the year this schedule should occur in.
        /// </summary>
        public string MonthsOfYear
        {
            get { return monYear; }
            set { monYear = value; }
        }
        /// <summary>
        /// Gets a sets an AosRollupParams object indicating the object which owns this instance.
        /// </summary>
        public AosRollupParams ParentParams
        {
            get { return myOwner; }
            set { myOwner = value; }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public ScheduledEventParams()
        { }
        public ScheduledEventParams(AosRollupParams owner)
            : this()
        {
            this.myOwner = owner;
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        /// <summary>
        /// Creates a 'shallow' copy of this instance.
        /// </summary>
        /// <returns>An object containing all the same settings as this instance.</returns>
        public object Clone()
        {
            return this.MemberwiseClone();
        }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        #endregion

        #region Override Methods
        //***************************************************************************
        // Override Methods
        // 
        /// <summary>
        /// Converts this instance into a string value representing the stored schedule.
        /// </summary>
        /// <returns>A string value.</returns>
        public override string ToString()
        {
            string retVal = "";

            if (myOwner != null)
                retVal += myOwner.RollupName + ": ";

            switch (this.eFreq)
            {
                case EventFrequency.daily:
                    if (this.evtInt == 1)
                        retVal += "Every day";
                    else
                        retVal += "Every " + this.evtInt.ToString() + " days";
                    break;
                case EventFrequency.monthly:
                    retVal += "The ";
                    if (this.dayMonth > 0)
                        retVal += "day " + this.dayMonth.ToString();
                    else
                        retVal += this.MonOccurance + " " + this.dayWeek;
                    if (this.monYear.Length > 0)
                    {
                        retVal += " in the month(s) of ";
                        string[] months = this.monYear.Split(',');
                        for (int i = 0; i < months.Length; i++)
                        {
                            if (i == (months.Length - 1))
                                retVal += "and ";
                            retVal += months[i].Trim().Substring(0, 1).ToUpper() + months[i].Trim().Substring(1).ToLower();
                            if (i < (months.Length - 1))
                                retVal += ", ";
                        }
                    }
                    else
                        retVal += " in no months";
                    break;
                case EventFrequency.weekly:
                    if (this.evtInt == 1)
                        retVal += "Every week on ";
                    else
                        retVal += "Every " + this.evtInt.ToString() + " weeks on ";
                    if (this.dayWeek.Length > 0)
                    {
                        string[] days = this.dayWeek.Split(',');
                        for (int i = 0; i < days.Length; i++)
                        {
                            if ((i == (days.Length) - 1) && (days.Length > 1))
                                retVal += "and ";
                            retVal += days[i].Trim().Substring(0, 1).ToUpper() + days[i].Trim().Substring(1).ToLower();
                            if (i < (days.Length - 1))
                                retVal += ", ";
                        }
                    }
                    else
                        retVal += "no days";
                    break;
                case EventFrequency.once:
                    retVal += "On " + this.StartDate.ToString("dddd, MMMM dd, yyyy");
                    break;
            }

            retVal += " at " + AosDateTime.GetDateTimeFromTimeSpan(this.evtTime).ToString("hh:mm:ss tt");

            if (this.repInt.TotalMinutes > 0)
            {
                retVal += " and repeating every ";
                if (this.repInt.Hours > 0)
                    retVal += this.repInt.Hours.ToString() + " hours";
                else if (this.repInt.Minutes > 0)
                    retVal += this.repInt.Minutes.ToString() + " minutes";
                retVal += " until ";
                if (this.repEndDur.TotalMinutes > 0)
                    if (this.repEndDur.Hours > 0)
                        retVal += this.repEndDur.Hours.ToString() + " hours has elapsed";
                    else
                        retVal += this.repEndDur.Minutes.ToString() + " minutes has elapsed";
                else
                    retVal += AosDateTime.GetDateTimeFromTimeSpan(this.repEndTime).ToString("hh:mm:ss tt");
            }

            if (this.eFreq != EventFrequency.once)
            {
                retVal += " starting on " + this.sDate.ToString("dddd, MMMM dd, yyyy");
                if (this.eDate.Ticks > this.sDate.Ticks)
                    retVal += " and ending on " + this.eDate.ToString("dddd, MMMM dd, yyyy");
            }

            return retVal;
        }
        #endregion    
    }
    public class ScheduledEventParamsCollection : RainstormStudios.Collections.ObjectCollectionBase<ScheduledEventParams>
    {
        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        /// <summary>
        /// Creates a 'deep' copy of this collection instance.
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            AosSchedParamsCollection newSchedList = new AosSchedParamsCollection();
            foreach (AosSchedParams srcParam in List)
                newSchedList.Add(srcParam.Clone() as AosSchedParams);
            return newSchedList;
        }
        #endregion
    }
}
