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
using System.Text;
using RainstormStudios.Collections;

namespace RainstormStudios.Controls
{
    public enum UndoStateType
    {
        Insert,
        Overwrite,
        Delete,
        Backspace,
        Clear,
        Cut,
        Paste,
        Move,
        Drop
    }
    public struct UndoState
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        internal string
            ID;
        //***************************************************************************
        // Public Fields
        // 
        public static readonly UndoState
            Empty;
        public UndoStateType
            StateType;
        public string
            Text,
            PreviousText,
            Name;
        public int
            CharPosition;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public int CharLength
        { get { return this.Text.Length; } }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public UndoState(UndoStateType type, string text, int pos)
            : this(type, type.ToString(), text, string.Empty, pos)
        { }
        public UndoState(UndoStateType type, string text, string prevText, int pos)
            : this(type, type.ToString(), text, prevText, pos)
        { }
        public UndoState(UndoStateType type, string name, string text, string prevText, int pos)
        {
            this.ID = string.Empty;
            this.StateType = type;
            this.Name = name;
            this.PreviousText = prevText;
            this.Text = text;
            this.CharPosition = pos;
        }
        #endregion
    }
    public class UndoStateCollection : ObjectCollectionBase<UndoState>
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        public int
            MaxUndoLevel = 100;
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public string Add(UndoStateType type, string text, int charPos)
        { return this.Add(new UndoState(type, text, charPos)); }
        public string Add(UndoState value)
        {
            if (this.Count + 1 > this.MaxUndoLevel)
                this.RemoveAt(0);

            value.ID = base.Add(value, "");
            return value.ID;
        }
        public UndoState GetLast()
        {
            return this[this.Count - 1];
        }
        public void RemoveLast()
        {
            this.RemoveAt(this.Count - 1);
        }
        public string[] GetNameList()
        {
            string[] strNms = new string[this.Count];
            for (int i = 0; i < strNms.Length; i++)
                strNms[i] = this[i].Name;
            return strNms;
        }
        #endregion
    }
}
