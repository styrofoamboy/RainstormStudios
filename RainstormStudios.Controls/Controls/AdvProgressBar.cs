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
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Collections;
using System.Text;
using System.Windows.Forms;
using RainstormStudios.Drawing;

namespace RainstormStudios.Controls
{
    [Author("Unfried, Michael")]
    [System.Drawing.ToolboxBitmap(typeof(System.Windows.Forms.ProgressBar))]
    public class AdvancedProgressBar : Control
    {
        #region Declarations
        //***************************************************************************
        // Global Variables
        // 
        Image _pImg;
        Image _bgImg;
        ImageLayout _pImgLo;
        WrapMode _pImgWm;
        ImageLayout _bgImgLo;
        WrapMode _bgImgWm;
        Color _bgColor;
        Color _brdColor;
        Color _txtColor;
        ContentAlignment _txtAlign;
        bool _txtAlignCtrl;
        int _max;
        int _min;
        int _val;
        int _incVal;
        int _border;
        ProgressBarStyle _style;
        int _fthr;
        bool _3d;
        int _blkWidth;
        int _blkSpace;
        //int _mStart;    // Marquee mode left position
        bool _mSamp;
        RotateFlipType _bgImgRot;
        RotateFlipType _pImgRot;
        float _rotBlockDeg;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        [Category("Appearance"), Description("Sets the image to draw as the progress bar."), Browsable(true)]
        public Image ProgressImage
        {
            get { return this._pImg; }
            set { this._pImg = value; this.Refresh(); }
        }
        public ImageLayout ProgressImageLayout
        {
            get { return this._pImgLo; }
            set { this._pImgLo = value; this.Refresh(); }
        }
        public WrapMode ProgressImageTileMode
        {
            get { return this._pImgWm; }
            set { this._pImgWm = value; this.Refresh(); }
        }
        public RotateFlipType ProgressImageRotateFlip
        {
            get { return this._pImgRot; }
            set { this._pImgRot = value; this.Refresh(); }
        }
        [Category("Appearance"), Description("Specifies the image to display behind the progress bar."), Browsable(true)]
        public Image ProgressBackgroundImage
        {
            get { return this._bgImg; }
            set { this._bgImg = value; this.Refresh(); }
        }
        public ImageLayout ProgressBackgroundImageLayout
        {
            get { return this._bgImgLo; }
            set { this._bgImgLo = value; this.Refresh(); }
        }
        public WrapMode ProgressBackgroundImageTileMode
        {
            get { return this._bgImgWm; }
            set { this._bgImgWm = value; this.Refresh(); }
        }
        public RotateFlipType ProgressBackgroundImageRotateFlip
        {
            get { return this._bgImgRot; }
            set { this._bgImgRot = value; this.Refresh(); }
        }
        [Category("Appearance"), Description("Specifies the background color of the area behind the progress bar."), Browsable(true)]
        public Color ProgressBackgroundColor
        {
            get { return this._bgColor; }
            set { this._bgColor = value; this.Refresh(); }
        }
        [Category("Appearance"), Description("Specifies the style to draw the progress bar with."), Browsable(true), DefaultValue(ProgressBarStyle.Blocks)]
        public ProgressBarStyle Style
        {
            get { return this._style; }
            set { this._style = value; this.Refresh(); }
        }
        [Category("Behavior"), Description("Specifies the maximum scale value for the progress bar."), Browsable(true), DefaultValue(100)]
        public int Maximum
        {
            get { return this._max; }
            set
            {
                this._max = value;
                this.Value = this._val;
                this.Refresh();
            }
        }
        //public int Minimum
        //{
        //    get { return this._min; }
        //    set { this._min = value; this.Refresh(); }
        //}
        [Category("Behavior"), Description("Specifies the current value of the progress bar"), Browsable(true), DefaultValue(0)]
        public int Value
        {
            get { return this._val; }
            set { this._val = System.Math.Min(value, this._max); this.Refresh(); }
        }
        [Category("Behavior"), Description("Specifies the amount to increment the progress bar's value property by when the 'Increment()' method is called."), Browsable(true), DefaultValue(1)]
        public int Step
        {
            get { return _incVal; }
            set { _incVal = System.Math.Min(value, this._max); }
        }
        [Category("Appearance"), Description("Specifies the number of pixels to feather (round) the corners of the border."), Browsable(true), DefaultValue(4)]
        public int CornerFeather
        {
            get { return this._fthr; }
            set
            {
                //if ((value * 2) >= (this.ClientRectangle.Width - 1))
                //    this._fthr = (this.ClientRectangle.Width - 1) / 2;
                //else if ((value * 2) >= (this.ClientRectangle.Height - 1))
                //    this._fthr = (this.ClientRectangle.Height - 1) / 2;
                //else
                //    this._fthr = value;

                this._fthr = System.Math.Min(value, ((this.ClientRectangle.Width > this.ClientRectangle.Height) ? (this.ClientRectangle.Height - 1) / 2 : (this.ClientRectangle.Width - 1) / 2));
                this.Refresh();
            }
        }
        [Category("Appearance"), Description("Specifies the number of pixels wide to draw the border."), Browsable(true), DefaultValue(1)]
        public int BorderSize
        {
            get { return this._border; }
            set { this._border = value; this.Refresh(); }
        }
        [Category("Appearance"), Description("Specifies the color of the control's border."), Browsable(true)]
        public Color BorderColor
        {
            get { return this._brdColor; }
            set { this._brdColor = value; this.Refresh(); }
        }
        [Category("Appearance"), Description("Turns the control's 3D effects on or off."), Browsable(true), DefaultValue(true)]
        public bool ThreeDimensionalEffect
        {
            get { return this._3d; }
            set { this._3d = value; this.Refresh(); }
        }
        [Category("Behavior"), Description("Specifies the width to draw the blocks when the progress bar is in 'block' mode."), Browsable(true), DefaultValue(6)]
        public int BlockWidth
        {
            get { return this._blkWidth; }
            set { this._blkWidth = System.Math.Max(value, 1); this.Refresh(); }
        }
        [Category("Behavior"), Description("Specifies the amount of white space (in pixels) between blocks when the progress bar is in 'block' mode."), Browsable(true), DefaultValue(2)]
        public int BlockSpacing
        {
            get { return this._blkSpace; }
            set { this._blkSpace = value; this.Refresh(); }
        }
        [Category("Design"), Description("Specifies whether or not the control should smooth the edges when drawing itself using multi-sampled anti-aliasing."), Browsable(true), DefaultValue(true)]
        public bool MultiSample
        {
            get { return this._mSamp; }
            set { this._mSamp = value; Refresh(); }
        }
        [Category("Design"), Description("Specifies the text to overlay on top of the progress bar. Some constants exist:\n'\\v' = current value.\n'\\m' = maximum value.\n'\\p' = percent."), Browsable(true), DefaultValue("")]
        public override string Text
        {
            get { return base.Text; }
            set { base.Text = value; this.Refresh(); }
        }
        [Category("Appearance"), Description("Specifies the color of text drawn over the progress bar."), Browsable(true)]
        public Color TextColor
        {
            get { return this._txtColor; }
            set { this._txtColor = value; this.Refresh(); }
        }
        [Category("Appearance"), Description("Specifies how text should be positioned in relation to the control."), Browsable(true), DefaultValue(ContentAlignment.MiddleCenter)]
        public ContentAlignment TextAlignment
        {
            get { return this._txtAlign; }
            set { this._txtAlign = value; this.Refresh(); }
        }
        [Category("Design"), Description("Specifies whether text should be aligned to the control's bounds (true) or to the progress bar itself (false)."), Browsable(true), DefaultValue(true)]
        public bool TextAlignToControl
        {
            get { return this._txtAlignCtrl; }
            set { this._txtAlignCtrl = value; this.Refresh(); }
        }
        [Category("Behavior"), Description("Specifies the number of degrees to rotate blocks about their individual centers when the progress bar is in 'block' mode."), Browsable(true), DefaultValue(0)]
        public float BlockRotationDegrees
        {
            get { return this._rotBlockDeg; }
            set { this._rotBlockDeg = value; this.Refresh(); }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public AdvancedProgressBar()
        {
            this.DoubleBuffered = true;
            this.MinimumSize = new Size(8, 8);
            this.Size = new Size(150, 24);
            _pImg = null;
            _bgImg = null;
            _pImgLo = ImageLayout.None;
            _pImgRot = RotateFlipType.RotateNoneFlipNone;
            _bgImgLo = ImageLayout.None;
            _bgImgRot = RotateFlipType.RotateNoneFlipNone;
            _min = 0;
            _max = 100;
            _val = 0;
            _incVal = 1;
            _border = 1;
            _style = ProgressBarStyle.Blocks;
            _fthr = 4;
            this.ForeColor = Color.FromArgb(131, 174, 118);
            this.BackColor = SystemColors.Control;
            _bgColor = Color.White;
            _brdColor = Color.Gray;
            _3d = true;
            _blkWidth = 6;
            _blkSpace = 2;
            //_mStart = 0;
            _mSamp = true;
            _txtColor = SystemColors.ControlText;
            _txtAlign = ContentAlignment.MiddleCenter;
            _txtAlignCtrl = true;
            _rotBlockDeg = 0;
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public int Increment()
        {
            return Increment(this._incVal);
        }
        public int Increment(int step)
        {
            this.Value = this.Value += step;
            return this.Value;
        }
        #endregion

        #region Base-Class Override Methods
        //***************************************************************************
        // Base-Class Override Methods
        // 
        protected override void OnPaint(PaintEventArgs e)
        {
            {
                Graphics g = e.Graphics;
                Region origRegion = g.Clip.Clone();
                g.Clear(this.BackColor);

                // We use this GraphicsPath to define the area the progress bar can
                //   be drawn in.
                using (GraphicsPath progressArea = new GraphicsPath())
                {
                    // Prepare the clipping region for the progress bar area.
                    FeatheredRectangle rClipRegion = new FeatheredRectangle(0, 0, this.ClientRectangle.Width, this.ClientRectangle.Height, this._fthr);

                    // Set the clipping region for this graphics device to only allow
                    //   drawing in the correct area for the progress bar.
                    g.ResetClip();
                    using (Region drawArea = new Region(rClipRegion.GetRegion()))
                        g.SetClip(drawArea, CombineMode.Replace);

                    // Fill the progress bar area with the selected backrgound color.
                    g.Clear(this._bgColor);

                    // Draw the background image, if any was specified.
                    // We align the image to the control's ClientRectangle, but it
                    //   will be clipped to the feathered rectangle created in
                    //   the 'progressArea' GraphicsPath object.
                    if (this._bgImg != null)
                    {
                        using (Image bgImg = (this._bgImg.Clone() as Image))
                        {
                            bgImg.RotateFlip(this._bgImgRot);
                            switch (this._bgImgLo)
                            {
                                case ImageLayout.Center:
                                    //int xPos = (this.ClientRectangle.Width / 2) - (this._bgImg.Width / 2);
                                    //int yPos = (this.ClientRectangle.Height / 2) - (this._bgImg.Height / 2);
                                    g.DrawImageUnscaled(bgImg, Imaging.CenterImage(bgImg, this.ClientRectangle));
                                    break;
                                case ImageLayout.None:
                                    g.DrawImageUnscaled(bgImg, this.ClientRectangle.Location);
                                    break;
                                case ImageLayout.Stretch:
                                    g.DrawImage(bgImg, this.ClientRectangle);
                                    break;
                                case ImageLayout.Tile:
                                    using (TextureBrush tBrush = new TextureBrush(bgImg, this._bgImgWm))
                                        g.FillRectangle(tBrush, this.ClientRectangle);
                                    break;
                                case ImageLayout.Zoom:
                                    g.DrawImage(bgImg, Imaging.ZoomImage(bgImg, this.ClientRectangle));
                                    break;
                            }
                        }
                    }

                    // Calculate the drawing area for the progress bar.
                    float thirdHt = (this.ClientRectangle.Height - this._border) / 3;
                    int startLeft = this.ClientRectangle.Left + (this._border + 3);
                    int startTop = this.ClientRectangle.Top + (this._border + 3);
                    int valWidth = ((this.ClientRectangle.Width - ((this._border + 3) * 2)) * this._val) / (this._max + this._min);
                    Rectangle bnds = new Rectangle(new Point(startLeft, startTop), new Size(valWidth, this.ClientRectangle.Height - ((this._border + 3) * 2)));

                    // We're not going to draw the progress bar if the control is
                    //   not enabled.
                    if (this.Enabled)
                    {
                        // If we determine that the drawing bounds is greater than nothing,
                        //   draw the progress bar.
                        if (bnds.Width > 0 && bnds.Height > 0)
                        {
                            //// Reset the clipping region for the progress bars
                            //FeatheredRectangle rProgressRegion = new FeatheredRectangle(bnds.Location.X, bnds.Location.Y, this.ClientRectangle.Width - ((this._border + 3) * 2), bnds.Height, this._fthr);
                            //g.SetClip(rProgressRegion.GetRegion(), CombineMode.Replace);

                            // Calculate the Highlight and Shadow colors
                            //   for the progress bar.
                            Color hColor = RgbColor.LightenColor(this.ForeColor, 60);
                            Color hhColor = RgbColor.LightenColor(this.ForeColor, 30);
                            Color sColor = RgbColor.DarkenColor(this.ForeColor, 20);

                            // Create the base brush.  If the 3d effect is on, this will be
                            //   the top of the progress bar.
                            Brush pBrush = null;
                            if (this._3d)
                            {
                                pBrush = new LinearGradientBrush(new Rectangle(bnds.Location, bnds.Size), hhColor, hhColor, LinearGradientMode.Vertical);
                                ColorBlend gradBlend = new ColorBlend(8);
                                gradBlend.Colors = new Color[] { this.ForeColor, hColor, hColor, this.ForeColor, this.ForeColor, sColor, this.ForeColor, hhColor };
                                gradBlend.Positions = new float[] { 0.0f, 0.15f, 0.25f, 0.4f, 0.7f, 0.8f, 0.90f, 1.0f };
                                (pBrush as LinearGradientBrush).InterpolationColors = gradBlend;
                            }
                            else
                                pBrush = new SolidBrush(this.ForeColor);

                            // Finally, draw the progress bar according to the
                            //   specified ProgressBarStyle.
                            switch (this.Style)
                            {
                                case ProgressBarStyle.Blocks:
                                    using (Matrix gMat = new Matrix())
                                    {
                                        for (int i = 1; i < bnds.Width; i = i + this._blkWidth + this._blkSpace)
                                        {
                                            int blockWidth = (((this._val == this._max) && (i + this._blkWidth + this._blkSpace > bnds.Right)) ? System.Math.Min(bnds.Width - i, this._blkWidth) : this._blkWidth);
                                            Rectangle rBlock = new Rectangle(new Point((i) + bnds.Left, bnds.Top), new Size(blockWidth, bnds.Height));
                                            gMat.RotateAt(this._rotBlockDeg, new PointF(rBlock.Right - (rBlock.Width / 2), rBlock.Bottom - (rBlock.Height / 2)));
                                            g.Transform = gMat;
                                            g.FillRectangle(pBrush, rBlock);

                                            if (this._pImg != null)
                                            {
                                                using (Image pImg = (this._pImg.Clone() as Image))
                                                {
                                                    pImg.RotateFlip(this._bgImgRot);
                                                    switch (this._pImgLo)
                                                    {
                                                        case ImageLayout.Center:
                                                            //int xPos = (rBlock.Width / 2) - (this._pImg.Width / 2);
                                                            //int yPos = (rBlock.Height / 2) - (this._pImg.Height / 2);
                                                            g.DrawImageUnscaled(pImg, Imaging.CenterImage(pImg, rBlock));
                                                            break;
                                                        case ImageLayout.None:
                                                            g.DrawImageUnscaled(pImg, rBlock.Location);
                                                            break;
                                                        case ImageLayout.Stretch:
                                                            g.DrawImage(pImg, rBlock);
                                                            break;
                                                        case ImageLayout.Tile:
                                                            using (TextureBrush tBrush = new TextureBrush(pImg, this._pImgWm))
                                                                g.FillRectangle(tBrush, rBlock);
                                                            break;
                                                        case ImageLayout.Zoom:
                                                            g.DrawImage(pImg, Imaging.ZoomImage(pImg, rBlock));
                                                            break;
                                                    }
                                                }
                                            }
                                            rBlock = Rectangle.Empty;
                                            gMat.Reset();
                                            g.Transform = gMat;
                                        }
                                    }
                                    break;
                                case ProgressBarStyle.Continuous:
                                    g.FillRectangle(pBrush, bnds);

                                    if (this._pImg != null)
                                    {
                                        using (Image pImg = (this._pImg.Clone() as Image))
                                        {
                                            pImg.RotateFlip(this._pImgRot);
                                            switch (this._pImgLo)
                                            {
                                                case ImageLayout.Center:
                                                    //int xPos = (bnds.Width / 2) - (this._pImg.Width / 2);
                                                    //int yPos = (bnds.Height / 2) - (this._pImg.Height / 2);
                                                    g.DrawImageUnscaled(pImg, Imaging.CenterImage(pImg, bnds));
                                                    break;
                                                case ImageLayout.None:
                                                    //g.DrawImageUnscaled(pImg, bnds.Location);
                                                    // If there's no scaling going on, then the image should be
                                                    //   drawn where the progress bar is.
                                                    g.DrawImageUnscaled(pImg, new Point(bnds.Left + valWidth, bnds.Top + (bnds.Height / 2 - pImg.Height / 2)));
                                                    break;
                                                case ImageLayout.Stretch:
                                                    g.DrawImage(pImg, bnds);
                                                    break;
                                                case ImageLayout.Tile:
                                                    using (TextureBrush tBrush = new TextureBrush(pImg, this._pImgWm))
                                                        g.FillRectangle(tBrush, bnds);
                                                    break;
                                                case ImageLayout.Zoom:
                                                    g.DrawImage(pImg, Imaging.ZoomImage(pImg, bnds));
                                                    break;
                                            }
                                        }
                                    }
                                    break;
                                case ProgressBarStyle.Marquee:
                                    break;
                            }
                            sColor = Color.Empty;
                            hColor = Color.Empty;
                            pBrush.Dispose();
                        }
                    }

                    // Draw any text on the progress bar.
                    if (!string.IsNullOrEmpty(this.Text))
                    {
                        using (StringFormat format = new StringFormat())
                        {
                            // Vertical alignment.
                            if (this._txtAlign == ContentAlignment.BottomCenter || this._txtAlign == ContentAlignment.BottomLeft || this._txtAlign == ContentAlignment.BottomRight)
                                format.LineAlignment = StringAlignment.Far;
                            else if (this._txtAlign == ContentAlignment.TopCenter || this._txtAlign == ContentAlignment.TopLeft || this._txtAlign == ContentAlignment.TopRight)
                                format.LineAlignment = StringAlignment.Near;
                            else
                                format.LineAlignment = StringAlignment.Center;

                            // Horizontal alignment.
                            if (this._txtAlign == ContentAlignment.BottomLeft || this._txtAlign == ContentAlignment.MiddleLeft || this._txtAlign == ContentAlignment.TopLeft)
                                format.LineAlignment = StringAlignment.Near;
                            else if (this._txtAlign == ContentAlignment.BottomRight || this._txtAlign == ContentAlignment.MiddleRight || this._txtAlign == ContentAlignment.TopRight)
                                format.LineAlignment = StringAlignment.Far;
                            else
                                format.LineAlignment = StringAlignment.Center;

                            // Draw the text align either to the entire control, or
                            //   only the progress bar.

                            string txtVal = this.Text.Replace(@"\v", this.Value.ToString()).Replace(@"\m", this._max.ToString()).Replace(@"\p", Convert.ToString(System.Math.Round((double)(this._val * 100) / this._max)));
                            using (SolidBrush brush = new SolidBrush(this._txtColor))
                                if (this._txtAlignCtrl)
                                    g.DrawString(txtVal, this.Font, brush, new RectangleF(startLeft, startTop, this.ClientRectangle.Width - ((this._border + 3) * 2), this.ClientRectangle.Height - ((this._border + 3) * 2)), format);
                                else
                                    if (g.MeasureString(txtVal, this.Font).Width > bnds.Width)
                                        g.DrawString(txtVal, this.Font, brush, (float)bnds.X, ((bnds.Height / 2) - (g.MeasureString(txtVal, this.Font).Height / 2)) + bnds.Y);
                                    else
                                        g.DrawString(txtVal, this.Font, brush, RectangleF.FromLTRB(bnds.Left, bnds.Top, bnds.Right, bnds.Bottom), format);
                        }
                    }

                    if (this._mSamp)
                    {
                        g.SmoothingMode = SmoothingMode.AntiAlias;
                        g.CompositingMode = CompositingMode.SourceOver;
                        g.CompositingQuality = CompositingQuality.HighQuality;
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    }

                    // Draw the borders.
                    using (Pen borderPen = new Pen(this.BorderColor, (float)this._border))
                    {
                        if (this._3d)
                            // Draw the top-left edge's inner drop shadow.
                            using (GraphicsPath shadowPath = (rClipRegion.GetBorder().Clone() as GraphicsPath))
                            {
                                shadowPath.Transform(new Matrix(rClipRegion.GetBounds(), new PointF[] { new PointF(1, 1), new PointF(rClipRegion.GetBounds().Width + 1, 1), new PointF(1, rClipRegion.GetBounds().Height + 1) }));
                                using (Pen sPen = new Pen(Color.FromArgb(120, Color.Black), 1.5f))
                                    g.DrawPath(sPen, shadowPath);
                                shadowPath.Transform(new Matrix(rClipRegion.GetBounds(), new PointF[] { new PointF(1, 1), new PointF(rClipRegion.GetBounds().Width + 1, 1), new PointF(1, rClipRegion.GetBounds().Height + 1) }));
                                using (Pen sPen = new Pen(Color.FromArgb(40, Color.Black), 2.0f))
                                    g.DrawPath(sPen, shadowPath);
                            }

                        // Reset the clipping region before drawing the outter
                        //   border lines.
                        g.ResetClip();
                        g.SetClip(origRegion, CombineMode.Replace);

                        // Draw the border lines.
                        g.DrawPath(borderPen, rClipRegion.GetBorder());
                        //}
                    }
                    bnds = Rectangle.Empty;
                }
            }
        }
        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            //base.OnPaintBackground(pevent);
        }
        protected override void OnClientSizeChanged(EventArgs e)
        {
            base.OnClientSizeChanged(e);
            if (this._fthr >= (this.ClientRectangle.Height - ((this._border / 2) * 2) - 1) / 2)
                this._fthr = (this.ClientRectangle.Height - ((this._border / 2) * 2) - 1) / 2;
            else if (this._fthr >= (this.ClientRectangle.Width - ((this._border / 2) * 2) - 1) / 2)
                this._fthr = (this.ClientRectangle.Width - ((this._border / 2) * 2) - 1) / 2;
            this.Refresh();
        }
        #endregion
    }
}
