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
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using RainstormStudios.Web.UI.WebControls;

namespace RainstormStudios.Providers
{
    [Author("Unfried, Michael")]
    public class DatabaseStreamProviderConfiguration : System.Configuration.ConfigurationSection
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
        [StringValidator(MinLength = 1), ConfigurationProperty("default", DefaultValue = "SqlDatabaseImageProvider")]
        public string Default
        {
            get { return (string)base["default"]; }
            set { base["default"] = value; }
        }
        #endregion
    }
    [Author("Unfried, Michael")]
    public class DatabaseStreamProviderManager
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private static DatabaseStreamProvider
            _defaultProvider;
        private static DatabaseStreamProviderCollection
            _providers;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public static DatabaseStreamProvider DefaultProvider
        { get { return _defaultProvider; } }
        public static DatabaseStreamProviderCollection Providers
        { get { return _providers; } }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        static DatabaseStreamProviderManager()
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
            DatabaseStreamProviderConfiguration config =
                (DatabaseStreamProviderConfiguration)ConfigurationManager.GetSection("DatabaseImageProvider");

            if (config == null)
                throw new ConfigurationErrorsException("Database image provider configuration section not set correctly.");

            _providers = new DatabaseStreamProviderCollection();

            System.Web.Configuration.ProvidersHelper.InstantiateProviders(config.Providers, _providers, typeof(DatabaseStreamProvider));

            _providers.SetReadOnly();

            _defaultProvider = _providers[config.Default];

            if (_defaultProvider == null)
                throw new ProviderException("No default provider specified.");
        }
        #endregion
    }
    [Author("Unfried, Michael")]
    public abstract class DatabaseStreamProvider : System.Configuration.Provider.ProviderBase
    {
        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public abstract DatabaseStreamData GetImage(string providerKey);
        #endregion
    }
    [Author("Unfried, Michael")]
    public class DatabaseStreamProviderCollection : System.Configuration.Provider.ProviderCollection
    {
        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public new DatabaseStreamProvider this[string name]
        {
            get { return (DatabaseStreamProvider)base[name]; }
        }
        #endregion
    }
    [Author("Unfried, Michael")]
    public class DatabaseStreamData
    {
        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public byte[] ImageData { get; private set; }
        public string ContentType { get; private set; }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public DatabaseStreamData(byte[] imgData, string contentType)
        {
            this.ImageData = imgData;
            this.ContentType = contentType;
        }
        #endregion
    }
}
