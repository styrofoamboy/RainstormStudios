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
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using RainstormStudios;

namespace RainstormStudios.Web.UI.WebControls
{
    [DefaultProperty("FilterColumns"), ParseChildren(ChildrenAsProperties = true), ToolboxData("<{0}:GridFilter runat=\"server\"></{0}:GridFilter>")]
    public class GridFilter : System.Web.UI.WebControls.Panel, System.Web.UI.INamingContainer
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private FilterColumnCollection
            _filterCols;
        private DropDownList
            _drpFilterColumn;
        private TextBox
            _txtFilterVal;
        private Image
            _imgIcon;
        private Button
            _btnFilter,
            _btnClear;
        //***************************************************************************
        // Public Events
        // 
        public event GridFilterEventHandler FilterClicked;
        public event EventHandler ClearClicked;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public string FilterButtonText
        {
            get
            {
                string vsVal = (string)this.ViewState["FilterButtonText"];
                return !string.IsNullOrEmpty(vsVal)
                        ? vsVal
                        : "Filter";
            }
            set { this.ViewState["FilterButtonText"] = value; }
        }
        public string ClearButtonText
        {
            get
            {
                string vsVal = (string)this.ViewState["ClearButtonText"];
                return !string.IsNullOrEmpty(vsVal)
                            ? vsVal
                            : "Clear";
            }
            set { this.ViewState["ClearButtonText"] = value; }
        }
        public string IconImageURL
        {
            get { return (string)this.ViewState["IconImageURL"]; }
            set { this.ViewState["IconImageURL"] = value; }
        }
        public bool IconImageVisible
        {
            get
            {
                object vsVal = this.ViewState["IconImageVisible"];
                bool bVsVal;
                if (vsVal == null || !bool.TryParse(vsVal.ToString(), out bVsVal))
                    return true;
                else
                    return bVsVal;
            }
            set { this.ViewState["IconImageVisible"] = value; }
        }
        public Unit FilterColumnWidth
        {
            get
            {
                object vsVal = this.ViewState["FilterColumnWidth"];
                if (vsVal == null || !(vsVal is Unit))
                    return Unit.Empty;
                else
                    return (Unit)vsVal;
            }
            set { this.ViewState["FilterColumnWidth"] = value; }
        }
        public Unit FilterValueWidth
        {
            get
            {
                object vsVal = this.ViewState["FilterValueWidth"];
                if (vsVal == null || !(vsVal is Unit))
                    return Unit.Empty;
                else
                    return (Unit)vsVal;
            }
            set { this.ViewState["FilterValueWidth"] = value; }
        }
        public System.Drawing.Color FilterValueForeColor
        {
            get
            {
                object vsVal = this.ViewState["FilterValueForeColor"];
                if (vsVal == null || !(vsVal is System.Drawing.Color))
                    return System.Drawing.Color.Black;
                else
                    return (System.Drawing.Color)vsVal;
            }
            set { this.ViewState["FilterValueForeColor"] = value; }
        }
        public System.Drawing.Color FilterValueBackColor
        {
            get
            {
                object vsVal = this.ViewState["FilterValueBackColor"];
                if (vsVal == null || !(vsVal is System.Drawing.Color))
                    return System.Drawing.Color.White;
                else
                    return (System.Drawing.Color)vsVal;
            }
            set { this.ViewState["FilterValueBackColor"] = value; }
        }
        public ButtonStyleTheme ButtonStyleTheme
        {
            get
            {
                object vsVal = this.ViewState["ButtonStyleTheme"];
                ButtonStyleTheme styleTheme;
                if (vsVal == null || !Enum.TryParse(vsVal.ToString(), out styleTheme))
                    return ButtonStyleTheme.Standard;
                else
                    return styleTheme;
            }
            set { this.ViewState["ButtonStyleTheme"] = value; }
        }
        public ButtonColorTheme ButtonColorTheme
        {
            get
            {
                object vsVal = this.ViewState["ButtonColorTheme"];
                ButtonColorTheme clrTheme;
                if (vsVal == null || !Enum.TryParse(vsVal.ToString(), out clrTheme))
                    return ButtonColorTheme.Blue;
                else
                    return clrTheme;
            }
            set { this.ViewState["ButtonColorTheme"] = value; }
        }
        public Style ButtonStyle
        {
            get
            {
                object vsVal = this.ViewState["ButtonStyle"];
                if (vsVal == null)
                {
                    System.Web.UI.WebControls.Style myStyle = new System.Web.UI.WebControls.Style(this.ViewState);
                    this.ButtonStyle = myStyle;
                    return myStyle;
                }
                else
                    return (System.Web.UI.WebControls.Style)vsVal;
            }
            private set { this.ViewState["ButtonStyle"] = value; }
        }
        public Unit ButtonTextVerticalOffset
        {
            get
            {
                object vsVal = this.ViewState["TextVertOffset"];
                if (vsVal == null)
                    return Unit.Empty;
                else
                    try { return Unit.Parse(vsVal.ToString()); }
                    catch { return Unit.Empty; }
            }
            set { this.ViewState["TextVertOffset"] = value; }
        }
        public string LinkedTableClientID
        {
            get { return (string)this.ViewState["LinkedTableClientID"]; }
            set { this.ViewState["LinkedTableClientID"] = value; }
        }
        public string FilterWaitIndicatorImageURL
        {
            get
            {
                string imgUrl = (string)this.ViewState["FilterWaitIndicatorImageURL"];
                if (string.IsNullOrEmpty(imgUrl))
                    return this.Page.ClientScript.GetWebResourceUrl(typeof(RainstormStudios.Web.UI.WebControls.GridFilter), "RainstormStudios.Web.UI.WebControls.images.gridFilter.loader.gif");
                else
                    return imgUrl;
            }
            set { this.ViewState["FilterWaitIndicatorImageURL"] = value; }
        }
        public string FilterWaitIndicatorBackgroundColor
        {
            get
            {
                string vsVal = (string)this.ViewState["FilterWaitIndicatorBackgroundColor"];
                if (string.IsNullOrEmpty(vsVal))
                    return "#fff";
                else
                    return vsVal;
            }
            set
            {
                try
                { RainstormStudios.Hex.GetSystemColor(value); }
                catch (Exception ex)
                { throw new Exception("The value provided for the GridFilter control's 'FilterWebIndicatorBackgroundColor' does not appear to be a valid web color.", ex); }

                this.ViewState["FilterWaitIndicatorBackgroundColor"] = "#" + value.Trim().TrimStart('#', ' ');
            }
        }
        public float FilterWaitIndicatorOpacity
        {
            get
            {
                object vsVal = this.ViewState["FilterWaitIndicatorOpacity"];
                float fVsVal;
                if (vsVal == null || !float.TryParse(vsVal.ToString(), out fVsVal))
                    return 0.5f;
                else
                    return fVsVal;
            }
            set
            {
                if (value < 0 || value > 1)
                    throw new ArgumentOutOfRangeException("The FilterWaitIndicatorOpacity property value should be a floating point value between '0' and '1'.");
                else
                    this.ViewState["FilterWaitIndicatorOpacity"] = value;
            }
        }
        /// <summary>
        /// The list of items to display in the drop down list in the following format: "Display Name:PropertyName,Display Name:PropertyName:etc...."
        /// </summary>
        public string FilterColumnsString
        {
            get { return this._filterCols.ToString(); }
            set
            {
                this._filterCols = new FilterColumnCollection(value);
                if (!this.ChildControlsCreated)
                    this.CreateChildControls();
                this.PopulateFilterColumnDropDown();
            }
        }
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public FilterColumnCollection FilterColumns
        {
            get { return this._filterCols; }
        }
        public bool AutoSortColumnList
        {
            get
            {
                object vsVal = this.ViewState["AutoSortColumnList"];
                bool bVsVal;
                if (vsVal == null || !bool.TryParse(vsVal.ToString(), out bVsVal))
                    return false;
                else
                    return bVsVal;
            }
            set { this.ViewState["AutoSortColumnList"] = value; }
        }
        [Browsable(false)]
        public string SelectedFilterColumn
        {
            get
            {
                this.EnsureChildControls();
                return this._drpFilterColumn.SelectedItem.Value;
            }
            set
            {
                this.EnsureChildControls();
                this._drpFilterColumn.SelectedValue = value;
            }
        }
        [Browsable(false)]
        public RainstormStudios.Data.Linq.ComparisonOperator SelectedFilterColumnOperator
        {
            get
            {
                this.EnsureChildControls();
                return this.FilterColumns[this._drpFilterColumn.SelectedItem.Text].Operator;
            }
        }
        [Browsable(false)]
        public bool SelectedFilterColumnMatchCase
        {
            get
            {
                this.EnsureChildControls();
                return this.FilterColumns[this._drpFilterColumn.SelectedItem.Text].MatchCase;
            }
        }
        [Browsable(false)]
        public System.Data.DbType SelectedFilterColumnDataType
        {
            get
            {
                this.EnsureChildControls();
                return this.FilterColumns[this._drpFilterColumn.SelectedItem.Text].DataType;
            }
        }
        [Browsable(false)]
        public bool SelectedFilterColumnAllowNull
        {
            get
            {
                this.EnsureChildControls();
                return this.FilterColumns[this._drpFilterColumn.SelectedItem.Text].AllowNullSearch;
            }
        }
        [Browsable(false)]
        public object FilterValue
        {
            get
            {
                this.EnsureChildControls();
                return this.GetEnteredValue();
            }
            set
            {
                this.EnsureChildControls();
                this._txtFilterVal.Text = (value != null) ? value.ToString() : string.Empty;
            }
        }
        //***************************************************************************
        // Private Properties
        // 
        private string JavaScriptKeyName
        { get { return "GridFilterWaitIndicator_" + this.ClientID; } }
        private string ValidationGroup
        { get { return "RSGridFilter_" + this.ClientID; } }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public GridFilter()
        {
            this._filterCols = new FilterColumnCollection();
            this._filterCols.Updated += new Collections.CollectionEventHandler(this.filterCol_CollectionAltered);
            this._filterCols.Inserted += new Collections.CollectionEventHandler(this.filterCol_CollectionAltered);
            this._filterCols.Removed += new Collections.CollectionEventHandler(this.filterCol_CollectionAltered);
        }
        #endregion

