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
using System.Collections;
using System.Text;
using System.Runtime.InteropServices;

namespace RainstormStudios.Unmanaged
{
    public class InteropHtmlHelp
    {
        #region Structs & Enums
        //***************************************************************************
        // Structs & Enums
        // 
        public enum HtmlHelpCommand : uint
        {
            HH_DISPLAY_TOPIC = 0,
            HH_DISPLAY_TOC = 1,
            HH_DISPLAY_INDEX = 2,
            HH_DISPLAY_SEARCH = 3,
            HH_HELP_CONTEXT = 0x000F,
            HH_CLOSE_ALL = 0x0012
        }
        struct HHCTRLOPSTRUCT
        {
            public IntPtr hwnd;
            public string hhFile;
            public UInt32 hhCmd;
            public Int32 hhData;
        }
        #endregion

        #region Declarations
        //***************************************************************************
        // Interop MFC Links
        // 
        [DllImport("hhctrl.ocx", SetLastError = true)]
        static extern IntPtr HtmlHelp([In] HHCTRLOPSTRUCT hhOpStruct);
        //***************************************************************************
        // Private Fields
        // 
        private HHCTRLOPSTRUCT
            _hhOp;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public IntPtr hwnd
        {
            set { this._hhOp.hwnd = value; }
        }
        public string HelpFile
        {
            set { this._hhOp.hhFile = value; }
        }
        public HtmlHelpCommand HelpCommand
        {
            set { this._hhOp.hhCmd = (UInt32)value; }
        }
        public Int32 hhData
        {
            set { this._hhOp.hhData = value; }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public InteropHtmlHelp()
        {
            this._hhOp.hwnd = IntPtr.Zero;
            this._hhOp = new HHCTRLOPSTRUCT();
            this._hhOp.hhCmd = (UInt32)HtmlHelpCommand.HH_DISPLAY_TOC;
            this._hhOp.hhFile = "";
            this._hhOp.hhData = 0;
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public IntPtr ShowHelp()
        {
            return HtmlHelp(this._hhOp);
        }
        #endregion
    }
}
