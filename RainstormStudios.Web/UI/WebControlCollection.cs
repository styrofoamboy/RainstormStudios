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
using System.Web.UI;
using System.Collections.Generic;
using System.Text;
using RainstormStudios.Collections;

namespace RainstormStudios.Collections
{
    public class WebControlCollection : ObjectCollectionBase<System.Web.UI.Control>
    {
        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public new string Add(System.Web.UI.Control ctrl)
        { return base.Add(ctrl, ctrl.ID); }
        public new void Add(System.Web.UI.Control ctrl, string key)
        { base.Add(ctrl, key); }
        public void RenderControl(int idx, HtmlTextWriter writer)
        { this[idx].RenderControl(writer); }
        public void RenderControl(string id, HtmlTextWriter writer)
        { this[id].RenderControl(writer); }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        #endregion
    }
}
