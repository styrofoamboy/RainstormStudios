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
    public class PlayingCardDeck : DeckBase<PlayingCard>
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        bool
            _jokers;
        #endregion

        #region Properties
        //***************************************************************************
        // Private Properties
        // 
        protected override int DefaultCardCount
        { get { return (this._jokers ? 54 : 52); } }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public PlayingCardDeck()
            : this(false)
        { }
        public PlayingCardDeck(bool useJokers)
            : base(true)
        {
            this._jokers = useJokers;
            base.InitDeck();
        }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        protected override void CreateDeck()
        {
            for (int s = 0; s < 4; s++)
                for (int i = 0; i < 13; i++)
                    base.AddCard(new PlayingCard(i + 1, (CardSuit)s + 1));
            if (this._jokers)
            {
                base.AddCard(new PlayingCard(1, CardSuit.Joker));
                base.AddCard(new PlayingCard(1, CardSuit.Joker));
            }
        }
        #endregion
    }
}
