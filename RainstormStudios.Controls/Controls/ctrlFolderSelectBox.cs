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

namespace RainstormStudios.Controls
{
    public partial class FolderSelectBox : RsUserControlBase
    {
        #region Declarations
        //***************************************************************************
        // Global Variables
        // 
        private string _dlgTitle = "";
        private Environment.SpecialFolder _dlgRootPath = 0;
        private bool _dlgShowNewButton = false;
        //***************************************************************************
        // Public Fields
        // 
        [Browsable(false)]
        public string SelectedPath
        {
            get { return txtFolderPath.Text; }
            set { txtFolderPath.Text = value; }
        }
        [Category("Appearance"), Description("Gets or sets the text displayed in the control's bounding box."), Browsable(true), DefaultValue("Folder Path")]
        public string ControlTitle
        {
            get { return grpFolderPath.Text; }
            set { grpFolderPath.Text = value; }
        }
        [Category("Appearance"), Description("Sets the title bar text of the file browse dialog box."), Browsable(true)]
        public string DialogTitle
        {
            get { return _dlgTitle; }
            set { _dlgTitle = value; }
        }
        [Category("Design"), Description("Specifies whether or not the 'New Folder' button should be displayed beneath the folder tree view."), Browsable(true), DefaultValue(false)]
        public bool CreateNewFolderButton
        {
            get { return _dlgShowNewButton; }
            set { _dlgShowNewButton = value; }
        }
        [Category("Behavior"), Description("Specifies the root folder from which browsing takes place."), Browsable(true)]
        public Environment.SpecialFolder RootFolder
        {
            get { return _dlgRootPath; }
            set { _dlgRootPath = value; }
        }
        //***************************************************************************
        // Public Events
        // 
        public event EventHandler BrowseCompleted;
        public event EventHandler BrowseCanceled;
        public event EventHandler FolderPathChanged;
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public FolderSelectBox()
        {
            InitializeComponent();
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
                case "cmdBrowse":
                    using (FolderBrowserDialog dlg = new FolderBrowserDialog())
                    {
                        dlg.Description = _dlgTitle;
                        dlg.RootFolder = _dlgRootPath;
                        dlg.ShowNewFolderButton = _dlgShowNewButton;
                        if (dlg.ShowDialog(this) == DialogResult.OK)
                        {
                            txtFolderPath.Text = dlg.SelectedPath;
                            if (this.BrowseCompleted != null)
                                this.BrowseCompleted.Invoke(this, EventArgs.Empty);
                        }
                        else
                        {
                            if (this.BrowseCanceled != null)
                                this.BrowseCanceled.Invoke(this, EventArgs.Empty);
                        }
                    }
                    break;
            }
        }
        private void txtFolderPath_onTextChanged(object sender, EventArgs e)
        {
            if (this.FolderPathChanged != null)
                this.FolderPathChanged.Invoke(this, EventArgs.Empty);
        }
#if DEBUG
        protected override void OnVisibleChanged(EventArgs e)
        {
            foreach (Control cn in Controls)
            {
                Console.WriteLine("Control Type: {0}", cn.GetType().ToString());
                if (cn.Controls.Count > 0)
                    foreach (Control cn1 in cn.Controls)
                        Console.WriteLine("Subcontrol Type: {0}", cn1.GetType().ToString());
            }
        }
#endif
        #endregion
    }
}
