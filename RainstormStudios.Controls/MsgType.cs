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
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using RainstormStudios.Unmanaged;

namespace RainstormStudios
{
    [Author("Unfried, Michael")]
    public enum ScrollBarInfoFlags : uint
    {
        SIF_RANGE = 0x1,
        SIF_PAGE = 0x2,
        SIF_POS = 0x4,
        SIF_DISABLENOSCROLL = 0x8,
        SIF_TRACKPOS = 0x10,
        SIF_ALL = (SIF_RANGE | SIF_PAGE | SIF_POS | SIF_TRACKPOS)
    }
    [Author("Unfried, Michael")]
    public enum EMFlags : int
    {
        EM_SETSCROLLPOS = 0x0400 + 222
    }
    [Author("Unfried, Michael")]
    public enum ScrollBarFlags : int
    {
        SBS_HORZ = 0x0000,
        SBS_VERT = 0x0001,
        SBS_TOPALIGN = 0x0002,
        SBS_LEFTALIGN = 0x0002,
        SBS_BOTTOMALIGN = 0x0004,
        SBS_RIGHTALIGN = 0x0004,
        SBS_SIZEBOXTOPLEFTALIGN = 0x0002,
        SBS_SIZEBOXBOTTOMRIGHTALIGN = 0x0004,
        SBS_SIZEBOX = 0x0008,
        SBS_SIZEGRIP = 0x0010
    }
    [Author("Unfried, Michael")]
    public enum ScrollBarTypes : uint
    {
        SB_HORZ = 0x0,
        SB_VERT = 0x1,
        SB_CTRL = 0x2,
        SB_BOTH = 0x3
    }
    [Author("Unfried, Michael")]
    [StructLayout(LayoutKind.Sequential)]                   // This is the default layout for a structure
    public struct NCCALCSIZE_PARAMS
    {
        public RECT rect0, rect1, rect2;                    // Can't use an array here so simulate one
        public IntPtr lppos;
    }
    [Author("Unfried, Michael")]
    public enum NCCALCSIZERETURN
    {
        // WM_NCCALCSIZE return flags
        WVR_ALIGNTOP = 0x10,
        WVR_ALIGNLEFT = 0x20,
        WVR_ALIGNBOTTOM = 0x40,
        WVR_ALIGNRIGHT = 0x80,
        WVR_HREDRAW = 0x100,
        WVR_VREDRAW = 0x200,
        WVR_REDRAW = (WVR_HREDRAW | WVR_VREDRAW),
        WVR_VALIDRECTS = 0x400
    }
    [Author("Unfried, Michael")]
    class MsgType
    {
        public MsgType() { }

        static internal string GetMessageName(int msg)
        {
            string s = "";
            if ((msg & (int)Win32Messages.WM_USER) > 0) { msg -= (int)Win32Messages.WM_USER; s = "WM_USER+"; }
            if ((msg & (int)Win32Messages.WM_REFLECT) > 0) { msg -= (int)Win32Messages.WM_REFLECT; s = "WM_REFLECT+"; }
            if ((msg & (int)Win32Messages.WM_APP) > 0) { msg -= (int)Win32Messages.WM_APP; s = "WM_APP+"; }
            if (Enum.IsDefined(typeof(Win32Messages), (Win32Messages)msg))
            {
                return s + Enum.GetName(typeof(Win32Messages), msg).ToString();
            }
            else
            {
                return s + msg.ToString();
            }
        }

        static internal string GetMessageName(int msg, bool includeUndefined, params string[] exclude)
        {
            string s = "";
            if ((msg & (int)Win32Messages.WM_USER) > 0) { msg -= (int)Win32Messages.WM_USER; s = "WM_USER+"; }
            if ((msg & (int)Win32Messages.WM_REFLECT) > 0) { msg -= (int)Win32Messages.WM_REFLECT; s = "WM_REFLECT+"; }
            if ((msg & (int)Win32Messages.WM_APP) > 0) { msg -= (int)Win32Messages.WM_APP; s = "WM_APP+"; }
            if (Enum.IsDefined(typeof(Win32Messages), (Win32Messages)msg))
            {
                s += Enum.GetName(typeof(Win32Messages), msg).ToString();
                foreach (string exclusion in exclude)
                    if (Regex.IsMatch(s, exclusion,
                        RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline))
                        return "";
                return s;
            }
            else
            {
                if (includeUndefined) return s + msg.ToString(); else return "";
            }

        }
    }
}
