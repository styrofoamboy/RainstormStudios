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
using System.Collections.Generic;
using System.Text;

namespace RainstormStudios.Data
{
    [Author("Unfried, Michael")]
    public class DataRead : IDisposable
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private DataSet
            _qryRslt;
        private string
            _connStr,
            _cmndStr;
        private AdoProviderType 
            _prvType;
        //***************************************************************************
        // Thread Delegates
        // 
        private delegate DataSet ExecuteCommandDelegate(string ConnectionString, string Command, AdoProviderType Provider);
        //***************************************************************************
        // Public Events
        // 
        public event EventHandler ReadComplete;
        public event EventHandler ReadStarted;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public DataSet QueryResult
        {
            get { return _qryRslt; }
        }
        public string ConnectionString
        {
            get { return _connStr; }
            set { _connStr = value; }
        }
        public string CommandString
        {
            get { return _cmndStr; }
            set { _cmndStr = value; }
        }
        public AdoProviderType DbProvider
        {
            get { return _prvType; }
            set { _prvType = value; }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public DataRead()
        {
            _connStr = "";
            _cmndStr = "";
            _prvType = AdoProviderType.Auto;
            _qryRslt = null;
        }
        public DataRead(string ConnectionString)
            : this(ConnectionString, "")
        { }
        public DataRead(string ConnectionString, string Command)
            : this()
        {
            this._connStr = ConnectionString;
            this._cmndStr = Command;
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public void Dispose()
        {
            this._cmndStr = String.Empty;
            this._connStr = String.Empty;
            this._qryRslt.Dispose();
        }
        public void BeginExecuteCommand(string Command)
        {
            this._cmndStr = Command;
            BeginExecuteCommand();
        }
        public void BeginExecuteCommand()
        {
            if (string.IsNullOrEmpty(this._connStr) || string.IsNullOrEmpty(this._cmndStr))
                throw new Exception("Connection and command strings must be populated in order for data read thread to complete.");

            ExecuteCommandDelegate del = new ExecuteCommandDelegate(this.ExecuteCommand);
            del.BeginInvoke(this._connStr, this._cmndStr, this._prvType, new AsyncCallback(this.ExecuteCommandCallback), del);
        }
        public DataSet ExecuteCommand()
        {
            return this.ExecuteCommand(this._connStr, this._cmndStr, this._prvType);
        }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        private DataSet ExecuteCommand(string ConnectionString, string Command, AdoProviderType Provider)
        {
            this.ReadStartedEvent();

            // Declare DB object.
            rsDb dbRead = null;

            try
            {
                // Initialize using the proper DB provider class.
                dbRead = rsDb.GetDbObject(Provider, ConnectionString, Command);
                dbRead.DataAdapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
                return dbRead.GetData();
            }
            catch(Exception ex)
            {
                throw new Exception("Unable to read data from database.", ex);
            }
            finally
            {
                if (dbRead != null)
                    dbRead.Dispose();
            }
        }
        //***************************************************************************
        // Thread Methods
        // 
        private void ExecuteCommandCallback(IAsyncResult state)
        {
            ExecuteCommandDelegate del = (ExecuteCommandDelegate)state.AsyncState;
            this._qryRslt = del.EndInvoke(state);

            this.ReadCompleteEvent();
        }
        #endregion

        #region Event Triggers
        //***************************************************************************
        // Event Triggers
        // 
        private void ReadStartedEvent()
        {
            if (this.ReadStarted != null)
                this.ReadStarted.Invoke(this, EventArgs.Empty);
        }
        private void ReadCompleteEvent()
        {
            if (this.ReadComplete != null)
                this.ReadComplete.Invoke(this, EventArgs.Empty);
        }
        #endregion
    }
}
