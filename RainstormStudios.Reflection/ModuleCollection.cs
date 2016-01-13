using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace RainstormStudios.Reflection
{
    /// <summary>
    /// Provides a strongly typed collection of <see cref="T:System.Reflection.Module"/> key/value pairs.
    /// </summary>
    public class ModuleCollection:RainstormStudios.Collections.ObjectCollectionBase<Module>
    {
        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public ModuleCollection()
            : base()
        { }
        public ModuleCollection(int capacity)
            : base(capacity)
        { }
        public ModuleCollection(Module[] mods, string[] keys)
            : base(mods, keys)
        { }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public string Add(Module value)
        {
            return this.Add(value, string.Empty);
        }
        public string Add(Module value, string key)
        {
            return base.Add(value, key);
        }
        public string Insert(int index, Module value)
        {
            return this.Insert(index, value, string.Empty);
        }
        public string Insert(int index, Module value, string key)
        {
            return base.Insert(index, value, key);
        }
        #endregion
    }
}
