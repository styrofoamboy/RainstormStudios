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
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using RainstormStudios.Data;

namespace RainstormStudios.Controls
{
    public partial class ConnectionStringBox : RsUserControlBase
    {
        #region Declarations
        //***************************************************************************
        // Global Variables
        // 
        private LockedProviderType lockType = LockedProviderType.None;
        //***************************************************************************
        // Public Events
        // 
        public event EventHandler OnConnectionStringChanged;
        public event EventHandler OnProviderTypeChanged;
        public event EventHandler OnStringBuilderOpened;
        public event EventHandler OnStringBuilderCanceled;
        public event EventHandler OnStringBuilderAccept;
        #endregion

        #region Public Properties
        //***************************************************************************
        // Public Fields
        // 
        [Browsable(false)]
        public string ConnectionString
        {
            get { return txtConnStr.Text; }
            set { txtConnStr.Text = value; }
        }
        [Browsable(false)]
        public AdoProviderType ProviderID
        {
            get { return (AdoProviderType)drpProviderType.SelectedIndex; }
            set { drpProviderType.SelectedIndex = (int)value; }
        }
        [Browsable(false)]
        public DbConnectionProperties ConnectionValues
        {
            get { return DbConnectionProperties.FromString(this.ProviderID, this.ConnectionString); }
        }
        [Category("Appearance"), Description("Sets the title text of the group box frame."), DefaultValue("Connection String"), Browsable(true)]
        public string GroupFrameTitle
        {
            get { return grpConnection.Text; }
            set { grpConnection.Text = value; }
        }
        [Category("Behavior"),Description("Provides a method of locking the control to accept and create only one type of connection string."),Browsable(true),DefaultValue(LockedProviderType.None)]
        public LockedProviderType LockedProviderType
        {
            get { return lockType; }
            set { lockType = value; }
        }
        [Category("Appearance"), Description("Sets the backrgound color of the TextBox and DropDownList controls."), Browsable(true)]
        public Color ControlBackColor
        {
            get { return txtConnStr.BackColor; }
            set
            {
                txtConnStr.BackColor = value;
                drpProviderType.BackColor = value;
            }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public ConnectionStringBox()
        {
            InitializeComponent();
            drpProviderType.SelectedIndex = 0;
        }
        #endregion

        #region Event Handlers
        //***************************************************************************
        // Event Handlers
        //
        private void cmdButton_onClick(object sender, EventArgs e)
        {
            switch (((Button)sender).Name)
            {
                case "cmdStringBuilder":
                    RainstormStudios.Forms.frmStringBuilder frm;
                    if (lockType == LockedProviderType.None)
                        frm = new RainstormStudios.Forms.frmStringBuilder();
                    else
                        frm = new RainstormStudios.Forms.frmStringBuilder((AdoProviderType)((int)lockType));

                    if (this.OnStringBuilderOpened != null)
                        this.OnStringBuilderOpened(this, EventArgs.Empty);

                    if (frm.ShowDialog(this) == DialogResult.OK)
                    {
                        if (this.OnStringBuilderAccept != null)
                            this.OnStringBuilderAccept(this, EventArgs.Empty);
                        if (lockType == LockedProviderType.None)
                            drpProviderType.SelectedIndex = (int)frm.AdoProvider;
                        txtConnStr.Text = frm.ConnectionString;
                    }
                    else
                        if (this.OnStringBuilderCanceled != null)
                            this.OnStringBuilderCanceled(this, EventArgs.Empty);

                    if (frm != null)
                        frm.Dispose();
                    break;
            }
        }
        private void drpProviderType_onSelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.OnProviderTypeChanged != null)
                this.OnProviderTypeChanged(this, EventArgs.Empty);
        }
        private void txtConnectionString_onTextChanged(object sender, EventArgs e)
        {
            if (this.OnConnectionStringChanged != null)
                this.OnConnectionStringChanged(this, EventArgs.Empty);
        }
        //***************************************************************************
        // Base Class Overrides
        // 
        protected override void OnVisibleChanged(EventArgs e)
        {
            if (lockType != LockedProviderType.None)
            {
                drpProviderType.SelectedIndex = (int)lockType;
                drpProviderType.Enabled = false;
            }
        }
        #endregion
    }
}
