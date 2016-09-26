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
using System.Threading;
using System.Windows.Forms;

namespace RainstormStudios.Forms
{
    public partial class frmProgress : Form
    {
        #region Declarations
        //***************************************************************************
        // Global Variables
        // 
        private Thread threadExProc = null;
        private GenericCrossThreadDelegate delegateExProc = null;
        #endregion

        #region Public Properties
        //***************************************************************************
        // Public Properties
        // 
        public GenericCrossThreadDelegate ExternalMethod
        {
            get { return this.delegateExProc; }
            set { this.delegateExProc = value; }
        }
        public int ProgressCornerFeather
        {
            get { return progressBar1.CornerFeather; }
            set { progressBar1.CornerFeather = value; }
        }
        public int ProgressBorderWidth
        {
            get { return progressBar1.BorderSize; }
            set { progressBar1.BorderSize = value; }
        }
        public Color ProgressBorderColor
        {
            get { return progressBar1.BorderColor; }
            set { progressBar1.BorderColor = value; }
        }
        public int ProgressValue
        {
            get { return progressBar1.Value; }
            set { UpdateProgress(value); }
        }
        public bool ThreeDimensionalEffect
        {
            get { return progressBar1.ThreeDimensionalEffect; }
            set { progressBar1.ThreeDimensionalEffect = value; }
        }
        public ProgressBarStyle ProgressBarStyle
        {
            get { return progressBar1.Style; }
            set { progressBar1.Style = value; }
        }
        public int ProgressStyleBlockWidth
        {
            get { return progressBar1.BlockWidth; }
            set { progressBar1.BlockWidth = value; }
        }
        public Color ProgressColor
        {
            get { return progressBar1.ForeColor; }
            set { progressBar1.ForeColor = value; }
        }
        public Image ProgressImage
        {
            get { return progressBar1.ProgressImage; }
            set { progressBar1.ProgressImage = value; }
        }
        public ImageLayout ProgressImageLayout
        {
            get { return progressBar1.ProgressImageLayout; }
            set { progressBar1.ProgressImageLayout = value; }
        }
        public RotateFlipType ProgressImageRotateFlip
        {
            get { return progressBar1.ProgressImageRotateFlip; }
            set { progressBar1.ProgressImageRotateFlip = value; }
        }
        public System.Drawing.Drawing2D.WrapMode ProgressImageTileMode
        {
            get { return progressBar1.ProgressImageTileMode; }
            set { progressBar1.ProgressImageTileMode = value; }
        }
        public string ProgressText
        {
            get { return progressBar1.Text; }
            set { progressBar1.Text = value; }
        }
        public Color ProgressTextColor
        {
            get { return progressBar1.TextColor; }
            set { progressBar1.TextColor = value; }
        }
        public Color ProgressBackgroundColor
        {
            get { return progressBar1.ProgressBackgroundColor; }
            set { progressBar1.ProgressBackgroundColor = value; }
        }
        public Image ProgressBackgroundImage
        {
            get { return progressBar1.ProgressBackgroundImage; }
            set { progressBar1.ProgressBackgroundImage = value; }
        }
        public ImageLayout ProgressBackgroundImageLayout
        {
            get { return progressBar1.ProgressBackgroundImageLayout; }
            set { progressBar1.ProgressBackgroundImageLayout = value; }
        }
        public RotateFlipType ProgressBackgroundImageRotationFlip
        {
            get { return progressBar1.ProgressBackgroundImageRotateFlip; }
            set { progressBar1.ProgressBackgroundImageRotateFlip = value; }
        }
        public System.Drawing.Drawing2D.WrapMode ProgressBackgroundImageTileMode
        {
            get { return progressBar1.ProgressBackgroundImageTileMode; }
            set { progressBar1.ProgressBackgroundImageTileMode = value; }
        }
        public bool AlignProgressTextToControl
        {
            get { return progressBar1.TextAlignToControl; }
            set { progressBar1.TextAlignToControl = value; }
        }
        public ContentAlignment ProgressTextAlignment
        {
            get { return progressBar1.TextAlignment; }
            set { progressBar1.TextAlignment = value; }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        private frmProgress()
        {
            InitializeComponent();

            threadExProc = new Thread(new ThreadStart(this.threadExProcInit));
            threadExProc.IsBackground = true;
        }
        public frmProgress(string WindowCaption, int ProgressMax, GenericCrossThreadDelegate ExternalMethodDelegate)
            : this()
        {
            this.Text = WindowCaption;
            this.progressBar1.Maximum = ProgressMax;
            this.delegateExProc = ExternalMethodDelegate;
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public void ProgressBarIncr()
        {
            this.ProgressBarIncr(1);
        }
        public void ProgressBarIncr(int step)
        {
            this.UpdateProgress(progressBar1.Value + step);
        }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        private void threadExProcInit()
        {
            Thread.CurrentThread.Join(50);
            BeginInvoke(delegateExProc);
        }
        private void ThreadCallback(IAsyncResult result)
        {
            GenericCrossThreadDelegate del = (GenericCrossThreadDelegate)result.AsyncState;
            del.EndInvoke(result);
            this.CloseMe();
        }
        #endregion

        #region Thread-Safe UI Update Methods
        //***************************************************************************
        // Thread-Safe UI Methods
        // 
        private delegate void UpdateProgressDelegate(int value);
        private void UpdateProgress(int value)
        {
            if (progressBar1.InvokeRequired)
            {
                UpdateProgressDelegate updPbDel = new UpdateProgressDelegate(this.UpdateProgress);
                BeginInvoke(updPbDel, new object[] { value });
            }
            else
            {
                if (value < 0)
                    progressBar1.Value = 0;
                else if (value > progressBar1.Maximum)
                    progressBar1.Value = progressBar1.Maximum;
                else
                    progressBar1.Value = value;
            }
        }
        private void CloseMe()
        {
            if (this.InvokeRequired)
            {
                GenericCrossThreadDelegate del = new GenericCrossThreadDelegate(this.CloseMe);
                this.Invoke(del);
            }
            else
                this.Close();
        }
        #endregion

        #region Event Handlers
        //***************************************************************************
        // Event Handlers
        // 
        private void frmProgress_onLoad(object sender, EventArgs e)
        {
            this.Refresh();
            Application.DoEvents();
            //threadExProc.Start();
            delegateExProc.BeginInvoke(new AsyncCallback(this.ThreadCallback), delegateExProc);
        }
        #endregion
    }
}