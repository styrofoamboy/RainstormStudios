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
    public enum InviteStatus : uint
    {
        NoResponse = 0,
        Accept = 1,
        Tentative,
        Decline
    }
    public enum InviteType : uint
    {
        RequiredAttendee = 0,
        OptionalAttendee,
        Resource
    }
    [Serializable]
    public class EventInvite : Serialization.SerializableItem
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private object
            _eventProviderKey;
        private CalendarEvent
            _parentEvent;
        System.Net.Mail.MailAddress
            _user;
        InviteType
            _type;
        bool
            _notify;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public object ProviderEventKey
        {
            get { return this._eventProviderKey; }
        }
        public System.Net.Mail.MailAddress InviteAddress
        {
            get { return this._user; }
            set { this._user = value; }
        }
        public string UserDisplayName
        {
            get { return this.InviteAddress.DisplayName; }
        }
        public InviteType InviteType
        {
            get { return this._type; }
            set { this._type = value; }
        }
        public bool SendNotification
        {
            get { return this._notify; }
            set { this._notify = value; }
        }
        //***************************************************************************
        // Private Properties
        // 
        protected override string SerializationElementName
        {
            get { return "rsCalEventInvite"; }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public EventInvite()
        {
            this._notify = true;
        }
        public EventInvite(System.Net.Mail.MailAddress addr)
            : this()
        {
            this._user = addr;
        }
        public EventInvite(System.Net.Mail.MailAddress addr, InviteType type)
            : this(addr)
        {
            this._type = type;
        }
        public EventInvite(System.Net.Mail.MailAddress addr, InviteType type, bool sendNotification)
            : this(addr, type)
        {
            this._notify = sendNotification;
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        #endregion
    }
    [Serializable]
    public class EventInviteCollection : Serialization.SerializableCollection<EventInvite>
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private CalendarEvent
            _parent;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public CalendarEvent EventOwner
        {
            get { return this._parent; }
        }
        //***************************************************************************
        // Private Properties
        // 
        protected override string SerializationElementName
        {
            get { return "rsCalEventInviteCol"; }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public EventInviteCollection(CalendarEvent parentEvent)
            : base()
        {
            this._parent = parentEvent;
        }
        public EventInviteCollection(CalendarEvent parentEvent, EventInvite[] values)
            : this(parentEvent)
        {
            for (int i = 0; i < values.Length; i++)
                this.Add(values[i]);
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public void Add(string emailAddress)
        { this.Add(emailAddress, InviteType.RequiredAttendee, true); }
        public void Add(string emailAddress, InviteType type)
        { this.Add(emailAddress, type, true); }
        public void Add(string emailAddress, InviteType type, bool sendNotification)
        { this.Add(new EventInvite(new System.Net.Mail.MailAddress(emailAddress), type, sendNotification)); }
        public void Add(string emailAddress, string displayName)
        { this.Add(emailAddress, displayName, InviteType.RequiredAttendee, true); }
        public void Add(string emailAddress, string displayName, InviteType type)
        { this.Add(emailAddress, displayName, type, true); }
        public void Add(string emailAddress, string displayName, InviteType type, bool sendNotification)
        { this.Add(new EventInvite(new System.Net.Mail.MailAddress(emailAddress, displayName), type, sendNotification)); }
        public void Add(EventInvite invite)
        { base.Add(invite, invite.InviteAddress.Address); }
        #endregion
    }
}
