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
using System.Text;

namespace RainstormStudios.Web.UI.WebControls
{
    public class DropDownList : System.Web.UI.WebControls.DropDownList
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private object
            _defSelVal;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public object DefaultSelectedValue
        {
            get { return this._defSelVal; }
            set { this._defSelVal = value; }
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public override void DataBind()
        {
            base.DataBind();

            // Loop through the data-bound items, try to find the default value
            //   and select it if it's found.
            for (int i = 0; i < this.Items.Count; i++)
                if (this.Items[i].Value == this._defSelVal.ToString())
                {
                    this.Items[i].Selected = true;
                    break;
                }
        }
        #endregion
    }
}
