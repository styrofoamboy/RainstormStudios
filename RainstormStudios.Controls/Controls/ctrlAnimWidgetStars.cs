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
using System.ComponentModel;
using System.Collections;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace RainstormStudios.Controls
{
    [Author("Unfried, Michael")]
    public class AnimWidgetStars : Panel
    {
        #region StarClass
        public class StarPoint
        {
            //***************************************************************************
            // Global Variables
            // 
            float _xPos, _yPos, _xSpd, _ySpd;
            int _steps;
            Rectangle _drawBounds;
            StarPointCollection _owner;
            //***************************************************************************
            // Public Properties
            // 
            public float xPosition
            {
                get { return _xPos; }
                set { _xPos = value; }
            }
            public float yPosition
            {
                get { return _yPos; }
                set { _yPos = value; }
            }
            public int StepCount
            {
                get { return _steps; }
            }
            public float xSpeed
            {
                get { return _xSpd; }
                set { _xSpd = value; }
            }
            public float ySpeed
            {
                get { return _ySpd; }
                set { _ySpd = value; }
            }
            public StarPointCollection OwnerCollection
            {
                get { return _owner; }
                set { _owner = value; }
            }
            public Rectangle EllipseDot
            {
                get { return new Rectangle((int)_xPos - 1, (int)_yPos - 1, 2, 2); }
            }
            public bool isVisible
            {
                get
                {
                    if (_xPos > _drawBounds.Left && _xPos < _drawBounds.Right && _yPos > _drawBounds.Top && _yPos < _drawBounds.Bottom)
                        return true;
                    else
                        return false;
                }
            }
            //***************************************************************************
            // Class Constructors
            // 
            public StarPoint(Rectangle drawLimits)
                : this(drawLimits, null)
            { }
            public StarPoint(Rectangle drawLimits, StarPointCollection owner)
            {
                this._drawBounds = drawLimits;
                int xCen = _drawBounds.Width / 2, yCen = _drawBounds.Height / 2;

                Random rnd;
                if (owner != null)
                {
                    this._owner = owner;
                    rnd = _owner.Rnd;
                }
                else
                    rnd = new Random();

                while (_xPos == 0 || _xPos == xCen || (_xPos > xCen - 5 && _xPos < xCen + 5))
                    this._xPos = rnd.Next(drawLimits.Width);
                while (_yPos == 0 || _yPos == yCen || (_yPos > yCen - 5 && _yPos < yCen + 5))
                    this._yPos = rnd.Next(drawLimits.Height);
                this._xSpd = (_xPos - xCen) / 2;
                this._ySpd = (_yPos - yCen) / 2;
                this._steps = 0;
            }
            //***************************************************************************
            // Public Methods
            // 
            public void MoveStar()
            {
                MoveStar(MovementSpeed.Default);
            }
            public void MoveStar(MovementSpeed speed)
            {
                this._xPos += (((_xSpd + ((int)speed)) / 4) * ((((_steps / 10) > 0) ? (_steps / 10) : 1) * System.Math.Sign(_xSpd))) - 1;
                this._yPos += (((_ySpd + ((int)speed)) / 4) * ((((_steps / 10) > 0) ? (_steps / 10) : 1) * System.Math.Sign(_ySpd))) - 1;
                if (_steps < 255)
                    this._steps += 3;
            }
        }
        public class StarPointCollection : CollectionBase
        {
            //***********************************************************************
            // Global Variables
            // 
            public Random Rnd;
            //***********************************************************************
            // Public Properties
            // 
            public StarPoint this[int index]
            {
                get { return (StarPoint)List[index]; }
                set { List[index] = value; }
            }
            //***********************************************************************
            // Class Constructors
            // 
            public StarPointCollection()
            {
                Rnd = new Random();
            }
            //***********************************************************************
            // Public Methods
            // 
            public void Add(StarPoint value)
            {
                value.OwnerCollection = this;
                this.List.Add(value);
            }
            public void Remove(StarPoint value)
            {
                this.List.Remove(value);
            }
        }
        #endregion

        #region Declarations
        //***************************************************************************
        // Global Variables
        // 
        private Timer _timer = new Timer();
        private int _starCount = 200;
        private MovementSpeed _starSpeed = MovementSpeed.Default;
        private StarPointCollection _starCol = null;
        //***************************************************************************
        // Public Properties
        // 
        [Category("Design"), Description("Specifies the number of stars to be visible."), DefaultValue(200), Browsable(true)]
        public int NumberOfStars
        {
            get { return _starCount; }
            set { _starCount = value; }
        }
        [Category("Design"), Description("Sets a global modifier for the speed as which the stars move."), DefaultValue(MovementSpeed.Default), Browsable(true)]
        public MovementSpeed SpeedOfStars
        {
            get { return _starSpeed; }
            set { _starSpeed = value; }
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public void Incr()
        {
            for (int i = 0; i < _starCol.Count; i++)
            {
                _starCol[i].MoveStar();
                if (!_starCol[i].isVisible)
                    _starCol[i] = new StarPoint(this.ClientRectangle, _starCol);
            }
        }
        public void Redraw()
        {
            using (Bitmap bmp = new Bitmap(this.ClientRectangle.Width, this.ClientRectangle.Height))
            {
                using (Graphics gBmp = Graphics.FromImage(bmp))
                {
                    gBmp.Clear(this.BackColor);
                    if (this.BackgroundImage != null)
                        gBmp.DrawImage(this.BackgroundImage, this.ClientRectangle);

                    using (SolidBrush starBrush = new SolidBrush(this.ForeColor))
                        foreach (StarPoint pnt in _starCol)
                        {
                            //starBrush.Color = Color.FromArgb(pnt.StepCount, this.ForeColor);
                            gBmp.FillEllipse(starBrush, pnt.EllipseDot);
                        }
                }
                using (Graphics g = this.CreateGraphics())
                    g.DrawImageUnscaled(bmp, this.ClientRectangle);
            }
        }
        public void AnimStart()
        {
            AnimStart(30);
        }
        public void AnimStart(int interval)
        {
            if (_timer.Enabled)
                _timer.Stop();
            _timer.Interval = interval;
            _timer.Tick += new EventHandler(this.timer_onTick);
            _timer.Start();
        }
        public void AnimStop()
        {
            if (_timer.Enabled)
                _timer.Stop();
        }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        private void InitStarfield()
        {
            if (this._starCol == null)
                this._starCol = new StarPointCollection();
            _starCol.Clear();
            for (int i = 0; i < _starCount; i++)
                _starCol.Add(new StarPoint(this.ClientRectangle, _starCol));
        }
        #endregion

        #region Event Handlers
        //***************************************************************************
        // Event Handlers
        // 
        private void timer_onTick(object sender, EventArgs e)
        {
            this.Incr();
            this.Redraw();
        }
        #endregion

        #region Base-Class Overrides
        //***************************************************************************
        // Base-Class Overrides
        // 
        protected override void OnPaint(PaintEventArgs e)
        {
            //base.OnPaint(e);
            if (!_timer.Enabled)
                using (Graphics g = this.CreateGraphics())
                {
                    g.Clear(this.BackColor);
                    if (this.BackgroundImage != null)
                        g.DrawImage(this.BackgroundImage, this.ClientRectangle);
                }
        }
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            //base.OnPaintBackground(e);
        }
        protected override void OnClientSizeChanged(EventArgs e)
        {
            base.OnClientSizeChanged(e);
            InitStarfield();
        }
        #endregion
    }
}