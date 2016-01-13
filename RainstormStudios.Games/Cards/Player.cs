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
    public class Player<TDeck, TCard> : IDisposable
        where TDeck : IDeck<TCard>
        where TCard : ICard
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private string
            _nm;
        private CardPile<TCard>
            _hand;
        IGameEngine<TDeck, TCard>
            _game;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public string Name
        { get { return this._nm; } }
        public CardPile<TCard> Hand
        { get { return this._hand; } }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public Player(string name, IGameEngine<TDeck, TCard> game)
            : this(name, new CardPile<TCard>(), game)
        { }
        public Player(string name, CardPile<TCard> hand, IGameEngine<TDeck, TCard> game)
        {
            this._nm = name;
            this._hand = hand;
            this._game = game;
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                this._hand.ReturnToDeck(this._game.Deck);
        }
        #endregion
    }
}
