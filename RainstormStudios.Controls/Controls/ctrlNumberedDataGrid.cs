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
using System.Data;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Text;

namespace RainstormStudios.Controls
{
    [Author("Unfried, Michael")]
    [System.Drawing.ToolboxBitmap(typeof(System.Windows.Forms.DataGrid))]
    public class NumberedDataGrid : System.Windows.Forms.DataGrid
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private bool
            useLns = true;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        [Category("Appearance"),Description("Specifies whether or not to overlay the row numbers on top of the row headers."),Browsable(true),DefaultValue(true)]
        public bool ShowLineNumbers
        {
            get { return useLns; }
            set { useLns = value; }
        }
        #endregion

        #region Event Handlers
        //***************************************************************************
        // Event Overrides
        // 
        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            base.OnPaint(e);

            // If line numbers are turned off, then return.
            if (!useLns) return;

            try
            {
                // We only want to redraw the row numbers if the row headers are being repainted.
                if ((e.ClipRectangle.Right - e.ClipRectangle.Left) < this.RowHeaderWidth)
                {
                    // If we can't determine the top row number, then abort.
                    if (this.HitTest(new Point(this.RowHeaderWidth + 5, 25)).Row < 0)
                        return;
                    // If the DataGrid doesn't have any visible rows, then there's nothing for us to do here.
                    if (this.VisibleRowCount < 1)
                        return;

                    int rCount = -1;
                    if ((this.DataSource as DataTable).Rows.Count >= this.VisibleRowCount)
                        rCount = this.VisibleRowCount;
                    else
                        rCount = (this.DataSource as DataTable).Rows.Count;

                    for (int i = 0; i <= (rCount - 1); i++)
                    {
                        RectangleF drawBounds = new RectangleF(15, (i * (this.PreferredRowHeight + 1)) + 22, this.RowHeaderWidth, this.PreferredRowHeight);
                        using (Font drawFont = new Font("Tahoma", 11f, FontStyle.Regular, GraphicsUnit.Pixel))
                        {
                            using (SolidBrush brush = new SolidBrush(Color.Black))
                            {
                                int lineNum = this.HitTest(new Point(this.RowHeaderWidth + 5, 25)).Row + 1;
                                e.Graphics.DrawString(lineNum.ToString(), drawFont, brush, drawBounds);
                            }
                        }
                    }
                }
            }
            #region Build Specific Catch Handler
#if DEBUG
            catch (Exception ex)
            { Console.WriteLine(ex.ToString()); }
#else
            catch { }
#endif
            #endregion
        }
        #endregion
    }
}
