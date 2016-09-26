using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RainstormStudios.Collections
{
    /// <summary>
    /// Provides a strongly-typed collection object for storing Byte values in key/value pairs.
    /// </summary>
    public class ByteCollection : ObjectCollectionBase<Byte>
    {
        #region Declarations
        //***************************************************************************
        // Public Fields
        //
        public static SortDirection
            DefaultSortDirection = SortDirection.Ascending;
        #endregion

        #region Public Properties
        //***************************************************************************
        // Public Properties
        // 
        public new byte this[int index]
        {
            get { return (byte)base[index]; }
            set { base[index] = value; }
        }
        public new byte this[string key]
        {
            get { return (byte)base[key]; }
            set { base[key] = value; }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public ByteCollection()
            : base()
        { }
        public ByteCollection(int capacity)
            : base(capacity)
        { }
        public ByteCollection(byte[] values, string[] keys)
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
        public string Add(byte value)
        { return base.Add(value, ""); }
        public void Add(byte value, string key)
        { base.Add(value, key); }
        public void AddRange(params byte[] vals)
        { base.AddRange(vals); }
        public string Insert(int index, byte value)
        { return base.Insert(index, value, ""); }
        public void Insert(int index, byte value, string key)
        { base.Insert(index, value, key); }
        public void Sort()
        {
            this.Sort(ByteCollection.DefaultSortDirection);
        }
        public void Sort(SortDirection dir)
        {
            byte[] vals = this.ToArray();
            string[] keys = (string[])this._keys.ToArray(typeof(string));

            // The default Array.Sort behavior is to sort based on the key name, not
            //  the value, which is why we're passing these two parameters in
            //  "backwards".
            Array.Sort<string, byte>(keys, vals);

            for (int i = 0; i < keys.Length; i++)
            {
                this._keys[i] = keys[i];
                this.InnerList[i] = vals[i];
            }
        }
        public ByteCollection SortClone()
        {
            return this.SortClone(ByteCollection.DefaultSortDirection);
        }
        public ByteCollection SortClone(SortDirection dir)
        {
            byte[] vals = this.ToArray();
            string[] keys = (string[])this._keys.ToArray(typeof(string));

            // The default Array.Sort behavior is to sort based on the key name, not
            //  the value, which is why we're passing these two parameters in
            //  "backwards".
            Array.Sort<string, byte>(keys, vals);

            return new ByteCollection(vals, keys);
        }
        #endregion
    }
}
