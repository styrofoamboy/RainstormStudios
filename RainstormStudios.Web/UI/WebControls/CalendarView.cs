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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using RainstormStudios.Providers;
using RainstormStudios.Web.UI.WebControls.Calendar;

namespace RainstormStudios.Web.UI.WebControls.Calendar
{
    public class CalendarView : CompositeControl, INamingContainer, ICallbackEventHandler
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private CalendarEventCollection
            _events;
        private string
            _prevMonText,
            _nextMonText,
            _smtpServer,
            _providerName;
        private Style
            _curMonDateStyle,
            _othMonDateStyle,
            _prevNextStyle,
            _dayHdrStyle,
            _eventStyle;
        private Uri
            _inviteResponseHandlerUrl;
        private bool
            _useJQ2;
        private CalendarProvider
            _provider;
        private DayOfWeek
            _firstDayOfWk;
        private bool
            _shortDayHdr;
        private Collections.WebControlCollection
            _lnkDates;
        //***************************************************************************
        // Child Control References
        // 
        private LinkButton
            _lnkPrevMon,
            _lnkNextMon,
            _lnkHdrSun,
            _lnkHdrMon,
            _lnkHdrTue,
            _lnkHdrWed,
            _lnkHdrThu,
            _lnkHdrFri,
            _lnkHdrSat;
        //***************************************************************************
        // Public Events
        // 
        public event DayRenderEventHandler DayRendered;
        public event EventRenderEventHandler EventRendered;
        public event DayClickedEventHandler DayClicked;
        public event EventClickedEventHandler EventClicked;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public CalendarEventCollection CalendarEvents
        {
            get { return this._events; }
        }
        public string PrevMonthText
        {
            get { return (!string.IsNullOrEmpty(this._prevMonText) ? this._prevMonText : "<<"); }
            set { this._prevMonText = value; }
        }
        public string NextMonthText
        {
            get { return (!string.IsNullOrEmpty(this._nextMonText) ? this._nextMonText : ">>"); }
            set { this._nextMonText = value; }
        }
        public string SMTPServerName
        {
            get { return this._smtpServer; }
            set { this._smtpServer = value; }
        }
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public Style CurrentMonthDateStyle
        {
            get { return this._curMonDateStyle; }
        }
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public Style OtherMonthDateStyle
        {
            get { return this._othMonDateStyle; }
        }
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public Style PrevNextStyle
        {
            get { return this._prevNextStyle; }
        }
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public Style DayHeaderStyle
        {
            get { return this._dayHdrStyle; }
        }
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public Style EventStyle
        {
            get { return this._eventStyle; }
        }
        public string ProviderName
        {
            get { return this._providerName; }
            set { this._providerName = value; }
        }
        public Uri InviteResponseHandlerURL
        {
            get { return this._inviteResponseHandlerUrl; }
            set { this._inviteResponseHandlerUrl = value; }
        }
        public DayOfWeek FirstDayOfWeek
        {
            get { return this._firstDayOfWk; }
            set { this._firstDayOfWk = value; }
        }
        public bool ShowAbbriviatedDayHeaders
        {
            get { return this._shortDayHdr; }
            set { this._shortDayHdr = value; }
        }
        public object DisplayUserKey
        {
            get
            {
                object vsVal = this.ViewState["DisplayUserKey"];
                if (vsVal == null)
                    return null;
                else
                    return vsVal;
            }
            set { this.ViewState["DisplayUserKey"] = value; }
        }
        public int CurrentMonth
        {
            get
            {
                object vsVal = this.ViewState["CurrentMonth"];
                int iVsVal;
                if (vsVal != null && (vsVal is int))
                    return (int)vsVal;
                else if (vsVal == null || !int.TryParse(vsVal.ToString(), out iVsVal))
                    return -1;
                else
                    return iVsVal;
            }
            set { this.ViewState["CurrentMonth"] = value; }
        }
        public int CurrentYear
        {
            get
            {
                object vsVal = this.ViewState["CurrentYear"];
                int iVsVal;
                if (vsVal != null && (vsVal is int))
                    return (int)vsVal;
                else if (vsVal == null || !int.TryParse(vsVal.ToString(), out iVsVal))
                    return -1;
                else
                    return iVsVal;
            }
            set { this.ViewState["CurrentYear"] = value; }
        }
        public int UpcomingEventNoticePollingInterval
        {
            get
            {
                object vsVal = this.ViewState["NoticePollingInterval"];
                int iVsVal;
                if (vsVal != null && (vsVal is int))
                    return (int)vsVal;
                else if (vsVal == null || !int.TryParse(vsVal.ToString(), out iVsVal))
                    return 15;
                else
                    return iVsVal;
            }
            set { this.ViewState["NoticePollingInterval"] = value; }
        }
        //***************************************************************************
        // Private Properties
        // 
        private DateTime dtCurMon
        {
            get { return new DateTime(this.CurrentYear, this.CurrentMonth, 1); }
            set
            {
                this.CurrentMonth = value.Month;
                this.CurrentYear = value.Year;
            }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public CalendarView()
        {
            this._events = new CalendarEventCollection();
            //this._lnkDates = new WebControlCollection();
            this._eventStyle = new Style(this.ViewState);
            this._prevNextStyle = new Style(this.ViewState);
            this._othMonDateStyle = new Style(this.ViewState);
            this._curMonDateStyle = new Style(this.ViewState);
            this._dayHdrStyle = new Style(this.ViewState);
            this.dtCurMon = DateTime.Now;
            this._firstDayOfWk = System.Globalization.DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek;
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public string GetCallbackResult()
        {
            return string.Empty;
        }
        public void RaiseCallbackEvent(string eventArgument)
        {
        }
        public void RaisePostBackEvent(string arg)
        {
        }
        public void LoadUserEvents(object userKey)
        {
            this.DisplayUserKey = userKey;
        }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Protected Methods
        // 
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (!string.IsNullOrEmpty(this._providerName))
                this._provider = CalendarProviderManager.Providers[this._providerName];
            else
                this._provider = CalendarProviderManager.Provider;

            if (this._provider == null)
                throw new Exception("Specified calendar provider name not found or no default calendar provider set.");
        }
        protected override void OnPreRender(EventArgs e)
        {
            // Add the CSS reference link to the page.
            Control lnkCheck = this.Page.Header.FindControl("CalendarCtrlCSS");
            if (lnkCheck == null)
            {
                System.Web.UI.HtmlControls.HtmlLink link = new System.Web.UI.HtmlControls.HtmlLink();
                link.ID = "CalendarCtrlCSS";
                link.Attributes.Add("href", this.Page.ClientScript.GetWebResourceUrl(typeof(RainstormStudios.Web.UI.WebControls.SlideShow), "RainstormStudios.Web.UI.WebControls.style.calendar.css"));
                link.Attributes.Add("type", "text/css");
                link.Attributes.Add("rel", "stylesheet");
                this.Page.Header.Controls.Add(link);
            }

            // Add the JavaScript file reference to the page.
            this.Page.ClientScript.RegisterClientScriptResource(typeof(RainstormStudios.Web.UI.WebControls.SlideShow), "RainstormStudios.Web.UI.WebControls.scripts.calendar.js");
            //string scriptPathRef = this.Page.ClientScript.GetWebResourceUrl(typeof(RainstormStudios.Web.UI.WebControls.SlideShow), "RainstormStudios.Web.UI.WebControls.scripts.calendar.js");
            //this.Page.ClientScript.RegisterClientScriptInclude(this.Page.GetType(), "CalendarCtrlScript", scriptPathRef);

            // Register "pending appointment" check.
            string callBack = Page.ClientScript.GetCallbackEventReference(this, (this.DisplayUserKey != null ? this.DisplayUserKey.ToString() : null), "CalendarView.CheckAppointments", this.ClientID, "CalendarView.CheckAppointmentsError", true);
            string appointCheckScript = string.Format("setInterval(\"{0}\", {1});", callBack, 15000);
            //this.Page.ClientScript.RegisterStartupScript(this.GetType(), "CalendarViewApptCheck", appointCheckScript, true);

            base.OnPreRender(e);
        }
        protected override void Render(HtmlTextWriter writer)
        {
            this.EnsureChildControls();

            if (string.IsNullOrEmpty(this.CssClass))
                writer.AddAttribute(HtmlTextWriterAttribute.Class, "rsCalendarView");
            else
                writer.AddAttribute(HtmlTextWriterAttribute.Class, this.CssClass);
            writer.AddStyleAttribute(HtmlTextWriterStyle.Width, this.Width.ToString());
            //writer.AddStyleAttribute(HtmlTextWriterStyle.Height, this.Height.ToString());
            writer.AddAttribute(HtmlTextWriterAttribute.Cellpadding, "0");
            writer.AddAttribute(HtmlTextWriterAttribute.Cellspacing, "0");
            this.ControlStyle.AddAttributesToRender(writer);
            writer.RenderBeginTag(HtmlTextWriterTag.Table);

            this.RenderColGroupBlock(writer);

            this.RenderControlRow(writer);

            this.RenderHeaderRow(writer);

            DateTime curDay = this.GetCalFirstDay();
            while (curDay.Month <= this.dtCurMon.Month)
            {
                DateTime newCurDay = this.RenderDayRow(writer, curDay);
                if (this.DisplayUserKey != null)
                    this.RenderEventRow(writer, curDay);
                curDay = newCurDay;
            }

            writer.RenderEndTag(); // TABLE
            //base.Render(writer);
        }
        protected void RenderColGroupBlock(HtmlTextWriter writer)
        {
            writer.RenderBeginTag(HtmlTextWriterTag.Colgroup);

            for (int i = 0; i < 7; i++)
            {
                writer.AddStyleAttribute(HtmlTextWriterStyle.Width, "14%");
                writer.RenderBeginTag(HtmlTextWriterTag.Col);
                writer.RenderEndTag();
            }

            writer.RenderEndTag(); // colgroup
        }
        protected void RenderControlRow(HtmlTextWriter writer)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "controls");
            writer.RenderBeginTag(HtmlTextWriterTag.Tr);

