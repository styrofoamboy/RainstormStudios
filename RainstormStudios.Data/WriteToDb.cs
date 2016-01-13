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
using System.Collections;
using System.Text;
using RainstormStudios.Collections;

namespace RainstormStudios.Data
{
    [Author("Unfried, Michael")]
    public enum WriteToDbMethod
    {
        /// <summary>
        /// The BulkCopy method is only valid for SQL data connections.
        /// </summary>
        BulkCopy = 0,
        /// <summary>
        /// Attempts to BulkCopy the data 10,000 records at a time.
        /// </summary>
        IncrementalCopy,
        /// <summary>
        /// Uses the ADO.NET DataAdapter.Update method to load the data.
        /// </summary>
        AdapterWrite,
        /// <summary>
        /// Builds a dynamic T-SQL statement for each data record to be uploaded.
        /// </summary>
        LineByLine
    }
    [Author("Unfried, Michael")]
    public struct WriteToDbResults
    {
        #region Declarations
        //***********************************************************************
        // Global Variables
        // 
        private WriteToDbAttemptCollection
            _attempts;
        private bool
            _dbSrc;
        private string
            _srcCnnStr,
            _srcQry,
            _dstCnnStr,
            _dstTable;
        #endregion

        #region Public Properties
        //***********************************************************************
        // Public Properties
        // 
        public WriteToDbAttemptCollection Attempts
        { get { return this._attempts; } }
        public bool DataTableSource
        { get { return !this._dbSrc; } }
        public string SourceConnectionString
        { get { return this._srcCnnStr; } }
        public string SourceQuery
        { get { return this._srcQry; } }
        public string DestinationConnectionString
        { get { return this._dstCnnStr; } }
        public string DestinationTableName
        { get { return this._dstTable; } }
        #endregion

        #region Class Constructors
        //***********************************************************************
        // Class Constructors
        // 
        private WriteToDbResults(bool dbSrc, string srcCnn, string srcQry, string dstCnn, string dstTable)
        {
            this._attempts = new WriteToDbAttemptCollection();
            this._dbSrc = dbSrc;
            this._dstCnnStr = dstCnn;
            this._dstTable = dstTable;
            this._srcCnnStr = srcCnn;
            this._srcQry = srcQry;
        }
        public WriteToDbResults(string SrcCnnString, string SrcQuery, string DstCnnString, string DstTableName)
            : this(false, SrcCnnString, SrcQuery, DstCnnString, DstTableName)
        { }
        public WriteToDbResults(string DstCnnString, string DstTableName)
            : this(true, "", "", DstCnnString, DstTableName)
        { }
        #endregion
    }
    [Author("Unfried, Michael")]
    public struct WriteToDbAttempt
    {
        #region Declarations
        //***********************************************************************
        // Global Variables
        // 
        private ExceptionCollection
            _errList;
        private bool
            _success,
            _continue,
            _dstExist,
            _dstCreate,
            _dstTrun,
            _setMap;
        private WriteToDbMethod
            _type;
        private double
            _rows;
        #endregion

        #region Public Properties
        //***********************************************************************
        // Public Properties
        // 
        public ExceptionCollection ErrorCollection
        { get { return this._errList; } }
        public bool AttemptSuccess
        {
            get { return this._success; }
            set { this._success = value; }
        }
        public WriteToDbMethod WriteMethod
        {
            get { return this._type; }
            set { this._type = value; }
        }
        public double RowsProcessed
        {
            get { return this._rows; }
            set { this._rows = value; }
        }
        public bool TryingNextMethod
        {
            get { return this._continue; }
            set { this._continue = value; }
        }
        public bool DestinationTableExist
        {
            get { return this._dstExist; }
            set { this._dstExist = value; }
        }
        public bool DestinationTableCreated
        {
            get { return this._dstCreate; }
            set { this._dstCreate = value; }
        }
        public bool DestinationTruncated
        {
            get { return this._dstTrun; }
            set { this._dstTrun = value; }
        }
        public bool SetupColumnMappings
        {
            get { return this._setMap; }
            set { this._setMap = value; }
        }
        #endregion

