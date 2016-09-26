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
using System.Runtime.InteropServices;

namespace RainstormStudios.Unmanaged
{
    class Api_Shlwapi
    {
        public const uint MAX_PATH = 260;

        [DllImport("Shlwapi.dll")]
        public static extern string PathGetArgs(string path);

        public static string SafePathGetArgs(string path)
        {
            try
            {
                return Api_Shlwapi.PathGetArgs(path);
            }
            catch (System.Exception) { }
            return string.Empty;
        }

        [DllImport("Shlwapi.dll")]
        public static extern int PathCompactPathEx(
            System.Text.StringBuilder pszOut, /* Address of the string that has been altered */
            System.Text.StringBuilder pszSrc, /* Pointer to a null-terminated string of max length (MAX_PATH) that contains the path to be altered */
            uint cchMax,					  /* Maximum number of chars to be contained in the new string, including the null character. Example: cchMax = 8, then 7 chars will be returned, the last for the null character. */
            uint dwFlags);					  /* Reserved */

        public static string PathCompactPathEx(string source, uint maxChars)
        {
            StringBuilder pszOut = new StringBuilder((int)Api_Shlwapi.MAX_PATH);
            StringBuilder pszSrc = new StringBuilder(source);

            int result = Api_Shlwapi.PathCompactPathEx(pszOut, pszSrc, maxChars, (uint)0);
            if (result == 1)
                return pszOut.ToString();
            else
            {
                System.Diagnostics.Debug.WriteLine("Win32.PathCompactPathEx failed to compact the path '" + source + "' down to '" + maxChars + "' characters.");
                return string.Empty;
            }
        }
    }
}
