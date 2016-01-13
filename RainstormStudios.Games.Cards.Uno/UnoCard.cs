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
    public class UnoCard : CardBase
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        CardColor
            _clr;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public CardColor Color
        { get { return this._clr; } }
        public bool IsWild
        { get { return this.Color == CardColor.Wild; } }
        public bool IsSkip
        { get { return this.Value == 10; } }
        public bool IsReverse
        { get { return this.Value == 11; } }
        public bool IsDrawTwo
        { get { return this.Value == 12; } }
        public bool IsDrawFour
        { get { return this.IsWild && this.Value == 1; } }
        //***************************************************************************
        // Private Properties
        // 
        protected override int MinCardVal
        { get { return 0; } }
        protected override int MaxCardVal
        { get { return 12; } }
        public override string Name
        {
            get {
                if (this.IsDrawFour)
                    return "Draw Four";
                else if (this.IsWild)
                    return "Wild";
                else if (this.IsSkip)
                    return "Skip";
                else if (this.IsReverse)
                    return "Reverse";
                else if (this.IsDrawTwo)
                    return "Draw Two";
                else
                    return string.Format("{0} {1}", this.Color, this.Value.ToWrittenNumber());
            }
        }
        protected override string ResourceImageName
        { get { return string.Format("unoCard_{0}_{1}", this.Color.ToString().ToLower(), this.Value); } }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        internal UnoCard(int value, CardColor clr)
            : base(value)
        {
            this._clr = clr;
        }
        #endregion
    }
}
