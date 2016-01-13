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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RainstormStudios.Web.UI.WebControls.DynamicMenu
{
    public class DynamicMenuItem
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        string
            _text,
            _imgUrl,
            _navUrl,
            _navTgt,
            _cmdName;
        object
            _key,
            _parentKey,
            _menuKey;
        DynamicMenuItemCollection
            _menuItems,
            _owner;
        bool
            _active,
            _collapsed;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public object MenuItemProviderKey
        {
            get { return this._key; }
            set { this._key = value; }
        }
        public object ParentMenuItemProviderKey
        {
            get { return this._parentKey; }
            set { this._parentKey = value; }
        }
        public object OwnerMenuProviderKey
        {
            get { return this._menuKey; }
            set { this._menuKey = value; }
        }
        public string Text
        {
            get { return this._text; }
            set { this._text = value; }
        }
        public string ImageUrl
        {
            get { return this._imgUrl; }
            set { this._imgUrl = value; }
        }
        public string NavigationUrl
        {
            get { return this._navUrl; }
            set { this._navUrl = value; }
        }
        public string Target
        {
            get { return this._navTgt; }
            set { this._navTgt = value; }
        }
        public string CommandName
        {
            get { return this._cmdName; }
            set { this._cmdName = value; }
        }
        public bool Activated
        {
            get { return this._active; }
            set { this._active = value; }
        }
        public bool Collapsed
        {
            get { return this._collapsed; }
            set { this._collapsed = value; }
        }
        public DynamicMenuItemCollection MenuItems
        {
            get { return this._menuItems; }
        }
        public DynamicMenuItemCollection Parent
        {
            get { return this._owner; }
            internal set { this._owner = value; }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public DynamicMenuItem()
        {
            this._menuItems = new DynamicMenuItemCollection(this);
        }
        #endregion
    }
    public class DynamicMenuItemCollection : Collections.ObjectCollectionBase<DynamicMenuItem>
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        DynamicMenuItem
            _owner = null;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public DynamicMenuItem Parent
        {
            get { return this._owner; }
            internal set { this._owner = value; }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public DynamicMenuItemCollection()
            : base()
        { }
        public DynamicMenuItemCollection(DynamicMenuItem parent)
            : this()
        {
            this._owner = parent;
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public string Add(DynamicMenuItem item)
        {
            string key = (item.MenuItemProviderKey != null ? item.MenuItemProviderKey.ToString() : string.Empty);
            if (this._owner != null)
            {
                item.ParentMenuItemProviderKey = this._owner.MenuItemProviderKey;
                item.OwnerMenuProviderKey = this._owner.OwnerMenuProviderKey;
            }
            item.Parent = this;
            return base.Add(item, key);
        }
        public void Remove(DynamicMenuItem item)
        {
            base.RemoveAt(base.IndexOf(item));
            item.Parent = null;
        }
        public override string[] AddRange(DynamicMenuItem[] values)
        {
            List<string> keys = new List<string>();
            for (int i = 0; i < values.Length; i++)
                keys.Add(this.Add(values[i]));
            return keys.ToArray();
        }
        #endregion
    }
}
