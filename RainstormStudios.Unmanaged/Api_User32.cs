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
using System.Drawing;
using System.Runtime.InteropServices;

namespace RainstormStudios.Unmanaged
{
    [Author("Unfried, Michael")]
    class Api_User32
    {
        const uint MAX_PATH = 260;

        #region Hooks - user32.dll

        /// <summary>
        /// Defines the layout of the MouseHookStruct
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct MSLLHOOKSTRUCT
        {
            public Point Point;
            public int MouseData;
            public int Flags;
            public int Time;
            public int ExtraInfo;
        }

        /// <summary>
        ///  EventArgs class for use as parameters by HookEventHandler delegates
        /// </summary>
        public class HookEventArgs : EventArgs
        {
            private int _code;
            private IntPtr _wParam;
            private IntPtr _lParam;

            /// <summary>
            /// Initializes a new instance of the HookEventArgs class
            /// </summary>
            /// <param name="code">the hook code</param>
            /// <param name="wParam">hook specific information</param>
            /// <param name="lParam">hook specific information</param>
            public HookEventArgs(int code, IntPtr wParam, IntPtr lParam)
            {
                _code = code;
                _wParam = wParam;
                _lParam = lParam;
            }

            /// <summary>
            /// The hook code
            /// </summary>
            public int Code
            {
                get
                {
                    return _code;
                }
            }

            /// <summary>
            /// A pointer to data
            /// </summary>
            public IntPtr wParam
            {
                get
                {
                    return _wParam;
                }
            }

            /// <summary>
            /// A pointer to data
            /// </summary>
            public IntPtr lParam
            {
                get
                {
                    return _lParam;
                }
            }
        }

        /// <summary>
        /// Event delegate for use with the HookEventArgs class
        /// </summary>
        public delegate void HookEventHandler(object sender, HookEventArgs e);

        /// <summary>
        /// Defines the various types of hooks that are available in Windows
        /// </summary>
        public enum HookTypes : int
        {
            WH_JOURNALRECORD = 0,
            WH_JOURNALPLAYBACK = 1,
            WH_KEYBOARD = 2,
            WH_GETMESSAGE = 3,
            WH_CALLWNDPROC = 4,
            WH_CBT = 5,
            WH_SYSMSGFILTER = 6,
            WH_MOUSE = 7,
            WH_HARDWARE = 8,
            WH_DEBUG = 9,
            WH_SHELL = 10,
            WH_FOREGROUNDIDLE = 11,
            WH_CALLWNDPROCRET = 12,
            WH_KEYBOARD_LL = 13,
            WH_MOUSE_LL = 14
        }

        public enum ShellHookMessages
        {
            HSHELL_WINDOWCREATED = 1,
            HSHELL_WINDOWDESTROYED = 2,
            HSHELL_ACTIVATESHELLWINDOW = 3,
            HSHELL_WINDOWACTIVATED = 4,
            HSHELL_GETMINRECT = 5,
            HSHELL_REDRAW = 6,
            HSHELL_TASKMAN = 7,
            HSHELL_LANGUAGE = 8,
            HSHELL_ACCESSIBILITYSTATE = 11
        }

