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
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Data.Odbc;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using RainstormStudios.Collections;

namespace RainstormStudios.Data
{
    [Author("Unfried, Michael")]
    public abstract class rsDb : IDisposable
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        protected DbConnection
            _dbConn;
        protected DbCommand
            _dbCmd;
        protected DbDataReader
            _dbRdr;
        protected DbDataAdapter
            _dbAdp;
        protected string
            _connStr,
            _qryStr;
        protected bool
            _disposed,
            _disposing,
            _qryExec;
        protected DbTransaction
            _trans;
        protected Regex
            _qrySpliter;
        //***************************************************************************
        // Public Events
        // 
        public event EventHandler ConnectionOpened;
        public event EventHandler ConnectionClosed;
        public event EventHandler CommandCreated;
        public event EventHandler ReaderOpened;
        public event EventHandler ReaderClosed;
        public event EventHandler AdapterOpened;
        public event EventHandler AdapterClosed;
        public event DataSetFilledEventHandler QueryCompleted;
        public event EventHandler TransactionStart;
        public event EventHandler TransactionCommit;
        public event EventHandler TransactionRollback;
        //***************************************************************************
        // Thread Delegates
        //
        protected delegate DataSet BeginGetDataDelegate(string tblName, int startRecord, int maxRecords, MissingSchemaAction schemaAction);
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        /// <summary>
        /// Returns the value of the specified column ordinal in the current database record.
        /// </summary>
        /// <param name="columnIndex"></param>
        /// <returns></returns>
        public object this[int columnIndex]
        {
            get
            {
                if (this.DataReader != null && this.DataReader.HasRows)
                    return this.DataReader[columnIndex];
                else
                    throw new Exception("The DataReader has not been initialized with data.");
            }
        }
        /// <summary>
        /// Returns the value of the specified column name in the current database record.
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public object this[string columnName]
        {
            get
            {
                if (this.DataReader != null && this.DataReader.HasRows)
                    return this.DataReader[columnName];
                else
                    throw new Exception("The DataReader has not been initialized with data.");
            }
        }
        public bool IsDisposed
        { get { return this._disposed; } }
        public bool IsDisposing
        { get { return this._disposing; } }
        public bool QueryExecuting
        { get { return this._qryExec; } }
        public virtual DbConnection DbConnection
        {
            get
            {
                if (this._dbConn == null && !this._disposing)
                    this.InitConnection(this._connStr);
                return this._dbConn;
            }
        }
        public virtual DbCommand DbCommand
        {
            get
            {
                if (this._dbCmd == null && !this._disposing)
                    this.InitCommand(this._qryStr);
                return this._dbCmd;
            }
        }
        public virtual DbDataReader DataReader
        {
            get
            {
                if (this._dbRdr == null && !this._disposing)
                    this.InitReader();
                return this._dbRdr;
            }
        }
        public virtual DbDataAdapter DataAdapter
        {
            get
            {
                if (this._dbAdp == null && !this._disposing)
                    this.InitAdapter();
                return this._dbAdp;
            }
        }
        public virtual string ConnectionString
        {
            get
            {
                //return (!string.IsNullOrEmpty(this.DbConnection.ConnectionString))
                //              ? this.DbConnection.ConnectionString
                //              : this._connStr;
                return this._connStr;
            }
            set { this.InitConnection(value); }
        }
        public virtual string QueryString
        {
            get { return this.DbCommand.CommandText; }
            set { this.InitCommand(value); }
        }
        public abstract AdoProviderType ProviderType
        { get; }
        public int CommandTimeout
        {
            get { return this.DbCommand.CommandTimeout; }
            set { this.DbCommand.CommandTimeout = value; }
        }
        public bool HasTransaction
        {
            get { return (this._trans != null && this._trans.Connection != null); }
        }
        //***************************************************************************
        // Private Properties
        // 
        protected Regex QuerySplitter
        {
            get
            {
                if (this._qrySpliter == null)
                    this._qrySpliter = new Regex(@"(?:(^|(\WGO[\W;])))(?<qry>.*?)(?:($|(\WGO[\W;])))", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                return this._qrySpliter;
            }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        protected rsDb()
        {
            this._disposed = false;
            this._disposing = false;
            this._qryExec = false;
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        /// <summary>
        /// Releases all resources in use by this object.
        /// </summary>
        public void Dispose()
        {
            this._disposing = true;
            if (this.HasTransaction)
                this.CommitTransaction();
            if (this.DataAdapter != null)
                this.DataAdapter.Dispose();
            if (this.DataReader != null)
                this.DataReader.Dispose();
            if (this.DbCommand != null)
                this.DbCommand.Dispose();
            if (this.DbConnection != null)
                this.DbConnection.Dispose();
            this._disposing = false;
            this._disposed = true;
        }
        /// <summary>
        /// Initialize this object's connection to the data source.
        /// </summary>
        /// <param name="connStr">The connection string defining how to connect to the datasource.</param>
        /// <returns>The resulting ConnectionState of the object's connection.</returns>
        public virtual void InitConnection(string connStr)
        {
            try
            {
                if (this._dbConn != null)
                    this.CloseConnection();

                this._connStr = connStr;
                this._dbConn = this.CreateConnectionObject(this._connStr);
            }
            catch
            { throw; }
        }
        public ConnectionState OpenConnection()
        {
            if (this._dbConn.State != ConnectionState.Open)
            {
                try
                {
                    int c = 0;
                    while (this._dbConn.State != ConnectionState.Open && c++ < 4)
                    {
                        this._dbConn.Open();
                        if (this._dbConn.State != ConnectionState.Open)
                            System.Threading.Thread.Sleep(500);
                    }
                    if (this._dbConn.State != ConnectionState.Open)
                        throw new Exception("Unable to establish database connection.");

                    this.ConnectionOpenedEvent();
                }
                catch
                { throw; }
            }
            return this._dbConn.State;
        }
        public virtual void CloseConnection()
        {
            try
            {
                int c = 0;
                while (this._dbConn.State != ConnectionState.Closed && c++ < 4)
                {
                    this._dbConn.Close();
                    if (this._dbConn.State != ConnectionState.Closed)
                        System.Threading.Thread.Sleep(500);
                }
                if (this._dbConn.State != ConnectionState.Closed)
                    throw new Exception("Database connection is not responding to the close instruction. Connection may not be properly terminated.");

                this._dbConn = null;
                this.ConnectionClosedEvent();
            }
            catch
            { throw; }
        }
        public virtual void InitCommand(string qryStr)
        {
            try
            {
                this.CheckConnectionReady();

                if (this._dbAdp != null)
                    this._dbAdp.Dispose();
                if (this._dbRdr != null)
                    this._dbRdr.Dispose();
                if (this._trans != null)
                    this.RollbackTransaction();
                if (this._dbCmd != null)
                    this._dbCmd.Dispose();

                this._qryStr = qryStr;
                this._dbCmd = this.CreateCommandObject(qryStr);
                this.CommandCreatedEvent();

                if (this._dbCmd.Connection == null)
                    this._dbCmd.Connection = this._dbConn;
            }
            catch
            { throw; }
        }
        public virtual void InitReader(string qryStr)
        {
            this.InitCommand(qryStr);
            this.InitReader();
        }
        public virtual void InitReader()
        {
            try
            {
                // Check to make sure everything's ready before we try to initailize
                //   the object's DataReader.
                this.CheckConnectionReady();
                this.CheckCommandReady();

                // Make sure we don't already have an open DataAdapter
                //   on this connection.
                if (this._dbAdp != null)
                    this.CloseAdapter();

                // Make sure the database connection is open.
                this.OpenConnection();

                // Everything's good:  Execute the reader.
                this._dbRdr = this._dbCmd.ExecuteReader();
                this.ReaderOpenedEvent();
            }
            catch
            { throw; }
        }
        public void CloseReader()
        {
            if (this._dbRdr != null)
            {
                this._dbRdr.Dispose();
                this._dbRdr = null;
                this.ReaderClosedEvent();
            }
        }
        public void InitAdapter(string qryStr)
        {
            this.InitCommand(qryStr);
            this.InitAdapter();
        }
        public virtual void InitAdapter()
        {
            try
            {
                // Check to make sure everything's ready before we try to initialize
                //   the object's DataAdapter.
                this.CheckConnectionReady();
                this.CheckCommandReady();

                // Make sure the database connection is open.
                this.OpenConnection();

                // Make sure we don't already have a DataReader on this connection.
                if (this._dbRdr != null)
                    this._dbRdr.Dispose();

                // Everything's good:  Execute the reader.
                this._dbAdp = this.CreateAdapterObject();
                this.AdapterOpenedEvent();
            }
            catch
            { throw; }
        }
        public void CloseAdapter()
        {
            if (this._dbAdp != null)
            {
                this._dbAdp.Dispose();
                this._dbAdp = null;
                this.AdapterClosedEvent();
            }
        }
        public virtual void TruncateTable(string tableName)
        {
            rsDb.TruncateTable(this.ProviderType, this.ConnectionString, tableName);
        }
        public virtual void DropTable(string tableName)
        {
            rsDb.DropTable(this.ProviderType, this.ConnectionString, tableName);
        }
        public DataSet GetData()
        {
            return this.GetData(string.Empty, 0, 0);
        }
        public DataSet GetData(string dtName)
        {
            return this.GetData(dtName, 0, 0);
        }
        public DataSet GetData(string dtName, int startRecord, int maxRecords)
        {
            return this.GetData(dtName, startRecord, maxRecords, this.DataAdapter.MissingSchemaAction);
        }
        public DataSet GetData(string dtName, int startRecord, int maxRecords, MissingSchemaAction schemaAction)
        {
            DataSet ds = new DataSet();
            this.GetData(ref ds, dtName, startRecord, maxRecords, schemaAction);
            return ds;
        }
        public int GetData(ref DataSet ds)
        {
            return this.GetData(ref ds, string.Empty);
        }
        public int GetData(ref DataSet ds, string tblName)
        {
            return this.GetData(ref ds, tblName, 0, 0);
        }
        public int GetData(ref DataSet ds, string tblName, int startRecord, int maxRecords)
        {
            return this.GetData(ref ds, tblName, startRecord, maxRecords, this.DataAdapter.MissingSchemaAction);
        }
        public int GetData(ref DataSet ds, string tblName, int startRecord, int maxRecords, MissingSchemaAction schemaAction)
        {
            this._qryExec = true;
            this.CheckConnectionReady();
            this.CheckCommandReady();
            this.DataAdapter.MissingSchemaAction = schemaAction;
            try
            {
                int iSclr = -1;

                //Regex rgx = new Regex(@"(?<qry>.*?)(\s|;)?go(\s|;)?", RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.IgnoreCase);
                //for (Match m = rgx.Match(this._qryStr); m.Success; m.NextMatch())
                //{
                //}

                if (!string.IsNullOrEmpty(tblName))
                    iSclr = this.DataAdapter.Fill(ds, startRecord, maxRecords, tblName);
                else
                    iSclr = this.DataAdapter.Fill(ds);
                return iSclr;
            }
            catch
            {
                throw;
            }
            finally
            {
                this._qryExec = false;
            }
        }
        public void BeginGetData()
        {
            this.BeginGetData(string.Empty);
        }
        public void BeginGetData(string tblName)
        {
            this.BeginGetData(tblName, 0, 0);
        }
        public void BeginGetData(string tblName, int startRecord, int maxRecords)
        {
            this.BeginGetData(tblName, startRecord, maxRecords, this.DataAdapter.MissingSchemaAction);
        }
        public void BeginGetData(string tblName, int startRecord, int maxRecords, MissingSchemaAction schemaAction)
        {
            BeginGetDataDelegate del = new BeginGetDataDelegate(this.GetData);
            del.BeginInvoke(tblName, startRecord, maxRecords, schemaAction, new AsyncCallback(this.BeginGetDataCallback), del);
        }
        public void AbortExecution()
        {
            if (this._qryExec && this.DbCommand != null)
                this.DbCommand.Cancel();
        }
        public string PrepareCommand()
        {
            try
            {
                this.DbCommand.Prepare();
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public bool IsNonQuery()
        {
            try
            {
                this.InitReader();
                bool retVal = (this.DataReader.HasRows);
                this.CloseReader();
                return !retVal;
            }
            catch
            { throw; }
        }
        public DataSet Execute(string sql)
        { return this.Execute(sql, CommandType.Text); }
        public DataSet Execute(string sql, params DbParameter[] dbParams)
        {
            return this.Execute(sql, CommandType.Text, dbParams);
        }
        public DataSet Execute(string sql, CommandType type, params DbParameter[] dbParams)
        {
            this._qryExec = true;
            try
            {
                this.CheckConnectionReady();

                using (DbCommand cmd = this.CreateCommandObject(sql))
                {
                    cmd.CommandType = type;
                    // If you specify 'CommandType.StoredProcedure' you can't put
                    //   the "EXEC" command at before the stored procedure name.
                    if (type == CommandType.StoredProcedure)
                        cmd.CommandText = (sql.ToUpper().StartsWith("EXEC "))
                                            ? sql.Substring(5)
                                            : (sql.ToUpper().StartsWith("EXECUTE "))
                                                    ? sql.Substring(8)
                                                    : sql;

                    for (int i = 0; i < dbParams.Length; i++)
                        cmd.Parameters.Add(dbParams[i]);

                    return rsDb.Execute(cmd);
                }
            }
            catch
            { throw; }
            finally
            {
                this._qryExec = false;
            }
        }
        public object ExecuteScalar(string sql)
        { return this.ExecuteScalar(sql, CommandType.Text); }
        public object ExecuteScalar(string sql, params DbParameter[] dbParams)
        {
            return this.ExecuteScalar(sql, CommandType.Text, dbParams);
        }
        public object ExecuteScalar(string sql, CommandType type, params DbParameter[] dbParams)
        {
            this._qryExec = true;
            try
            {
                this.CheckConnectionReady();
                using (DbCommand cmd = this.CreateCommandObject(sql))
                {
                    cmd.CommandType = type;
                    // If you specify 'CommandType.StoredProceedure' you can't put
                    //   the "EXEC" command at before the stored proceedure name.
                    if (type == CommandType.StoredProcedure)
                        cmd.CommandText = (sql.ToUpper().StartsWith("EXEC "))
                                            ? sql.Substring(5)
                                            : (sql.ToUpper().StartsWith("EXECUTE "))
                                                    ? sql.Substring(8)
                                                    : sql;

                    for (int i = 0; i < dbParams.Length; i++)
                        cmd.Parameters.Add(dbParams[i]);

                    return rsDb.ExecuteScalar(cmd);
                }
            }
            catch
            { throw; }
            finally
            {
                this._qryExec = false;
            }
        }
        public int ExecuteNonQuery()
        {
            this._qryExec = true;
            try
            {
                this.CheckConnectionReady();
                this.CheckCommandReady();
                return this.DbCommand.ExecuteNonQuery();
            }
            catch
            { throw; }
            finally
            {
                this._qryExec = false;
            }
        }
        public int ExecuteNonQuery(string sql)
        { return this.ExecuteNonQuery(sql, CommandType.Text); }
        public int ExecuteNonQuery(string sql, params DbParameter[] dbParams)
        {
            return this.ExecuteNonQuery(sql, CommandType.Text, dbParams);
        }
        public int ExecuteNonQuery(string sql, CommandType type, params DbParameter[] dbParams)
        {
            this._qryExec = true;
            try
            {
                this.CheckConnectionReady();
                using (DbCommand cmd = this.CreateCommandObject(sql))
                {
                    cmd.CommandType = type;
                    cmd.Connection = this.DbConnection;

                    for (int i = 0; i < dbParams.Length; i++)
                        cmd.Parameters.Add(dbParams[i]);

                    return cmd.ExecuteNonQuery();
                }
            }
            catch
            { throw; }
            finally
            {
                this._qryExec = false;
            }
        }
        public void LoadDataTable(string sqlCommand, DataTable dtDest)
        {
            this.InitReader(sqlCommand);
            dtDest.Load(this._dbRdr, LoadOption.OverwriteChanges);
        }
        public virtual DbTransaction BeginTransaction()
        {
            this.CheckConnectionReady();
            this._trans = this.DbConnection.BeginTransaction();
            this.DbCommand.Transaction = this._trans;
            this.OnTransactionStart(EventArgs.Empty);
            return this._trans;
        }
        public virtual void RollbackTransaction()
        {
            if (this._trans == null)
                throw new ApplicationException("No transaction is current in process.");
            this._trans.Rollback();
            this._trans.Dispose();
            this._trans = null;
            this.OnTransactionRollback(EventArgs.Empty);
        }
        public virtual void CommitTransaction()
        {
            if (this._trans == null)
                throw new ApplicationException("No transaction is current in process.");
            this._trans.Commit();
            this._trans.Dispose();
            this._trans = null;
            this.OnTransactionCommit(EventArgs.Empty);
        }
        public virtual bool NextRecord()
        {
            return this.DataReader.Read();
        }
        public virtual bool NextResult()
        {
            return this.DataReader.NextResult();
        }
        public virtual string[] GetQueryParts(string sql)
        {
            // Determine if this is a batch query.
            List<string> cmdStr = new List<string>();
            Match m;
            for (m = this.QuerySplitter.Match(sql); m.Success; m = m.NextMatch())
                cmdStr.Add(m.Groups["qry"].Value);
            if (cmdStr.Count < 1)
                cmdStr.Add(sql);
            return cmdStr.ToArray();
        }
        public SqlTableParams GetTableFormat(string tableName)
        {
            DataTable dtFmt = null;
            try
            {
                this.LoadDataTable("SELECT TOP 1 * FROM " + tableName + " (nolock)", dtFmt);
                SqlTableParams p = new SqlTableParams(dtFmt);
                return p;
            }
            catch
            { throw; }
            finally
            {
                if (dtFmt != null)
                    dtFmt.Dispose();
            }
        }
        //***************************************************************************
        // Static Methods
        // 
        public static DataSet Execute(AdoProviderType dbProvider, string connStr, string sql, params DbParameter[] p)
        {
            try
            { return rsDb.Execute(dbProvider, connStr, sql, CommandType.Text, p); }
            catch
            { throw; }
        }
        public static DataSet Execute(AdoProviderType dbProvider, string connStr, string sql, CommandType type, params DbParameter[] p)
        {
            try
            { return rsDb.Execute(dbProvider, connStr, sql, type, 0, p); }
            catch
            { throw; }
        }
        public static DataSet Execute(AdoProviderType dbProvider, string connStr, string sql, CommandType type, int timeOut, params DbParameter[] p)
        {
            try
            {
                using (rsDb db = rsDb.GetDbObject(dbProvider, connStr, sql))
                {
                    db.DbCommand.CommandType = type;
                    db.DbCommand.CommandTimeout = timeOut;
                    if (p != null)
                        for (int i = 0; i < p.Length; i++)
                            db.DbCommand.Parameters.Add((DbParameter)p[i]);
                    return rsDb.Execute(db.DbCommand);
                }
            }
            catch
            { throw; }
        }
        public static DataSet Execute(DbCommand cmd)
        {
            try
            {
                DataSet dsRet = new DataSet();
                using (DbDataReader rdr = cmd.ExecuteReader())
                {
                    while (true)
                    {
                        DataTable dt = dsRet.Tables.Add();
                        for (int i = 0; i < rdr.FieldCount; i++)
                            dt.Columns.Add(rdr.GetName(i), rdr.GetFieldType(i));

                        dt.BeginLoadData();
                        while (rdr.Read())
                        {
                            DataRow dr = dt.NewRow();
                            //rdr.GetValues(dr.ItemArray);
                            for (int c = 0; c < rdr.FieldCount; c++)
                                dr[c] = rdr.GetValue(c);
                            dt.Rows.Add(dr);
                        }
                        dt.EndLoadData();
                        dt.AcceptChanges();
                        if (!rdr.NextResult())
                            break;
                    }
                }
                return dsRet;
            }
            catch
            { throw; }
        }
        public static object ExecuteScalar(DbCommand cmd)
        {
            using (DataSet ds = rsDb.Execute(cmd))
            {
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0 && ds.Tables[0].Rows[0].ItemArray.Length > 0)
                    return ds.Tables[0].Rows[0].ItemArray[0];
                else
                    return null;
            }
        }
        public static int ExecuteNonQuery(AdoProviderType providerId, string connStr, string queryString, params DbParameter[] p)
        { return rsDb.ExecuteNonQuery(providerId, connStr, queryString, CommandType.Text, p); }
        public static int ExecuteNonQuery(AdoProviderType providerId, string connStr, string queryString, CommandType type, params DbParameter[] p)
        { return rsDb.ExecuteNonQuery(providerId, connStr, queryString, type, 0, p); }
        public static int ExecuteNonQuery(AdoProviderType providerId, string connStr, string queryString, CommandType type, int timeOut, params DbParameter[] p)
        {
            try
            {
                using (rsDb db = rsDb.GetDbObject(providerId, connStr, queryString))
                {
                    db.DbCommand.CommandType = type;
                    db.DbCommand.CommandTimeout = timeOut;
                    if (p != null)
                        for (int i = 0; i < p.Length; i++)
                            db.DbCommand.Parameters.Add((DbParameter)p[i]);
                    return db.DbCommand.ExecuteNonQuery();
                }
            }
            catch
            { throw; }
        }
        public static rsDb GetDbObject(DbConnectionProperties connParams)
        { return rsDb.GetDbObject(connParams, ""); }
        public static rsDb GetDbObject(DbConnectionProperties connParams, string qryString)
        { return rsDb.GetDbObject(connParams.ProviderID, connParams.ToString(), qryString); }
        public static rsDb GetDbObject(AdoProviderType id, string connStr, string qryString)
        {
            switch (id)
            {
                case AdoProviderType.Auto:
                    return rsDb.GetDbObject(rsData.ParseProviderType(connStr), connStr, qryString);
                case AdoProviderType.SqlProvider:
                    return new rsDbSql(connStr, qryString);
                case AdoProviderType.OleProvider:
                    return new rsDbOle(connStr, qryString);
                case AdoProviderType.OdbcProvider:
                    return new rsDbOdbc(connStr, qryString);
                case AdoProviderType.DB2Provider:
                    //return new rsDbDb2(connStr, qryString);
                    string dllFn = "RainstormStudios.Data.DB2.dll";
                    if (!System.IO.File.Exists(System.IO.Path.Combine(Environment.CurrentDirectory, dllFn)))
                        throw new System.IO.FileNotFoundException("RainstormStudios DB2 Library was not found in the application path.", dllFn);
                    System.Reflection.Assembly asm = System.Reflection.Assembly.LoadWithPartialName(dllFn);
                    if (asm == null)
                        throw new Exception("Unable to load RainstormStudios.Data.DB2.dll library file.");
                    object db2 = asm.CreateInstance("RainstormStudios.Data.DB2.rsDbDb2");
                    if (db2 == null)
                        throw new Exception("An error occured while trying to create rsDbDb2 instance.");
                    else
                        return (rsDb)db2;
                default:
                    // We should *never* hit this line of code.
                    throw new ArgumentException("Unrecognized AdoProviderType value.");
            }
        }
        public static void TruncateTable(AdoProviderType id, string connStr, string tableName)
        {
            try
            {
                string qryStr = String.Format("TRUNCATE TABLE {0}", tableName);
                using (rsDb db = rsDb.GetDbObject(id, connStr, qryStr))
                    db.ExecuteNonQuery();
            }
            catch
            { throw; }
        }
        public static void DropTable(AdoProviderType id, string connStr, string tableName)
        {
            try
            {
                string qryStr = String.Format("DROP TABLE {0}", tableName);
                using (rsDb db = rsDb.GetDbObject(id, connStr, qryStr))
                    db.ExecuteNonQuery();
            }
            catch
            { throw; }
        }
        public static void UploadObject(AdoProviderType id, string connStr, string tableName, object typeObj)
        {
            using (rsDb db = rsDb.GetDbObject(id, connStr, "SELECT * FROM " + tableName))
            {
                // Retrieve the existing data and the Type of the passed
                //   object to be uploaded.
                DataSet dt = db.GetData();
                Type objType = typeObj.GetType();

                // Get the properties defined by the passed object.
                System.Reflection.PropertyInfo[] pi = objType.GetProperties(System.Reflection.BindingFlags.DeclaredOnly | System.Reflection.BindingFlags.Instance);

                // Try and grab the DataTable

            }
        }
        public static string GetInitialCatalog(string connStr)
        {
            int initCat = connStr.ToLower().IndexOf("initial catalog=");
            if (initCat < 0)
                throw new Exception("Specified connection string does not contain an 'Initial Catalog' field.");

            int initCatStart = connStr.ToLower().IndexOf('=', initCat);
            int initCatEnd = connStr.ToLower().IndexOf(';', initCatStart);
            if (initCatEnd < 0 || initCatEnd > connStr.Length)
                initCatEnd = connStr.Length - 1;

            try
            { return connStr.Substring(initCatStart, initCatEnd - initCatStart).Trim('=', ' ', ';'); }
            catch
            { throw; }
        }
        public static bool IsSQL(DbConnectionProperties conn)
        { return rsDb.IsSQL(conn.ToString(), conn.ProviderID); }
        public static bool IsSQL(string connStr, AdoProviderType dbType)
        {
            int iStrt = connStr.ToLower().IndexOf("provider");
            if (iStrt >= 0) iStrt = connStr.IndexOf('=', iStrt);
            int iEnd = (iStrt > -1) ? connStr.IndexOf(';', iStrt) : -1;
            return dbType == AdoProviderType.SqlProvider
                    || (dbType == AdoProviderType.OleProvider && iStrt > -1
                            && ((iEnd > -1 && connStr.Substring(iStrt, iEnd - iStrt).ToUpper().Contains("SQL"))
                            || (connStr.Substring(iStrt).ToUpper().Contains("SQL"))));
        }
        public static bool IsSQL(rsDb db)
        {
            try
            {
                object objVer = db.ExecuteScalar("SELECT @@VERSION");
                return (objVer != null && objVer.ToString().ToLower().StartsWith("microsoft sql"));
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Protected Methods
        // 
        protected void CheckConnectionReady()
        {
            if (this._dbConn == null)
                throw new NullReferenceException("You must initialize the object's database connection first.");
            if (this._dbConn.State == ConnectionState.Fetching || this._dbConn.State == ConnectionState.Executing)
                throw new Exception("The data connection is currently processing data.  Please wait for it to finish before making another request.");
            this.OpenConnection();
        }
        protected void CheckCommandReady()
        {
            if (this._dbCmd == null || string.IsNullOrEmpty(this._dbCmd.CommandText))
                throw new Exception("You must initialize the object's query command first.");
        }
        protected int InsertDataRow(DataRow schemaRow, string tableName, AdoProviderType providerID)
        {
            using (System.Data.Common.DbCommand cmd = rsData.GetInsertCommand(schemaRow, tableName, providerID))
                return cmd.ExecuteNonQuery();
        }
        protected int UpdateDataRow(DataRow schemaRow, string tableName, string keyField, AdoProviderType providerID)
        {
            using (System.Data.Common.DbCommand cmd = rsData.GetUpdateCommand(schemaRow, tableName, keyField, providerID))
                return cmd.ExecuteNonQuery();
        }
        protected void OnConnectionOpened(EventArgs e)
        {
            if (this.ConnectionOpened != null)
                this.ConnectionOpened.Invoke(this, e);
        }
        protected void OnConnectionClosed(EventArgs e)
        {
            if (this.ConnectionClosed != null)
                this.ConnectionClosed.Invoke(this, e);
        }
        protected void OnCommandCreated(EventArgs e)
        {
            if (this.CommandCreated != null)
                this.CommandCreated.Invoke(this, e);
        }
        protected void OnReaderOpened(EventArgs e)
        {
            if (this.ReaderOpened != null)
                this.ReaderOpened.Invoke(this, e);
        }
        protected void OnReaderClosed(EventArgs e)
        {
            if (this.ReaderClosed != null)
                this.ReaderClosed.Invoke(this, e);
        }
        protected void OnAdapterOpened(EventArgs e)
        {
            if (this.AdapterOpened != null)
                this.AdapterOpened.Invoke(this, e);
        }
        protected void OnAdapaterClosed(EventArgs e)
        {
            if (this.AdapterClosed != null)
                this.AdapterClosed.Invoke(this, e);
        }
        protected void OnTransactionStart(EventArgs e)
        {
            if (this.TransactionStart != null)
                this.TransactionStart.Invoke(this, e);
        }
        protected void OnTransactionCommit(EventArgs e)
        {
            if (this.TransactionCommit != null)
                this.TransactionCommit.Invoke(this, e);
        }
        protected void OnTransactionRollback(EventArgs e)
        {
            if (this.TransactionRollback != null)
                this.TransactionRollback.Invoke(this, e);
        }
        //***************************************************************************
        // Event Triggers
        // 
        private void ConnectionOpenedEvent()
        {
            this.OnConnectionOpened(EventArgs.Empty);
        }
        private void ConnectionClosedEvent()
        {
            this.OnConnectionClosed(EventArgs.Empty);
        }
        private void CommandCreatedEvent()
        {
            this.OnCommandCreated(EventArgs.Empty);
        }
        private void ReaderOpenedEvent()
        {
            this.OnReaderOpened(EventArgs.Empty);
        }
        private void ReaderClosedEvent()
        {
            this.OnReaderClosed(EventArgs.Empty);
        }
        private void AdapterOpenedEvent()
        {
            this.OnAdapterOpened(EventArgs.Empty);
        }
        private void AdapterClosedEvent()
        {
            this.OnAdapaterClosed(EventArgs.Empty);
        }
        private void TransactionStartEvent()
        {
            this.OnTransactionStart(EventArgs.Empty);
        }
        private void TransactionCommitEvent()
        {
            this.OnTransactionCommit(EventArgs.Empty);
        }
        private void TransactionRollbackEvent()
        {
            this.OnTransactionRollback(EventArgs.Empty);
        }
        //***************************************************************************
        // Thread Callbacks
        // 
        private void BeginGetDataCallback(IAsyncResult state)
        {
            BeginGetDataDelegate del = (BeginGetDataDelegate)state.AsyncState;
            DataSet ds = del.EndInvoke(state);

            if (this.QueryCompleted != null)
                this.QueryCompleted.Invoke(this, new DataSetFilledEventArgs(ds));
        }
        //***************************************************************************
        // Abstract Methods
        // 
        protected abstract DbConnection CreateConnectionObject(string connStr);
        protected abstract DbCommand CreateCommandObject(string sql);
        protected abstract DbDataAdapter CreateAdapterObject();
        #endregion
    }
    [Author("Unfried, Michael")]
    public class rsDbSql : rsDb
    {
        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public override AdoProviderType ProviderType
        { get { return AdoProviderType.SqlProvider; } }
        public new SqlConnection DbConnection
        { get { return (SqlConnection)base.DbConnection; } }
        public new SqlCommand DbCommand
        { get { return (SqlCommand)base.DbCommand; } }
        public new SqlDataReader DataReader
        { get { return (SqlDataReader)base.DataReader; } }
        public new SqlDataAdapter DataAdapter
        { get { return (SqlDataAdapter)base.DataAdapter; } }
        //***************************************************************************
        // Private Properties
        private SqlTransaction OpenTransaction
        { get { return (this.HasTransaction) ? (SqlTransaction)this._trans : null; } }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public rsDbSql(SqlConnection conn)
            : base()
        {
            this._dbConn = conn;
        }
        public rsDbSql(string connStr)
            : base()
        {
            try { this.InitConnection(connStr); }
            catch { throw; }
        }
        public rsDbSql(string connStr, string qryStr)
            : this(connStr)
        {
            try { this.InitCommand(qryStr); }
            catch { throw; }
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public new SqlTransaction BeginTransaction()
        {
            this.CheckConnectionReady();
            this._trans = this.DbConnection.BeginTransaction();
            this.DbCommand.Transaction = this.OpenTransaction;
            this.OnTransactionStart(EventArgs.Empty);
            return this.OpenTransaction;
        }
        public SqlTransaction BeginTransaction(string name)
        {
            this.CheckConnectionReady();
            this._trans = this.DbConnection.BeginTransaction(name);
            this.DbCommand.Transaction = this.OpenTransaction;
            this.OnTransactionStart(EventArgs.Empty);
            return this.OpenTransaction;
        }
        public SqlTransaction BeginTransaction(IsolationLevel isoLvl)
        {
            this.CheckConnectionReady();
            this._trans = this.DbConnection.BeginTransaction(isoLvl);
            this.DbCommand.Transaction = this.OpenTransaction;
            this.OnTransactionStart(EventArgs.Empty);
            return this.OpenTransaction;
        }
        public SqlTransaction BeginTransaction(IsolationLevel isoLvl, string name)
        {
            this.CheckConnectionReady();
            this._trans = this.DbConnection.BeginTransaction(isoLvl, name);
            this.DbCommand.Transaction = this.OpenTransaction;
            this.OnTransactionStart(EventArgs.Empty);
            return this.OpenTransaction;
        }
        public void RollbackTransaction(string name)
        {
            if (this._trans == null)
                throw new ApplicationException("No transaction is current in process.");
            this.OpenTransaction.Rollback(name);
            this._trans.Dispose();
            this._trans = null;
            this.OnTransactionRollback(EventArgs.Empty);
        }
        public void SaveTransaction(string name)
        {
            if (this._trans == null)
                throw new ApplicationException("No transaction is current in process.");
            this.OpenTransaction.Save(name);
        }
        public int InsertDataRow(DataRow schemaRow, string tableName)
        { return base.InsertDataRow(schemaRow, tableName, AdoProviderType.SqlProvider); }
        public int UpdateDataRow(DataRow schemaRow, string tableName, string keyField)
        { return base.UpdateDataRow(schemaRow, tableName, keyField, AdoProviderType.SqlProvider); }
        public string[] GetDatabaseList()
        {
            this._qryExec = true;
            try
            {
                this.CheckConnectionReady();
                StringCollection dbCol = new StringCollection();
                using (SqlCommand cmd = new SqlCommand("SELECT name FROM master.dbo.sysdatabases ORDER BY name", this.DbConnection))
                using (SqlDataReader rdr = cmd.ExecuteReader())
                    while (rdr.Read())
                        dbCol.Add(rdr.GetValue(0).ToString());
                return dbCol.ToArray();
            }
            catch
            { throw; }
            finally
            {
                this._qryExec = false;
            }
        }
        public string[] GetSchemaList()
        {
            this._qryExec = true;
            try
            {
                this.CheckConnectionReady();
                StringCollection schemaCol = new StringCollection();
                using (SqlCommand cmd = new SqlCommand("SELECT name FROM sys.schemas ORDER BY name", this.DbConnection))
                using (SqlDataReader rdr = cmd.ExecuteReader())
                    while (rdr.Read())
                        schemaCol.Add(rdr.GetValue(0).ToString());
                return schemaCol.ToArray();
            }
            catch
            { throw; }
            finally
            {
                this._qryExec = false;
            }
        }
        public DataSet GetDatabaseInfo()
        {
            try
            { return this.GetDatabaseInfo(rsDb.GetInitialCatalog(this._connStr)); }
            catch
            { throw; }
        }
        public DataSet GetDatabaseInfo(string dbName)
        {
            this._qryExec = true;
            try
            {
                DataSet dsRetVal = new DataSet();
                using (SqlCommand cmd = new SqlCommand("USE master EXEC sp_helpdb '" + dbName + "'", this.DbConnection))
                {
                    cmd.CommandTimeout = 120;
                    cmd.CommandType = CommandType.Text;
                    using (SqlDataAdapter adp = new SqlDataAdapter(cmd))
                        adp.Fill(dsRetVal);
                }
                return dsRetVal;
            }
            catch
            { throw; }
            finally
            {
                this._qryExec = false;
            }
        }
        public string[] GetTableList()
        {
            try
            { return this.GetTableList(rsDb.GetInitialCatalog(this._connStr)); }
            catch
            { throw; }
        }
        public string[] GetTableList(string databaseName)
        {
            this._qryExec = true;
            try
            {
                this.CheckConnectionReady();
                StringCollection tblCol = new StringCollection();
                using (SqlCommand cmd = new SqlCommand(string.Format("SELECT (su.name + '.' + so.name) as [name] FROM {0}.sys.sysobjects so INNER JOIN {0}.sys.schemas su ON su.schema_id = so.uid WHERE xtype=N'U' ORDER BY so.name", databaseName), this.DbConnection))
                using (SqlDataReader rdr = cmd.ExecuteReader())
                    while (rdr.Read())
                        tblCol.Add(rdr.GetValue(0).ToString());
                return tblCol.ToArray();
            }
            catch
            { throw; }
            finally
            {
                this._qryExec = false;
            }
        }
        public string[] GetViewList()
        {
            try
            { return this.GetViewList(rsDb.GetInitialCatalog(this._connStr)); }
            catch
            { throw; }
        }
        public string[] GetViewList(string databaseName)
        {
            this._qryExec = true;
            try
            {
                this.CheckConnectionReady();
                StringCollection vCol = new StringCollection();
                using (SqlCommand cmd = new SqlCommand(string.Format("SELECT (su.name + '.' + so.name) as [name] FROM {0}.sys.sysobjects so INNER JOIN {0}.sys.schemas su ON su.schema_id = so.uid WHERE xtype=N'V' ORDER BY so.name", databaseName), this.DbConnection))
                using (SqlDataReader rdr = cmd.ExecuteReader())
                    while (rdr.Read())
                        vCol.Add(rdr.GetValue(0).ToString());
                return vCol.ToArray();
            }
            catch
            { throw; }
            finally
            {
                this._qryExec = false;
            }
        }
        public string[] GetStoredProceedureList()
        {
            try
            { return this.GetStoredProceedureList(rsDb.GetInitialCatalog(this._connStr)); }
            catch
            { throw; }
        }
        public string[] GetStoredProceedureList(string databaseName)
        {
            this._qryExec = true;
            try
            {
                this.CheckConnectionReady();
                StringCollection spCol = new StringCollection();
                using (SqlCommand cmd = new SqlCommand(string.Format("SELECT (su.name + '.' + so.name) as [name] FROM {0}.sys.sysobjects so INNER JOIN {0}.sys.schemas su ON su.schema_id = so.uid WHERE xtype=N'P' ORDER BY so.name", databaseName), this.DbConnection))
                using (SqlDataReader rdr = cmd.ExecuteReader())
                    while (rdr.Read())
                        spCol.Add(rdr.GetValue(0).ToString());
                return spCol.ToArray();
            }
            catch
            { throw; }
            finally
            {
                this._qryExec = false;
            }
        }
        public string[] GetTableValueFunctionList()
        {
            try
            { return this.GetTableValueFunctionList(rsDb.GetInitialCatalog(this._connStr)); }
            catch
            { throw; }
        }
        public string[] GetTableValueFunctionList(string databaseName)
        {
            this._qryExec = true;
            try
            {
                this.CheckConnectionReady();
                StringCollection fCol = new StringCollection();
                using (SqlCommand cmd = new SqlCommand(string.Format("SELECT (su.name + '.' + so.name) as [name] FROM {0}.sys.sysobjects so INNER JOIN {0}.sys.schemas su ON su.schema_id = so.uid WHERE xtype=N'IF' ORDER BY so.name", databaseName), this.DbConnection))
                using (SqlDataReader rdr = cmd.ExecuteReader())
                    while (rdr.Read())
                        fCol.Add(rdr.GetValue(0).ToString());
                return fCol.ToArray();
            }
            catch
            { throw; }
            finally
            {
                this._qryExec = false;
            }
        }
        public string[] GetScalarFunctionList()
        {
            try
            { return this.GetScalarFunctionList(rsDb.GetInitialCatalog(this._connStr)); }
            catch
            { throw; }
        }
        public string[] GetScalarFunctionList(string databaseName)
        {
            this._qryExec = true;
            try
            {
                this.CheckConnectionReady();
                StringCollection fCol = new StringCollection();
                using (SqlCommand cmd = new SqlCommand(string.Format("SELECT (su.name + '.' + so.name) as [name] FROM {0}.sys.sysobjects so INNER JOIN {0}.sys.schemas su ON su.schema_id = so.uid WHERE xtype=N'FN' ORDER BY so.name", databaseName), this.DbConnection))
                using (SqlDataReader rdr = cmd.ExecuteReader())
                    while (rdr.Read())
                        fCol.Add(rdr.GetValue(0).ToString());
                return fCol.ToArray();
            }
            catch
            { throw; }
            finally
            {
                this._qryExec = false;
            }
        }
        public string[] GetAllFunctionList()
        {
            try
            { return this.GetAllFunctionList(rsDb.GetInitialCatalog(this._connStr)); }
            catch
            { throw; }
        }
        public string[] GetAllFunctionList(string databaseName)
        {
            this._qryExec = true;
            try
            {
                StringCollection fCol = new StringCollection();
                foreach (string str in this.GetTableValueFunctionList(databaseName))
                    fCol.Add(str);
                foreach (string str in this.GetScalarFunctionList(databaseName))
                    fCol.Add(str);
                return fCol.ToArray();
            }
            catch
            { throw; }
            finally
            {
                this._qryExec = false;
            }
        }
        public string[] GetUserList()
        {
            try
            { return this.GetUserList(rsDb.GetInitialCatalog(this._connStr)); }
            catch
            { throw; }
        }
        public string[] GetUserList(string databaseName)
        {
            this._qryExec = true;
            try
            {
                StringCollection uCol = new StringCollection();
                using (SqlCommand cmd = new SqlCommand(string.Format("SELECT su.name FROM {0}.sys.sysusers su WHERE su.sid IS NOT NULL ORDER BY su.name", databaseName), this.DbConnection))
                using (SqlDataReader rdr = cmd.ExecuteReader())
                    while (rdr.Read())
                        uCol.Add(rdr.GetValue(0).ToString());
                return uCol.ToArray();
            }
            catch
            { throw; }
            finally
            {
                this._qryExec = false;
            }
        }
        public string[] GetPrimaryKeyList(string tableName)
        {
            try
            { return this.GetPrimaryKeyList(rsDb.GetInitialCatalog(this._connStr), tableName); }
            catch
            { throw; }
        }
        public string[] GetPrimaryKeyList(string databaseName, string tableName)
        {
            this._qryExec = true;
            try
            {
                // Try and parse the passed table name for a user/schema.
                string tblNm, usrNm;
                rsDbSql.ParseSchemaAndTableName(tableName, out tblNm, out usrNm);

                this.CheckConnectionReady();
                StringCollection pkCol = new StringCollection();
                using (SqlCommand cmd = new SqlCommand(string.Format("SELECT name FROM {0}.sys.sysobjects WHERE xtype=N'PK' AND parent_obj = (SELECT so.id FROM {0}.sys.sysobjects so INNER JOIN {0}.sys.schemas su ON su.schema_id = so.uid AND su.name = '{2}' WHERE xtype=N'U' AND so.name = '{1}') ORDER BY name", databaseName, tblNm, usrNm), this.DbConnection))
                using (SqlDataReader rdr = cmd.ExecuteReader())
                    while (rdr.Read())
                        pkCol.Add(rdr.GetValue(0).ToString());
                return pkCol.ToArray();
            }
            catch
            { throw; }
            finally
            {
                this._qryExec = false;
            }
        }
        public string[] GetForeignKeyList(string tableName)
        {
            try
            { return this.GetForeignKeyList(rsDb.GetInitialCatalog(this._connStr), tableName); }
            catch
            { throw; }
        }
        public string[] GetForeignKeyList(string databaseName, string tableName)
        {
            this._qryExec = true;
            try
            {
                string tblNm, usrNm;
                rsDbSql.ParseSchemaAndTableName(tableName, out tblNm, out usrNm);

                this.CheckConnectionReady();
                StringCollection fkCol = new StringCollection();
                using (SqlCommand cmd = new SqlCommand(string.Format("SELECT name FROM {0}.sys.sysobjects WHERE xtype=N'F' AND parent_obj = (SELECT so.id FROM {0}.sys.sysobjects so INNER JOIN {0}.sys.schemas su ON su.schema_id = so.uid AND su.name = '{2}' WHERE xtype=N'U' AND so.name='{1}') ORDER BY name", databaseName, tblNm, usrNm), this.DbConnection))
                using (SqlDataReader rdr = cmd.ExecuteReader())
                    while (rdr.Read())
                        fkCol.Add(rdr.GetValue(0).ToString());
                return fkCol.ToArray();
            }
            catch
            { throw; }
            finally
            {
                this._qryExec = false;
            }
        }
        public string[] GetTriggerList(string tableName)
        {
            try
            { return this.GetTriggerList(rsDb.GetInitialCatalog(this._connStr), tableName); }
            catch
            { throw; }
        }
        public string[] GetTriggerList(string databaseName, string tableName)
        {
            try
            { return this.GetTriggerList(databaseName, tableName, false); }
            catch
            { throw; }
        }
        public string[] GetTriggerList(string databaseName, string tableName, bool isView)
        {
            this._qryExec = true;
            try
            {
                string tblNm, usrNm;
                rsDbSql.ParseSchemaAndTableName(tableName, out tblNm, out usrNm);

                this.CheckConnectionReady();
                StringCollection trCol = new StringCollection();
                using (SqlCommand cmd = new SqlCommand(string.Format("SELECT name FROM {0}.sys.sysobjects WHERE xtype=N'TR' AND parent_obj = (SELECT so.id FROM {0}.sys.sysobjects so INNER JOIN {0}.sys.schemas su ON su.schema_id = so.uid AND su.name = '{2}' WHERE xtype=N'{3}' AND so.name='{1}') ORDER BY name", databaseName, tblNm, usrNm, (isView) ? "V" : "U"), this.DbConnection))
                using (SqlDataReader rdr = cmd.ExecuteReader())
                    while (rdr.Read())
                        trCol.Add(rdr.GetValue(0).ToString());
                return trCol.ToArray();
            }
            catch
            { throw; }
            finally
            {
                this._qryExec = false;
            }
        }
        public string[] GetConstraintList(string tableName)
        {
            try
            { return this.GetConstraintList(rsDb.GetInitialCatalog(this._connStr), tableName); }
            catch
            { throw; }
        }
        public string[] GetConstraintList(string databaseName, string tableName)
        {
            this._qryExec = true;
            try
            {
                string tblNm, usrNm;
                rsDbSql.ParseSchemaAndTableName(tableName, out tblNm, out usrNm);

                this.CheckConnectionReady();
                StringCollection cnCol = new StringCollection();
                using (SqlCommand cmd = new SqlCommand(string.Format("SELECT name FROM {0}.sys.sysobjects WHERE xtype=N'D' AND parent_obj = (SELECT so.id FROM {0}.sys.sysobjects so INNER JOIN {0}.sys.schemas su ON su.schema_id = so.uid AND su.name = '{2}' WHERE xtype=N'U' AND so.name='{1}') ORDER BY name", databaseName, tblNm, usrNm), this.DbConnection))
                using (SqlDataReader rdr = cmd.ExecuteReader())
                    while (rdr.Read())
                        cnCol.Add(rdr.GetValue(0).ToString());
                return cnCol.ToArray();
            }
            catch
            { throw; }
            finally
            {
                this._qryExec = false;
            }
        }
        public string[] GetIndexList(string tableName)
        {
            try
            { return this.GetIndexList(rsDb.GetInitialCatalog(this._connStr), tableName); }
            catch
            { throw; }
        }
        public string[] GetIndexList(string databaseName, string tableName)
        {
            try
            { return this.GetIndexList(databaseName, tableName, false, false); }
            catch
            { throw; }
        }
        public string[] GetIndexList(string databaseName, string tableName, bool isView)
        {
            try
            { return this.GetIndexList(databaseName, tableName, isView, false); }
            catch
            { throw; }
        }
        public string[] GetIndexList(string databaseName, string tableName, bool isView, bool allIdxs)
        {
            this._qryExec = true;
            try
            {
                string tblNm, usrNm;
                rsDbSql.ParseSchemaAndTableName(tableName, out tblNm, out usrNm);

                this.CheckConnectionReady();
                StringCollection idxCol = new StringCollection();
                using (SqlCommand cmd = new SqlCommand(string.Format("SELECT name FROM {0}.sys.sysindexes WHERE id = (SELECT so.id FROM {0}.sys.sysobjects so INNER JOIN {0}.sys.schemas su ON su.schema_id = so.uid AND su.name = '{2}' WHERE xtype=N'{3}' AND so.name='{1}') ORDER BY name", databaseName, tblNm, usrNm, (isView) ? "V" : "U"), this.DbConnection))
                using (SqlDataReader rdr = cmd.ExecuteReader())
                    while (rdr.Read())
                    {
                        string rowVal = rdr.GetString(0);
                        if (allIdxs || (!rowVal.StartsWith("_dta_") && !rowVal.StartsWith("_WA_sys_")))
                            idxCol.Add(rowVal);
                    }
                return idxCol.ToArray();
            }
            catch
            { throw; }
            finally
            {
                this._qryExec = false;
            }
        }
        public string[] GetColumnList(string tableName)
        {
            try
            { return this.GetColumnList(rsDb.GetInitialCatalog(this._connStr), tableName); }
            catch
            { throw; }
        }
        public string[] GetColumnList(string databaseName, string tableName)
        {
            try
            { return this.GetColumnList(databaseName, tableName, false); }
            catch
            { throw; }
        }
        public string[] GetColumnList(string databaseName, string tableName, bool isView)
        {
            this._qryExec = true;
            try
            {
                string tblNm, usrNm;
                rsDbSql.ParseSchemaAndTableName(tableName, out tblNm, out usrNm);

                this.CheckConnectionReady();
                StringCollection colmnCol = new StringCollection();
                string qry = string.Format("SELECT sc.name FROM {0}.sys.syscolumns sc INNER JOIN {0}.sys.sysobjects so ON sc.id = so.id AND so.id = (SELECT so.id FROM {0}.sys.sysobjects so INNER JOIN {0}.sys.schemas su ON su.schema_id = so.uid AND su.name = '{2}' WHERE so.name = '{1}' AND so.xtype=N'{3}') ORDER BY sc.colorder", databaseName, tblNm, usrNm, (!isView) ? "U" : "V");
                using (SqlCommand cmd = new SqlCommand(qry, this.DbConnection))
                using (SqlDataReader rdr = cmd.ExecuteReader())
                    while (rdr.Read())
                        colmnCol.Add(rdr.GetValue(0).ToString());
                return colmnCol.ToArray();
            }
            catch
            { throw; }
            finally
            {
                this._qryExec = false;
            }
        }
        public ColumnParamsCollection GetColumns(string tableName)
        {
            try
            { return this.GetColumns(rsDb.GetInitialCatalog(this._connStr), tableName); }
            catch
            { throw; }
        }
        public ColumnParamsCollection GetColumns(string databaseName, string tableName)
        {
            try
            { return this.GetColumns(databaseName, tableName, false); }
            catch
            { throw; }
        }
        public ColumnParamsCollection GetColumns(string databaseName, string tableName, bool isView)
        {
            this._qryExec = true;
            try
            {
                string tblNm, usrNm;
                rsDbSql.ParseSchemaAndTableName(tableName, out tblNm, out usrNm);

                this.CheckConnectionReady();
                ColumnParamsCollection cols = new ColumnParamsCollection(null);
                string qry = string.Format("SELECT sc.name, st.name, sc.colorder, sc.length, sc.colstat, sc.isnullable, so.uid FROM {0}.sys.syscolumns sc INNER JOIN {0}.sys.sysobjects so ON sc.id = so.id AND so.id = (SELECT so2.id FROM {0}.sys.sysobjects so2 INNER JOIN {0}.sys.schemas su2 ON su2.schema_id = so2.uid AND su2.name = '{2}' WHERE so2.name = '{1}' AND so2.xtype=N'{3}') INNER JOIN {0}.dbo.systypes st ON st.xtype = sc.xtype ORDER BY sc.colorder", databaseName, tblNm, usrNm, (!isView) ? "U" : "V");
                using (SqlCommand cmd = new SqlCommand(qry, this.DbConnection))
                using (SqlDataReader rdr = cmd.ExecuteReader())
                    while (rdr.Read())
                    {
                        ColumnParams cp = new ColumnParams(); //rdr.GetValue(0).ToString(), rdr.GetValue(1).ToString(), int.Parse(rdr.GetValue(3).ToString()));
                        cp.ColumnName = rdr.GetValue(0).ToString();
                        SqlDbType colType = SqlDbType.Variant;
                        try { colType = (SqlDbType)Enum.Parse(typeof(SqlDbType), rdr.GetValue(1).ToString(), true); }
                        catch { colType = SqlDbType.Variant; }
                        cp.DataType = colType;
                        int fldSz;
                        if (!int.TryParse(rdr.GetValue(3).ToString(), out fldSz))
                            fldSz = -1;
                        cp.FieldSize = fldSz;
                        cp.IsIdentity = (rdr.GetValue(4).ToString() == "1");
                        cp.IsNullable = (rdr.GetValue(5).ToString() == "1");
                        cp.SetOrdinal(int.Parse(rdr.GetValue(2).ToString()));
                        string colKeyBase = string.Format("{0}.{1}.{2}.{3}", databaseName, usrNm, tblNm, rdr.GetValue(0));
                        string colKey = colKeyBase;
                        int colKeyCnt = 1;
                        while (cols.ContainsKey(colKey))
                            colKey = colKeyBase + colKeyCnt.ToString().PadLeft(2, '0');
                        cols.Add(cp, colKey);
                    }
                return cols;
            }
            catch
            { throw; }
            finally
            {
                this._qryExec = false;
            }
        }
        public string[] SearchForStoredProceedure(string srchName)
        { return this.SearchForStoredProceedure(srchName, false); }
        public string[] SearchForStoredProceedure(string srchName, bool continueOnPermError)
        {
            this._qryExec = true;
            try
            {
                return this.SearchForObjName("GetStoredProceedureList", srchName, continueOnPermError);
            }
            catch
            { throw; }
            finally
            {
                this._qryExec = false;
            }
        }
        public string[] SearchForTable(string srchName)
        { return this.SearchForTable(srchName, false); }
        public string[] SearchForTable(string srchName, bool continueOnPermError)
        {
            this._qryExec = true;
            try
            { return this.SearchForObjName("GetTableList", srchName, continueOnPermError); }
            catch
            { throw; }
            finally
            { this._qryExec = false; }
        }
        public string[] SearchForView(string srchName)
        { return this.SearchForView(srchName, false); }
        public string[] SearchForView(string srchName, bool continueOnPermError)
        {
            this._qryExec = true;
            try
            { return this.SearchForObjName("GetViewList", srchName, continueOnPermError); }
            catch
            { throw; }
            finally
            { this._qryExec = false; }
        }
        public string[] SearchForFunction(string srchName)
        { return this.SearchForFunction(srchName, false); }
        public string[] SearchForFunction(string srchName, bool continueOnPermError)
        {
            this._qryExec = true;
            try
            { return this.SearchForObjName("GetAllFunctionList", srchName, continueOnPermError); }
            catch
            { throw; }
            finally
            { this._qryExec = false; }
        }
        public string[] GetObjectTextCreate(string spName)
        {
            try
            { return this.GetObjectTextCreate(rsDb.GetInitialCatalog(this._connStr), spName); }
            catch
            { throw; }
        }
        public string[] GetObjectTextCreate(string databaseName, string spName)
        {
            this._qryExec = true;
            try
            {
                this.CheckConnectionReady();
                //if (spName.IndexOf('.') > -1)
                //    spName = spName.Substring(spName.LastIndexOf('.') + 1);
                spName = spName.Replace("'", "''");
                string qry = string.Format("USE {0} EXEC sp_helptext '{1}'", databaseName, spName);
                StringCollection returnText = new StringCollection();
                using (SqlCommand cmd = new SqlCommand(qry, this.DbConnection))
                using (SqlDataReader rdr = cmd.ExecuteReader())
                    while (rdr.Read())
                        returnText.Add(rdr.GetString(0).TrimEnd());
                return returnText.ToArray();
            }
            catch
            { throw; }
            finally
            {
                this._qryExec = false;
            }
        }
        public string[] GetObjectTextAlter(string spName)
        {
            try
            { return this.GetObjectTextAlter(rsDb.GetInitialCatalog(this._connStr), spName); }
            catch
            { throw; }
        }
        public string[] GetObjectTextAlter(string databaseName, string spName)
        {
            string[] val = this.GetObjectTextCreate(databaseName, spName);
            for (int i = 0; i < val.Length; i++)
                if (val[i].ToUpper().StartsWith("CREATE"))
                {
                    val[i] = val[i].Replace("CREATE", "ALTER");
                    break;
                }
            return val;
        }
        public string[] ScriptCreateTable(string tableName)
        {
            try
            { return this.ScriptCreateTable(rsDb.GetInitialCatalog(this._connStr), tableName); }
            catch
            { throw; }
        }
        public string[] ScriptCreateTable(string databaseName, string tableName)
        { return this.ScriptCreateTable(databaseName, tableName, true); }
        public string[] ScriptCreateTable(string databaseName, string tableName, bool scriptUsing)
        {
            // Trim any qualifiers off the table name. We'll be determining the
            //   owner name for ourselves and they shouldn't be specifying
            //   the database name with a qualifier.
            string spTblNm = (tableName.IndexOf('.') > -1) ? tableName.Substring(tableName.LastIndexOf('.') + 1) : tableName;
            spTblNm = spTblNm.Trim('[', ']', ' ', '.');
            string curDbName = string.Empty;
            if (this.DbConnection.Database != databaseName)
            {
                curDbName = this.DbConnection.Database;
                this.DbConnection.ChangeDatabase(databaseName);
            }

            List<string> sql = new List<string>();
            try
            {
                using (DataSet ds = this.Execute(string.Format("EXEC sp_help N'{0}'", tableName)))
                {
                    // Determine the table owner
                    string tblOwner = ds.Tables[0].Rows[0]["Owner"].ToString();

                    if (scriptUsing)
                    {
                        sql.Add(string.Format("USE [{0}]", databaseName));
                        sql.Add("GO");
                    }
                    else
                        sql.Add("");
                    sql.Add(string.Format("/****** Object: Table [{2}].[{0}]    Script Date: {1} ******/", spTblNm, DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"), tblOwner));
                    sql.Add("SET ANSI_NULLS ON");
                    sql.Add("GO");
                    sql.Add("SET QUOTED_IDENTIFIER ON");
                    sql.Add("GO");
                    sql.Add("SET ANSI_PADDING ON");
                    sql.Add("GO");

                    #region Build Table Script
                    // Start the script.
                    sql.Add(string.Format("CREATE Table [{0}].[{1}](", spTblNm, tblOwner));

                    // Build the column definitions.
                    ColumnParamsCollection colParams = this.GetColumns(databaseName, tableName);
                    foreach (ColumnParams col in colParams)
                    {
                        string dataTypeNm = col.DataType.ToString().ToLower();
                        sql.Add(string.Format("\t[{0}] [{1}]{2} {3} NULL {4},", col.ColumnName, col.DataType, (dataTypeNm.StartsWith("var") || dataTypeNm.StartsWith("nvar")) ? " (" + col.FieldSize + ")" : "", (col.IsNullable) ? "" : "NOT", (col.DefaultValue != null) ? " DEFAULT (" + col.DefaultValue.ToString() + ")" : "").TrimEnd());
                    }
                    // Remove the comma from the end of the last column entry.
                    sql[sql.Count - 1] = sql[sql.Count - 1].ToString().TrimEnd(',');

                    // If there is a primary key, build the script for it.
                    if (ds.Tables.Count > 5 && ds.Tables[5].Rows.Count > 0)
                    {
                        DataRow drPk = null;
                        for (int i = 0; i < ds.Tables[5].Rows.Count; i++)
                            if (ds.Tables[5].Rows[i]["index_name"].ToString().ToUpper().StartsWith("PK_"))
                            { drPk = ds.Tables[5].Rows[i]; break; }
                        if (drPk != null)
                        {
                            string indexDesc = drPk["index_description"].ToString();
                            string type = indexDesc.Substring(0, indexDesc.IndexOfAny(new char[] { ',', ' ' }));
                            string loc = indexDesc.Substring(indexDesc.ToLower().IndexOf("located on") + 11);
                            bool isUnique = indexDesc.ToLower().Contains(", unique");
                            sql.Add(string.Format("CONSTRAINT [{0}] PRIMARY KEY {1}", drPk["index_name"], type.ToUpper()));
                            string[] cols = drPk["index_keys"].ToString().Split(',');
                            for (int i = 0; i < cols.Length; i++)
                            {
                                bool sortDesc = (cols[i].IndexOf('(') > -1);
                                string colNm = (sortDesc) ? cols[i].Substring(0, cols[i].IndexOf('(')) : cols[i];
                                sql.Add(string.Format("\t[{0}] {1}{2}", colNm.Trim(), (sortDesc) ? "DESC" : "ASC", (i < cols.Length - 1) ? "," : ""));
                            }
                            sql.Add(string.Format("){0} ON [{1}]", (isUnique) ? " WITH (IGNORE_DUP_KEY = OFF)" : "", loc));
                        }
                    }
                    // Close the table create script.
                    sql.Add(") ON [PRIMARY]");
                    #endregion

                    #region Build the Foreign Keys Script
                    using (DataSet dsFk = this.Execute(string.Format("exec sp_fkeys NULL, N'{0}', NULL, N'{1}'", tblOwner, spTblNm)))
                    {
                        if (dsFk != null && dsFk.Tables.Count > 0 && dsFk.Tables[0].Rows.Count > 0)
                        {
                            sql.Add("");
                            sql.Add("GO");
                            sql.Add("SET ANSI_PADDING OFF");
                            sql.Add("GO");
                            if (scriptUsing)
                            {
                                sql.Add(string.Format("USE [{0}]", databaseName));
                                sql.Add("GO");
                            }
                            for (int i = 0; i < dsFk.Tables[0].Rows.Count; i++)
                                sql.AddRange(this.GetFKeyCreateScript(dsFk.Tables[0].Rows[i], spTblNm));
                        }
                    }
                    #endregion

                    #region Build Table Indices
                    // By default, indexes do not get built into the script in SQL
                    //using (DataSet dsIdx = this.Execute("exec sp_helpindex N'" + spTblNm + "'"))
                    //    if (dsIdx != null && dsIdx.Tables.Count > 0)
                    //        for (int i = 0; i < dsIdx.Tables[0].Rows.Count; i++)
                    //        {
                    //            string idxName = dsIdx.Tables[0].Rows[i][0].ToString().ToLower();
                    //            string idxDesc = dsIdx.Tables[0].Rows[i][1].ToString().ToLower();
                    //            if (!idxName.ToUpper().StartsWith("PK_") && !idxDesc.Contains("auto create") && !idxName.StartsWith("_dta_index_"))
                    //            {
                    //                sql.Add("");
                    //                sql.AddRange(this.GetIndexCreateScript(dsIdx.Tables[0].Rows[i], spTblNm));
                    //            }
                    //        }
                    #endregion
                }
            }
            finally
            {
                if (!string.IsNullOrEmpty(curDbName))
                    this.DbConnection.ChangeDatabase(curDbName);
            }

            return sql.ToArray();
        }
        public string[] ScriptAlterTable(string tableName)
        {
            try
            { return this.ScriptAlterTable(rsDb.GetInitialCatalog(this._connStr), tableName); }
            catch
            { throw; }
        }
        public string[] ScriptAlterTable(string databaseName, string tableName)
        { return this.ScriptAlterTable(databaseName, tableName, true); }
        public string[] ScriptAlterTable(string databaseName, string tableName, bool scriptUsing)
        {
            string[] lns = this.ScriptCreateTable(databaseName, tableName, scriptUsing);
            for (int i = 0; i < lns.Length; i++)
                if (lns[i].Contains("CREATE Table "))
                    lns[i] = lns[i].Replace("CREATE Table ", " ALTER Table ");
            return lns;
        }
        public string[] ScriptCreateIndex(string tableName, string indexName)
        {
            try
            { return this.ScriptCreateIndex(rsDb.GetInitialCatalog(this._connStr), tableName, indexName); }
            catch
            { throw; }
        }
        public string[] ScriptCreateIndex(string databaseName, string tableName, string indexName)
        {
            string curCatalog = string.Empty;
            try
            {
                if (tableName.Contains("."))
                    tableName = tableName.Substring(tableName.LastIndexOf('.') + 1);
                if (databaseName != this.DbConnection.Database)
                {
                    curCatalog = this.DbConnection.Database;
                    this.DbConnection.ChangeDatabase(databaseName);
                }
                using (DataSet ds = this.Execute("exec sp_helpindex N'" + tableName + "'", CommandType.Text))
                {
                    if (ds == null || ds.Tables.Count < 1)
                        throw new Exception("No indexes found for selected object.");

                    DataRow[] drNm = ds.Tables[0].Select("index_name = '" + indexName + "'");
                    if (drNm.Length < 1)
                        throw new Exception("No index found matching given index name.");
                    else if (drNm.Length > 1)
                        // We should *never* be able to reach this line of code, since SQL
                        //   shouldn't allow two indexes with the same name to exist on
                        //   a single table.
                        throw new Exception("Multiple indexes found matching given index name.");
                    else
                    {
                        ArrayList sql = new ArrayList();
                        sql.Add(string.Format("USE [{0}]", databaseName));
                        sql.Add("GO");
                        sql.Add(string.Format("/****** Object: Index [{0}]   Script Date: {1} ******/", indexName, DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss")));
                        sql.AddRange(this.GetIndexCreateScript(drNm[0], tableName));
                        return (string[])sql.ToArray(typeof(System.String));
                    }
                }
            }
            catch
            { throw; }
            finally
            {
                // If we changed database's to execute the stored procedure,
                //   make sure we switch the connectin's default database
                //   back to it original value.
                if (!string.IsNullOrEmpty(curCatalog))
                    this.DbConnection.ChangeDatabase(curCatalog);
            }
        }
        public string[] ScriptCreateForeignKey(string databaseName, string tableName, string fkeyName)
        {
            try
            {
                if (tableName.Contains("."))
                    tableName = tableName.Substring(tableName.LastIndexOf('.') + 1);
                using (DataSet ds = this.Execute(string.Format("exec sp_fkeys NULL, N'dbo', NULL, '{0}'", tableName, CommandType.Text)))
                {
                    if (ds == null || ds.Tables.Count < 1)
                        throw new Exception("No indexes found for selected object.");

                    DataRow[] drNm = ds.Tables[0].Select(string.Format("fk_name = '{0}'", fkeyName));
                    if (drNm.Length < 1)
                        throw new Exception("No foreign key found matching given key name.");
                    else if (drNm.Length > 1)
                        // We shouldn't be able to reach this line of code, since SQL should never
                        //   allow two keys with the same name on a any given table.
                        throw new Exception("Multiple indexes found matching given foreign key name.");
                    else
                    {
                        ArrayList sql = new ArrayList();
                        sql.Add(string.Format("USE [{0}]", databaseName));
                        sql.Add("GO");
                        sql.Add(string.Format("/****** Object: Foreign Key [{0}]   Script Date: {1} ******/", fkeyName, DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss")));
                        sql.AddRange(this.GetFKeyCreateScript(drNm[0], tableName));
                        return (string[])sql.ToArray(typeof(System.String));
                    }
                }
            }
            catch
            { throw; }
        }
        public int GetLastIdentity()
        {
            return (int)this.ExecuteScalar("SELECT @@IDENTITY");
        }
        public bool TableExists(string tableName)
        {
            string tblNm = tableName;
            if (tblNm.IndexOf('.') > -1)
                tblNm = tableName.Substring(0, tableName.LastIndexOf('.'));
            tblNm = tblNm.Trim('[', ']', ' ', '.');
            string tblOwner;
            try
            { tblOwner = this.GetObjectOwner(tblNm); }
            catch (Exception ex)
            {
                if (ex.Message == "The specified table does not exist in the database.")
                    return false;
                else
                    throw;
            }
            return rsDbSql.TableExists(this.DbConnection, tblNm, tblOwner, this.OpenTransaction);
        }
        public string CreateTable(SqlTableParams tableFormat)
        {
            try
            { return rsDbSql.CreateTable(tableFormat, this.DbConnection, this.OpenTransaction); }
            catch
            { throw; }
        }
        //***************************************************************************
        // Static Methods
        // 
        public static void ParseSchemaAndTableName(string name, out string tableName, out string schemaName)
        {
            string[] tblParts = name.Split('.');
            if (tblParts.Length == 1)
            { tableName = name; schemaName = "dbo"; }
            else if (tblParts.Length > 2)
            { throw new ArgumentException("Table name parameter is not correctly formed.", "name"); }
            else
            { tableName = tblParts[1]; schemaName = tblParts[0]; }
        }
        public static bool TableExists(SqlConnection conn, string tableName)
        { return rsDbSql.TableExists(conn, tableName, "dbo"); }
        public static bool TableExists(SqlConnection conn, string tableName, string schemaName)
        { return TableExists(conn, tableName, schemaName, null); }
        public static bool TableExists(SqlConnection conn, string tableName, string schemaName, SqlTransaction transaction)
        {
            try
            {
                if (conn.State != ConnectionState.Open)
                    throw new ArgumentException("Connection must be valid and in an 'Open' state.", "conn");

                string sQry = string.Format("SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[{0}].[{1}]') and OBJECTPROPERTY(id, N'IsUserTable') = 1", schemaName, tableName);
                using (SqlCommand cmd = new SqlCommand(sQry, conn))
                {
                    if (transaction != null)
                        cmd.Transaction = transaction;
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                        return rdr.HasRows;
                }
            }
            catch
            { throw; }
        }
        public static string CreateTable(SqlTableParams tableFormat, SqlConnection conn)
        { return rsDbSql.CreateTable(tableFormat, conn, null); }
        public static string CreateTable(SqlTableParams tableFormat, SqlConnection conn, SqlTransaction transaction)
        {
            try
            {
                if (conn.State != ConnectionState.Open)
                    throw new ArgumentException("Connection must be valid and in an 'Open' state.", "conn");

                // Check to see if the specified table already exists on the SQL server.
                if (rsDbSql.TableExists(conn, tableFormat.TableName, tableFormat.TableOwner, transaction))
                    throw new Exception("Specified table already exists");

                // Build the create table script.
                StringBuilder sb = new StringBuilder();
                //sb.AppendLine(string.Format("USE {0}", tableFormat.DatabaseName));
                sb.AppendLine(string.Format("CREATE TABLE [{0}].[{1}] ( ", tableFormat.TableOwner, tableFormat.TableName));

                // Loop through the tableFormat's column list
                for (int i = 0; i < tableFormat.Columns.Count; i++)
                {
                    ColumnParams col = tableFormat.Columns[i];
                    sb.AppendLine(string.Format("\t{0}[{1}] [{2}] {3} {4} {5} NULL {5}",
                        (i > 0) ? "," : " ",
                        col.ColumnName,
                        col.DataType.ToString(),
                        (col.DataType == SqlDbType.VarChar && (col.FieldSize > 0 || col.FieldSize == -2))
                                ? "(" + ((col.FieldSize == -2) ? "MAX" : col.FieldSize.ToString()) + ")"
                                : "",
                        (col.IsIdentity)
                                ? "IDENTITY(" + col.IdentitySeed.ToString() + "," + col.IdentityIncrement.ToString() + ")"
                                : "",
                        (col.IsNullable)
                                ? ""
                                : "NOT",
                        (col.DefaultValue != null)
                                ? "DEFAULT('" + col.DefaultValue.ToString() + "')"
                                : ""));
                }
                //sb.AppendLine(string.Format(") ON [{0}]", tableFormat.DatabaseName));
                sb.AppendLine(") ON [PRIMARY]");

                using (SqlCommand cmd = new SqlCommand(sb.ToString(), conn))
                {
                    if (transaction != null)
                        cmd.Transaction = transaction;
                    cmd.ExecuteNonQuery();
                }

                return sb.ToString();
            }
            catch (Exception ex)
            {
                DebugOnly.DebugWrite("ERROR: CreateTable -- " + ex.Message);
                throw;
            }
        }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        private string[] SearchForObjName(string funcName, string srchName)
        { return this.SearchForObjName(funcName, srchName, false); }
        private string[] SearchForObjName(string funcName, string srchName, bool continueOnPermError)
        {
            System.Reflection.MethodInfo miFunc = this.GetType().GetMethod(funcName, new Type[] { typeof(string) });
            this._qryExec = true;
            try
            {
                // Create a RegularExpression object so that we can easily search
                //   using wildcard characters ('*' or '%').
                string regExStr = "^" + srchName.Replace("*", ".*").Replace("%", ".*") + "$";
                System.Text.RegularExpressions.Regex regxWldCrd = new System.Text.RegularExpressions.Regex(regExStr, System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // Get a list of all databases for the given connection string.
                string[] dbList = this.GetDatabaseList();
                this._qryExec = true;

                // Create a StringCollection
                StringCollection tblCol = new StringCollection();
                foreach (string dbName in dbList)
                {
                    try
                    {
                        string[] tblList = (string[])miFunc.Invoke(this, new object[] { dbName });
                        this._qryExec = true;
                        foreach (string tblName in tblList)
                        {
                            try
                            {
                                // If the passed search string doesn't have a schema qualifier,
                                //   then remove the one from the current table name.
                                string tblNameQual = (tblName.IndexOf('.') > -1 && srchName.IndexOf('.') < 0)
                                                        ? tblName.Substring(tblName.IndexOf('.') + 1)
                                                        : tblName;

                                // If a match was found, add the table name to the collection
                                if (regxWldCrd.IsMatch(tblNameQual))
                                    tblCol.Add(dbName + "." + tblName);
                            }
                            catch (System.Exception ex)
                            {
                                if (ex.InnerException is System.Data.SqlClient.SqlException && continueOnPermError && ex.ToString().Contains("The server principal ") && ex.ToString().Contains(" is not able to access the database ") && ex.ToString().Contains(" under the current security context."))
                                {
                                    // We're going to ignore this error and continue
                                    //   searching whichever databases we *are*
                                    //   allowed to access.
                                }
                                else
                                    throw;
                            }
                        }
                    }
                    catch (System.Exception ex)
                    {
                        if (ex.InnerException is System.Data.SqlClient.SqlException && continueOnPermError && ex.ToString().Contains("The server principal ") && ex.ToString().Contains(" is not able to access the database ") && ex.ToString().Contains(" under the current security context."))
                        {
                            // We're going to ignore this error and continue
                            //   searching whichever databases we *are*
                            //   allowed to access.
                        }
                        else
                            throw;
                    }
                }
                // Return the collection as a flat string array.
                return tblCol.ToArray();
            }
            catch
            { throw; }
            finally
            {
                this._qryExec = false;
            }
        }
        protected override DbConnection CreateConnectionObject(string connStr)
        {
            return new SqlConnection(connStr);
        }
        protected override DbCommand CreateCommandObject(string sql)
        {
            SqlCommand cmd = new SqlCommand(sql, this.DbConnection);
            if (this._trans != null)
                cmd.Transaction = this.OpenTransaction;
            return cmd;
        }
        protected override DbDataAdapter CreateAdapterObject()
        {
            return new SqlDataAdapter(this.DbCommand);
        }
        private string GetObjectOwner(string tableName)
        {
            // Make sure we didn't get a table name with any qualifiers.
            if (tableName.IndexOf('.') > -1)
                throw new Exception("You cannot qualify the table name when requesting a table's owner.");

            string qryOwner = string.Format("SELECT su.name FROM sys.sysobjects so INNER JOIN sys.sysusers su ON su.uid = so.uid WHERE so.xtype = N'U' AND so.name = '{0}'", tableName);
            object objOwnerNm = this.ExecuteScalar(qryOwner);
            if (objOwnerNm == null)
                throw new Exception("The specified table does not exist in the database.");

            return objOwnerNm.ToString();
        }
        private string[] GetIndexCreateScript(DataRow dr, string tableName)
        {
            if (tableName.IndexOf('.') > -1)
                tableName = tableName.Substring(tableName.IndexOf('.')).Trim('[', ']', ' ', '.');
            string dbOwner = this.GetObjectOwner(tableName);

            List<string> sql = new List<string>();
            string createClause = string.Empty;
            string descCd = dr[1].ToString().ToLower();
            bool isUnique = dr[1].ToString().ToLower().Contains(", unique");
            if (descCd.StartsWith("nonclustered"))
            {
                createClause = string.Format("CREATE{3} NONCLUSTERED INDEX [{0}] ON [{1}].[{2}]", dr[0], dbOwner, tableName, (isUnique) ? " UNIQUE" : "");
            }
            else //if(descCd.StartsWith("clustered"))
            {
                createClause += string.Format("ALTER TABLE [{0}].[{1}] ADD CONSTRAINT [{2}] PRIMARY KEY CLUSTERED", dbOwner, tableName, dr[0]);
            }
            sql.Add(createClause);
            sql.Add("(");
            string[] cols = dr[2].ToString().Split(',');
            for (int i = 0; i < cols.Length; i++)
            {
                bool sortDesc = cols[i].IndexOf('(') > -1;
                string colNm = (sortDesc) ? cols[i].Substring(0, cols[i].IndexOf('(')) : cols[i];
                sql.Add(string.Format("\t[{0}] {1},", colNm.Trim(), (sortDesc) ? "DESC" : "ASC"));
            }
            sql[sql.Count - 1] = sql[sql.Count - 1].ToString().TrimEnd(',');
            sql.Add(string.Format(") ON [{0}]", dr[1].ToString().Substring(dr[1].ToString().LastIndexOf(' ') + 1)));

            return sql.ToArray();
        }
        private string[] GetFKeyCreateScript(DataRow dr, string tableName)
        {
            if (tableName.IndexOf('.') > -1)
                tableName = tableName.Substring(tableName.IndexOf('.')).Trim('[', ']', ' ', '.');
            string tblOwner = this.GetObjectOwner(tableName);

            List<string> sql = new List<string>();
            sql.Add(string.Format("ALTER TABLE [{0}].[{1}]  WITH CHECK ADD  CONSTRAINT [{2} FOREIGN KEY ({3})", tblOwner, tableName, dr["FK_NAME"], dr["FKCOLUMN_NAME"]));
            sql.Add(string.Format("REFERENCES [{0}].[{1}].[{2}] CHECK CONSTRAINT [{3}]", dr["PKTABLE_QUALIFIER"], dr["PKTABLE_OWNER"], dr["PKTABLE_NAME"], dr["PKCOLUMN_NAME"]));
            string
                updRule = dr["UPDATE_RULE"].ToString(),
                delRule = dr["DELETE_RULE"].ToString();
            if (updRule != "1")
                sql.Add("ON UPDATE " + ((updRule == "0") ? "CASCADE" : "SET NULL"));
            if (delRule != "1")
                sql.Add("ON DELETE " + ((delRule == "0") ? "CASCADE" : "SET NULL"));
            sql.Add("GO");

            return sql.ToArray();

            //ArrayList sql = new ArrayList();
            //sql.Add(string.Format("ALTER TABLE [{0}].[{1}]  WITH NOCHECK ADD  CONTSTRAINT [{2}] FOREIGN KEY([{3}])", dbOwner, tableName, dr["FK_NAME"], dr["FKCOLUMN_NAME"]));
            //sql.Add(string.Format("REFERENCES [{0}].[{1}] ([{2}])", this.GetObjectOwner(dr["PKTABLE_NAME"].ToString()), dr["PKTABLE_NAME"], dr["PKCOLUMN_NAME"]));
            //sql.Add("GO");
            //sql.Add(string.Format("ALTER TABLE [{0}].[{1}] CHECK CONSTRAINT [{2}]", dbOwner, dr["FKTABLE_NAME"], dr["FK_NAME"]));
            //return (string[])sql.ToArray(typeof(System.String));
        }
        #endregion
    }
    [Author("Unfried, Michael")]
    public class rsDbOle : rsDb
    {
        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public override AdoProviderType ProviderType
        { get { return AdoProviderType.OleProvider; } }
        public new OleDbConnection DbConnection
        { get { return (OleDbConnection)base.DbConnection; } }
        public new OleDbCommand DbCommand
        { get { return (OleDbCommand)base.DbCommand; } }
        public new OleDbDataReader DataReader
        { get { return (OleDbDataReader)base.DataReader; } }
        public new OleDbDataAdapter DataAdapter
        { get { return (OleDbDataAdapter)base.DataAdapter; } }
        //***************************************************************************
        // Private Properties
        // 
        private OleDbTransaction OpenTransaction
        { get { return (OleDbTransaction)this._trans; } }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public rsDbOle(OleDbConnection conn)
            : base()
        {
            this._dbConn = conn;
        }
        public rsDbOle(string connStr)
            : base()
        {
            this.InitConnection(connStr);
        }
        public rsDbOle(string connStr, string qryStr)
            : this(connStr)
        {
            this.InitCommand(qryStr);
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public new OleDbTransaction BeginTransaction()
        {
            this.CheckConnectionReady();
            this._trans = this.DbConnection.BeginTransaction();
            this.DbCommand.Transaction = this.OpenTransaction;
            this.OnTransactionStart(EventArgs.Empty);
            return this.OpenTransaction;
        }
        public OleDbTransaction BeginTransaction(IsolationLevel isoLvl)
        {
            this.CheckConnectionReady();
            this._trans = this.DbConnection.BeginTransaction(isoLvl);
            this.DbCommand.Transaction = this.OpenTransaction;
            this.OnTransactionStart(EventArgs.Empty);
            return this.OpenTransaction;
        }
        public void BeginNestedTransaction()
        {
            if (this._trans == null)
                throw new ApplicationException("No transaction is current in process.");
            this.OpenTransaction.Begin();
        }
        public void BeginNestedTransaction(IsolationLevel isoLvl)
        {
            if (this._trans == null)
                throw new ApplicationException("No transaction is current in process.");
            this.OpenTransaction.Begin(isoLvl);
        }
        public int InsertDataRow(DataRow schemaRow, string tableName)
        { return base.InsertDataRow(schemaRow, tableName, AdoProviderType.OleProvider); }
        public int UpdateDataRow(DataRow schemaRow, string tableName, string keyField)
        { return base.UpdateDataRow(schemaRow, tableName, keyField, AdoProviderType.OleProvider); }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        protected override DbConnection CreateConnectionObject(string connStr)
        { return new OleDbConnection(connStr); }
        protected override DbCommand CreateCommandObject(string sql)
        {
            OleDbCommand cmd = new OleDbCommand(sql, this.DbConnection);
            if (this._trans != null)
                cmd.Transaction = this.OpenTransaction;
            return cmd;
        }
        protected override DbDataAdapter CreateAdapterObject()
        { return new OleDbDataAdapter(this.DbCommand); }
        #endregion
    }
    [Author("Unfried, Michael")]
    public class rsDbOdbc : rsDb
    {
        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public override AdoProviderType ProviderType
        { get { return AdoProviderType.OdbcProvider; } }
        public new OdbcConnection DbConnection
        { get { return (OdbcConnection)base.DbConnection; } }
        public new OdbcCommand DbCommand
        { get { return (OdbcCommand)base.DbCommand; } }
        public new OdbcDataReader DataReader
        { get { return (OdbcDataReader)base.DataReader; } }
        public new OdbcDataAdapter DataAdapter
        { get { return (OdbcDataAdapter)base.DataAdapter; } }
        //***************************************************************************
        // Private Methods
        // 
        private OdbcTransaction OpenTransaction
        { get { return (OdbcTransaction)this._trans; } }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public rsDbOdbc(OdbcConnection conn)
            : base()
        {
            this._dbConn = conn;
        }
        public rsDbOdbc(string connStr)
            : base()
        {
            this.InitConnection(connStr);
        }
        public rsDbOdbc(string connStr, string qryStr)
            : this(connStr)
        {
            this.InitCommand(qryStr);
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public new OdbcTransaction BeginTransaction()
        {
            this.CheckConnectionReady();
            this._trans = this.DbConnection.BeginTransaction();
            this.DbCommand.Transaction = this.OpenTransaction;
            this.OnTransactionStart(EventArgs.Empty);
            return this.OpenTransaction;
        }
        public OdbcTransaction BeginTransaction(IsolationLevel isoLvl)
        {
            this.CheckCommandReady();
            this._trans = this.DbConnection.BeginTransaction(isoLvl);
            this.DbCommand.Transaction = this.OpenTransaction;
            this.OnTransactionStart(EventArgs.Empty);
            return this.OpenTransaction;
        }
        public int InsertDataRow(DataRow schemaRow, string tableName)
        { return base.InsertDataRow(schemaRow, tableName, AdoProviderType.OdbcProvider); }
        public int UpdateDataRow(DataRow schemaRow, string tableName, string keyField)
        { return base.UpdateDataRow(schemaRow, tableName, keyField, AdoProviderType.OdbcProvider); }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        protected override DbConnection CreateConnectionObject(string connStr)
        { return new OdbcConnection(connStr); }
        protected override DbCommand CreateCommandObject(string sql)
        {
            OdbcCommand cmd = new OdbcCommand(sql, this.DbConnection);
            if (this._trans != null)
                cmd.Transaction = this.OpenTransaction;
            return cmd;
        }
        protected override DbDataAdapter CreateAdapterObject()
        { return new OdbcDataAdapter(this.DbCommand); }
        #endregion
    }
}
