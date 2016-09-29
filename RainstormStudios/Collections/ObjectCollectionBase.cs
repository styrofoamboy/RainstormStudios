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
using System.Collections;
using System.Text;

namespace RainstormStudios.Collections
{
    /// <summary>
    /// A common event handler delegate for all RainstormStudios collections events.
    /// </summary>
    /// <param name="sender">The instance that triggered the event.</param>
    /// <param name="e">An object of type <see cref="T:RainstormStudios.Collections.CollectionEventArgs"/> containing information about the collection's event.</param>
    public delegate void CollectionEventHandler(object sender, CollectionEventArgs e);
    /// <summary>
    /// Contains event information for all RainstormStudios collections object's events.
    /// </summary>
    public class CollectionEventArgs : EventArgs
    {
        #region Declarations
        //***************************************************************************
        // Public Fields
        //
        public readonly int
            Index;
        public readonly object
            Value;
        public readonly CollectionAction
            Action;
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public CollectionEventArgs(int index, object value, CollectionAction action)
        {
            this.Index = index;
            this.Value = value;
            this.Action = action;
        }
        #endregion
    }
    /// <summary>
    /// Event handler delegate for RainstormStudios collections object sort operations' progress.
    /// </summary>
    /// <param name="sender">The instance that triggered the event.</param>
    /// <param name="e">An object of type <see cref="T:RainstormStudios.Collections.SortProgressEventArgs"/> containing information about the progress of a sort operation.</param>
    public delegate void SortProgressEventHandler(object sender, SortProgressEventArgs e);
    /// <summary>
    /// Contains information about the progress of a sort operation.
    /// </summary>
    public class SortProgressEventArgs : EventArgs
    {
        #region Declarations
        //***************************************************************************
        // Public Fields
        // 
        internal int
            _pcntComp;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public int PercentComplete
        { get { return this._pcntComp; } }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public SortProgressEventArgs(int percentComp)
            : base()
        {
            this._pcntComp = percentComp;
        }
        #endregion
    }
    /// <summary>
    /// Provides a custom enumerator for the generic <see cref="T:RainstormStudios.Collections.ObjectCollectionBase"/> class.
    /// This class is sealed, and has no public constructor.
    /// </summary>
    /// <typeparam name="T">A generic type descriptor that defines the type of data that the enumerator will iterate through.</typeparam>
    public sealed class ObjectCollectionEnumerator<T> : System.Collections.Generic.IEnumerator<T>
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        ObjectCollectionBase<T>
            _collection;
        int
            _curIdx;
        T
            _curVal;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public T Current
        { get { return this._curVal; } }
        object IEnumerator.Current
        { get { return this.Current; } }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        internal ObjectCollectionEnumerator(ObjectCollectionBase<T> collection)
        {
            this._collection = collection;
            this._curIdx = -1;
            this._curVal = default(T);
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public bool MoveNext()
        {
            if (++this._curIdx >= this._collection.Count)
                return false;

            else
                this._curVal = this._collection[this._curIdx];

            return true;
        }
        public void Reset()
        { this._curIdx = -1; }
        void IDisposable.Dispose()
        { }
        #endregion
    }
    /// <summary>
    /// Provides a base object for strongly-type collections of key/value pairs.  This class is abstract.
    /// </summary>
    public abstract class ObjectCollectionBase<T> : CollectionBase, System.Collections.ICollection, System.Collections.Generic.IEnumerable<T>
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        /// <summary>
        /// Stores the keys used to access values in this collection.
        /// </summary>
        protected ArrayList
            _keys;
        /// <summary>
        /// Used for thread concurrency locks.
        /// </summary>
        private object
            _lockMe;
        //***************************************************************************
        // Public Fields
        // 
        /// <summary>
        /// Determine the minimum number of digits to used to automatically generating keys.
        /// </summary>
        public int
            AutoKeyLength = 10;
        public bool
            ReturnNullForKeyNotFound = false,
            ReturnNullForIndexNotFound = false;
        public static SortDirection
            DefaultSortDirection = SortDirection.Ascending;
        //***************************************************************************
        // Public Events
        // 
        /// <summary>
        /// Occurs whenever an item is added to the collection.
        /// </summary>
        public event CollectionEventHandler Inserted;
        /// <summary>
        /// Occurs whenever an item is removed from the collection.
        /// </summary>
        public event CollectionEventHandler Removed;
        /// <summary>
        /// Occurs whenever the collection is cleared.
        /// </summary>
        public event EventHandler Cleared;
        /// <summary>
        /// Occurs whenever an item within the collection is updated.
        /// </summary>
        public event CollectionEventHandler Updated;
        /// <summary>
        /// Occurs regularly durring a sort operation.
        /// </summary>
        public event SortProgressEventHandler Sorting;
        /// <summary>
        /// Occurs when a call to 'BeginSort' is complete.
        /// </summary>
        public event EventHandler SortComplete;
        //***************************************************************************
        // Thread Delegates
        // 
        private delegate
            void BeginSortByValueDelegate(string valueSortProperty, SortDirection dir);
        private delegate
            void BeginSortByComparerDelegate(SortDirection dir, IComparer comparer);
        #endregion

