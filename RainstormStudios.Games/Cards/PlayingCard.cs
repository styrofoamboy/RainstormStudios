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
    public class PlayingCard : CardBase
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        CardSuit
            _suit;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public CardSuit Suit
        { get { return this._suit; } }
        //***************************************************************************
        // Private Properties
        // 
        protected override int MinCardVal
        { get { return 1; } }
        protected override int MaxCardVal
        { get { return 13; } }
        public override string Name
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                if (this._suit > 0)
                {
                    switch (this.Value)
                    {
                        case 13:
                            sb.Append("King");
                            break;
                        case 12:
                            sb.Append("Queen");
                            break;
                        case 11:
                            sb.Append("Jack");
                            break;
                        case 10:
                            sb.Append("Ten");
                            break;
                        case 9:
                            sb.Append("Nine");
                            break;
                        case 8:
                            sb.Append("Eight");
                            break;
                        case 7:
                            sb.Append("Seven");
                            break;
                        case 6:
                            sb.Append("Six");
                            break;
                        case 5:
                            sb.Append("Five");
                            break;
                        case 4:
                            sb.Append("Four");
                            break;
                        case 3:
                            sb.Append("Three");
                            break;
                        case 2:
                            sb.Append("Two");
                            break;
                        case 1:
                            sb.Append("Ace");
                            break;
                    }
                    sb.AppendFormat(" of {0}s", this.Suit);
                }
                else { sb.Append("Joker"); }
                return sb.ToString();
            }
        }
        protected override string ResourceImageName
        { get { return string.Format("playingCard_{0}_{1}", this.Suit.ToString().ToLower(), this.Value); } }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        internal PlayingCard(int value, CardSuit suit)
            : base(value)
        {
            this._suit = suit;
        }
        #endregion
    }
}
