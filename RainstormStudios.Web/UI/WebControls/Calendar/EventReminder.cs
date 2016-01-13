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
    public enum EventReminderType : uint
    {
        Email = 0,
        SMS
    }
    public enum EventReminderDuration : uint
    {
        Minute = 1,
        Hour,
        Day,
        Week,
        Month,
        Year
    }
    [Serializable]
    public class EventReminder:Serialization.SerializableItem
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private object
            _providerRemindKey;
        private object
            _ownerKey;
        private CalendarEvent
            _parentEvent;
        private EventReminderDuration
            _remDur;
        private int
            _remLen;
        private EventReminderType
            _remType;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public object ProviderReminderKey
        {
            get { return this._providerRemindKey; }
            internal set { this._providerRemindKey = value; }
        }
        public object OwnerKey
        {
            get { return this._ownerKey; }
            internal set { this._ownerKey = value; }
        }
        public CalendarEvent ParentEvent
        {
            get { return this._parentEvent; }
            internal set { this._parentEvent = value; }
        }
        public EventReminderDuration ReminderDuration
        {
            get { return this._remDur; }
            set
            {
                this._remDur = value;
                this.IsEdited = true;
            }
        }
        public int ReminderLength
        {
            get { return this._remLen; }
            set
            {
                this._remLen = value;
                this.IsEdited = true;
            }
        }
        public EventReminderType ReminderType
        {
            get { return this._remType; }
            set
            {
                this._remType = value;
                this.IsEdited = true;
            }
        }
        //***************************************************************************
        // Private Properties
        // 
        protected override string SerializationElementName
        {
            get { return "rsCalEventRemind"; }
        }
        #endregion

        #region Class Constructor
        //***************************************************************************
        // Class Constructors
        // 
        public EventReminder()
            : base()
        { }
        #endregion
    }
    public class EventReminderCollection : Serialization.SerializableCollection<EventReminder>
    {
        #region Properties
        //***************************************************************************
        // Private Properties
        // 
        protected override string SerializationElementName
        {
            get { return "rsCalEventRemindCol"; }
        }
        #endregion
    }
}
