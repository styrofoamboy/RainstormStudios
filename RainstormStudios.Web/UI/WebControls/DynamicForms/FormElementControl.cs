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
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RainstormStudios.Drawing;

namespace RainstormStudios.Web.UI.WebControls.DynamicForms
{
    /// <summary>
    /// Serves as a base class for all of the Dynamic Form element control types.
    /// This class cannot be instantiated directly.  It uses a simple factory model.  To create a new control instance, use "FormElementControl.GetControl(FormElementData)".
    /// </summary>
    public abstract class FormElementControl : System.Web.UI.WebControls.CompositeControl, INamingContainer, IScriptControl
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        ScriptManager
            _sm;
        protected FormElementData
            _qData;
        bool
            _editMode,
            _autoPostback,
            _useValExtender;
        Control
            _validationControl,
            _answerControl;
        Panel
            _container;
        string
            _clientAnsChanged,
            _clientVisChanged,
            _clientFocus,
            _clientBlur;
        DynamicForm
            _owner;
        //***************************************************************************
        // Public Events
        // 
        public event DynamicFormElementRenderingEventHandler
            ElementRendering;
        public event DynamicFormElementEventHandler
            ElementRendered;
        public event DynamicFormElementEventHandler
            AnswerChanged;
        public event DynamicFormElementEventHandler
            VisibilityChanged;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public FormElementData DataContext
        { get { return this._qData; } }
        public object ElementProviderKey
        { get { return this._qData.ElementProviderKey; } }
        public string Text
        { get { return this._qData.Text; } }
        public string HintText
        { get { return this._qData.HintText; } }
        public string Suffix
        { get { return this._qData.Suffix; } }
        public bool Required
        { get { return this._qData.Required; } }
        public override Unit Width
        { get { return this._qData.ElementWidth; } }
        public override Unit Height
        { get { return this._qData.ElementHeight; } }
        public override System.Drawing.Color ForeColor
        { get { return this._qData.ForeColor; } }
        public override System.Drawing.Color BackColor
        { get { return this._qData.BackColor; } }
        public override FontInfo Font
        { get { return this._qData.Font; } }
        public override System.Drawing.Color BorderColor
        { get { return this._qData.BorderColor; } }
        public override BorderStyle BorderStyle
        { get { return this._qData.BorderStyle; } }
        public override Unit BorderWidth
        { get { return this._qData.BorderWidth; } }
        public override string CssClass
        { get { return this._qData.CssClass; } }
        public int ColumnCount
        { get { return this._qData.ColumnCount; } }
        public int RowCount
        { get { return this._qData.RowCount; } }
        public Unit MarginLeft
        { get { return this._qData.MarginLeft; } }
        public Unit MarginRight
        { get { return this._qData.MarginRight; } }
        public FormElementDataType DataType
        { get { return this._qData.DataType; } }
        public FormElementDisplayType DisplayType
        { get { return this._qData.DisplayType; } }
        public FormElementDisplayOptions DisplayOptions
        { get { return this._qData.DisplayOptions; } }
        public string NavigationUrl
        { get { return this._qData.NavigationUrl; } }
        public string ImageUrl
        { get { return this._qData.ImageUrl; } }
        public ImageAlign ImageAlignment
        { get { return this._qData.ImageAlignment; } }
        public bool EditMode
        {
            get { return this._editMode; }
            set { this._editMode = value; }
        }
        public bool AutoPostback
        {
            get { return this._autoPostback; }
            set { this._autoPostback = value; }
        }
        public bool UseValidatorControlExtender
        {
            get { return this._useValExtender; }
            set { this._useValExtender = value; }
        }
        public override bool Visible
        {
            get { return base.Visible; }
            set
            {
                if (value != base.Visible)
                {
                    base.Visible = value;
                    this.OnVisibilityChanged(new DynamicFormElementEventArgs(this, this._qData));
                }
            }
        }
        public string OnClientAnswerChanged
        {
            get { return this._clientAnsChanged; }
            set { this._clientAnsChanged = value; }
        }
        public string OnClientVisibilityChanged
        {
            get { return this._clientVisChanged; }
            set { this._clientVisChanged = value; }
        }
        public string OnClientFocus
        {
            get { return this._clientFocus; }
            set { this._clientFocus = value; }
        }
        public string OnClientBlur
        {
            get { return this._clientBlur; }
            set { this._clientBlur = value; }
        }
        public FormElementUserInput UserInput
        { get { return this.GetCurrentAnswer(); } }
        public DynamicForm Owner
        {
            get { return this._owner; }
            internal set { this._owner = value; }
        }
        //***************************************************************************
        // Private Properties
        // 
        protected override System.Web.UI.HtmlTextWriterTag TagKey
        { get { return System.Web.UI.HtmlTextWriterTag.Div; } }
        protected Control ValidationControl
        {
            get { return this._validationControl; }
            set { this._validationControl = value; }
        }
        protected Control AnswerControl
        {
            get { return this._answerControl; }
            set { this._answerControl = value; }
        }
        protected Panel ControlContainer
        {
            get { return this._container; }
            set { this._container = value; }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        protected FormElementControl()
            : base()
        {
            this._qData = new FormElementData();
        }
        protected FormElementControl(FormElementData qData)
            : this()
        {
            this._qData = qData;
            this.ID = "formElementCtrl_" + this._qData.ElementProviderKey.ToString();
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public void SaveInput()
        {
            this.Owner.Provider.SaveFormElementInput(this.UserInput);
        }
        //***************************************************************************
        // Static Methods
        // 
        public static Panel CreateControlContainer(FormElementData data, bool editMode)
        {
            Panel pnl = new Panel();
            pnl.ID = "pnlElementContainer_" + data.ElementProviderKey.ToString();
            pnl.CssClass = "dynamicFormElementContainer";
            pnl.Width = data.ElementWidth;
            pnl.Height = data.ElementHeight;
            pnl.BorderStyle = data.BorderStyle;
            pnl.BorderColor = data.BorderColor;
            pnl.BorderWidth = data.BorderWidth;
            pnl.BackColor = data.BackColor;
            pnl.ForeColor = data.ForeColor;
            if (editMode)
                pnl.Style.Add(HtmlTextWriterStyle.Padding, "4px");
            return pnl;
        }
        public static FormElementControl GetControl(FormElementData data)
        {
            switch (data.DisplayType)
            {
                case FormElementDisplayType.Grid:
                    return new FormElementGrid(data);

                case FormElementDisplayType.GridCell:
                    return new FormElementGridCell(data);

                case FormElementDisplayType.CheckBoxes:
                    return new FormElementCheckBoxList(data);

                case FormElementDisplayType.DropdownList:
                    return new FormElementDropDownList(data);

                case FormElementDisplayType.HiddenField:
                    return new FormElementHidden(data);

                case FormElementDisplayType.Image:
                    return new FormElementImage(data);

                case FormElementDisplayType.Message:
                    return new FormElementMessage(data);

                case FormElementDisplayType.RadioButtons:
                    return new FormElementRadioButtonList(data);

                case FormElementDisplayType.Textbox:
                    return new FormElementTextbox(data);

                case FormElementDisplayType.Url:
                    return new FormElementLink(data);

                default:
                    throw new Exception("Cannot determine dynamic form control. Unrecognized display type: " + data.DisplayType.ToString());
            }
        }
        public static void AddElementStyleAttributes(HtmlTextWriter writer, FormElementData data)
        {
            if (data.MarginLeft != Unit.Empty)
                writer.AddStyleAttribute(HtmlTextWriterStyle.MarginLeft, data.MarginLeft.ToString());
            if (data.MarginRight != Unit.Empty)
                writer.AddStyleAttribute(HtmlTextWriterStyle.MarginRight, data.MarginRight.ToString());
            if (data.BorderStyle != BorderStyle.None)
            {
                writer.AddStyleAttribute(HtmlTextWriterStyle.BorderStyle, data.BorderStyle.ToString());
                writer.AddStyleAttribute(HtmlTextWriterStyle.BorderWidth, data.BorderWidth.ToString());
                writer.AddStyleAttribute(HtmlTextWriterStyle.BorderColor, data.BorderColor.GetWebColor());
            }
            if (data.ForeColor != System.Drawing.Color.Empty)
                writer.AddStyleAttribute(HtmlTextWriterStyle.Color, data.ForeColor.GetWebColor());
            if (data.BackColor != System.Drawing.Color.Empty)
                writer.AddStyleAttribute(HtmlTextWriterStyle.BackgroundColor, data.BackColor.GetWebColor());
            if (data.ElementWidth != Unit.Empty)
                writer.AddStyleAttribute(HtmlTextWriterStyle.Width, data.ElementWidth.ToString());
            if (data.ElementHeight != Unit.Empty)
                writer.AddStyleAttribute(HtmlTextWriterStyle.Height, data.ElementHeight.ToString());
            if (!string.IsNullOrEmpty(data.CssClass))
                writer.AddAttribute(HtmlTextWriterAttribute.Class, data.CssClass);
        }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        protected override void OnPreRender(EventArgs e)
        {
            if (!this.DesignMode)
            {
                this._sm = ScriptManager.GetCurrent(this.Page);

                if (this._sm == null)
                    throw new HttpException("A ScriptManager control must exist on the current page.");

                _sm.RegisterScriptControl(this);
            }

            base.OnPreRender(e);
        }
        protected override void Render(HtmlTextWriter writer)
        {
            this.EnsureChildControls();

            if (this._qData.DisplayType != FormElementDisplayType.HiddenField)
            {
                if (!string.IsNullOrEmpty(this._qData.HintText))
                    this.ControlContainer.Controls.Add(new LiteralControl(string.Format("<span class=\"dynamicFormElementHintText\">{0}</span>", this._qData.HintText)));

                if (!string.IsNullOrEmpty(this._qData.Text))
                    this.ControlContainer.Controls.AddAt(0, new LiteralControl(string.Format("<span class=\"dynamicFormElementText\">{0}</span>", this._qData.Text)));
            }

            writer.BeginRender();
            try
            {
                //this.ControlContainer.RenderControl(writer);
                this.RenderContents(writer);


            }
            finally
            { writer.EndRender(); }

            //base.Render(writer);
        }
        protected override void CreateChildControls()
        {
            if (this.EditMode)
            {
                // Build out the edit controls.
            }

            if (this.ValidationControl != null)
            {
                if (this.DisplayType == FormElementDisplayType.Textbox && !string.IsNullOrEmpty(this.DataType.ValidationString))
                {
                    // Build out the regex validator.
                    RegularExpressionValidator rgx = new RegularExpressionValidator();
                    rgx.ID = "RgxValidator_" + this.ElementProviderKey.ToString();
                    rgx.EnableClientScript = true;
                    rgx.Display = (this.UseValidatorControlExtender ? ValidatorDisplay.None : ValidatorDisplay.Dynamic);
                    rgx.ControlToValidate = this.ValidationControl.ID;
                    rgx.ErrorMessage = "This is not a valid entry.  Please enter a valid " + this.DataType.DataTypeName + ".";
                    rgx.ForeColor = System.Drawing.Color.Red;
                    rgx.ValidationExpression = this.DataType.ValidationString;
                    rgx.ValidationGroup = "DynamicForm_" + this._qData.FormProviderKey.ToString();
                    this.Controls.Add(rgx);

                    if (this.UseValidatorControlExtender)
                    {
                        AjaxControlToolkit.ValidatorCalloutExtender rgxExt = new AjaxControlToolkit.ValidatorCalloutExtender();
                        rgxExt.ID = "RgxExtender_" + this.ElementProviderKey.ToString();
                        rgxExt.TargetControlID = rgx.ID;

                    }
                }

                if (this.Required)
                {
                    // Build out the required field validator.
                    RequiredFieldValidator req = new RequiredFieldValidator();
                    req.ID = "ReqValidator_" + this.ElementProviderKey.ToString();
                    req.EnableClientScript = true;
                    req.Display = (this.UseValidatorControlExtender ? ValidatorDisplay.None : ValidatorDisplay.Dynamic);
                    req.ControlToValidate = this.ValidationControl.ID;
                    req.ErrorMessage = "This field is required.";
                    req.ForeColor = System.Drawing.Color.Red;
                    req.ValidationGroup = "DynamicForm_" + this._qData.FormProviderKey.ToString();
                    this.Controls.Add(req);
                }
            }

            base.CreateChildControls();
        }
        protected virtual IEnumerable<ScriptDescriptor> GetScriptDescriptors()
        {
            ScriptControlDescriptor descriptor = new ScriptControlDescriptor("RainstormStudios.Web.UI.WebControls.DynamicForm.FormElementControl", this.ClientID);
            descriptor.AddProperty("onClientVisibilityChanged", this.OnClientVisibilityChanged);
            descriptor.AddProperty("onClientAnswerChanged", this.OnClientAnswerChanged);
            descriptor.AddProperty("onClientFocus", this.OnClientFocus);
            descriptor.AddProperty("onClientBlur", this.OnClientBlur);
            descriptor.AddProperty("answerControlID", (this.AnswerControl != null ? this.AnswerControl.ID : ""));
            yield return descriptor;
        }
        protected virtual IEnumerable<ScriptReference> GetScriptReferences()
        {
            //yield return new ScriptReference("RainstormStudios.Web.UI.WebControls.scripts.DynamicForms.dynamicFormElementControl.js", this.GetType().Assembly.FullName);
            yield return new ScriptReference("RainstormStudios.Web.UI.WebControls.scripts.dynamicForms.dynamicFormElementControl.js", typeof(RainstormStudios.Web.UI.WebControls.DynamicForms.FormElementControl).Assembly.FullName);
        }
        IEnumerable<ScriptReference> IScriptControl.GetScriptReferences()
        {
            return this.GetScriptReferences();
        }
        IEnumerable<ScriptDescriptor> IScriptControl.GetScriptDescriptors()
        {
            return this.GetScriptDescriptors();
        }
        private FormElementUserInput GetCurrentAnswer()
        {
            Control ansCtrl = this.AnswerControl;
            if (ansCtrl == null)
                // If we don't know which control is the answer control (or there
                //   isn't one), just return null.
                return null;

            string answerText = string.Empty;
            List<FormElementDataAnswer> selectedAnswers = new List<FormElementDataAnswer>();
            if (ansCtrl is CheckBoxList)
            {
                // we have to handle the check box list differently than all other
                //   control types, since multiple answers can be selected
                //   simultaneously.
                CheckBoxList chkLst = (CheckBoxList)ansCtrl;
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                for (int i = 0; i < chkLst.Items.Count; i++)
                    if (chkLst.Items[i].Selected)
                    {
                        selectedAnswers.AddRange(this._qData.Answers.Where(a => a.AnswerProviderKey.ToString() == chkLst.Items[i].Value));
                        sb.AppendFormat(", {0}", chkLst.Items[i].Text);
                    }
                answerText = sb.ToString().TrimStart(',', ' ');
            }

            else if (ansCtrl is ListControl)
            {
                ListControl lstCtrl = ((ListControl)ansCtrl);
                selectedAnswers.AddRange(this._qData.Answers.Where(a => a.AnswerProviderKey.ToString() == lstCtrl.SelectedValue));
                answerText = lstCtrl.SelectedItem.Text;
            }

            else if (ansCtrl is TextBox)
                answerText = ((TextBox)ansCtrl).Text;

            else if (ansCtrl is CheckBox)
                answerText = ((CheckBox)ansCtrl).Checked.ToString();

            else if (ansCtrl is HiddenField)
                // Even though this isn't *technically* user-entered data, we're going
                //   to treat it as such, rather than risk 'losing' the answer when
                //   saving the form data.
                answerText = ((HiddenField)ansCtrl).Value;

            else
                // All other display types have no "user selectable" answer.
                selectedAnswers = null;

            return new FormElementUserInput(this, this._qData, answerText, selectedAnswers.ToArray());
        }
        #endregion

        #region Event Handlers
        //***************************************************************************
        // Event Triggers
        // 
        protected virtual void OnVisibilityChanged(DynamicFormElementEventArgs e)
        {
            if (this.VisibilityChanged != null)
                this.VisibilityChanged.Invoke(this, e);
        }
        protected virtual void OnAnswerChanged(DynamicFormElementEventArgs e)
        {
            if (this.AnswerChanged != null)
                this.AnswerChanged.Invoke(this, e);
        }
        protected virtual void OnElementRendering(DynamicFormElementRenderingEventArgs e)
        {
            if (this.ElementRendering != null)
                this.ElementRendering.Invoke(this, e);
        }
        protected virtual void OnElementRendered(DynamicFormElementEventArgs e)
        {
            if (this.ElementRendered != null)
                this.ElementRendered.Invoke(this, e);
        }
        //***************************************************************************
        // Event Handlers
        // 
        protected void SelectedIndexChanged(object sender, EventArgs e)
        {
            this.OnAnswerChanged(new DynamicFormElementEventArgs(this, this._qData));
        }
        protected void TextChanged(object sender, EventArgs e)
        {
            this.OnAnswerChanged(new DynamicFormElementEventArgs(this, this._qData));
        }
        #endregion
    }
}
