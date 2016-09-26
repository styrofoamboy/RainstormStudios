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
    public class CardPile<TCard> : System.Collections.Generic.IEnumerable<TCard>
        where TCard : ICard
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        ArrayList
            _cards;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public TCard this[int index]
        { get { return (TCard)this._cards[index]; } }
        public int Count
        { get { return this._cards.Count; } }
        public TCard TopCard
        { get { return this[0]; } }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public CardPile()
        {
            this._cards = new ArrayList();
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public void AddPile(CardPile<TCard> pile)
        {
            this._cards.AddRange(pile._cards);
            pile._cards.Clear();
        }
        public void AddPiles(System.Collections.Generic.IEnumerable<CardPile<TCard>> piles)
        {
            foreach (CardPile<TCard> p in piles)
                this.AddPile(p);
        }
        public void MoveTopCard(CardPile<TCard> pile)
        { this.MoveCard(0, pile); }
        public void MoveCard(int index, CardPile<TCard> pile)
        {
            pile.AddCardOnTop(this[index]);
            this._cards.RemoveAt(index);
        }
        public void ReturnToDeck(IDeck<TCard> deck)
        {
            deck.ReturnCards((TCard[])this._cards.ToArray(typeof(TCard)));
            this._cards.Clear();
        }
        public System.Collections.Generic.IEnumerator<TCard> GetEnumerator()
        { return new CardPileEnumerator<TCard>(this); }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        internal void AddCard(TCard val)
        { this._cards.Add(val); }
        internal void AddCardOnTop(TCard val)
        { this._cards.Insert(0, val); }
        internal void AddCards(TCard[] val)
        { this._cards.AddRange(val); }
        private IEnumerator GetEnumerator1()
        { return this.GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator()
        { return GetEnumerator1(); }
        #endregion
    }
    public class CardPileCollection<TCard> : RainstormStudios.Collections.ObjectCollectionBase<CardPile<TCard>>
        where TCard : ICard
    {
        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public void Add(CardPile<TCard> value)
        {
            base.Add(value, null);
        }
        public void Add(CardPile<TCard> value, string name)
        {
            base.Add(value, name);
        }
        public void AssignName(int idx, string name)
        {
            this._keys[idx] = name;
        }
        public void AssignName(CardPile<TCard> value, string name)
        {
            if (!this.Contains(value))
                throw new ArgumentException("Specified CardPile was not found in collection.");

            this.AssignName(this.IndexOf(value), name);
        }
        #endregion
    }
}
