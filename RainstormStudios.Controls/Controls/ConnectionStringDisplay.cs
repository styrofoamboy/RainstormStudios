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
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace RainstormStudios.Controls
{
    [Author("Unried, Michael")]
    public class ConnectionStringDisplay : TextBox
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        protected string
            _realText;
        protected Regex
            _regx;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public new string Text
        {
            get { return base.Text; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    return;

                this._realText = value;
                Match match = this._regx.Match(value);
                if (match.Success)
                    base.Text = value.Substring(0, match.Index) + "".PadLeft(match.Length, '*') + value.Substring(match.Index + match.Length);
                else
                    base.Text = value;
            }
        }
        public string OriginalText
        {
            get { return this._realText; }
            set { this.Text = value; }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public ConnectionStringDisplay()
            : base()
        {
            this._regx = new Regex(@"(?<=((password)|(pwd))=)[\w\W]*(?=[;$])");
        }
        #endregion

        #region Event Handlers
        //***************************************************************************
        // Event Overrides
        // 
        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
        }
        #endregion
    }
}
