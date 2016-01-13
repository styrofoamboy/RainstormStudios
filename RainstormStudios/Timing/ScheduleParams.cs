using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RainstormStudios.Timing
{
    [Author("Michael Unfried")]
    public class ScheduleParams : ICloneable
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        protected EventOccurance
            _eFreq;
        protected EventOccuranceType
            _eFreqType;
        protected MonthOfYear
            _monOcc;
        protected WeeklyOccurance
            _dayOcc;
        protected DateTime
            _startDate,
            _endDate;
        protected TimeSpan
            _evtTime = TimeSpan.Zero,
            _repInt = TimeSpan.Zero,
            _repEndTime = TimeSpan.Zero,
            _repEndDur = TimeSpan.Zero;
        protected int[]
            _dayMonth;
        protected int
            _evtInt;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        /// <summary>
        /// Gets or sets a <see cref="T:RainstormStudios.Timing.EventOccurance"/> enum value indicating how often the event should reoccur after its first scheduled start date.
        /// </summary>
        public EventOccurance EventReoccurance
        {
            get { return this._eFreq; }
            set { this._eFreq = value; }
        }
        /// <summary>
        /// Gets or sets a <see cref="T:RainstormStudios.Timing.EventOccuranceType"/> enum value indicating specific occurances of calendar dates or clock times on which the event should occur.
        /// </summary>
        public EventOccuranceType ReoccuranceType
        {
            get { return this._eFreqType; }
            set { this._eFreqType = value; }
        }
        /// <summary>
        /// Gets or set a <see cref="T:RainstormStudios.Timing.MonthOfYear"/> enum value indicating in which months the event should occur.
        /// </summary>
        public MonthOfYear MonthsOfYear
        {
            get { return this._monOcc; }
            set { this._monOcc = value; }
        }
        /// <summary>
        /// Gets or sets a <see cref="T:RainstormStudios.Timeing.DailyOccurance"/> enum value indicating on which days of the week the event should occur.
        /// </summary>
        public WeeklyOccurance DaysOfWeek
        {
            get { return this._dayOcc; }
            set { this._dayOcc = value; }
        }
        /// <summary>
        /// Gets or sets a <see cref="T:System.Int[]"/> array containing the dates of a month on which the event should occur.
        /// </summary>
        public int[] EventDates
        {
            get { return this._dayMonth; }
            set { this._dayMonth = value; }
        }
        /// <summary>
        /// Gets or set a <see cref="T:System.Timespan"/> value indicating the time of day at which the event should fire.  If the event is setup for reoccurance accross days, it will fire at this time each day.
        /// </summary>
        public TimeSpan TimeOfDay
        {
            get { return this._evtTime; }
            set { this._evtTime = value; }
        }
        /// <summary>
        /// Gets or sets a <see cref="T:System.Integer"/> value indicating how often the event should execute with it's scope.  For daily events, 1 = every day, 2 = every other day, etc.
        /// </summary>
        public int EventInterval
        {
            get { return this._evtInt; }
            set { this._evtInt = value; }
        }
        /// <summary>
        /// Gets or sets a <see cref="T:System.DateTime"/> value indicating when this event should begin scheduling.
        /// </summary>
        public DateTime StartDate
        {
            get { return this._startDate; }
            set
            {
                this._startDate = value;
                if (this._evtTime == TimeSpan.Zero)
                    this._evtTime = value.TimeOfDay;
            }
        }
        /// <summary>
        /// Gets or sets a <see cref="T:System.DateTime"/> value indicating when this event should stop scheduling.
        /// </summary>
        public DateTime EndDate
        {
            get { return this._endDate; }
            set { this._endDate = value; }
        }
        /// <summary>
        /// Gets or set a <see cref="T:System.Timespan"/> value indicating the length of time to wait before re-executing this event after each successful execution.
        /// </summary>
        public TimeSpan RepeatInterval
        {
            get { return this._repInt; }
            set { this._repInt = value; }
        }
        /// <summary>
        /// Gets or sets a <see cref="T:System.Timespan"/> value indicating the length of time until which an event with a valid RepeatInterval will continue to re-execute.
        /// </summary>
        /// <remarks>Setting this value will automatically clear any value assigned to <paramref name="RepeatUntilTimeOfDay"/>, since both cannot be set.</remarks>
        public TimeSpan RepeatUntilDuration
        {
            get { return this._repEndDur; }
            set
            {
                this.RepeatUntilTimeOfDay = TimeSpan.Zero;
                this._repEndDur = value;
            }
        }
        /// <summary>
        /// Gets or sets a <see cref="T:System.Timespan"/> value indicating the time of day until which an event with a valid RepeatInternal will continue to re-execute.
        /// </summary>
        /// <remarks>Setting this value will automatically clear any value assigned to <paramref name="RepeatUntilDuration"/>, since both cannot be set.</remarks>
        public TimeSpan RepeatUntilTimeOfDay
        {
            get { return this._repEndTime; }
            set
            {
                this.RepeatUntilDuration = TimeSpan.Zero;
                this._repEndTime = value;
            }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public ScheduleParams()
            : this(DateTime.Now)
        { }
        public ScheduleParams(DateTime eventStart)
            :this(eventStart.TimeOfDay)
        {
            this._startDate = eventStart;
        }
        public ScheduleParams(TimeSpan eventStart)
        {
            this._startDate = DateTime.Now;
            this._evtTime = eventStart;
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
            StringBuilder sb = new StringBuilder();

            switch (this.EventReoccurance)
            {
                case EventOccurance.Daily:
                    if (this.EventInterval == 1)
                        sb.Append("Every day");
                    else
                        sb.AppendFormat("Every {0} days", this.EventInterval);
                    break;
                case EventOccurance.Monthly:
                    sb.Append("The ");
                    if (this.EventDates != null && this.EventDates.Length > 0)
                        sb.Append(string.Join(", ", this.EventDates.Where(i => i > 0 && i <= 31).Select(i => i.ToString() + DateTimeExtensions.GetNumericSuffix(i)).ToArray()) + " days");
                    else
                        sb.AppendFormat("{0} {1}", this.ReoccuranceType, this.DaysOfWeek);
                    if (this.MonthsOfYear > 0)
                    {
                        sb.Append(" in the month(s) of ");
                        MonthOfYear[] moyVals = ((IEnumerable<MonthOfYear>)Enum.GetValues(typeof(MonthOfYear))).ToArray();
                        StringBuilder sbMonVals = new StringBuilder();
                        for (int i = 0; i < moyVals.Length; i++)
                            if (this.MonthsOfYear.HasFlag(moyVals[i]))
                                sbMonVals.AppendFormat(", {0}", moyVals[i]);

                        string monStr = sbMonVals.ToString().TrimStart(',', ' ');
                        int iLastComma = monStr.LastIndexOf(',');
                        if (iLastComma > 0)
                            monStr = monStr.Substring(0, iLastComma - 1) + " and" + monStr.Substring(iLastComma + 1);
                        sb.Append(monStr);
                    }
                    else
                        sb.Append(" in no months");
                    break;
                case EventOccurance.Weekly:
                    if (this.EventInterval == 1)
                        sb.Append("Every week on ");
                    else
                        sb.AppendFormat("Every {0} weeks on ", this.EventInterval);
                    if (this.DaysOfWeek > 0)
                    {
                        WeeklyOccurance[] doyVals = ((IEnumerable<WeeklyOccurance>)Enum.GetValues(typeof(WeeklyOccurance))).ToArray();
                        StringBuilder sbDoyVals = new StringBuilder();
                        for (int i = 0; i < doyVals.Length; i++)
                            if (this.DaysOfWeek.HasFlag(doyVals[i]))
                                sbDoyVals.AppendFormat(", {0}", doyVals[i]);

                        string dayStr = sbDoyVals.ToString().TrimStart(',', ' ');
                        int iLastComma = dayStr.LastIndexOf(',');
                        if (iLastComma > 0)
                            dayStr = dayStr.Substring(0, iLastComma - 1) + " and" + dayStr.Substring(iLastComma + 1);
                        sb.Append(dayStr);
                    }
                    else
                        sb.Append("no days");
                    break;
                case EventOccurance.Once:
                    sb.AppendFormat("On {0} at {1}", this.StartDate.ToString("dddd, MMMM dd, yyyy"), this.TimeOfDay.ToString("hh:mm:ss tt"));
                    break;
            }

            sb.AppendFormat(" at {0}", rsDateTime.GetDateTimeFromTimeSpan(this.TimeOfDay).ToString("hh:mm:ss tt"));

            if (this.RepeatInterval.TotalMinutes > 0)
            {
                sb.Append(" and repeating every ");
                if (this.RepeatInterval.Hours > 0)
                    sb.AppendFormat("{0} hours", this.RepeatInterval.Hours.ToString());
                else if (this.RepeatInterval.Minutes > 0)
                    sb.AppendFormat("{0} minutes", this.RepeatInterval.Minutes.ToString());
                sb.Append(" until ");
                if (this.RepeatUntilDuration.TotalMinutes > 0)
                    if (this.RepeatUntilDuration.Hours > 0)
                        sb.AppendFormat("{0} hours has elapsed", this.RepeatUntilDuration.Hours.ToString());
                    else
                        sb.AppendFormat("{0} minutes has elapsed", this.RepeatUntilDuration.Minutes.ToString());
                else if (this.RepeatUntilTimeOfDay.TotalMinutes > 0)
                    sb.Append(rsDateTime.GetDateTimeFromTimeSpan(this.RepeatUntilTimeOfDay).ToString("hh:mm:ss tt"));
                else
                    sb.Append("forever");
            }

            if (this.EventReoccurance != EventOccurance.Once)
            {
                sb.AppendFormat(" starting on {0}", this.StartDate.ToString("dddd, MMMM dd, yyyy"));
                if (this.EndDate.Ticks > this.StartDate.Ticks)
                    sb.AppendFormat(" and ending on {0}", this.EndDate.ToString("dddd, MMMM dd, yyyy"));
            }

            return sb.ToString();
        }
        #endregion
    }
    [Author("Michael Unfried")]
    public class ScheduleParamsCollection : Collections.ObjectCollectionBase<ScheduleParams>
    {
        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public object Clone()
        {
            ScheduleParamsCollection newList = new ScheduleParamsCollection();
            for (int i = 0; i < this.Count; i++)
                newList.Add(this[i].Clone() as ScheduleParams, string.Empty);
            return newList;
        }
        public new void Add(ScheduleParams val)
        { base.Add(val, string.Empty); }
        #endregion
    }
}
