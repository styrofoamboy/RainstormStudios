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
using System.Windows.Forms;

namespace RainstormStudios.Controls
{
    public class DataGridDropListColumn : System.Windows.Forms.DataGridTextBoxColumn
    {
        #region Declarations
        //***************************************************************************
        // Global Variables
        // 
        // All rows will share a single combo box.
        private ComboBox _cboColumn;
        // The datasource for the incoming data.
        private object _objSource;
        private string _strMember;
        private string _strValue;
        // We have to know if the combo is currently bound to the DataGrid
        private bool _bIsBound = false;
        // Custom brush colors for painting the cell.
        private Brush _backBrush = null;
        private Brush _foreBrush = null;
        // We have to retain this for the "Leave" event handler.
        private int _iRowNum;
        private CurrencyManager _cmSource;
        //***************************************************************************
        // Public Fields
        // 
        public System.Drawing.Color ForegroundColor
        {
            set { if (value == System.Drawing.Color.Transparent)this._foreBrush = null; else this._foreBrush = new SolidBrush(value); }
        }
        public System.Drawing.Color BackgroundColor
        {
            set { if (value == System.Drawing.Color.Transparent)this._backBrush = null; else this._backBrush = new SolidBrush(value); }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public DataGridDropListColumn(object DataSource, string DataMember, string DataValue)
        {
            _objSource = DataSource;
            _strMember = DataMember;
            _strValue = DataValue;

            // Create a new ComboBox.
            _cboColumn = new ComboBox();

            _cboColumn.DataSource = _objSource;
            _cboColumn.DisplayMember = _strMember;
            _cboColumn.ValueMember = _strValue;
            _cboColumn.DropDownStyle = ComboBoxStyle.DropDownList;
            _cboColumn.Leave += new EventHandler(_cboColumn_Leave);
            _cboColumn.Visible = false;
        }
        #endregion

        #region Event Handlers
        //***************************************************************************
        // Event Handlers
        // 
        void _cboColumn_Leave(object sender, EventArgs e)
        {
            // Here, we're going to remove the ComboBox and write the actuall value
            //   member into the field.
            object objValue = _cboColumn.SelectedValue;

            // We can't pass a 'null' into the DataGrid, so we need to handle for this.
            if (objValue == null)
                objValue = DBNull.Value;

            // And finally set the data & clear the ComboBox.
            this.SetColumnValueAtRow(_cmSource, _iRowNum, objValue);
            _cboColumn.Visible = false;
        }
        #endregion

        #region Override Methods
        //***************************************************************************
        // Override Methods
        // 
        protected override void Edit(CurrencyManager source, int rowNum, Rectangle bounds, bool readOnly, string displayText, bool cellIsVisible)
        {
            if (!_bIsBound)
            {
                this.DataGridTableStyle.DataGrid.Controls.Add(_cboColumn);
                _bIsBound = true;
            }

            // This is used when the ComboBox loses focus
            _iRowNum = rowNum;
            _cmSource = source;

            // Sync the font size to the text box.
            _cboColumn.Font = this.TextBox.Font;

            // We need to retrieve the current value & set the ComboBox to that value.
            object objValue = this.GetColumnValueAtRow(source, rowNum);

            // And make the ComboBox fill the entire cell.
            _cboColumn.Bounds = bounds;

            // Do not paint the control until we've set the currect position in the items list.
            _cboColumn.BeginUpdate();

            // You have to set the visible property to true here, or the ComboBox will not populate.
            _cboColumn.Visible = true;

            // Check for null values returned by "GetValueAtRow".
            if (objValue.GetType() == typeof(System.DBNull))
                _cboColumn.SelectedIndex = 0;
            else
                _cboColumn.SelectedValue = objValue;

            // Now that everything's set, we can let the ComboBox paint itself.
            _cboColumn.EndUpdate();
            _cboColumn.Focus();

            // We don't want the base method to fire, or it will draw a text box over our control.
            //base.Edit(source, rowNum, bounds, readOnly, displayText, cellIsVisible);
        }
        protected override void Paint(Graphics g, Rectangle bounds, CurrencyManager source, int rowNum, Brush backBrush, Brush foreBrush, bool alignToRight)
        {
            string strValue = "?";
            DataRow[] dr;

            // Retrieve the value at the current column-row.
            object objValue = this.GetColumnValueAtRow(source, rowNum);

            // Use this object to access the datasource, again handling for null values.
            if (objValue.GetType() != typeof(System.DBNull))
                dr = ((DataTable)_objSource).Select(_strValue + " = " + objValue.ToString());
            else
                dr = ((DataTable)_objSource).Select();

            // If we found the value in the source data, grab the text value.
            if (dr.Length > 0)
                strValue = dr[0][_strMember].ToString();

            Rectangle rect = bounds;
            // Only attempt to use the custom backcolor if the user has set it.
            if (this._backBrush == null)
                g.FillRectangle(backBrush, rect);
            else
                g.FillRectangle(_backBrush, rect);

            // Resize the vertical height to compensate for the cell border.
            rect.Y += 2;
            if (this._foreBrush == null)
                g.DrawString(strValue, this.TextBox.Font, foreBrush, rect);
            else
                g.DrawString(strValue, this.TextBox.Font, _foreBrush, rect);

            // Again, we don't want the base method to fire.
            //base.Paint(g, bounds, source, rowNum, backBrush, foreBrush, alignToRight);
        }
        #endregion
    }
    public class DataGridFileBrowseColumn : System.Windows.Forms.DataGridTextBoxColumn
    {
        #region Declarations
        //***************************************************************************
        // Global Variables
        // 
        // All the cells will share a single 'browse' button and text box.
        private TextBox _txtColumn;
        private Button _cmdColumn;
        // We have to know if the button is bound to the DataGrid
        private bool _bIsBound = false;
        // Custom fore/background colors
        private Brush _backBrush = null;
        private Brush _foreBrush = null;
        // Information aquired when we start edit on a field, used to update that field
        //   in the 'Leave' event handler.
        private int _iRowNum;
        private CurrencyManager _cmSource;
        // This will tell us whether or not to hide the browse button when it loses focus
        private bool _clicking = false;
        //***************************************************************************
        // Public Fields
        // 
        public System.Drawing.Color ForegroundColor
        {
            set { if (value == System.Drawing.Color.Transparent)_foreBrush = null; else _foreBrush = new SolidBrush(value); }
        }
        public System.Drawing.Color BackgroundColor
        {
            set { if (value == System.Drawing.Color.Transparent)_backBrush = null; else _backBrush = new SolidBrush(value); }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public DataGridFileBrowseColumn()
        {
            _txtColumn = new TextBox();
            _txtColumn.Visible = false;
            _txtColumn.BorderStyle = BorderStyle.None;
            _txtColumn.Font = TextBox.Font;
            _txtColumn.Leave += new EventHandler(_txtColumn_Leave);
            _cmdColumn = new Button();
            _cmdColumn.Width = 20;
            _cmdColumn.FlatStyle = FlatStyle.Popup;
            _cmdColumn.Text = "ooo";
            _cmdColumn.Font = new Font(FontFamily.GenericSansSerif, 2.5f, FontStyle.Bold);
            _cmdColumn.BackColor = SystemColors.ButtonFace;
            _cmdColumn.ForeColor = SystemColors.WindowText;
            _cmdColumn.TextAlign = ContentAlignment.BottomCenter;
            _cmdColumn.Visible = false;
            _cmdColumn.Click += new EventHandler(_cmdColumn_Click);
            _cmdColumn.MouseEnter += new EventHandler(_cmdColumn_onMouseOver);
            _cmdColumn.MouseLeave += new EventHandler(_cmdColumn_onMouseOut);
            _txtColumn.Controls.Add(_cmdColumn);
            this.ReadOnly = true;
        }
        #endregion

        #region Event Handlers
        //***************************************************************************
        // Event Handlers
        // 
        void _cmdColumn_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.ShowNewFolderButton = true;
            dlg.Description = "Select File Path";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                this.ColumnStartedEditing(this.DataGridTableStyle.DataGrid);
                this.SetColumnValueAtRow(_cmSource, _iRowNum, dlg.SelectedPath);
                //this.Paint(_gGrphx, _rBounds, _cmSource, _iRowNum, SystemBrushes.Window, SystemBrushes.WindowText, false);
                this.EndEdit();
            }
            _cmdColumn.Visible = false;
        }
        void _cmdColumn_onMouseOver(object sender, EventArgs e)
        {
            this._clicking = true;
        }
        void _cmdColumn_onMouseOut(object sender, EventArgs e)
        {
            this._clicking = false;
        }
        void _txtColumn_Leave(object sender, EventArgs e)
        {
            object objValue;
            if (string.IsNullOrEmpty(_txtColumn.Text))
                objValue = DBNull.Value;
            else
                objValue = _txtColumn.Text;
            this.SetColumnValueAtRow(_cmSource, _iRowNum, objValue);
            _txtColumn.Visible = false;
            if (!_clicking)
                _cmdColumn.Visible = false;
        }
        #endregion

