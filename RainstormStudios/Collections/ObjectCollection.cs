using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RainstormStudios.Collections
{
    /// <summary>
    /// Provides a collection object for storing generic key/value pairs.
    /// </summary>
    public class ObjectCollection : ObjectCollectionBase<Object>
    {
        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public ObjectCollection()
        { }
        public ObjectCollection(object[] values)
            : this(values, new string[0])
        { }
        public ObjectCollection(object[] values, string[] keys)
            : base(values, keys)
        { }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public string Add(object value)
        { return base.Add(value, ""); }
        public new void Add(object value, string key)
        { base.Add(value, key); }
        public string Insert(int index, object value)
        { return base.Insert(index, value, ""); }
        public new void Insert(int index, object value, string key)
        { base.Insert(index, value, key); }
        /// <summary>
        /// Sort the contents of this collection based on the collection item's 'ToString()' return values.
        /// </summary>
        public virtual void Sort()
        {
            this.Sort(string.Empty, ObjectCollectionBase<Object>.DefaultSortDirection);
        }
        /// <summary>
        /// Sort the contents of this collection based on the collection item's 'ToString()' return values using the specified sort order.
        /// </summary>
        /// <param name="dir">A value from the SortDirection enumeration defining the order to sort into.</param>
        public virtual void Sort(SortDirection dir)
        {
            this.Sort(string.Empty, dir);
        }
        /// <summary>
        /// Sort the contents of this collection based on a public property exposed by the collection item, using the specified sort order.
        /// </summary>
        /// <param name="valueSortProperty">The name of the public property whose 'ToString()' value will be used for the sort.</param>
        /// <param name="dir">A value from the SortDirection enumeration defining the order to sort into.</param>
        public new void Sort(string valueSortProperty, SortDirection dir)
        {
            base.Sort(valueSortProperty, dir);
        }
        public new string[] AddRange(Array values)
        {
            return this.AddRange(values, new string[0]);
        }
        public new string[] AddRange(Array values, string[] keys)
        {
            // We don't want the caller to have to pass only arrays of type "object[]", but we also have to keep the generic base class
            //   happy.  Using the "new" keyword instead of "override" (because the method signatures don't match), and then using Linq
            //   to cast each array element to a type of "object" will accomplish this transparently.
            return base.AddRange(values.Cast<object>().ToArray(), keys);
        }
        #endregion
    }
}
