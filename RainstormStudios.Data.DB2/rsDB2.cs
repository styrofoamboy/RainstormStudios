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
using System.Data.Common;
using System.Collections.Generic;
using System.Text;
using RainstormStudios;
using RainstormStudios.Data;
using IBM.Data.DB2;

namespace RainstormStudios.Data.DB2
{
    public class rsDbDb2 : rsDb
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        protected new DB2Connection
            _dbConn;
        protected new DB2Command
            _dbCmd;
        protected new DB2DataReader
            _dbRdr;
        protected new DB2DataAdapter
            _dbAdp;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public override AdoProviderType ProviderType
        { get { return AdoProviderType.DB2Provider; } }
        public new DB2Connection DbConnection
        { get { return this._dbConn; } }
        public new DB2Command DbCommand
        { get { return this._dbCmd; } }
        public new DB2DataReader DataReader
        { get { return this._dbRdr; } }
        public new DB2DataAdapter DataAdapter
        { get { return this._dbAdp; } }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public rsDbDb2(DB2Connection conn)
            : base()
        {
            this._dbConn = conn;
        }
        public rsDbDb2(string connStr)
            : base()
        {
            this.InitConnection(connStr);
        }
        public rsDbDb2(string connStr, string qryStr)
            : this(connStr)
        {
            this.InitCommand(qryStr);
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public override void InitConnection(string connStr)
        {
            try
            {
                if (this._dbConn != null)
                    this.CloseConnection();

                this._connStr = connStr;
                this.CreateConnectionObject(this._connStr);
            }
            catch
            { throw; }
        }
        public override void InitCommand(string qryStr)
        {
            try
            {
                this.CheckConnectionReady();

                if (this._dbCmd != null)
                    this._dbCmd.Dispose();

                this._qryStr = qryStr;
                this.CreateCommandObject(qryStr);
                //this.CommandCreatedEvent();

                if (this._dbCmd.Connection == null)
                    this._dbCmd.Connection = this._dbConn;
            }
            catch
            { throw; }
        }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        /// <summary>
        /// The DB2 Implementation of this assigns internal variable directly and returns NULL,
        /// due to the DB2 classes not inheriting from the Sytem.Data.Generic namespace classes.
        /// </summary>
        /// <param name="connStr"></param>
        /// <returns></returns>
        protected override DbConnection CreateConnectionObject(string connStr)
        {
            this._dbConn = new DB2Connection(connStr);
            return null;
        }
        /// <summary>
        /// The DB2 Implementation of this assigns internal variable directly and returns NULL,
        /// due to the DB2 classes not inheriting from the Sytem.Data.Generic namespace classes.
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        protected override DbCommand CreateCommandObject(string sql)
        {
            this._dbCmd = new DB2Command(sql, this.DbConnection);
            return null;
        }
        /// <summary>
        /// The DB2 Implementation of this assigns internal variable directly and returns NULL,
        /// due to the DB2 classes not inheriting from the Sytem.Data.Generic namespace classes.
        /// </summary>
        /// <returns></returns>
        protected override DbDataAdapter CreateAdapterObject()
        { return new DB2DataAdapter(this._dbCmd); }
        #endregion
    }
}
