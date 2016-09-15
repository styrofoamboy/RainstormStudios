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
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace RainstormStudios.DirectX
{
    [Author("Unfried, Michael")]
    public struct Direct3DCamera
    {
        #region Global Objects
        //***************************************************************************
        // Public Fields
        // 
        public static readonly Direct3DCamera
            Empty;
        public Vector3
            Position,
            LookAt,
            UpVector;
        #endregion

        #region Public Properties
        //***************************************************************************
        // Public Properties
        // 
        public Matrix ViewMatrix
        { get { return Matrix.LookAtLH(this.Position, this.LookAt, this.UpVector); } }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public Direct3DCamera(Vector3 pos, Vector3 lookat)
            : this(pos, lookat, new Vector3(0.0f, 0.0f, 1.0f))
        { }
        public Direct3DCamera(Vector3 pos, Vector3 lookat, Vector3 up)
        {
            this.Position = pos;
            this.LookAt = lookat;
            this.UpVector = up;
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public void MoveForward(double speed)
        {

        }
        public void MoveLeft(double speed)
        {

        }
        public void MoveRight(double speed)
        {

        }
        public void Translate(float x, float y, float z)
        {
            this.Position.TransformCoordinate(Matrix.Translation(x, y, z));
        }
        public void Rotate(float x, float y, float z)
        {
            this.LookAt.TransformCoordinate(Matrix.RotationYawPitchRoll(x, y, z));
        }
        #endregion
    }
}
