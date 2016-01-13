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
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Configuration;
using System.Configuration.Provider;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.Configuration;
using RainstormStudios.Web.UI.WebControls.DynamicForms;

namespace RainstormStudios.Providers
{
    public class DynamicFormProviderConfiguration : System.Configuration.ConfigurationSection
    {
        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        [ConfigurationProperty("providers")]
        public ProviderSettingsCollection Providers
        {
            get { return (ProviderSettingsCollection)base["providers"]; }
        }
        [StringValidator(MinLength = 1), ConfigurationProperty("default", DefaultValue = "SqlDynamicFormProvider")]
        public string Default
        {
            get { return (string)base["default"]; }
            set { base["default"] = value; }
        }
        #endregion
    }
    public abstract class DynamicFormProviderManager
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private static DynamicFormProvider
            _defaultProvider;
        private static DynamicFormProviderCollection
            _providers;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public static DynamicFormProvider Provider
        { get { return _defaultProvider; } }
        public static DynamicFormProviderCollection Providers
        { get { return _providers; } }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        static DynamicFormProviderManager()
        {
            Initialize();
        }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        private static void Initialize()
        {
            DynamicFormProviderConfiguration config =
                (DynamicFormProviderConfiguration)ConfigurationManager.GetSection("DynamicFormProvider");

            if (config == null)
                throw new ConfigurationErrorsException("Dynamic form provider configuration section is not set correctly.");

            _providers = new DynamicFormProviderCollection();

            System.Web.Configuration.ProvidersHelper.InstantiateProviders(config.Providers, _providers, typeof(DynamicFormProvider));

            _providers.SetReadOnly();

            _defaultProvider = _providers[config.Default];

            if (_defaultProvider == null)
                throw new ProviderException("No default dynamic form provider specified.");
        }
        #endregion
    }
    public abstract class DynamicFormProvider : System.Configuration.Provider.ProviderBase
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private FormElementDataTypeCollection
            _dataTypes;
        private bool
            _hasLoadedCustomDataTypes = false;
        private bool
            _writeExEventLog;
        private string
            _eventSource = "DynamicFormProvider",
            _eventLog = "Application";
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public FormElementDataTypeCollection DataTypes
        {
            get
            {
                if (this._dataTypes == null)
                    this._dataTypes = new FormElementDataTypeCollection();
                if (!this._hasLoadedCustomDataTypes)
                    this.LoadCustomDataTypes();
                return this._dataTypes;
            }
        }
        public virtual string ValidatorCalloutExtenderCssClass
        { get { return string.Empty; } }
        public bool WriteExceptionsToEventLog
        {
            get { return this._writeExEventLog; }
            protected set { this._writeExEventLog = value; }
        }
        public string EventLogSource
        {
            get { return this._eventSource; }
            protected set { this._eventSource = value; }
        }
        public string EventLog
        {
            get { return this._eventLog; }
            protected set { this._eventLog = value; }
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Abstract Methods
        // 
        public abstract string[] GetForms();
        public abstract FormVersionData[] GetFormVersions(object formProviderKey);
        public abstract FormElementData[] GetFormElements(object formProviderKey);
        public abstract void AddFormElement(FormElementData newElement, object formProviderKey, FormElementData parent);
        public abstract object SaveFormElementInput(FormElementUserInput data, object assciatedKey = null);
        public abstract void SaveFormElementChanges(FormElementData data);
        public abstract FormElementUserInput LoadPreviousUserData(object formInstanceProviderKey, object elementProviderKey);
        //***************************************************************************
        // Public Methods
        // 
        public virtual void AddFormElement(FormElementData newElement, object formProviderKey)
        { this.AddFormElement(newElement, formProviderKey, null); }
        public virtual void LoadCustomDataTypes()
        {
            // If you want to define your own data types, you should override this method
            //   and instantiate them here, then add them to the FormElementDataTypeCollection
            //   found at the property named 'DataTypes'.  This method will be called only once,
            //   so make sure your override ends with base.LoadCustomDataTypes() so that the class
            //   can properly flag that this method has been executed.
            this._hasLoadedCustomDataTypes = true;
        }
        public virtual FormElementData[] GetChildElements(FormElementData data)
        {
            data.HasLoadedChildren = true;
            return null;
        }
        public virtual object[] GetDependantKeys(FormElementData data)
        {
            data.HasLoadedDependants = true;
            return null;
        }
        public virtual FormElementDataAnswer[] GetAnswers(FormElementData data)
        {
            data.HasLoadedAnswers = true;
            return null;
        }
        public virtual FormVersionData GetLastestFormVersion(object formProviderKey)
        {
            List<FormVersionData> data = new List<FormVersionData>();
            data.AddRange(this.GetFormVersions(formProviderKey));
            DateTime dtMax = data.Select(d => d.FormVersionCreatedDate).Max();
            return data.Where(d => d.FormVersionCreatedDate == dtMax).SingleOrDefault();
        }
        public virtual object[] SaveForm(FormElementUserInput[] inputs, object assciatedKey = null)
        {
            List<object> keys = new List<object>();
            if (inputs != null)
                for (int i = 0; i < inputs.Length; i++)
                    keys.Add(this.SaveFormElementInput(inputs[i], assciatedKey));
            return keys.ToArray();
        }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        protected string GetConfigurationValue(string configValue, string defaultValue)
        {
            if (string.IsNullOrEmpty(configValue))
                return defaultValue;
            return configValue;
        }
        protected void WriteEvent(Exception ex, string action)
        {
            if (this._writeExEventLog)
            {
                System.Diagnostics.EventLog log = new System.Diagnostics.EventLog();
                log.Source = _eventSource;
                log.Log = _eventLog;

                if (!System.Diagnostics.EventLog.SourceExists(_eventSource))
                    System.Diagnostics.EventLog.CreateEventSource(_eventSource, _eventLog);

                string msg = "An exception occured communicating with the data source.\n\n";
                msg += "Action: " + action + "\n\n";
                msg += "Exception: " + ex.ToString();

                log.WriteEntry(msg, System.Diagnostics.EventLogEntryType.Error);
            }
        }
        #endregion
    }
    public class DynamicFormProviderCollection : System.Configuration.Provider.ProviderCollection
    {
        // Return an instance of DynamicFormProviderBase
        //   for a specified provider name.
        public new DynamicFormProvider this[string name]
        {
            get { return (DynamicFormProvider)base[name]; }
        }
    }
    public struct FormData
    {
        #region Declarations
        //***************************************************************************
        // Public Fields
        // 
        public readonly object
            FormProviderKey;
        public readonly string
            FormName;
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public FormData(object frmProvKey, string frmName)
        {
            this.FormProviderKey = frmProvKey;
            this.FormName = frmName;
        }
        #endregion
    }
    public struct FormVersionData
    {
        #region Declarations
        //***************************************************************************
        // Public Fields
        // 
        public readonly object
            FormVersionProviderKey,
            ParentFormProviderKey;
        public readonly DateTime
            FormVersionCreatedDate;
        public readonly Nullable<DateTime>
            FormVersionEffectiveDate;
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public FormVersionData(object frmVerProvKey, object parentFrmVerProvKey, DateTime frmVerCreated, Nullable<DateTime> frmVerEffective)
        {
            this.FormVersionProviderKey = frmVerProvKey;
            this.ParentFormProviderKey = parentFrmVerProvKey;
            this.FormVersionCreatedDate = frmVerCreated;
            this.FormVersionEffectiveDate = frmVerEffective;
        }
        #endregion
    }
    public class SqlDynamicFormProvider : DynamicFormProvider
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        string
            _connStr;
        int
            _connRetryCount,
            _connRetryDelay;
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            // Init values from web.config
            if (config == null)
                throw new ArgumentNullException("config", "No configuration data was provided.");

            if (name == null || name.Length == 0)
                name = "SqlDynamicFormProvider";

            if (string.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "ITC Default SQL DynamicForm Provider");
            }

            base.Initialize(name, config);

            this.WriteExceptionsToEventLog = Boolean.Parse(this.GetConfigurationValue(config["WriteExceptionsToEventLog"], "true"));
            this.EventLog = this.GetConfigurationValue(config["EventLog"], "Application");
            this.EventLogSource = this.GetConfigurationValue(config["EventLogSource"], "SqlDynamicFormProvider");
            ConnectionStringSettings connStrSettings = ConfigurationManager.ConnectionStrings[config["connectionStringName"]];
            if (connStrSettings == null || connStrSettings.ConnectionString.Trim() == "")
                throw new ProviderException("You must specify a SQL connection string.");
            this._connStr = connStrSettings.ConnectionString;
            this._connRetryDelay = int.Parse(this.GetConfigurationValue(config["SqlConnectionRetryDelay"], "3000"));
            this._connRetryCount = int.Parse(this.GetConfigurationValue(config["SqlConnectionRetryCount"], "5"));
        }
        public override string[] GetForms()
        {
            throw new NotImplementedException();
        }
        public override FormVersionData[] GetFormVersions(object formProviderKey)
        {
            throw new NotImplementedException();
        }
        public override void AddFormElement(FormElementData newElement, object formProviderKey, FormElementData parent)
        {
            throw new NotImplementedException();
        }
        public override void SaveFormElementChanges(FormElementData data)
        {
            throw new NotImplementedException();
        }
        public override FormElementData[] GetFormElements(object formProviderKey)
        {
            throw new NotImplementedException();
        }
        public override object[] SaveForm(FormElementUserInput[] inputs, object assciatedKey = null)
        {
            return base.SaveForm(inputs);
        }
        public override object SaveFormElementInput(FormElementUserInput data, object assciatedKey = null)
        {
            throw new System.NotImplementedException();
        }
        public override FormElementUserInput LoadPreviousUserData(object formInstanceProviderKey, object elementProviderKey)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Private Methods
        //*******************************************************************************
        // Private Methods
        // 
        protected virtual SqlConnection GetOpenConnection()
        {
            return (SqlConnection)RainstormStudios.Data.rsData.GetOpenConnection(Data.AdoProviderType.SqlProvider, this._connStr, this._connRetryCount, this._connRetryDelay);
        }
        protected virtual FormElementData GetFormElementDataFromReader(SqlDataReader rdr)
        {
            FormElementData item = new FormElementData();
            //item.BackColor =  rdr[""];

            return item;
        }
        #endregion
    }
}