        #region Class Constructors
        //***********************************************************************
        // Class Constructors
        // 
        public WriteToDbAttempt(WriteToDbMethod method, bool success, bool tryingNext, double rows)
        {
            this._errList = new ExceptionCollection();
            this._success = success;
            this._type = method;
            this._success = success;
            this._continue = tryingNext;
            this._rows = rows;
            this._dstCreate = false;
            this._dstExist = false;
            this._dstTrun = false;
            this._setMap = false;
        }
        public WriteToDbAttempt(WriteToDbMethod method)
            : this(method, false, false, -1)
        { }
        public WriteToDbAttempt(WriteToDbMethod method, bool success)
            : this(method, success, false, -1)
        { }
        public WriteToDbAttempt(WriteToDbMethod method, bool success, double rows)
            : this(method, success, false, rows)
        { }
        #endregion
    }
    [Author("Unfried, Michael")]
    public class WriteToDbAttemptCollection : CollectionBase
    {
        #region Public Properties
        //***********************************************************************
        // Public Properties
        // 
        public WriteToDbAttempt this[int index]
        {
            get { return (WriteToDbAttempt)this.List[index]; }
            set { this.List[index] = value; }
        }
        #endregion

        #region Class Constructors
        //***********************************************************************
        // Class Constructors
        // 
        public WriteToDbAttemptCollection()
            :base()
        { }
        public WriteToDbAttemptCollection(WriteToDbAttempt[] vals)
            : this()
        {
            foreach (WriteToDbAttempt atmpt in vals)
                this.Add(atmpt);
        }
        #endregion

        #region Public Methods
        //***********************************************************************
        // Public Methods
        // 
        public void Add(WriteToDbAttempt value)
        {
            this.List.Add(value);
        }
        public void Remove(WriteToDbAttempt value)
        {
            this.List.Remove(value);
        }
        public bool Contains(WriteToDbAttempt value)
        {
            return this.InnerList.Contains(value);
        }
        #endregion
    }
    [Author("Unfried, Michael")]
    public class WriteToDb
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private string
            _connStr,
            _tableName;
        private long
            _startRow,
            _rowCnt,
            _c;
        private int
            _incSize;
        private bool
            _trunFirst;
        private object 
            _srcDt;
        private DbMappingRules
            _mapRules;
        //***************************************************************************
        // Delegates
        // 
        private delegate bool BeginWriteDelegate(string cnnStr, string tableName, object srcData, DbMappingRules rules, bool trunFirst);
        //***************************************************************************
        // Public Events
        // 
        public event RowProcessedEventHandler RowProcessed;
        public event WriteToDbMethodCompleteEventHandler MethodComplete;
        public event WriteToDbOperationCompleteEventHandler OperationComplete;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public string DestConnectionString
        {
            get { return this._connStr; }
            set { this._connStr = value; }
        }
        public string DestTableName
        {
            get { return this._tableName; }
            set { this._tableName = value; }
        }
        public object DataSource
        { get { return this._srcDt; } }
        public DbMappingRules MappingRules
        { get { return this._mapRules; } }
        public bool TruncateFirst
        {
            get { return this._trunFirst; }
            set { this._trunFirst = value; }
        }
        public long StartRow
        {
            get { return this._startRow; }
            set { this._startRow = value; }
        }
        public long RowCount
        {
            get { return this._rowCnt; }
            set { this._rowCnt = value; }
        }
        public long RowsProcessed
        { get { return this._c; } }
        public int IncrementalUploadThreshold
        {
            get { return this._incSize; }
            set { this._incSize = value; }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        private WriteToDb(string connStr, string tblName, object srcData, bool truncate, DbMappingRules rules)
        {
            bool validDataSrc = false;
            foreach (Type t in srcData.GetType().GetInterfaces())
                if (t.Name == "IEnumerable")
                    validDataSrc = true;
            if (!validDataSrc)
                throw new ArgumentException("Specified datasource does not inherit the IEnumerable interface.", "srcData");

            this._connStr = connStr;
            this._tableName = tblName;
            this._srcDt = srcData;
            this._mapRules = rules;
            this._trunFirst = TruncateFirst;

            this._c = 0;
            this._startRow = -1;
            this._rowCnt = -1;
            this._incSize = 100;
        }
        /// <summary>
        /// Provides a single-call method for writing data to DB using a variety of data sources.
        /// </summary>
        private WriteToDb()
            : this("", "", null, false, null)
        { }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        #endregion
    }
}