        #region Event Triggers
        //***************************************************************************
        // Event Triggers
        // 
        protected void OnFilter(GridFilterEventArgs e)
        {
            if (this.FilterClicked != null)
                this.FilterClicked.Invoke(this, e);
        }
        protected void OnClear(EventArgs e)
        {
            this._txtFilterVal.Text = string.Empty;
            this._txtFilterVal.Focus();
            if (this.ClearClicked != null)
                this.ClearClicked.Invoke(this, e);
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public void ClearFilterValue()
        {
            this._txtFilterVal.Text = string.Empty;
        }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        protected void PopulateFilterColumnDropDown()
        {
            this._drpFilterColumn.Items.Clear();
            if (this.AutoSortColumnList)
                this.FilterColumns.Sort();
            for (int i = 0; i < this.FilterColumns.Count; i++)
                this._drpFilterColumn.Items.Add(new ListItem(this.FilterColumns[i].DisplayName, this.FilterColumns[i].DataColumn));
        }
        protected override void OnPreRender(EventArgs e)
        {
            this.EnsureChildControls();

            if (string.IsNullOrEmpty(this._imgIcon.ImageUrl))
                this._imgIcon.ImageUrl = this.Page.ClientScript.GetWebResourceUrl(typeof(RainstormStudios.Web.UI.WebControls.GridFilter), "RainstormStudios.Web.UI.WebControls.images.gridFilter.icon_search_b.png");

            if (!string.IsNullOrEmpty(this.LinkedTableClientID))
            {
                string scriptPathRef = this.Page.ClientScript.GetWebResourceUrl(typeof(RainstormStudios.Web.UI.WebControls.GridFilter), "RainstormStudios.Web.UI.WebControls.scripts.gridFilter.js");
                this.Page.ClientScript.RegisterClientScriptInclude(this.Page.GetType(), "GridFilterCover", scriptPathRef);

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("$(document).ready(function () {");
                sb.AppendLine(string.Format("initGridFilterSearchIndicator('{0}', '{4}', '{1}', {2}, '{3}')", this.LinkedTableClientID, this.FilterWaitIndicatorBackgroundColor, this.FilterWaitIndicatorOpacity, this.FilterWaitIndicatorImageURL, this.ClientID));
                sb.AppendLine("});");
                this.Page.ClientScript.RegisterClientScriptBlock(this.Page.GetType(), this.JavaScriptKeyName, sb.ToString(), true);
            }

            //this._btnFilter.ID += "_" + this.ClientID;
            //this._btnClear.ID += "_" + this.ClientID;
            this.DefaultButton = this._btnFilter.ID;

            //this._drpFilterColumn.ValidationGroup =
            //    this._txtFilterVal.ValidationGroup =
            //    this._btnFilter.ValidationGroup = this.ValidationGroup;

            base.OnPreRender(e);
        }
        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            base.Render(writer);
        }
        protected override void RenderChildren(System.Web.UI.HtmlTextWriter writer)
        {
            this._imgIcon.RenderControl(writer);

            writer.AddStyleAttribute("float", "none");
            writer.AddStyleAttribute("vertical-align", "middle");
            writer.AddStyleAttribute("margin-left", "8px");
            writer.RenderBeginTag("span");

            this._drpFilterColumn.RenderControl(writer);

            writer.RenderEndTag();

            writer.AddStyleAttribute("float", "none");
            writer.AddStyleAttribute("vertical-align", "middle");
            writer.AddStyleAttribute("margin-left", "8px");
            writer.RenderBeginTag("span");

            this._txtFilterVal.RenderControl(writer);

            writer.RenderEndTag();

            writer.AddStyleAttribute("float", "none");
            writer.AddStyleAttribute("vertical-align", "middle");
            writer.AddStyleAttribute("margin-left", "8px");
            writer.RenderBeginTag("span");

            this._btnFilter.RenderControl(writer);

            writer.AddStyleAttribute("float", "none");
            writer.AddStyleAttribute("margin-left", "8px");
            this._btnClear.RenderControl(writer);

            writer.RenderEndTag();

            // We don't allow the user to specify child controls here.
            //base.RenderChildren(writer);
        }
        protected override void CreateChildControls()
        {
            if (this.ChildControlsCreated)
                return;

            Image imgIcon = new Image();
            imgIcon.ID = "imgSearch";
            imgIcon.AlternateText = "Grid Filter";
            imgIcon.ImageUrl = this.IconImageURL;
            imgIcon.Visible = this.IconImageVisible;
            imgIcon.ImageAlign = ImageAlign.Middle;
            this._imgIcon = imgIcon;

            DropDownList drpFilterColumn = new DropDownList();
            drpFilterColumn.ID = "drpFilterColumn";
            drpFilterColumn.Width = this.FilterColumnWidth;
            this._drpFilterColumn = drpFilterColumn;
            this.PopulateFilterColumnDropDown();

            TextBox txtFilterVal = new TextBox();
            txtFilterVal.ID = "txtFilterVal";
            txtFilterVal.Width = this.FilterValueWidth;
            txtFilterVal.ForeColor = this.FilterValueForeColor;
            txtFilterVal.BackColor = this.FilterValueBackColor;
            this._txtFilterVal = txtFilterVal;

            Button btnFilter = new Button();
            btnFilter.ID = "btnFilter";
            btnFilter.Text = this.FilterButtonText;
            btnFilter.StyleTheme = this.ButtonStyleTheme;
            btnFilter.ColorTheme = this.ButtonColorTheme;
            btnFilter.ControlStyle.MergeWith(this.ButtonStyle);
            btnFilter.TextVerticalOffset = this.ButtonTextVerticalOffset;
            btnFilter.Click += new EventHandler(this.btnFilter_onClick);
            this._btnFilter = btnFilter;

            Button btnClear = new Button();
            btnClear.ID = "btnClear";
            btnClear.Text = this.ClearButtonText;
            btnClear.StyleTheme = this.ButtonStyleTheme;
            btnClear.ColorTheme = this.ButtonColorTheme;
            btnClear.ControlStyle.MergeWith(this.ButtonStyle);
            btnClear.TextVerticalOffset = this.ButtonTextVerticalOffset;
            btnClear.Click += new EventHandler(this.btnClear_onClick);
            this._btnClear = btnClear;

            this.Controls.Add(this._imgIcon);
            this.Controls.Add(this._drpFilterColumn);
            this.Controls.Add(this._txtFilterVal);
            this.Controls.Add(this._btnFilter);
            this.Controls.Add(this._btnClear);

            //base.CreateChildControls();
            this.ChildControlsCreated = true;
        }
        private object GetEnteredValue()
        {
            object filterVal = null;
            string txtVal = this._txtFilterVal.Text;

