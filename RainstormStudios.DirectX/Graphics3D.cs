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
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace RainstormStudios.DirectX
{
    public class Graphics3D : IDisposable
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        // Stores a reference to the Direct3D rendering device.
        private Device
            _device;
        // Stores the Direct3D presentation parameters.
        private PresentParameters
            _presentParams;
        private int
            _adpNum;
        // Stores details about the selected adapter's device capabilities.
        private Caps
            _devCaps;
        // Stores the Control object that Direct3D will render to.
        private Control
            _ctrl;
        // Allows the user to store a collection of D3D Texture object for use
        //   with rendering dynamically generated vertex buffers.
        private TextureCollection
            _textures;
        // Allows the user to store a colleciton of D3D Material objects for use
        //   with rendering dynamically generated vertex buffers.
        private MaterialCollection
            _materials;
        // Stores a collection of mesh objects for this device to render.
        private MeshObjectCollection
            _meshes;
        private VertexBufferCollection
            _vertBuffers;
        // This value is stored only in order to reinitialize the graphics device
        //   without making the user re-sprecify the values.
        private DeviceType
            _devType;
        // This value is stored only in order to reinitialize the graphics device
        //   without making the user re-sprecify the values.
        private CreateFlags
            _devFlags;
        private Direct3DCamera
            _camera;
        private float
            _fov,
            _clipNear,
            _clipFar,
            _aspect;
        private Version
            _minVS,
            _minPS;
        //***************************************************************************
        // Public Events
        // 
        public event EventHandler DeviceReset;
        #endregion

        #region Public Properties
        //***************************************************************************
        // Public Properties
        // 
        public Caps DeviceCapabilities
        { get { return this._devCaps; } }
        public Device GraphicsDevice
        { get { return this._device; } }
        public PresentParameters PresentationParameters
        { get { return this._presentParams; } }
        public int AdapterNumber
        { get { return this._adpNum; } }
        public TextureCollection Textures
        { get { return this._textures; } }
        public MaterialCollection Materials
        { get { return this._materials; } }
        public MeshObjectCollection Meshes
        { get { return this._meshes; } }
        public VertexBufferCollection VertexBuffers
        { get { return this._vertBuffers; } }
        public RenderStateManager RenderState
        { get { return this._device.RenderState; } }
        public SamplerStateManagerCollection SamplerState
        { get { return this._device.SamplerState; } }
        public LightsCollection Lights
        { get { return this._device.Lights; } }
        public Direct3DCamera Camera
        { get { return this._camera; } set { this._camera = value; } }
        public float FieldOfView
        { get { return this._fov; } set { this._fov = value; } }
        public float NearClippingPlane
        { get { return this._clipNear; } set { this._clipNear = value; } }
        public float FarClippingPlane
        { get { return this._clipFar; } set { this._clipFar = value; } }
        public float AspectRatio
        { get { return this._aspect; } set { this._aspect = value; } }
        public Matrix ProjectionMatrix
        { get { return Matrix.PerspectiveFovLH(this._fov, this._aspect, this._clipNear, this._clipFar); } }
        public Version MinimumVertexShaderVersion
        { get { return this._minVS; } set { this._minVS = value; } }
        public Version MaximumPixelShaderVersion
        { get { return this._minPS; } set { this._minPS = value; } }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        private Graphics3D()
        {
            this._materials = new MaterialCollection();
            this._textures = new TextureCollection();
            this._meshes = new MeshObjectCollection();
            this._vertBuffers = new VertexBufferCollection();
            this._presentParams = new PresentParameters();
            this._presentParams.Windowed = true;
            this._presentParams.SwapEffect = SwapEffect.Discard;
            this._presentParams.AutoDepthStencilFormat = DepthFormat.D16;
            this._presentParams.EnableAutoDepthStencil = true;
            this._fov = ((float)System.Math.PI / 4.0f);
            this._clipNear = 1.0f;
            this._clipFar = 50.0f;
            this._aspect = 1.333f;
            this._minVS = new Version(0, 0);
            this._minPS = new Version(0, 0);
            this._camera = new Direct3DCamera();
        }
        public Graphics3D(int adpNum, Control window)
            : this()
        {
            this._adpNum = adpNum;
            this._ctrl = window;
            this.GetDeviceCaps(adpNum);
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
        public void InitializeDevice()
        {
            // Set the default device type and capabilities.
            DeviceType devType = DeviceType.Reference;
            CreateFlags devFlags = CreateFlags.SoftwareVertexProcessing;

            // Check the device's real capabilities and adjust the DeviceType
            //   and CreateFlags to match the adpater's features.
            if ((this._devCaps.VertexShaderVersion >= this._minVS) && (this._devCaps.PixelShaderVersion >= this._minPS))
            {
                devType = DeviceType.Hardware;
                if (this._devCaps.DeviceCaps.SupportsHardwareTransformAndLight)
                {
                    devFlags = CreateFlags.HardwareVertexProcessing;
                    if (this._devCaps.DeviceCaps.SupportsPureDevice)
                        devFlags |= CreateFlags.PureDevice;
                }
            }
            this.InitializeDevice(devType, devFlags);
        }
        public void InitializeDevice(DeviceType devType, CreateFlags devFlags)
        {
            this._devType = devType;
            this._devFlags = devFlags;
            this._device = new Device(this._adpNum, devType, this._ctrl, devFlags, this._presentParams);
            this._device.DeviceReset += new EventHandler(this.device_onDeviceReset);
        }
        public void CommitProjectionTransform()
        { this._device.Transform.Projection = this.ProjectionMatrix; }
        public void Clear()
        { this.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black); }
        public void Clear(ClearFlags flags)
        { this.Clear(flags, Color.Black); }
        public void Clear(ClearFlags flags, Color color)
        { this._device.Clear(flags, color, 1.0f, 0); }
        public void AddTexture(string filename, string keyName)
        {
            this._textures.Add(TextureLoader.FromFile(this._device, filename), keyName);
        }
        public void AddTexture(Stream stream, string keyName)
        {
            this._textures.Add(TextureLoader.FromStream(this._device, stream), keyName);
        }
        public void AddMesh(MeshObject value)
        {
            if (value.Mesh.Device == this._device)
                this._meshes.Add(value);
            else
                throw new ArgumentException("Associated mesh device does not match current Direct3D render device.");
        }
        public void AddMesh(MeshObject value, string name)
        {
            if (value.Mesh.Device == this._device)
                this._meshes.Add(value, name);
            else
                throw new ArgumentException("Associated mesh device does not match current Direct3D render device.");
        }
        public void AddMesh(Mesh value, Material[] materials, Texture[] textures)
        { this._meshes.Add(new MeshObject(this._device, value, materials, textures)); }
        public void BeginScene()
        { this._device.BeginScene(); }
        public void EndScene()
        { this._device.EndScene(); this._device.Present(); }
        public void DrawMesh(string meshName)
        { this.DrawMesh(this._meshes.IndexOfKey(meshName)); }
        public void DrawMesh(int index)
        { Graphics3D.DrawMesh(this._meshes[index]); }
        public void DrawVertexBuffer(int index, int material, int texture, PrimitiveType vbType, int primitiveCount)
        {
            this._device.VertexFormat = this._vertBuffers[index].Description.VertexFormat;
            if (material >= 0)
                this._device.Material = this._materials[material];
            if (texture >= 0)
                this._device.SetTexture(0, this._textures[texture]);
            this._device.DrawUserPrimitives(vbType, primitiveCount, this._vertBuffers[index]);
        }
        public void RenderScene()
        {
            this.RenderScene(string.Empty);
        }
        public void RenderScene(string technique)
        {
            // Suspend render while we update the scene.
            this.BeginScene();

            // Iterate through each vertex buffer.
            for (int i = 0; i < this._vertBuffers.Count; i++)
                this.DrawVertexBuffer(i, 0, 0, PrimitiveType.TriangleStrip, 2);

            // Iterate through each mesh.
            for (int i = 0; i < this._meshes.Count; i++)
                this.DrawMesh(i);

            // Render and present.
            this.EndScene();
        }
        public void SetCameraPosition(float x, float y, float z)
        { this._camera.SetPosition(new Vector3(x, y, z)); }
        public void SetCameraLookAt(float x, float y, float z)
        { this._camera.SetLookAt(new Vector3(x, y, z)); }
        public void MoveCamera(float speed)
        { this._camera.MoveForward(speed); }
        public void CommitViewTransform()
        { this._device.Transform.View = this._camera.ViewMatrix; }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        private void GetDeviceCaps(int adpNum)
        {
            this._devCaps = Manager.GetDeviceCaps(adpNum, DeviceType.Hardware);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this._device != null)
                    this._device.Dispose();
                this._camera = Direct3DCamera.Empty;
                this._materials.Clear();
                this._textures.Clear();
                this._fov = float.NaN;
                this._clipNear = float.NaN;
                this._clipFar = float.NaN;
                this._aspect = float.NaN;
                this._adpNum = int.MinValue;
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
        #endregion

        #region Event Handlers
        //***************************************************************************
        // Event Handlers
        // 
        protected void device_onDeviceReset(object sender, EventArgs e)
        {
            this.OnDeviceReset(e);
        }
        #endregion

        #region Static Methods
        //***************************************************************************
        // Static Methods
        // 
        public static void DrawMesh(MeshObject mesh)
        {
            Graphics3D.DrawMesh(mesh.Mesh, mesh.Materials, mesh.Textures,
                Matrix.Translation(mesh.Position.X, mesh.Position.Y, mesh.Position.Z) *
                Matrix.RotationYawPitchRoll(mesh.Rotation.X, mesh.Rotation.Y, mesh.Rotation.Z));
        }
        /// <summary>
        /// Draws the specified mesh using the supplied material and texture Arrays.  Arrays should contain one element for each subset in the mesh (although not every must be filled), or be 'null' for no materials/textures.
        /// </summary>
        /// <param name="mesh">The Mesh to be drawn.</param>
        /// <param name="materials"></param>
        /// <param name="textures"></param>
        /// <param name="transform">The Mmatrix used as the world transform before drawing the mesh.  Pass 'null' for no change.</param>
        public static void DrawMesh(Mesh mesh, Material[] materials, Texture[] textures, Matrix transform)
        {
            if (transform != null)
                mesh.Device.Transform.World = transform;
            if (materials != null && materials.Length > 0)
            {
                for (int i = 0; i < materials.Length; i++)
                    if (textures.Length > 3)
                    {
                        mesh.Device.Material = materials[i];
                        mesh.Device.SetTexture(0, textures[i]);
                        mesh.DrawSubset(i);
                    }
            }
            else
            {
                int numSubsets = mesh.GetAttributeTable().Length;
                for (int i = 0; i < numSubsets; i++)
                    mesh.DrawSubset(i);
            }
        }
        #endregion
    }
}
