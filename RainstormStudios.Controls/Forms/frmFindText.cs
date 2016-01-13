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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace RainstormStudios.Forms
{
    [Author("Unfried, Michael")]
    public partial class frmFindText : Form
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private Control
            _owner;
        //***************************************************************************
        // Nested Types
        // 
        public enum Scope
        {
            FromCurrentPosition = 0,
            FromBeginning = 1,
            FromEnd = 2
        }
        //***************************************************************************
        // Public Events
        // 
        public event EventHandler FindNext;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public string SearchText
        {
            get { return this.txtSrch.Text; }
            set { this.txtSrch.Text = value; }
        }
        public bool MatchCase
        {
            get { return this.chkMatchCase.Checked; }
            set { this.chkMatchCase.Checked = value; }
        }
        public bool WholeWordOnly
        {
            get { return this.chkWholeWord.Checked; }
            set { this.chkWholeWord.Checked = value; }
        }
        public Scope SearchScope
        {
            get { return (this.rdoFromCurPos.Checked) 
                        ? Scope.FromCurrentPosition 
                        : (this.rdoFromBegin.Checked) 
                                ? Scope.FromBeginning 
                                : Scope.FromEnd; }
        }
        public bool SearchUp
        {
            get { return this.rdoDirUp.Checked; }
            set { this.rdoDirUp.Checked = value; }
        }
        public new Control Owner
        {
            get { return this._owner; }
        }
        public bool EnableRegExSearch
        {
            get { return this.chkRegEx.Enabled; }
            set { this.chkRegEx.Enabled = value; }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public frmFindText()
        {
            InitializeComponent();
        }
        public frmFindText(string defSearch)
            : this()
        {
            this.SearchText = defSearch;
        }
        public frmFindText(Control owner)
            : this()
        {
            this._owner = owner;
        }
        public frmFindText(Control owner, string defSearch)
            : this(defSearch)
        {
            this._owner = owner;
        }
        #endregion

        #region Event Handlers
        //***************************************************************************
        // Event Handlers
        // 
        private void cmdFindNext_Click(object sender, EventArgs e)
        {
            this.InvokeFindNext();
        }
        private void cmdCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void txtSrch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
                this.InvokeFindNext();
        }
        //***************************************************************************
        // Event Triggers
        // 
        private void InvokeFindNext()
        {
            if (this.FindNext != null)
                this.FindNext.Invoke(this, EventArgs.Empty);
        }
        #endregion
    }
}