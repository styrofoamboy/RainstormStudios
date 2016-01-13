using System;
using System.Data;
using System.Collections;
using System.Text;

namespace RainstormStudios.Data
{
    public enum WriteToSqlMethod
    {
        DirectCopy = 0,
        IncrementalCopy,
        AdapterWrite,
        LineByLine
    }
    public struct WriteToSqlResults
    {
        #region Global Objects
        //***********************************************************************
        // Global Variables
        // 
        private WriteToSqlAttemptCollection
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
        public WriteToSqlAttemptCollection Attempts
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
        private WriteToSqlResults(bool dbSrc, string srcCnn, string srcQry, string dstCnn, string dstTable)
        {
            this._attempts = new WriteToSqlAttemptCollection();
            this._dbSrc = dbSrc;
            this._dstCnnStr = dstCnn;
            this._dstTable = dstTable;
            this._srcCnnStr = srcCnn;
            this._srcQry = srcQry;
        }
        public WriteToSqlResults(string SrcCnnString, string SrcQuery, string DstCnnString, string DstTableName)
            : this(false, SrcCnnString, SrcQuery, DstCnnString, DstTableName)
        { }
        public WriteToSqlResults(string DstCnnString, string DstTableName)
            : this(true, "", "", DstCnnString, DstTableName)
        { }
        #endregion
    }
    public struct WriteToSqlAttempt
    {
        #region Global Objects
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
        private WriteToSqlMethod 
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
        public WriteToSqlMethod WriteMethod
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
        public WriteToSqlAttempt(WriteToSqlMethod method, bool success, bool tryingNext, double rows)
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
        public WriteToSqlAttempt(WriteToSqlMethod method)
            : this(method, false, false, -1)
        { }
        public WriteToSqlAttempt(WriteToSqlMethod method, bool success)
            : this(method, success, false, -1)
        { }
        public WriteToSqlAttempt(WriteToSqlMethod method, bool success, double rows)
            : this(method, success, false, rows)
        { }
        #endregion
    }
    public class WriteToSqlAttemptCollection : CollectionBase
    {
        #region Public Properties
        //***********************************************************************
        // Public Properties
        // 
        public WriteToSqlAttempt this[int index]
        {
            get { return (WriteToSqlAttempt)this.List[index]; }
            set { this.List[index] = value; }
        }
        #endregion

        #region Class Constructors
        //***********************************************************************
        // Class Constructors
        // 
        public WriteToSqlAttemptCollection()
        { }
        #endregion

        #region Public Methods
        //***********************************************************************
        // Public Methods
        // 
        public void Add(WriteToSqlAttempt value)
        {
            this.List.Add(value);
        }
        public void Remove(WriteToSqlAttempt value)
        {
            this.List.Remove(value);
        }
        public bool Contains(WriteToSqlAttempt value)
        {
            return this.InnerList.Contains(value);
        }
        #endregion
    }
    public class WriteToSql
    {
        #region Global Objects
        //***************************************************************************
        // Private Fields
        // 

        #endregion
    }
}
