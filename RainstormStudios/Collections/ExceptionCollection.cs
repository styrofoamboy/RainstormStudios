using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RainstormStudios.Collections
{
    /// <summary>
    /// Provides a strongly-typed collection object for storing Exception objects in key/value pairs.
    /// </summary>
    public class ExceptionCollection : ObjectCollectionBase<Exception>
    {
        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public ExceptionCollection()
            : base()
        { }
        public ExceptionCollection(int capacity)
            : base(capacity)
        { }
        public ExceptionCollection(Exception[] values)
            : this(values, new string[0])
        { }
        public ExceptionCollection(Exception[] values, string[] keys)
            : base(values, keys)
        { }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public string Add(Exception value)
        { return base.Add(value, ""); }
        public void Add(Exception value, string key)
        { base.Add(value, key); }
        public string Insert(int index, Exception value)
        { return base.Insert(index, value, ""); }
        public void Insert(int index, Exception value, string key)
        { base.Insert(index, value, key); }
        #endregion
    }
}
