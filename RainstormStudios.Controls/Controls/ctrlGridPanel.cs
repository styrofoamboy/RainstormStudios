using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;

namespace RainstormStudios.Controls
{
    public class GridPanel : System.Windows.Forms.Panel
    {
        #region Nested Types
        //***************************************************************************
        // Nested Enums
        // 
        public enum GridType : uint
        {
            Grid = 0,
            Dots = 1//,
            //Checker = 2
        }
        #endregion

        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private int
            _spc = 10;
        private Color
            _grdClr = Color.Gray;
        private GridType
            _type = GridType.Grid;
        private int
            _opacity = 255;
        private Bitmap
            _bmpOverlay;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public GridType Mode
        {
            get { return this._type; }
            set
            {
                if (this._type != value)
                {
                    this._type = value;
                    if (this._bmpOverlay != null)
                        this._bmpOverlay.Dispose();
                    this._bmpOverlay = null;
                    this.Refresh();
                }
            }
        }
        public Color GridColor
        {
            get { return this._grdClr; }
            set
            {
                if (this._grdClr != value)
                {
                    this._grdClr = value;
                    if (this._bmpOverlay != null)
                        this._bmpOverlay.Dispose();
                    this._bmpOverlay = null;
                    this.Refresh();
                }
            }
        }
        public int GridSpacing
        {
            get { return this._spc; }
            set
            {
                if (this._spc != value)
                {
                    this._spc = value;
                    if (this._bmpOverlay != null)
                        this._bmpOverlay.Dispose();
                    this._bmpOverlay = null;
                    this.Refresh();
                }
            }
        }
        public int GridOpacity
        {
            get { return this._opacity; }
            set
            {
                if (this._opacity != value)
                {
                    if (value > 255 || value < 0)
                        throw new ArgumentOutOfRangeException();
                    this._opacity = value;
                    if (this._bmpOverlay != null)
                        this._bmpOverlay.Dispose();
                    this._bmpOverlay = null;
                    this.Refresh();
                }
            }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public GridPanel()
            : base()
        {
            this.DoubleBuffered = true;
        }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Base Overrides
        // 
        protected override void OnResize(EventArgs eventargs)
        {
            this._bmpOverlay = null;
            base.OnResize(eventargs);
        }
        protected override void OnClientSizeChanged(EventArgs e)
        {
            this._bmpOverlay = null;
            base.OnClientSizeChanged(e);
        }
        protected sealed override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            base.OnPaint(e);

            if (this._bmpOverlay == null)
            {
                this._bmpOverlay = new Bitmap(this.ClientRectangle.Width, this.ClientRectangle.Height);
                using (Graphics g = Graphics.FromImage(this._bmpOverlay))
                using (SolidBrush b = new SolidBrush(Color.FromArgb(this._opacity, this._grdClr)))
                {
                    g.Clear(Color.Transparent);
                    if (this.Mode == GridType.Dots)
                    {
                        for (int x = this.ClientRectangle.Left + (this._spc / 2); x < this.ClientRectangle.Right; x += this._spc)
                            for (int y = this.ClientRectangle.Top + (this._spc / 2); y < this.ClientRectangle.Bottom; y += this._spc)
                                if (x >= e.ClipRectangle.Left && x <= e.ClipRectangle.Right && y >= e.ClipRectangle.Top && y <= e.ClipRectangle.Bottom)
                                    g.FillEllipse(b, x - 0.5f, y - 0.5f, 1.5f, 1.5f);
                    }
                    else if (this.Mode == GridType.Grid)
                    {
                        using (Pen p = new Pen(b))
                        {
                            for (int x = this.ClientRectangle.Left + (this._spc - 1); x < this.ClientRectangle.Right; x += this._spc)
                                g.DrawLine(p, new Point(x, this.ClientRectangle.Top), new Point(x, this.ClientRectangle.Bottom));
                            for (int y = this.ClientRectangle.Top + (this._spc - 1); y < this.ClientRectangle.Bottom; y += this._spc)
                                g.DrawLine(p, new Point(this.ClientRectangle.Left, y), new Point(this.ClientRectangle.Right, y));
                        }
                    }
                    //else if (this.Mode == GridType.Checker)
                    //{
                    //    using(Pen p = new Pen(Brushes.Red))
                    //    for (int y = this.ClientRectangle.Top; y < this.ClientRectangle.Bottom; y += this._spc)
                    //        for (int x = this.ClientRectangle.Left; x < this.ClientRectangle.Right; x += this._spc)
                    //            if (((y / this._spc) % 2 == 0 && (x / this._spc) % 2 != 0) || ((y / this._spc) % 2 != 0 && (x / this._spc) % 2 == 0))
                    //                g.FillRectangle(b, new Rectangle(x, y, x + this._spc, y + this._spc));
                    //            else
                    //                g.DrawRectangle(p, new Rectangle(x, y, x + this._spc, y + this._spc));
                    //}
                }
            }

            e.Graphics.DrawImageUnscaled(_bmpOverlay, new Point(0, 0));
        }
        #endregion
    }
}
