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
    public class UnoGame : GameEngineBase<UnoDeck, UnoCard>
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        CardColor
            _playClr,
            _wildSel;
        int
            _playNum;
        bool
            _cardJustPlayed = false;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public CardColor WildSelection
        {
            get { return this._wildSel; }
            set { this._wildSel = value; }
        }
        //***************************************************************************
        // Private Properties
        // 
        protected override int HandSize
        { get { return 7; } }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public UnoGame()
            : base(4)
        { }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public override void BeginTurn()
        {
            UnoCard topCard = this.Piles["DiscardPile"][0];

            if (_cardJustPlayed)
            {
                if (topCard.IsDrawTwo)
                {
                    this.DrawCard(2);
                    this.EndTurn();
                }
                else if (topCard.IsDrawFour)
                {
                    this.DrawCard(4);
                    this.EndTurn();
                }
            }

            this._cardJustPlayed = false;
        }
        public override void EndTurn()
        {
            UnoCard topCard = this.Piles["DiscardPile"][0];
            this._playClr = topCard.Color;
            this._playNum = topCard.Value;
        }
        public void DrawCard()
        { this.DrawCard(1); }
        public void DrawCard(int num)
        {
            for (int i = 0; i < num; i++)
                this.Piles["DrawPile"].MoveTopCard(this.CurrentPlayer.Hand);
        }
        public void PlayCard(int idx)
        {
            if (idx < 0 || idx > this.CurrentPlayer.Hand.Count)
                throw new ArgumentOutOfRangeException("Specified card number is invalid.");

            this.CurrentPlayer.Hand.MoveCard(idx, this.Piles["DiscardPile"]);
            this._cardJustPlayed = true;
        }
        public int[] GetValidCards()
        {
            List<int> validCards = new List<int>();
            for (int i = 0; i < this.CurrentPlayer.Hand.Count; i++)
            {
                var card = this.CurrentPlayer.Hand[i];
                if (card.IsWild || card.Color == this._playClr || (!card.IsWild && card.Value == this._playNum))
                    validCards.Add(i);
            }
            return validCards.ToArray();
        }
        #endregion

        #region Private Methds
        //***************************************************************************
        // Private Methods
        // 
        public override void NewGame()
        {
            base.NewGame();
            this.Deck.MoveCardsToPile(this.Piles["DrawPile"]);
        }
        protected override void InitPiles()
        {
            base.InitPiles();
            this._piles.Add(new CardPile<UnoCard>(), "DrawPile");
        }
        #endregion
    }
}
