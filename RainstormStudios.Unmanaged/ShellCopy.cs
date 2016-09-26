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
using System.ComponentModel;

namespace RainstormStudios.Unmanaged
{
    /// <summary>
    /// Provides a managed class to access Windows' Shell32.dll,SHFileOperation unmanaged code.
    /// </summary>
    public class InteropShellFileOperation
    {
        #region Structs & Enums
        //***************************************************************************
        // Structs & Enums
        // 
        public enum FO_Func : uint
        {
            FO_MOVE = 0x0001,
            FO_COPY = 0x0002,
            FO_DELETE = 0x0003,
            FO_RENAME = 0x0004
        }
        struct SHFILEOPSSTRUCT
        {
            public IntPtr hwnd;
            public FO_Func wFunc;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pFrom;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pTo;
            public ushort fFlags;
            public bool fAnyOperationsAborted;
            public IntPtr hNameMappings;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpszProgressTitle;
        }
        #endregion

        #region Declarations
        //***************************************************************************
        // Interop MFC Links
        // 
        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        static extern int SHFileOperation([In] ref SHFILEOPSSTRUCT lpFileOp);

        //***************************************************************************
        // Global Variables
        // 
        private SHFILEOPSSTRUCT _ShFile;
        public FILEOP_FLAGS fFlags;