        #region Override Methods
        //***************************************************************************
        // Override Methods
        // 
        protected override void Edit(CurrencyManager source, int rowNum, Rectangle bounds, bool readOnly, string displayText, bool cellIsVisible)
        {
            // We want to be sure to grab the CurrencyManager and rowNum values.
            _iRowNum = rowNum;
            _cmSource = source;

            // Then, we overlay our browse button on top of that.
            if (!_bIsBound)
            {
                this.DataGridTableStyle.DataGrid.Controls.Add(_cmdColumn);
                this.DataGridTableStyle.DataGrid.Controls.Add(_txtColumn);
                _bIsBound = true;
            }
            Rectangle cmdBounds = new Rectangle(new Point(bounds.Right - _cmdColumn.Width, bounds.Y), new Size(_cmdColumn.Width, bounds.Height));
            _cmdColumn.FlatStyle = FlatStyle.Popup;
            _cmdColumn.Bounds = cmdBounds;
            Rectangle txtBounds = new Rectangle(bounds.Left + 2, bounds.Top + 2, bounds.Width - _cmdColumn.Width, bounds.Height);
            _txtColumn.Bounds = txtBounds;
            _txtColumn.Text = this.GetColumnValueAtRow(source, rowNum).ToString();
            _cmdColumn.Visible = true;
            _txtColumn.Visible = true;
            _txtColumn.BringToFront();
            _txtColumn.SelectAll();
            _txtColumn.Focus();

            //base.Edit(source, rowNum, new Rectangle(bounds.X, bounds.Y, bounds.Width - _cmdColumn.Width, bounds.Height), readOnly, displayText, cellIsVisible);
        }
        protected override void Paint(Graphics g, Rectangle bounds, CurrencyManager source, int rowNum, Brush backBrush, Brush foreBrush, bool alignToRight)
        {
            if (this._backBrush == null)
                g.FillRectangle(backBrush, bounds);
            else
                g.FillRectangle(_backBrush, bounds);

            string strValue = "";
            object objValue = this.GetColumnValueAtRow(source, rowNum);
            if (objValue.GetType() != typeof(System.DBNull))
                strValue = objValue.ToString();

            using (StringFormat format = new StringFormat())
            {
                format.LineAlignment = StringAlignment.Near;
                if (alignToRight)
                    format.Alignment = StringAlignment.Far;
                else
                    format.Alignment = StringAlignment.Near;

                bounds.Y += 2;
                if (this._foreBrush == null)
                    g.DrawString(strValue, _txtColumn.Font, foreBrush, bounds, format);
                else
                    g.DrawString(strValue, _txtColumn.Font, _foreBrush, bounds, format);
            }

            //base.Paint(g, bounds, source, rowNum, backBrush, foreBrush, alignToRight);
        }
        #endregion
    }
}
