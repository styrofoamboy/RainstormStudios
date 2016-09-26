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
using System.Drawing;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace RainstormStudios.Controls
{
    public class ThreeDLine : System.Windows.Forms.Control
    {
        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        [Category("Design"), Description("Specifies the minimum size of the control in pixels."), Browsable(true), DefaultValue(0.0)]
        public override System.Drawing.Size MinimumSize
        {
            get
            {
                return base.MinimumSize;
            }
            set
            {
                //base.MinimumSize = value;
            }
        }
        [Category("Design"), Description("Specifies the maximum size of the control in pixels."), Browsable(true), DefaultValue(0.0)]
        public override System.Drawing.Size MaximumSize
        {
            get
            {
                return base.MaximumSize;
            }
            set
            {
                //base.MaximumSize = value;
            }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public ThreeDLine()
            : base()
        {
            //base.MinimumSize = new System.Drawing.Size(0, 2);
            //base.MaximumSize = new System.Drawing.Size(50000, 2);
        }
        #endregion

        #region Event Handlers
        //***************************************************************************
        // Event Overrides
        // 
        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            //base.OnPaint(e);
            int r = this.ClientRectangle.Width - 1;
            int c = this.ClientRectangle.Height / 2;
            Graphics g = e.Graphics;
            g.Clear(this.BackColor);
            g.DrawLine(SystemPens.ControlDark, new Point(0, c), new Point(0, c + 1));
            g.DrawLine(SystemPens.ControlDark, new Point(1, c), new Point(r - 1, c));
            g.DrawLine(SystemPens.ControlLightLight, new Point(1, c + 1), new Point(r - 1, c + 1));
            g.DrawLine(SystemPens.ControlLightLight, new Point(r, c), new Point(r, c + 1));
        }
        #endregion
    }
}
