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
    public class BattleGame : GameEngineBase<PlayingCardDeck, PlayingCard>
    {
        #region Nested Types
        //***************************************************************************
        // Nested Classes
        // 
        public struct RoundResult
        {
            public static readonly RoundResult
                Empty;
            public readonly bool
                Player1Wins,
                Player1OutOfCards;
            public readonly bool
                Player2Wins,
                Player2OutOfCards;
            public readonly bool
                IsBattle;
            public readonly int
                CardCount;

            public RoundResult(bool p1win, bool p1OoCrds, bool p2win, bool p2OoCrds, bool battle, int cardCnt)
            {
                this.Player1Wins = p1win;
                this.Player1OutOfCards = p1OoCrds;
                this.Player2Wins = p2win;
                this.Player2OutOfCards = p2OoCrds;
                this.IsBattle = battle;
                this.CardCount = cardCnt;
            }
        }
        #endregion

        #region Declarations
        //***************************************************************************
        // Constants
        // 
        const string 
            player1DiscardPileName = "p1Discard";
        const string 
            player2DiscardPileName = "p2Discard";
        //***************************************************************************
        // Private Fields
        // 
        int
            _moves;
        bool
            _isBattle;
        RoundResult
            _prevResult;
        //***************************************************************************
        // Public Events
        // 
        public event EventHandler
            Player1Wins;
        public event EventHandler
            Player2Wins;
        public event EventHandler
            GameOver;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public override bool IsGameOver
        { get { return !(this.Players[0].Hand.Count > 0 && this.Players[1].Hand.Count > 0); } }
        public RoundResult PreviousRoundResult
        { get { return this._prevResult; } }
        public int DiscardPileCount
        { get { return this.Piles[player1DiscardPileName].Count + this.Piles[player1DiscardPileName].Count; } }
        public PlayingCard Player1VisibleCard
        { get { return this.Piles[player1DiscardPileName][this._isBattle ? 1 : 0]; } }
        public PlayingCard Player2VisibleCard
        { get { return this.Piles[player2DiscardPileName][this._isBattle ? 1 : 0]; } }
        public int TurnCount
        { get { return this._moves; } }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public BattleGame()
            : base(2)
        { }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public override void NewGame()
        {
            base.NewGame();
            this._moves = 0;
            this._isBattle = false;
        }
        public override void BeginTurn()
        {
            this.Players[0].Hand.MoveTopCard(this.Piles[player1DiscardPileName]);
            this.Players[1].Hand.MoveTopCard(this.Piles[player2DiscardPileName]);
        }
        public override void EndTurn()
        {
            bool p1Win = false;
            bool p2Win = false;
            int roundCardCnt = this.DiscardPileCount;

            PlayingCard
                p1Card = this.Piles[player1DiscardPileName].TopCard,
                p2Card = this.Piles[player2DiscardPileName].TopCard;

            if (p1Card.Value > 10 && p1Card.Value > p2Card.Value)
            {
                // Player1 has a higher face card, and gets all the cards.
                this.Players[0].Hand.AddPiles(this._piles);
                this._isBattle = false;
                p1Win = true;
            }
            else if (p2Card.Value > 10 && p2Card.Value > p1Card.Value)
            {
                // Player2 has a higher face card, and gets all the cards.
                this.Players[1].Hand.AddPiles(this._piles);
                this._isBattle = false;
                p2Win = true;
            }
            else if (p1Card.Value > 10 && p2Card.Value > 10 && p1Card.Value == p2Card.Value)
            {
                // BATTLE!

                // Make sure neither player runs out of cards during the battle.
                if (this.IsGameOver)
                {
                    this._isBattle = false;
                    this.OnGameOver(EventArgs.Empty);
                }
                else
                {
                    // Each player places 1 card from their hand face down.
                    this.Players[0].Hand.MoveTopCard(this.Piles[player1DiscardPileName]);
                    this.Players[1].Hand.MoveTopCard(this.Piles[player2DiscardPileName]);

                    roundCardCnt = this.DiscardPileCount;
                    this._isBattle = true;
                }
            }
            else if (this._isBattle)
            {
                // If we're in a BATTLE round, then it's just high-card, not face cards only.
                // NOTE: a subsequent 'tie' would be handled by the else if above this one,
                //   (and would start another battle)so we don't need to worry about the
                //   two card values being the same here.
                if (p1Card.Value > p2Card.Value)
                {
                    this.Players[0].Hand.AddPiles(this._piles);
                    p1Win=true;
                }
                else
                {
                    this.Players[1].Hand.AddPiles(this._piles);
                    p2Win = true;
                }

                this._isBattle = false;
            }
            else
            {
                // Number cards, or tied in battle. Just continue.
            }
            this._moves++;

            this._prevResult = new RoundResult(p1Win, this.Players[0].Hand.Count < 1, p2Win, this.Players[1].Hand.Count < 1, this._isBattle, roundCardCnt);

            if (this.IsGameOver)
                this.OnGameOver(EventArgs.Empty);
        }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        protected override void InitPiles()
        {
            base.InitPiles(false);
            this._piles.Add(new CardPile<PlayingCard>(), player1DiscardPileName);
            this._piles.Add(new CardPile<PlayingCard>(), player2DiscardPileName);
        }
        //***************************************************************************
        // Event Triggers
        // 
        protected virtual void OnPlayer1Wins(EventArgs e)
        {
            if (this.Player1Wins != null)
                this.Player1Wins.Invoke(this, e);
        }
        protected virtual void OnPlayer2Wins(EventArgs e)
        {
            if (this.Player2Wins != null)
                this.Player2Wins.Invoke(this, e);
        }
        protected virtual void OnGameOver(EventArgs e)
        {
            if (this.GameOver != null)
                this.GameOver.Invoke(this, e);
        }
        #endregion
    }
}
