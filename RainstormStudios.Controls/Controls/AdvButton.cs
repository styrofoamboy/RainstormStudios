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
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;
//using System.Windows.Forms.VisualStyles;
using RainstormStudios.Drawing;

namespace RainstormStudios.Controls
{
    [Author("Unfried, Michael")]
    [System.Drawing.ToolboxBitmap(typeof(System.Windows.Forms.Button)), System.ComponentModel.DesignerCategoryAttribute("UserControl")]
    public class AdvancedButton : Button
    {
        #region Nested Types
        //***************************************************************************
        // Enums
        // 
        public enum AdvButtonStyle
        {
            Flat = 0,
            Popup = 1,
            System = 2,
            Standard = System,
            Beveled = 3,
            Image = 4
        }
        #endregion

        #region Declarations
        //***************************************************************************
        // Global Variables
        // 
        bool _mSamp;
        int _fthr;
        Color _brdClr;
        int _brdWidth;
        GraphicsPath _btnShape;
        AdvButtonStyle _btnType;
        int _3dHeight;
        float _hlMul;
        Color _hvrClr;
        int _hvrOpc;
        bool _msOver;
        bool _msDown;
        bool _txtShd;
        int _txtShdO;
        float _txtShdW;
        bool _txtWrp;
        bool _rndCrnr;
        Image _imgHover;
        Image _imgDown;
        int _imgPad;
        bool _txtVert;
        bool _txtOutln;
        float _txtOutlnW;
        Color _txtOutlnC;
        int _txtOutlnO;
        float _rotTxtDeg;
        float _rotBgImgDeg;
        bool _toggleType;
        bool _trigd;
        Color _disFcolor;
        Color _disBcolor;
        Color _tglClr;
        float _fgReserved;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        [Category("Design"),Description("Specifies the size of feathering to do on the button's corners."),Browsable(true),DefaultValue(3)
        ,DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public int CornerFeather
        {
            get { return this._fthr; }
            set
            {
                this._fthr = System.Math.Min(value, ((this.ClientRectangle.Width > this.ClientRectangle.Height) 
                    ? (this.ClientRectangle.Height - 1) / 2 
                    : (this.ClientRectangle.Width - 1) / 2));
                this.SetButtonRegion();
                if (this.Visible) this.Refresh();
            }
        }
        [Category("Design"),Description("Specifies the color of the button's border."),Browsable(true)]
        public Color BorderColor
        {
            get { return this._brdClr; }
            set { this._brdClr = value; if (this.Visible) this.Refresh(); }
        }
        [Category("Design"),Description("Specifies the width of the button's border."),Browsable(true)]
        public int BorderWidth
        {
            get { return this._brdWidth; }
            set { this._brdWidth = System.Math.Min(value, ((this.ClientRectangle.Width > this.ClientRectangle.Height) ? (this.ClientRectangle.Height / 5) * 2 : (this.ClientRectangle.Width / 5) * 2)); this.SetButtonRegion(); this.Refresh(); }
        }
        [Category("Behavior"),Description("Specifies whether or not the control should use Multi-sampling when drawing iteself."),Browsable(true),DefaultValue(true)]
        public bool MultiSample
        {
            get { return this._mSamp; }
            set { _mSamp = value; if (this.Visible) this.Refresh(); }
        }
        [Browsable(false)]
        public GraphicsPath ButtonShape
        {
            get { return this._btnShape; }
            set
            {
                _btnShape = value;
                this._fthr = 0;
                if (this.Visible) this.Refresh();
            }
        }
        [Category("Design"),Description("Specifies whether or not the button should draw a shadow below its text."),Browsable(true),DefaultValue(false)]
        public bool TextShadow
        {
            get { return this._txtShd; }
            set { this._txtShd = value; this.Refresh(); }
        }
        [Category("Design"),Description("Specifies the offset to use when drawing a text shadow."),Browsable(true),DefaultValue(1.0f)]
        public float TextShadowOffset
        {
            get { return this._txtShdW; }
            set { this._txtShdW = value; this.Refresh(); }
        }
        [Category("Design"),Description("Specifies the opacity of the text shadow, using a 0-255 scale."),Browsable(true),DefaultValue(80)]
        public int TextShadowOpacity
        {
            get { return this._txtShdO; }
            set { this._txtShdO = System.Math.Max(System.Math.Min(value, 255), 0); this.Refresh(); }
            //set
            //{
            //    if (value > 100) this._txtShdO = 255;
            //    else if (value < 0) this._txtShdO = 0;
            //    else this._txtShdO = (value * 255) / 100;
            //}
        }
        public bool TextOutline
        {
            get { return this._txtOutln; }
            set { this._txtOutln = value; if (this.Visible) this.Refresh(); }
        }
        public Color TextOutlineColor
        {
            get { return this._txtOutlnC; }
            set { this._txtOutlnC = value; if (this.Visible) this.Refresh(); }
        }
        public float TextOutlineWeight
        {
            get { return this._txtOutlnW; }
            set { this._txtOutlnW = System.Math.Max(value, 0); if (this.Visible) this.Refresh(); }
        }
        public int TextOutlineOpacity
        {
            get { return this._txtOutlnO; }
            set { this._txtOutlnO = System.Math.Max(0, System.Math.Min(value, 255)); if (this.Visible) this.Refresh(); }
        }
        [Category("Design"),Description("Indicates whether the button's text should be drawn vertically, rotated 90 degress counter-clockwise."),Browsable(true),DefaultValue(false)]
        public bool TextVeritcal
        {
            get { return this._txtVert; }
            set { this._txtVert = value; if (this.Visible) this.Refresh(); }
        }
        public Color HoverHighlightColor
        {
            get { return this._hvrClr; }
            set { _hvrClr = value; }
        }
        public int HoverHighlightOpacity
        {
            get { return this._hvrOpc; }
            set { this._hvrOpc = System.Math.Max(System.Math.Min(value, 255), 0); }
        }
        public int ThreeDEffectDepth
        {
            get { return this._3dHeight; }
            set { this._3dHeight = System.Math.Max(System.Math.Min(value, 100), 0); if (this.Visible) this.Refresh(); }
        }
        public float HighlightMultiplier
        {
            get { return this._hlMul; }
            set { this._hlMul = System.Math.Max(System.Math.Min(value, 10), 1); if (this.Visible) this.Refresh(); }
        }
        public new AdvButtonStyle FlatStyle
        {
            get { return this._btnType; }
            set { this._btnType = value; this.Refresh(); }
        }
        [Category("Behavior"),Description("Indicates whether or not the button's text should automatically wrap when it is too long to fit the control's available space."),Browsable(true),DefaultValue(false)]
        public bool AllowWordWrap
        {
            get { return this._txtWrp; }
            set { this._txtWrp = value; this.Refresh(); }
        }
        public Image HoverImage
        {
            get { return this._imgHover; }
            set { this._imgHover = value; }
        }
        public Image MouseDownImage
        {
            get { return this._imgDown; }
            set { this._imgDown = value; }
        }
        public int ImagePadding
        {
            get { return this._imgPad; }
            set { this._imgPad = value; if (this.Visible) this.Refresh(); }
        }
        [Category("Design"),Description("Specifies the text rotation in degrees."),Browsable(true),DefaultValue(0.0f)]
        public float TextRotationDegrees
        {
            get { return this._rotTxtDeg; }
            set { this._rotTxtDeg = value; if (this.Visible) this.Refresh(); }
        }
        [Category("Design"),Description("Specifieds the background rotation in degrees."),Browsable(true),DefaultValue(0.0f)]
        public float BackgroundRotationDegrees
        {
            get { return this._rotBgImgDeg; }
            set { this._rotBgImgDeg = value; if (this.Visible) this.Refresh(); }
        }
        [Category("Behavior"),Description("Gets or sets a bool value indicating whether or not this button should behave as a 'Toggle', where the button's depressed state persists after being clicked."),Browsable(true),DefaultValue(false)]
        public bool ToggleType
        {
            get { return this._toggleType; }
            set { this._toggleType = value; if (this.Activated)this.Refresh(); }
        }
        [Category("Design"), Description("Gets or sets a bool value indicating whether or not a 'Toggle' type button is in the depressed state."),Browsable(true),DefaultValue(false)]
        public bool Activated
        {
            get { return this._trigd; }
            set { this._trigd = value; if (this.Visible) this.Refresh(); }
        }
        [Category("Appearance"), Description("Defines the color used to draw the button's text when the button's 'Enabled' state is set to false."),Browsable(true)]
        public Color DisabledForeColor
        {
            get { return this._disFcolor; }
            set { this._disFcolor = value; if (!this.Enabled) this.Refresh(); }
        }
        [Category("Appearance"), Description("Defines the color used to draw the button's background when the button's 'Enabled' state is set to false."),Browsable(true)]
        public Color DisabledBackColor
        {
            get { return this._disBcolor; }
            set { this._disBcolor = value; if (!this.Enabled) this.Refresh(); }
        }
        [Category("Appearance"), Description("Gets or sets a value indicating whether the button should draw itself with rounded corners or chiseled corners."), Browsable(true), DefaultValue(true)]
        public bool RoundedCornders
        {
            get { return this._rndCrnr; }
            set
            {
                this._rndCrnr = value;
                this.SetButtonRegion();
                if (this.Visible) this.Refresh();
            }
        }
        public Color ToggleActiveColor
        {
            get { return this._tglClr; }
            set { this._tglClr = value; if (this.Visible)this.Refresh(); }
        }
        //***************************************************************************
        // Private Properties
        // 
        /// <summary>
        /// Provided so that inherting classes can reserve a portion of the right-hand side of the control's surface area for secondary drawing.
        /// </summary>
        protected float ForegroundReservedSpace
        {
            get { return this._fgReserved; }
            set { this._fgReserved = value; }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public AdvancedButton()
        {
            this.MinimumSize = new Size(8, 8);
            this._fthr = 3;
            this._brdClr = Color.FromArgb(0, 60, 116);
            this.BackColor = Color.FromArgb(233, 235, 246);
            this._brdWidth = 1;
            this._mSamp = true;
            this._btnShape = null;
            this._imgHover = null;
            this._imgDown = null;
            this._imgPad = 0;
            this._btnType = AdvButtonStyle.Standard;
            this._hvrClr = Color.Orange;
            this._hvrOpc = 200;
            this._msOver = false;
            this._msDown = false;
            this._3dHeight = 50;
            this._hlMul = 2.0f;
            this._txtShd = false;
            this._txtShdO = 80;
            this._txtShdW = 1.0f;
            this._txtOutln = false;
            this._txtOutlnC = Color.Empty;
            this._txtOutlnW = 2;
            this._txtOutlnO = 255;
            this._rotTxtDeg = 0;
            this._rotBgImgDeg = 0;
            this._disFcolor = Color.Gray;
            this._disBcolor = Color.WhiteSmoke;
            this._rndCrnr = true;
            this._tglClr = Color.Empty;
            this._fgReserved = 0.0f;
        }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        private void SetButtonRegion()
        {
            int iBorderHalf = this._brdWidth / 2;
            if (this._rndCrnr)
            {
                FeatheredRectangle fRegion = new FeatheredRectangle(iBorderHalf, iBorderHalf, this.ClientRectangle.Width - (iBorderHalf * 2), this.ClientRectangle.Height - (iBorderHalf * 2), this._fthr);
                this._btnShape = fRegion.GetRegion();
                fRegion = FeatheredRectangle.Empty;
            }
            else
            {
                int t = iBorderHalf,
                    l = iBorderHalf,
                    b = this.ClientRectangle.Height - (iBorderHalf * 2),
                    r = this.ClientRectangle.Width - (iBorderHalf * 2);
                Point[] pts = new Point[] { 
                        new Point(l, t + this._fthr), 
                        new Point(l + this._fthr, t), 
                        new Point(r - this._fthr, t), 
                        new Point(r, t + this._fthr), 
                        new Point(r, b - this._fthr), 
                        new Point(r - this._fthr, b), 
                        new Point(l + this._fthr, b), 
                        new Point(l, b - this._fthr) };
                byte[] bts = new byte[] { 
                        (byte)PathPointType.Start, 
                        (byte)PathPointType.Line,
                        (byte)PathPointType.Line,
                        (byte)PathPointType.Line,
                        (byte)PathPointType.Line,
                        (byte)PathPointType.Line,
                        (byte)PathPointType.Line,
                        (byte)PathPointType.CloseSubpath };
                this._btnShape = new GraphicsPath(pts, bts);
            }
        }
        #endregion

        #region Base-Class Overrides
        //***************************************************************************
        // Base-Class Overrides
        // 
        protected override void OnParentBackColorChanged(EventArgs e)
        {
            this.Refresh();
            base.OnParentBackColorChanged(e);
        }
        protected override void OnPaint(PaintEventArgs pevent)
        {
            Graphics g = pevent.Graphics;
            g.Clear(this.Parent.BackColor);
            int iFeatherSq = this._fthr * 2;
            int iBorderHalf = this._brdWidth / 2;
            PointF pCenter = new PointF(this.ClientRectangle.Right - (this.ClientRectangle.Width / 2), this.ClientRectangle.Bottom - (this.ClientRectangle.Height / 2));

            // Set the button rotation transform.
            using (Matrix gMat = new Matrix())
            {
                #region Set the Blending Mode
                //-----------------------------------------------------------------------
                if (this._mSamp)
                {
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.CompositingMode = CompositingMode.SourceOver;
                    g.CompositingQuality = CompositingQuality.HighQuality;
                    g.InterpolationMode = InterpolationMode.Bicubic;
                }
                //-----------------------------------------------------------------------
                #endregion

                #region Set the clipping bounds for drawing
                //-----------------------------------------------------------------------
                if (this._fthr > 0 || this._btnShape != null)
                {
                    if (this._btnShape == null)
                        SetButtonRegion();
                    g.SetClip(this._btnShape, CombineMode.Replace);
                }
                //-----------------------------------------------------------------------
                #endregion

                #region Draw the background based on button style/mouse status
                //-----------------------------------------------------------------------
                // Determine the background rectangle.
                RectangleF rBndsF = (this._btnShape != null)
                        ? this._btnShape.GetBounds()
                        : new RectangleF(
                            new PointF((float)this.ClientRectangle.Left, (float)this.ClientRectangle.Top),
                            new SizeF((float)this.ClientRectangle.Width, (float)this.ClientRectangle.Height));

                // We need to clear the button with the background color before we go any further.
                g.Clear((this.Enabled) 
                        ? ((this.ToggleType && this.Activated && this.ToggleActiveColor != Color.Empty) 
                                ? this.ToggleActiveColor 
                                : this.BackColor) 
                        : this._disBcolor);

                Image bgImg = null;
                if (this.Enabled)
                {
                    // Determine the appropriate background image
                    if (this._btnType == AdvButtonStyle.Image && this._imgDown != null && (this._msDown || (this._toggleType && this._trigd)))
                        bgImg = (this._imgDown.Clone() as Image);
                    else if (this._btnType == AdvButtonStyle.Image && this._imgHover != null && this._msOver)
                        bgImg = (this._imgHover.Clone() as Image);
                    else if (this.BackgroundImage != null)
                        bgImg = (this.BackgroundImage.Clone() as Image);


                    if (bgImg != null)
                    {
                        gMat.RotateAt(this._rotBgImgDeg, pCenter);
                        g.Transform = gMat;

                        // We need to draw the image before drawing the gradient.
                        switch (this.BackgroundImageLayout)
                        {
                            case ImageLayout.Center:
                                g.DrawImageUnscaled(bgImg, (int)((rBndsF.Width / 2) - (bgImg.Width / 2)), (int)((rBndsF.Height / 2) - (bgImg.Height / 2)));
                                break;
                            case ImageLayout.None:
                                g.DrawImageUnscaled(bgImg, (int)rBndsF.X, (int)rBndsF.Y);
                                break;
                            case ImageLayout.Stretch:
                                g.DrawImage(bgImg, rBndsF);
                                break;
                            case ImageLayout.Tile:
                                using (TextureBrush iBrush = new TextureBrush(bgImg, WrapMode.Tile, new Rectangle(0, 0, bgImg.Width, bgImg.Height)))
                                    g.FillRectangle(iBrush, rBndsF);
                                break;
                            case ImageLayout.Zoom:
                                float iW = 0, iH = 0;
                                if (bgImg.Width > rBndsF.Width && bgImg.Height > rBndsF.Height)
                                {
                                    // The picture is larger than the bounding rectangle
                                    //   in both width and height, so we have to determine
                                    //   how to shrink it.
                                }
                                else if (bgImg.Width > rBndsF.Width)
                                {
                                    // The height is fine, but it's too wide.
                                    iW = rBndsF.Width;
                                    iH = (rBndsF.Width * bgImg.Height) / bgImg.Width;
                                }
                                else if (bgImg.Height > rBndsF.Height)
                                {
                                    // The width is fine, but it's too tall.
                                    iW = (rBndsF.Height * bgImg.Width) / bgImg.Height;

                                }
                                else
                                {
                                    // If we get here, it means the image is smaller than
                                    //   the bound rectangle, so we get to enlarge it.
                                    // First, we have to figure out whether width or
                                    //   height gets precidence over the final size.
                                }
                                break;
                        }
                        gMat.Reset();
                        g.Transform = gMat;
                    }

                    // Fill the background based on the button style.
                    switch (this._btnType)
                    {
                        case AdvButtonStyle.Flat:
                            // Fill the new button drawing region with the specified backcolor.
                            if ((this._msDown || (this._toggleType && this._trigd)) && bgImg == null)
                                g.Clear((this.FlatAppearance.MouseDownBackColor != Color.Empty) ? this.FlatAppearance.MouseDownBackColor : this.BackColor);
                            else if (this._msOver && bgImg == null)
                                g.Clear((this.FlatAppearance.MouseOverBackColor != Color.Empty) ? this.FlatAppearance.MouseOverBackColor : this.BackColor);
                            //else if (bgImg == null)
                            //    g.Clear(this.BackColor);

                            break;
                        case AdvButtonStyle.Popup:
                            //if (bgImg == null)
                            //    g.Clear(this.BackColor);
                            if (bgImg == null && (this._toggleType && this._trigd))
                                g.Clear(ControlPaint.Light(this.BackColor));
                            break;
                        case AdvButtonStyle.Standard:
                            //if (bgImg == null)
                            //    g.Clear(this.BackColor);
                            Color hColor = Color.Empty, sColor = Color.Empty, bColor = Color.Empty;
                            // Get the base background color
                            Color baseClr = this.BackColor;

                            // If this is s "Toggle" button and a different BG color is specified and the button
                            //   is currently triggered, then set the background color to the ActiveToggleColor.
                            if (this._toggleType && this._tglClr != Color.Empty && this._trigd)
                                baseClr = this._tglClr;

                            // If the background image is null, we're drawing the background in color.
                            // Otherwise, we use black and white to overlay on top of the image.
                            if (bgImg == null)
                            {
                                hColor = Color.FromArgb(System.Math.Min(baseClr.R + (int)((float)this._3dHeight * this._hlMul), 255), System.Math.Min(baseClr.G + (int)((float)this._3dHeight * this._hlMul), 255), System.Math.Min(baseClr.B + (int)((float)this._3dHeight * this._hlMul), 255));
                                sColor = Color.FromArgb(System.Math.Max(baseClr.R - this._3dHeight, 0), System.Math.Max(baseClr.G - this._3dHeight, 0), System.Math.Max(baseClr.B - this._3dHeight, 0));
                                bColor = this.BackColor;
                            }
                            else
                            {
                                hColor = Color.FromArgb(this._3dHeight, Color.White);
                                sColor = Color.FromArgb(this._3dHeight, Color.Black);
                                bColor = Color.Transparent;
                            }
                            ColorBlend blend = new ColorBlend(4);
                            if (this._msDown || (this._toggleType && this._trigd))
                                blend.Colors = new Color[] { sColor, sColor, bColor, hColor };
                            else
                                blend.Colors = new Color[] { hColor, hColor, bColor, sColor };
                            blend.Positions = new float[] { 0.0f, ((this._hlMul / 2) / 10), 0.5f, 1.0f };

                            // File the rectangle with the gradient.
                            if (rBndsF.Width > 0 && rBndsF.Height > 0)
                                using (LinearGradientBrush backBrush = new LinearGradientBrush(rBndsF, bColor, bColor, LinearGradientMode.Vertical))
                                {
                                    backBrush.InterpolationColors = blend;
                                    g.FillRectangle(backBrush, rBndsF);
                                }
                            break;
                        case AdvButtonStyle.Beveled:
                            //if (bgImg == null)
                            //    g.Clear(this.BackColor);
                            break;
                        case AdvButtonStyle.Image:
                            if ((this._msDown || (this._toggleType && this._trigd)) && bgImg == null)
                                g.Clear((this.FlatAppearance.MouseDownBackColor != Color.Empty) ? this.FlatAppearance.MouseDownBackColor : this.BackColor);
                            else if (this._msOver && bgImg == null)
                                g.Clear((this.FlatAppearance.MouseOverBackColor != Color.Empty) ? this.FlatAppearance.MouseOverBackColor : this.BackColor);
                            break;
                    }
                }
                if (bgImg != null)
                    bgImg.Dispose();
                //-----------------------------------------------------------------------
                #endregion

                #region Draw the text/image on the button.
                //-----------------------------------------------------------------------
                if (this._btnType != AdvButtonStyle.Image)
                {
                    #region Draw Text-Based Button
                    if (this._msOver && !this._msDown && this._btnType == AdvButtonStyle.Popup)
                        gMat.Translate(-1.0f, -0.5f);
                    else if ((this._msDown || (this._toggleType && this._trigd)) && (this._btnType == AdvButtonStyle.Popup || this._btnType == AdvButtonStyle.Beveled))
                        gMat.Translate(1.0f, 0.5f);
                    gMat.RotateAt(this._rotTxtDeg, pCenter);
                    g.Transform = gMat;

                    using (SolidBrush tBrush = new SolidBrush((this.Enabled) ? this.ForeColor : this._disFcolor))
                    using (StringFormat format = new StringFormat())
                    {
                        format.Trimming = ((this.AutoEllipsis) ? StringTrimming.EllipsisCharacter : StringTrimming.Character);
                        if (this.RightToLeft == RightToLeft.Yes)
                            format.FormatFlags |= StringFormatFlags.DirectionRightToLeft;
                        if (!this._txtWrp)
                            format.FormatFlags |= StringFormatFlags.NoWrap;
                        if (this._txtVert)
                            format.FormatFlags |= StringFormatFlags.DirectionVertical;

                        #region Set the string alignment based on the TextAlign property.
                        //---------------------------------------------------------------
                        switch (this.TextAlign)
                        {
                            case ContentAlignment.BottomCenter:
                                format.LineAlignment = StringAlignment.Far;
                                format.Alignment = StringAlignment.Center;
                                break;
                            case ContentAlignment.BottomLeft:
                                format.LineAlignment = StringAlignment.Far;
                                format.Alignment = StringAlignment.Near;
                                break;
                            case ContentAlignment.BottomRight:
                                format.LineAlignment = StringAlignment.Far;
                                format.Alignment = StringAlignment.Far;
                                break;
                            case ContentAlignment.MiddleCenter:
                                format.LineAlignment = StringAlignment.Center;
                                format.Alignment = StringAlignment.Center;
                                break;
                            case ContentAlignment.MiddleLeft:
                                format.LineAlignment = StringAlignment.Center;
                                format.Alignment = StringAlignment.Near;
                                break;
                            case ContentAlignment.MiddleRight:
                                format.LineAlignment = StringAlignment.Center;
                                format.Alignment = StringAlignment.Far;
                                break;
                            case ContentAlignment.TopCenter:
                                format.LineAlignment = StringAlignment.Near;
                                format.Alignment = StringAlignment.Center;
                                break;
                            case ContentAlignment.TopLeft:
                                format.LineAlignment = StringAlignment.Near;
                                format.Alignment = StringAlignment.Near;
                                break;
                            case ContentAlignment.TopRight:
                                format.LineAlignment = StringAlignment.Near;
                                format.Alignment = StringAlignment.Far;
                                break;
                        }
                        //---------------------------------------------------------------
                        #endregion

                        // Calculate the viable text region.
                        // This will be altered to adjust for a forground image,
                        //   if one has been specified.
                        RectangleF txtBnds = RectangleF.Empty;
                        float rX = (this._fthr / 2) + iBorderHalf;
                        float rY = (this._fthr / 2) + iBorderHalf;
                        float rW = this.ClientRectangle.Width - (rX * 2) - this._fgReserved;
                        float rH = this.ClientRectangle.Height - (rY * 2);

                        #region Offset text and draw image, if provided
                        //---------------------------------------------------------------
                        if (this.Image != null)
                        {
                            //ImageAttributes imgAttr = new ImageAttributes();
                            // Copy the correct Image object into the curImg variable.
                            // This way we have a singular object to reference in the
                            //   following code.
                            Bitmap curImg;
                            if (this._imgDown != null && (this._msDown || (this._toggleType && this._trigd)))
                                curImg = (this._imgDown as Bitmap);
                            else if (this._imgHover != null && this._msOver)
                                curImg = (this._imgHover as Bitmap);
                            else
                                curImg = (this.Image as Bitmap);


                            if (curImg != null)
                            {
                                if (!this.Enabled)
                                {
                                    try
                                    {
                                        curImg = BitmapFilter.GrayScale(curImg);
                                        curImg.MakeTransparent();
                                    }
                                    catch { }
                                }

                                float iPicX = 0, iPicY = 0;
                                if (!string.IsNullOrEmpty(this.Text))
                                {
                                    // Calcuate the image offset for the current button state.
                                    SizeF txtSize = g.MeasureString(this.Text, this.Font, new SizeF(rW, rH), format);
                                    switch (this.TextImageRelation)
                                    {
                                        case TextImageRelation.ImageAboveText:
                                            #region Draw image above text position
                                            //-----------------------------------------------
                                            // Vertical Allignment
                                            if (this.TextAlign == ContentAlignment.BottomCenter || this.TextAlign == ContentAlignment.BottomLeft || this.TextAlign == ContentAlignment.BottomRight)
                                            {   // Bottom
                                                iPicY = (rH - (curImg.Height + txtSize.Height + this._imgPad)) + rY;
                                            }
                                            else if (this.TextAlign == ContentAlignment.TopCenter || this.TextAlign == ContentAlignment.TopLeft || this.TextAlign == ContentAlignment.TopRight)
                                            {   // Top
                                                iPicY = rY + this._imgPad;
                                                rY += curImg.Height + (this._imgPad * 2);
                                            }
                                            else
                                            {   // Middle
                                                iPicY = ((rH / 2) - ((curImg.Height + this._imgPad + txtSize.Height) / 2)) + rY;
                                                rY += (curImg.Height + this._imgPad) / 2;
                                            }
                                            // Horizontal Allignment
                                            if (this.TextAlign == ContentAlignment.BottomLeft || this.TextAlign == ContentAlignment.MiddleLeft || this.TextAlign == ContentAlignment.TopLeft)
                                            {   // Left
                                                iPicX = rX + ((txtSize.Width / 2) - (curImg.Width / 2));
                                            }
                                            else if (this.TextAlign == ContentAlignment.BottomRight || this.TextAlign == ContentAlignment.MiddleRight || this.TextAlign == ContentAlignment.TopRight)
                                            {   // Right
                                                iPicX = ((rW - txtSize.Width) + ((txtSize.Width / 2) - (curImg.Width / 2))) + rX;
                                            }
                                            else
                                            {   // Center
                                                iPicX = ((rW / 2) - (curImg.Width / 2)) + rX;
                                            }
                                            break;
                                        //-----------------------------------------------
                                            #endregion
                                        case TextImageRelation.ImageBeforeText:
                                            #region Draw image to the left of the text
                                            //-----------------------------------------------
                                            // Vertical Allignment
                                            if (this.TextAlign == ContentAlignment.BottomCenter || this.TextAlign == ContentAlignment.BottomLeft || this.TextAlign == ContentAlignment.BottomRight)
                                            {   // Bottom
                                                iPicY = (rH - curImg.Height) + rY;
                                                rH -= System.Math.Max((curImg.Height - txtSize.Height), (txtSize.Height - curImg.Height)) / 2;
                                            }
                                            else if (this.TextAlign == ContentAlignment.TopCenter || this.TextAlign == ContentAlignment.TopLeft || this.TextAlign == ContentAlignment.TopRight)
                                            {   // Top
                                                iPicY = rY + this._imgPad;
                                                rY += System.Math.Max((curImg.Height - txtSize.Height), (txtSize.Height - curImg.Height)) / 2;
                                            }
                                            else
                                            {   // Middle
                                                iPicY = ((rH / 2) - (curImg.Height / 2)) + rY;
                                            }
                                            // Horizontal Allignment
                                            if (this.TextAlign == ContentAlignment.BottomLeft || this.TextAlign == ContentAlignment.MiddleLeft || this.TextAlign == ContentAlignment.TopLeft)
                                            {   // Left
                                                iPicX = rX + this._imgPad;
                                                rX += curImg.Width + (this._imgPad * 2);
                                            }
                                            else if (this.TextAlign == ContentAlignment.BottomRight || this.TextAlign == ContentAlignment.MiddleRight || this.TextAlign == ContentAlignment.TopRight)
                                            {   // Rigtht
                                                iPicX = (rW - (txtSize.Width + curImg.Width + this._imgPad)) + rX;
                                            }
                                            else
                                            {   // Center
                                                iPicX = ((rW / 2) - ((curImg.Width + txtSize.Width + _imgPad) / 2)) + rX;
                                                rX += (curImg.Width + this._imgPad) / 2;
                                            }
                                            break;
                                        //-----------------------------------------------
                                            #endregion
                                        case TextImageRelation.Overlay:
                                            #region Draw image in the same position as the text
                                            //-----------------------------------------------
                                            // Vertical Allignment
                                            if (this.TextAlign == ContentAlignment.BottomCenter || this.TextAlign == ContentAlignment.BottomLeft || this.TextAlign == ContentAlignment.BottomRight)
                                            {   // Bottom
                                                iPicY = (rH - rY) - (curImg.Height);
                                            }
                                            else if (this.TextAlign == ContentAlignment.TopCenter || this.TextAlign == ContentAlignment.TopLeft || this.TextAlign == ContentAlignment.TopRight)
                                            {   // Top
                                                iPicY = rY;
                                            }
                                            else
                                            {   // Middle
                                                iPicY = ((rH / 2) - (curImg.Height / 2)) + rY;
                                            }
                                            // Horizontal Allignment
                                            if (this.TextAlign == ContentAlignment.BottomLeft || this.TextAlign == ContentAlignment.MiddleLeft || this.TextAlign == ContentAlignment.TopLeft)
                                            {   // Left
                                                iPicX = rX;
                                            }
                                            else if (this.TextAlign == ContentAlignment.BottomRight || this.TextAlign == ContentAlignment.MiddleRight || this.TextAlign == ContentAlignment.TopRight)
                                            {   // Right
                                                iPicX = (rW - rX) - (curImg.Width);
                                            }
                                            else
                                            {   // Center
                                                iPicX = ((rW / 2) - (curImg.Width / 2)) + rX;
                                            }
                                            break;
                                        //-----------------------------------------------
                                            #endregion
                                        case TextImageRelation.TextAboveImage:
                                            #region Draw image below text position
                                            //-----------------------------------------------
                                            // Vertical Allignment
                                            if (this.TextAlign == ContentAlignment.BottomCenter || this.TextAlign == ContentAlignment.BottomLeft || this.TextAlign == ContentAlignment.BottomRight)
                                            {   // Bottom
                                                iPicY = (rH - curImg.Height) + rY;
                                                rH -= (curImg.Height + this._imgPad);
                                            }
                                            else if (this.TextAlign == ContentAlignment.TopCenter || this.TextAlign == ContentAlignment.TopLeft || this.TextAlign == ContentAlignment.TopRight)
                                            {   // Top
                                                iPicY = (txtSize.Height + this._imgPad) + rY;
                                            }
                                            else
                                            {   // Middle
                                                iPicY = (((rH / 2) - ((curImg.Height + this._imgPad + txtSize.Height) / 2)) + (txtSize.Height + this._imgPad)) + rY;
                                                rH -= (curImg.Height + this._imgPad);
                                            }
                                            // Horizontal Allignment
                                            if (this.TextAlign == ContentAlignment.BottomLeft || this.TextAlign == ContentAlignment.MiddleLeft || this.TextAlign == ContentAlignment.TopLeft)
                                            {   // Left
                                                iPicX = rX + ((txtSize.Width / 2) - (curImg.Width / 2));
                                            }
                                            else if (this.TextAlign == ContentAlignment.BottomRight || this.TextAlign == ContentAlignment.MiddleRight || this.TextAlign == ContentAlignment.TopRight)
                                            {   // Right
                                                iPicX = ((rW - txtSize.Width) + ((txtSize.Width / 2) - (curImg.Width / 2))) + rX;
                                            }
                                            else
                                            {   // Center
                                                iPicX = ((rW / 2) - (curImg.Width / 2)) + rX;
                                            }
                                            break;
                                        //-----------------------------------------------
                                            #endregion
                                        case TextImageRelation.TextBeforeImage:
                                            #region Draw image to the right of the text
                                            //-----------------------------------------------
                                            // Vertical Allignment
                                            if (this.TextAlign == ContentAlignment.BottomCenter || this.TextAlign == ContentAlignment.BottomLeft || this.TextAlign == ContentAlignment.BottomRight)
                                            {   // Bottom
                                                iPicY = (rH - curImg.Height) + rY;
                                                rH -= System.Math.Max((curImg.Height - txtSize.Height), (txtSize.Height - curImg.Height)) / 2;
                                            }
                                            else if (this.TextAlign == ContentAlignment.TopCenter || this.TextAlign == ContentAlignment.TopLeft || this.TextAlign == ContentAlignment.TopRight)
                                            {   // Top
                                                iPicY = rY + this._imgPad;
                                                rY += System.Math.Max((curImg.Height - txtSize.Height), (txtSize.Height - curImg.Height)) / 2;
                                            }
                                            else
                                            {   // Middle
                                                iPicY = ((rH / 2) - (curImg.Height / 2)) + rY;
                                            }
                                            // Horizontal Allignment
                                            if (this.TextAlign == ContentAlignment.BottomLeft || this.TextAlign == ContentAlignment.MiddleLeft || this.TextAlign == ContentAlignment.TopLeft)
                                            {   // Left
                                                iPicX = rX + this._imgPad + txtSize.Width;
                                            }
                                            else if (this.TextAlign == ContentAlignment.BottomRight || this.TextAlign == ContentAlignment.MiddleRight || this.TextAlign == ContentAlignment.TopRight)
                                            {   // Right
                                                iPicX = (rW - (curImg.Width + this._imgPad)) + rX;
                                                rW -= (curImg.Width + (this._imgPad * 2));
                                            }
                                            else
                                            {   // Center
                                                iPicX = ((rW / 2) + (((txtSize.Width / 2) + (curImg.Width / 2) + this._imgPad) / 2)) + rX;
                                                rW -= (curImg.Width + (this._imgPad * 2));
                                            }
                                            break;
                                        //-----------------------------------------------
                                            #endregion
                                    }
                                }
                                else
                                {
                                    GraphicsUnit pgUnit = g.PageUnit;
                                    RectangleF rect = curImg.GetBounds(ref pgUnit);
                                    iPicX = (rW / 2) - (float)(rect.Width / 2);
                                    iPicY = (rH / 2) - (float)(rect.Height / 2);
                                }
                                //g.DrawImage(curImg, new Rectangle((int)iPicX, (int)iPicY, curImg.Width, curImg.Height), 0, 0, curImg.Width, curImg.Height, GraphicsUnit.Pixel, imgAttr);
                                g.DrawImageUnscaled(curImg, (int)iPicX, (int)iPicY);
                            }
                            curImg.Dispose();
                        }
                        //---------------------------------------------------------------
                        #endregion

                        txtBnds = new RectangleF(rX, rY, rW, rH);

                        if (this.Enabled)
                        {
                            // Draw the text outline, if it's turned on.
                            if (this._txtOutln && this._txtOutlnC != Color.Empty)
                                using (GraphicsPath gpTextOutln = new GraphicsPath(FillMode.Alternate))
                                {
                                    gpTextOutln.AddString(this.Text, this.Font.FontFamily, (int)this.Font.Style, this.Font.Size * 1.318f, txtBnds, format);
                                    gpTextOutln.CloseAllFigures();
                                    gpTextOutln.Flatten();
                                    using (Pen tOutlnPen = new Pen(Color.FromArgb(this._txtOutlnO, this._txtOutlnC), this._txtOutlnW))
                                        g.DrawPath(tOutlnPen, gpTextOutln);
                                }

                            // Draw the text shadow, if it's turned on.
                            if (this._txtShd)
                                using (GraphicsPath gpTextShadow = new GraphicsPath(FillMode.Winding))
                                {
                                    gpTextShadow.AddString(this.Text, this.Font.FontFamily, (int)this.Font.Style, this.Font.Size * 1.318f, txtBnds, format);
                                    gpTextShadow.CloseAllFigures();
                                    gpTextShadow.Flatten();
                                    using (Matrix matTrans = new Matrix())
                                    {
                                        matTrans.Translate(this._txtShdW, this._txtShdW);
                                        gpTextShadow.Transform(matTrans);
                                        using (SolidBrush tShadowBrush = new SolidBrush(Color.FromArgb(this._txtShdO, Color.Black)))
                                            g.FillPath(tShadowBrush, gpTextShadow);
                                    }
                                }
                        }
                        {
                            //txtBnds.Offset(0.5f * (float)this._txtShdW, 1.0f * (float)this._txtShdW);
                            //using (SolidBrush tsBrush = new SolidBrush(Color.FromArgb(this._txtShdO, Color.Black)))
                            //    g.DrawString(this.Text, this.Font, tsBrush, txtBnds, format);
                            //txtBnds.Offset(0.5f * (float)this._txtShdW, 1.0f * (float)this._txtShdW);
                        }

                        // Finally, draw the actual text into the bounding rectangle.
                        if (!string.IsNullOrEmpty(this.Text))
                            g.DrawString(this.Text, this.Font, tBrush, txtBnds, format);
                        txtBnds = RectangleF.Empty;
                    }
                    gMat.Reset();
                    g.Transform = gMat;
                    #endregion
                }
                else
                {
                    #region Draw Image-Based Button
                    // Determine which image we're going to draw based on the button state.
                    Bitmap curImg = null;
                    if (this._msDown || (this._toggleType && this._trigd))
                    {
                        if (this._imgDown != null)
                            curImg = (this._imgDown as Bitmap);
                        else if (this._imgHover != null)
                            curImg = (this._imgHover as Bitmap);
                        else
                            curImg = (this.Image as Bitmap);
                    }
                    else if (this._msOver && this._imgHover != null)
                        curImg = (this._imgHover as Bitmap);
                    else
                        curImg = (this.Image as Bitmap);

                    // Now draw the actuall image.
                    if (curImg != null)
                    {
                        // If the button is disabled, we want to 'dim' the image.
                        if (!this.Enabled)
                        {
                            curImg = BitmapFilter.GrayScale(curImg);
                            curImg.MakeTransparent();
                        }

                        ImageAttributes imgAttr = new ImageAttributes();
                        imgAttr.SetWrapMode(WrapMode.Clamp);
                        g.DrawImage(curImg, new Rectangle(Imaging.CenterImage(curImg, Rectangle.Truncate(g.ClipBounds)), Size.Truncate(g.ClipBounds.Size)), 0, 0, curImg.Width, curImg.Height, GraphicsUnit.Pixel, imgAttr);
                    }
                    #endregion
                }
                //-----------------------------------------------------------------------
                #endregion

                #region Restore the clipping rectangle and draw the border.
                //-----------------------------------------------------------------------
                if (this.FlatStyle != AdvButtonStyle.Image)
                {
                    float brdrWidth = (float)this._brdWidth;

                    // If this button is the Form's default, we want to make the border "bold",
                    //   but before we restore the clipping region.
                    #region Draw "Default" Button Border
                    if (this.IsDefault && !(this._msOver || this._msDown))
                    {
                        Color clrBldBorder = Color.FromArgb(160, this._brdClr);
                        FeatheredRectangle rBldBorderPath = new FeatheredRectangle(iBorderHalf, iBorderHalf, this.ClientRectangle.Width - (iBorderHalf * 2), this.ClientRectangle.Height - (iBorderHalf * 2), this._fthr);
                        using (Pen pBldBorder = new Pen(clrBldBorder, brdrWidth + 2))
                        {
                            if (this._fthr > 0)
                            {
                                GraphicsPath grphPathBoldBorder = null;
                                try
                                {
                                    if (this._rndCrnr)
                                    {
                                        // Calculate the shape of the border
                                        FeatheredRectangle rBorderPath = new FeatheredRectangle(iBorderHalf, iBorderHalf, this.ClientRectangle.Width - (iBorderHalf * 2), this.ClientRectangle.Height - (iBorderHalf * 2), this._fthr);
                                        grphPathBoldBorder = rBorderPath.GetBorder();
                                    }
                                    else
                                    {
                                        grphPathBoldBorder = this._btnShape;
                                    }

                                    // Draw the "Bold" border.
                                    g.DrawPath(pBldBorder, grphPathBoldBorder);
                                }
                                finally
                                {
                                    if (grphPathBoldBorder != null)
                                        grphPathBoldBorder.Dispose();
                                }
                            }
                            else
                            {
                                Rectangle rBorder = new Rectangle(iBorderHalf, iBorderHalf, this.ClientRectangle.Width - (iBorderHalf * 2), this.ClientRectangle.Height - (iBorderHalf * 2));
                                g.DrawRectangle(pBldBorder, rBorder);
                                rBorder = Rectangle.Empty;
                            }
                        }
                    }
                    #endregion

                    g.SetClip(pevent.ClipRectangle, CombineMode.Replace);

                    using (Pen pBorder = new Pen((this.Enabled) ? this._brdClr : this._disFcolor, brdrWidth))
                    {
                        // We have to process the border differently it the button's
                        //   corners are feathered.
                        if (this._fthr > 0)
                        {
                            GraphicsPath grphPath = null;
                            try
                            {
                                if (this._rndCrnr)
                                {
                                    // Calculate the shape of the border
                                    FeatheredRectangle rBorderPath = new FeatheredRectangle(iBorderHalf, iBorderHalf, this.ClientRectangle.Width - (iBorderHalf * 2), this.ClientRectangle.Height - (iBorderHalf * 2), this._fthr);
                                    grphPath = rBorderPath.GetBorder();
                                }
                                else
                                {
                                    grphPath = this._btnShape;
                                }

                                // Draw the hover/click border highlight
                                if ((this._msOver || this._msDown) && this._hvrOpc > 0)
                                    using (GraphicsPath gpHighlight = (grphPath.Clone() as GraphicsPath))
                                    {
                                        float offset = 3;
                                        Rectangle rBorder = Rectangle.Truncate(grphPath.GetBounds());
                                        using (Matrix resizeMatrix = new Matrix())
                                        {
                                            resizeMatrix.Scale((rBorder.Width - offset) / rBorder.Width, (rBorder.Height - offset) / rBorder.Height);
                                            resizeMatrix.Translate(((rBorder.Width - offset) / rBorder.Width) + (offset / 10), ((rBorder.Height - offset) / rBorder.Height) + (offset / 10));
                                            gpHighlight.Transform(resizeMatrix);
                                            using (Pen overPen = new Pen(((this._msDown) ? Color.White : Color.FromArgb(this._hvrOpc, this._hvrClr)), ((this._msDown) ? 1.0f : 2.0f)))
                                                g.DrawPath(overPen, gpHighlight);
                                        }
                                    }

                                // Finally, draw the actual border
                                g.DrawPath(pBorder, grphPath);

                                // For the 'popup' style, we need to make the button appear to 'popup'
                                //   if the mouse is hovering over it.
                                // For the 'beveled' style, the highlight/shadow edges should always be visible.
                                if ((this._btnType == AdvButtonStyle.Beveled && !this._msDown) || (this._btnType == AdvButtonStyle.Popup && this._msOver && !this._msDown))
                                {
                                    g.SetClip(this._btnShape, CombineMode.Replace);
                                    using (GraphicsPath gpBtnHlt = (grphPath.Clone() as GraphicsPath))
                                    using (GraphicsPath gpBtnShw = (grphPath.Clone() as GraphicsPath))
                                    {
                                        using (Matrix resizeMatrix = new Matrix())
                                        {
                                            resizeMatrix.Translate(0.5f, 0.5f);
                                            gpBtnHlt.Transform(resizeMatrix);
                                            g.DrawPath(SystemPens.ButtonHighlight, gpBtnHlt);

                                            resizeMatrix.Reset();
                                            resizeMatrix.Translate(-0.5f, -0.5f);
                                            gpBtnShw.Transform(resizeMatrix);
                                            g.DrawPath(SystemPens.ButtonShadow, gpBtnShw);
                                        }
                                    }
                                }
                            }
                            finally
                            {
                                if (grphPath != null)
                                    grphPath.Dispose();
                            }
                        }
                        else
                        {
                            Rectangle rBorder = new Rectangle(iBorderHalf, iBorderHalf, this.ClientRectangle.Width - (iBorderHalf * 2), this.ClientRectangle.Height - (iBorderHalf * 2));
                            if (this._msOver || this._msDown)
                            {
                                Rectangle hoverEdge = new Rectangle(this._brdWidth, this._brdWidth, this.ClientRectangle.Width - (this._brdWidth * 3), this.ClientRectangle.Height - (this._brdWidth * 3));
                                using (Pen overPen = new Pen(((this._msDown) ? Color.White : Color.FromArgb(this._hvrOpc, this._hvrClr)), ((this._msDown) ? 1.0f : 2.0f)))
                                    g.DrawRectangle(overPen, hoverEdge);
                            }
                            g.DrawRectangle(pBorder, rBorder);
                            rBorder = Rectangle.Empty;
                        }
                    }
                }
                //-----------------------------------------------------------------------
                #endregion
            }
            g = null;
        }
        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            //base.OnPaintBackground(pevent);
        }
        protected override void OnClientSizeChanged(EventArgs e)
        {
            base.OnClientSizeChanged(e);
            if ((this._fthr) >= (this.ClientRectangle.Width - ((this._brdWidth / 2) * 2) - 1) / 2)
                this._fthr = (this.ClientRectangle.Width - ((this._brdWidth / 2) * 2) - 1) / 2;
            else if ((this._fthr) >= (this.ClientRectangle.Height - ((this._brdWidth / 2) * 2) - 1) / 2)
                this._fthr = (this.ClientRectangle.Height - ((this._brdWidth / 2) * 2) - 1) / 2;

            // If the button is feathered, we have to calcuate the drawing region.
            //if (this._fthr > 0)
            //    SetButtonRegion();
            if (this._btnShape != null)
                this._btnShape.Dispose();
            this._btnShape = null;
            this.Refresh();
        }
        protected override void OnMouseEnter(EventArgs eventargs)
        {
            if (this.DesignMode)
                return;

            if (this._btnShape == null || this._btnShape.IsVisible(PointToClient(Control.MousePosition)))
                this._msOver = true;
            else if (this._btnShape.IsVisible(PointToClient(Control.MousePosition)))
                this._msOver = true;
            base.OnMouseEnter(eventargs);
        }
        protected override void OnMouseMove(MouseEventArgs mevent)
        {
            // Determine if we're *really* over the button and set the value.
            if (this._btnShape == null)
                this._msOver = true;
            //this._state = PushButtonState.Hot;
            else
            {
                bool was = this._msOver;
                if (this._btnShape.IsVisible(PointToClient(Control.MousePosition)))
                    this._msOver = true;
                else
                    this._msOver = false;

                // If the value changed, we need to redraw;
                if (was != this._msOver)
                    this.Refresh();
            }

            // Call the base-class method.
            base.OnMouseMove(mevent);
        }
        protected override void OnMouseLeave(EventArgs eventargs)
        {
            this._msOver = false;
            base.OnMouseLeave(eventargs);
        }
        protected override void OnMouseDown(MouseEventArgs mevent)
        {
            if (this._btnShape == null)
                this._msDown = true;
            else if (this._btnShape.IsVisible(PointToClient(Control.MousePosition)))
                this._msDown = true;
            //if (this._dlgRslt != DialogResult.None)
            //{
            //    Control ctrlOwner = this.Parent;
            //    while (ctrlOwner.GetType() != typeof(System.Windows.Forms.Form))
            //    {
            //        ctrlOwner = ctrlOwner.Parent;
            //    }
            //    ((Form)ctrlOwner).DialogResult = this._dlgRslt;
            //}
            base.OnMouseDown(mevent);
        }
        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            this._msDown = false;
            if (this._toggleType && mevent.Button == MouseButtons.Left)
                this._trigd = !this._trigd;
            base.OnMouseUp(mevent);
            this.Refresh();
        }
        protected override void OnDragDrop(DragEventArgs drgevent)
        {
            base.OnDragDrop(drgevent);
            this._trigd = (!this._trigd);
        }
        protected override void OnDragEnter(DragEventArgs drgevent)
        {
            this._msOver = true;
            base.OnDragEnter(drgevent);
        }
        protected override void OnDragOver(DragEventArgs drgevent)
        {
            this._msDown = (Control.MouseButtons == MouseButtons.Left);
            base.OnDragOver(drgevent);
        }
        protected override void OnDragLeave(EventArgs e)
        {
            this._msDown = false;
            this._msOver = false;
            base.OnDragLeave(e);
            this.Invalidate();
        }
        #endregion
    }
}
