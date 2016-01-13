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
using RainstormStudios.Collections;

namespace RainstormStudios.Data
{
    [Author("Unfried, Michael")]
    public struct FieldMapping : ICloneable
    {
        #region Declarations
        //***************************************************************************
        // Public Fields
        // 
        public static readonly FieldMapping
            Empty;
        public string
            SourceName, DestinationName;
        public int
            SourceOrdinal, DestinationOrdinal;
        public bool
            Mandatory, ForcedAlpha;
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public FieldMapping(string srcName, int srcOrd, string dstName, int dstOrd, bool mand, bool alpha)
        {
            this.SourceName = srcName;
            this.SourceOrdinal = srcOrd;
            this.DestinationName = dstName;
            this.DestinationOrdinal = dstOrd;
            this.Mandatory = mand;
            this.ForcedAlpha = alpha;
        }
        public FieldMapping(string srcName, int srcOrd, string dstName, int dstOrd)
            : this(srcName, srcOrd, dstName, dstOrd, false, false)
        { }
        public FieldMapping(string srcName, string dstName)
            : this(srcName, 0, dstName, 0)
        { }
        public FieldMapping(string srcName, int dstOrd)
            : this(srcName, 0, "", dstOrd)
        { }
        public FieldMapping(int srcOrd, string dstName)
            : this("", srcOrd, dstName, 0)
        { }
        public FieldMapping(int srcOrd, int dstOrd)
            : this("", srcOrd, "", dstOrd)
        { }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public object Clone()
        {
            return this.MemberwiseClone();
        }
        //public void SetSource(string name, int ord)
        //{
        //    this.srcName = name;
        //    this.srcOrd = ord;
        //}
        //public void SetDestination(string name, int ord)
        //{
        //    this.dstName = name;
        //    this.dstOrd = ord;
        //}
        #endregion
    }
    [Author("Unfried, Michael")]
    class FieldMappingCollection : ObjectCollectionBase<FieldMapping>
    {
        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public FieldMappingCollection()
            : base()
        { }
        public FieldMappingCollection(FieldMapping[] values)
            : this()
        {
            foreach (FieldMapping fm in values)
            {
                string fmKey = this.GetFmKey(fm);
                base.Add(fm, fmKey);
            }
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public string Add(FieldMapping value)
        {
            string fmKey = this.GetFmKey(value);
            if (this.ContainsKey(fmKey))
                fmKey = "";
            return base.Add(value, fmKey);
        }
        public void Add(FieldMapping value, string key)
        { base.Add(value, key); }
        public string Insert(int index, FieldMapping value)
        { return base.Insert(index, value, ""); }
        public void Insert(int index, FieldMapping value, string key)
        { base.Insert(index, value, key); }
        public new FieldMapping[] ToArray()
        { return this.ToArray(0, this.List.Count); }
        public new FieldMapping[] ToArray(int offset, int length)
        { return base.ToArray(offset, length); }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        private string GetFmKey(FieldMapping fm)
        {
            string fmKey = string.Empty;
            if (!string.IsNullOrEmpty(fm.SourceName))
                fmKey = fm.SourceName;
            //else if (!string.IsNullOrEmpty(fm.DestinationName))
            //    fmKey = fm.DestinationName;
            else
                fmKey = "Column" + fm.SourceOrdinal.ToString().PadLeft(3, '0');
            return fmKey;
        }
        #endregion
    }
    [Author("Unfried, Michael")]
    public class DbMappingRules : ICloneable
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private int
            _curRow = -1;
        private FieldMappingCollection
            _mapRules;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public FieldMapping this[int index]
        {
            get { return this._mapRules[index]; }
            set { this._mapRules[index] = value; }
        }
        public string[] SourceFields
        {
            get
            {
                string[] flds = new string[this._mapRules.Count];
                for (int i = 0; i < this._mapRules.Count; i++)
                    if (!string.IsNullOrEmpty(this._mapRules[i].SourceName))
                        flds[i] = this._mapRules[i].SourceName;
                    else
                        flds[i] = this._mapRules[i].SourceOrdinal.ToString();
                return flds;
            }
        }
        public string[] DestinationFields
        {
            get
            {
                string[] flds = new string[this._mapRules.Count];
                for (int i = 0; i < this._mapRules.Count; i++)
                    if (!string.IsNullOrEmpty(this._mapRules[i].DestinationName))
                        flds[i] = this._mapRules[i].DestinationName;
                    else
                        flds[i] = this._mapRules[i].DestinationOrdinal.ToString();
                return flds;
            }
        }
        public FieldMapping CurrentRule
        { get { return (FieldMapping)this._mapRules[this._curRow].Clone(); } }
        public int CurrentRuleOrdinal
        { get { return this._curRow; } }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public DbMappingRules()
        { }
        /// <summary>
        /// Creates a new instance of the DbMappingRules object.
        /// </summary>
        /// <param name="ruleString">A string value containing the column names, seperated by coma's, in the order in which they should appear in the destination table.</param>
        public DbMappingRules(string ruleString)
            : this(ruleString.Split(','))
        { }
        /// <summary>
        /// Creates a new instance of the DbMappingRules object.
        /// </summary>
        /// <param name="ruleString">A string array containing the column names in the order in which they should appear in the destination table.</param>
        public DbMappingRules(string[] ruleString)
            : this()
        {
            for (int i = 0; i < ruleString.Length; i++)
                this._mapRules.Add(new FieldMapping(i, ruleString[i]));
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public object Clone()
        { return this.MemberwiseClone(); }
        public void Add(FieldMapping rule)
        { this._mapRules.Add(rule); }
        public void Remove(FieldMapping rule)
        { this._mapRules.RemoveAt(this._mapRules.IndexOf(rule)); }
        public bool Contains(FieldMapping rule)
        { return this._mapRules.Contains(rule); }
        public FieldMapping Read()
        {
            if (this._curRow >= this._mapRules.Count)
                this._curRow = -1;
            return this._mapRules[this._curRow++];
        }
        public FieldMapping FindSource(string fieldName)
        {
            for (int i = 0; i < this._mapRules.Count; i++)
                if (this._mapRules[i].SourceName == fieldName)
                    return this._mapRules[i];
            return FieldMapping.Empty;
        }
        public FieldMapping finddestination(string fieldName)
        {
            for (int i = 0; i < this._mapRules.Count; i++)
                if (this._mapRules[i].DestinationName == fieldName)
                    return this._mapRules[i];
            return FieldMapping.Empty;
        }
        #endregion
    }
}
