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
using System.Drawing.Printing;
using System.Collections;
using System.ComponentModel;
using System.Text;

namespace RainstormStudios.Controls
{
    [Author("Unfried, Michael")]
    [System.Drawing.ToolboxBitmap(typeof(System.Windows.Forms.DataGridView))]
    public class NumberedDataGridView : System.Windows.Forms.DataGridView
    {
        #region Declarations
        //***************************************************************************
        // Global Variables
        // 
        private bool
            _useLns = true,
            _padNum = false,
            _zeroBase = false;
        private PrintDocument
            _printDoc;
        private int
            _printDocPg,
            _printNextRow,
            _printNextCol;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        [Category("Appearance"), Description("Specifies whether or not to overlay the row numbers on top of the row headers."), Browsable(true), DefaultValue(true)]
        public bool ShowLineNumbers
        {
            get { return this._useLns; }
            set { this._useLns = value; }
        }
        [Category("Appearance"), Description("Specifies whether or not line numbers should be padded with zeros so that all numbers are the same length."), Browsable(true), DefaultValue(false)]
        public bool PadLineNumbers
        {
            get { return this._padNum; }
            set { this._padNum = value; }
        }
        public bool ZeroBased
        {
            get { return this._zeroBase; }
            set { this._zeroBase = value; }
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public void MakeRowVisible(int rowIndex)
        {
            if (rowIndex > this.Rows.Count)
                throw new ArgumentOutOfRangeException("rowIndex", "The specified index must reference a row number less than or equal to the number of rows in the DataGridView.");

            this.FirstDisplayedScrollingRowIndex = rowIndex;
            //if (this.FirstDisplayedScrollingRowIndex > rowIndex)
            //    while (this.FirstDisplayedScrollingRowIndex > rowIndex)
            //        this.FirstDisplayedScrollingRowIndex--;
            //else
            //    while (this.FirstDisplayedScrollingRowIndex < rowIndex)
            //        this.FirstDisplayedScrollingRowIndex++;
        }
        public void MakeColumnVisible(int colIndex)
        {
            if (colIndex > this.Columns.Count)
                throw new ArgumentOutOfRangeException("colIndex", "The specified index must reference a column within the DataGridView");

            if (this.FirstDisplayedScrollingColumnIndex > colIndex)
                while (this.FirstDisplayedScrollingColumnIndex > colIndex)
                    this.FirstDisplayedScrollingColumnIndex--;
            else
                while (this.FirstDisplayedScrollingColumnIndex + this.ColumnCount < colIndex)
                    this.FirstDisplayedScrollingColumnIndex++;
        }
        public bool PrintSettings()
        {
            System.Windows.Forms.DialogResult dlgRslt = System.Windows.Forms.DialogResult.None;
            this.CreatePrintDocument();
            using (System.Windows.Forms.PrintDialog dlg = new System.Windows.Forms.PrintDialog())
            {
                dlg.Document = this._printDoc;
                dlg.AllowSomePages = false;
                dlg.AllowCurrentPage = false;
                dlgRslt = dlg.ShowDialog(this.FindForm());
                if (dlgRslt == System.Windows.Forms.DialogResult.OK)
                    this._printDoc.PrinterSettings = dlg.PrinterSettings;
            }
            return (dlgRslt == System.Windows.Forms.DialogResult.OK);
        }
        public void Print(string docName)
        {
            if (this.PrintSettings())
            {
                this._printDoc.DocumentName = docName;
                this._printDoc.Print();
            }
        }
        public void PrintPreview()
        {
            this.CreatePrintDocument();
            using (System.Windows.Forms.PrintPreviewDialog dlg = new System.Windows.Forms.PrintPreviewDialog())
            {
                dlg.Document = this._printDoc;
                dlg.ShowDialog(this.FindForm());
            }
        }
        public object GetClipboardContent(bool includeHeaders)
        {
            object cbData = base.GetClipboardContent();
            if (!includeHeaders)
                // If the user passed "false", there's nothing more to do here.
                return cbData;

            // If this isn't a DataObject, I don't know how to process it.
            System.Windows.Forms.DataObject dataObj = (cbData as System.Windows.Forms.DataObject);
            if (dataObj == null)
                return cbData;

            if (dataObj.ContainsText(System.Windows.Forms.TextDataFormat.CommaSeparatedValue))
            {
                string dataVal = (string)dataObj.GetData("Csv", true);
                StringBuilder sbCols = new StringBuilder();
                for (int i = 0; i < this.Columns.Count; i++)
                {
                    if (this.Columns[i].Displayed)
                        sbCols.AppendFormat(",{1}{0}{1}", this.Columns[i].HeaderText, this.Columns[i].HeaderText.ContainsAny(",", " ") ? "\"" : "");
                }
                sbCols.AppendFormat("\r\n{0}", dataVal);
                dataObj.SetData("Csv", sbCols.ToString().TrimStart(','));
            }

            if (dataObj.ContainsText(System.Windows.Forms.TextDataFormat.Text) || dataObj.ContainsText(System.Windows.Forms.TextDataFormat.UnicodeText))
            {
                string dataVal = (string)dataObj.GetData("Text", true);
                StringBuilder sbCols = new StringBuilder();
                for (int i = 0; i < this.Columns.Count; i++)
                {
                    if (this.Columns[i].Displayed)
                        sbCols.AppendFormat("\t{0}", this.Columns[i].HeaderText);
                }
                sbCols.AppendFormat("\r\n{0}", dataVal);

                if (dataObj.ContainsText(System.Windows.Forms.TextDataFormat.Text))
                    dataObj.SetData("Text", System.Text.Encoding.ASCII.GetString(System.Text.Encoding.ASCII.GetBytes(sbCols.ToString().TrimStart('\t').ToCharArray())));

                if (dataObj.ContainsText(System.Windows.Forms.TextDataFormat.UnicodeText))
                    dataObj.SetData("UnicodeText", System.Text.Encoding.Unicode.GetString(System.Text.Encoding.Unicode.GetBytes(sbCols.ToString().TrimStart('\t').ToCharArray())));
            }

            if (dataObj.ContainsText(System.Windows.Forms.TextDataFormat.Html))
            {

            }

            return dataObj;
        }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        private void CreatePrintDocument()
        {
            if (this._printDoc == null)
            {
                this._printDoc = new PrintDocument();
                this._printDoc.OriginAtMargins = false;
                this._printDoc.BeginPrint += new PrintEventHandler(this.printDocument_onBeginPrint);
                this._printDoc.PrintPage += new PrintPageEventHandler(this.printDocument_onPrintPage);
            }
        }
        #endregion

        #region Event Handlers
        //***************************************************************************
        // Event Handlers
        // 
        private void printDocument_onBeginPrint(object sender, PrintEventArgs e)
        {
            this._printDocPg = 0;
            this._printNextRow = 0;
            this._printNextCol = 0;
        }
        private void printDocument_onPrintPage(object sender, PrintPageEventArgs e)
        {
            // Determine the current page number.
            this._printDocPg++;
            int lX = 0, lY = 0;

            //// Determine if the page should be printed.
            //if (e.PageSettings.PrinterSettings.PrintRange == PrintRange.AllPages || (e.PageSettings.PrinterSettings.PrintRange == PrintRange.SomePages && this._printDocPg >= e.PageSettings.PrinterSettings.FromPage && this._printDocPg <= e.PageSettings.PrinterSettings.ToPage))
            //{ }

            // Draw the 'blank' cell in the top-left corner.
            RectangleF dedBnds = new RectangleF(
                e.MarginBounds.Left,
                e.MarginBounds.Top,
                this.RowHeadersWidth,
                this.ColumnHeadersHeight);
            e.Graphics.FillRectangle(Brushes.DarkGray,
                dedBnds.X, dedBnds.Y, dedBnds.Width, dedBnds.Height);
            e.Graphics.DrawRectangle(Pens.Black,
                dedBnds.X, dedBnds.Y, dedBnds.Width, dedBnds.Height);

            // Calculate where to start drawing cells based on headers and page margins.
            float curColOffset = (float)this.RowHeadersWidth + e.MarginBounds.Left,
                curRowOffset = (float)this.ColumnHeadersHeight + e.MarginBounds.Top;

            // Draw the column headers.
            using (StringFormat format = new StringFormat(StringFormatFlags.NoWrap | StringFormatFlags.FitBlackBox))
            {
                format.Alignment = StringAlignment.Near;
                format.LineAlignment = StringAlignment.Center;
                for (int i = this._printNextCol; i < this.ColumnCount; i++)
                {
                    if (e.Cancel) break;

                    RectangleF bnds = new RectangleF(curColOffset, e.MarginBounds.Top, this.Columns[i].Width, this.ColumnHeadersHeight);
                    if (bnds.Width + curColOffset > e.MarginBounds.Right)
                        break;

                    using (SolidBrush brush = new SolidBrush(e.Graphics.GetNearestColor(
                                (this.Columns[i].HeaderCell.Style.BackColor != Color.Empty)
                                        ? this.Columns[i].HeaderCell.Style.BackColor
                                        : this.ColumnHeadersDefaultCellStyle.BackColor)))
                        e.Graphics.FillRectangle(brush, bnds.X, bnds.Y, bnds.Width, bnds.Height);

                    RectangleF tBnds = new RectangleF(bnds.X + 1, bnds.Y + 1, bnds.Width - 2, bnds.Height - 2);
                    using (SolidBrush txtBrush = new SolidBrush(e.Graphics.GetNearestColor(
                                                                    (this.Columns[i].HeaderCell.Style.ForeColor != Color.Empty)
                                                                            ? this.Columns[i].HeaderCell.Style.ForeColor
                                                                            : this.ColumnHeadersDefaultCellStyle.ForeColor)))
                        e.Graphics.DrawString(this.Columns[i].HeaderText,
                                (this.Columns[i].HeaderCell.Style.Font != null)
                                        ? this.Columns[i].HeaderCell.Style.Font
                                        : this.ColumnHeadersDefaultCellStyle.Font,
                                txtBrush, tBnds, format);
                    e.Graphics.DrawRectangle(Pens.Black, bnds.X, bnds.Y, bnds.Width, bnds.Height);
                    curColOffset += this.Columns[i].Width;
                }
            }

            // Draw the actual cell data.
            curRowOffset = (float)this.ColumnHeadersHeight + e.MarginBounds.Top;
            using (Font rHdrFont = new Font("Tahoma", 11f, FontStyle.Regular, GraphicsUnit.Pixel))
            using (SolidBrush txtBrush = new SolidBrush(e.Graphics.GetNearestColor(this.ForeColor)))
            {
                using (StringFormat rowFormat = new StringFormat(StringFormatFlags.NoWrap))
                {
                    rowFormat.Alignment = StringAlignment.Far;
                    rowFormat.LineAlignment = StringAlignment.Far;
                    using (StringFormat colFormat = new StringFormat(StringFormatFlags.FitBlackBox | StringFormatFlags.NoWrap))
                    {
                        colFormat.Alignment = StringAlignment.Near;
                        colFormat.LineAlignment = StringAlignment.Far;
                        for (int r = this._printNextRow; r < this.RowCount; r++)
                        {
                            if (e.Cancel) break;

                            RectangleF rbnds = new RectangleF(e.MarginBounds.Left, curRowOffset, this.RowHeadersWidth, this.Rows[r].Height);
                            if (curRowOffset + rbnds.Height > e.MarginBounds.Bottom)
                            {
                                this._printNextRow = r;
                                break;
                            }

                            using (SolidBrush brush = new SolidBrush(e.Graphics.GetNearestColor(this.RowHeadersDefaultCellStyle.BackColor)))
                                e.Graphics.FillRectangle(brush, rbnds.X, rbnds.Y, rbnds.Width, rbnds.Height);

                            RectangleF tBnds = new RectangleF(rbnds.X + 1, rbnds.Y + 1, rbnds.Width - 2, rbnds.Height - 2);
                            e.Graphics.DrawString(Convert.ToString(r + 1), rHdrFont, txtBrush, tBnds, rowFormat);
                            e.Graphics.DrawRectangle(Pens.Black, rbnds.X, rbnds.Y, rbnds.Width, rbnds.Height);

                            curColOffset = (float)this.RowHeadersWidth + e.MarginBounds.Left;
                            for (int c = this._printNextCol; c < this.ColumnCount; c++)
                            {
                                RectangleF cbnds = new RectangleF(curColOffset, curRowOffset, this.Columns[c].Width, this.Rows[r].Height);
                                if (e.Cancel) break; ;

                                if (curColOffset + cbnds.Width > e.MarginBounds.Right)
                                {
                                    if (r == this.RowCount - 1)
                                    {
                                        this._printNextRow = 0;
                                        this._printNextCol = c;
                                    }
                                    break;
                                }

                                // Determine if this cell has a background color.
                                Color bkClr = Color.Empty;
                                if (this.Rows[r].Cells[c].Style.BackColor != Color.Empty)
                                    bkClr = this.Rows[r].Cells[c].Style.BackColor;
                                else if (this.Rows[r].DefaultCellStyle.BackColor != Color.Empty)
                                    bkClr = this.Rows[r].DefaultCellStyle.BackColor;

                                // Fill the cell's background color if not white or transparent.
                                if (bkClr != Color.Empty && bkClr != Color.White && bkClr != Color.Transparent)
                                    using (SolidBrush brush = new SolidBrush(e.Graphics.GetNearestColor(bkClr)))
                                        e.Graphics.FillRectangle(brush, cbnds.X, cbnds.Y, cbnds.Width, cbnds.Height);

                                // Draw the cell's value as a string.
                                RectangleF ctBnds = new RectangleF(cbnds.X + 1, cbnds.Y + 1, cbnds.Width - 2, cbnds.Height - 2);
                                e.Graphics.DrawString(this.Rows[r].Cells[c].Value.ToString(), this.Font, txtBrush, ctBnds, colFormat);
                                e.Graphics.DrawRectangle(Pens.Black, cbnds.X, cbnds.Y, cbnds.Width, cbnds.Height);
                                curColOffset += cbnds.Width;

                                // Determine if more pages need to be drawn.
                                e.HasMorePages = !(r == this.RowCount - 1 && c == this.ColumnCount - 1);
                                lY = r; lX = c;
                            }
                            curRowOffset += rbnds.Height;
                        }
                    }
                }
            }
        }
        //***************************************************************************
        // Event Overrides
        // 
        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            base.OnPaint(e);

            // Only line numbers are turned off, we're done.
            if (!_useLns) return;

            RectangleF drawBounds;
            try
            {
                // If there's no visible rows, then there's no point drawing numbers.
                if (this.RowCount < 1) return;

                // If the shmuck turned off row headers, but left the line numbers
                //   visible, there's little we can do about it now.
                if (!this.RowHeadersVisible) return;

                // We only want to redraw the row numbers if the row headers
                //   are being redrawn.
                if (e.ClipRectangle.Left < this.RowHeadersWidth)
                {
                    // Figure out how many numbers we're going to draw.
                    //int rCount = -1;
                    //if ((this.DataSource as DataTable).Rows.Count >= this.RowCount)
                    //    rCount = this.RowCount;
                    //else
                    //    rCount = (this.DataSource as DataTable).Rows.Count;
                    int rCount = this.RowCount;

                    using (Font drawFont = new Font("Tahoma", 11f, FontStyle.Regular, GraphicsUnit.Pixel))
                    using (SolidBrush brush = new SolidBrush(this.ForeColor))
                    using (StringFormat format = new StringFormat(StringFormatFlags.NoWrap))
                    {
                        format.LineAlignment = StringAlignment.Far;
                        format.Alignment = StringAlignment.Far;
                        for (int i = this.FirstDisplayedScrollingRowIndex; i <= (this.Rows.Count - 1); i++)
                        {
                            // Get the drawing boundary for this row's header cell.
                            //drawBounds = new RectangleF(5, ((i - this.FirstDisplayedScrollingRowIndex) * (this.Rows[i].Height + 5)) + this.ColumnHeadersHeight, this.RowHeadersWidth, this.Rows[i].Height);
                            drawBounds = new RectangleF(12, ((i - this.FirstDisplayedScrollingRowIndex) * (this.Rows[i].Height)) + (this.ColumnHeadersHeight), this.RowHeadersWidth - 15, this.Rows[i].Height);

                            // When we reach a row that's not visible, or
                            //   outside the clip rectangle, break the loop.
                            if (!this.Rows[i].Visible) break;

                            if (drawBounds.Top > e.ClipRectangle.Top || drawBounds.Bottom < e.ClipRectangle.Bottom || drawBounds.Left > e.ClipRectangle.Left || drawBounds.Right < e.ClipRectangle.Right)
                            {
                                string lnDisp = Convert.ToString(i + ((this._zeroBase) ? 0 : 1));
                                if (_padNum)
                                    lnDisp = lnDisp.PadLeft(this.RowCount.ToString().Length, '0');

                                // And draw the number.
                                e.Graphics.DrawString(lnDisp, drawFont, brush, drawBounds, format);
                            }
                        }
                    }
                }
            }
            #region Build-Specific Catch Handler
#if DEBUG
            catch (Exception ex)
            { Console.WriteLine(ex.ToString()); }
#else
            catch { }
#endif
            #endregion
            finally
            {
                // Don't want to leave these lying around in memory, since we
                //   create a new one for every row.
                drawBounds = RectangleF.Empty;
                e.Graphics.Dispose();
            }
        }
        protected override void OnCellPainting(System.Windows.Forms.DataGridViewCellPaintingEventArgs e)
        {
            if (e.Value == System.DBNull.Value && this.DefaultCellStyle.NullValue != null)
            {
                System.Windows.Forms.DataGridViewCellStyle style = e.CellStyle;
                using (Font fontItl = new Font(this.Font.FontFamily, this.Font.Size - 0.1f, FontStyle.Italic, this.Font.Unit))
                {
                    style.Font = fontItl;
                    style.ForeColor = System.Drawing.Color.Gray;
                    e.CellStyle.ApplyStyle(style);
                }
            }
            base.OnCellPainting(e);
        }
        #endregion
    }
}
