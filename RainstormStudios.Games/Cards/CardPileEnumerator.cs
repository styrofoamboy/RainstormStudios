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
using System.Linq;
using System.Text;

namespace RainstormStudios.Games.Cards
{
    public sealed class CardPileEnumerator<TCard> : System.Collections.Generic.IEnumerator<TCard>
        where TCard : ICard
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        CardPile<TCard>
            _collection;
        int
            _curIdx;
        TCard
            _curVal;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public TCard Current
        { get { return this._curVal; } }
        //***************************************************************************
        // Private Properties
        // 
        private object Current1
        { get { return this.Current; } }
        object IEnumerator.Current
        { get { return this.Current1; } }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        internal CardPileEnumerator(CardPile<TCard> collection)
        {
            this._collection = collection;
            this._curIdx = -1;
            this._curVal = default(TCard);
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
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                this._collection = null;
                this._curVal = default(TCard);
            }
        }
        #endregion
    }
}
