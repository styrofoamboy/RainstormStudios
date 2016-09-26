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

namespace RainstormStudios
{
    /// <summary>
    /// Provides static methods for generation output only when compiled with the DEBUG switch.
    /// </summary>
    public static partial class DebugOnly
    {
        [System.Diagnostics.Conditional("DEBUG")]
        public static void ConsoleWrite(string format, params object[] args)
        {
            DebugOnly.ConsoleWrite(string.Format(format, args));
        }
        [System.Diagnostics.Conditional("DEBUG")]
        public static void ConsoleWrite(string msg)
        {
            Console.WriteLine(msg);
        }
        [System.Diagnostics.Conditional("DEBUG")]
        public static void DebugWrite(string format, params object[] args)
        {
            DebugOnly.DebugWrite(string.Format(format, args));
        }
        [System.Diagnostics.Conditional("DEBUG")]
        public static void DebugWrite(string msg)
        {
            System.Diagnostics.Debug.WriteLine(msg);
        }
    }
}
