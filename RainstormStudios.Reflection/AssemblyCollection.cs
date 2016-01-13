using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace RainstormStudios.Reflection
{
    public class AssemblyCollection:RainstormStudios.Collections.ObjectCollectionBase<Assembly>
    {
        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public AssemblyCollection()
            : base()
        { }
        public AssemblyCollection(int capacity) :
            this()
        { }
        public AssemblyCollection(Assembly[] asm, string[] keys)
            : base(asm, keys)
        { }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public string Add(Assembly value)
        {
            return base.Add(value, string.Empty);
        }
        public string Add(Assembly value, string key)
        {
            return base.Add(value, key);
        }
        public string Insert(int index, Assembly value)
        {
            return base.Insert(index, value, string.Empty);
        }
        public string Insert(int index, Assembly value, string key)
        {
            return base.Insert(index, value, key);
        }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        #endregion
    }
}
