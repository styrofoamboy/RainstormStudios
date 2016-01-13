using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RainstormStudios.Collections
{
    /// <summary>
    /// Provides a strongly-type collection object for storing Int32 values in key/value pairs.
    /// </summary>
    [Author("Unfried, Michael")]
    public class Int32Collection : ObjectCollectionBase<Int32>
    {
        #region Declarations
        //***************************************************************************
        // Public Fields
        //
        public static SortDirection
            DefaultSortDirection = SortDirection.Ascending;
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public Int32Collection()
            : base()
        { }
        public Int32Collection(int capacity)
            : base(capacity)
        { }
        public Int32Collection(int[] values, string[] keys)
            : this()
        {
            for (int i = 0; i < values.Length; i++)
                if (keys != null && keys.Length > i && !string.IsNullOrEmpty(keys[i]))
                    this.Add(values[i], keys[i]);
                else
                    this.Add(values[i]);
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public string Add(int value)
        { return base.Add(value, ""); }
        public void Add(int value, string key)
        { base.Add(value, key); }
        public string Insert(int index, int value)
        { return base.Insert(index, value, ""); }
        public void Insert(int index, int value, string key)
        { base.Insert(index, value, key); }
        public void Sort()
        {
            this.Sort(Int32Collection.DefaultSortDirection);
        }
        public void Sort(SortDirection dir)
        {
            int[] vals = this.ToArray();
            string[] keys = (string[])this._keys.ToArray(typeof(string));
            Array.Sort<string, int>(keys, vals);

            this.Clear();
            for (int i = 0; i < keys.Length; i++)
                this.Add(vals[i], keys[i]);
        }
        public Int32Collection SortClone()
        {
            return this.SortClone(Int32Collection.DefaultSortDirection);
        }
        public Int32Collection SortClone(SortDirection dir)
        {
            int[] vals = this.ToArray();
            string[] keys = (string[])this._keys.ToArray(typeof(string));
            Array.Sort<string, int>(keys, vals);

            return new Int32Collection(vals, keys);
        }
        #endregion
    }
}
