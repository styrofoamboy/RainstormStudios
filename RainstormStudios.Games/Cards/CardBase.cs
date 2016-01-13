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

namespace RainstormStudios.Games.Cards
{
    public abstract class CardBase : ICard
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        int
            _val;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public int Value
        { get { return this._val; } }
        //***************************************************************************
        // Abstract Properties
        // 
        public abstract string Name { get; }
        protected abstract int MaxCardVal { get; }
        protected abstract int MinCardVal { get; }
        protected abstract string ResourceImageName { get; }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        protected CardBase(int value)
        {
            if (value < MinCardVal || value > MaxCardVal)
                throw new ArgumentOutOfRangeException("value");
            this._val = value;
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public virtual System.Drawing.Image GetImage()
        {
            return (System.Drawing.Image)global::RainstormStudios.Games.Properties.Resources.ResourceManager.GetObject(this.ResourceImageName);
        }
        #endregion
    }
}
