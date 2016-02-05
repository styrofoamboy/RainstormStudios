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
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;

namespace RainstormStudios.Unmanaged
{
    /// <summary>
    /// Provides access to unmanaged Win32 MFC functions.
    /// </summary>
    [Author("Unfried, Michael")]
    public abstract class Win32
    {
        public static bool
            AbortEnumWindows = false;

        #region Nested Types
        //***************************************************************************
        // Nested Classes
        // 
        /// <summary>
        /// Provides events that will notify the registering class of windows that are created and destroyed.
        /// This class is implemented as a singleton.
        /// </summary>
        public sealed class WindowEventManager
        {
            #region Declarations
            //***************************************************************************
            // Public Fields
            // 
            public static int
                PollInterval = 5000;
            //***************************************************************************
            // Private Fields
            // 
            System.Threading.ManualResetEvent
                _mre;
            bool
                _running;
            List<IntPtr>
                _oldWindows;
            //***************************************************************************
            // Singleton Instance
            private static readonly WindowEventManager
                singleInstance = new WindowEventManager();
            //***************************************************************************
            // Private Events
            // 
            private event WindowHookEventHandler innerWindowCreated;
            private event WindowHookEventHandler innerWindowDestroyed;
            private event EventHandler ManagerStarted;
            private event EventHandler ManagerStopped;
            //***************************************************************************
            // Public Events
            // 
            public event WindowHookEventHandler WindowCreated
            {
                add
                {
                    innerWindowCreated += value;
                    if (!_running)
                        this.StartThread();
                }
                remove
                {
                    innerWindowCreated -= value;
                    if (innerWindowCreated == null && innerWindowDestroyed == null)
                        this.StopThread();
                }
            }
            public event WindowHookEventHandler WindowDestroyed
            {
                add
                {
                    innerWindowDestroyed += value;
                    if (!_running)
                        this.StartThread();
                }
                remove
                {
                    innerWindowDestroyed -= value;
                    if (innerWindowCreated == null && innerWindowDestroyed == null)
                        this.StopThread();
                }
            }
            #endregion

            #region Properties
            //***************************************************************************
            // Public Properties
            // 
            public static WindowEventManager Instance
            { get { return singleInstance; } }
            #endregion

            #region Class Constructors
            //***************************************************************************
            // Class Constructors
            // 
            private WindowEventManager()
            {
                this._oldWindows = new List<IntPtr>();
                this._mre = new System.Threading.ManualResetEvent(false);
                this._running = false;
            }
            #endregion

            #region Public Methods
            //***************************************************************************
            // Public Methods
            // 
            #endregion

