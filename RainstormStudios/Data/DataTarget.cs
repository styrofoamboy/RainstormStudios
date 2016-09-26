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
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace RainstormStudios.Data
{
    public abstract class DataTarget
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public abstract DataTargetType TargetType
        { get; }
        #endregion
    }
    public class FlatDataTarget : DataTarget
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private string
            _fileName;
        private bool
            _colHdrs,
            _trimVals;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public override DataTargetType TargetType
        {
            get { return DataTargetType.Text; }
        }
        public string FullFileName
        {
            get { return this._fileName; }
            set { this._fileName = value; }
        }
        public string FileName
        { get { return Path.GetFileName(this.FileName); } }
        public bool ColumnHeaders
        { get { return this._colHdrs; } }
        public bool TrimValues
        { get { return this._trimVals; } }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public FlatDataTarget(string fileName)
            : base()
        {
            this._fileName = fileName;
        }
        public FlatDataTarget(string fileName, bool columnHeaders)
            : this(fileName)
        {
            this._colHdrs = columnHeaders;
        }
        public FlatDataTarget(string fileName, bool columnHeaders, bool trimValues)
            : this(fileName, columnHeaders)
        {
            this._trimVals = trimValues;
        }
        #endregion
    }
    public abstract class TextDataTarget : FlatDataTarget
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        public string
            _delim;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public abstract override DataTargetType TargetType
        { get; }
        public abstract string Delimiter
        { get; }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        protected TextDataTarget(string fileName)
            : base(fileName)
        { }
        protected TextDataTarget(string fileName, bool columnHeaders)
            : base(fileName, columnHeaders)
        { }
        protected TextDataTarget(string fileName, bool columnHeaders, bool trimValues)
            : base(fileName, columnHeaders, trimValues)
        { }
        #endregion
    }
    public class CsvDataTarget : TextDataTarget
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public override DataTargetType TargetType
        { get { return DataTargetType.Csv; } }
        public override string Delimiter
        { get { return ","; } }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public CsvDataTarget(string fileName)
            : base(fileName)
        { }
        public CsvDataTarget(string fileName, bool columnHeaders)
            : base(fileName, columnHeaders)
        { }
        public CsvDataTarget(string fileName, bool columnHeaders, bool trimValues)
            : base(fileName, columnHeaders, trimValues)
        { }
        #endregion
    }
    public class DelimDataTarget : TextDataTarget
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        //
        public override DataTargetType TargetType
        { get { return DataTargetType.Text; } }
        public override string Delimiter
        { get { return this._delim; } }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public DelimDataTarget(string fileName)
            : base(fileName)
        { }
        public DelimDataTarget(string fileName, bool columnHeaders)
            : base(fileName, columnHeaders)
        { }
        public DelimDataTarget(string filename, bool columnHeaders, bool trimValues)
            : base(filename, columnHeaders, trimValues)
        { }
        #endregion
    }
    public class FixedWidthDataTarget : FlatDataTarget
    {
        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public override DataTargetType TargetType
        { get { return DataTargetType.Text; } }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public FixedWidthDataTarget(string fileName)
            : base(fileName)
        { }
        public FixedWidthDataTarget(string fileName, bool columnHeaders)
            : base(fileName, columnHeaders)
        { }
        #endregion
    }
    public class ExcelDataTarget : FlatDataTarget
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public override DataTargetType TargetType
        { get { return DataTargetType.Excel; } }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        //
        public ExcelDataTarget(string fileName)
            : base(fileName)
        { }
        public ExcelDataTarget(string fileName, bool columnHeaders)
            : base(fileName, columnHeaders)
        { }
        public ExcelDataTarget(string fileName, bool columnHeaders, bool trimValues)
            : base(fileName, columnHeaders, trimValues)
        { }
        #endregion
    }
    public class XmlDataTarget : FlatDataTarget
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private string
            _xmlSchemaPath;
        private bool
            _writeXmlSchema;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        //
        public override DataTargetType TargetType
        { get { return DataTargetType.Xml; } }
        public string XmlSchemaPath
        {
            get { return this._xmlSchemaPath; }
            set
            {
                this._xmlSchemaPath = value;
                if (!string.IsNullOrEmpty(this._xmlSchemaPath))
                    this._writeXmlSchema = true;
            }
        }
        public bool WriteXmlSchema
        {
            get { return this._writeXmlSchema; }
            set { this._writeXmlSchema = value; }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public XmlDataTarget(string fileName)
            : base(fileName)
        { }
        public XmlDataTarget(string fileName, bool trimValues)
            : base(fileName, false, trimValues)
        { }
        #endregion
    }
}
