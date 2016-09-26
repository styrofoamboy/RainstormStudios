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
using System.Text;
using DX = Microsoft.DirectX;
using D3D = Microsoft.DirectX.Direct3D;

namespace RainstormStudios.DirectX
{
    public class MeshObject : IDisposable
    {
        #region Global Objects
        //***************************************************************************
        // Private Fields
        // 
        private D3D.Mesh
            _mesh;
        private D3D.Texture[]
            _textures;
        private D3D.Material[]
            _materials;
        private DX.Vector3
            _position,
            _angle;
        private float
            _bndRadius,
            _scale;
        //private D3D.Device
        //    _owner;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public D3D.Mesh Mesh
        {
            get { return this._mesh; }
            set { this._mesh = value; }
        }
        public D3D.Texture[] Textures
        {
            get { return this._textures; }
        }
        public D3D.Material[] Materials
        {
            get { return this._materials; }
        }
        public DX.Vector3 Position
        {
            get { return this._position; }
            set { this._position = value; }
        }
        public DX.Vector3 Rotation
        {
            get { return this._angle; }
            set { this._angle = value; }
        }
        public float BoundingSphereRadius
        {
            get { return this._bndRadius; }
            set { this._bndRadius = value; }
        }
        public float RenderScale
        {
            get { return this._scale; }
            set { this._scale = value; }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public MeshObject(D3D.Device dev, string filename)
        { this.FillMesh(dev, filename); }
        public MeshObject(D3D.Device dev, Stream stream)
        { this.FillMesh(dev, stream); }
        public MeshObject(D3D.Device dev, D3D.Mesh mesh)
            : this(dev, mesh, new D3D.Material[0], new D3D.Texture[0])
        { }
        public MeshObject(D3D.Device dev, D3D.Mesh mesh, D3D.Material[] materials, D3D.Texture[] textures)
        {
            //this._owner = dev;
            this._mesh = mesh;
            this._materials = materials;
            this._textures = textures;
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        /// <summary>
        /// Releases all resources in use by this object and prepares the object for garbage collection.
        /// </summary>
        public void Dispose()
        {
            this._mesh.Dispose();
            Array.Clear(this._materials, 0, this._materials.Length);
            this._materials = new D3D.Material[0];
            Array.Clear(this._textures, 0, this._textures.Length);
            this._textures = new D3D.Texture[0];
            this._angle = DX.Vector3.Empty;
            this._position = DX.Vector3.Empty;
        }
        /// <summary>
        /// Determines if a given ray intersects this mesh.
        /// </summary>
        /// <param name="rayPos">The ray's point of origin.</param>
        /// <param name="rayDir">The ray's direction of travel.</param>
        /// <returns>A bool value indicating true if the ray passes through the mesh.  Otherwise, false.</returns>
        public bool Intersect(DX.Vector3 rayPos, DX.Vector3 rayDir)
        {
            D3D.IntersectInformation dummy1;
            D3D.IntersectInformation[] dummy2;
            return this.Intersect(rayPos, rayDir, out dummy1, out dummy2);
        }
        public bool Intersect(DX.Vector3 rayPos, DX.Vector3 rayDir, out D3D.IntersectInformation closeHit)
        {
            D3D.IntersectInformation[] dummy;
            return this.Intersect(rayPos, rayDir, out closeHit, out dummy);
        }
        public bool Intersect(DX.Vector3 rayPos, DX.Vector3 rayDir, out D3D.IntersectInformation[] AllHits)
        {
            D3D.IntersectInformation dummy;
            return this.Intersect(rayPos, rayDir, out dummy, out AllHits);
        }
        public bool Intersect(DX.Vector3 rayPos, DX.Vector3 rayDir, out D3D.IntersectInformation closeHit, out D3D.IntersectInformation[] allHits)
        { return this._mesh.Intersect(rayPos, rayDir, out closeHit, out allHits); }
        public void MoveFoward(float speed)
        {
            MeshObject.MoveForward(ref this._position, ref this._angle, speed);
        }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        private void FillMesh(D3D.Device dev, string filename)
        {
            using (FileStream stream = new FileStream(filename, FileMode.Open, FileAccess.Read))
                this.FillMesh(dev, stream);
        }
        private void FillMesh(D3D.Device dev, Stream stream)
        {
        }
        #endregion

        #region Static Methods
        //***************************************************************************
        // Static Methods
        // 
        public static void LoadMesh(string filename, D3D.Device dev, ref D3D.Mesh mesh, ref D3D.Material[] meshMaterials, ref D3D.Texture[] meshTextures)
        {
            using (FileStream stream = new FileStream(filename, FileMode.Open, FileAccess.Read))
                MeshObject.LoadMesh(stream, dev, ref mesh, ref meshMaterials, ref meshTextures);
        }
        public static void LoadMesh(Stream stream, D3D.Device dev, ref D3D.Mesh mesh, ref D3D.Material[] meshMaterials, ref D3D.Texture[] meshTextures)
        {
            D3D.ExtendedMaterial[] materialArray;
            mesh = D3D.Mesh.FromStream(stream, D3D.MeshFlags.Managed, dev, out materialArray);

            if ((materialArray != null) && (materialArray.Length > 0))
            {
                meshMaterials = new D3D.Material[materialArray.Length];
                meshTextures = new D3D.Texture[materialArray.Length];
                for (int i = 0; i < materialArray.Length; i++)
                {
                    meshMaterials[i] = materialArray[i].Material3D;
                    if (!string.IsNullOrEmpty(materialArray[i].TextureFilename))
                        meshTextures[i] = D3D.TextureLoader.FromFile(dev, materialArray[i].TextureFilename);
                }
            }
        }
        public static float GetBoundingRadius(D3D.Mesh mesh)
        {
            return MeshObject.GetBoundingRadius(mesh, 1.0f);
        }
        public static float GetBoundingRadius(D3D.Mesh mesh, float scale)
        {
            D3D.VertexBuffer verts = mesh.VertexBuffer;
            DX.GraphicsStream stream = verts.Lock(0, 0, D3D.LockFlags.None);
            DX.Vector3 meshCenter;
            float radius = D3D.Geometry.ComputeBoundingSphere(stream, mesh.NumberVertices, mesh.VertexFormat, out meshCenter) * scale;
            verts.Unlock();
            stream = null;
            verts = null;
            return radius;
        }
        public static void GetBoundingBox(D3D.Mesh mesh, out DX.Vector3 min, out DX.Vector3 max)
        {
            MeshObject.GetBoundingBox(mesh, 1.0f, out min, out max);
        }
        public static void GetBoundingBox(D3D.Mesh mesh, float scale, out DX.Vector3 min, out DX.Vector3 max)
        {
            D3D.VertexBuffer verts = mesh.VertexBuffer;
            DX.GraphicsStream stream = verts.Lock(0, 0, D3D.LockFlags.None);
            D3D.Geometry.ComputeBoundingBox(stream, mesh.NumberVertices, mesh.VertexFormat, out min, out max);
            verts.Unlock();
            stream = null;
            verts = null;
        }
        public static void MoveForward(ref DX.Vector3 position, ref DX.Vector3 angles, float speed)
        {
            DX.Vector3 v = new DX.Vector3();
            v.X += (float)System.Math.Sin(angles.X);
            v.Y += (float)System.Math.Cos(angles.Z);
            v.Z -= (float)System.Math.Tan(angles.Y);
            v.Normalize();

            position += v * speed;
            v = DX.Vector3.Empty;
        }
        #endregion
    }
}
