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
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using D3D = Microsoft.DirectX.Direct3D;
using Microsoft.DirectX.DirectSound;
using DS = Microsoft.DirectX.DirectSound;
using RainstormStudios;
using RainstormStudios.Collections;

namespace RainstormStudios.DirectX
{
    public class VertexBufferCollection : ObjectCollectionBase<VertexBuffer>
    {
        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public VertexBufferCollection()
            : base()
        { }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public string Add(VertexBuffer value)
        {
            string key = this.GetAutoKey(value);
            this.Add(value, key);
            return key;
        }
        public void Add(VertexBuffer value, string key)
        { base.Add(value, key); }
        public string Insert(int index, VertexBuffer value)
        {
            string key = this.GetAutoKey(value);
            this.Insert(index, value, key);
            return key;
        }
        public void Insert(int index, VertexBuffer value, string key)
        { base.Insert(index, value, key); }
        public void Remove(VertexBuffer value)
        { base.RemoveAt(this.IndexOf(value)); }
        public void Remove(string key)
        { base.RemoveAt(this.IndexOfKey(key)); }
        #endregion
    }
    public class TextureCollection : ObjectCollectionBase<Texture>
    {
        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public TextureCollection()
            : base()
        { }
        public TextureCollection(Texture[] vals)
            : this(vals, new string[0])
        { }
        public TextureCollection(Texture[] vals, string[] keys)
            : base(vals, keys)
        { }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public string Add(Texture value)
        {
            string key = this.GetAutoKey(value);
            this.Add(value, key);
            return key;
        }
        public void Add(Texture value, string key)
        { base.Add(value, key); }
        public string Insert(int index, Texture value)
        {
            string key = this.GetAutoKey(value);
            this.Insert(index, value, key);
            return key;
        }
        public void Insert(int index, Texture value, string key)
        { base.Insert(index, value, key); }
        public void Remove(Texture value)
        { base.RemoveAt(this.IndexOf(value)); }
        public void Remove(string key)
        { base.RemoveAt(this.IndexOfKey(key)); }
        #endregion
    }
    public class MaterialCollection : ObjectCollectionBase<Material>
    {
        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public MaterialCollection()
            : base()
        { }
        public MaterialCollection(Material[] values)
            : this(values, new string[0])
        { }
        public MaterialCollection(Material[] values, string[] keys)
            : base(values, keys)
        { }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public string Add(Material value)
        {
            string key = this.GetAutoKey(value);
            this.Add(value, key);
            return key;
        }
        public void Add(Material value, string key)
        { base.Add(value, key); }
        public string Insert(int index, Material value)
        {
            string key = this.GetAutoKey(value);
            this.Insert(index, value, key);
            return key;
        }
        public void Insert(int index, Material value, string key)
        { base.Insert(index, value, key); }
        public void Remove(Material value)
        { base.RemoveAt(this.IndexOf(value)); }
        public void Remove(string key)
        { base.RemoveAt(this.IndexOfKey(key)); }
        #endregion
    }
    public class Font3DCollection : ObjectCollectionBase<D3D.Font>
    {
        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public Font3DCollection()
            : base()
        { }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public string Add(D3D.Font value)
        {
            string key = this.GetAutoKey(value);
            this.Add(value, key);
            return key;
        }
        public void Add(D3D.Font value, string key)
        { base.Add(value, key); }
        public string Insert(int index, D3D.Font value)
        {
            string key = this.GetAutoKey(value);
            this.Insert(index, value, key);
            return key;
        }
        public void Insert(int index, D3D.Font value, string key)
        { base.Insert(index, value, key); }
        public void Remove(D3D.Font value)
        { base.RemoveAt(this.IndexOf(value)); }
        public void Remove(string key)
        { base.RemoveAt(this.IndexOfKey(key)); }
        #endregion
    }
    public class EffectCollection : ObjectCollectionBase<Effect>
    {
        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public EffectCollection()
            : base()
        { }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public string Add(Effect value)
        {
            string key = this.GetAutoKey(value);
            this.Add(value, key);
            return key;
        }
        public void Add(Effect value, string key)
        { base.Add(value, key); }
        public string Insert(int index, Effect value)
        {
            string key = this.GetAutoKey(value);
            this.Insert(index, value, key);
            return key;
        }
        public void Insert(int index, Effect value, string key)
        { base.Insert(index, value, key); }
        public void Remove(Effect value)
        { base.RemoveAt(this.IndexOf(value)); }
        public void Remove(string key)
        { base.RemoveAt(this.IndexOfKey(key)); }
        #endregion
    }
    public class MeshObjectCollection : ObjectCollectionBase<MeshObject>
    {
        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public MeshObjectCollection()
            : base()
        { }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public string Add(MeshObject value)
        {
            string key = this.GetAutoKey(value);
            this.Add(value, key);
            return key;
        }
        public void Add(MeshObject value, string key)
        { base.Add(value, key); }
        public string Insert(int index, MeshObject value)
        {
            string key = this.GetAutoKey(value);
            this.Insert(index, value, key);
            return key;
        }
        public void Insert(int index, MeshObject value, string key)
        { base.Insert(index, value, key); }
        public void Remove(MeshObject value)
        { base.RemoveAt(this.IndexOf(value)); }
        public void Remove(string key)
        { base.RemoveAt(this.IndexOfKey(key)); }
        #endregion
    }
}
