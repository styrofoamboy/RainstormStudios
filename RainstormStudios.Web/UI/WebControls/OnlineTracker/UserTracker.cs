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

namespace RainstormStudios.Web.UI.WebControls.OnlineTracker
{
    public class UserTracker : System.Web.UI.Control, System.Web.UI.ICallbackEventHandler
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        static PageUserCollection
            pageCol;
        static readonly object
            lockMe = new object();
        int
            _pollInt = 5;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public int PollingInterval
        {
            get { return this._pollInt; }
            set { this._pollInt = value; }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public UserTracker()
            : base()
        {
            pageCol = new PageUserCollection();
        }
        public UserTracker(int pollInterval)
            : this()
        {
            this._pollInt = pollInterval;
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public void RaiseCallbackEvent(string eventArgument)
        {
            
        }
        public string GetCallbackResult()
        {
            return "1";
        }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        private void AddCurrentUserToPage()
        {
            string thisPage = Context.Request.Path.ToLower();
            string sessionID = Context.Session.SessionID;

            lock (lockMe)
            {
                if (System.Web.Security.Membership.Provider != null)
                {
                }
            }
        }
        #endregion
    }
}
