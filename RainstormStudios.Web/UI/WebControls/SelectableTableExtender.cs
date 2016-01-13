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
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

[assembly: System.Web.UI.WebResource("RainstormStudios.Web.UI.WebControls.scripts.selectableTable.js", "text/javascript", PerformSubstitution = false)]

namespace RainstormStudios.Web.UI.WebControls
{
    /// <summary>
    /// Provides client-side functionality for making the rows of a generic HTML table dynamically selectable.  This requires JQuery be included on the page.
    /// </summary>
    [DefaultProperty("TableID"), ParseChildren(false), ToolboxData("<{0}:SelectableTableExtender runat=\"server\"></{0}:SelectableTableExtender>")]
    public class SelectableTableExtender : System.Web.UI.ExtenderControl
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private bool
            _rowIdxCleared = false;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properites
        // 
        public string HoverStyleClass
        {
            get { return (string)this.ViewState["HoverStyleClass"]; }
            set { this.ViewState["HoverStyleClass"] = value; }
        }
        public string SelectedStyleClass
        {
            get
            {
                string selStyle = (string)this.ViewState["SelectedStyleClass"];
                return (string.IsNullOrEmpty(selStyle) ? this.HoverStyleClass : selStyle);
            }
            set { this.ViewState["SelectedStyleClass"] = value; }
        }
        public string TableContainerID
        {
            get
            {
                string cntrID = (string)this.ViewState["TableContainerID"];
                return (string.IsNullOrEmpty(cntrID) ? this.TableID : cntrID);
            }
            set { this.ViewState["TableContainerID"] = value; }
        }
        public string TableID
        {
            get { return (string)this.ViewState["TableID"]; }
            set { this.ViewState["TableID"] = value; }
        }
        public string ClickPostbackControlID
        {
            get { return (string)this.ViewState["ClickPostbackControlID"]; }
            set { this.ViewState["ClickPostbackControlID"] = value; }
        }
        public bool SelectRowOnClick
        {
            get
            {
                object vsVal = this.ViewState["SelectRowOnClick"];
                bool bVsVal;
                if (vsVal == null || !bool.TryParse(vsVal.ToString(), out bVsVal))
                    return true;
                else
                    return bVsVal;
            }
            set { this.ViewState["SelectRowOnClick"] = value; }
        }
        //***************************************************************************
        // Private Properties
        // 
        private int SelectedRowIdx
        {
            get
            {
                object vsVal = this.ViewState["SelectedRowIdx"];
                int iVsVal;
                if (vsVal == null || !int.TryParse(vsVal.ToString(), out iVsVal))
                    return -1;
                else
                    return iVsVal;
            }
            set { this.ViewState["SelectedRowIdx"] = value; }
        }
        private string HiddenFieldName
        { get { return "hdnRowSelIdx_" + this.ClientID; } }
        private string JavaScriptKeyName
        { get { return "SelectableTableStartup_" + this.ClientID; } }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public SelectableTableExtender()
        { }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public void ClearSelectedRow()
        {
            this.SelectedRowIdx = -1;
            this._rowIdxCleared = true;
        }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        protected override IEnumerable<ScriptReference> GetScriptReferences()
        {
            yield return new ScriptReference("RainstormStudios.Web.UI.WebControls.scripts.gridRowHover.js", this.GetType().Assembly.FullName);
        }
        protected override IEnumerable<ScriptDescriptor> GetScriptDescriptors(Control targetControl)
        {
            ScriptControlDescriptor descriptor = new ScriptControlDescriptor("RainstormStudios.Web.UI.WebControls.SelectableTableExtender", targetControl.ClientID);
            descriptor.AddProperty("hoverStyleClass", this.HoverStyleClass);
            descriptor.AddProperty("selectedStyleClass", this.SelectedStyleClass);
            descriptor.AddProperty("containerID", this.TableContainerID);
            descriptor.AddProperty("tableID", this.TableID);
            descriptor.AddProperty("postbackCtrlID", this.ClickPostbackControlID);
            descriptor.AddProperty("selectRowOnClick", this.SelectRowOnClick);
            descriptor.AddProperty("selectedRowIdx", this.SelectRowOnClick);
            yield return descriptor;
        }
        //protected override void OnInit(EventArgs e)
        //{
        //    base.OnInit(e);
        //}
        //protected override void OnPreRender(EventArgs e)
        //{
        //    if (string.IsNullOrEmpty(this.TableID))
        //        throw new Exception("You must specify the ID of the HTML table you want the effects applied to.");
        //    if (string.IsNullOrEmpty(this.HoverStyleClass))
        //        throw new Exception("You must specify the hover style CSS class name.");

        //    //string scriptPathRef = this.Page.ClientScript.GetWebResourceUrl(typeof(RainstormStudios.Web.UI.WebControls.SelectableTableExtender), "RainstormStudios.Web.UI.WebControls.scripts.gridRowHover.js");
        //    //this.Page.ClientScript.RegisterClientScriptInclude(this.Page.GetType(), "SelectableTableJS", scriptPathRef);
        //    this.Page.ClientScript.RegisterClientScriptResource(typeof(RainstormStudios.Web.UI.WebControls.SelectableTableExtender), "RainstormStudios.Web.UI.WebControls.scripts.gridRowHover.js");

        //    StringBuilder sb = new StringBuilder();
        //    sb.AppendLine("$(document).ready(function () {");
        //    sb.AppendFormat("setTableSelectable('{0}', '{1}', '{2}', '{3}', '{4}', '{5}'", this.TableContainerID, this.TableID, HiddenFieldName, this.HoverStyleClass, this.SelectedStyleClass, this.SelectRowOnClick);
        //    if (!string.IsNullOrEmpty(this.ClickPostbackControlID))
        //        sb.AppendFormat(", '{0}'", this.ClickPostbackControlID);
        //    sb.AppendLine(");");
        //    sb.AppendLine("});");
        //    this.Page.ClientScript.RegisterClientScriptBlock(this.Page.GetType(), this.JavaScriptKeyName, sb.ToString(), true);

        //    base.OnPreRender(e);
        //}
        //protected override void Render(HtmlTextWriter writer)
        //{
        //    if (this.SelectRowOnClick)
        //    {
        //        if (!this._rowIdxCleared)
        //        {
        //            int iVal;
        //            if (int.TryParse(this.Page.Request.Form[HiddenFieldName], out iVal))
        //                this.SelectedRowIdx = iVal;
        //        }
        //        this.Page.ClientScript.RegisterHiddenField(HiddenFieldName, this.SelectedRowIdx.ToString());
        //    }
        //    base.Render(writer);
        //}
        #endregion
    }
}
