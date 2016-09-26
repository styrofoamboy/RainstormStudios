using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RainstormStudios.Collections
{
    /// <summary>
    /// Provides a strongly-typed collection object for storing String values in key/value pairs and provides functions for working with those strings.
    /// </summary>
    public class StringCollection : ObjectCollectionBase<String>
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
        public StringCollection()
            : base()
        { }
        public StringCollection(int capacity)
            : base(capacity)
        { }
        public StringCollection(string list)
            : this(list, ',', '|')
        { }
        public StringCollection(string list, string keyList)
            : this(list, keyList, ',', '|')
        { }
        public StringCollection(string list, params char[] seperators)
            : this(list, "", seperators)
        { }
        public StringCollection(string list, string keyList, params char[] seperators)
            : this(list.Split(seperators, StringSplitOptions.RemoveEmptyEntries), keyList.Split(seperators, StringSplitOptions.RemoveEmptyEntries))
        { }
        public StringCollection(string[] values)
            : this(values, new string[0])
        { }
        public StringCollection(string[] values, string[] keys)
            : base(values, keys)
        { }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public string Add(string value)
        { return base.Add(value, ""); }
        public void Add(string value, string key)
        { base.Add(value, key); }
        public string Insert(int index, string value)
        { return base.Insert(index, value, ""); }
        public void Insert(int index, string value, string key)
        { base.Insert(index, value, key); }
        public void Remove(string value)
        {
            this.RemoveAt(this.IndexOf(value));
        }
        /// <summary>
        /// Adds a value to the collection if that value does not already exist.
        /// </summary>
        /// <param name="val">The String value to be entered.</param>
        /// <returns>An Boolean value indicating true of the item was added to the collection. Otherwise, false.</returns>
        public bool AddUnique(string val)
        { return this.AddUnique(val, ""); }
        /// <summary>
        /// Adds a value to the collection if that value does not already exist.
        /// </summary>
        /// <param name="val">The String value to be entered.</param>
        /// <param name="key">A String value indicating the key to be used for this key/value pair.</param>
        /// <returns>An Boolean value indicating true of the item was added to the collection. Otherwise, false.</returns>
        public bool AddUnique(string val, string key)
        {
            if (this.Contains(val))
                return false;
            else
                if (!string.IsNullOrEmpty(key))
                    this.Add(val, key);
                else
                    this.Add(val);
            return true;
        }
        /// <summary>
        /// Adds a value to the collection if that value does not already exist.
        /// </summary>
        /// <param name="val">The String value to be entered.</param>
        /// <returns>An Int32 value containing the index of the item in the collection.</returns>
        public int LoadString(string val)
        { return this.LoadString(val, ""); }
        /// <summary>
        /// Adds a value to the collection if that value does not already exist.
        /// </summary>
        /// <param name="val">The String value to be entered.</param>
        /// <param name="key">A String value indicating the key to be used for this key/value pair.</param>
        /// <returns>An Int32 value containing the index of the item in the collection.</returns>
        public int LoadString(string val, string key)
        {
            this.AddUnique(val, key);
            return base.IndexOf(val);
        }
        /// <summary>
        /// Updates an existing string value, or creates a new entry if no matching value is found.
        /// </summary>
        /// <param name="val">The value to update.</param>
        /// <param name="key">The key reference for the value.</param>
        public void Update(string val, string key)
        {
            if (base.ContainsKey(key))
                this[key] = val;
            else
                base.Add(val, key);
        }
        /// <summary>
        /// Gets all the values of this collection as a single string with each value seperated by commas.
        /// </summary>
        /// <returns></returns>
        public string GetList()
        {
            return this.GetList(',');
        }
        /// <summary>
        /// Gets all the values of this collection as a single string with each value seperated by the specified character.
        /// </summary>
        /// <param name="seperator">The character to insert into the output string between the individual collection values.</param>
        /// <returns></returns>
        public string GetList(char seperator)
        {
            string retVal = "";
            foreach (object str in this.List)
                retVal += seperator + (string)str;
            return (retVal.Length > 1) ? retVal.Substring(1) : "";
        }
        public void Sort()
        {
            this.Sort(StringCollection.DefaultSortDirection);
        }
        public void Sort(SortDirection dir)
        {
            string[] vals = this.ToArray();
            string[] keys = (string[])this._keys.ToArray(typeof(string));
            Array.Sort<string, string>(keys, vals);

            #region DEPRECIATED
            // This will cause events to be fired
            //this.Clear();
            //for (int i = 0; i < keys.Length; i++)
            //    this.Add(vals[i], keys[i]);
            #endregion

            for (int i = 0; i < keys.Length; i++)
            {
                this._keys[i] = keys[i];
                this.InnerList[i] = vals[i];
            }
        }
        public StringCollection SortClone()
        {
            return this.SortClone(StringCollection.DefaultSortDirection);
        }
        public StringCollection SortClone(SortDirection dir)
        {
            string[] vals = this.ToArray();
            string[] keys = (string[])this._keys.ToArray(typeof(string));
            Array.Sort<string, string>(keys, vals);

            return new StringCollection(vals, keys);
        }
        public new string[] ToArray()
        {
            return this.ToArray(0, this.List.Count);
        }
        public new string[] ToArray(int offset, int length)
        {
            //return Array.ConvertAll<object, string>(base.ToArray(offset, length), new Converter<object, string>(Convert.ToString));
            return base.ToArray(offset, length);
        }
        #endregion
    }
}