        #region Public Properties
        //***************************************************************************
        // Public Properties
        // 
        /// <summary>
        /// Returns the item in the collection at the specified ordinal position.
        /// </summary>
        /// <param name="index">A <see cref="System.Int32"/> value indicating the ordinal position of the value to return.</param>
        /// <returns>An object of type <see cref="System.Object"/> which was stored at the specified ordinal value.</returns>
        public virtual T this[int index]
        {
            get
            {
                if (index >= this.InnerList.Count)
                {
                    if (this.ReturnNullForIndexNotFound)
                        return default(T);
                    else
                        throw new ArgumentOutOfRangeException("index", "Index must be a value within the bounds of the collection.");
                }
                else
                    return (T)List[index];
            }
            set { List[index] = value; this.OnUpdated(new CollectionEventArgs(index, List[index], CollectionAction.Update)); }
        }
        /// <summary>
        /// Returns the item in the collection with the specified key value attached.
        /// </summary>
        /// <param name="key">A <see cref="System.String"/> value of the key by which to located the desired collection value.</param>
        /// <returns>An object of type <see cref="System.Object"/> which was stored at the specified ordinal value.</returns>
        public virtual T this[string key]
        {
            get
            {
                if (!this._keys.Contains(key))
                {
                    if (this.ReturnNullForKeyNotFound)
                        return default(T);
                    else
                        throw new ArgumentOutOfRangeException("key", "Specified key was not found in the collection.");
                }
                else
                    return (T)List[this.IndexOfKey(key)];
            }
            set
            {
                int idx = this._keys.IndexOf(key);
                List[idx] = value;
                this.OnUpdated(new CollectionEventArgs(idx, List[idx], CollectionAction.Update));
            }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public ObjectCollectionBase()
            : base()
        { this._keys = new ArrayList(); }
        public ObjectCollectionBase(int capacity)
            : base(capacity)
        { this._keys = new ArrayList(capacity); }
        protected ObjectCollectionBase(T[] values, string[] keys)
            : this()
        {
            this.AddRange(values, keys);
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        /// <summary>
        /// Returns a key value's ordinal position within the collection.
        /// </summary>
        /// <param name="key">A string value containing the key to search for.</param>
        /// <returns>An integer value indicating the key's ordinal position. (or -1, if the key does not exist within the collection)</returns>
        public int IndexOfKey(string key)
        {
            if (!this._keys.Contains(key))
                //throw new ArgumentOutOfRangeException("key", "Specified key was not found in the collection.");
                return -1;
            return this._keys.IndexOf(key);
        }
        /// <summary>
        /// Determines if a specified key exists within the collection.
        /// </summary>
        /// <param name="key">A string value containing the key to search for.</param>
        /// <returns>A boolean value indicating true if the specified key was found within the collection. Otherwise, false.</returns>
        public bool ContainsKey(string key)
        {
            return this._keys.Contains(key);
        }
        /// <summary>
        /// Retrieves the key name contained at a specific ordinal position within the collection.
        /// </summary>
        /// <param name="index">The ordinal position of the key to be retrieved.</param>
        /// <returns>A string value containing the name of the key at the specified ordinal position.</returns>
        public string GetKey(int index)
        {
            if (index < 0 || index > this._keys.Count - 1)
                throw new ArgumentOutOfRangeException("index", "Specified index is outside the bounds of the collection.");
            return (string)this._keys[index];
        }
        /// <summary>
        /// Sets the key name at a specific ordinal position.
        /// </summary>
        /// <param name="oldKey">The name of the key to be changed.</param>
        /// <param name="newKey">The new name to be assigned to the key.</param>
        /// <returns>An integer value indicating the ordinal position of the key which was changed.</returns>
        public int SetKey(string oldKey, string newKey)
        {
            if (!this._keys.Contains(oldKey))
                throw new ArgumentOutOfRangeException("oldKey", "Specified key was not found in the collection.");
            int keyIdx = this.IndexOfKey(oldKey);
            this.SetKey(keyIdx, newKey);
            return keyIdx;
        }
        /// <summary>
        /// Sets the key name at a specific ordinal position.
        /// </summary>
        /// <param name="index">The ordinal position of the key to be changed.</param>
        /// <param name="newKey">The new name to be assigned to the key.</param>
        public void SetKey(int index, string newKey)
        {
            if (index < 0 || index > this._keys.Count - 1)
                throw new ArgumentOutOfRangeException("index", "Specified index it outside the bounds of the collection.");
            this._keys[index] = newKey;
        }
        public new System.Collections.Generic.IEnumerator<T> GetEnumerator()
        {
            return new ObjectCollectionEnumerator<T>(this);
        }
        //***************************************************************************
        // Virtual Methods
        // 
        /// <summary>
        /// Adds an array of objects to the collection after the last element's ordinal position.
        /// </summary>
        /// <param name="values">The array of values to be added to the collection.</param>
        /// <returns>An array of System.String values containing the generated collection keys.</returns>
        public virtual string[] AddRange(T[] values)
        {
            return this.AddRange(values, new string[0]);
        }
        /// <summary>
        /// Adds an array of objects to the collection, after the last element's ordinal position, using the specified key values.
        /// </summary>
        /// <param name="values">The array of values to be added to the collection.</param>
        /// <param name="keys">An array of type System.String whose values will be used as keys for the added values. If the array of keys contains fewer elements than the array of values, or any key values are empty strings, generic keys will be generated for the values with no keys.</param>
        /// <returns>The keys assigned to the new values.</returns>
        public virtual string[] AddRange(T[] values, string[] keys)
        {
            string[] retVal = new string[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                string thisKey = (keys != null && keys.Length > i && !string.IsNullOrEmpty(keys[i])) ? keys[i] : "";
                retVal[i] = this.Add(values.GetValue(i), thisKey);
            }
            return retVal;
        }
        /// <summary>
        /// Removes the specified number of elements beginning at the specified ordinal position.
        /// </summary>
        /// <param name="index">The ordinal position from which to start removing items from the collection.</param>
        /// <param name="count">The total number of items to remove from the collection.</param>
        public virtual void RemoveRange(int index, int count)
        {
            for (int i = index; i < (index + count); i++)
            {
                this.RemoveAt(i);
                this._keys.RemoveAt(i);
            }
        }
        public virtual void RemoveByKey(string key)
        {
            if (!this.ContainsKey(key))
                throw new ArgumentOutOfRangeException("Specified key was not found in the collection.", "key");
            int idx = this.IndexOfKey(key);
            this.RemoveAt(idx);
            // Key removal is handled by the 'OnRemoveComplete' event handler.
            //this._keys.RemoveAt(idx);
        }
        public virtual bool Remove(T value)
        {
            if (!this.Contains(value))
                //throw new ArgumentOutOfRangeException("Specified element was not found in the collection", "value");
                return false;

            int idx = this.IndexOf(value);
            this.RemoveAt(idx);
            // Key removal is handled by the 'OnRemoveComplete' event handler.
            //this._keys.RemoveAt(idx);
            return true;
        }
        public virtual int IndexOf(T value)
        {
            return this.InnerList.IndexOf(value);
        }
        public virtual bool Contains(T value)
        {
            return this.InnerList.Contains(value);
        }
        public virtual T[] ToArray()
        {
            return this.ToArray(0, this.List.Count);
        }
        public virtual T[] ToArray(int offset, int length)
        {
            T[] retVal = new T[length];
            for (int i = 0; i < length; i++)
                retVal[i] = (T)this[i + offset];
            return retVal;
        }
        #endregion

        #region Protected Methods
        //***************************************************************************
        // Protected Methods
        // 
        /// <summary>Provides a means of inserting values into the collection.</summary>
        /// <param name="value">The value to be inserted.</param>
        /// <param name="key">The unique name of the key assigned to the value, or an empty string to have a unique key name generated.</param>
        /// <returns>A string value containing the name of the inserted value's key.</returns>
        protected virtual string Add(object value, string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                if (!this.ValidKey(key))
                    throw new ArgumentException("Specified key already exists in collection.", key);
            }
            else
                key = this.GetAutoKey(value);
            this._keys.Add(key);
            this.List.Add(value);
            return key;
        }
        /// <summary>Inserts a value into the collection at a specific ordinal position.</summary>
        /// <param name="index">The ordinal position at which to insert the new value.</param>
        /// <param name="value">The value to insert into the collection.</param>
        /// <param name="key">The name of the new value's key.</param>
        /// <returns>The name of the new value's key.</returns>
        protected virtual string Insert(int index, object value, string key)
        {
            if (index < 0 || index > this.List.Count)
                throw new ArgumentOutOfRangeException("index", "Specified ordinal position is outside the bounds of the array.");

            if (!string.IsNullOrEmpty(key))
            {
                if (!this.ValidKey(key))
                    throw new ArgumentException("Specified key already exists in collection.", key);
            }
            else
                key = this.GetAutoKey(value);

            this._keys.Insert(index, key);
            this.List.Insert(index, value);
            return key;
        }
        protected virtual string GetAutoKey(object value)
        {
            int num = this.List.Count;
            while (this._keys.IndexOf(((value != null) ? value.ToString() : "Item") + num.ToString().PadLeft(AutoKeyLength, '0')) > -1)
                num++;
            return ((value != null) ? value.ToString() : "Item") + num.ToString().PadLeft(AutoKeyLength, '0');
        }
        /// <summary>
        /// Determines if the specified key exists in the collection.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected virtual bool ValidKey(string key)
        {
            if (this._keys.IndexOf(key) > -1)
                return false;
            else
                return true;
        }
        /// <summary>
        /// Sort the contents of this collection based on a public property exposed by the collection item, using the specified sort order.
        /// </summary>
        /// <param name="valueSortProperty">The name of the public property whose 'ToString()' value will be used for the sort.</param>
        /// <param name="dir">A value from the SortDirection enumeration defining the order to sort into.</param>
        protected virtual void Sort(string valueSortProperty, SortDirection dir)
        {
            lock (_lockMe)
            {
                SortProgressEventArgs progArgs = null;
                if (this.Sorting != null)
                    progArgs = new SortProgressEventArgs(0);

                if (this.InnerList.Count < 1)
                    // If there aren't any items in the collection, then there's nothing
                    //   to do here.
                    return;

                // Setup the binding flags for our Reflection search.
                System.Reflection.BindingFlags flags = System.Reflection.BindingFlags.Public
                                                        | System.Reflection.BindingFlags.GetProperty
                                                        | System.Reflection.BindingFlags.SetProperty
                                                        | System.Reflection.BindingFlags.Instance
                                                        | System.Reflection.BindingFlags.Static;

                // We need two collections: one to hold the string values we're sorting
                //   on, and the other to hold the value keys.
                string[] valNms = new string[this.Count];
                string[] valKys = (string[])this._keys.ToArray(typeof(System.String[]));
                for (int i = 0; i < this.Count; i++)
                {
                    object itemVal = null;
                    if (!string.IsNullOrEmpty(valueSortProperty))
                    {
                        // The the caller specified a 'valueSortProperty', use Reflection
                        //   to try and find the property accessor on the current object.
                        Type t = this[i].GetType();
                        System.Reflection.PropertyInfo pnfo = t.GetProperty(valueSortProperty, flags);
                        if (pnfo != null)
                            // If we found it, get it's value for the sort.
                            itemVal = pnfo.GetValue(this[i], null);
                    }
                    // If we still haven't populated the 'itemVal' object with anything
                    //   yet, just get the current item's ToString() value.
                    if (itemVal == null)
                        itemVal = this[i].ToString();

                    // Add the found string to the array.
                    valNms[i] = itemVal.ToString();

                    if (progArgs != null && i % 10 == 0)
                    {
                        progArgs._pcntComp = (((i / 4) / 100) * this.Count);
                        this.OnSorting(progArgs);
                    }
                }
                // The 'Array' class will do the basic string sorting for us.
                Array.Sort<string, string>(valKys, valNms);
                if (progArgs != null)
                {
                    progArgs._pcntComp = ((50 / 100) * this.Count);
                    this.OnSorting(progArgs);
                }

                // Array.Sort always sorts ascending, so if the specified to have the
                //   items sorting in descending order, we need to reverse the contents
                //   of the valKys array.  NOTE: We're not going to use the valNms
                //   array again, so there's no reason to waste processor time
                //   reversing it also.
                if (dir == SortDirection.Descending)
                    Array.Reverse(valKys);

                // Now we've got two string arrays containing sorted values/keys, we
                //   need to offload our entire collection to a temp space to hold
                //   the "real" objects, since our value array only holds simple
                //   string values.
                ObjectCollection objCol = new ObjectCollection();
                for (int i = 0; i < this.Count; i++)
                    objCol.Add(this[i], this.GetKey(i));
                if (progArgs != null)
                {
                    progArgs._pcntComp = ((75 / 100) * this.Count);
                    this.OnSorting(progArgs);
                }

                // Now, we're going to clear the current collection.
                this.Clear();

                // ...And then rebuild it from our backup source.
                // NOTE: The key values array got reorganized along with the string
                //   values when we made the call to Array.Sort, so we really don't
                //   care about the string values anymore, only the physical object
                //   collection where we backed up the contents of this collection
                //   before we cleared it.
                for (int i = 0; i < valKys.Length; i++)
                    this.Add(objCol[valKys[i]], valKys[i]);
                if (progArgs != null)
                {
                    progArgs._pcntComp = ((100 / 100) * this.Count);
                    this.OnSorting(progArgs);
                }
            }
        }
        protected virtual void Sort(SortDirection dir)
        {
            this.Sort(dir, Comparer.Default);
        }
        /// <summary>
        /// Sort the contents of this collection using a custom IComparer object.
        /// </summary>
        /// <param name="dir">A value from the SortDirection enumeration defining the order to sort into.</param>
        /// <param name="comparer">An instance of an object conforming to the IComparable interface that will be used to compare the objects.</param>
        protected virtual void Sort(SortDirection dir, IComparer comparer)
        {
            lock (_lockMe)
            {
                SortProgressEventArgs progArgs = null;
                if (this.Sorting != null)
                    progArgs = new SortProgressEventArgs(0);

                T[] vals = new T[this.InnerList.Count];
                for (int i = 0; i < this.InnerList.Count; i++)
                    vals[i] = (T)this.InnerList[i];
                if (progArgs != null)
                {
                    progArgs._pcntComp = ((33 / 100) * this.Count);
                    this.OnSorting(progArgs);
                }

                string[] keys = (string[])this._keys.ToArray(typeof(string));
                //Array.Sort<string, int>(keys, vals);

                Array.Sort(vals, keys, comparer);
                if (progArgs != null)
                {
                    progArgs._pcntComp = ((66 / 100) * this.Count);
                    this.OnSorting(progArgs);
                }

                if (dir == SortDirection.Descending)
                {
                    Array.Reverse(vals);
                    Array.Reverse(keys);
                }

                // This method will just "replace" all the existing data.
                for (int i = 0; i < keys.Length; i++)
                {
                    this._keys[i] = keys[i];
                    this.InnerList[i] = vals[i];
                }
                if (progArgs != null)
                {
                    progArgs._pcntComp = ((66 / 100) * this.Count);
                    this.OnSorting(progArgs);
                }
            }
        }
        protected void BeginSort(string valueSortProperty, SortDirection dir)
        {
            BeginSortByValueDelegate del = new BeginSortByValueDelegate(this.Sort);
            del.BeginInvoke(valueSortProperty, dir, new AsyncCallback(this.BeginSortCallback), del);
        }
        protected void BeginSort(SortDirection dir)
        {
            this.BeginSort(dir, Comparer.Default);
        }
        protected void BeginSort(SortDirection dir, IComparer comparer)
        {
            BeginSortByComparerDelegate del = new BeginSortByComparerDelegate(this.Sort);
            del.BeginInvoke(dir, comparer, new AsyncCallback(this.BeginSortCallback), del);
        }
        //***************************************************************************
        // Override Methods
        // 
        protected override void OnInsertComplete(int index, object value)
        {
            // If an item is added to the list, and the key collection
            //   is out-of-sync, add an auto-key at the new item's index.
            base.OnInsertComplete(index, value);
            if (this._keys.Count < this.List.Count)
                this._keys.Insert(index, this.GetAutoKey(value));
            this.OnInserted(new CollectionEventArgs(index, value, CollectionAction.Insert));
        }
        protected override void OnRemoveComplete(int index, object value)
        {
            // If a value is removed from the list, delete the corresponding key.
            base.OnRemoveComplete(index, value);
            this._keys.RemoveAt(index);
            this.OnRemoved(new CollectionEventArgs(index, value, CollectionAction.Remove));
        }
        protected override void OnClearComplete()
        {
            // If the list is cleared, clear the keys also.
            base.OnClearComplete();
            this._keys.Clear();
            this.OnCleared(EventArgs.Empty);
        }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Thread Callbacks
        // 
        private void BeginSortCallback(IAsyncResult state)
        {
            BeginSortByValueDelegate del1 = (state.AsyncState as BeginSortByValueDelegate);
            BeginSortByComparerDelegate del2 = (state.AsyncState as BeginSortByComparerDelegate);

            if (del1 != null)
                del1.EndInvoke(state);

            else if (del2 != null)
                del2.EndInvoke(state);

            this.OnSortComplete(EventArgs.Empty);
        }
        #endregion

        #region Event Triggers
        //***************************************************************************
        // Event Triggers
        //
        protected virtual void OnInserted(CollectionEventArgs e)
        {
            if (this.Inserted != null)
                this.Inserted.Invoke(this, e);
        }
        protected virtual void OnUpdated(CollectionEventArgs e)
        {
            if (this.Updated != null)
                this.Updated.Invoke(this, e);
        }
        protected virtual void OnRemoved(CollectionEventArgs e)
        {
            if (this.Removed != null)
                this.Removed.Invoke(this, e);
        }
        protected virtual void OnCleared(EventArgs e)
        {
            if (this.Cleared != null)
                this.Cleared.Invoke(this, e);
        }
        protected virtual void OnSorting(SortProgressEventArgs e)
        {
            if (this.Sorting != null)
                this.Sorting.Invoke(this, e);
        }
        protected virtual void OnSortComplete(EventArgs e)
        {
            if (this.SortComplete != null)
                this.SortComplete.Invoke(this, e);
        }
        #endregion
    }
}
