//  Copyright (c) 2008, Michael unfried
//  Email:  serbius3@gmail.com
//  All rights reserved.

//  Redistribution and use in source and binary forms, with or without modification, 
//  are permitted provided that the following conditions are met:

//  Redistributions of source code must retain the above copyright notice, 
//  this list of conditions and the following disclaimer. 
//  Redistributions in binary form must reproduce the above copyright notice, 
//  this list of conditions and the following disclaimer in the documentation 
//  and/or other materials provided with the distribution. 

//  THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
//  KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
//  IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
//  PURPOSE. IT CAN BE DISTRIBUTED FREE OF CHARGE AS LONG AS THIS HEADER 
//  REMAINS UNCHANGED.
using System;
using System.Data;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using RainstormStudios.Collections;

namespace RainstormStudios.Web.UI.WebControls
{
    [DataObject(true)]
    public sealed class EventCalendarOld : System.Web.UI.WebControls.Calendar, System.Web.UI.INamingContainer, IPostBackEventHandler, IBindableTemplate
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private CalendarEventCollectionOld
            _events;
        private object
            _dataSrc;
        private string
            _dataSrcID,
            _dataMbr,
            _dataMbrName,
            _dataMbrStDt,
            _dataMbrEndDt,
            _dataMbrImgUrl;
        //***************************************************************************
        // Public Events
        // 
        public new event DayRenderEventHandler DayRender;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public CalendarEventCollectionOld CalendarEvents
        {
            get { return this._events; }
        }
        public string DataSourceID
        {
            get { return this._dataSrcID; }
            set { this._dataSrcID = value; }
        }
        public object DataSource
        {
            get { return this._dataSrc; }
            set { this._dataSrc = value; }
        }
        public string DataMember
        {
            get { return this._dataMbr; }
            set { this._dataMbr = value; }
        }
        public string DataNameField
        {
            get { return this._dataMbrName; }
            set { this._dataMbrName = value; }
        }
        public string DataStartDateField
        {
            get { return this._dataMbrStDt; }
            set { this._dataMbrStDt = value; }
        }
        public string DataEndDateField
        {
            get { return this._dataMbrEndDt; }
            set { this._dataMbrEndDt = value; }
        }
        public string DataImageUrlField
        {
            get { return this._dataMbrImgUrl; }
            set { this._dataMbrImgUrl = value; }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public EventCalendarOld()
        {
            this.VisibleDate = DateTime.Today;
            this._events = new CalendarEventCollectionOld(this);
            this.InitEventDefinitions();
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public override void DataBind()
        {
            base.DataBind();
        }
        public void InitEventDefinitions()
        {
            // This should load any events 'manually' configured in the ASPX page.
        }
        public void InstantiateIn(System.Web.UI.Control ctrl)
        {
        }
        public System.Collections.Specialized.IOrderedDictionary ExtractValues(System.Web.UI.Control ctrl)
        {
            System.Collections.Specialized.OrderedDictionary
                nvCol = new System.Collections.Specialized.OrderedDictionary();

            return nvCol;
        }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        protected override void Render(HtmlTextWriter writer)
        {
            // First thing we need to do is some quick calc's about the current day/month.

            TimeSpan tsOneDay = new TimeSpan(24, 0, 0);
            // The first day of the displayed month.
            DateTime dt = new DateTime(this.VisibleDate.Year, this.VisibleDate.Month, 1);
            int iMonthLen = System.Globalization.CultureInfo.CurrentCulture.Calendar.GetDaysInMonth(dt.Year, dt.Month);
            // The last day of this month.
            DateTime dtEm = new DateTime((dt.Month < 12) ? dt.Year : dt.Year + 1, (dt.Month < 12) ? dt.Month + 1 : 1, 1).Subtract(tsOneDay);
            // The last day of last month.
            DateTime dtLm = dt.Subtract(tsOneDay);
            // The current day we're rendering.
            DateTime dtCur = dt.Subtract(tsOneDay);
            // We use this to calc the values for the Prev/Next buttons.
            DateTime dtRoot = new DateTime(1999, 12, 31);

            // Back up until we hit the first day of the week.
            while (((int)dtCur.DayOfWeek) != ((this.FirstDayOfWeek == FirstDayOfWeek.Default)
                                            ? (int)System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek
                                            : (int)this.FirstDayOfWeek))
                dtCur = dtCur.Subtract(tsOneDay);

            // Next, we're going to init any DataBindings and grab the DataTable we'll work with.
            DataTable dtSrc = null;
            if (this._dataSrc != null)
            {
                string dsType = this._dataSrc.GetType().ToString();
                if (dsType.EndsWith("DataSet"))
                {
                    if (((DataSet)this._dataSrc).Tables.Contains(this._dataMbr))
                        dtSrc = ((DataSet)this._dataSrc).Tables[this._dataMbr];
                }
                else if (dsType.EndsWith("DataTable"))
                {
                    dtSrc = (DataTable)this._dataSrc;
                }

                // Make sure the DataSource contains all the fields that it's supposed to contain.
                if (dtSrc != null)
                {
                    if (!string.IsNullOrEmpty(this._dataMbrName) && !dtSrc.Columns.Contains(this._dataMbrName))
                        dtSrc = null;
                    else if (!string.IsNullOrEmpty(this._dataMbrStDt) && !dtSrc.Columns.Contains(this._dataMbrStDt))
                        dtSrc = null;
                    else if (!string.IsNullOrEmpty(this._dataMbrEndDt) && !dtSrc.Columns.Contains(this._dataMbrEndDt))
                        dtSrc = null;
                    else if (!string.IsNullOrEmpty(this._dataMbrImgUrl) && !dtSrc.Columns.Contains(this._dataMbrImgUrl))
                        dtSrc = null;
                }
            }

            // Now we're ready to start rendering the control.
            writer.BeginRender();
            writer.AddAttribute(HtmlTextWriterAttribute.Cellspacing, this.CellSpacing.ToString());
            writer.AddAttribute(HtmlTextWriterAttribute.Cellpadding, this.CellPadding.ToString());
            writer.AddAttribute(HtmlTextWriterAttribute.Border, "0");
            writer.AddAttribute(HtmlTextWriterAttribute.Title, "EventCalendar");
            this.AddAttributesToRender(writer);
            writer.AddStyleAttribute(HtmlTextWriterStyle.BorderWidth, (this.BorderWidth == Unit.Empty) ? "1px" : this.BorderWidth.ToString());
            writer.RenderBeginTag(HtmlTextWriterTag.Table);
            {
                writer.RenderBeginTag(HtmlTextWriterTag.Tr);
                {
                    writer.AddAttribute(HtmlTextWriterAttribute.Colspan, "7");
                    if (this.TitleStyle.BorderStyle != BorderStyle.NotSet)
                        writer.AddStyleAttribute(HtmlTextWriterStyle.BorderStyle, this.TitleStyle.BorderStyle.ToString());
                    if (this.TitleStyle.BackColor != Color.Empty)
                        writer.AddStyleAttribute(HtmlTextWriterStyle.BackgroundColor, RainstormStudios.Hex.GetWebColor(this.TitleStyle.BackColor));
                    if (this.TitleStyle.Font != null && this.TitleStyle.Font.Size != FontUnit.Empty)
                        writer.AddStyleAttribute(HtmlTextWriterStyle.Height, this.TitleStyle.Font.Size.ToString());
                    this.TitleStyle.AddAttributesToRender(writer);
                    writer.RenderBeginTag(HtmlTextWriterTag.Td);
                    {
                        writer.AddAttribute(HtmlTextWriterAttribute.Cellspacing, "0");
                        writer.AddAttribute(HtmlTextWriterAttribute.Border, "0");
                        writer.AddStyleAttribute(HtmlTextWriterStyle.Width, "100%");
                        writer.AddStyleAttribute(HtmlTextWriterStyle.BorderCollapse, "collapse");
                        writer.RenderBeginTag(HtmlTextWriterTag.Table);
                        {
                            writer.RenderBeginTag(HtmlTextWriterTag.Tr);
                            {
                                writer.AddAttribute(HtmlTextWriterAttribute.Valign, "bottom");
                                writer.AddStyleAttribute(HtmlTextWriterStyle.Width, "15%");
                                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                                {
                                    DateTime dtLm1 = new DateTime(dtLm.Year, dtLm.Month, 1);
                                    TimeSpan tsDiff = dtLm1.Subtract(dtRoot);
                                    writer.AddAttribute(HtmlTextWriterAttribute.Href, this.Page.ClientScript.GetPostBackClientHyperlink(this, "V" + Convert.ToString(tsDiff.TotalDays - 1)));
                                    writer.AddAttribute(HtmlTextWriterAttribute.Title, "Go to the previous month");
                                    this.NextPrevStyle.AddAttributesToRender(writer);
                                    writer.RenderBeginTag(HtmlTextWriterTag.A);
                                    {
                                        string outText = dtLm.ToString("MMMM");
                                        if (this.NextPrevFormat == NextPrevFormat.CustomText)
                                            outText = this.PrevMonthText;
                                        else if (this.NextPrevFormat == NextPrevFormat.ShortMonth)
                                            outText = dtLm.ToString("MMM");
                                        writer.Write(outText);
                                    }
                                    writer.RenderEndTag();
                                }
                                writer.RenderEndTag();
                                writer.AddAttribute(HtmlTextWriterAttribute.Align, "center");
                                writer.AddStyleAttribute(HtmlTextWriterStyle.Width, "70%");
                                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                                {
                                    writer.Write(dt.ToString((this.TitleFormat == TitleFormat.MonthYear)
                                                                ? "MMMM yyyy" : "MMMM"));
                                }
                                writer.RenderEndTag();
                                writer.AddAttribute(HtmlTextWriterAttribute.Align, "Right");
                                writer.AddAttribute(HtmlTextWriterAttribute.Valign, "bottom");
                                writer.AddAttribute(HtmlTextWriterAttribute.Title, "Go to the next month");
                                writer.AddStyleAttribute(HtmlTextWriterStyle.Width, "15%");
                                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                                {
                                    DateTime dtNm1 = dtEm.AddDays(1);
                                    TimeSpan tsDiff = dtNm1.Subtract(dtRoot);
                                    writer.AddAttribute(HtmlTextWriterAttribute.Href, this.Page.ClientScript.GetPostBackClientHyperlink(this, "V" + Convert.ToString(tsDiff.TotalDays - 1)));
                                    writer.AddAttribute(HtmlTextWriterAttribute.Title, "Go to the next month");
                                    this.NextPrevStyle.AddAttributesToRender(writer);
                                    writer.RenderBeginTag(HtmlTextWriterTag.A);
                                    {
                                        string outText = dtEm.AddDays(1).ToString("MMMM");
                                        if (this.NextPrevFormat == NextPrevFormat.CustomText)
                                            outText = this.NextMonthText;
                                        else if (this.NextPrevFormat == NextPrevFormat.ShortMonth)
                                            outText = dtEm.AddDays(1).ToString("MMM");
                                        writer.Write(outText);
                                    }
                                    writer.RenderEndTag();
                                }
                                writer.RenderEndTag();
                            }
                            writer.RenderEndTag();
                        }
                        writer.RenderEndTag();
                    }
                    writer.RenderEndTag();
                }
                writer.RenderEndTag();

                // Render the DayHeader
                writer.RenderBeginTag(HtmlTextWriterTag.Tr);
                {
                    int stDay = (this.FirstDayOfWeek == FirstDayOfWeek.Default)
                                ? (int)System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek
                                : (int)this.FirstDayOfWeek;
                    for (int i = 0; i < 7; i++)
                    {
                        DayOfWeek dow = (DayOfWeek)((int)System.Math.Abs(stDay - i));
                        writer.AddAttribute(HtmlTextWriterAttribute.Align, "center");
                        writer.AddAttribute(HtmlTextWriterAttribute.Abbr, dow.ToString());
                        writer.AddAttribute(HtmlTextWriterAttribute.Scope, "col");
                        this.DayHeaderStyle.AddAttributesToRender(writer);
                        writer.RenderBeginTag(HtmlTextWriterTag.Th);
                        {
                            writer.Write(dow.ToString().Substring(0, 3));
                        }
                        writer.RenderEndTag();
                    }
                }
                writer.RenderEndTag();

                // Now we're ready to render the actual calender fields.
                // As long as the current day we're rendering is within the display month,
                //   then start a new line and render the next week.
                //while ((dtCur.Month <= dt.Month && dtCur.Year == dt.Year) || (dtCur.Month == 12 && dt.Month == 1 && dtCur.Year == dt.Year - 1))
                //{
                for (int w = 0; w < 6; w++)
                {
                    writer.RenderBeginTag(HtmlTextWriterTag.Tr);
                    {
                        // We know we're going to render a whole week, so we don't
                        //   need to do any checks durring the 'week' loop.
                        for (int i = 0; i < 7; i++)
                        {
                            TableCell cell = new TableCell();
                            cell.ApplyStyle(this.DayStyle);
                            if (dtCur.Month != dt.Month)
                                cell.ApplyStyle(this.OtherMonthDayStyle);
                            if (dtCur.DayOfYear == DateTime.Now.DayOfYear && dtCur.Year == DateTime.Now.Year)
                                cell.ApplyStyle(this.TodayDayStyle);
                            if (this.SelectedDates.Contains(dtCur))
                                cell.ApplyStyle(this.SelectedDayStyle);
                            cell.Style.Remove(HtmlTextWriterStyle.Width);
                            cell.Style.Add(HtmlTextWriterStyle.Width, (this.DayStyle.Width == Unit.Empty) ? "14%" : this.DayStyle.Width.ToString());
                            TimeSpan dtDiff = dtCur.Subtract(dtRoot);
                            string lnkStyle = string.Empty;
                            if (cell.ForeColor != Color.Empty)
                                lnkStyle = "color:" + RainstormStudios.Hex.GetWebColor(cell.ForeColor);
                            else if (!string.IsNullOrEmpty(cell.Style[HtmlTextWriterStyle.Color]))
                                lnkStyle = "color:" + cell.Style[HtmlTextWriterStyle.Color];
                            else if (this.ForeColor != Color.Empty)
                                lnkStyle = "color:" + RainstormStudios.Hex.GetWebColor(this.ForeColor);
                            LiteralControl lnkDate = new LiteralControl();
                            lnkDate.Text = string.Format("<a href=\"{0}\"{1} title=\"Select Date\">{2}</a>",
                                        this.Page.ClientScript.GetPostBackClientHyperlink(this, Convert.ToString(dtDiff.TotalDays - 1)),
                                        (!string.IsNullOrEmpty(lnkStyle)) ? " style=\"" + lnkStyle + "\"" : "",
                                        dtCur.Day.ToString());
                            cell.Controls.Add(lnkDate);

                            CalendarDay cDay = new CalendarDay(dtCur,
                                    (dtCur.DayOfWeek == DayOfWeek.Saturday || dtCur.DayOfWeek == DayOfWeek.Sunday),
                                    (dtCur.DayOfYear == DateTime.Now.DayOfYear && dtCur.Year == DateTime.Now.Year),
                                    this.SelectedDates.Contains(dtCur),
                                    dtCur.Month != dt.Month,
                                    dtCur.Day.ToString());

                            if (dtSrc != null)
                            {
                                DateTime dtEvnt = DateTime.MinValue;
                            }

                            DayRenderEventArgs args = new DayRenderEventArgs(cell, cDay);
                            this.RaiseDayRender(args);

                            cell.RenderControl(writer);

                            //writer.RenderBeginTag(HtmlTextWriterTag.Td);
                            //{

                            //    writer.Write(dtCur.Day);
                            //}
                            //writer.RenderEndTag();
                            // Add a day to the current calender position each iteration.
                            dtCur = dtCur.AddDays(1);
                        }
                    }
                    writer.RenderEndTag();
                }
            }
            writer.RenderEndTag();
            writer.EndRender();
        }
        private void RaiseDayRender(DayRenderEventArgs e)
        {
            if (this.DayRender != null)
                this.DayRender.Invoke(this, e);
        }
        #endregion

        #region Event Handlers
        //***************************************************************************
        // Child Control Events
        // 
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
        }
        protected override void RaisePostBackEvent(string eventArgument)
        {
            base.RaisePostBackEvent(eventArgument);
        }
        #endregion
    }
    public struct CalendarEventOld
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private string
            _nm;
        //_desc;
        private Uri
            _imgUrl;
        private DateTime
            _stDate,
            _endDate;
        internal EventCalendarOld
            _owner;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public string EventName
        {
            get { return this._nm; }
            set { this._nm = value; }
        }
        //public string EventDescription
        //{
        //    get { return this._desc; }
        //    set { this._desc = value; }
        //}
        public Uri ImageUrl
        {
            get { return this._imgUrl; }
            set { this._imgUrl = value; }
        }
        public DateTime StartDate
        {
            get { return this._stDate; }
            set { this._stDate = value; }
        }
        public DateTime EndDate
        {
            get { return this._endDate; }
            set { this._endDate = value; }
        }
        public EventCalendarOld Owner
        {
            get { return this._owner; }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public CalendarEventOld(string name, string description, DateTime startDate)
            : this(name, startDate, startDate)
        { }
        public CalendarEventOld(string name, DateTime startDate, DateTime endDate)
        {
            this._nm = name;
            //this._desc = description;
            this._stDate = startDate;
            this._endDate = endDate;
            this._imgUrl = null;
            this._owner = null;
        }
        public CalendarEventOld(string name, DateTime startDate, DateTime endDate, Uri imageUrl)
            : this(name, startDate, endDate)
        {
            this._imgUrl = imageUrl;
        }
        #endregion
    }
    public class CalendarEventCollectionOld : RainstormStudios.Collections.ObjectCollectionBase<CalendarEventOld>
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        internal EventCalendarOld
            _owner;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public EventCalendarOld Owner
        {
            get { return this._owner; }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        private CalendarEventCollectionOld()
            : base()
        { }
        internal CalendarEventCollectionOld(EventCalendarOld owner)
            : this()
        {
            this._owner = owner;
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public new void Add(CalendarEventOld value, string key)
        {
            base.Add(value, key);
        }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        #endregion
    }
}