            if (string.IsNullOrEmpty(txtVal))
                return null;

            switch (this.SelectedFilterColumnDataType)
            {
                case System.Data.DbType.Object:
                case System.Data.DbType.AnsiString:
                case System.Data.DbType.AnsiStringFixedLength:
                case System.Data.DbType.String:
                case System.Data.DbType.VarNumeric:
                case System.Data.DbType.StringFixedLength:
                    filterVal = txtVal;
                    break;
                case System.Data.DbType.Binary:
                    filterVal = System.Text.Encoding.Unicode.GetBytes(txtVal);
                    break;
                case System.Data.DbType.Boolean:
                    {
                        bool bVal;
                        if (!bool.TryParse(txtVal.Trim(), out bVal))
                            throw new InvalidCastException("Unable to convert entered value into a boolean: " + txtVal.Trim());
                        filterVal = bVal;
                    }
                    break;
                case System.Data.DbType.Byte:
                    {
                        byte btVal;
                        if (!byte.TryParse(txtVal.Trim(), out btVal))
                            throw new InvalidCastException("Unable to convert entered value into a byte: " + txtVal.Trim());
                        filterVal = btVal;
                    }
                    break;
                case System.Data.DbType.SByte:
                    {
                        sbyte sbtVal;
                        if (!sbyte.TryParse(txtVal.Trim(), out sbtVal))
                            throw new InvalidCastException("Unable to convert entered value into a signed byte: " + txtVal.Trim());
                        filterVal = sbtVal;
                    }
                    break;
                case System.Data.DbType.Int16:
                    {
                        Int16 iVal;
                        if (!Int16.TryParse(txtVal.Trim(), out iVal))
                            throw new InvalidCastException("Unable to convert entered value into a 16-bit integer: " + txtVal);
                        filterVal = iVal;
                    }
                    break;
                case System.Data.DbType.Int32:
                    {
                        Int32 iVal;
                        if (!Int32.TryParse(txtVal.Trim(), out iVal))
                            throw new InvalidCastException("Unable to convert entered value into a 32-bit integer: " + txtVal.Trim());
                        filterVal = iVal;
                    }
                    break;
                case System.Data.DbType.Int64:
                    {
                        Int64 iVal;
                        if (!Int64.TryParse(txtVal.Trim(), out iVal))
                            throw new InvalidCastException("Unable to convert entered value into a 64-bit integer: " + txtVal.Trim());
                        filterVal = iVal;
                    }
                    break;
                case System.Data.DbType.UInt16:
                    {
                        UInt16 iVal;
                        if (!UInt16.TryParse(txtVal.Trim(), out iVal))
                            throw new InvalidCastException("Unable to convert entered value into an unsigned 16-bit integer: " + txtVal.Trim());
                        filterVal = iVal;
                    }
                    break;
                case System.Data.DbType.UInt32:
                    {
                        UInt32 iVal;
                        if (!UInt32.TryParse(txtVal.Trim(), out iVal))
                            throw new InvalidCastException("Unable to convert entered value into an unsigned 32-bit integer: " + txtVal.Trim());
                        filterVal = iVal;
                    }
                    break;
                case System.Data.DbType.UInt64:
                    {
                        UInt64 iVal;
                        if (!UInt64.TryParse(txtVal.Trim(), out iVal))
                            throw new InvalidCastException("Unable to convert entered value into an unsigned 64-bit integer: " + txtVal.Trim());
                        filterVal = iVal;
                    }
                    break;
                case System.Data.DbType.Single:
                case System.Data.DbType.Currency:
                case System.Data.DbType.Decimal:
                case System.Data.DbType.Double:
                    {
                        double dVal;
                        if (!double.TryParse(txtVal.Trim(), out dVal))
                            throw new InvalidCastException("Unable to convert entered value into a " + this.SelectedFilterColumnDataType.ToString().ToLower() + ": " + txtVal.Trim());
                        filterVal = dVal;
                    }
                    break;
                case System.Data.DbType.Date:
                case System.Data.DbType.Time:
                case System.Data.DbType.DateTime:
                    {
                        DateTime dtVal;
                        if (!DateTime.TryParse(txtVal.Trim(), out dtVal))
                            throw new InvalidCastException("Unable to convert entered value into a date time: " + txtVal.Trim());
                        filterVal = dtVal;
                    }
                    break;
                case System.Data.DbType.DateTimeOffset:
                case System.Data.DbType.DateTime2:
                    throw new NotSupportedException("Specified data type is not supported: " + this.SelectedFilterColumnDataType.ToString());
            }
            return filterVal;
        }
        #endregion

