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
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Data.Odbc;
using RainstormStudios.Collections;

namespace RainstormStudios.Data
{
    [Author("Unfried, Michael")]
    public struct DbConnectionProperties
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private AdoProviderType
            _dbType;
        private StringCollection
            _vals;
        //***************************************************************************
        // Public Fields
        // 
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public AdoProviderType ProviderID
        {
            get { return this._dbType; }
            set
            {
                if (value == AdoProviderType.Auto)
                    throw new ArgumentException("Enumeration value \"Auto\" is not valid for DbConnectionProperties object.", "value");
                else
                    this._dbType = value;
            }
        }
        public string this[int index]
        {
            get { return this._vals[index]; }
            set { this._vals[index] = value; }
        }
        public string this[string key]
        {
            get
            {
                if (this._vals.ContainsKey(key))
                    return this._vals[key];
                else
                    return string.Empty;
            }
            set
            {
                if (this._vals.ContainsKey(key))
                    this._vals[key] = value;
                else
                    this._vals.Add(value, key);
            }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public DbConnectionProperties(AdoProviderType providerid, string connString)
            :this(providerid)
        {
            this = DbConnectionProperties.FromString(providerid, connString);
        }
        public DbConnectionProperties(AdoProviderType providerid)
        {
            if (providerid == AdoProviderType.Auto)
                throw new ArgumentException("Enumeration value \"Auto\" is not valid for DbConnectionProperties object.", "providerid");
            this._vals = new StringCollection();
            this._dbType = providerid;
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public void Add(string keyword, string value)
        {
            this._vals.Add(value, keyword);
        }
        public override string ToString()
        {
            return this.GetConnStr();
        }
        //***************************************************************************
        // Static Methods
        // 
        public static DbConnectionProperties FromString(AdoProviderType provid, string value)
        {
            if (provid == AdoProviderType.Auto)
                throw new ArgumentException("Enumeration value \"Auto\" is not valid for this function.", "provid");

            DbConnectionProperties retVal = new DbConnectionProperties();
            retVal._dbType = provid;

            DbConnectionStringBuilder connBldr = null;
            switch (provid)
            {
                case AdoProviderType.SqlProvider:
                    connBldr = new SqlConnectionStringBuilder(value);
                    break;
                case AdoProviderType.OleProvider:
                    connBldr = new OleDbConnectionStringBuilder(value);
                    break;
                case AdoProviderType.OdbcProvider:
                    connBldr = new OdbcConnectionStringBuilder(value);
                    break;
                case AdoProviderType.DB2Provider:
                    connBldr = new Db2ConnectionStringBuilder(value);
                    break;
            }
            string[] keys = new string[connBldr.Keys.Count];
            connBldr.Keys.CopyTo(keys, 0);
            string[] vals = new string[connBldr.Values.Count];
            connBldr.Values.CopyTo(vals, 0);
            for (int i = 0; i < keys.Length; i++)
                retVal._vals.Add(vals[i], keys[i]);

            return retVal;

            #region Depreciated Code
            //AdoProviderType provid;
            //string ds = "", un = "", pw = "", ic = "", exAttr = "";

            //// Parse the provider type.
            //if (value.ToUpper().Contains("PROVIDER="))
            //    provid = AdoProviderType.OleProvider;
            //else if (value.ToUpper().Contains("DRIVER="))
            //    provid = AdoProviderType.OdbcProvider;
            //else
            //    provid = AdoProviderType.SqlProvider;

            //// Parse the datasource.
            //int dsIdx = value.ToUpper().IndexOf("DATA SOURCE=");
            //if (dsIdx > -1)
            //{
            //    int dsLen = value.IndexOf(';', dsIdx) - dsIdx;
            //    string dsStr = value.Substring(dsIdx, dsLen);
            //    ds = dsStr.Substring(dsStr.IndexOf('=') + 1);
            //}

            //// Parse the username.
            //int unIdx = value.ToUpper().IndexOf("USER ID=");
            //if (unIdx > -1)
            //{
            //    int unLen = value.IndexOf(';', unIdx) - unIdx;
            //    string unStr = value.Substring(unIdx, unLen);
            //    un = unStr.Substring(unStr.IndexOf('=') + 1);
            //}

            //// Parse the password.
            //int pwIdx = value.ToUpper().IndexOf("PASSWORD=");
            //if (pwIdx > -1)
            //{
            //    int pwLen = value.IndexOf(';', pwIdx) - pwIdx;
            //    string pwStr = value.Substring(pwIdx, pwLen);
            //    pw = pwStr.Substring(pwStr.IndexOf('=') + 1);
            //}

            //// Parse the initial catalog.
            //int icIdx = value.ToUpper().IndexOf("INITIAL CATALOG=");
            //if (icIdx > -1)
            //{
            //    int icLen = value.IndexOf(';', icIdx) - icIdx;
            //    string icStr = value.Substring(icIdx, icLen);
            //    ic = icStr.Substring(icStr.IndexOf('=') + 1);
            //}

            //// Create a new object based on the captured values.
            //DbConnectionProperties dbProp = new DbConnectionProperties(provid, ds, un, pw, ic);

            //// Check for any extended properties
            //int epIdx = value.ToUpper().IndexOf("EXTENDED PROPERTIES=");
            //if (epIdx > -1)
            //{
            //    int epLen = value.IndexOf(';', epIdx) - epIdx;
            //    string epStr = value.Substring(epIdx, epLen);
            //    dbProp.ExtendedProperties = epStr;
            //}

            //// Return the new struct.
            //return dbProp;
            #endregion
        }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        private void InitValues(AdoProviderType val)
        {
            switch (val)
            {
                case AdoProviderType.SqlProvider:
                    this._vals.Add("", "DataSource");
                    this._vals.Add("", "Initial Catalog");
                    break;
                case AdoProviderType.OleProvider:
                    this._vals.Add("", "Provider");
                    this._vals.Add("", "DataSource");
                    break;
                case AdoProviderType.OdbcProvider:
                    this._vals.Add("", "Driver");
                    break;
                case AdoProviderType.DB2Provider:
                    break;
            }
        }
        private string GetConnStr()
        {
            // Declare a generic ConnectionStringBuilder.
            DbConnectionStringBuilder connBldr = null;

            // Determine which type of actual ConnectionStringBuilder
            //   to instantiate.
            switch (this._dbType)
            {
                case AdoProviderType.OleProvider:
                    connBldr = new OleDbConnectionStringBuilder();
                    break;
                case AdoProviderType.OdbcProvider:
                    connBldr = new OdbcConnectionStringBuilder();
                    break;
                case AdoProviderType.SqlProvider:
                    connBldr = new SqlConnectionStringBuilder();
                    break;
                case AdoProviderType.DB2Provider:
                    connBldr = new Db2ConnectionStringBuilder();
                    break;
                default:
                    // It *shouldn't* be possible to hit this line of code,
                    //   unless I screwed up somewhere & allowed someone to
                    //   specify AdoProviderType.Auto somewhere.
                    throw new Exception("Unrecognized ADO provider type.");
            }

            // Load all the values from this instance into the
            //   ConnectionStringBuilder.
            for (int i = 0; i < this._vals.Count; i++)
                connBldr.Add(this._vals.GetKey(i), this._vals[i]);

            // Return the ConnectionStringBuilder's 'ToString' method result.
            return connBldr.ToString();
        }
        #endregion
    }
}
