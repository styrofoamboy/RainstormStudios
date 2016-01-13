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
    public abstract class DeckBase<TCard> : IDeck<TCard>, IDisposable
        where TCard : ICard
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private ArrayList
            _arrCards;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public TCard this[int index]
        { get { return (TCard)this._arrCards[index]; } }
        public int CardCount
        { get { return this._arrCards.Count; } }
        //***************************************************************************
        // Abstract Properties
        // 
        protected abstract int DefaultCardCount { get; }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        protected DeckBase(bool delayInit = false)
        {
            if (!delayInit)
                this.InitDeck();
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        public void Shuffle()
        {
            Random rnd = new Random(DateTime.Now.Millisecond);
            ArrayList tmp1 = new ArrayList(this._arrCards);
            this._arrCards.Clear();
            while (tmp1.Count > 0)
            {
                int idx = rnd.Next(tmp1.Count);
                this._arrCards.Add(tmp1[idx]);
                tmp1.RemoveAt(idx);
            }
            tmp1.Clear();
        }
        public CardPile<TCard>[] Deal(int playerCount)
        { return this.Deal(playerCount, 0); }
        public CardPile<TCard>[] Deal(int playerCount, int cardsPerPile)
        {
            CardPile<TCard>[] piles = new CardPile<TCard>[playerCount];
            for (int i = 0; i < piles.Length; i++)
                piles[i] = new CardPile<TCard>();

            this.Deal(ref piles, cardsPerPile);
            return piles;
        }
        public void Deal(ref CardPile<TCard>[] piles, int cardsPerPile)
        {
            if (cardsPerPile == 0)
            {
                // We're dealing out all the cards.
                int i = -1;
                while (this._arrCards.Count > 0)
                {
                    i = i + 1;
                    if (i > piles.Length - 1)
                        i = 0;
                    this.PlayTopCard(piles[i]);
                }
            }
            else
            {
                // We're dealing a particular numer of cards per pile.
                if (piles.Length * cardsPerPile > this._arrCards.Count)
                    throw new Exception("Not enough cards in the deck.");

                int i = -1;
                for (int t = 0; t < piles.Length * cardsPerPile; t++)
                {
                    i = i + 1;
                    if (i > piles.Length - 1)
                        i = 0;
                    this.PlayTopCard(piles[i]);
                }
            }
        }
        public void PlayTopCard(CardPile<TCard> pile)
        {
            pile.AddCard((TCard)this._arrCards[0]);
            this._arrCards.RemoveAt(0);
        }
        public void MoveCardsToPile(CardPile<TCard> pile)
        {
            pile.AddCards((TCard[])this._arrCards.ToArray(typeof(TCard)));
            this._arrCards.Clear();
        }
        public void ReturnCards(params CardPile<TCard>[] piles)
        {
            for (int i = 0; i < piles.Length; i++)
                piles[i].ReturnToDeck(this);
        }
        public void ReturnCards(TCard[] cards)
        { this._arrCards.AddRange(cards); }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        protected void InitDeck()
        {
            if (this._arrCards == null)
                this._arrCards = new ArrayList(this.DefaultCardCount);
            else
                this._arrCards.Clear();

            this.CreateDeck();
        }
        protected void AddCard(TCard card)
        {
            this._arrCards.Add(card);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this._arrCards.Clear();
                this._arrCards = null;
            }
        }
        //***************************************************************************
        // Abstract Methods
        // 
        protected abstract void CreateDeck();
        #endregion
    }
}