            #region Private Methods
            //***************************************************************************
            // Private Methods
            // 
            private void StartThread()
            {
                this._running = true;
                this._mre.Reset();
                RainstormStudios.GenericCrossThreadDelegate del = new GenericCrossThreadDelegate(this.ThreadWorker);
                del.BeginInvoke(new AsyncCallback(this.ThreadWorkerCallback), del);
            }
            private void StopThread()
            {
                this._running = false;
                this._mre.Set();
            }
            //***************************************************************************
            // Thread Workers
            // 
            private void ThreadWorker()
            {
                this._oldWindows = Win32.GetWindows();
                this.OnManagerStarted(EventArgs.Empty);
                while (this._running)
                {
                    this.PollChanges();
                    this._mre.WaitOne(PollInterval);
                }
            }
            private void ThreadWorkerCallback(IAsyncResult state)
            {
                RainstormStudios.GenericCrossThreadDelegate del = (RainstormStudios.GenericCrossThreadDelegate)state.AsyncState;
                del.EndInvoke(state);
                this._oldWindows.Clear();
                this.OnManagerStopped(EventArgs.Empty);
            }
            private void PollChanges()
            {
                List<IntPtr> currentWindows = Win32.GetWindows();
                if (this.innerWindowCreated != null)
                {
                    IntPtr[] newWindows = currentWindows.Except(this._oldWindows).ToArray();
                    for (int i = 0; i < newWindows.Length; i++)
                        this.OnWindowCreated(new WindowHookEventArgs(newWindows[i], Win32.GetWindowTitle(newWindows[i]), Win32.GetClassName(newWindows[i])));
                    newWindows = null;
                }

                if (this.innerWindowDestroyed != null)
                {
                    IntPtr[] lostWindows = this._oldWindows.Except(currentWindows).ToArray();
                    for (int i = 0; i < lostWindows.Length; i++)
                        this.OnWindowDestroyed(new WindowHookEventArgs(lostWindows[i], Win32.GetWindowTitle(lostWindows[i]), Win32.GetClassName(lostWindows[i])));
                    lostWindows = null;
                }

                this._oldWindows.Clear();
                this._oldWindows = currentWindows;
            }
            //***************************************************************************
            // Event Triggers
            // 
            private void OnWindowCreated(WindowHookEventArgs e)
            {
                if (this.innerWindowCreated != null)
                    innerWindowCreated.Invoke(this, e);
            }
            private void OnWindowDestroyed(WindowHookEventArgs e)
            {
                if (this.innerWindowDestroyed != null)
                    innerWindowDestroyed.Invoke(this, e);
            }
            private void OnManagerStarted(EventArgs e)
            {
                if (this.ManagerStarted != null)
                    this.ManagerStarted.Invoke(this, e);
            }
            private void OnManagerStopped(EventArgs e)
            {
                if (this.ManagerStopped != null)
                    this.ManagerStopped.Invoke(this, e);
            }
            #endregion
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // GDI32
        // 
        public static IntPtr CreateScreenDC(int screenNum)
        {
            return Win32.CreateScreenDC(Screen.AllScreens[screenNum].DeviceName);
        }
        public static IntPtr CreateScreenDC(string deviceName)
        {
            return Api_Gdi32.CreateDC(IntPtr.Zero, deviceName, IntPtr.Zero, IntPtr.Zero);
        }
        public static IntPtr GetWindowDC(IntPtr hWnd)
        {
            return Api_User32.GetWindowDC(hWnd);
        }
        public static bool DestroyDC(IntPtr deviceContext)
        {
            return Api_Gdi32.DeleteDC(deviceContext);
        }
        public static bool BitBlit(IntPtr hdcDestination, int xDst, int yDst, int wDst, int hDst, IntPtr hdcSource, int xSrc, int ySrc, TernaryRasterOperations rasterOp)
        {
            return Api_Gdi32.BitBlt(hdcDestination, xDst, yDst, wDst, hDst, hdcSource, xSrc, ySrc, (uint)rasterOp);
        }
        public static bool StretchBlit(IntPtr hdcDestination, int xDst, int yDst, int wDst, int hDst, IntPtr hdcSource, int xSrc, int ySrc, int wSrc, int hSrc, TernaryRasterOperations rasterOp)
        {
            return Api_Gdi32.StretchBlt(hdcDestination, xDst, yDst, wDst, hDst, hdcSource, xSrc, ySrc, wSrc, hSrc, (uint)rasterOp);
        }
        //***************************************************************************
        // USER32 - Windows
        // 
        public static IntPtr GetActiveWindow()
        {
            return Api_User32.GetActiveWindow();
        }
        public static IntPtr GetDesktopWindow()
        {
            return Api_User32.GetDesktopWindow();
        }
        public static IntPtr GetForegroundWindow()
        {
            return Api_User32.GetForegroundWindow();
        }
        public static bool CloseWindow(IntPtr hWnd)
        {
            return Api_User32.CloseWindow(hWnd);
        }
        public static bool BringWindowToTop(IntPtr hWnd)
        {
            return Api_User32.BringWindowToTop(hWnd);
        }
        public static void SetFocus(IntPtr hWnd)
        {
            Api_User32.SetFocus(hWnd);
        }
        public static bool IsWindowVisible(IntPtr hWnd)
        {
            return Api_User32.IsWindowVisible(hWnd);
        }
        public static IntPtr FindWindow(string windowName)
        {
            return Win32.FindWindow(null, windowName);
        }
        public static IntPtr FindWindow(string className, string windowName)
        {
            return Api_User32.FindWindow(className, windowName);
        }
        public static IntPtr FindWindowEx(IntPtr parentHandle, string className)
        {
            return Api_User32.FindWindowEx(parentHandle, IntPtr.Zero, className, IntPtr.Zero);
        }
        public static int GetWindowProcessID(IntPtr hWnd)
        {
            int procId;
            Api_User32.GetWindowThreadProcessId(hWnd, out procId);
            return procId;
        }
        public static IntPtr GetParent(IntPtr hWnd)
        {
            return Api_User32.GetParent(hWnd);
        }
        public static IntPtr GetWindowFromPoint(int x, int y)
        {
            return Win32.GetWindowFromPoint(new Point(x, y));
        }
        public static IntPtr GetWindowFromPoint(Point pt)
        {
            return Api_User32.WindowFromPoint(pt);
        }
        [Obsolete("This method has been depreciated.  Please use 'GetWindowTitle'.",false)]
        public static string GetWindowText(IntPtr hwnd)
        {
            return GetWindowTitle(hwnd);
        }
        [Obsolete("This method has been depreciated.  Please use 'GetWindowTitle'.", false)]
        public static string GetWindowText(IntPtr hwnd, int maxLen)
        {
            return GetWindowTitle(hwnd, maxLen);
        }
        public static string GetWindowTitle(IntPtr hWnd)
        {
            return Win32.GetWindowTitle(hWnd, 100);
        }
        public static string GetWindowTitle(IntPtr hWnd, int maxLen)
        {
            StringBuilder sb = new StringBuilder(maxLen);
            Api_User32.GetWindowText(hWnd, sb, sb.Capacity);
            return sb.ToString();
        }
        public static WindowInfo GetWindowInfo(IntPtr hwnd)
        {
            WindowInfo wnfo = new WindowInfo();
            wnfo.cbSize = Marshal.SizeOf(wnfo);
            if (!Api_User32.GetWindowInfo(hwnd, ref wnfo))
                throw new Win32Exception(Marshal.GetLastWin32Error(), "Unable to retrieve window information.");
            return wnfo;
        }
        public static string GetClassName(IntPtr hwnd)
        {
            return Win32.GetClassName(hwnd, 100);
        }
        public static string GetClassName(IntPtr hwnd, int maxLen)
        {
            StringBuilder sb = new StringBuilder(maxLen);
            Api_User32.GetClassName(hwnd, sb, sb.Capacity);
            return sb.ToString();
        }
        public static ScrollBarInfo GetScrollBarInfo(IntPtr hwnd, ScrollBarObjectID objId)
        {
            ScrollBarInfo info = new ScrollBarInfo();
            info.cbSize = Marshal.SizeOf(info);
            if (Api_User32.GetScrollBarInfo(hwnd, (uint)objId, ref info) != IntPtr.Zero)
                throw new Win32Exception(Marshal.GetLastWin32Error(), "Unable to obtain scrollbar information.");
            return info;
        }
        public static WindowPlacement GetWindowPlacement(IntPtr hWnd)
        {
            WindowPlacement wplc = new WindowPlacement();
            if (!Api_User32.GetWindowPlacement(hWnd, ref wplc))
                throw new Win32Exception(Marshal.GetLastWin32Error(), "An error occured trying to obtain window placement information.");
            return wplc;
        }
        public static List<IntPtr> GetWindows()
        {
            List<IntPtr> result = new List<IntPtr>();
            GCHandle listHandle = GCHandle.Alloc(result);
            try
            {
                Win32Callback childProc = new Win32Callback(EnumWindow);
                Api_User32.EnumWindows(childProc, GCHandle.ToIntPtr(listHandle));
            }
            finally
            {
                if (listHandle.IsAllocated)
                    listHandle.Free();
            }
            return result;
        }
        public static List<IntPtr> GetChildWindows(IntPtr hwndParent)
        {
            List<IntPtr> result = new List<IntPtr>();
            GCHandle listHandle = GCHandle.Alloc(result);
            try
            {
                Win32Callback childProc = new Win32Callback(EnumWindow);
                Api_User32.EnumChildWindows(hwndParent, childProc, GCHandle.ToIntPtr(listHandle));
            }
            finally
            {
                if (listHandle.IsAllocated)
                    listHandle.Free();
            }
            return result;
        }
        public static string GetTextContent(IntPtr hWnd)
        {
            StringBuilder sb = new StringBuilder();
            Api_User32.SendMessage(hWnd, (int)Win32Messages.WM_GETTEXT, (IntPtr)sb.Capacity, sb);
            return sb.ToString();
        }
        public static string SetTextContent(IntPtr hWnd)
        {
            StringBuilder sb = new StringBuilder();
            Api_User32.SendMessage(hWnd, (int)Win32Messages.WM_SETTEXT, (IntPtr)sb.Capacity, sb);
            return sb.ToString();
        }
        public static List<string> GetListContent(IntPtr hWnd)
        {
            List<string> lst = new List<string>();
            int count = (int)Api_User32.SendMessage(hWnd, (int)Win32Messages.LB_GETCOUNT, IntPtr.Zero, null);
            
            for (int i = 0; i < count; i++)
            {
                StringBuilder sb = new StringBuilder();
                Api_User32.SendMessage(hWnd, Win32Const.WM_GETTEXT, (IntPtr)i, sb);
                lst.Add(sb.ToString());
            }
            return lst;
        }
        private static bool EnumWindow(IntPtr handle, IntPtr pointer)
        {
            GCHandle gch = GCHandle.FromIntPtr(pointer);
            List<IntPtr> list = gch.Target as List<IntPtr>;
            if (list == null)
                throw new InvalidCastException("GCHandle Target could not be cast as List<IntPtr>");

            list.Add(handle);

            if (Win32.AbortEnumWindows)
                return false;

            return true;
        }
        //***************************************************************************
        // USER32 - Graphics Functions
        // 
        public static Rectangle GetWindowRect(IntPtr hWnd)
        {
            RECT bnds = new RECT();
            if (Api_User32.GetWindowRect(hWnd, ref bnds))
                return new Rectangle(bnds.left, bnds.top, bnds.Width, bnds.Height);
            else
                return Rectangle.Empty;
        }
        public static Rectangle GetClientRect(IntPtr hWnd)
        {
            RECT bnds = new RECT();
            if (Api_User32.GetClientRect(hWnd, ref bnds))
                return new Rectangle(bnds.left, bnds.top, bnds.Width, bnds.Height);
            else
                return Rectangle.Empty;
        }
        public static void RedrawWindow(IntPtr hwnd)
        {
            Win32.RedrawWindow(hwnd, RedrawWindowFlags.RDW_FRAME | RedrawWindowFlags.RDW_UPDATENOW | RedrawWindowFlags.RDW_INVALIDATE);
        }
        public static void RedrawWindow(IntPtr hwnd, RedrawWindowFlags flags)
        {
            Api_User32.RedrawWindow(hwnd, IntPtr.Zero, IntPtr.Zero, (uint)flags);
        }
        public static IntPtr GetDeviceContext(IntPtr hWnd)
        {
            return Api_User32.GetDC(hWnd);
        }
        public static IntPtr GetDeviceContext(HandleRef hWnd)
        {
            return Api_User32.GetDC(hWnd);
        }
        public static void ReleaseDeviceContext(IntPtr hWnd, IntPtr hDC)
        {
            Api_User32.ReleaseDC(hWnd, hDC);
        }
        public static void ReleaseDeviceContext(HandleRef hWnd, IntPtr hDC)
        {
            Api_User32.ReleaseDC(hWnd, hDC);
        }
        public static IntPtr CreateCursor(Bitmap xor, Bitmap and, Point hotspot)
        {
            return CreateCursor(xor, and, hotspot.X, hotspot.Y);
        }
        public static IntPtr CreateCursor(Bitmap xor, Bitmap and, int xHotspot, int yHotspot)
        {
            ImageConverter ic = new ImageConverter();
            byte[] bfrXor = (byte[])ic.ConvertTo(xor, typeof(byte[]));
            byte[] bfrAnd = (byte[])ic.ConvertTo(and, typeof(byte[]));
            return Api_User32.CreateCursor(IntPtr.Zero, xHotspot, yHotspot, and.Width, and.Height, bfrAnd, bfrXor);
        }
        public static void SetCursor(IntPtr hCursor)
        {
            Api_User32.SetCursor(hCursor);
        }
        public static void ReleaseCursor(IntPtr hCursor)
        {
            Api_User32.DestroyCursor(hCursor);
        }
        //***************************************************************************
        // USER32 - Message Pumping
        public static IntPtr SendMessage(HandleRef hWnd, Win32Messages msg, IntPtr wParam, IntPtr lParam)
        {
            return Win32.SendMessage(hWnd.Handle, msg, wParam, lParam);
        }
        public static IntPtr SendMessage(IntPtr hWnd,Win32Messages msg,IntPtr wParam,IntPtr lParam)
        {
            IntPtr retVal = Api_User32.SendMessage(hWnd, (int)msg, wParam, lParam);
            //if (retVal.ToInt32() > 0)
            //    throw new Win32Exception(retVal.ToInt32(), "An error occured while sending the window message.");
            return retVal;
        }
        public static IntPtr SendMessage(HandleRef hWnd, Win32Messages msg, int wParam, IntPtr lParam)
        {
            return Win32.SendMessage(hWnd.Handle, msg, wParam, lParam);
        }
        public static IntPtr SendMessage(IntPtr hWnd, Win32Messages msg, int wParam, IntPtr lParam)
        {
            IntPtr retVal = Api_User32.SendMessage(hWnd, (int)msg, wParam, lParam);
            //if (retVal.ToInt32() > 0)
            //    throw new Win32Exception(retVal.ToInt32(), "An error occured while sending the window message.");
            return retVal;
        }
        //***************************************************************************
        // USER32 - Input
        public static void SendKeyPress(VK key)
        {
            INPUT[] inp = new INPUT[1];
            inp[0].type = Win32Const.INPUT_KEYBOARD;
            inp[0].ki.wScan = 0;
            inp[0].ki.dwFlags = 0;
            inp[0].ki.time = 0;
            inp[0].ki.wVk = (ushort)key;
            inp[0].ki.dwExtraInfo = Api_User32.GetMessageExtraInfo();
            Win32.SendInput(inp);

            // And now send the key release.
            inp[0].ki.dwFlags = Win32Const.KEYEVENTF_KEYUP;
            Win32.SendInput(inp);
        }
        public static uint SendInput(INPUT[] inputs)
        {
            if (inputs.Length > 0)
                return Api_User32.SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(inputs[0]));
            else
                return 0;
        }
        public static void SetMousePosition(int dx, int dy)
        {
            Win32.SetMousePosition(dx, dy, false);
        }
        public static void SetMousePosition(int dx, int dy, bool relative)
        {
            INPUT[] inp = new INPUT[1];
            inp[0].type = Win32Const.INPUT_MOUSE;
            if (relative)
                inp[0].mi.dwFlags = Win32Const.MOUSEEVENTF_MOVE;
            else
                inp[0].mi.dwFlags = Win32Const.MOUSEEVENTF_MOVE | Win32Const.MOUSEEVENTF_ABSOLUTE;
            inp[0].mi.dx = dx;
            inp[0].mi.dy = dy;
            inp[0].mi.mouseData = 0;
            inp[0].mi.time = 0;
            inp[0].mi.dwExtraInfo = Api_User32.GetMessageExtraInfo();
            Win32.SendInput(inp);
        }
        public static bool HideCaret(Control ctrl)
        {
            return Win32.HideCaret(ctrl.Handle);
        }
        public static bool HideCaret(IntPtr hWnd)
        {
            return Api_User32.HideCaret(hWnd);
        }
        public static bool ShowCaret(Control ctrl)
        {
            return Win32.ShowCaret(ctrl.Handle);
        }
        public static bool ShowCaret(IntPtr hWnd)
        {
            return Api_User32.ShowCaret(hWnd);
        }
        public static bool BlockInputOn()
        {
            return Api_User32.BlockInput(true);
        }
        public static bool BlockInputOff()
        {
            return Api_User32.BlockInput(false);
        }
        #endregion
    }
    public delegate void WindowHookEventHandler(object sender, WindowHookEventArgs e);
    public class WindowHookEventArgs : EventArgs
    {
        public readonly IntPtr
            Handle;
        public string
            WindowTitle;
        public string
            WindowClass;

        public WindowHookEventArgs(IntPtr hWnd, string wTitle, string wClass)
        {
            this.Handle = hWnd;
            this.WindowTitle = wTitle;
            this.WindowClass = wClass;
        }

        public override string ToString()
        { return string.Format("[WindowHookEventArgs[Title:{0}|Class:{1}|Handle:{2}", WindowTitle, WindowClass, Handle); }
    }
}
