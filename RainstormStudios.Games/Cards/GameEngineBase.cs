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
    public abstract class GameEngineBase<TDeck, TCard> : IGameEngine<TDeck, TCard>
        where TDeck : IDeck<TCard>, new()
        where TCard : ICard
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        protected TDeck[]
            _deck;
        Player<TDeck, TCard>[]
            _players;
        protected int
            _curPlayer;
        protected CardPileCollection<TCard>
            _piles;
        string[]
            _playerNames;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public virtual TDeck Deck
        {
            get
            {
                if (this._deck != null)
                    return this._deck[0];
                else
                    throw new Exception("You must initialize the deck by calling 'NewGame()' before accessing the deck(s).");
            }
        }
        public virtual CardPile<TCard> DiscardPile
        { get { return (this._piles != null ? this._piles["DiscardPile"] : null); } }
        public CardPileCollection<TCard> Piles
        { get { return this._piles; } }
        public virtual Player<TDeck, TCard> CurrentPlayer
        { get { return (this._players != null ? this._players[this._curPlayer] : null); } }
        public Player<TDeck, TCard>[] Players
        { get { return this._players; } }
        public string[] PlayerNames
        {
            get { return this._playerNames; }
            set { this._playerNames = value; }
        }
        //***************************************************************************
        // Private Properties
        // 
        protected virtual int NumOfDecks
        { get { return 1; } }
        protected virtual int HandSize
        {
            get { return 0; }   // NOTE: '0' means deal the whole deck.
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        protected GameEngineBase(int numOfPlayers)
        {
            this._players = new Player<TDeck, TCard>[numOfPlayers];
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        public virtual void NewGame()
        {
            this._curPlayer = 0;
            this.InitDecks();
            this.InitPiles();
            this.DealHands();
        }
        //***************************************************************************
        // Abstract Methods
        // 
        public abstract void BeginTurn();
        public abstract void EndTurn();
        #endregion

        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        private void InitDecks()
        {
            if (this._deck == null)
                this._deck = new TDeck[this.NumOfDecks];

            for (int i = 0; i < this._deck.Length; i++)
                this._deck[i] = this.CreateDeck(i);
        }
        protected virtual TDeck CreateDeck(int deckNum)
        {
            var newDeck = new TDeck();
            newDeck.Shuffle();
            return newDeck;
        }
        protected virtual void InitPiles()
        {
            if (this._piles == null)
                this._piles = new CardPileCollection<TCard>();
            else
                this._piles.Clear();

            this._piles.Add(new CardPile<TCard>(), "DiscardPile");
        }
        protected virtual void DealHands()
        {
            var hands = this.Deck.Deal(this._players.Length, this.HandSize);
            for (int i = 0; i < hands.Length; i++)
                this.InitPlayer(i, (this._playerNames.Length > i ? this._playerNames[i] : string.Empty), hands[i]);
        }
        protected virtual void InitPlayer(int idx, string name, CardPile<TCard> hand)
        {
            this._players[idx] = new Player<TDeck, TCard>(name, hand, this);
        }
        #endregion
    }
}
