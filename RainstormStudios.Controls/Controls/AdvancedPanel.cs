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
using System.Runtime.InteropServices;

namespace RainstormStudios.Controls
{
    [Author("Unfried, Michael")]
    [System.Drawing.ToolboxBitmap(typeof(System.Windows.Forms.Panel))]
    public class AdvancedPanel : System.Windows.Forms.Panel
    {
        #region Declarations
        //***************************************************************************
        // Public Fields
        // 
        private bool
            _susDraw;
        //***************************************************************************
        // Interop Method Imports
        // 
        //[DllImport("user32.dll")]
        //protected static extern bool LockWindowUpdate(IntPtr hWndLock);
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public AdvancedPanel()
            : base()
        { this._susDraw = false; }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public void SuspendRefresh()
        {
            this._susDraw = true;
            //LockWindowUpdate(this.Handle);
        }
        public void ResumeRefresh()
        { this.ResumeRefresh(true); }
        public void ResumeRefresh(bool refreshNow)
        {
            this._susDraw = false;
            //LockWindowUpdate(IntPtr.Zero);
            if (refreshNow)
                this.Refresh();
        }
        public void ForceBackgroundRefresh()
        { this.ForceBackgroundRefresh(this.ClientRectangle); }
        public void ForceBackgroundRefresh(System.Drawing.Rectangle bounds)
        {
            bool tmpVal = this._susDraw;
            try
            {
                this._susDraw = false;
                using (System.Drawing.Graphics g = this.CreateGraphics())
                    this.InvokePaintBackground(this, new System.Windows.Forms.PaintEventArgs(g, bounds));
            }
            catch
            { throw; }
            finally
            {
                this._susDraw = tmpVal;
            }
        }
        public void ForceRefresh()
        { this.ForceRefresh(this.ClientRectangle); }
        public void ForceRefresh(System.Drawing.Rectangle bounds)
        {
            bool tmpVal = this._susDraw;
            try
            {
                this._susDraw = false;
                using (System.Drawing.Graphics g = this.CreateGraphics())
                    this.InvokePaint(this, new System.Windows.Forms.PaintEventArgs(g, bounds));
            }
            catch
            { throw; }
            finally
            {
                this._susDraw = tmpVal;
            }
        }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            if (m.Msg == (int)RainstormStudios.Unmanaged.Win32Messages.WM_PAINT && this._susDraw)
                return;
            base.WndProc(ref m);
        }
        #endregion

        #region Event Handlers
        //***************************************************************************
        // Event Overrides
        // 
        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            if (!this._susDraw)
                base.OnPaint(e);
        }
        protected override void OnPaintBackground(System.Windows.Forms.PaintEventArgs e)
        {
            //if (!this._susDraw)
                base.OnPaintBackground(e);
        }
        #endregion
    }
}
