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
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RainstormStudios.Web.UI.WebControls.DynamicForms;

namespace RainstormStudios.Web.UI.WebControls
{
    [Author("Unfried, Michael")]
    public class DynamicForm : CompositeControl, INamingContainer, IScriptControl, IPostBackDataHandler, IPostBackEventHandler
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private string
            _providerName;
        private Providers.DynamicFormProvider
            _provider;
        private ScriptManager
            _sm;
        private object
            _frmProviderKey;
        private string
            _clientElementAnswerChanged,
            _clientElementRendering,
            _clientElementRendered,
            _clientElementVisibilityChanged;
        private bool
            _formSaved = false;
        //***************************************************************************
        // Public Events
        // 
        public event DynamicForms.DynamicFormElementEventHandler
            ElementAnswerChanged;
        public event DynamicForms.DynamicFormElementRenderingEventHandler
            ElementRendering;
        public event DynamicForms.DynamicFormElementEventHandler
            ElementRendered;
        public event DynamicForms.DynamicFormElementEventHandler
            ElementVisibilityChanged;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public string ProviderName
        {
            get { return this._providerName; }
            set { this._providerName = value; }
        }
        public Providers.DynamicFormProvider Provider
        { get { return this._provider; } }
        public object FormProviderKey
        {
            get { return this.ViewState["FormProviderKey"]; }
            set { this.ViewState["FormProviderKey"] = value; }
        }
        public string OnClientElementAnswerChanged
        {
            get { return this._clientElementAnswerChanged; }
            set { this._clientElementAnswerChanged = value; }
        }
        public string OnClientElementRendering
        {
            get { return this._clientElementRendering; }
            set { this._clientElementRendering = value; }
        }
        public string OnClientElementRendered
        {
            get { return this._clientElementRendered; }
            set { this._clientElementRendered = value; }
        }
        public string OnClientElementVisibilityChanged
        {
            get { return this._clientElementVisibilityChanged; }
            set { this._clientElementVisibilityChanged = value; }
        }
        //***************************************************************************
        // Private Properties
        // 
        protected override HtmlTextWriterTag TagKey
        { get { return HtmlTextWriterTag.Div; } }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public DynamicForm()
            : base()
        { }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public void RaisePostBackEvent(string data)
        {
        }
        public void RaisePostDataChangedEvent()
        {
        }
        public bool LoadPostData(string data, System.Collections.Specialized.NameValueCollection vals)
        {
            return false;
        }
        public virtual object[] SaveFormData(object associatedKey)
        {
            try
            {
#if DEBUG
                DateTime dtStart = DateTime.Now;
#endif
                FormElementUserInput[] inputs = this.GetControlUserInput(this.Controls);
#if DEBUG
                DateTime dtEnd = DateTime.Now;
                TimeSpan tsTaken = dtEnd.Subtract(dtStart);
                System.Diagnostics.Debug.Write("Get User Input - Time Taken: " + tsTaken.ToString());
#endif

                object[] vals = this.Provider.SaveForm(inputs, associatedKey);

                this._formSaved = true;
                return vals;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occured while trying to save form data.", ex);
            }
        }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            // Load provider.
            if (!string.IsNullOrEmpty(this._providerName))
                this._provider = Providers.DynamicFormProviderManager.Providers[this._providerName];
            else
                this._provider = Providers.DynamicFormProviderManager.Provider;