        #region Event Handlers
        //***************************************************************************
        // Event Handlers
        // 
        private void filterCol_CollectionAltered(object sender, Collections.CollectionEventArgs e)
        {
            if (!this.ChildControlsCreated)
                this.CreateChildControls();
            this.PopulateFilterColumnDropDown();
        }
        private void btnFilter_onClick(object sender, EventArgs e)
        {
            try
            {
                if (this.FilterValue != null || this.SelectedFilterColumnAllowNull)
                    this.OnFilter(new GridFilterEventArgs(this.FilterValue, this._drpFilterColumn.SelectedItem.Text, this._drpFilterColumn.SelectedValue, this.SelectedFilterColumnDataType));
            }
            catch (InvalidCastException)
            { this.OnFilter(new GridFilterEventArgs(null, this._drpFilterColumn.SelectedItem.Text, this._drpFilterColumn.SelectedValue, this.SelectedFilterColumnDataType, true)); }
        }
        private void btnClear_onClick(object sender, EventArgs e)
        {
            this.OnClear(e);
        }
        #endregion
    }
    [PersistChildren(true)]
    public class FilterColumn : INamingContainer
    {
        public string
            DisplayName { get; set; }
        public string
            DataColumn { get; set; }
        public bool
            MatchCase { get; set; }
        public Data.Linq.ComparisonOperator
            Operator { get; set; }
        public System.Data.DbType
            DataType { get; set; }
        public bool
            AllowNullSearch { get; set; }

        public FilterColumn()
        { }
        public FilterColumn(string dispNm, string dataClm)
            : this()
        {
            this.DisplayName = dispNm;
            this.DataColumn = dataClm;
            this.Operator = Data.Linq.ComparisonOperator.Contains;
            this.MatchCase = false;
            this.DataType = System.Data.DbType.String;
            this.AllowNullSearch = false;
        }
        public FilterColumn(string dispNm, string dataClm, bool matchCase, Data.Linq.ComparisonOperator op)
            : this(dispNm, dataClm)
        {
            this.MatchCase = matchCase;
            this.Operator = op;
        }
    }
    [PersistChildren(true)]
    public class FilterColumnCollection : Collections.ObjectCollectionBase<FilterColumn>
    {
        #region Class Constructors
        //***********************************************************************
        // Class Constructors
        // 
        internal FilterColumnCollection()
            : base()
        { }
        internal FilterColumnCollection(string values)
            : this()
        {
            string[] filterItems = values.Split(',');
            for (int i = 0; i < filterItems.Length; i++)
            {
                string[] vals = filterItems[i].Split(':');
                this.Add(vals[0].Trim(), vals.Length > 1 ? vals[1].Trim() : vals[0].Trim());
            }
        }
        #endregion

        #region Public Methods
        //***********************************************************************
        // Public Methods
        // 
        public void Add(string displayName, string dataColumn)
        {
            this.Add(new FilterColumn(displayName, dataColumn));
        }
        public void Add(FilterColumn value)
        {
            if (base._keys.Contains(value.DisplayName))
                throw new Exception("You cannot add multiple filter columns with the same name.");
            base.Add(value, value.DisplayName);
        }
        public new void Sort(Collections.SortDirection dir = Collections.SortDirection.Ascending)
        {
            base.Sort("DisplayName", dir);
        }
        #endregion
    }
    public delegate void GridFilterEventHandler(object sender, GridFilterEventArgs e);
    public class GridFilterEventArgs : EventArgs
    {
        public readonly string
            FilterColumn,
            DataColumn;
        public readonly object
            FilterValue;
        public readonly bool
            InvalidFilterValue;
        public readonly System.Data.DbType
            DataType;

        public GridFilterEventArgs(object filterVal, string filterCol, string dataCol, System.Data.DbType dataType)
        {
            this.FilterValue = filterVal;
            this.FilterColumn = filterCol;
            this.DataColumn = dataCol;
            this.DataType = dataType;
            this.InvalidFilterValue = false;
        }
        public GridFilterEventArgs(object filterVal, string filterCol, string dataCol, System.Data.DbType dataType, bool invalidValue)
            : this(filterVal, filterCol, dataCol, dataType)
        {
            this.InvalidFilterValue = invalidValue;
        }
    }
}
