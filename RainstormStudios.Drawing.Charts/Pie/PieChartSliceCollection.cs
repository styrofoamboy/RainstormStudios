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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Text;
using RainstormStudios.Collections;

namespace RainstormStudios.Drawing.Charts.Pie
{
    [Author("Unfried, Michael")]
    public class PieChartSliceCollection : ObjectCollectionBase<PieChartSlice>, ICollection<PieChartSlice>
    {
        #region Global Objects
        //***************************************************************************
        // Private Fields
        // 
        private PieChart
            _owner;
        #endregion

        #region Public Properties
        //***************************************************************************
        // Public Properties
        // 
        public PieChart Owner
        { get { return this._owner; } }
        public int Sum
        {
            get
            {
                int retVal = 0;
                foreach (PieChartSlice pcs in this.List)
                    retVal += pcs.Value;
                return retVal;
            }
        }
        public bool IsReadOnly
        { get { return false; } }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        internal PieChartSliceCollection(PieChart owner)
            : this()
        { this._owner = owner; }
        public PieChartSliceCollection()
            : base()
        { }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public void Add(PieChartSlice value)
        {
            value._owner = this._owner;
            value._position = this.List.Count;
            base.Add(value, "");
        }
        public void Add(PieChartSlice value, string key)
        {
            value._owner = this._owner;
            value._position = this.List.Count;
            base.Add(value, key);
        }
        public string Insert(int index, PieChartSlice value)
        {
            value._owner = this._owner;
            value._position = index;
            if (index < (this.List.Count - 1))
                for (int i = index; i < this.List.Count; i++)
                {
                    PieChartSlice oldPcs = this[i];
                    oldPcs._position++;
                }
            return base.Insert(index, value, "");
        }
        public void Insert(int index, PieChartSlice value, string key)
        {
            value._owner = this._owner;
            value._position = index;
            if (index < (this.List.Count - 1))
                for (int i = index; i < this.List.Count; i++)
                {
                    PieChartSlice oldPcs = this[i];
                    oldPcs._position++;
                }
            base.Insert(index, value, key);
        }
        public void CopyTo(PieChartSlice[] arr, int i)
        {
            this.InnerList.CopyTo(arr, i);
        }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        protected override void OnInsert(int index, object value)
        {
            if (value.GetType().Name != "PieChartSlice")
                throw new ArgumentException("Cannot add this object type to collection.", "value");
            else if (this._owner != null && (this.Sum + ((PieChartSlice)value).Value) > this._owner.MaxValue)
                throw new ArgumentException("Total size of all slices cannot exceed total value of pie chart.");

            base.OnInsert(index, value);
        }
        protected override void OnInsertComplete(int index, object value)
        {
            //PieChartSlice pcs = (PieChartSlice)this.List[index];
            //pcs._owner = this._owner;
            //pcs._position = index;
            //if (index < (this.List.Count - 1))
            //    for (int i = index; i < this.List.Count; i++)
            //    {
            //        PieChartSlice oldPcs = this[i];
            //        oldPcs._position++;
            //    }
            base.OnInsertComplete(index, value);
        }
        internal void SetOwner(PieChart value)
        {
            this._owner = value;
            for (int i = 0; i < this.List.Count; i++)
            {
                PieChartSlice oldPcs = this[i];
                oldPcs._owner = this._owner;
            }
        }
        #endregion
    }
}
