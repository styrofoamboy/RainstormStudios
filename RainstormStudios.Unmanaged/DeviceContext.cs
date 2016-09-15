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
using System.Drawing;
using System.Collections.Generic;
using System.Text;

namespace RainstormStudios.Unmanaged
{
    [Author("Unfried, Michael")]
    public class DeviceContext : IDisposable
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private IntPtr
            _dc;
        private System.Drawing.Graphics
            _grphx;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public Graphics ManagedGraphics
        {
            get
            {
                if (this._grphx != null)
                    return this._grphx;
                else
                    return this.GetGraphics();
            }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        private DeviceContext(IntPtr dc)
        {
            this._dc = dc;
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public void Dispose()
        {
            if (this._grphx != null)
                this._grphx.Dispose();
            if (this._dc != IntPtr.Zero)
                Win32.DestroyDC(this._dc);
            this._dc = IntPtr.Zero;
        }
        public Graphics GetGraphics()
        {
            this._grphx = Graphics.FromHdc(this._dc);
            return this._grphx;
        }
        //***************************************************************************
        // Static Methods
        // 
        public static DeviceContext GetPrimaryScreen()
        {
            return DeviceContext.GetDevice(System.Windows.Forms.Screen.PrimaryScreen.DeviceName);
        }
        public static DeviceContext GetScreen(int screenNum)
        {
            return DeviceContext.GetScreen(System.Windows.Forms.Screen.AllScreens[screenNum].DeviceName);
        }
        public static DeviceContext GetScreen(string deviceName)
        {
            return DeviceContext.GetDevice(deviceName);
        }
        public static DeviceContext GetWindow(string windowName)
        {
            return DeviceContext.GetWindow(Win32.FindWindow(windowName));
        }
        public static DeviceContext GetWindow(IntPtr hwnd)
        {
            return new DeviceContext(Win32.GetWindowDC(hwnd));
        }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Static Methods
        // 
        private static DeviceContext GetDevice(string devName)
        {
            return new DeviceContext(Win32.CreateScreenDC(devName));
        }
        #endregion
    }
}
