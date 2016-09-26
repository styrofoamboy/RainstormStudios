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
    /// <summary>
    /// Provides a managed class for accessing Windows' shell32.dll,ShellExecute unmanaged code.
    /// </summary>
    public class InteropShellExecute
    {
        #region Structs & Enums
        //***************************************************************************
        // Structs & Enums
        // 
        public enum ShowCommands : int
        {
            SW_HIDE = 0,
            SW_SHOWNORMAL = 1,
            SW_NORMAL = 1,
            SW_SHOWMINIMIZED = 2,
            SW_SHOWMAXIMIZED = 3,
            SW_MAXIMIZE = 3,
            SW_SHOWNOACTIVE = 4,
            SW_SHOW = 5,
            SW_MINIMIZE = 6,
            SW_SHOWMINNOACTIVE = 7,
            SW_SHOWNA = 8,
            SW_RESTORE = 9,
            SW_SHOWDEFAULT = 10,
            SW_FORCEMINIMIZE = 11,
            SW_MAX = 11
        }
        //public enum OperationType
        //{
        //    Edit = 0,
        //    Explore,
        //    Find,
        //    Open,
        //    Print,
        //    Null
        //}
        struct SHEXECOPSTRUCT
        {
            public IntPtr hwnd;
            public string lpOperation;
            public string lpFile;
            public string lpParameters;
            public string lpDirectory;
            public ShowCommands lpShowCmd;
        }
        #endregion

        #region Declarations
        //***************************************************************************
        // Interop MFC Links
        // 
        [DllImport("shell32.dll")]
        static extern IntPtr ShellExecute([In] SHEXECOPSTRUCT lpExecOp);
        //***************************************************************************
        // Global Variables
        // 
        private SHEXECOPSTRUCT _ShExec;
        //***************************************************************************
        // Public Fields
        // 
        /// <summary>
        /// The IntPtr value of the window which owns this object.
        /// </summary>
        public IntPtr hwnd
        {
            set { this._ShExec.hwnd = value; }
        }
        /// <summary>
        /// Details what type of operation to perform.  Possible values are 'edit', 'open', 'find', 'explore', and 'print'.
        /// </summary>
        public string lpOperation
        {
            set { this._ShExec.lpOperation = value; }
        }
        /// <summary>
        /// The file to perform the action on when the command is executed.
        /// </summary>
        public string lpFile
        {
            set { this._ShExec.lpFile = value; }
        }
        /// <summary>
        /// Command line parameters to pass to the file.
        /// </summary>
        public string lpParameters
        {
            set { this._ShExec.lpParameters = value; }
        }
        /// <summary>
        /// The directory from which to execute the command.
        /// </summary>
        public string lpDirectory
        {
            set { this._ShExec.lpDirectory = value; }
        }
        /// <summary>
        /// Specifies how the window should appear when the command is executed.
        /// </summary>
        public ShowCommands lpShowCmd
        {
            set { this._ShExec.lpShowCmd = value; }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        /// <summary>
        /// Provides a managed class for accessing Windows' shell32.dll,ShellExecute unmanaged code.
        /// </summary>
        public InteropShellExecute()
        {
            this._ShExec = new SHEXECOPSTRUCT();
            this._ShExec.hwnd = IntPtr.Zero;
            this._ShExec.lpDirectory = "";
            this._ShExec.lpFile = "";
            this._ShExec.lpOperation = null;
            this._ShExec.lpParameters = "";
            this._ShExec.lpShowCmd = ShowCommands.SW_SHOWNORMAL;
        }
        //***************************************************************************
        // Class Destructors
        // 
        ~InteropShellExecute()
        {
            this._ShExec.hwnd = IntPtr.Zero;
            this._ShExec.lpDirectory = null;
            this._ShExec.lpFile = null;
            this._ShExec.lpOperation = null;
            this._ShExec.lpParameters = null;
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        /// <summary>
        /// Executes the shell command stored within the object.
        /// </summary>
        /// <returns>Returns the IntPtr value of the new window.</returns>
        public IntPtr Execute()
        {
            return ShellExecute(this._ShExec);
        }
        #endregion
    }
}
