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
using RainstormStudios.Collections;

namespace RainstormStudios.Data
{
    [Author("Unfried, Michael")]
    public struct ColumnParams : ICloneable
    {
        #region Declarations
        //***************************************************************************
        // Public Fields
        // 
        public static readonly ColumnParams
            Empty;
        //***************************************************************************
        // Private Fields
        // 
        private ColumnParamsCollection
            _parent;
        private string
            _colName;
        private SqlDbType
            _dataType;
        private int
            _fieldSize,
            _colOrd,
            _idSeed,
            _idIncr;
        private bool
            _isNullable,
            _isIdent;
        private object
            _defValue;
        //***************************************************************************
        // Private Events
        // 
        //private event EventHandler OrdinalChanged;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public string ColumnName
        {
            get { return this._colName; }
            set { this._colName = value; }
        }
        public SqlDbType DataType
        {
            get { return this._dataType; }
            set { this._dataType = value; }
        }
        public int FieldSize
        {
            get { return this._fieldSize; }
            set { this._fieldSize = value; }
        }
        public int OrdinalPosition
        { get { return this._colOrd; } }
        public bool IsNullable
        {
            get { return this._isNullable; }
            set { this._isNullable = value; }
        }
        public bool IsIdentity
        {
            get { return this._isIdent; }
            set { this._isIdent = value; }
        }
        public int IdentitySeed
        {
            get { return this._idSeed; }
            set { this._idSeed = value; }
        }
        public int IdentityIncrement
        {
            get { return this._idIncr; }
            set { this._idIncr = value; }
        }
        public object DefaultValue
        {
            get { return this._defValue; }
            set { this._defValue = value; }
        }
        public ColumnParamsCollection Parent
        { get { return this._parent; } }
        public SqlTableParams Owner
        { get { return this._parent.Owner; } }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public ColumnParams(string ColumnName, Type DataType, int ColumnSize)
            : this(ColumnName, DataType, ColumnSize, -1)
        { }
        public ColumnParams(string ColumnName, Type DataType, int ColumnSize, int OrdinalPos)
            : this(ColumnName, rsData.GetSqlDataType(DataType), ColumnSize, OrdinalPos)
        { }
        public ColumnParams(string ColumnName, string ColumnType, int ColumnSize)
            : this(ColumnName, ColumnType, ColumnSize, -1)
        { }
        public ColumnParams(string ColumnName, string ColumnType, int ColumnSize, int OrdinalPos)
            : this(ColumnName, (SqlDbType)Enum.Parse(typeof(SqlDbType), ColumnType, true), ColumnSize, OrdinalPos)
        { }
        public ColumnParams(string ColumnName, SqlDbType ColumnType, int ColumnSize)
            : this(ColumnName, ColumnType, ColumnSize, -1)
        { }
        public ColumnParams(string colName, SqlDbType dataType, int fieldSize, int ordinalPos)
        {
            this._colName = colName;
            this._dataType = dataType;
            this._fieldSize = fieldSize;
            this._colOrd = ordinalPos;
            //this.OrdinalChanged = null;
            this._isNullable = false;
            this._isIdent = false;
            this._idSeed = -1;
            this._idIncr = -1;
            this._defValue = null;
            this._parent = null;
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public object Clone()
        {
            return this.MemberwiseClone();
        }
        public override string ToString()
        {
            string strFmt = string.Empty;
            if (this._dataType == SqlDbType.VarChar || this._dataType == SqlDbType.NVarChar)
                strFmt = "{0} ({1}({2}), {3}null)";
            else
                strFmt = "{0} ({1}, {3}null)";
            return string.Format(strFmt, this._colName, this._dataType.ToString(), this._fieldSize, (!this._isNullable) ? "not " : "");
        }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Internal Methods
        // 
        internal void SetOrdinal(int ord)
        {
            this._colOrd = ord;
        }
        internal void SetParent(ColumnParamsCollection parent)
        {
            if (parent != null && this._parent != null && this._parent != parent)
                throw new Exception("This ColumParams object already belongs to another SqlTableParams columns collection.");

            this._parent = parent;
        }
        //***************************************************************************
        // Event Triggers
        // 
        //private void OrdinalChangedEvent()
        //{
        //    if (this.OrdinalChanged != null)
        //        this.OrdinalChanged.Invoke(this, EventArgs.Empty);
        //}
        #endregion
    }
    [Author("Unfried, Michael")]
    public class ColumnParamsCollection : ObjectCollectionBase<ColumnParams>, ICloneable
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private SqlTableParams
            _owner;
        private Int32Collection
            _colOrds;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public SqlTableParams Owner
        { get { return this._owner; } }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        //
        internal ColumnParamsCollection(SqlTableParams owner)
        {
            this._owner = owner;
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public object Clone()
        {
            object copy = this.MemberwiseClone();
            ((ColumnParamsCollection)copy).SetOwner(null);
            return copy;
        }
        public string Add(ColumnParams value)
        {
            value.SetParent(this);
            return base.Add(value, "");
        }
        public void Add(ColumnParams value, string key)
        {
            value.SetParent(this);
            base.Add(value, key);
        }
        public string Insert(int index, ColumnParams value)
        {
            value.SetParent(this);
            return base.Insert(index, value, "");
        }
        public void Insert(int index, ColumnParams value, string key)
        {
            value.SetParent(this);
            base.Insert(index, value, key);
        }
        public void Remove(ColumnParams value)
        {
            int idx = this.IndexOf(value);
            if (idx < 0)
                throw new ArgumentException("Could not find selected ColumnParams object in the collection.", "value");
            value.SetParent(null);
            this.RemoveAt(idx);
        }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        internal void SetOwner(SqlTableParams owner)
        {
            if (owner != null && this._owner != null && this._owner != owner)
                throw new Exception("This ColumnParamsCollection already belongs to another SqlTableParams object.");
            this._owner = owner;
        }
        #endregion

        #region Event Overrides
        //***************************************************************************
        // Event Overrides
        // 
        protected override void OnInsert(int index, object value)
        {
            base.OnInsert(index, value);

            //if (this.Count > 0)
            //{
            //    // We have to check for conflicting column ordinals.
            //    // First, grab the ordinal position of the new column.
            //    int newOrd = this[index].OrdinalPosition;
            //    // Then check to see if that value already exists in the
            //    //   ordinal collection.
            //    if (this._colOrds.Contains(newOrd))
            //    {
            //        // If it does, we need to do some shuffling.
            //        this._colOrds.Add(this._colOrds[this._colOrds.Count - 1]);
            //        for (int i = this._colOrds.Count - 2; i > newOrd; i--)
            //            this._colOrds[i + 1] = this._colOrds[i];
            //    }
            //}
            //this._colOrds.Add(newOrd, this[index].ColumnName);
        }
        #endregion
    }
    [Author("Unfried, Michael")]
    public class SqlTableParams
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private static readonly char[]
            trimChars = new char[] { '[', ']', ' ', '.' };
        private ColumnParamsCollection
            _cols;
        private string
            _tName,
            _oName,
            _dbName;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public string TableName
        {
            get { return this._tName; }
            set
            {
                string
                    tnm = string.Empty,
                    onm = string.Empty,
                    dnm = string.Empty;

                if (value.Contains("."))
                {
                    string[] tblNmPrts = value.Split(new char[] { '.' }, StringSplitOptions.None);
                    switch (tblNmPrts.Length)
                    {
                        case 3:
                            dnm = tblNmPrts[0];
                            onm = tblNmPrts[1];
                            tnm = tblNmPrts[2];
                            break;
                        case 2:
                            onm = tblNmPrts[0];
                            tnm = tblNmPrts[1];
                            break;
                        case 1:
                            // Length of array after split *can't* be less than two, since
                            //   we've already confirmed that the table name contains
                            //   a period (.) character.
                            break;
                        default:
                            throw new Exception("Table name was not in an expected format.");
                    }
                }
                else
                    tnm = value;

                this._dbName = (string.IsNullOrEmpty(dnm.Trim(trimChars))) ? "dbo" : dnm.Trim(trimChars);
                this._oName = (string.IsNullOrEmpty(onm.Trim(trimChars))) ? "dbo" : onm.Trim(trimChars);
                this._tName = (string.IsNullOrEmpty(tnm.Trim(trimChars))) ? "dbo" : tnm.Trim(trimChars);
            }
        }
        public string TableOwner
        {
            get { return this._oName; }
            set { this._oName = value; }
        }
        public string DatabaseName
        {
            get { return this._dbName; }
            set { this._dbName = value; }
        }
        public ColumnParamsCollection Columns
        { get { return this._cols; } }
        public ColumnParams this[int index]
        { get { return this._cols[index]; } }
        public ColumnParams this[string columnName]
        { get { return this._cols[columnName]; } }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        private SqlTableParams()
        {
            this._cols = new ColumnParamsCollection(this);
            this._dbName = "master";
        }
        public SqlTableParams(string TableName)
            : this(TableName, "dbo")
        { }
        public SqlTableParams(string TableName, string Owner)
            : this()
        {
            this._tName = TableName;
            this._oName = Owner;
        }
        public SqlTableParams(DataTable template)
            : this(template, false)
        { }
        public SqlTableParams(DataTable template, bool autoDetermineFieldSz)
            : this(template.TableName)
        {
            foreach (DataColumn dc in template.Columns)
            {
                ColumnParams col = new ColumnParams(dc.ColumnName, dc.DataType, dc.MaxLength, dc.Ordinal);
                col.DefaultValue = dc.DefaultValue;
                col.IsNullable = dc.AllowDBNull;
                col.IsIdentity = dc.AutoIncrement;
                col.IdentitySeed = (int)dc.AutoIncrementSeed;
                col.IdentityIncrement = (int)dc.AutoIncrementStep;
                if ((col.DataType == SqlDbType.VarChar || col.DataType == SqlDbType.NVarChar) && dc.MaxLength < 0)
                {
                    int maxSize = 0;
                    if (autoDetermineFieldSz)
                    {
                        for (int i = 0; i < template.Rows.Count; i++)
                            maxSize = (int)System.Math.Max(maxSize, template.Rows[i][dc.Ordinal].ToString().Length);
                    }
                    if (maxSize > 8000)
                        maxSize = -2;
                    if (maxSize == 0)
                        maxSize = 50;
                    col.FieldSize = maxSize;
                }
                this.Columns.Add(col);
            }
        }
        #endregion
    }
}