        public delegate int HookProc(int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr SetWindowsHookEx(HookTypes hookType, HookProc hookProc, IntPtr hInstance, int nThreadId);

        [DllImport("user32.dll")]
        public static extern int UnhookWindowsHookEx(IntPtr hookHandle);

        [DllImport("user32.dll")]
        public static extern int CallNextHookEx(IntPtr hookHandle, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern int RegisterShellHookWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern int DeregisterShellHookWindow(IntPtr hWnd);

        #endregion Hooks

        #region System - Kernel32.dll

        [DllImport("kernel32.dll", EntryPoint = "GetLastError", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern Int32 GetLastError();

        [DllImport("kernel32.dll")]
        static extern IntPtr LocalFree(IntPtr hMem);

        #endregion

        #region Windows - user32.dll

        [DllImport("user32.dll")]
        public static extern int GetWindowModuleFileName(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        public static extern int SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll")]
        public static extern bool GetWindowPlacement(IntPtr window, ref WindowPlacement position);

        [DllImport("User32.dll")]
        public static extern int RegisterWindowMessage(string message);

        [DllImport("User32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)] // Return value is a 4-byte Win32 'BOOL'
        public static extern void EnumWindows(Win32Callback callback, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)] // Return value is a 4-byte Win32 'BOOL'
        public static extern bool EnumChildWindows(IntPtr hwndParent, Win32Callback callback, IntPtr lParam);

        [DllImport("User32.dll")]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("User32.dll")]
        public static extern Int32 GetWindow(IntPtr hWnd, Int32 wCmd);

        [DllImport("user32.dll")]
        public static extern IntPtr GetActiveWindow();

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className, IntPtr windowTitle);

        [DllImport("user32.dll")]
        public static extern bool CloseWindow(IntPtr hwnd);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

        [DllImport("user32.dll")]
        public static extern IntPtr GetMessageExtraInfo();

        [DllImport("User32.dll")]
        public static extern IntPtr GetParent(IntPtr hWnd);

        [DllImport("User32.dll")]
        public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("User32.dll")]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("User32.dll")]
        public static extern WindowStyles GetWindowLong(IntPtr hWnd, int index);

        [DllImport("User32.dll")]
        public static extern int SendMessageTimeout(IntPtr hWnd, int uMsg, int wParam, int lParam, int fuFlags, int uTimeout, out int lpdwResult);

        [DllImport("User32.dll")]
        public static extern int GetClassLong(IntPtr hWnd, int index);

        [DllImport("User32.dll")]
        public static extern int GetWindowThreadProcessId(IntPtr hWnd, out int processId);

        [DllImport("User32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int uMsg, IntPtr wParam, IntPtr lParam);

        [DllImport("User32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int uMsg, IntPtr wParam, [Out] StringBuilder lParam);

        [DllImport("User32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int uMsg, IntPtr wParam, ref RECT lParam);

        [DllImport("User32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int uMsg, IntPtr wParam, ref POINT lParam);

        [DllImport("User32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int uMsg, int wParam, IntPtr lParam);

        [DllImport("User32.dll")]
        public static extern IntPtr PostMessage(IntPtr hWnd, int uMsg, IntPtr wParam, IntPtr lParam);

        [DllImport("User32.dll")]
        public static extern void SwitchToThisWindow(IntPtr hWnd, int altTabActivated);

        [DllImport("User32.dll")]
        public static extern int ShowWindowAsync(IntPtr hWnd, int command);

        [DllImport("user32.dll")]
        public static extern IntPtr GetDesktopWindow();

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool BringWindowToTop(IntPtr window);

        [DllImport("user32.dll")]
        public static extern bool GetWindowInfo(IntPtr hwnd, ref WindowInfo info);

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowDC(IntPtr hwnd);

        [DllImport("user32.dll")]
        public static extern IntPtr GetDC(IntPtr hwnd);

        [DllImport("user32.dll")]
        public static extern IntPtr GetDC(HandleRef hwnd);

        [DllImport("user32.dll")]
        public static extern Int32 ReleaseDC(IntPtr hwnd, IntPtr hdc);

        [DllImport("user32.dll")]
        public static extern Int32 ReleaseDC(HandleRef hwnd, IntPtr hdc);

        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hwnd, ref RECT rectangle);

        [DllImport("user32.dll")]
        public static extern bool GetClientRect(IntPtr hwnd, ref RECT rectangle);

        [DllImport("user32.dll")]
        public static extern IntPtr WindowFromPoint(Point pt);

        [DllImport("user32.dll")]
        public static extern IntPtr SetCapture(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern int ReleaseCapture();

        [DllImport("user32.dll")]
        public static extern IntPtr SelectObject(IntPtr hDc, IntPtr hObject);

        [DllImport("user32.dll")]
        public static extern IntPtr GetStockObject(int nObject);

        [DllImport("user32.dll")]
        public static extern int InvalidateRect(IntPtr hWnd, IntPtr lpRect, int bErase);

        [DllImport("user32.dll")]
        public static extern int UpdateWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern int RedrawWindow(IntPtr hWnd, IntPtr lprcUpdate, IntPtr hrgnUpdate, uint flags);

        [DllImport("user32.dll")]
        public static extern bool RedrawWindow(IntPtr hWnd, [In] ref RECT lprcUpdate, IntPtr hrgnUpdate, uint flags);

        [DllImport("user32.dll")]
        public static extern IntPtr SetFocus(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true, EntryPoint = "GetScrollBarInfo")]
        public static extern IntPtr GetScrollBarInfo(IntPtr hWnd, uint idObject, ref ScrollBarInfo psbi);

        [DllImport("user32.dll")]
        public static extern bool HideCaret(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool ShowCaret(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool BlockInput(bool blockIt);

        [DllImport("user32.dll")]
        public static extern IntPtr CreateCursor(IntPtr hInst, int xHotSpot, int yHotSpot, int nWidth, int nHeight, byte[] pvANDPlane, byte[] pvXORPlane);

        [DllImport("user32.dll")]
        public static extern IntPtr SetCursor(IntPtr hCursor);

        [DllImport("user32.dll")]
        public static extern bool DestroyCursor(IntPtr hCursor);

        #endregion

        [DllImport("User32")]
        public static extern int LockWindowUpdate(IntPtr windowHandle);

        [DllImport("User32")]
        static extern int RegisterHotKey(IntPtr hWnd, int id, uint modifiers, uint virtualkeyCode);

        [DllImport("User32")]
        static extern int UnregisterHotKey(IntPtr hWnd, int id);

        [DllImport("Kernel32")]
        static extern short GlobalAddAtom(string atomName);

        [DllImport("Kernel32")]
        static extern short GlobalDeleteAtom(short atom);		

        public static short MAKEWORD(byte a, byte b)
        {
            return ((short)(((byte)(a & 0xff)) | ((short)((byte)(b & 0xff))) << 8));
        }

        public static byte LOBYTE(short a)
        {
            return ((byte)(a & 0xff));
        }

        public static byte HIBYTE(short a)
        {
            return ((byte)(a >> 8));
        }

        public static int MAKELONG(short a, short b)
        {
            return (((int)(a & 0xffff)) | (((int)(b & 0xffff)) << 16));
        }

        public static short HIWORD(int a)
        {
            return ((short)(a >> 16));
        }

        public static short LOWORD(int a)
        {
            return ((short)(a & 0xffff));
        }

        [DllImport("Kernel32")]
        public static extern int CopyFile(string source, string destination, int failIfExists);
    }
}