            writer.AddAttribute(HtmlTextWriterAttribute.Colspan, "2");
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "prev");
            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            this._prevNextStyle.AddAttributesToRender(writer, this._lnkPrevMon);
            this._lnkPrevMon.RenderControl(writer);
            writer.RenderEndTag();

            writer.AddAttribute(HtmlTextWriterAttribute.Colspan, Convert.ToString(System.Globalization.DateTimeFormatInfo.CurrentInfo.DayNames.Length - 4));
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "monthHdr");
            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            writer.WriteEncodedText(this.dtCurMon.ToString("MMMM yyyy", System.Globalization.DateTimeFormatInfo.CurrentInfo));
            writer.RenderEndTag();

            writer.AddAttribute(HtmlTextWriterAttribute.Colspan, "2");
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "next");
            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            this._prevNextStyle.AddAttributesToRender(writer, this._lnkNextMon);
            this._lnkNextMon.RenderControl(writer);
            writer.RenderEndTag();

            writer.RenderEndTag(); // TR (prev/next controls)
        }
        protected void RenderHeaderRow(HtmlTextWriter writer)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "header");
            writer.RenderBeginTag(HtmlTextWriterTag.Tr);

            DayOfWeek curDay = this._firstDayOfWk;
            int hdrCnt = 0;
            while (curDay != this._firstDayOfWk || hdrCnt == 0)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Class, "dayHeader");
                writer.RenderBeginTag(HtmlTextWriterTag.Td);

                switch (curDay)
                {
                    case DayOfWeek.Sunday:
                        this._lnkHdrSun.RenderControl(writer);
                        curDay = DayOfWeek.Monday;
                        break;
                    case DayOfWeek.Monday:
                        this._lnkHdrMon.RenderControl(writer);
                        curDay = DayOfWeek.Tuesday;
                        break;
                    case DayOfWeek.Tuesday:
                        this._lnkHdrTue.RenderControl(writer);
                        curDay = DayOfWeek.Wednesday;
                        break;
                    case DayOfWeek.Wednesday:
                        this._lnkHdrWed.RenderControl(writer);
                        curDay = DayOfWeek.Thursday;
                        break;
                    case DayOfWeek.Thursday:
                        this._lnkHdrThu.RenderControl(writer);
                        curDay = DayOfWeek.Friday;
                        break;
                    case DayOfWeek.Friday:
                        this._lnkHdrFri.RenderControl(writer);
                        curDay = DayOfWeek.Saturday;
                        break;
                    case DayOfWeek.Saturday:
                        this._lnkHdrSat.RenderControl(writer);
                        curDay = DayOfWeek.Sunday;
                        break;
                }

                writer.RenderEndTag();
                hdrCnt++;
            }

            writer.RenderEndTag(); // TR (day of week header);
        }
        protected DateTime RenderDayRow(HtmlTextWriter writer, DateTime rowStart)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "dates");
            writer.RenderBeginTag(HtmlTextWriterTag.Tr);

            DateTime curDay = rowStart;
            for (int i = 0; i < 7; i++)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Class, "day");
                writer.AddAttribute(HtmlTextWriterAttribute.Valign, "top");
                if (this.ControlStyle != null)
                {
                    System.Web.UI.WebControls.Style tdStyle = new Style();
                    tdStyle.BorderStyle = this.ControlStyle.BorderStyle;
                    tdStyle.BorderColor = this.ControlStyle.BorderColor;
                    tdStyle.BorderWidth = this.ControlStyle.BorderWidth;
                    tdStyle.ForeColor = this.ControlStyle.ForeColor;
                    tdStyle.BackColor = this.ControlStyle.BackColor;
                    tdStyle.AddAttributesToRender(writer);
                }
                writer.RenderBeginTag(HtmlTextWriterTag.Td);

                //LinkButton lnkDate = new LinkButton();
                //lnkDate.ID = "lnkDate_" + curDay.ToString("yyyyMMdd");
                //lnkDate.Text = curDay.Day.ToString();
                //lnkDate.CssClass = "date";
                //lnkDate.CommandName = "ShowDay";
                //lnkDate.CommandArgument = curDay.ToString("yyyyMMdd");
                //lnkDate.Command += new CommandEventHandler(this.lnkDate_OnCommand);
                //lnkDate.ApplyStyle((curDay.Month == this.dtCurMon.Month) ? this._curMonDateStyle : this._othMonDateStyle);
                //this.Controls.Add(lnkDate);
                LinkButton lnkDate = (LinkButton)this._lnkDates[curDay.ToString("yyyyMMdd")];
                lnkDate.RenderControl(writer);
                
                writer.RenderEndTag();
                curDay = curDay.AddDays(1);
            }
            writer.RenderEndTag(); // TR (date row)
            return curDay;
        }
        protected void RenderEventRow(HtmlTextWriter writer, DateTime rowStart)
        {
            DateTime rowEnd = rowStart.Date.AddDays(6);

            writer.AddAttribute(HtmlTextWriterAttribute.Class, "events");
            writer.RenderBeginTag(HtmlTextWriterTag.Tr);

            CalendarEvent[] events = this._provider.GetEvents(this.DisplayUserKey, rowStart.Date, rowEnd.Date);
            if (events.Length == 0)
            {
                for (int i = 0; i < 7; i++)
                    this.RenderEmptyEvent(writer);
            }
            else
            {
                Style eventTextStyle = new Style();
                eventTextStyle.MergeWith(this._eventStyle);
                eventTextStyle.BorderStyle = BorderStyle.None;

                List<CalendarEvent> unrenderedEvents = new List<CalendarEvent>(events);
                DateTime curDay = rowStart;
                while (unrenderedEvents.Count > 0)
                {
                    // Doing the event this way, lets us put multiple events on the same
                    //   row, so long as they don't "interfere" with each other.
                    CalendarEvent curEv = null;
                    if (curDay.Date == rowStart.Date)
                        // Make sure that we only allow events to occured before the
                        //   "current" date if we're on the first cell in the row.
                        curEv = unrenderedEvents.Where(e => e.EventStartDate.Date <= curDay.Date).FirstOrDefault();
                    else
                        curEv = unrenderedEvents.Where(e => e.EventStartDate.Date == curDay.Date).FirstOrDefault();

                    while (curEv == null && curDay.Date <= rowEnd.Date)
                    {
                        this.RenderEmptyEvent(writer);
                        curDay = curDay.AddDays(1);
                        curEv = unrenderedEvents.Where(e => e.EventStartDate.Date == curDay.Date).FirstOrDefault();
                    }

                    if (curEv == null)
                    {
                        // If we still have no current event at this point, end this row,
                        //   start a new one, and let the search start over.
                        curDay = rowStart;
                        writer.RenderEndTag(); // TR (event row)
                        writer.AddAttribute(HtmlTextWriterAttribute.Class, "events");
                        writer.RenderBeginTag(HtmlTextWriterTag.Tr);
                    }
                    else
                    {
                        // Now, we write the actual event.
                        int eventDays = curEv.EventEndDate.Date <= rowEnd.Date
                            ? (int)System.Math.Ceiling(curEv.EventEndDate.Subtract(curDay).TotalDays)
                            : (int)System.Math.Ceiling(rowEnd.Subtract(curDay).TotalDays) + 1;
                        if (eventDays > 1)
                            writer.AddAttribute(HtmlTextWriterAttribute.Colspan, eventDays.ToString());
                        if (curEv.EventStartDate < rowStart && curEv.EventEndDate > rowEnd)
                            writer.AddAttribute(HtmlTextWriterAttribute.Class, "evMid");
                        else if (curEv.EventStartDate < rowStart)
                            writer.AddAttribute(HtmlTextWriterAttribute.Class, "evEnd");
                        else if (curEv.EventEndDate > rowEnd)
                            writer.AddAttribute(HtmlTextWriterAttribute.Class, "evStart");
                        else
                            writer.AddAttribute(HtmlTextWriterAttribute.Class, "evFull");
                        if (this.ControlStyle != null)
                        {
                            System.Web.UI.WebControls.Style tdStyle = new Style();
                            tdStyle.BorderStyle = this.ControlStyle.BorderStyle;
                            tdStyle.BorderColor = this.ControlStyle.BorderColor;
                            tdStyle.BorderWidth = this.ControlStyle.BorderWidth;
                            tdStyle.ForeColor = this.ControlStyle.ForeColor;
                            tdStyle.BackColor = this.ControlStyle.BackColor;
                            tdStyle.AddAttributesToRender(writer);
                        }
                        writer.RenderBeginTag(HtmlTextWriterTag.Td);

                        //if (curEv.EventStartDate < rowStart && curEv.EventEndDate > rowEnd)
                        //    writer.AddAttribute(HtmlTextWriterAttribute.Class, "evMid");
                        //else if (curEv.EventStartDate < rowStart)
                        //    writer.AddAttribute(HtmlTextWriterAttribute.Class, "evEnd");
                        //else if (curEv.EventEndDate > rowEnd)
                        //    writer.AddAttribute(HtmlTextWriterAttribute.Class, "evStart");
                        //else
                        //    writer.AddAttribute(HtmlTextWriterAttribute.Class, "evFull");
                        this._eventStyle.AddAttributesToRender(writer);
                        writer.RenderBeginTag(HtmlTextWriterTag.Div);

                        LinkButton lnkEvent = new LinkButton();
                        lnkEvent.ID = "lnkEvent_" + curEv.ProviderEventKey + "_" + curDay.ToString("yyyyMMdd");
                        lnkEvent.Text = (curEv.EventStartDate.Date == curDay.Date)
                            ? string.Format("{0} {1}", curEv.EventStartDate.ToString(System.Globalization.DateTimeFormatInfo.CurrentInfo.ShortTimePattern), curEv.Subject)
                            : string.Format("<< {0}", curEv.Subject);
                        lnkEvent.ApplyStyle(eventTextStyle);
                        lnkEvent.CommandName = "ShowEvent";
                        lnkEvent.CommandArgument = curEv.ProviderEventKey.ToString();
                        lnkEvent.Command += new CommandEventHandler(this.lnkEvent_Command);

                        this.Controls.Add(lnkEvent);
                        lnkEvent.RenderControl(writer);

                        writer.RenderEndTag(); // DIV
                        writer.RenderEndTag(); // TD

                        unrenderedEvents.Remove(curEv);
                        curDay = curEv.EventEndDate.AddDays(1);
                    }
                }
                while (curDay.Date <= rowEnd.Date)
                {
                    // "Fill" any remaining days on this row.
                    this.RenderEmptyEvent(writer);
                    curDay = curDay.AddDays(1);
                }
            }

            writer.RenderEndTag(); // TR (event row)
        }
        protected void RenderEmptyEvent(HtmlTextWriter writer)
        {
            if (this.ControlStyle != null)
            {
                System.Web.UI.WebControls.Style tdStyle = new Style();
                tdStyle.BorderStyle = this.ControlStyle.BorderStyle;
                tdStyle.BorderColor = this.ControlStyle.BorderColor;
                tdStyle.BorderWidth = this.ControlStyle.BorderWidth;
                tdStyle.ForeColor = this.ControlStyle.ForeColor;
                tdStyle.BackColor = this.ControlStyle.BackColor;
                tdStyle.AddAttributesToRender(writer);
            }
            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "NoEvent");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            writer.Write("&nbsp;");
            writer.RenderEndTag();
            writer.RenderEndTag();
        }
        protected override void CreateChildControls()
        {
            LinkButton lnkPrevMon = new LinkButton();
            lnkPrevMon.ID = "lnkPrevMon";
            lnkPrevMon.Text = this.PrevMonthText;
            lnkPrevMon.Click += new EventHandler(this.lnkPrevMon_OnClick);
            this._lnkPrevMon = lnkPrevMon;

            LinkButton lnkNextMon = new LinkButton();
            lnkNextMon.ID = "lnkNextMon";
            lnkNextMon.Text = this.NextMonthText;
            lnkNextMon.Click += new EventHandler(this.lnkNextMon_OnClick);
            this._lnkNextMon = lnkNextMon;

            Style hdrTextStyle = new System.Web.UI.WebControls.Style();
            hdrTextStyle.MergeWith(this._dayHdrStyle);
            hdrTextStyle.BorderStyle = BorderStyle.None;

            LinkButton lnkHdrSun = new LinkButton();
            lnkHdrSun.ID = "lnkHdrSun";
            lnkHdrSun.Text = this.GetDayName(DayOfWeek.Sunday);
            lnkHdrSun.ApplyStyle(hdrTextStyle);
            lnkHdrSun.CommandName = "ShowDay";
            lnkHdrSun.CommandArgument = DayOfWeek.Sunday.ToString();
            lnkHdrSun.Command += new CommandEventHandler(this.lnkHdr_OnCommand);
            this._lnkHdrSun = lnkHdrSun;

            LinkButton lnkHdrMon = new LinkButton();
            lnkHdrMon.ID = "lnkHdrMon";
            lnkHdrMon.Text = this.GetDayName(DayOfWeek.Monday);
            lnkHdrMon.ApplyStyle(hdrTextStyle);
            lnkHdrMon.CommandName = "ShowDay";
            lnkHdrMon.CommandArgument = DayOfWeek.Monday.ToString();
            lnkHdrMon.Command += new CommandEventHandler(this.lnkHdr_OnCommand);
            this._lnkHdrMon = lnkHdrMon;

            LinkButton lnkHdrTue = new LinkButton();
            lnkHdrTue.ID = "lnkHdrTue";
            lnkHdrTue.Text = this.GetDayName(DayOfWeek.Tuesday);
            lnkHdrTue.ApplyStyle(hdrTextStyle);
            lnkHdrTue.CommandName = "ShowDay";
            lnkHdrTue.CommandArgument = DayOfWeek.Tuesday.ToString();
            lnkHdrTue.Command += new CommandEventHandler(this.lnkHdr_OnCommand);
            this._lnkHdrTue = lnkHdrTue;

            LinkButton lnkHdrWed = new LinkButton();
            lnkHdrWed.ID = "lnkHdrWed";
            lnkHdrWed.Text = this.GetDayName(DayOfWeek.Wednesday);
            lnkHdrWed.ApplyStyle(hdrTextStyle);
            lnkHdrWed.CommandName = "ShowDay";
            lnkHdrWed.CommandArgument = DayOfWeek.Wednesday.ToString();
            lnkHdrWed.Command += new CommandEventHandler(this.lnkHdr_OnCommand);
            this._lnkHdrWed = lnkHdrWed;

            LinkButton lnkHdrThu = new LinkButton();
            lnkHdrThu.ID = "lnkHdrThu";
            lnkHdrThu.Text = this.GetDayName(DayOfWeek.Thursday);
            lnkHdrThu.ApplyStyle(hdrTextStyle);
            lnkHdrThu.CommandName = "ShowDay";
            lnkHdrThu.CommandArgument = DayOfWeek.Thursday.ToString();
            lnkHdrThu.Command += new CommandEventHandler(this.lnkHdr_OnCommand);
            this._lnkHdrThu = lnkHdrThu;

            LinkButton lnkHdrFri = new LinkButton();
            lnkHdrFri.ID = "lnkHdrFri";
            lnkHdrFri.Text = this.GetDayName(DayOfWeek.Friday);
            lnkHdrFri.ApplyStyle(hdrTextStyle);
            lnkHdrFri.CommandName = "ShowDay";
            lnkHdrFri.CommandArgument = DayOfWeek.Friday.ToString();
            lnkHdrFri.Command += new CommandEventHandler(this.lnkHdr_OnCommand);
            this._lnkHdrFri = lnkHdrFri;

            LinkButton lnkHdrSat = new LinkButton();
            lnkHdrSat.ID = "lnkHdrSat";
            lnkHdrSat.Text = this.GetDayName(DayOfWeek.Saturday);
            lnkHdrSat.ApplyStyle(hdrTextStyle);
            lnkHdrSat.CommandName = "ShowDay";
            lnkHdrSat.CommandArgument = DayOfWeek.Saturday.ToString();
            lnkHdrSat.Command += new CommandEventHandler(this.lnkHdr_OnCommand);
            this._lnkHdrSat = lnkHdrSat;

            // Create all link buttons for the days we're going to display.
            if (this._lnkDates != null)
                this._lnkDates.Clear();
            this._lnkDates = new Collections.WebControlCollection();
            DateTime curWkStart = this.GetCalFirstDay();
            while (curWkStart.Date < this.dtCurMon.AddMonths(1))
            {
                DateTime curDay = curWkStart;
                for (int i = 0; i < 7; i++)
                {
                    LinkButton lnkDate = new LinkButton();
                    lnkDate.ID = "lnkDate_" + curDay.ToString("yyyyMMdd");
                    lnkDate.Text = curDay.Day.ToString();
                    lnkDate.CssClass = "date";
                    lnkDate.CommandName = "ShowDay";
                    lnkDate.CommandArgument = curDay.ToString("yyyyMMdd");
                    lnkDate.Command += new CommandEventHandler(this.lnkDate_OnCommand);
                    lnkDate.ApplyStyle((curDay.Month == this.dtCurMon.Month) ? this._curMonDateStyle : this._othMonDateStyle);
                    this._lnkDates.Add(lnkDate, curDay.ToString("yyyMMdd"));

                    curDay = curDay.AddDays(1);
                }
                curWkStart = curDay;
            }

            base.CreateChildControls();

            this.Controls.Add(this._lnkPrevMon);
            this.Controls.Add(this._lnkNextMon);
            this.Controls.Add(this._lnkHdrSun);
            this.Controls.Add(this._lnkHdrMon);
            this.Controls.Add(this._lnkHdrTue);
            this.Controls.Add(this._lnkHdrWed);
            this.Controls.Add(this._lnkHdrThu);
            this.Controls.Add(this._lnkHdrFri);
            this.Controls.Add(this._lnkHdrSat);

            for (int i = 0; i < this._lnkDates.Count; i++)
                this.Controls.Add(this._lnkDates[i]);
        }
        protected override bool OnBubbleEvent(object source, EventArgs args)
        {
            if (args is CommandEventArgs)
            {
                //this.OnModalItemCommand(new ModalItemCommandEventArgs(this, source, (CommandEventArgs)args));
                base.OnBubbleEvent(source, args);
                return true;
            }
            return false;
        }
        //***************************************************************************
        // Private Methods
        // 
        protected DateTime GetFirstOfMonth()
        {
            return new DateTime(this.dtCurMon.Year, this.dtCurMon.Month, 1);
        }
        protected DateTime GetCalFirstDay()
        {
            DateTime monStart = this.GetFirstOfMonth();
            DateTime calStart = monStart;
            while (calStart.DayOfWeek != this.FirstDayOfWeek)
                calStart = calStart.AddDays(-1);
            return calStart;
        }
        protected string GetDayName(DayOfWeek day)
        {
            return (this._shortDayHdr
                    ? System.Globalization.DateTimeFormatInfo.CurrentInfo.GetAbbreviatedDayName(day)
                    : System.Globalization.DateTimeFormatInfo.CurrentInfo.GetDayName(day));
        }
        //***************************************************************************
        // Event Triggers
        // 
        protected void OnDayClicked(DayRenderEventArgs e)
        {
            
        }
        #endregion

        #region Event Handlers
        //***************************************************************************
        // Event Handlers
        // 
        protected void lnkPrevMon_OnClick(object sender, EventArgs e)
        {
            this.dtCurMon = this.dtCurMon.AddMonths(-1);
            this.ClearChildControlState();
            this.RecreateChildControls();
        }
        protected void lnkNextMon_OnClick(object sender, EventArgs e)
        {
            this.dtCurMon = this.dtCurMon.AddMonths(1);
            this.ClearChildControlState();
            this.RecreateChildControls();
        }
        protected void lnkHdr_OnCommand(object sender, CommandEventArgs e)
        {
        }
        protected void lnkDate_OnCommand(object sender, CommandEventArgs e)
        {
        }
        protected void lnkEvent_Command(object sender, EventArgs e)
        {
        }
        #endregion
    }
    public enum CalendarViewMode : uint
    {
        Month = 0,
        Week,
        WorkWeek,
        Day
    }
}
