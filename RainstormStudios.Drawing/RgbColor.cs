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

namespace RainstormStudios.Drawing
{
    [Author("Unfried, Michael")]
    public class RgbColor
    {
        #region Declarations
        //***************************************************************************
        // Global Variables
        // 
        private int _r, _g, _b, _cycR, _cycG, _cycB, _opac;
        //***************************************************************************
        // Public Fields
        // 
        public System.Drawing.Color Color
        {
            get { return System.Drawing.Color.FromArgb(_opac, _r, _g, _b); }
        }
        public int ArgbColor
        {
            get { return this.Color.ToArgb(); }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        private RgbColor()
        {
            _r = 0;
            _g = 0;
            _b = 0;
            _cycR = 0;
            _cycG = 0;
            _cycB = 0;
            _opac = 0;
        }
        public RgbColor(System.Drawing.Color color)
            : this(color.R, color.G, color.B, color.A)
        { }
        public RgbColor(int red, int green, int blue)
            : this(red, green, blue, 255)
        { }
        public RgbColor(int red, int green, int blue, int alpha)
            : this()
        {
            _r = red;
            _g = green;
            _b = blue;
            _cycR = ((_r > 128) ? -1 : 1);
            _cycG = ((_g > 128) ? -1 : 1);
            _cycB = ((_b > 128) ? -1 : 1);
            _opac = alpha;
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public System.Drawing.Color CycleColor()
        {
            this._r += this._cycR;
            if (this._r >= 255 || this._r <= 0)
                this._cycR *= -1;

            this._g += this._cycG;
            if (this._g >= 255 || this._g <= 0)
                this._cycG *= -1;


            this._b += this._cycB;
            if (this._b >= 255 || this._b <= 0)
                this._cycB *= -1;

            return this.Color;
        }
        #endregion

        #region Static Methods
        //***************************************************************************
        // Static Methods
        // 
        private static Random RandColorGenerator = new Random();
        public static System.Drawing.Color RandomColor()
        {
            return RandomColor(0, 255, 255);
        }
        public static System.Drawing.Color RandomColor(int min)
        {
            return RandomColor(min, 255, 255);
        }
        public static System.Drawing.Color RandomColor(int min, int max)
        {
            return RandomColor(min, max, 255);
        }
        public static System.Drawing.Color RandomColor(int min, int max, int alpha)
        {
            int r = RandColorGenerator.Next(min, max);
            int g = RandColorGenerator.Next(min, max);
            int b = RandColorGenerator.Next(min, max);
            return System.Drawing.Color.FromArgb(r, g, b);
        }
        public static System.Drawing.Color LightenColor(System.Drawing.Color initColor, int lum)
        {
            return System.Drawing.Color.FromArgb(
                initColor.A,
                System.Math.Min(initColor.R + lum, 255),
                System.Math.Min(initColor.G + lum, 255),
                System.Math.Min(initColor.B + lum, 255));
        }
        public static System.Drawing.Color DarkenColor(System.Drawing.Color initColor, int lum)
        {
            return System.Drawing.Color.FromArgb(
                initColor.A,
                System.Math.Max(initColor.R - lum, 0),
                System.Math.Max(initColor.G - lum, 0),
                System.Math.Max(initColor.B - lum, 0));
        }
        #endregion
    }
}
