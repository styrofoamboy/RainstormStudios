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
using System.Drawing.Imaging;

namespace RainstormStudios.Drawing
{
    [Author("Unfried, Michael")]
    public class YUV
    {
        #region Declarations
        //***************************************************************************
        // Public Fields
        // 
        public byte
            Y, U, V;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public Color RGB
        {
            get
            {
                int c = this.Y - 16;
                int d = this.U - 128;
                int e = this.V - 128;

                byte r, g, b;
                r = (byte)System.Math.Max(System.Math.Min(((298 * c + 409 * e + 128) >> 8), 255), 0);
                g = (byte)System.Math.Max(System.Math.Min(((298 * c - 100 * d - 208 * e + 128) >> 8), 255), 0);
                b = (byte)System.Math.Max(System.Math.Min(((298 * c + 516 * d + 128) >> 8), 255), 0);

                return Color.FromArgb(r, g, b);
            }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public YUV(byte Y, byte U, byte V)
        {
            this.Y = Y;
            this.U = U;
            this.V = V;
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Static Methods
        // 
        public static YUV FromRGB(Color c)
        {
            byte y, u, v;

            y = (byte)(((66 * c.R + 129 * c.G + 25 * c.B + 128) >> 8) + 16);
            u = (byte)(((-38 * c.R - 74 * c.G + 112 * c.B + 128) >> 8) + 128);
            v = (byte)(((112 * c.R - 94 * c.G - 18 * c.B + 128) >> 8) + 128);

            return new YUV(y, u, v);
        }
        #endregion
    }
    [Author("Unfried, Michael")]
    public class HSL
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private float
            _h, _s, _l;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public float Hue
        {
            get { return this._h; }
            set { this._h = (float)(System.Math.Abs(value) % 360); }
        }
        public float Saturation
        {
            get { return this._s; }
            set { this._s = (float)System.Math.Max(System.Math.Min(1.0, value), 0.0); }
        }
        public float Luminance
        {
            get { return this._l; }
            set { this._l = (float)System.Math.Max(System.Math.Min(1.0, value), 0.0); }
        }
        public Color RGB
        {
            get
            {
                double
                    r = 0, g = 0, b = 0,
                temp1, temp2,
                normalisedH = this._h / 360.0;

                if (this._l == 0)
                    r = g = b = 0;
                else
                {
                    if (this._s == 0)
                        r = g = b = this._l;
                    else
                    {
                        temp2 = ((_l <= 0.5) ? _l * (1.0 + _s) : _l + _s - (_l * _s));
                        temp1 = 2.0 * _l - temp2;
                        double[] t3 = new double[] { normalisedH + 1.0 / 3.0, normalisedH, normalisedH - 1.0 / 3.0 };
                        double[] clr = new double[] { 0, 0, 0 };
                        for (int i = 0; i < 3; i++)
                        {
                            if (t3[i] < 0)
                                t3[i] += 1.0;
                            else if (t3[i] > 1)
                                t3[i] -= 1.0;

                            if (6.0 * t3[i] < 1.0)
                                clr[i] = temp1 + (temp2 - temp1) * t3[i] * 6.0;
                            else if (2.0 * t3[i] < 1.0)
                                clr[i] = temp2;
                            else if (3.0 * t3[3] < 2.0)
                                clr[i] = (temp1 + (temp2 - temp1) * ((2.0 / 3.0) - t3[i]) * 6.0);
                            else
                                clr[i] = temp1;
                        }
                        r = clr[0];
                        g = clr[1];
                        b = clr[2];
                    }
                }
                return Color.FromArgb((int)(255 * r), (int)(255 * g), (int)(255 * b));
            }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        private HSL()
        { }
        public HSL(float hue, float saturation, float luminance)
        {
            this.Hue = hue;
            this.Saturation = saturation;
            this.Luminance = luminance;
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Static Methods
        // 
        public static HSL FromRGB(byte red, byte green, byte blue)
        { return HSL.FromRGB(Color.FromArgb(red, green, blue)); }
        public static HSL FromRGB(Color c)
        { return new HSL(c.GetHue(), c.GetSaturation(), c.GetBrightness()); }
        #endregion
    }
    [Author("Unfried, Michael")]
    public enum HSLFilterType
    {
        Hue,
        Saturation,
        Luminance
    }
    [Author("Unfried, Michael")]
    public sealed class ColorSpace
    {
        #region Public Methods
        //***************************************************************************
        // Static Methods
        // 
        public static Bitmap Luminance(Bitmap source, float factor)
        { return ColorSpace.ImgFilter(source, factor, HSLFilterType.Luminance); }
        public static Bitmap Hue(Bitmap source, float factor)
        { return ColorSpace.ImgFilter(source, factor, HSLFilterType.Hue); }
        public static Bitmap Saturation(Bitmap source, float factor)
        { return ColorSpace.ImgFilter(source, factor, HSLFilterType.Saturation); }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Static Methods
        // 
        private static Bitmap ImgFilter(Bitmap source, float factor, HSLFilterType type)
        {
            int
                width = source.Width,
                height = source.Height;

            Rectangle rc = new Rectangle(0, 0, width, height);

            if (source.PixelFormat != PixelFormat.Format24bppRgb)
                source = source.Clone(rc, PixelFormat.Format24bppRgb);

            Bitmap dest = new Bitmap(width, height, source.PixelFormat);

            BitmapData dataSrc = source.LockBits(rc, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            BitmapData dataDest = dest.LockBits(rc, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            int offset = dataSrc.Stride - (width * 3);

            unsafe
            {
                byte* bytesSrc = (byte*)(void*)dataSrc.Scan0;
                byte* bytesDest = (byte*)(void*)dataDest.Scan0;
                for (int y = 0; y < height; ++y)
                {
                    for (int x = 0; x < width; ++x)
                    {
                        HSL hsl = HSL.FromRGB(bytesSrc[2], bytesSrc[1], bytesSrc[0]);   // Still BGR (probably due to little-endian byte format.
                        switch (type)
                        {
                            case HSLFilterType.Luminance:
                                hsl.Luminance *= factor;
                                break;
                            case HSLFilterType.Hue:
                                hsl.Hue *= factor;
                                break;
                            case HSLFilterType.Saturation:
                                hsl.Saturation *= factor;
                                break;
                        }

                        Color c = hsl.RGB;

                        bytesDest[0] = c.B;
                        bytesDest[1] = c.G;
                        bytesDest[2] = c.R;

                        bytesSrc += 3;
                        bytesDest += 3;
                    }
                    bytesDest += offset;
                    bytesSrc += offset;
                }
                source.UnlockBits(dataSrc);
                dest.UnlockBits(dataDest);
            }
            return dest;

        }
        #endregion
    }
}
