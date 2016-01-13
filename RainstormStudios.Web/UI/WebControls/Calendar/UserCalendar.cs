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
    [Serializable]
    public class UserCalendar : Serialization.SerializableItem
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private object
            _calKey,
            _ownerKey;
        private string
            _calNm;
        private DateTime
            _created;
        private object
            _createdByKey;
        private System.Drawing.Color
            _clr;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        [Serialization.SerializeProperty()]
        public object ProviderCalendarKey
        {
            get { return this._calKey; }
            internal set { this._calKey = value; }
        }
        [Serialization.SerializeProperty()]
        public object OwnerKey
        {
            get { return this._ownerKey; }
        }
        [Serialization.SerializeProperty()]
        public string CalendarName
        {
            get { return this._calNm; }
            set { this._calNm = value; }
        }
        [Serialization.SerializeProperty()]
        public DateTime CreatedDate
        {
            get { return this._created; }
        }
        [Serialization.SerializeProperty()]
        public object CreatedByKey
        {
            get { return this._createdByKey; }
        }
        [Serialization.SerializeProperty()]
        public System.Drawing.Color Color
        {
            get { return this._clr; }
            set { this._clr = value; }
        }
        //***************************************************************************
        // Private Properties
        // 
        protected override string SerializationElementName
        { get { return "rsUserCal"; } }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public UserCalendar()
            : base()
        { }
        #endregion
    }
    [Serializable]
    public class UserCalendarCollection : Serialization.SerializableCollection<UserCalendar>
    {
        #region Properties
        //***************************************************************************
        // Private Properties
        // 
        protected override string SerializationElementName
        {
            get { return "rsUserCalCol"; }
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public void Add(UserCalendar val)
        {
            base.Add(val, val.CalendarName);
        }
        #endregion
    }
}