        //***************************************************************************
        // Public Fields
        // 
        /// <summary>
        /// The IntPtr value of the window which owns this object.
        /// </summary>
        public IntPtr hwnd
        {
            set { this._ShFile.hwnd = value; }
        }
        /// <summary>
        /// The file operation function to perform.
        /// </summary>
        public FO_Func wFunc
        {
            set { this._ShFile.wFunc = value; }
        }
        /// <summary>
        /// The source file or path to operate on.
        /// </summary>
        public string pFrom
        {
            set { this._ShFile.pFrom = value + '\0' + '\0'; }
        }
        /// <summary>
        /// The destination file or path to operate on.
        /// </summary>
        public string pTo
        {
            set { this._ShFile.pTo = value + '\0' + '\0'; }
        }
        /// <summary>
        /// Return error if any operations where aborted.
        /// </summary>
        public bool fAnyOperationsAborted
        {
            set { this._ShFile.fAnyOperationsAborted = value; }
        }
        public IntPtr hNameMappings
        {
            set { this._ShFile.hNameMappings = value; }
        }
        /// <summary>
        /// Specifies the text in the title bar of the progress window.
        /// </summary>
        public string lpszProgressTitle
        {
            set { this._ShFile.lpszProgressTitle = value + '\0'; }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        /// <summary>
        /// Provides a managed class to access Windows' Shell32.dll,SHFileOperation unmanaged code.
        /// </summary>
        public InteropShellFileOperation()
        {
            this.fFlags = new FILEOP_FLAGS();
            this._ShFile = new SHFILEOPSSTRUCT();
            this._ShFile.hwnd = IntPtr.Zero;
            this._ShFile.wFunc = FO_Func.FO_COPY;
            this._ShFile.pFrom = "";
            this._ShFile.pTo = "";
            this._ShFile.fAnyOperationsAborted = false;
            this._ShFile.hNameMappings = IntPtr.Zero;
            this._ShFile.lpszProgressTitle = "";
        }
        //***************************************************************************
        // Class Destructors
        // 
        ~InteropShellFileOperation()
        {
            this._ShFile.hwnd = IntPtr.Zero;
            this._ShFile.fFlags = 0;
            this._ShFile.hNameMappings = IntPtr.Zero;
            this._ShFile.lpszProgressTitle = null;
            this._ShFile.pFrom = null;
            this._ShFile.pTo = null;
            this._ShFile.fFlags = 0;
            this.fFlags = null;
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public bool Execute()
        {
            // Prepare the parameter flags.
            this._ShFile.fFlags = this.fFlags.Flag;

            // Execute the operation.
            int ReturnValue = SHFileOperation(ref this._ShFile);

            // Return error level.
            if (ReturnValue == 0)
                return true;
            else
                return false;
        }
        #endregion
    }
    /// <summary>
    /// Contains bool option flags for shell32.dll,SHFileOperation calls.
    /// </summary>
    public class FILEOP_FLAGS
    {
        #region Structs & Enums
        //***************************************************************************
        // Structs & Enums
        // 
        private enum FILEOP_FLAGS_ENUM : ushort
        {
            FOF_MULTIDESTFILES=0x0001,
            FOF_CONFIRMMOUSE=0x0002,
            FOF_SILENT=0x0004,                  // Don't create progress report.
            FOF_RENAMEONCOLLISION=0x0008,
            FOF_NOCONFIRMATION=0x0010,          // Don't prompt the user.
            FOF_WANTMAPPINGHANDLE=0x0020,       // File in SHFILEOPSTRUCT.hNameMappings -- Must be freed using SHFreeMappings
            FOF_ALLOWUNDO=0x0040,
            FOF_FILESONLY=0x0080,               // on *.*, do files only
            FOF_SIMPLEPROGRESS=0x0100,          // means don't show names of files
            FOF_NOCONFIRMMKDIR=0x0200,          // don't confirm created any need directories
            FOF_NOERRORUI=0x0400,               // don't display errors to the screen
            FOF_NOCOPYSECURITYATTRIBS=0x0800,   // don't copy NT file security attributes
            FOF_NORECURSION=0x1000,             // don't recurse into subdirectories
            FOF_NO_CONNECTED_ELEMENTS=0x2000,   // don't operate on connected elements
            FOF_WANTNUKEWARNING=0x4000,         // during delete operation, warn if nuking instead of recycling (partially overrides FOF_NOCONFIRMATION)
            FOF_NORECURSEREPARSE=0x8000         // treats reparse points as objects, not as containers
        }
        #endregion

        #region Declarations
        //***************************************************************************
        // Global Variables
        // 
        public bool FOF_MULTIDESTFILES = false;
        public bool FOF_CONFIRMMOUSE = false;
        public bool FOF_SILENT = false;
        public bool FOF_RENAMEONCOLLISION = false;
        public bool FOF_NOCONFIRMATION = false;
        public bool FOF_WANTMAPPINGHANDLE = false;
        public bool FOF_ALLOWUNDO = false;
        public bool FOF_FILESONLY = false;
        public bool FOF_SIMPLEPROGRESS = false;
        public bool FOF_NOCONFIRMMKDIR = false;
        public bool FOF_NOERRORUI = false;
        public bool FOF_NOCOPYSECURITYATTRIBS = false;
        public bool FOF_NORECURSION = false;
        public bool FOF_NO_CONNECTED_ELEMENTS = false;
        public bool FOF_WANTNUKEWARNING = false;
        public bool FOF_NORECURSEREPARSE = false;

        //***************************************************************************
        // Public Fields
        // 
        public ushort Flag
        {
            get
            {
                ushort ReturnValue = 0;

                if (this.FOF_MULTIDESTFILES)
                    ReturnValue |= (ushort)FILEOP_FLAGS_ENUM.FOF_MULTIDESTFILES;
                if (this.FOF_CONFIRMMOUSE)
                    ReturnValue |= (ushort)FILEOP_FLAGS_ENUM.FOF_CONFIRMMOUSE;
                if (this.FOF_SILENT)
                    ReturnValue |= (ushort)FILEOP_FLAGS_ENUM.FOF_SILENT;
                if (this.FOF_RENAMEONCOLLISION)
                    ReturnValue |= (ushort)FILEOP_FLAGS_ENUM.FOF_RENAMEONCOLLISION;
                if (this.FOF_NOCONFIRMATION)
                    ReturnValue |= (ushort)FILEOP_FLAGS_ENUM.FOF_NOCONFIRMATION;
                if (this.FOF_WANTMAPPINGHANDLE)
                    ReturnValue |= (ushort)FILEOP_FLAGS_ENUM.FOF_WANTMAPPINGHANDLE;
                if (this.FOF_ALLOWUNDO)
                    ReturnValue |= (ushort)FILEOP_FLAGS_ENUM.FOF_ALLOWUNDO;
                if (this.FOF_FILESONLY)
                    ReturnValue |= (ushort)FILEOP_FLAGS_ENUM.FOF_FILESONLY;
                if (this.FOF_SIMPLEPROGRESS)
                    ReturnValue |= (ushort)FILEOP_FLAGS_ENUM.FOF_SIMPLEPROGRESS;
                if (this.FOF_NOCONFIRMMKDIR)
                    ReturnValue |= (ushort)FILEOP_FLAGS_ENUM.FOF_NOCONFIRMMKDIR;
                if (this.FOF_NOERRORUI)
                    ReturnValue |= (ushort)FILEOP_FLAGS_ENUM.FOF_NOERRORUI;
                if (this.FOF_NOCOPYSECURITYATTRIBS)
                    ReturnValue |= (ushort)FILEOP_FLAGS_ENUM.FOF_NOCOPYSECURITYATTRIBS;
                if (this.FOF_NORECURSION)
                    ReturnValue |= (ushort)FILEOP_FLAGS_ENUM.FOF_NORECURSION;
                if (this.FOF_NO_CONNECTED_ELEMENTS)
                    ReturnValue |= (ushort)FILEOP_FLAGS_ENUM.FOF_NO_CONNECTED_ELEMENTS;
                if (this.FOF_WANTNUKEWARNING)
                    ReturnValue |= (ushort)FILEOP_FLAGS_ENUM.FOF_WANTNUKEWARNING;
                if (this.FOF_NORECURSEREPARSE)
                    ReturnValue |= (ushort)FILEOP_FLAGS_ENUM.FOF_NORECURSEREPARSE;

                return ReturnValue;
            }
        }
        #endregion
    }
}
