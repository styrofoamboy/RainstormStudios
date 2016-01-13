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
using System.Security.Permissions;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RainstormStudios.Web.UI.WebControls
{
    /// <summary>
    /// !!!THIS CONTROL DOES NOT WORK PROPERLY!!!
    /// </summary>
    [DefaultProperty("Columns"), ParseChildren(true), ToolboxData("<{0}:GridView runat=\"server\"></{0}:GridView>"), Designer("RainstormStudios.Web.UI.WebControls.GridViewDesigner")]
    public class GridView : System.Web.UI.WebControls.CompositeDataBoundControl, INamingContainer
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        Label
            _spanPgBtns;
        ImageButton
            _imgFPg,
            _imgPrevPg,
            _imgNextPg,
            _imgLastPg;
        DropDownList
            _drpPageSz;
        PagerSettings
            _pgrSettings = new PagerSettings();
        TableItemStyle
            _hdrStyle,
            _ftrStyle,
            _itemStyle,
            _altItemStyle;
        DataControlFieldCollection
            _cols;
        GridViewRow
            _hdrRow,
            _ftrRow;
        ArrayList
            _rows;
        GridViewRowCollection
            _rowCol;
        //***************************************************************************
        // Public Events
        // 
        public event GridViewItemEventHandler ItemCreated;
        public event GridViewItemEventHandler ItemDataBound;
        public event GridViewItemCommandEventHandler ItemCommand;
        public event EventHandler PreviousPageClicked;
        public event EventHandler NextPageClicked;
        public event EventHandler FirstPageClicked;
        public event EventHandler LastPageClicked;
        public event CommandEventHandler SortClicked;
        public event EventHandler PageSizeChanged;
        public event GridViewSortEventHandler Sorting;
        public event GridViewPageEventHandler PageIndexChanging;
        public event EventHandler PageIndexChanged;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        /// <summary>
        /// Gets a ControlCollection object that represents the child controls for the control in the UI hierarchy.
        /// </summary>
        [Browsable(false)]
        public override ControlCollection Controls
        {
            get
            {
                // Make sure the child controls have been created before allowing programmatic
                // access to the control hierarchy
                this.EnsureChildControls();
                return base.Controls;
            }
        }
        //public string SortByColumn
        //{
        //    get { return (string)this.ViewState["SortByColumn"]; }
        //    set
        //    {
        //        //this.Sort(this._sortCol, (value == this._sortCol) ? System.Web.UI.WebControls.SortDirection.Ascending : System.Web.UI.WebControls.SortDirection.Descending);
        //        this.ViewState["SortByColumn"] = value;
        //    }
        //}
        [DefaultValue(true)]
        public bool ShowHeader
        {
            get
            {
                object vsVal = this.ViewState["ShowHeader"];
                bool bVsVal;
                if (vsVal == null || !bool.TryParse(vsVal.ToString(), out bVsVal))
                    return false;
                else
                    return bVsVal;
            }
            set { this.ViewState["ShowHeader"] = value; }
        }
        [DefaultValue(true)]
        public bool ShowFooter
        {
            get
            {
                object vsVal = this.ViewState["ShowFooter"];
                bool bVsVal;
                if (vsVal == null || !bool.TryParse(vsVal.ToString(), out bVsVal))
                    return false;
                else
                    return bVsVal;
            }
            set { this.ViewState["ShowFooter"] = value; }
        }
        [DefaultValue(true)]
        public bool ShowHeaderWhenEmpty
        {
            get
            {
                object vsVal = this.ViewState["ShowHeaderWhenEmpty"];
                bool bVsVal;
                if (vsVal == null || !bool.TryParse(vsVal.ToString(), out bVsVal))
                    return false;
                else
                    return bVsVal;
            }
            set { this.ViewState["ShowHeaderWhenEmpty"] = value; }
        }
        [DefaultValue(10)]
        public int PageSize
        {
            get
            {
                object vsVal = this.ViewState["PageSize"];
                int iVsVal;
                if (vsVal == null || !int.TryParse(vsVal.ToString(), out iVsVal))
                    return 10;
                else
                    return iVsVal;
            }
            set { this.ViewState["PageSize"] = value; }
        }
        [DefaultValue(0)]
        public int PageIndex
        {
            get
            {
                object vsVal = this.ViewState["PageIdx"];
                int iVsVal;
                if (vsVal == null || !int.TryParse(vsVal.ToString(), out iVsVal))
                    return -1;
                else
                    return iVsVal;
            }
            private set { this.ViewState["PageIdx"] = value; }
        }
        [DefaultValue(10)]
        public int PageCount
        {
            get
            {
                object vsVal = this.ViewState["PageCount"];
                int iVsVal;
                if (vsVal == null || !int.TryParse(vsVal.ToString(), out iVsVal))
                    return -1;
                else
                    return iVsVal;
            }
            private set { this.ViewState["PageCount"] = value; }
        }
        [Browsable(false)]
        public int TotalRecordCount
        {
            get
            {
                object vsVal = this.ViewState["TotalRecordCount"];
                int iVsVal;
                if (vsVal == null || !int.TryParse(vsVal.ToString(), out iVsVal))
                    return 0;
                else
                    return iVsVal;
            }
            private set { this.ViewState["TotalRecordCount"] = value; }
        }
        [Browsable(false)]
        public int VisibleRecordCount
        {
            get
            {
                object vsVal = this.ViewState["VisiableRecordCount"];
                int iVsVal;
                if (vsVal == null || !int.TryParse(vsVal.ToString(), out iVsVal))
                    return 0;
                else
                    return iVsVal;
            }
            private set { this.ViewState["VisibleRecordCount"] = value; }
        }
        [DefaultValue(true)]
        public bool AllowPaging
        {
            get
            {
                object vsVal = this.ViewState["AllowPaging"];
                bool bVsVal;
                if (vsVal == null || !bool.TryParse(vsVal.ToString(), out bVsVal))
                    return true;
                else
                    return bVsVal;
            }
            set { this.ViewState["AllowPaging"] = value; }
        }
        [DefaultValue(true)]
        public bool AllowSorting
        {
            get
            {
                object vsVal = this.ViewState["AllowSorting"];
                bool bVsVal;
                if (vsVal == null || !bool.TryParse(vsVal.ToString(), out bVsVal))
                    return true;
                else
                    return bVsVal;
            }
            set { this.ViewState["AllowSorting"] = value; }
        }
        [DefaultValue(false)]
        public int RowCount
        {
            get
            {
                object vsVal = this.ViewState["RowCount"];
                int iVsVal;
                if (vsVal == null || !int.TryParse(vsVal.ToString(), out iVsVal))
                    return 0;
                else
                    return iVsVal;
            }
            set { this.ViewState["RowCount"] = value; }
        }
        public bool AutoGenerateColumns
        {
            get
            {
                object vsVal = this.ViewState["AutoGenerateColumns"];
                bool bVsVal;
                if (vsVal == null || !bool.TryParse(vsVal.ToString(), out bVsVal))
                    return false;
                else
                    return bVsVal;
            }
            set { this.ViewState["AutoGenerateCoumns"] = value; }
        }
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public PagerSettings PagerSettings
        {
            get { return this._pgrSettings; }
        }
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public DataControlFieldCollection Columns
        {
            get { return this._cols; }
        }
        [Browsable(false)]
        public GridViewRowCollection Rows
        { get { return this._rowCol; } }
        [Browsable(false)]
        public GridViewRow HeaderRow
        { get { return this._hdrRow; } }
        [Browsable(false)]
        public GridViewRow FooterRow
        { get { return this._ftrRow; } }
        [Browsable(true)
        , DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
        , NotifyParentProperty(true)
        , Description("Specifies the data source columns to use as unique identifiers for each record.")]
        public string[] DataKeyNames
        {
            get
            {
                object vsVal = this.ViewState["DataKeyNames"];
                if (vsVal == null)
                    return null;
                else
                    return vsVal.ToString().Trim('[', ']').Split(new string[] { "]|[" }, StringSplitOptions.RemoveEmptyEntries);
            }
            set { this.ViewState["DataKeyNames"] = string.Format("[{0}]", string.Join("]|[", value)); }
        }
        /// <summary>
        /// A TableItemStyle object that contains the style properties for the header section of the GridView table. The default value is an empty TableItemStyle object.
        /// </summary>
        /// <value>A TableItemStyle object that contains the style properties of the heading section in the <see cref="GridView"/> control. 
        /// The default value is an empty TableItemStyle object.</value>
        /// <remarks>Use this property to provide a custom style for the heading section of the <see cref="GridView"/> control. 
        /// Common style attributes that can be adjusted include forecolor, backcolor, font, and content alignment 
        /// within the cell. Providing a different style enhances the appearance of the <see cref="GridView"/> control.</remarks>
        [Browsable(true)
        , PersistenceMode(PersistenceMode.InnerProperty)
        , DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
        , NotifyParentProperty(true)
        , Description("Specifies the style for the header.")]
        public virtual TableItemStyle HeaderStyle
        {
            get
            {
                if (this._hdrStyle == null)
                    this._hdrStyle = new TableItemStyle();

                if (IsTrackingViewState)
                    ((IStateManager)this._hdrStyle).TrackViewState();

                return this._hdrStyle;
            }
        }
        /// <summary>
        /// A TableItemStyle object that contains the style properties for the footer section of the GridView table. The default value is an empty TableItemStyle object.
        /// </summary>
        /// <value>A TableItemStyle object that contains the style properties of the heading section in the <see cref="GridView"/> control. 
        /// The default value is an empty TableItemStyle object.</value>
        /// <remarks>Use this property to provide a custom style for the heading section of the <see cref="GridView"/> control. 
        /// Common style attributes that can be adjusted include forecolor, backcolor, font, and content alignment 
        /// within the cell. Providing a different style enhances the appearance of the <see cref="GridView"/> control.</remarks>
        [Browsable(true)
        , PersistenceMode(PersistenceMode.InnerProperty)
        , DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
        , NotifyParentProperty(true)
        , Description("Specifies the style for the footer.")]
        public virtual TableItemStyle FooterStyle
        {
            get
            {
                if (this._ftrStyle == null)
                    this._ftrStyle = new TableItemStyle();

                if (IsTrackingViewState)
                    ((IStateManager)this._ftrStyle).TrackViewState();

                return this._ftrStyle;
            }
        }
        /// <summary>
        /// A TableItemStyle object that contains the style properties for each row in the GridView table. The default value is an empty TableItemStyle object.
        /// </summary>
        /// <value>A TableItemStyle object that contains the style properties of the items in the <see cref="GridView"/> control. 
        /// The default value is an empty TableItemStyle object.</value>
        /// <remarks>Use this property to provide a custom style for the item cells of the RssFeed control. 
        /// Common style attributes that can be adjusted include forecolor, backcolor, font, and content alignment 
        /// within the cell.</remarks>
        [Browsable(true)
        , Category("Appearance")
        , PersistenceMode(PersistenceMode.InnerProperty)
        , DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
        , NotifyParentProperty(true)
        , Description("Specifies the style for rows.")]
        public virtual TableItemStyle ItemStyle
        {
            get
            {
                if (this._itemStyle == null)
                    this._itemStyle = new TableItemStyle();

                if (IsTrackingViewState)
                    ((IStateManager)this._itemStyle).TrackViewState();

                return this._itemStyle;
            }
        }
        /// <summary>
        /// A TableItemStyle object that contains the style properties for each alternating row in the GridView table. The default value is an empty TableItemStyle object.
        /// </summary>
        /// <value>A TableItemStyle object that contains the style properties of the items in the <see cref="GridView"/> control. 
        /// The default value is an empty TableItemStyle object.</value>
        /// <remarks>Use this property to provide a custom style for the item cells of the RssFeed control. 
        /// Common style attributes that can be adjusted include forecolor, backcolor, font, and content alignment 
        /// within the cell.</remarks>
        [Browsable(true)
        , Category("Appearance")
        , PersistenceMode(PersistenceMode.InnerProperty)
        , DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
        , NotifyParentProperty(true)
        , Description("Specifies the style for alternating rows.")]
        public virtual TableItemStyle AlternatingItemStyle
        {
            get
            {
                if (this._altItemStyle == null)
                    this._altItemStyle = new TableItemStyle();

                if (IsTrackingViewState)
                    ((IStateManager)this._altItemStyle).TrackViewState();

                return this._altItemStyle;
            }
        }
        /// <summary>
        /// Gets or sets the URL of an image to display in the background of the <see cref="RssFeed"/> control.
        /// </summary>
        /// <value>
        /// The URL of an image to display in the background of a table control. The default is String.Empty.
        /// </value>
        /// <remarks>Use the <b>BackImageUrl</b> property to specify an image to display in the background of the <see cref="RssFeed"/> control.</remarks>
        [Bindable(true),
        Category("Appearance"),
        DefaultValue("")]
        public virtual string BackImageUrl
        {
            get
            {
                if (!this.ControlStyleCreated)
                    return string.Empty;
                else
                    return ((TableStyle)this.ControlStyle).BackImageUrl;
            }
            set { ((TableStyle)this.ControlStyle).BackImageUrl = value; }
        }
        /// <summary>
        /// The distance (in pixels) between the contents of a cell and the cell's border. The default is -1, which indicates that this property is not set.
        /// </summary>
        /// <remarks>Use the <b>CellPadding</b> property to control the spacing between the contents of a cell and the cell's border. The padding amount specified is added to all four sides of the cell.</remarks>
        [Bindable(true),
        Category("Appearance"),
        DefaultValue(0)]
        public virtual int CellPadding
        {
            get
            {
                if (!this.ControlStyleCreated)
                    return -1;
                else
                    return ((TableStyle)this.ControlStyle).CellPadding;
            }
            set { ((TableStyle)this.ControlStyle).CellPadding = value; }
        }
        /// <summary>
        /// One of the HorizontalAlign enumeration values. The default is NotSet.
        /// </summary>
        /// <remarks>Use the <b>HorizontalAlign</b> property to specify the horizontal alignment of a data listing control 
        /// within its container. This property is set with one of the HorizontalAlign enumeration values.</remarks>
        [Bindable(true)
        , Category("Appearance")
        , DefaultValue(HorizontalAlign.NotSet)]
        public virtual HorizontalAlign HorizontalAlign
        {
            get
            {
                if (!this.ControlStyleCreated)
                    return System.Web.UI.WebControls.HorizontalAlign.NotSet;
                else
                    return ((TableStyle)this.ControlStyle).HorizontalAlign;
            }
            set { ((TableStyle)this.ControlStyle).HorizontalAlign = value; }
        }
        public new virtual Unit Width
        {
            get
            {
                if (!this.ControlStyleCreated)
                    return Unit.Empty;
                else
                    return ((TableStyle)this.ControlStyle).Width;
            }
            set { ((TableStyle)this.ControlStyle).Width = value; }
        }
        public new virtual Unit Height
        {
            get
            {
                if (!this.ControlStyleCreated)
                    return Unit.Empty;
                else
                    return ((TableStyle)this.ControlStyle).Height;
            }
            set { ((TableStyle)this.ControlStyle).Height = value; }
        }
        public virtual bool ShowPager
        {
            get { return this._pgrSettings.Visible; }
            set { this._pgrSettings.Visible = value; }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constrcutors
        // 
        public GridView()
        {
            this._cols = new DataControlFieldCollection();
            this.PagerSettings.Visible = false;
            this.ChildControlsCreated = false;
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public override void Dispose()
        {
            base.Dispose();
        }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        protected override Style CreateControlStyle()
        {
            TableStyle style = new TableStyle(ViewState);

            style.BackImageUrl = this.BackImageUrl;
            style.CellPadding = this.CellPadding;
            style.HorizontalAlign = this.HorizontalAlign;
            style.GridLines = GridLines.None;
            style.CellSpacing = 0;
            style.BorderStyle = BorderStyle.None;

            return style;
        }
        protected override void OnPreRender(EventArgs e)
        {
            Control link = this.Page.Header.FindControl("CustomGridViewCss");
            if (link == null)
            {
                System.Web.UI.HtmlControls.HtmlLink newLink = new System.Web.UI.HtmlControls.HtmlLink();
                newLink.ID = "CustomGridViewCss";
                newLink.Attributes.Add("href", this.Page.ClientScript.GetWebResourceUrl(typeof(RainstormStudios.Web.UI.WebControls.GridView), "RainstormStudios.Web.UI.WebControls.style.CustomGridView.css"));
                newLink.Attributes.Add("type", "text/css");
                newLink.Attributes.Add("rel", "stylesheet");
                this.Page.Header.Controls.Add(newLink);
            }

            base.OnPreRender(e);
            this.EnsureChildControls();
        }
        protected override void Render(HtmlTextWriter writer)
        {
            if ((this.Site != null) && (this.Site.DesignMode))
            {
                this.ChildControlsCreated = false; this.EnsureChildControls();
                try { this.OnPreRender(EventArgs.Empty); }
                catch { }
            }

            if (this._rows == null)
                this.RequiresDataBinding = true;

            if (this.ShowHeader && this.HeaderRow != null && (this._rowCol.Count > 0 || this.ShowHeaderWhenEmpty))
                this.Controls.Add(this.HeaderRow);
            if (this._rows != null)
                for (int i = 0; i < this._rows.Count; i++)
                    this.Controls.Add(this._rowCol[i]);
            if (this.ShowFooter && this.FooterRow != null)
                this.Controls.Add(this.FooterRow);

            base.Render(writer);
        }
        public override void RenderBeginTag(HtmlTextWriter writer)
        {
            Table tbl = new Table();
            tbl.ID = "tbl_" + this.ID;
            tbl.MergeStyle(this.ControlStyle);
            tbl.RenderBeginTag(writer);
        }
        public override void RenderEndTag(HtmlTextWriter writer)
        {
            writer.Write("</table>");
        }
        protected void RegisterControlEvents()
        {
            if (this._imgFPg != null)
                this.Page.ClientScript.RegisterForEventValidation(this._imgFPg.UniqueID);
            if (this._imgPrevPg != null)
                this.Page.ClientScript.RegisterForEventValidation(this._imgPrevPg.UniqueID);
            if (this._imgNextPg != null)
                this.Page.ClientScript.RegisterForEventValidation(this._imgNextPg.UniqueID);
            if (this._imgLastPg != null)
                this.Page.ClientScript.RegisterForEventValidation(this._imgLastPg.UniqueID);
            if (this._drpPageSz != null)
                this.Page.ClientScript.RegisterForEventValidation(this._drpPageSz.UniqueID);

            for (int i = 1; i < this.Columns.Count; i++)
            {
                ImageButton img = (this.HeaderRow.Cells[i].FindControl("imgSort_" + i.ToString()) as ImageButton);
                if (img != null)
                    this.Page.ClientScript.RegisterForEventValidation(img.UniqueID);
            }
        }
        protected override void CreateChildControls()
        {
            //this.CreateChildControls(null, false);
        }
        protected override int CreateChildControls(IEnumerable dataSource, bool dataBinding)
        {
            // Clear first (the reason makes sense when called later by DataBind())
            // Use base...or it will trigger CompositeControls.EnsureChildControls first...
            base.Controls.Clear();

            //if (dataSource==null || dataSource.GetType().Name == "DummyDataSource")
            //{
            //    object cachedDataSrc = System.Web.HttpContext.Current.Cache.Get("GridViewObj" + this.UniqueID);
            //    dataSource = (cachedDataSrc as IEnumerable);
            //}
            //if (dataSource == null)
            //    throw new Exception("Unable to retrieve data source.");

            //if (dataSource == null)
            //    dataSource = (IEnumerable)this.ViewState["!_tblData"];


            int ctrlCnt = this.CreateControlHeirarchy(dataSource, dataBinding);

            // Set the flag so that the CreateChildControls doesn't get called
            //   again, unless by DataBind() -- which is the only exception.
            this.ChildControlsCreated = true;

            return ctrlCnt;
        }
        protected int CreateControlHeirarchy(IEnumerable dataSource, bool useDataSource)
        {
            // Clear/create the rows ArrayList
            if (this._rows == null)
                this._rows = new ArrayList();
            else
                this._rows.Clear();

            GridViewRow hdr = this.BuildHeader();

            if (this.PageIndex < 0)
                this.PageIndex = 0;
            //int startRec = this.PageIndex * this.PageSize;
            int itemCount = 0; //, recCount = 0;
            if (dataSource != null)
            {
                foreach (var dataItem in dataSource)
                {
                    //if (recCount < startRec || itemCount >= this.PageSize)
                    //{
                    //    recCount++;
                    //    continue;
                    //}

                    GridViewRow tr = new GridViewRow(itemCount + (this.ShowHeader ? 1 : 0), itemCount, DataControlRowType.DataRow, (itemCount % 2 == 0 ? DataControlRowState.Normal : DataControlRowState.Alternate));
                    tr.CssClass = (itemCount % 2 == 0) ? "GridViewLineAlt" : "GridViewLine";

                    TableCell tdL = new TableCell();
                    tdL.CssClass = "GridViewLineLeft";
                    tr.Cells.Add(tdL);

                    int curColCount = 0;
                    for (int i = 0; i < this.Columns.Count; i++)
                    {
                        TableCell td = new TableCell();

                        DataControlField column = this.Columns[i];

                        bool usedTemplate = false;
                        if (column is TemplateField)
                        {
                            TemplateField fld = (column as TemplateField);
                            ITemplate template = null;
                            if (itemCount % 2 != 0)
                                template = fld.AlternatingItemTemplate;
                            if (template == null)
                                template = fld.ItemTemplate;

                            if (template != null)
                            {
                                template.InstantiateIn(td);
                                usedTemplate = true;
                            }
                        }

                        if (!usedTemplate)
                        {
                            string dataStr = string.Empty;
                            if (column is BoundField)
                            {
                                BoundField fld = (column as BoundField);
                                if (!string.IsNullOrEmpty(fld.DataField))
                                    dataStr = DataBinder.GetPropertyValue(dataItem, fld.DataField, fld.DataFormatString);
                                else
                                {
                                    PropertyDescriptorCollection props = TypeDescriptor.GetProperties(dataItem);
                                    if (props.Count >= 1)
                                        if (null != props[0].GetValue(dataItem))
                                            dataStr = props[0].GetValue(dataItem).ToString();
                                }
                            }

                            if (column is CheckBoxField)
                            {
                                CheckBoxField fld = (column as CheckBoxField);
                                CheckBox chkFld = new CheckBox();
                                chkFld.ID = string.Format("chkField_{0}_{1}", itemCount, i);
                                string fldStr = string.Empty;
                                if (!string.IsNullOrEmpty(fld.Text))
                                    fldStr = fld.Text;
                                else if (!string.IsNullOrEmpty(fld.DataField))
                                    fldStr = dataStr;
                                chkFld.Text = fldStr;
                                chkFld.MergeStyle(fld.ControlStyle);
                                td.Controls.Add(chkFld);
                            }
                            else if (column is CommandField)
                            {
                                CommandField fld = (column as CommandField);

                                string keyArg = string.Empty;
                                if (this.DataKeyNames != null && this.DataKeyNames.Length > 0)
                                    keyArg = string.Join(",", this.DataKeyNames);

                                if (fld.ShowSelectButton)
                                {
                                    WebControl btn = this.GetButton(fld, "Select", keyArg, itemCount, i, fld.SelectText, fld.SelectImageUrl);
                                    td.Controls.Add(btn);
                                }

                                if (fld.ShowEditButton)
                                {
                                    WebControl btn = this.GetButton(fld, "Edit", keyArg, itemCount, i, fld.EditText, fld.EditImageUrl);
                                    td.Controls.Add(btn);
                                }

                                if (fld.ShowDeleteButton)
                                {
                                    WebControl btn = this.GetButton(fld, "Delete", keyArg, itemCount, i, fld.DeleteText, fld.DeleteImageUrl);
                                    td.Controls.Add(btn);
                                }
                            }
                            else if (column is ButtonField)
                            {
                                ButtonField fld = (column as ButtonField);

                                string keyArg = string.Empty;
                                if (this.DataKeyNames != null && this.DataKeyNames.Length > 0)
                                    keyArg = string.Join(",", this.DataKeyNames);

                                string btnText = dataStr = DataBinder.GetPropertyValue(dataItem, fld.DataTextField, fld.DataTextFormatString);
                                WebControl btn = this.GetButton(fld, fld.CommandName, keyArg, itemCount, i, btnText, fld.ImageUrl);
                                td.Controls.Add(btn);
                            }
                            else
                            {
                                td.Text = dataStr;
                            }
                        }

                        td.MergeStyle(column.ItemStyle);
                        if (curColCount > 0)
                            td.Style.Add("border-left", "solid 1px #ccccd3");
                        curColCount++;
                        tr.Cells.Add(td);
                    }

                    if (this.AutoGenerateColumns)
                    {
                        // If auto-generate columns is set to true, then create a column for each field in the data.
                        PropertyDescriptorCollection props = TypeDescriptor.GetProperties(dataItem);
                        for (int i = 0; i < props.Count; i++)
                        {
                            TableCell td = new TableCell();

                            if (null != props[i].GetValue(dataItem))
                                td.Text = props[i].GetValue(dataItem).ToString();

                            td.MergeStyle(this.ItemStyle);
                            if (curColCount > 0)
                                td.Style.Add("border-left", "solid 1px #ccccd3");
                            curColCount++;
                            tr.Cells.Add(td);
                        }
                    }

                    TableCell tdR = new TableCell();
                    tdR.CssClass = "GridViewLineRight";
                    tr.Cells.Add(tdR);

                    _rows.Add(tr);
                    this.OnItemCreated(new GridViewItemEventArgs(tr));
                    itemCount++;
                }
                //this.TotalRecordCount = recCount;
                //this.PageCount = (int)Math.Ceiling((double)this.TotalRecordCount / (double)this.PageSize);
            }
            this._rowCol = new GridViewRowCollection(this._rows);
            //this.ViewState["!_tblData"] = dataSource;

            GridViewRow ftr = this.BuildFooter();

            //if (this.CacheDuration > 0)
            //{
            //    System.Web.HttpContext.Current.Cache.Remove("GridViewObj" + this.UniqueID);
            //    System.Web.HttpContext.Current.Cache.Insert("GridViewObj" + this.UniqueID, dataSource, null, DateTime.Now.AddMinutes(this.CacheDuration), TimeSpan.Zero);
            //}

            return itemCount;
        }
        protected override void PerformSelect()
        {
            if (!this.IsBoundUsingDataSourceID)
                this.OnDataBinding(EventArgs.Empty);

            var view = this.GetData();
            view.Select(CreateDataSourceSelectArguments(),
                new DataSourceViewSelectCallback((IEnumerable retreivedData) =>
                {
                    if (this.IsBoundUsingDataSourceID)
                        this.OnDataBinding(EventArgs.Empty);
                    this.PerformDataBinding(retreivedData);
                })
            );
        }
        protected override void PerformDataBinding(IEnumerable data)
        {
            base.PerformDataBinding(data);
            if (data != null)
            {
                var pgData = data;
                if (this.AllowPaging)
                {
                    int maxPg, visRec, ttlRec;
                    pgData = RainstormStudios.Data.Linq.DataHelper.GetPaged(data, this.PageIndex, this.PageSize, out maxPg, out visRec, out ttlRec);
                    this.PageCount = maxPg;
                    this.VisibleRecordCount = visRec;
                    this.TotalRecordCount = ttlRec;
                }

                this.CreateChildControls(pgData, true);
            }

            this.RequiresDataBinding = false;
            this.MarkAsDataBound();
            this.OnDataBound(EventArgs.Empty);
        }
        protected GridViewRow BuildHeader()
        {
            GridViewRow tr = new GridViewRow(0, -1, DataControlRowType.Header, DataControlRowState.Normal);

            TableCell thTL = new TableCell();
            thTL.Style.Add(HtmlTextWriterStyle.Width, "6px");
            thTL.CssClass = "GridViewHeaderTL";
            tr.Cells.Add(thTL);

            for (int i = 0; i < this.Columns.Count; i++)
            {
                TableCell th = new TableCell();
                th.CssClass = "GridViewHeaderTC";

                if (this.Columns[i] is SortableField && ((this.Columns[i] as SortableField).ShowSort))
                {
                    ImageButton imgSort = new ImageButton();
                    imgSort.ID = "imgSort_" + i.ToString();
                    imgSort.CssClass = "GridViewHeaderSort";
                    imgSort.ImageUrl = this.Page.ClientScript.GetWebResourceUrl(typeof(RainstormStudios.Web.UI.WebControls.GridView), "RainstormStudios.Web.UI.WebControls.images.gridView.Sort.png");
                    imgSort.AlternateText = "";
                    imgSort.CommandArgument = (this.Columns[i] as BoundField).DataField;
                    imgSort.CommandName = "Sort";
                    imgSort.Command += new CommandEventHandler(this.imgSort_OnCommand);
                    th.Controls.Add(imgSort);
                    imgSort.Attributes.Add("name", imgSort.UniqueID);
                }

                bool usedTemplate = false;
                if (this.Columns[i] is System.Web.UI.WebControls.TemplateField)
                {
                    TemplateField fld = (this.Columns[i] as TemplateField);
                    ITemplate hdrTmpl = fld.HeaderTemplate;
                    if (hdrTmpl != null)
                    {
                        hdrTmpl.InstantiateIn(th);
                        usedTemplate = true;
                    }
                }

                if (!usedTemplate)
                {
                    // Standard field
                    Label lblHdr = new Label();
                    lblHdr.Text = this.Columns[i].HeaderText;
                    th.Controls.Add(lblHdr);
                }

                th.MergeStyle(this.Columns[i].HeaderStyle);
                tr.Cells.Add(th);
            }

            TableCell thTR = new TableCell();
            thTR.Style.Add(HtmlTextWriterStyle.Width, "6px");
            thTR.CssClass = "GridViewHeaderTR";
            tr.Cells.Add(thTR);

            this._hdrRow = tr;
            this.OnItemCreated(new GridViewItemEventArgs(tr));
            return tr;
        }
        protected GridViewRow BuildFooter()
        {
            GridViewRow tr = new GridViewRow(-1, -1, DataControlRowType.Footer, DataControlRowState.Normal);

            TableCell tdBL = new TableCell();
            tdBL.Style.Add(HtmlTextWriterStyle.Width, "6px");
            tdBL.CssClass = "GridViewFooterBL";
            tr.Cells.Add(tdBL);

            int colCount = this.HeaderRow.Cells.Count - 2;

            TableCell td = new TableCell();
            td.ID = "tdFooterControls";
            td.CssClass = "GridViewFooterBC";
            td.ColumnSpan = colCount;

            if (this.PagerSettings.Visible)
            {
                this._spanPgBtns = new Label();
                this._spanPgBtns.ID = "spanPgButtons";
                this._spanPgBtns.Style.Add("float", "right");
                this._spanPgBtns.Style.Add("margin-right", "20px");

                this._imgFPg = new ImageButton();
                this._imgFPg.ID = "imgFPg";
                this._imgFPg.CssClass = "FirstPg";
                this._imgFPg.ImageUrl = this.Page.ClientScript.GetWebResourceUrl(typeof(RainstormStudios.Web.UI.WebControls.GridView), "RainstormStudios.Web.UI.WebControls.images.gridView.FstPg.png");
                this._imgFPg.ImageAlign = ImageAlign.Middle;
                this._imgFPg.CommandName = "FirstPage";
                this._imgFPg.Command += new CommandEventHandler(this.imgPg_OnCommand);
                this._spanPgBtns.Controls.Add(this._imgFPg);

                this._imgPrevPg = new ImageButton();
                this._imgPrevPg.ID = "imgPrevPg";
                this._imgPrevPg.CssClass = "PrevPg";
                this._imgPrevPg.ImageUrl = this.Page.ClientScript.GetWebResourceUrl(typeof(RainstormStudios.Web.UI.WebControls.GridView), "RainstormStudios.Web.UI.WebControls.images.gridView.PrevPg.png");
                this._imgPrevPg.ImageAlign = ImageAlign.Middle;
                this._imgPrevPg.CommandName = "PrevPage";
                this._imgPrevPg.Command += new CommandEventHandler(this.imgPg_OnCommand);
                this._spanPgBtns.Controls.Add(this._imgPrevPg);

                Label lblPageNum = new Label();
                lblPageNum.ID = "lblPageNum";
                lblPageNum.Width = new Unit("50px");
                lblPageNum.Text = string.Format("{0} / {1}", this.PageIndex + 1, this.PageCount);
                lblPageNum.Style.Add(HtmlTextWriterStyle.TextAlign, "center");
                this._spanPgBtns.Controls.Add(lblPageNum);

                this._imgNextPg = new ImageButton();
                this._imgNextPg.ID = "imgNextPg";
                this._imgNextPg.CssClass = "NextPg";
                this._imgNextPg.ImageUrl = this.Page.ClientScript.GetWebResourceUrl(typeof(RainstormStudios.Web.UI.WebControls.GridView), "RainstormStudios.Web.UI.WebControls.images.gridView.NextPg.png");
                this._imgNextPg.ImageAlign = ImageAlign.Middle;
                this._imgNextPg.CommandName = "NextPage";
                this._imgNextPg.Command += new CommandEventHandler(this.imgPg_OnCommand);
                this._spanPgBtns.Controls.Add(this._imgNextPg);

                this._imgLastPg = new ImageButton();
                this._imgLastPg.ID = "imgLastPg";
                this._imgLastPg.CssClass = "LastPg";
                this._imgLastPg.ImageUrl = this.Page.ClientScript.GetWebResourceUrl(typeof(RainstormStudios.Web.UI.WebControls.GridView), "RainstormStudios.Web.UI.WebControls.images.gridView.LstPg.png");
                this._imgLastPg.ImageAlign = ImageAlign.Middle;
                this._imgLastPg.CommandName = "LastPage";
                this._imgLastPg.Command += new CommandEventHandler(this.imgPg_OnCommand);
                this._spanPgBtns.Controls.Add(this._imgLastPg);

                td.Controls.Add(this._spanPgBtns);
            }

            Label spanPageSz = new Label();
            spanPageSz.ID = "spanPageSz";
            spanPageSz.Style.Add("margin-left", "20px");

            this._drpPageSz = new DropDownList();
            this._drpPageSz.ID = "drpPageSzSelect";
            this._drpPageSz.AutoPostBack = true;
            this._drpPageSz.SelectedIndexChanged += new EventHandler(drpPageSz_SelectedIndexChanged);
            this._drpPageSz.Items.Add(new ListItem("10", "10"));
            this._drpPageSz.Items.Add(new ListItem("25", "25"));
            this._drpPageSz.Items.Add(new ListItem("50", "50"));
            this._drpPageSz.Items.Add(new ListItem("100", "100"));
            spanPageSz.Controls.Add(this._drpPageSz);
            td.Controls.Add(spanPageSz);

            Label lblRecVis = new Label();
            lblRecVis.ID = "lblRecordsCount";
            lblRecVis.Style.Add("margin-left", "20px");
            lblRecVis.Text = string.Format("Displaying {0} of {1} records.", this.VisibleRecordCount, this.TotalRecordCount);
            td.Controls.Add(lblRecVis);
            tr.Cells.Add(td);

            TableCell tdBR = new TableCell();
            tdBR.Style.Add(HtmlTextWriterStyle.Width, "6px");
            tdBR.CssClass = "GridViewFooterBR";
            tr.Cells.Add(tdBR);

            if (this._pgrSettings.Visible)
            {
                this._imgFPg.Attributes.Add("name", this._imgFPg.UniqueID);
                this._imgPrevPg.Attributes.Add("name", this._imgPrevPg.UniqueID);
                this._imgNextPg.Attributes.Add("name", this._imgNextPg.UniqueID);
                this._imgLastPg.Attributes.Add("name", this._imgLastPg.UniqueID);
            }
            this._drpPageSz.Attributes.Add("name", this._drpPageSz.UniqueID);

            this._ftrRow = tr;
            this.OnItemCreated(new GridViewItemEventArgs(tr));
            return tr;
        }
        private WebControl GetButton(DataControlField fld, string commandName, string commandArg, int itemIdx, int colIdx, string buttonText, string imgUrl)
        {
            ButtonType btnType = ButtonType.Button;
            if (fld is CommandField)
                btnType = ((CommandField)fld).ButtonType;
            else if (fld is ButtonField)
                btnType = ((ButtonField)fld).ButtonType;
            else
                throw new Exception("Unrecognized DataControlField type: " + fld.GetType().Name);


            switch (btnType)
            {
                case ButtonType.Button:
                    {
                        Button btn = new Button();
                        btn.ID = string.Format("btn{0}_{1}_{2}", commandName, itemIdx, colIdx);
                        btn.Text = buttonText;
                        btn.CommandName = commandName;
                        btn.CommandArgument = commandArg;

                        return btn;
                    }
                case ButtonType.Link:
                    {
                        LinkButton lnk = new LinkButton();
                        lnk.ID = string.Format("lnk{0}_{1}_{2}", commandName, itemIdx, colIdx);
                        lnk.Text = buttonText;
                        lnk.CommandName = commandName;
                        lnk.CommandArgument = commandArg;

                        return lnk;
                    }
                case ButtonType.Image:
                    {
                        ImageButton img = new ImageButton();
                        img.ID = string.Format("img{0}_{1}_{2}", commandName, itemIdx, colIdx);
                        img.ImageUrl = imgUrl;
                        img.CommandName = commandName;
                        img.CommandArgument = commandArg;

                        return img;
                    }
                default:
                    throw new Exception("Unrecognized control ButtonType: " + btnType.ToString());
            }
        }
        private void ChangePage(int newIdx)
        {
            GridViewPageEventArgs pgea = new GridViewPageEventArgs(newIdx);
            this.OnPageIndexChanging(pgea);
            if (pgea.Cancel)
                return;

            this.PageIndex = newIdx;

            this.OnPageIndexChanged(EventArgs.Empty);
        }
        //***************************************************************************
        // Event Triggers
        // 
        protected void OnSortClicked(CommandEventArgs e)
        {
            if (this.SortClicked != null)
                this.SortClicked.Invoke(this, e);
        }
        protected void OnFirstPageClicked(EventArgs e)
        {
            if (this.FirstPageClicked != null)
                this.FirstPageClicked.Invoke(this, e);

            this.ChangePage(0);
        }
        protected void OnLastPageClicked(EventArgs e)
        {
            if (this.LastPageClicked != null)
                this.LastPageClicked.Invoke(this, e);

            this.ChangePage(this.PageCount - 1);
        }
        protected void OnPreviousPageClicked(EventArgs e)
        {
            if (this.PageIndex > 0)
            {
                if (this.PreviousPageClicked != null)
                    this.PreviousPageClicked.Invoke(this, e);

                this.ChangePage(this.PageIndex - 1);
            }
        }
        protected void OnNextPageClicked(EventArgs e)
        {
            if (this.PageIndex < this.PageCount - 1)
            {
                if (this.NextPageClicked != null)
                    this.NextPageClicked.Invoke(this, e);

                this.ChangePage(this.PageIndex + 1);
            }
        }
        protected void OnPageSizeChanged(EventArgs e)
        {
            if (this.PageSizeChanged != null)
                this.PageSizeChanged.Invoke(this, e);
        }
        protected void OnPageIndexChanging(GridViewPageEventArgs e)
        {
            if (this.PageIndexChanging != null)
                this.PageIndexChanging.Invoke(this, e);
        }
        protected void OnPageIndexChanged(EventArgs e)
        {
            if (this.PageIndexChanged != null)
                this.PageIndexChanged.Invoke(this, e);
        }
        protected void OnItemCreated(GridViewItemEventArgs e)
        {
            if (this.ItemCreated != null)
                this.ItemCreated.Invoke(this, e);
        }
        protected void OnItemCommand(GridViewItemCommandEventArgs e)
        {
            if (this.ItemCommand != null)
                this.ItemCommand.Invoke(this, e);
        }
        protected void OnItemDataBound(GridViewItemEventArgs e)
        {
            if (this.ItemDataBound != null)
                this.ItemDataBound.Invoke(this, e);
        }
        #endregion

        #region Event Handlers
        //***************************************************************************
        // Event Handlers
        // 
        private void imgSort_OnCommand(object sender, CommandEventArgs e)
        {
            this.OnSortClicked(e);
        }
        private void imgPg_OnCommand(object sender, CommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "FirstPage":
                    this.OnFirstPageClicked(EventArgs.Empty);
                    break;
                case "LastPage":
                    this.OnLastPageClicked(EventArgs.Empty);
                    break;
                case "PrevPage":
                    this.OnPreviousPageClicked(EventArgs.Empty);
                    break;
                case "NextPage":
                    this.OnNextPageClicked(EventArgs.Empty);
                    break;
            }
        }
        private void drpPageSz_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList drpPgSz = (sender as DropDownList);

            if (drpPgSz != null)
            {
                this.PageSize = int.Parse(drpPgSz.SelectedValue);
                this.OnPageSizeChanged(e);
            }
            else
                throw new Exception("Unable to determine page size: cannot cast sender as DropDownList.");
        }
        private void GridView_PageIndexChanged(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }
        private void GridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            //throw new NotImplementedException();
        }
        private void GridView_Sorting(object sender, GridViewSortEventArgs e)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
    public class GridViewDesigner : System.Web.UI.Design.ControlDesigner
    {
        public override string GetDesignTimeHtml()
        {
            return base.GetDesignTimeHtml();
        }
    }
    public class SortableField : System.Web.UI.WebControls.BoundField
    {
        private bool
            _canSort = true;

        public bool ShowSort
        {
            get { return this._canSort; }
            set { this._canSort = value; }
        }
    }
    public delegate void GridViewItemEventHandler(object sender, GridViewItemEventArgs e);
    public class GridViewItemEventArgs : EventArgs
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private GridViewRow
            _row;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public GridViewRow Item
        { get { return this._row; } }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public GridViewItemEventArgs(GridViewRow row)
        {
            this._row = row;
        }
        #endregion
    }
    public delegate void GridViewItemCommandEventHandler(object sender, GridViewItemCommandEventArgs e);
    public class GridViewItemCommandEventArgs : CommandEventArgs
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private GridViewRow
            _row;
        private object
            _cmdSrc;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public GridViewRow Item
        { get { return this._row; } }
        public object CommandSource
        { get { return this._cmdSrc; } }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public GridViewItemCommandEventArgs(GridViewRow row, object cmdSrc, CommandEventArgs cea)
            : base(cea)
        {
            this._row = row;
            this._cmdSrc = cmdSrc;
        }
        #endregion
    }
}
