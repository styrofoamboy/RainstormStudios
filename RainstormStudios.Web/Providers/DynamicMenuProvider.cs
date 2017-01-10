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
using RainstormStudios.Web.UI.WebControls.DynamicMenu;

namespace RainstormStudios.Providers
{
    public class DynamicMenuProviderConfiguration : System.Configuration.ConfigurationSection
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
        [StringValidator(MinLength = 1), ConfigurationProperty("default", DefaultValue = "SqlDynamicMenuProvider")]
        public string Default
        {
            get { return (string)base["default"]; }
            set { base["default"] = value; }
        }
        #endregion
    }
    public class DynamicMenuProviderManager
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private static DynamicMenuProvider
            _defaultProvider;
        private static DynamicMenuProviderCollection
            _providers;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public static DynamicMenuProvider Provider
        {
            get { return _defaultProvider; }
        }
        public static DynamicMenuProviderCollection Providers
        {
            get { return _providers; }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        static DynamicMenuProviderManager()
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
            DynamicMenuProviderConfiguration config =
                (DynamicMenuProviderConfiguration)ConfigurationManager.GetSection("DynamicMenuProvider");

            if (config == null)
                throw new ConfigurationErrorsException("Dynamic menu provider configuration section is not set correctly.");

            _providers = new DynamicMenuProviderCollection();

            System.Web.Configuration.ProvidersHelper.InstantiateProviders(config.Providers, _providers, typeof(DynamicMenuProvider));

            _providers.SetReadOnly();

            _defaultProvider = _providers[config.Default];

            if (_defaultProvider == null)
                throw new ProviderException("No default provider sepecified.");
        }
        #endregion
    }
    public abstract class DynamicMenuProvider : System.Configuration.Provider.ProviderBase
    {
        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public abstract DynamicMenuItem[] GetMenuItems(string menuName);
        public virtual void AddMenuItem(DynamicMenuItem newItem, string menuName)
        { this.AddMenuItem(newItem, menuName, null); }
        public abstract void AddMenuItem(DynamicMenuItem newItem, string menuName, DynamicMenuItem parent);
        public abstract string[] GetMenuNames();
        #endregion
    }
    public class DynamicMenuProviderCollection : System.Configuration.Provider.ProviderCollection
    {
        // Return an instance of CalendarProviderBase
        //   for a specified provider name.
        public new DynamicMenuProvider this[string name]
        {
            get { return (DynamicMenuProvider)base[name]; }
        }
    }
    public class SqlDynamicMenuProvider : DynamicMenuProvider
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private bool
            _writeExEventLog;
        private string
            _connStr;
        private int
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
                name = "SqlDynamicMenuProvider";

            if (string.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "Default DynamicMenu Provider");
            }

            base.Initialize(name, config);

            this._writeExEventLog = Boolean.Parse(this.GetConfigurationValue(config["WriteExceptionsToEventLog"], "true"));
            ConnectionStringSettings connStrSettings = ConfigurationManager.ConnectionStrings[config["connectionStringName"]];
            if (connStrSettings == null || connStrSettings.ConnectionString.Trim() == "")
                throw new ProviderException("You must specify a SQL connection string.");
            this._connStr = connStrSettings.ConnectionString;
            this._connRetryDelay = int.Parse(this.GetConfigurationValue(config["SqlConnectionRetryDelay"], "3000"));
        }
        public override DynamicMenuItem[] GetMenuItems(string menuName)
        {
            int menuID;
            List<DynamicMenuItem> items = new List<DynamicMenuItem>();
            using (SqlConnection conn = this.GetOpenConnection())
            {
                string sqlMenu = "SELECT MenuID FROM dbo.Menus WHERE LOWER(MenuName) = @0";
                using (SqlCommand cmd = new SqlCommand(sqlMenu, conn))
                {
                    cmd.Parameters.Add(new SqlParameter { ParameterName = "@0", Value = menuName.ToLower(), DbType = DbType.String });

                    object cmdVal = cmd.ExecuteScalar();
                    if (cmdVal == null || !int.TryParse(cmdVal.ToString(), out menuID))
                        throw new Exception("Specified menu was not found in the database.");
                }
                string sqlMenuItems = "SELECT * FROM MenuItems WHERE MenuID = @0 AND ActiveFlag = 'true' AND ParentMenuItemID IS NULL ORDER BY OrdinalValue, MenuItemName";
                using (SqlCommand cmd = new SqlCommand(sqlMenuItems, conn))
                {
                    cmd.Parameters.Add(new SqlParameter { ParameterName = "@0", Value = menuID, DbType = DbType.Int32 });
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                        while (rdr.Read())
                            items.Add(GetItemFromReader(rdr));
                }
                for (int i = 0; i < items.Count; i++)
                    items[i].MenuItems.AddRange(GetChildren(conn, items[i]));
            }
            return items.ToArray();
        }
        public override void AddMenuItem(DynamicMenuItem newItem, string menuName, DynamicMenuItem parent)
        {
            throw new NotImplementedException();
        }
        public override string[] GetMenuNames()
        {
            List<string> names = new List<string>();
            using (SqlConnection conn = this.GetOpenConnection())
            {
                string sql = "SELECT MenuName FROM Menus WHERE ActiveFlag = 'true' ORDER BY MenuName";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                using (SqlDataReader rdr = cmd.ExecuteReader())
                    while (rdr.Read())
                        if (rdr[0] != null)
                            names.Add(rdr[0].ToString());
            }
            return names.ToArray();
        }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        private string GetConfigurationValue(string configValue, string defaultValue)
        {
            if (string.IsNullOrEmpty(configValue))
                return defaultValue;
            return configValue;
        }
        private SqlConnection GetOpenConnection()
        {
            SqlConnection conn = new SqlConnection(this._connStr);
            conn.Open();
            int attemptCnt = 0;
            while (conn.State != ConnectionState.Open && attemptCnt++ < 5)
            {
                System.Threading.Thread.Sleep(this._connRetryDelay);
                conn.Open();
            }
            if (conn.State != ConnectionState.Open)
                throw new Exception("Unable to open connection to database using specified connection string.");
            return conn;
        }
        private DynamicMenuItem GetItemFromReader(SqlDataReader rdr)
        {
            DynamicMenuItem menuitem = new DynamicMenuItem();
            menuitem.MenuItemProviderKey = rdr["MenuID"];
            menuitem.Text = (string)rdr["MenuItemName"];
            menuitem.NavigationUrl = (string)rdr["NavigationUrl"];
            menuitem.ImageUrl = (string)rdr["MenuItemImageUrl"];
            menuitem.Target = (string)rdr["Target"];
            return menuitem;
        }
        private DynamicMenuItem[] GetChildren(SqlConnection conn, DynamicMenuItem item)
        {
            List<DynamicMenuItem> items = new List<DynamicMenuItem>();
            string sql = "SELECT * FROM MenuItems WHERE MenuID = @0 AND ParentMenuItemID = @1 AND ActiveFlag = 'true' ORDER BY OrdinalValue, MenuItemName";
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            using (SqlDataReader rdr = cmd.ExecuteReader())
                while (rdr.Read())
                    items.Add(GetItemFromReader(rdr));

            for (int i = 0; i < items.Count; i++)
                items[i].MenuItems.AddRange(GetChildren(conn, items[i]));

            return items.ToArray();
        }
        #endregion
    }
}
