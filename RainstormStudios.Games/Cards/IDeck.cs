﻿//  Copyright (c) 2008, Michael unfried
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
    public interface IDeck<TCard>
        where TCard: ICard
    {
        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        int CardCount { get; }
        TCard this[int index] { get; }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        void Shuffle();
        CardPile<TCard>[] Deal(int playerCount);
        CardPile<TCard>[] Deal(int playerCount, int cardsPerPile);
        void Deal(ref CardPile<TCard>[] piles, int cardsPerPile);
        void PlayTopCard(CardPile<TCard> pile);
        void MoveCardsToPile(CardPile<TCard> pile);
        void ReturnCards(params CardPile<TCard>[] piles);
        void ReturnCards(TCard[] cards);
        #endregion
    }
}
