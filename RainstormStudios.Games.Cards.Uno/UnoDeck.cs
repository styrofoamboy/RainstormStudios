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
    public class UnoDeck : DeckBase<UnoCard>
    {
        #region Properties
        //***************************************************************************
        // Private Properties
        // 
        protected override int DefaultCardCount
        { get { return 108; } }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public UnoDeck()
            : base()
        { }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        protected override void CreateDeck()
        {
            // Each color gets 2 cards 1-9, but only one '0' card of each color.
            CardColor[] colors = new CardColor[] { CardColor.Red, CardColor.Yellow, CardColor.Green, CardColor.Blue };
            foreach (var clr in colors)
                for (int t = 0; t < 2; t++)
                    for (int i = 0; i <= 12; i++)
                        if (t == 0 || i > 0)
                            base.AddCard(new UnoCard(i, clr));

            // Create the "Wild" and "Draw 4" cards (4 each).
            for (int t = 0; t < 2; t++)
                for (int i = 0; i < 4; i++)
                    base.AddCard(new UnoCard(t, CardColor.Wild));
        }
        #endregion
    }
}
