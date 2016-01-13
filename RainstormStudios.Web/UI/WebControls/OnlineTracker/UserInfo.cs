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
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RainstormStudios.Web.UI.WebControls.OnlineTracker
{
    public class UserInfo
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private object
            _userKey;
        private string
            _userId,
            _hostName;
        private DateTime
            _startTime,
            _lastUpdateTime;
        #endregion
    }
    public class UserInfoCollection : Collections.ObjectCollectionBase<UserInfoCollection>
    {
        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public void Add(UserInfo val, string userKey)
        { base.Add(val, userKey); }
        #endregion
    }
    public class PageUserCollection : Collections.ObjectCollectionBase<UserInfoCollection>
    {
        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public override UserInfoCollection this[string key]
        {
            get
            {
                UserInfoCollection baseVal = base[key];
                if (baseVal == null)
                {
                    // If no UserInfoCollection object exists for this page, just create one.
                    baseVal = new UserInfoCollection();
                    base.Add(baseVal, key);
                }
                return baseVal;
            }
            set
            {
                base[key] = value;
            }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public PageUserCollection()
            : base()
        {
            base.ReturnNullForKeyNotFound = true;
            base.ReturnNullForIndexNotFound = false;
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public void Add(UserInfoCollection val, string pageKey)
        { base.Add(val, pageKey); }
        #endregion
    }
}
