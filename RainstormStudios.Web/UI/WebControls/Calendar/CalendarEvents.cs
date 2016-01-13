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

namespace RainstormStudios.Web.UI.WebControls.Calendar
{
    public enum UserStatus : uint
    {
        Free = 0,
        Tentative,
        Busy,
        OutOfOffice
    }
    public enum EventPriority : uint
    {
        Low = 0,
        Normal,
        High,
        Private
    }
    [Serializable]
    public class CalendarEvent : Serialization.SerializableItem
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private UserCalendar
            _parentCal;
        private object
            _providerEventKey;
        private string
            _subject,
            _desc,
            _location,
            _category;
        private System.Net.Mail.MailAddress
            _host;
        private Uri
            _imgUrl,
            _imgUrlThmb;
        private DateTime
            _stDate,
            _edDate,
            _created;
        private bool
            _reoccuring,
            _allDay;
        private CalendarEvent
            _parentEvent;
        private EventPriority
            _priority;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public UserCalendar ParentCalendar
        {
            get { return this._parentCal; }
            internal set { this._parentCal = value; }
        }
        public object ProviderEventKey
        {
            get { return this._providerEventKey; }
            internal set { this._providerEventKey = value; }
        }
        public DateTime DateCreated
        {
            get { return this._created; }
            internal set { this._created = value; }
        }
        public string Subject
        {
            get { return this._subject; }
            set
            {
                this._subject = value;
                this.IsEdited = true;
            }
        }
        public string Description
        {
            get { return this._desc; }
            set { this._desc = value; }
        }
        public string Location
        {
            get { return this._location; }
            set
            {
                this._location = value;
                this.IsEdited = true;
            }
        }
        public string Category
        {
            get { return this._category; }
            set
            {
                this._category = value;
                this.IsEdited = true;
            }
        }
        public Uri ImageUri
        {
            get { return this._imgUrl; }
            set
            {
                this._imgUrl = value;
                this.IsEdited = true;
            }
        }
        public Uri ImageUriThumbnail
        {
            get { return this._imgUrlThmb; }
            set
            {
                this._imgUrlThmb = value;
                this.IsEdited = true;
            }
        }
        public DateTime EventStartDate
        {
            get { return this._stDate; }
            set
            {
                this._stDate = value;
                this.IsEdited = true;
            }
        }
        public DateTime EventEndDate
        {
            get { return this._edDate; }
            set
            {
                this._edDate = value;
                this.IsEdited = true;
            }
        }
        public bool Reoccuring
        {
            get { return this._reoccuring; }
            set
            {
                this._reoccuring = value;
                this.IsEdited = true;
            }
        }
        public CalendarEvent ParentEvent
        {
            get { return this._parentEvent; }
            internal set { this._parentEvent = value; }
        }
        public bool AllDayEvent
        {
            get { return this._allDay; }
            set
            {
                this._allDay = value;
                this.IsEdited = true;
            }
        }
        public EventPriority Priority
        {
            get { return this._priority; }
            set
            {
                this._priority = value;
                this.IsEdited = true;
            }
        }
        //***************************************************************************
        // Private Properites
        // 
        protected override string SerializationElementName
        {
            get { return "rsCalEvent"; }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public CalendarEvent(System.Net.Mail.MailAddress host)
        {
            this._host = host;
        }
        public CalendarEvent(string hostAddr)
            : this(new System.Net.Mail.MailAddress(hostAddr))
        { }
        #endregion
    }
    [Serializable]
    public class CalendarEventCollection : Serialization.SerializableCollection<CalendarEvent>
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        protected override string SerializationElementName
        {
            get { return "rsCalEventCol"; }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public CalendarEventCollection()
            : base()
        { }
        public CalendarEventCollection(CalendarEvent[] events)
            : this()
        {
            for (int i = 0; i < events.Length; i++)
                this.Add(events[i]);
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public void Add(CalendarEvent eventItem)
        {
            base.Add(eventItem, string.Empty);
        }
        #endregion
    }
}
