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
    [Author("Unfried, Michael")]
    public partial class FileSelectBox : RsUserControlBase
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private FileDialogType fdType = FileDialogType.Open;
        private bool addExt = false;
        private bool chkFileExst = true;
        private bool chkPathExst = true;
        private string defExt = "";
        private string dlgFilter = "";
        private int intFilterIndex = 0;
        private string dlgInitDir = "";
        private bool dlgMultSel = false;
        private bool dlgRestDir = false;
        private bool dlgSupDotExt = true;
        private string dlgTitle = "";
        private bool dlgValNames = true;
        private bool dlgCreatePrompt = false;
        private bool dlgOvrWritePrompt = true;
        //***************************************************************************
        // Public Events
        // 
        public event EventHandler FileBrowserCompleted;
        public event EventHandler FileBrowserCanceled;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        [Browsable(false)]
        public string FileName
        {
            get { return txtFileName.Text; }
            set { txtFileName.Text = value; }
        }
        [Browsable(false)]
        public string[] FileNames
        {
            get { return txtFileName.Lines; }
            set { txtFileName.Lines = value; }
        }
        [Category("Appearance"), Description("Gets or sets the text displayed in the control's bounding box."), Browsable(true), DefaultValue("File Path")]
        public string ControlTitle
        {
            get { return grpFileName.Text; }
            set { grpFileName.Text = value; }
        }
        [Category("Behavior"), Description("Sets the type of dialog to open when the browse button is clicked."), Browsable(true)]
        public FileDialogType FileSelectType
        {
            get { return fdType; }
            set
            {
                fdType = value;
                if (value == FileDialogType.Save)
                {
                    dlgMultSel = false;
                    txtFileName.Multiline = false;
                    txtFileName.ScrollBars = ScrollBars.None;
                }
            }
        }
        [Category("Behavior"), Description("Specifies whether or not the dialog box should automatically add an extension to filenames."), Browsable(true), DefaultValue(true)]
        public bool AddExtension
        {
            get { return addExt; }
            set { addExt = value; }
        }
        [Category("Behavior"), Description("Specifies whether or not the dialog box should check to see if the file exists."), Browsable(true), DefaultValue(true)]
        public bool CheckFileExists
        {
            get { return chkFileExst; }
            set { chkFileExst = value; }
        }
        [Category("Behavior"), Description("Specifies whether or not the dialog box should check to see if the path exists."), Browsable(true), DefaultValue(true)]
        public bool CheckPathExists
        {
            get { return chkPathExst; }
            set { chkPathExst = value; }
        }
        [Category("Behavior"), Description("Sets the default extension to be appended to filenames entered into the file browse dialog box."), Browsable(true)]
        public string DefaultExtension
        {
            get { return defExt; }
            set { defExt = value; }
        }
        [Category("Behavior"), Description("Sets the file filters for the file browse dialog box.  The format is a pipe-delimited string with each pair of values representing one filter.\n  ie: 'Display File Type (*.*)|*.*'"), Browsable(true)]
        public string DialogFilter
        {
            get { return dlgFilter; }
            set { dlgFilter = value; }
        }
        [Category("Behavior"), Description("Sets the default selected file type filter in the file browse dialog box.  This is a zero-based index counting each pair of pipe-delimited values in the FileBrowseFilter property."), Browsable(true)]
        public int DialogFilterIndex
        {
            get { return intFilterIndex; }
            set { intFilterIndex = value; }
        }
        [Category("Behavior"), Description("Sets the initial directory of the file browse dialog box."), Browsable(true)]
        public string InitialDirectory
        {
            get { return dlgInitDir; }
            set { dlgInitDir = value; }
        }
        [Category("Behavior"), Description("Specifies whether or not the file browse dialog box should allow multiple files to be selected."), Browsable(true), DefaultValue(false)]
        public bool MultiSelect
        {
            get { return dlgMultSel; }
            set
            {
                if (fdType == FileDialogType.Save)
                {
                    dlgMultSel = false;
                    txtFileName.Multiline = false;
                    txtFileName.ScrollBars = ScrollBars.None;
                }
                else
                {
                    dlgMultSel = value;
                    txtFileName.Multiline = value;
                    if (value)
                        txtFileName.ScrollBars = ScrollBars.Vertical;
                    else
                        txtFileName.ScrollBars = ScrollBars.None;
                }
            }
        }
        [Category("Behavior"), Description("Specifies whether or not the file browse dialog box should reset to its initial directory after it closes."), Browsable(true), DefaultValue(true)]
        public bool RestoreDirectory
        {
            get { return dlgRestDir; }
            set { dlgRestDir = value; }
        }
        [Category("Behavior"), Description("Specifies whether or not the file browse dialog box supports file names with multi-dotted extensions."), Browsable(true), DefaultValue(true)]
        public bool SupportMultiDottedExtensions
        {
            get { return dlgSupDotExt; }
            set { dlgSupDotExt = value; }
        }
        [Category("Appearance"), Description("Sets the title bar text of the file browse dialog box."), Browsable(true)]
        public string DialogTitle
        {
            get { return dlgTitle; }
            set { dlgTitle = value; }
        }
        [Category("Behavior"), Description("Specifies whether or not the file browse dialog box should validate file names."), Browsable(true), DefaultValue(true)]
        public bool ValidateFileNames
        {
            get { return dlgValNames; }
            set { dlgValNames = value; }
        }
        [Category("Behavior"), Description("Specifies whether or not the file browse dialog box should ask for use confirmation to create the specified file path if it does not exist.  Only applicable in 'Save' mode."), Browsable(true), DefaultValue(false)]
        public bool CreatePathPrompt
        {
            get { return dlgCreatePrompt; }
            set { dlgCreatePrompt = value; }
        }
        [Category("Behavior"), Description("Specifies whether or not the file browse dialog box should ask the user for confirmation before overwriting an existing file.  Only applicable in 'Save' mode."), Browsable(true), DefaultValue(true)]
        public bool OverwritePrompt
        {
            get { return dlgOvrWritePrompt; }
            set { dlgOvrWritePrompt = value; }
        }
        #endregion*/

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public FileSelectBox()
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
                    if (fdType == FileDialogType.Open)
                    {
                        // Open dialog
                        using (OpenFileDialog dlgOpen = new OpenFileDialog())
                        {
                            dlgOpen.AddExtension = addExt;
                            dlgOpen.CheckFileExists = chkFileExst;
                            dlgOpen.CheckPathExists = chkPathExst;
                            if (!string.IsNullOrEmpty(defExt))
                                dlgOpen.DefaultExt = defExt;
                            if (!string.IsNullOrEmpty(dlgFilter))
                            {
                                dlgOpen.Filter = dlgFilter;
                                dlgOpen.FilterIndex = intFilterIndex;
                            }
                            if (!string.IsNullOrEmpty(dlgInitDir))
                                dlgOpen.InitialDirectory = dlgInitDir;
                            dlgOpen.Multiselect = dlgMultSel;
                            dlgOpen.RestoreDirectory = dlgRestDir;
                            dlgOpen.SupportMultiDottedExtensions = dlgSupDotExt;
                            if (!string.IsNullOrEmpty(dlgTitle))
                                dlgOpen.Title = dlgTitle;
                            dlgOpen.ValidateNames = dlgValNames;

                            if (dlgOpen.ShowDialog(this) == DialogResult.OK)
                            {
                                if (dlgOpen.Multiselect)
                                    txtFileName.Lines = dlgOpen.FileNames;
                                else
                                    txtFileName.Text = dlgOpen.FileName;

                                if (this.FileBrowserCompleted != null)
                                    this.FileBrowserCompleted(this, EventArgs.Empty);
                            }
                            else if (this.FileBrowserCanceled != null)
                                this.FileBrowserCanceled(this, EventArgs.Empty);
                        }
                    }
                    else
                    {
                        using (SaveFileDialog dlgSave = new SaveFileDialog())
                        {
                            dlgSave.AddExtension = addExt;
                            dlgSave.CheckFileExists = chkFileExst;
                            dlgSave.CheckPathExists = chkPathExst;
                            dlgSave.CreatePrompt = dlgCreatePrompt;
                            if (!string.IsNullOrEmpty(defExt))
                                dlgSave.DefaultExt = defExt;
                            if (!string.IsNullOrEmpty(dlgFilter))
                            {
                                dlgSave.Filter = dlgFilter;
                                dlgSave.FilterIndex = intFilterIndex;
                            }
                            if (!string.IsNullOrEmpty(dlgInitDir))
                                dlgSave.InitialDirectory = dlgInitDir;
                            dlgSave.OverwritePrompt = dlgOvrWritePrompt;
                            dlgSave.RestoreDirectory = dlgRestDir;
                            if (!string.IsNullOrEmpty(dlgTitle))
                                dlgSave.Title = dlgTitle;
                            dlgSave.ValidateNames = dlgValNames;

                            if (dlgSave.ShowDialog(this) == DialogResult.OK)
                            {
                                txtFileName.Text = dlgSave.FileName;
                                if (this.FileBrowserCompleted != null)
                                    this.FileBrowserCompleted(this, EventArgs.Empty);
                            }
                            else if (this.FileBrowserCanceled != null)
                                this.FileBrowserCanceled(this, EventArgs.Empty);
                        }
                    }
                    break;
            }
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