            if (this._provider == null)
                throw new Exception("Specified dynamic form provider name not found or no default provider.");
        }
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
        protected override void CreateChildControls()
        {
            //FormElementData[] data = this._provider.GetFormElements(this._frmProviderKey);
            FormElementData[] data = this._provider.GetFormElements(this.FormProviderKey);

            if (data != null)
            {
                // This is just the top-level container.  The provider is responsible for all the data, and the
                //   controls are responsible for processing that data and generating their children.
                for (int i = 0; i < data.Length; i++)
                {
                    FormElementControl ctrl = FormElementControl.GetControl(data[i]);
                    this.Controls.Add(ctrl);
                    ctrl.AnswerChanged += new DynamicFormElementEventHandler(this.formElementCtrl_onAnswerChanged);
                    ctrl.ElementRendering += new DynamicFormElementRenderingEventHandler(this.formElementCtrl_onRendering);
                    ctrl.ElementRendered += new DynamicFormElementEventHandler(this.formElementCtrl_onRendered);
                    ctrl.VisibilityChanged += new DynamicFormElementEventHandler(this.formElementCtrl_onVisibilityChanged);
                    ctrl.Owner = this;
                }
            }

            base.CreateChildControls();
        }
        protected virtual IEnumerable<ScriptDescriptor> GetScriptDescriptors()
        {
            ScriptControlDescriptor descriptor = new ScriptControlDescriptor("RainstormStudios.Web.UI.WebControls.DynamicForm", this.ClientID);
            descriptor.AddProperty("onClientElementVisibilityChanged", this.OnClientElementVisibilityChanged);
            descriptor.AddProperty("onClientElementAnswerChanged", this.OnClientElementAnswerChanged);
            descriptor.AddProperty("onClientElementRendering", this.OnClientElementRendering);
            descriptor.AddProperty("onClientElementRendered", this.OnClientElementRendered);
            yield return descriptor;
        }
        protected virtual IEnumerable<ScriptReference> GetScriptReferences()
        {
            // The whole "this.GetType()" breaks if you inherit this control in another assembly.  Using "typeof" seems like a more robust approach.
            //yield return new ScriptReference("RainstormStudios.Web.UI.WebControls.scripts.dynamicForms.dynamicFormControl.js", this.GetType().Assembly.FullName);
            yield return new ScriptReference("RainstormStudios.Web.UI.WebControls.scripts.dynamicForms.dynamicFormControl.js", typeof(RainstormStudios.Web.UI.WebControls.DynamicForm).Assembly.FullName);
        }
        IEnumerable<ScriptReference> IScriptControl.GetScriptReferences()
        {
            return this.GetScriptReferences();
        }
        IEnumerable<ScriptDescriptor> IScriptControl.GetScriptDescriptors()
        {
            return this.GetScriptDescriptors();
        }
        protected FormElementUserInput[] GetControlUserInput(ControlCollection ctrlCol)
        {
            List<FormElementUserInput> answers = new List<FormElementUserInput>();

            if (ctrlCol != null)
            {
                for (int i = 0; i < ctrlCol.Count; i++)
                {
                    Control ctrl = ctrlCol[i];

                    answers.AddRange(this.GetControlUserInput(ctrl.Controls));

                    FormElementControl eCtrl = (ctrl as FormElementControl);
                    if (eCtrl != null)
                        answers.Add(eCtrl.UserInput);
                }
            }

            return answers.ToArray();
        }
        protected void SetAsControlParent(FormElementControl ctrl)
        { ctrl.Owner = this; }
        #endregion

        #region Event Handlers
        //***************************************************************************
        // Event Handlers
        // 
        protected void formElementCtrl_onAnswerChanged(object sender, DynamicFormElementEventArgs e)
        {
            this.OnAnswerChanged(e);
        }
        protected void formElementCtrl_onRendering(object sender, DynamicFormElementRenderingEventArgs e)
        {
            this.OnElementRendering(e);
        }
        protected void formElementCtrl_onRendered(object sender, DynamicFormElementEventArgs e)
        {
            this.OnElementRendered(e);
        }
        protected void formElementCtrl_onVisibilityChanged(object sender, DynamicFormElementEventArgs e)
        {
            this.OnElementVisibilityChanged(e);
        }
        //***************************************************************************
        // Event Triggers
        // 
        protected virtual void OnAnswerChanged(DynamicFormElementEventArgs e)
        {
            if (this.ElementAnswerChanged != null)
                this.ElementAnswerChanged.Invoke(this, e);
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
        protected virtual void OnElementVisibilityChanged(DynamicFormElementEventArgs e)
        {
            if (this.ElementVisibilityChanged != null)
                this.ElementVisibilityChanged.Invoke(this, e);
        }
        #endregion
    }
}
