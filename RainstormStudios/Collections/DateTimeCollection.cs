using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RainstormStudios.Collections
{
    /// <summary>
    /// Provides a strongly-typed collection object for stored DateTime objects in key/value pairs.
    /// </summary>
    [Author("Unfried, Michael")]
    public class DateTimeCollection : ObjectCollectionBase<DateTime>
    {
        #region Nested Classes
        //***************************************************************************
        // Custom Compare Engines
        // 
        private class DateComparer : System.Collections.IComparer
        {
            #region Declarations
            //***********************************************************************
            // Private Fields
            // 
            SortDirection
                _sortDir;
            #endregion

            #region Class Constructors
            //***********************************************************************
            // Class Constructors
            // 
            public DateComparer()
            {
                this._sortDir = SortDirection.Ascending;
            }
            public DateComparer(SortDirection sortDirection)
            {
                this._sortDir = sortDirection;
            }
            #endregion

            #region Public Methods
            //***********************************************************************
            // Public Methods
            // 
            public int Compare(Object x, Object y)
            {
                if (x == y)
                    return 0;
                if (x == null)
                    return -1;
                if (y == null)
                    return 1;

                //DateTime t1 = DateTime.Parse((string)x);
                //DateTime t2 = DateTime.Parse((string)y);
                DateTime t1 = (DateTime)x;
                DateTime t2 = (DateTime)y;
                if (_sortDir == SortDirection.Descending)
                    return DateTime.Compare(t2, t1);
                else
                    return DateTime.Compare(t1, t2);
            }
            #endregion
        }
        #endregion

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
        public DateTimeCollection()
            : base()
        { }
        public DateTimeCollection(int capacity)
            : base(capacity)
        { }
        public DateTimeCollection(DateTime[] values)
            : this(values, new string[0])
        { }
        public DateTimeCollection(DateTime[] values, string[] keys)
            : base(values, keys)
        { }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public string Add(DateTime value)
        { return base.Add(value, ""); }
        public void Add(DateTime value, string key)
        { base.Add(value, key); }
        public string Insert(int index, DateTime value)
        { return base.Insert(index, value, ""); }
        public void Insert(int index, DateTime value, string key)
        { base.Insert(index, value, key); }
        public void Sort()
        {
            this.Sort(DateTimeCollection.DefaultSortDirection);
        }
        public new void Sort(SortDirection dir)
        {
            DateComparer dtComp = new DateComparer(dir);
            base.Sort(SortDirection.Ascending, dtComp);
        }
        public DateTimeCollection SortClone()
        {
            return this.SortClone(Int32Collection.DefaultSortDirection);
        }
        public DateTimeCollection SortClone(SortDirection dir)
        {
            DateTime[] vals = this.ToArray();
            string[] keys = (string[])this._keys.ToArray(typeof(string));
            DateComparer dtComp = new DateComparer(dir);
            Array.Sort(vals, keys, dtComp);

            return new DateTimeCollection(vals, keys);
        }
        #endregion
    }
}
