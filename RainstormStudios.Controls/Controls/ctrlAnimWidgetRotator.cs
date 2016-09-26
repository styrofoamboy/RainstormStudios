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
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.ComponentModel;

namespace RainstormStudios.Controls
{
    public class AnimWidgetRotator : System.Windows.Forms.Panel
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        const int
            base_prec = 30;
        private Timer
            _timer;
        private int
            _prec = 1,
            _spd = 300,
            _penWidth = 2,
            _lineCnt = 3,
            _inrDiam = 0,
            _t = 0;
        private float 
            _bgRot = 0.0f;
        private bool
            _funk = false,
            _mSamp = false,
            _drawing = false,
            _autoSt = true;
        private System.Drawing.Drawing2D.CompositingQuality
            _compQual = System.Drawing.Drawing2D.CompositingQuality.Default;
        private System.Drawing.Drawing2D.InterpolationMode
            _intrpQual = System.Drawing.Drawing2D.InterpolationMode.Default;
        private System.Drawing.Drawing2D.SmoothingMode
            _smthMode = System.Drawing.Drawing2D.SmoothingMode.Default;
        #endregion

        #region Public Properties
        //***************************************************************************
        // Public Properties
        // 
        /// <summary>
        /// Gets or sets and integer value specifying the pen width to use for the 'slices' when drawing the animation.
        /// </summary>
        [Category("Design"), Description("Gets or sets and integer value specifying the pen width to use for the 'slices' when drawing the animation."), Browsable(true), DefaultValue(2)]
        public int PenWidth
        {
            get { return _penWidth; }
            set { _penWidth = value; this.Refresh(); }
        }
        /// <summary>
        /// Gets or sets an intiger value specifying the size of the 'hole' drawn in the center of the control durring animation.  '0' = no hole.
        /// </summary>
        [Category("Design"), Description("Gets or sets an intiger value specifying the size of the 'hole' drawn in the center of the control durring animation.  '0' = no hole."), Browsable(true), DefaultValue(0)]
        public int HoleDiameter
        {
            get { return _inrDiam; }
            set { _inrDiam = value; this.Refresh(); }
        }
        /// <summary>
        /// Gets or sets an integer value indicating the precision of the calculation the control uses for animation.
        /// </summary>
        [Category("Design"), Description("Gets or sets an integer value indicating the precision of the calculation the control uses for animation."), Browsable(true), DefaultValue(1)]
        public int CalcPrecision
        {
            get { return _prec; }
            set
            {
                _prec = value;
                if (_prec < 1)
                    _prec = 1;
                else if (_prec > 10)
                    _prec = 10;
                this.Refresh();
            }
        }
        /// <summary>
        /// Gets or sets a bool value indicating whether or not the control should use an alternate calculation for its animation style.
        /// </summary>
        [Category("Behavior"), Description("Gets or sets a bool value indicating whether or not the control should use an alternate calculation for its animation style."), Browsable(true), DefaultValue(false)]
        public bool FunkyMode
        {
            get { return _funk; }
            set { _funk = value; this.Refresh(); }
        }
        /// <summary>
        /// Gets or sets the number of 'slices' shown when drawing the animation.
        /// </summary>
        [Category("Design"), Description("Gets or sets the number of 'slices' shown when drawing the animation."), Browsable(true), DefaultValue(3)]
        public int SliceCount
        {
            get { return _lineCnt; }
            set
            { _lineCnt = value; this.Refresh(); }
        }
        /// <summary>
        /// Gets or sets a bool value indicating whether the control should be drawn with multisampling enabled.
        /// </summary>
        [Category("Appearance"), Description("Gets or sets a bool value indicating whether the control should be drawn with multisampling enabled."), Browsable(true), DefaultValue(false)]
        public bool MultiSample
        {
            get { return _mSamp; }
            set { _mSamp = value; this.Refresh(); }
        }
        /// <summary>
        /// Gets or sets the level of compositing quality used to draw the animation.
        /// </summary>
        [Category("Appearance"), Description("Gets or sets the level of compositing quality used to draw the animation."), Browsable(true), DefaultValue(System.Drawing.Drawing2D.CompositingQuality.Default)]
        public System.Drawing.Drawing2D.CompositingQuality CompositingQuality
        {
            get { return _compQual; }
            set { _compQual = value; this.Refresh(); }
        }
        /// <summary>
        /// Gets or sets the interpolation mode used to draw the animation.
        /// </summary>
        [Category("Appearance"), Description("Gets or sets the interpolation mode used to draw the animation."), Browsable(true), DefaultValue(System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor)]
        public System.Drawing.Drawing2D.InterpolationMode InterpolationMode
        {
            get { return _intrpQual; }
            set { _intrpQual = value; this.Refresh(); }
        }
        /// <summary>
        /// Gets or sets the smoothing mode used to draw the animation.
        /// </summary>
        [Category("Appearance"), Description("Gets or sets the smoothing mode used to draw the animation."), Browsable(true), DefaultValue(System.Drawing.Drawing2D.SmoothingMode.None)]
        public System.Drawing.Drawing2D.SmoothingMode SmoothingMode
        {
            get { return _smthMode; }
            set { _smthMode = value; this.Refresh(); }
        }
        /// <summary>
        /// Gets or sets the angle of rotation to apply to the background image.
        /// </summary>
        [Category("Appearance"), Description("Gets or sets the angle of rotation to apply to the background image."), Browsable(true), DefaultValue(0)]
        public float BackgroundImageRotation
        {
            get { return this._bgRot; }
            set { this._bgRot = value; this.Refresh(); }
        }
        /// <summary>
        /// Gets a bool value indicating whether or not the control is currently animating itself
        /// </summary>
        [Browsable(false)]
        public bool Active
        {
            get { return this._timer.Enabled; }
        }
        /// <summary>
        /// Gets a bool value indicating whether or not the control is currently in the process of drawing itself.
        /// </summary>
        [Browsable(false)]
        public bool Drawing
        {
            get { return this._drawing; }
        }
        /// <summary>
        /// Gets or sets a boolean value indicating whether or not animation should start/stop automatically based on the control's visibility.
        /// </summary>
        [Category("Behavior"), Description("Gets or sets a boolean value indicating whether or not animation should start/stop automatically based on the control's visibility."), Browsable(true), DefaultValue(true)]
        public bool AutoStartStop
        {
            get { return this._autoSt; }
            set { this._autoSt = value; }
        }
        /// <summary>
        /// Gets or sets an integer value indicating the number of miliseconds that should elapse between attempting to redraw the next frame.
        /// </summary>
        [Category("Behavior"), Description("Gets or sets an integer value indicating the number of miliseconds that should elapse between attempting to redraw the next frame."), Browsable(true), DefaultValue(300)]
        public int RefreshDelayMS
        {
            get { return this._spd; }
            set { this._spd = value; }
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        public void Start()
        {
            Start(this._spd);
        }
        public void Start(int RefreshInterval)
        {
            if (this._timer != null && this._timer.Enabled)
                this.Stop();

            this._timer = new Timer();
            this._timer.Interval = RefreshInterval;
            this._timer.Tick += new EventHandler(_timer_Tick);
            this._timer.Start();
        }
        public void Stop()
        {
            if (this._timer != null && this._timer.Enabled)
            {
                this._timer.Stop();
                this._timer.Dispose();
            }
        }
        public void Incr()
        {
            _t++;
            if (_t > (base_prec * _prec))
                _t = 0;
        }
        public void Redraw()
        {
            //float xScl = 0, yScl = 0;
            this._drawing = true;
            try
            {
                // Use a bitmap object for drawing.  This prevents the flicker.
                using (Bitmap bmp = new Bitmap(this.ClientRectangle.Width, this.ClientRectangle.Height))
                {
                    // Create a Graphics object to draw into the bitmap object.
                    using (Graphics gBmp = Graphics.FromImage(bmp))
                    {
                        if (_mSamp)
                        {
                            gBmp.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
                            gBmp.CompositingQuality = this._compQual;
                            gBmp.InterpolationMode = this._intrpQual;
                            gBmp.SmoothingMode = this._smthMode;
                        }
                        // Create a brush to use for drawing the control's background.
                        Brush backBrush = null; Bitmap bgImg = null;
                        {
                            // Calculate the center of the control's client area.
                            int xCen = this.ClientRectangle.Width / 2, yCen = this.ClientRectangle.Height / 2;

                            // Draw the background, effectively erasing the control.
                            // First, we need to clear the image to the proper
                            //   background color.
                            gBmp.Clear(this.BackColor);

                            // Then draw the background image, if there is one.
                            if (this.BackgroundImage != null)
                            {

                                // If the control has a background image, we need to draw
                                //   that image instead of the background color.
                                bgImg = new Bitmap(this.ClientRectangle.Width, this.ClientRectangle.Height);
                                using (Graphics gBg = Graphics.FromImage(bgImg))
                                {
                                    // Clear the bgImg with the backcolor.
                                    gBg.Clear(this.BackColor);

                                    // Get a texture brush to draw the control's
                                    //   background image onto.
                                    using (TextureBrush tBrush = new TextureBrush(this.BackgroundImage))
                                    {
                                        // Determine the image layout.
                                        switch (this.BackgroundImageLayout)
                                        {
                                            case ImageLayout.None:
                                                {
                                                    tBrush.WrapMode = System.Drawing.Drawing2D.WrapMode.Clamp;
                                                }
                                                break;
                                            case ImageLayout.Tile:
                                                {
                                                    tBrush.WrapMode = System.Drawing.Drawing2D.WrapMode.Tile;
                                                }
                                                break;
                                            case ImageLayout.Zoom:
                                                {
                                                    tBrush.WrapMode = System.Drawing.Drawing2D.WrapMode.Clamp;
                                                    Rectangle drawSize = RainstormStudios.Drawing.Imaging.ZoomImage(this.BackgroundImage, this.ClientRectangle);
                                                    tBrush.TranslateTransform(drawSize.X, drawSize.Y);
                                                    tBrush.ScaleTransform(((float)drawSize.Width) / ((float)this.BackgroundImage.Width), ((float)drawSize.Height) / ((float)this.BackgroundImage.Height));
                                                    //xScl = ((float)drawSize.Width) / ((float)this.BackgroundImage.Width);
                                                    //yScl = ((float)drawSize.Height) / ((float)this.BackgroundImage.Height);
                                                }
                                                break;
                                            case ImageLayout.Stretch:
                                                {
                                                    tBrush.WrapMode = System.Drawing.Drawing2D.WrapMode.Clamp;
                                                    tBrush.ScaleTransform(((float)this.ClientRectangle.Width) / ((float)this.BackgroundImage.Width), ((float)this.ClientRectangle.Height) / ((float)this.BackgroundImage.Height));
                                                    //xScl = ((float)this.ClientRectangle.Width) / ((float)this.BackgroundImage.Width);
                                                    //yScl = ((float)this.ClientRectangle.Height) / ((float)this.BackgroundImage.Height);
                                                }
                                                break;
                                            case ImageLayout.Center:
                                                {
                                                    tBrush.WrapMode = System.Drawing.Drawing2D.WrapMode.Clamp;
                                                    Point drawPoint = RainstormStudios.Drawing.Imaging.CenterImage(this.BackgroundImage, this.ClientRectangle);
                                                    tBrush.TranslateTransform(drawPoint.X, drawPoint.Y);
                                                }
                                                break;
                                        }
                                        if (this._bgRot != 0)
                                        {
                                            tBrush.RotateTransform(this._bgRot);

                                            // Rotation is done at the top-left corner, so to
                                            //   keep the image centered, we have to adjust
                                            //   the translation transformation.
                                            // TODO:: Will determine how to do this later.
                                        }
                                        gBg.FillRectangle(tBrush, 0, 0, bgImg.Width, bgImg.Height);
                                    }
                                }
                                backBrush = new TextureBrush(bgImg);
                                gBmp.FillRectangle(backBrush, this.ClientRectangle);
                            }
                            else
                                // If there's no background image, then just set the
                                //   background brush to the background color.
                                backBrush = new SolidBrush(this.BackColor);

                            // Create a brush using the control's ForeColor and draw a
                            //   circle padded 5px from the control's client area on all sides.
                            using (SolidBrush foreBrush = new SolidBrush(this.ForeColor))
                                gBmp.FillEllipse(foreBrush, this.ClientRectangle.X + 5, this.ClientRectangle.Y + 5, this.ClientRectangle.Width - 10, this.ClientRectangle.Height - 10);

                            // If the designer specified a center hole "punch-out",
                            //   draw another circle with the background brush.
                            if (_inrDiam > 0)
                            {
                                // Calculate the inner circle size/position.
                                Point inrOrig = new Point((bmp.Width / 2) - _inrDiam, (bmp.Height / 2) - _inrDiam);
                                Size inrSize = new Size(bmp.Width - (((bmp.Width / 2) - _inrDiam) * 2), bmp.Height - (((bmp.Height / 2) - _inrDiam) * 2));
                                Rectangle inrRect = new Rectangle(inrOrig, inrSize);

                                // Draw the Ellipse.
                                gBmp.FillEllipse(backBrush, inrRect);

                                // Clear the size/position objects to release memory.
                                inrRect = Rectangle.Empty;
                                inrOrig = Point.Empty;
                                inrSize = Size.Empty;
                            }
                            //if (_holeWidth > 0)
                            //    gBmp.FillEllipse(backBrush, (this.ClientRectangle.X / 2) - _holeWidth, (this.ClientRectangle.Y / 2) - _holeWidth, _holeWidth * 2, _holeWidth * 2);


                            // Calculate the 'orbital' positions around the center of the control
                            //   that the lines will be drawn to.
                            int[] t = new int[_lineCnt];
                            double[] a = new double[_lineCnt];
                            double[] xPos = new double[_lineCnt];
                            double[] yPos = new double[_lineCnt];

                            using (Pen penLine = new Pen(backBrush, _penWidth))
                            {
                                for (int i = 0; i < _lineCnt; i++)
                                {
                                    t[i] = _t + (i * ((base_prec * _prec) / _lineCnt));
                                    if (t[i] > (base_prec * _prec))
                                        t[i] = t[i] - (base_prec * _prec);
                                    a[i] = System.Math.PI * t[i] / ((base_prec * _prec) / 2);
                                    if (_funk)
                                    {
                                        if (i % 2 > 0)
                                        {
                                            xPos[i] = xCen + (this.ClientRectangle.Width / 2) * System.Math.Sin(a[i]);
                                            yPos[i] = (yCen * (this.ClientRectangle.Height / 2) * System.Math.Cos(a[i])) / 2;
                                        }
                                        else
                                        {
                                            xPos[i] = (xCen * (this.ClientRectangle.Width / 2) * System.Math.Sin(a[i])) / 2;
                                            yPos[i] = yCen + (this.ClientRectangle.Height / 2) * System.Math.Cos(a[i]);
                                        }
                                    }
                                    else
                                    {
                                        xPos[i] = xCen + (this.ClientRectangle.Width / 2) * System.Math.Sin(a[i]);
                                        yPos[i] = yCen + (this.ClientRectangle.Height / 2) * System.Math.Cos(a[i]);
                                    }

                                    gBmp.DrawLine(penLine, (float)xCen, (float)yCen, (float)xPos[i], (float)yPos[i]);
                                }
                            }
                        }
                        // DEBUG:  Draw the transform values:
                        //gBmp.FillRectangle(new SolidBrush(this.BackColor), 0, 0, this.ClientRectangle.Width, 15);
                        //gBmp.DrawString("Transform: x=" + xScl.ToString() + "/y=" + yScl.ToString() + "  Offset: x=" + ((TextureBrush)backBrush).Transform.OffsetX + "/y=" + ((TextureBrush)backBrush).Transform.OffsetY + "  Elements: " + AosConvert.ConcatArray(((TextureBrush)backBrush).Transform.Elements, ","), this.Font, SystemBrushes.ControlText, this.ClientRectangle.Location);

                        backBrush.Dispose();
                        if (bgImg != null)
                            bgImg.Dispose();
                        // Finally, create a Graphics object to draw the Bitmap to the control.
                        using (Graphics g = this.CreateGraphics())
                            g.DrawImageUnscaled(bmp, new Point(0, 0));
                    }
                }
            }
            finally
            { this._drawing = false; }
        }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        #endregion

        #region Event Handlers
        //***************************************************************************
        // Event Handlers
        // 
        void _timer_Tick(object sender, EventArgs e)
        {
            this.Incr();
            this.Redraw();
        }
        #endregion

        #region Override Methods
        //***************************************************************************
        // Override Methods
        // 
        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);

            if (this._autoSt && !this.DesignMode)
            {
                if (this.Visible)
                    this.Start(this._spd);
                else
                    this.Stop();
            }
        }
        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            //base.OnPaint(e);
            if ((this._timer != null && !this._timer.Enabled) || (this.DesignMode && this.Handle != IntPtr.Zero))
                this.Redraw();
        }
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            //base.OnPaintBackground(e);
        }
        protected override void Dispose(bool disposing)
        {
            this.Stop();
            while (this._drawing)
                Application.DoEvents();
            base.Dispose(disposing);
        }
        #endregion
    }
}