using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using RainstormStudios.Collections;

namespace RainstormStudios.Reflection
{
    public class MemberInfoCollection : ObjectCollectionBase<MemberInfo>
    {
        #region Declarations
        //***************************************************************************
        // Global Variables
        // 
        Type _owner;
        #endregion

        #region Public Properties
        //***************************************************************************
        // Public Properties
        // 
        public Type Parent
        { get { return this._owner; } set { this._owner = value; } }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public MemberInfoCollection()
            : base()
        { }
        public MemberInfoCollection(Type t)
            : this()
        {
            this._owner = t;
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public string Add(MemberInfo value)
        { return base.Add(value, ""); }
        public void Add(MemberInfo value, string key)
        { base.Add(value, key); }
        public string Insert(int index, MemberInfo value)
        { return base.Insert(index, value, ""); }
        public void Insert(int index, MemberInfo value, string key)
        { base.Insert(index, value, key); }
        public string AddUnique(MemberInfo value)
        {
            bool doAdd = true;
            for (int i = 0; i < this.List.Count; i++)
                if (this[i].Name == value.Name)
                { doAdd = false; break; }
            if (doAdd)
                return this.Add(value);
            else
                return string.Empty;
        }
        public void AddUnique(MemberInfo value, string key)
        {
            bool doAdd = true;
            for (int i = 0; i < this.List.Count; i++)
                if (this[i].Name == value.Name)
                { doAdd = false; break; }
            if (doAdd)
                this.Add(value, key);
        }
        public void LoadMembers()
        {
            if (this._owner == null)
                throw new Exception("Cannot load members into collection.  Parent Type not set.");

            this.LoadMembers(this._owner, MemberTypes.All, BindingFlags.DeclaredOnly | BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
        }
        public void LoadMembers(Type t, MemberTypes mType, BindingFlags bFlags)
        {
            this._owner = t;
            foreach (MemberInfo mi in t.GetMembers(bFlags))
                if ((mType & mi.MemberType) == mi.MemberType)
                    this.Add(mi, rsMemberInfo.GetUniqueName(mi));
        }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        #endregion
    }
}
