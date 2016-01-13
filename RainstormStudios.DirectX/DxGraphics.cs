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
using System.ComponentModel;
using System.Text;
using DX = Microsoft.DirectX;
using D3D = Microsoft.DirectX.Direct3D;

namespace RainstormStudios.DirectX
{
    [Author("Unfried, Michael")]
    public class DxGraphics : IDisposable
    {
        #region Sub Classes
        //***************************************************************************
        // Sub Classes
        // 
        public enum RenderStyle
        {
            TriangleStrip = 0,
            TriangleList,
            LineStrip,
            LineList
        }
        public enum DrawMode
        {
            Solid = 0,
            Wireframe
        }
        #endregion

        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private D3D.Device
            _dev;
        private int
            _adpNum;
        private IntPtr
            _hWnd;
        private bool
            _disposed;
        private Version
            _minVtxVer = new Version(2, 0),
            _minPxlVer = new Version(2, 0);
        private bool
            _pprmWindowed = true,
            _pprmAutoDepthStencil = true;
        private D3D.DepthFormat
            _pprmDepthStencilFormat = D3D.DepthFormat.D16;
        private D3D.SwapEffect
            _pprmSwapEffect = D3D.SwapEffect.Discard;
        private D3D.Format
            _pprmBkBfrFmt = D3D.Format.R5G6B5;
        private D3D.PresentInterval
            _pprmIntvl = D3D.PresentInterval.Default;
        private D3D.PresentFlag
            _pprmPrsFlg = D3D.PresentFlag.None;
        private D3D.MultiSampleType
            _pprmMsType = D3D.MultiSampleType.None;
        private int
            _pprmMsQual = 0,
            _pprmBkBfrCnt = 1,
            _pprmBkBfrW = 800,
            _pprmBkBfrH = 600,
            _pprmRefreshRt = 60;
        //***************************************************************************
        // Public Events
        // 
        public event EventHandler DeviceReset;
        public event EventHandler DeviceLost;
        public event CancelEventHandler DeviceResizing;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        /// <summary>
        /// Gets a boolean value indicating whether or not this object has been disposed.
        /// </summary>
        public bool Disposed
        { get { return this._disposed; } }
        /// <summary>
        /// Returns the capabilities of the currently initialized Direct3D device.
        /// </summary>
        public D3D.Caps DeviceCaps
        {
            get
            {
                return D3D.Manager.GetDeviceCaps(this._adpNum, (this._dev != null)
                        ? this._dev.DeviceCaps.DeviceType
                        : DxGraphics.GetDeviceType(this._adpNum));
            }
        }
        /// <summary>
        /// Gets the Direct3D DisplayMode for the currently initialized Direct3D Device.
        /// </summary>
        public D3D.DisplayMode DisplayMode
        {
            get
            {
                if (this._dev != null)
                    return this._dev.DisplayMode;
                else
                    throw new Exception("You must initialize the Direct3D graphic device before requesting the display mode.");
            }
        }
        /// <summary>
        /// Gets or sets the minimum pixel shader version for the Direct3D Device to be initialized for hardware rendering.
        /// </summary>
        public Version MinimumPixelShaderVersion
        {
            get { return this._minPxlVer; }
            set { this._minPxlVer = value; }
        }
        /// <summary>
        /// Gets or sets the minimum vertex shader version for the Direct3D Device to be initialized for hardware rendering.
        /// </summary>
        public Version MinimumVertexShaderVersion
        {
            get { return this._minVtxVer; }
            set { this._minVtxVer = value; }
        }
        /// <summary>
        /// Gets or sets a boolean value indicating whether the Direct3D Device will render in a window.
        /// </summary>
        public bool WindowedRendering
        {
            get { return this._pprmWindowed; }
            set { this._pprmWindowed = value; }
        }
        /// <summary>
        /// Gets or sets a boolean value indicating whether the Direct3D Device should use an automatic depth stencil.
        /// </summary>
        public bool AutoDepthStencil
        {
            get { return this._pprmAutoDepthStencil; }
            set { this._pprmAutoDepthStencil = value; }
        }
        public D3D.DepthFormat AutoStencilDepthFormat
        {
            get { return this._pprmDepthStencilFormat; }
            set { this._pprmDepthStencilFormat = value; }
        }
        public D3D.MultiSampleType MultiSampleType
        {
            get { return this._pprmMsType; }
            set { this._pprmMsType = value; }
        }
        public int MultiSampleQuality
        {
            get { return this._pprmMsQual; }
            set
            {
                if (value < 0 || value > 100)
                    throw new ArgumentOutOfRangeException("MutliSampleQuality must be an integer value between 0 and 100.");
                this._pprmMsQual = value;
            }
        }
        public D3D.PresentFlag PresentFlags
        {
            get { return this._pprmPrsFlg; }
            set { this._pprmPrsFlg = value; }
        }
        public D3D.PresentInterval PresentInterval
        {
            get { return this._pprmIntvl; }
            set { this._pprmIntvl = value; }
        }
        public int BackBufferCount
        {
            get { return this._pprmBkBfrCnt; }
            set
            {
                if (value > 4 || value < 0)
                    throw new ArgumentOutOfRangeException("BackBufferCount must be an integer value between 0 and 4.");

                this._pprmBkBfrCnt = value;
            }
        }
        public int BackBufferWidth
        {
            get { return this._pprmBkBfrW; }
            set
            {
                if (value < 1)
                    throw new ArgumentOutOfRangeException("BackBufferWidth must be an integer value greater than zero.");
                this._pprmBkBfrW = value;
            }
        }
        public int BackBufferHeight
        {
            get { return this._pprmBkBfrH; }
            set
            {
                if (value < 1)
                    throw new ArgumentOutOfRangeException("BackBufferHeight must be an integer value greater than zero.");
                this._pprmBkBfrH = value;
            }
        }
        public D3D.Format BackBufferFormat
        {
            get { return this._pprmBkBfrFmt; }
            set { this._pprmBkBfrFmt = value; }
        }
        public int FullScreenRefreshRate
        {
            get { return this._pprmRefreshRt; }
            set
            {
                if (value < 1)
                    throw new ArgumentOutOfRangeException("FullScreenRefreshRate must be an integer value greater than 0.");
                this._pprmRefreshRt = value;
            }
        }
        public D3D.SwapEffect SwapEffect
        {
            get { return this._pprmSwapEffect; }
            set { this._pprmSwapEffect = value; }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        private DxGraphics()
        {
            this._disposed = false;
        }
        public DxGraphics(int adapterNumber, System.Windows.Forms.IWin32Window renderWindow)
            : this(adapterNumber, renderWindow.Handle)
        { }
        public DxGraphics(int adapterNumber, IntPtr windowHandle)
            : this()
        {
            this._adpNum = adapterNumber;
            this._hWnd = windowHandle;
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        public void InitDevice()
        {
            D3D.DeviceType devType = DxGraphics.GetDeviceType(this._adpNum);
            D3D.CreateFlags flags = DxGraphics.GetDeviceCreateFlags(this._adpNum, devType);
            this.InitDevice(devType, flags);
        }
        public void InitDevice(D3D.DeviceType forcedDevice, D3D.CreateFlags forcedFlags)
        {
            if (this._dev != null)
                this.Dispose(true);

            try
            {
                D3D.PresentParameters pprms = new Microsoft.DirectX.Direct3D.PresentParameters();
                pprms.Windowed = this._pprmWindowed;
                if (!this._pprmWindowed)
                {
                    pprms.BackBufferWidth = this._pprmBkBfrW;
                    pprms.BackBufferHeight = this._pprmBkBfrH;
                    pprms.BackBufferFormat = this._pprmBkBfrFmt;
                    pprms.FullScreenRefreshRateInHz = this._pprmRefreshRt;
                }
                pprms.AutoDepthStencilFormat = this._pprmDepthStencilFormat;
                pprms.SwapEffect = this._pprmSwapEffect;
                pprms.PresentationInterval = this._pprmIntvl;
                pprms.PresentFlag = this._pprmPrsFlg;
                pprms.MultiSample = this._pprmMsType;
                pprms.MultiSampleQuality = 0;
                pprms.BackBufferCount = this._pprmBkBfrCnt;

                this._dev = new D3D.Device(this._adpNum, forcedDevice, this._hWnd, forcedFlags, pprms);
                this._dev.DeviceReset += new EventHandler(this.dev_onDeviceReset);
                this._dev.DeviceLost += new EventHandler(this.dev_onDeviceLost);
                this._dev.DeviceResizing += new System.ComponentModel.CancelEventHandler(this.dev_onDeviceResizing);
                this._disposed = false;
            }
            catch
            {
                throw;
            }
        }
        public void BeginScene()
        {
            if (this._dev == null)
                throw new Exception("You must initialize the Direct3D device before calling the BeginScene method.");
            this._dev.BeginScene();
        }
        public void EndScene()
        {
            if (this._dev == null)
                throw new Exception("You must initialize the Direct3D device before calling the EndScene method.");
            this._dev.EndScene();
        }
        //***************************************************************************
        // Static Methods
        // 
        public static D3D.DeviceType GetDeviceType(int adpNum)
        {
            DebugOnly.ConsoleWrite("Determining device type...");
            D3D.DeviceType devType = D3D.DeviceType.Reference;
            D3D.Caps devCaps = D3D.Manager.GetDeviceCaps(adpNum, D3D.DeviceType.Hardware);

            if (devCaps.DeviceCaps.SupportsHardwareRasterization)
                devType = D3D.DeviceType.Hardware;
            else if (devCaps.DeviceType == D3D.DeviceType.Software)
                devType = D3D.DeviceType.Software;

            DebugOnly.ConsoleWrite("Found DeviceType: " + devType.ToString());
            return devType;
        }
        public static D3D.CreateFlags GetDeviceCreateFlags(int adpNum, D3D.DeviceType type)
        {
            DebugOnly.ConsoleWrite("Getting device creation flags...");
            D3D.Caps caps = D3D.Manager.GetDeviceCaps(adpNum, type);
            D3D.CreateFlags flags = D3D.CreateFlags.SoftwareVertexProcessing;
            if (caps.DeviceCaps.SupportsHardwareRasterization && caps.DeviceCaps.SupportsHardwareTransformAndLight)
            {
                flags = D3D.CreateFlags.HardwareVertexProcessing;
                if (caps.DeviceCaps.SupportsPureDevice)
                    flags |= D3D.CreateFlags.PureDevice;
            }
            DebugOnly.ConsoleWrite("Found flags: " + flags.ToString());
            return flags;
        }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this._dev != null)
                    this._dev.Dispose();
                this._disposed = true;
            }
        }
        //***************************************************************************
        // Event Triggers
        // 
        protected virtual void OnDeviceReset(EventArgs e)
        {
            if (this.DeviceReset != null)
                this.DeviceReset.Invoke(this, e);
        }
        protected virtual void OnDeviceLost(EventArgs e)
        {
            if (this.DeviceLost != null)
                this.DeviceLost.Invoke(this, e);
        }
        protected virtual void OnDeviceResizing(CancelEventArgs e)
        {
            if (this.DeviceResizing != null)
                this.DeviceResizing.Invoke(this, e);
        }
        #endregion

        #region Event Handlers
        //***************************************************************************
        // Event Handlers
        // 
        public void dev_onDeviceReset(object sender, EventArgs e)
        {
            this.OnDeviceReset(e);
        }
        public void dev_onDeviceLost(object sender, EventArgs e)
        {
            this.OnDeviceLost(e);
        }
        public void dev_onDeviceResizing(object sender, CancelEventArgs e)
        {
            this.OnDeviceResizing(e);
        }
        #endregion
    }
}
