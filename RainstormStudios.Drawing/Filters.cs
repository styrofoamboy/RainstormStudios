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
    public class ConvMatrix
    {
        #region Declarations
        //***************************************************************************
        // Public Fields
        // 
        public int TopLeft = 0, TopMid = 0, TopRight = 0;
        public int MidLeft = 0, Pixel = 1, MidRight = 0;
        public int BottomLeft = 0, BottomMid = 0, BottomRight = 0;
        public int Factor = 1;
        public int Offset = 0;
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public ConvMatrix()
        { }
        public ConvMatrix(int nval)
        { this.SetAll(nval); }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public void SetAll(int nval)
        {
            TopLeft = TopMid = TopRight = MidLeft = Pixel = MidRight = BottomLeft = BottomMid = BottomRight = nval;
        }
        #endregion
    }
    public class GausianKernel
    {
        #region Declarations
        //***************************************************************************
        // Private Methods
        // 
        private int
            _size;
        private float
            _weight;
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public GausianKernel(int size, float weight)
        {
            this._size = size;
            this._weight = weight;
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public double[,] Calc()
        {
            double[,] k = new double[this._size, this._size];
            double sumTtl = 0;

            int radius = this._size / 2;
            double d = 0;

            double euler = 1.0 / (2.0 * System.Math.PI * System.Math.Pow(this._weight, 2));
            for (int y = -radius; y <= radius; y++)
                for (int x = -radius; x <= radius; x++)
                {
                    d = ((x * x) + (y * y)) / (2 * (this._weight * this._weight));
                    k[y + radius, x + radius] = euler * System.Math.Exp(-d);
                    sumTtl += k[y + radius, x + radius];
                }

            for (int y = 0; y <= this._size; y++)
                for (int x = 0; x < this._size; x++)
                    k[y, x] = k[y, x] * (1.0 / sumTtl);

            return k;
        }
        #endregion
    }
    public enum EdgeDetectionMethod : short
    {
        Kirsh = 1,
        Prewitt = 2,
        Sobel = 3
    }
    public enum BitmapFilterType
    {
        Invert,
        GrayScale,
        Brightness,
        Contrast,
        Gamma,
        Color
    }
    public class BitmapFilter
    {
        #region Public Methods
        //***************************************************************************
        // Static Methods
        // 
        public static Bitmap Invert(Bitmap b)
        { return BitmapFilter.ProcessBitmap(b, BitmapFilterType.Invert); }
        public static Bitmap GrayScale(Bitmap b)
        { return BitmapFilter.ProcessBitmap(b, BitmapFilterType.GrayScale); }
        public static Bitmap Brightness(Bitmap b, int nBrightness)
        { return BitmapFilter.ProcessBitmap(b, BitmapFilterType.Brightness, nBrightness); }
        public static Bitmap Contrast(Bitmap b, sbyte nContrast)
        {
            if (nContrast < -100 || nContrast > 100)
                throw new ArgumentException("Contrast must be a value between -100 and 100", "nContrast");

            return BitmapFilter.ProcessBitmap(b, BitmapFilterType.Contrast, nContrast);
        }
        public static Bitmap Gamma(Bitmap b, double red, double green, double blue)
        {
            if (red < 0.2 || red > 5)
                throw new ArgumentException("Gamma values must a double precision value between 0.2 and 5.", "red");
            else if (green < .2 || green > 5)
                throw new ArgumentException("Gamma values must a double precision value between 0.2 and 5.", "green");
            else if (blue < .2 || blue > 5)
                throw new ArgumentException("Gamma values must a double precision value between 0.2 and 5.", "blue");

            return BitmapFilter.ProcessBitmap(b, BitmapFilterType.Gamma, red, green, blue);
        }
        public static Bitmap Color(Bitmap b, int red, int green, int blue)
        {
            if (red < -255 || red > 255)
                throw new ArgumentException("Color must be an integer value between -255 and 255", "red");
            else if (green < -255 || green > 255)
                throw new ArgumentException("Color must be an integer value between -255 and 255", "green");
            else if (blue < -255 || blue > 255)
                throw new ArgumentException("Color must be an integer value between -255 and 255", "blue");

            return BitmapFilter.ProcessBitmap(b, BitmapFilterType.Color, red, green, blue);
        }
        public static Bitmap Conv3x3(Bitmap b, ConvMatrix m)
        {
            // Avoid divide by zero errors.
            if (m.Factor == 0)
                throw new ArgumentException("Conversion matrix factor must be greater than zero.", "m");

            Bitmap bmpDest = new Bitmap(b.Width, b.Height);
            // GDI+ lies - the return format is BGR, not RGB (stupid little-endian)
            BitmapData bitData = bmpDest.LockBits(new Rectangle(0, 0, bmpDest.Width, bmpDest.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            BitmapData bitSrc = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

            try
            {
                int stride = bitSrc.Stride;
                int stride2 = bitSrc.Stride * 2;

                unsafe
                {
                    for (int y = 0; y < b.Height - 2; y++)
                    {
                        byte* ptrSrc = (byte*)bitSrc.Scan0 + y * bitSrc.Stride;
                        byte* ptrDest = (byte*)bitSrc.Scan0 + y * bitData.Stride;

                        for (int x = 0; x < b.Width - 2; x++)
                        {
                            // Set R channel (remember, bytes are BGR, not RGB, thanks to little endian byte order).
                            int nPixelR = (((ptrSrc[x * 3 + 2] * m.TopLeft) + (ptrSrc[x * 3 + 5] * m.TopMid) + (ptrSrc[x * 3 + 8] * m.TopRight) +
                                        (ptrSrc[x * 3 + 2 + stride] * m.MidLeft) + (ptrSrc[x * 3 + 5 + stride] * m.Pixel) + (ptrSrc[x * 3 + 8 + stride] * m.MidRight) +
                                        (ptrSrc[x * 3 + 2 + stride2] * m.BottomLeft) + (ptrSrc[x * 3 + 5 + stride2] * m.BottomMid) + (ptrSrc[x * 3 + 8 + stride2] * m.BottomRight)) / m.Factor) + m.Offset;
                            nPixelR = (int)System.Math.Max(0, System.Math.Min(255, nPixelR));
                            ptrDest[5 + stride] = (byte)nPixelR;

                            // Set G channel.
                            int nPixelG = (((ptrSrc[x * 3 + 1] * m.TopLeft) + (ptrSrc[x * 3 + 4] * m.TopMid) + (ptrSrc[x * 3 + 7] * m.TopRight) +
                                        (ptrSrc[x * 3 + 1 + stride] * m.MidLeft) + (ptrSrc[x * 3 + 4 + stride] * m.Pixel) + (ptrSrc[x * 3 + 7 + stride] * m.MidRight) +
                                        (ptrSrc[x * 3 + 1 + stride2] * m.BottomLeft) + (ptrSrc[x * 3 + 4 + stride2] * m.BottomMid) + (ptrSrc[x * 3 + 7 + stride2] * m.BottomRight)) / m.Factor) + m.Offset;
                            nPixelG = (int)System.Math.Max(0, System.Math.Min(255, nPixelG));
                            ptrDest[4 + stride] = (byte)nPixelG;

                            // Set B channel.
                            int nPixelB = (((ptrSrc[x * 3 + 0] * m.TopLeft) + (ptrSrc[x * 3 + 3] * m.TopMid) + (ptrSrc[x * 3 + 6] * m.TopRight) +
                                        (ptrSrc[x * 3 + 0 + stride] * m.MidLeft) + (ptrSrc[x * 3 + 3 + stride] * m.Pixel) + (ptrSrc[x * 3 + 6 + stride] * m.MidRight) +
                                        (ptrSrc[x * 3 + 0 + stride2] * m.BottomLeft) + (ptrSrc[x * 3 + 3 + stride2] * m.BottomMid) + (ptrSrc[x * 3 + 6 + stride2] * m.BottomRight)) / m.Factor) + m.Offset;
                            nPixelB = (int)System.Math.Max(0, System.Math.Min(255, nPixelB));
                            ptrDest[3 + stride] = (byte)nPixelB;
                        }
                    }
                }
            }
            catch
            {
                if (bmpDest != null)
                    bmpDest.Dispose();
                throw;
            }
            finally
            {
                bmpDest.UnlockBits(bitData);
                b.UnlockBits(bitSrc);
            }
            return bmpDest;
        }
        //public static Bitmap Conv3x3(Bitmap b, ConvMatrix m)
        //{
        //    // Avoid divide by zero errors.
        //    if (m.Factor == 0)
        //        throw new ArgumentException("Conversion matrix factor must be greater than zero.", "m");

        //    using (Bitmap bSrc = (Bitmap)b.Clone())
        //    {
        //        // GDI+ lies - the return format is BGR, not RGB (stupid little-endian)
        //        BitmapData bmpData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
        //        BitmapData bmpSrc = bSrc.LockBits(new Rectangle(0, 0, bSrc.Width, bSrc.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

        //        try
        //        {
        //            int stride = bmpData.Stride;
        //            int stride2 = stride * 2;

        //            unsafe
        //            {
        //                byte* p = (byte*)(void*)bmpData.Scan0;
        //                byte* pSrc = (byte*)(void*)bmpSrc.Scan0;

        //                int nOffset = stride - b.Width * 3;
        //                int nWidth = b.Width - 2;
        //                int nHeight = b.Height - 2;

        //                int nPixel;

        //                for (int y = 0; y < nHeight; ++y)
        //                {
        //                    for (int x = 0; x < nWidth; ++x)
        //                    {
        //                        nPixel = ((((pSrc[2] * m.TopLeft) + (pSrc[5] * m.TopMid) + (pSrc[8] * m.TopRight) +
        //                                (pSrc[2 + stride] * m.MidLeft) + (pSrc[5 + stride] * m.Pixel) + (pSrc[8 + stride] * m.MidRight) +
        //                                (pSrc[2 + stride2] * m.BottomLeft) + (pSrc[5 + stride2] * m.BottomMid) + (pSrc[8 + stride2] * m.BottomRight)) / m.Factor) + m.Offset);

        //                        nPixel = (int)System.Math.Min(0, System.Math.Max(nPixel, 255));
        //                        p[5 + stride] = (byte)nPixel;

        //                        nPixel = ((((pSrc[1] * m.TopLeft) + (pSrc[4] * m.TopMid) + (pSrc[7] * m.TopRight) +
        //                                (pSrc[1 + stride] * m.MidLeft) + (pSrc[4 + stride] * m.Pixel) + (pSrc[7 + stride] * m.MidRight) +
        //                                (pSrc[1 + stride2] * m.BottomLeft) + (pSrc[4 + stride2] * m.BottomMid) + (pSrc[7 + stride2] * m.BottomRight)) / m.Factor) + m.Offset);

        //                        nPixel = (int)System.Math.Min(0, System.Math.Max(nPixel, 255));
        //                        p[4 + stride] = (byte)nPixel;

        //                        nPixel = ((((pSrc[0] * m.TopLeft) + (pSrc[3] * m.TopMid) + (pSrc[6] + m.TopRight) +
        //                                (pSrc[0 + stride] * m.MidLeft) + (pSrc[3 + stride] * m.Pixel) + (pSrc[6 + stride] * m.MidRight) +
        //                                (pSrc[0 + stride2] * m.BottomLeft) + (pSrc[3 + stride2] * m.BottomMid) + (pSrc[6 + stride2] * m.BottomRight)) / m.Factor) + m.Offset);

        //                        nPixel = (int)System.Math.Min(0, System.Math.Max(nPixel, 255));
        //                        p[3 + stride] = (byte)nPixel;

        //                        p += 3;
        //                        pSrc += 3;
        //                    }
        //                    p += nOffset;
        //                    pSrc += nOffset;
        //                }
        //            }
        //        }
        //        catch
        //        { throw; }
        //        finally
        //        {
        //            b.UnlockBits(bmpData);
        //            bSrc.UnlockBits(bmpSrc);
        //        }
        //    }
        //    return b;
        //}
        public static Bitmap Smooth(Bitmap b)
        { return BitmapFilter.Smooth(b, 1); }
        public static Bitmap Smooth(Bitmap b, int nWeight)
        {
            ConvMatrix m = new ConvMatrix(1);
            m.Pixel = nWeight;
            m.Factor = nWeight + 8;
            return BitmapFilter.Conv3x3(b, m);
        }
        public static Bitmap GausianBlur(Bitmap b)
        { return BitmapFilter.GausianBlur(b, 3, 5.5f); }
        public static Bitmap GausianBlur(Bitmap b, int size, float weight)
        {
            //ConvMatrix m = new ConvMatrix(1);
            //m.Pixel = nWeight;
            //m.TopMid = m.MidLeft = m.MidRight = m.BottomMid = 2;
            //m.Factor = nWeight + 12;
            //return BitmapFilter.Conv3x3(b, m);
            GausianKernel kernel = new GausianKernel(size, weight);
            return BitmapFilter.AdvConvFilter(b, kernel.Calc(), size, 0);
        }
        public static Bitmap MeanRemoval(Bitmap b)
        { return BitmapFilter.MeanRemoval(b, 9); }
        public static Bitmap MeanRemoval(Bitmap b, int nWeight)
        {
            ConvMatrix m = new ConvMatrix(-1);
            m.Pixel = nWeight;
            m.Factor = nWeight - 8;
            return BitmapFilter.Conv3x3(b, m);
        }
        public static Bitmap Sharpen(Bitmap b)
        { return BitmapFilter.Sharpen(b, 11); }
        public static Bitmap Sharpen(Bitmap b, int nWeight)
        {
            ConvMatrix m = new ConvMatrix(0);
            m.Pixel = nWeight;
            m.TopMid = m.MidLeft = m.MidRight = m.BottomMid = -2;
            m.Factor = nWeight - 8;
            return BitmapFilter.Conv3x3(b, m);
        }
        public static Bitmap EmbossLaplacian(Bitmap b)
        {
            ConvMatrix m = new ConvMatrix(-1);
            m.TopMid = m.MidLeft = m.MidRight = m.BottomMid = 0;
            m.Pixel = 4;
            m.Offset = 127;
            return BitmapFilter.Conv3x3(b, m);
        }
        public static Bitmap EdgeDetectQuick(Bitmap b)
        {
            ConvMatrix m = new ConvMatrix();
            m.TopLeft = m.TopMid = m.TopRight = -1;
            m.MidLeft = m.Pixel = m.MidRight = 0;
            m.BottomLeft = m.BottomMid = m.BottomRight = 1;
            m.Offset = 127;
            return BitmapFilter.Conv3x3(b, m);
        }
        public static Bitmap EdgeDetectConvolution(Bitmap b, EdgeDetectionMethod nType, byte nThreshold)
        {
            ConvMatrix m = new ConvMatrix();

            // First we need to keep a copy of the original Bitmap.
            using (Bitmap bTemp = (Bitmap)b.Clone())
            {
                try
                {
                    switch (nType)
                    {
                        case EdgeDetectionMethod.Sobel:
                            m.SetAll(0);
                            m.TopLeft = m.BottomLeft = 1;
                            m.TopRight = m.BottomRight = -1;
                            m.MidLeft = 2;
                            m.MidRight = -2;
                            m.Offset = 0;
                            break;
                        case EdgeDetectionMethod.Prewitt:
                            m.SetAll(0);
                            m.TopLeft = m.MidLeft = m.BottomLeft = -1;
                            m.TopRight = m.MidRight = m.BottomRight = 1;
                            m.Offset = 0;
                            break;
                        case EdgeDetectionMethod.Kirsh:
                            m.SetAll(-3);
                            m.Pixel = 0;
                            m.TopLeft = m.MidLeft = m.BottomLeft = 5;
                            m.Offset = 0;
                            break;
                    }
                    BitmapFilter.Conv3x3(b, m);

                    switch (nType)
                    {
                        case EdgeDetectionMethod.Sobel:
                            m.SetAll(0);
                            m.TopLeft = m.TopRight = 1;
                            m.BottomLeft = m.BottomRight = -1;
                            m.TopMid = 2;
                            m.BottomMid = -2;
                            m.Offset = 0;
                            break;
                        case EdgeDetectionMethod.Prewitt:
                            m.SetAll(0);
                            m.BottomLeft = m.BottomMid = m.BottomRight = -1;
                            m.TopLeft = m.TopMid = m.TopRight = 1;
                            m.Offset = 0;
                            break;
                        case EdgeDetectionMethod.Kirsh:
                            m.SetAll(-3);
                            m.Pixel = 0;
                            m.BottomLeft = m.BottomMid = m.BottomRight = 5;
                            m.Offset = 0;
                            break;
                    }
                    BitmapFilter.Conv3x3(bTemp, m);
                }
                catch
                { throw; }

                // GDI+ lies - return format is BGR, not RGB.
                BitmapData bmpData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                BitmapData bmpData2 = bTemp.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

                try
                {
                    int stride = bmpData.Stride;

                    unsafe
                    {
                        byte* p = (byte*)(void*)bmpData.Scan0;
                        byte* p2 = (byte*)(void*)bmpData2.Scan0;

                        int nOffset = stride - b.Width * 3;
                        int nWidth = b.Width * 3;

                        int nPixel = 0;

                        for (int y = 0; y < b.Height; ++y)
                        {
                            for (int x = 0; x < nWidth; ++x)
                            {
                                nPixel = (int)System.Math.Sqrt((p[0] * p[0]) + (p2[0] * p2[0]));
                                if (nPixel < nThreshold) nPixel = nThreshold;
                                if (nPixel > 255) nPixel = 255;
                                p[0] = (byte)nPixel;
                                ++p;
                                ++p2;
                            }
                            p += nOffset;
                            p2 += nOffset;
                        }
                    }
                }
                catch
                { throw; }
                finally
                {
                    b.UnlockBits(bmpData);
                    bTemp.UnlockBits(bmpData2);
                }
            }
            return b;
        }
        public static Bitmap EdgeDetectHorizontal(Bitmap b)
        {
            using (Bitmap bTemp = (Bitmap)b.Clone())
            {
                BitmapData bmpData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                BitmapData bmpData2 = bTemp.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

                try
                {
                    int stride = bmpData.Stride;

                    unsafe
                    {
                        byte* p = (byte*)(void*)bmpData.Scan0;
                        byte* p2 = (byte*)(void*)bmpData2.Scan0;

                        int nOffset = stride - b.Width * 3;
                        int nWidth = b.Width * 3;
                        int nPixel = 0;

                        p += stride;
                        p2 += stride;

                        for (int y = 1; y < b.Height; ++y)
                        {
                            p += 9;
                            p2 += 9;

                            for (int x = 9; x < nWidth - 9; ++x)
                            {
                                nPixel = ((p2 + stride - 9)[0] +
                                        (p2 + stride - 6)[0] +
                                        (p2 + stride - 3)[0] +
                                        (p2 + stride)[0] +
                                        (p2 + stride + 3)[0] +
                                        (p2 + stride + 6)[0] +
                                        (p2 + stride + 9)[0] +
                                        (p2 - stride - 9)[0] +
                                        (p2 - stride - 6)[0] +
                                        (p2 - stride - 3)[0] +
                                        (p2 - stride)[0] +
                                        (p2 - stride + 3)[0] +
                                        (p2 - stride + 6)[0] +
                                        (p2 - stride + 9)[0]);

                                nPixel = (int)System.Math.Min(0, System.Math.Max(nPixel, 255));
                                (p + stride)[0] = (byte)nPixel;

                                ++p;
                                ++p2;
                            }
                            p += 9 + nOffset;
                            p2 += 9 + nOffset;
                        }
                    }
                }
                catch
                { throw; }
                finally
                {
                    b.UnlockBits(bmpData);
                    bTemp.UnlockBits(bmpData2);
                }
            }
            return b;
        }
        public static Bitmap EdgeDetectVertical(Bitmap b)
        {
            using (Bitmap bTemp = (Bitmap)b.Clone())
            {
                BitmapData bmpData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                BitmapData bmpData2 = bTemp.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

                try
                {
                    int stride = bmpData.Stride;

                    unsafe
                    {
                        byte* p = (byte*)(void*)bmpData.Scan0;
                        byte* p2 = (byte*)(void*)bmpData2.Scan0;

                        int nOffset = stride - b.Width * 3;
                        int nWidth = b.Width * 3;
                        int nPixel = 0;
                        int nStride2 = stride * 2;
                        int nStride3 = stride * 3;

                        for (int y = 3; y < b.Height - 3; ++y)
                        {
                            p += 3;
                            p2 += 3;
                            for (int x = 3; x < nWidth - 3; ++x)
                            {
                                nPixel = ((p2 + nStride3 + 3)[0] +
                                        (p2 + nStride2 + 3)[0] +
                                        (p2 + 3)[0] +
                                        (p2 - stride + 3)[0] +
                                        (p2 - nStride2 + 3)[0] +
                                        (p2 - nStride3 + 3)[0] +
                                        (p2 + nStride3 - 3)[0] +
                                        (p2 + nStride2 - 3)[0] +
                                        (p2 + stride - 3)[0] +
                                        (p2 - 3)[0] +
                                        (p2 - stride - 3)[0] +
                                        (p2 - nStride2 - 3)[0] +
                                        (p2 - nStride3 - 3)[0]);

                                nPixel = (int)System.Math.Min(0, System.Math.Max(nPixel, 255));
                                p[0] = (byte)nPixel;

                                ++p;
                                ++p2;
                            }
                            p += 3 + nOffset;
                            p2 += 3 + nOffset;
                        }
                    }
                }
                catch
                { throw; }
                finally
                {
                    b.UnlockBits(bmpData);
                    bTemp.UnlockBits(bmpData2);
                }
            }
            return b;
        }
        public static Bitmap EdgeDetectHomogenity(Bitmap b, byte nThreshold)
        {
            using (Bitmap b2 = (Bitmap)b.Clone())
            {
                BitmapData bmpData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                BitmapData bmpData2 = b2.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

                try
                {
                    int stride = bmpData.Stride;

                    unsafe
                    {
                        byte* p = (byte*)(void*)bmpData.Scan0;
                        byte* p2 = (byte*)(void*)bmpData2.Scan0;

                        int nOffset = stride - b.Width * 3;
                        int nWidth = b.Width * 3;
                        int nPixel = 0, nPixelMax = 0;

                        p += stride;
                        p2 += stride;

                        for (int y = 1; y < b.Height; ++y)
                        {
                            p += 3;
                            p2 += 3;
                            for (int x = 3; x < nWidth - 3; ++x)
                            {
                                nPixelMax = System.Math.Abs(p2[0] - (p2 + stride - 3)[0]);
                                nPixel = System.Math.Abs(p2[0] - (p2 + stride)[0]);
                                nPixelMax = System.Math.Max(nPixel, nPixelMax);

                                nPixel = System.Math.Abs(p2[0] - (p2 + stride + 3)[0]);
                                nPixelMax = System.Math.Max(nPixel, nPixelMax);

                                nPixel = System.Math.Abs(p2[0] - (p2 - stride)[0]);
                                nPixelMax = System.Math.Max(nPixel, nPixelMax);

                                nPixel = System.Math.Abs(p2[0] - (p2 + stride)[0]);
                                nPixelMax = System.Math.Max(nPixel, nPixelMax);

                                nPixel = System.Math.Abs(p2[0] - (p2 - stride - 3)[0]);
                                nPixelMax = System.Math.Max(nPixel, nPixelMax);

                                nPixel = System.Math.Abs(p2[0] - (p2 - stride)[0]);
                                nPixelMax = System.Math.Max(nPixel, nPixelMax);

                                nPixel = System.Math.Abs(p2[0] - (p2 - stride + 3)[0]);
                                nPixelMax = System.Math.Max(nPixel, nPixelMax);

                                nPixelMax = System.Math.Max((int)nThreshold, 0);
                                p[0] = (byte)nPixelMax;
                                ++p;
                                ++p2;
                            }
                            p += 3 + nOffset;
                            p2 += 3 + nOffset;
                        }
                    }
                }
                catch
                { throw; }
                finally
                {
                    b.UnlockBits(bmpData);
                    b2.UnlockBits(bmpData2);
                }
            }
            return b;
        }

        public static Bitmap OffsetFilterAbs(Bitmap b, Point[,] offset)
        {
            using (Bitmap bSrc = (Bitmap)b.Clone())
            {
                BitmapData bmpData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                BitmapData bmpSrc = bSrc.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

                try
                {
                    int stride = bmpData.Stride;

                    unsafe
                    {
                        byte* p = (byte*)(void*)bmpData.Scan0;
                        byte* p2 = (byte*)(void*)bmpSrc.Scan0;

                        int nOffset = bmpData.Stride - b.Width * 3;
                        int nWidth = b.Width;
                        int nHeight = b.Height;
                        int xOffset, yOffset;

                        for (int y = 0; y < nHeight; ++y)
                        {
                            for (int x = 0; x < nWidth; ++x)
                            {
                                xOffset = offset[x, y].X;
                                yOffset = offset[x, y].Y;
                                if (yOffset >= 0 && yOffset < nHeight && xOffset >= 0 && xOffset < nWidth)
                                {
                                    p[0] = p2[(yOffset * stride) + (xOffset * 3)];
                                    p[1] = p2[(yOffset * stride) + (xOffset * 3) + 1];
                                    p[2] = p2[(yOffset * stride) + (xOffset * 3) + 2];
                                }
                                p += 3;
                            }
                            p += nOffset;
                        }
                    }
                }
                catch
                { throw; }
                finally
                {
                    b.UnlockBits(bmpData);
                    bSrc.UnlockBits(bmpSrc);
                }
            }
            return b;
        }

        public static Bitmap Flip(Bitmap b, bool bHorz, bool bVert)
        {
            try
            {
                Point[,] ptFlip = new Point[b.Width, b.Height];

                int nWidth = b.Width;
                int nHeight = b.Height;

                for (int x = 0; x < nWidth; ++x)
                    for (int y = 0; y < nHeight; ++y)
                    {
                        ptFlip[x, y].X = (bHorz) ? nWidth - (x + 1) : x;
                        ptFlip[x, y].Y = (bVert) ? nHeight - (y + 1) : y;
                    }
                b = BitmapFilter.OffsetFilterAbs(b, ptFlip);
            }
            catch
            { throw; }

            return b;
        }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Static Methods
        // 
        private static Bitmap ProcessBitmap(Bitmap b, BitmapFilterType type, params object[] args)
        {
            if (b.PixelFormat != PixelFormat.Format24bppRgb)
                b = b.Clone(new Rectangle(0, 0, b.Width, b.Height), PixelFormat.Format24bppRgb);

            BitmapData bmpData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            unsafe
            {
                byte* p = (byte*)(void*)bmpData.Scan0;

                int nWidth = b.Width * 3;
                int nOffset = bmpData.Stride - nWidth;

                switch (type)
                {
                    case BitmapFilterType.Invert:
                        #region Invert Bitmap
                        {
                            for (int y = 0; y < b.Height; ++y)
                            {
                                for (int x = 0; x < nWidth; ++x)
                                {
                                    p[0] = (byte)(255 - p[0]);
                                    ++p;
                                }
                                p += nOffset;
                            }
                            break;
                        }
                        #endregion
                    case BitmapFilterType.GrayScale:
                        #region GrayScape Bitmap
                        {
                            byte blue, green, red;
                            for (int y = 0; y < b.Height; ++y)
                            {
                                for (int x = 0; x < b.Width; ++x)
                                {
                                    blue = p[0];
                                    green = p[1];
                                    red = p[2];

                                    p[0] = p[1] = p[2] = (byte)(.299 * red + .587 * green + .114 * blue);

                                    p += 3;
                                }
                                p += nOffset;
                            }
                            break;
                        }
                        #endregion
                    case BitmapFilterType.Brightness:
                        #region Adjust Brightness
                        {
                            int nBrightness = (int)args[0];
                            int nVal = 0;
                            for (int y = 0; y < b.Height; ++y)
                            {
                                for (int x = 0; x < nWidth; ++x)
                                {
                                    nVal = (int)(p[0] + nBrightness);

                                    if (nVal < 0) nVal = 0;
                                    if (nVal > 255) nVal = 255;

                                    p[0] = (byte)nVal;
                                    ++p;
                                }
                                p += nOffset;
                            }
                            break;
                        }
                        #endregion
                    case BitmapFilterType.Contrast:
                        #region Adjust Contrast
                        {
                            sbyte nContrast = (sbyte)args[0];
                            double pixel = 0, contrast = (100.0 + nContrast) / 100.0;
                            contrast *= contrast;
                            int red, green, blue;
                            for (int y = 0; y < b.Height; ++y)
                            {
                                for (int x = 0; x < b.Width; ++x)
                                {
                                    blue = p[0];
                                    green = p[1];
                                    red = p[2];

                                    pixel = red / 255.0;
                                    pixel -= 0.5;
                                    pixel *= contrast;
                                    pixel += 0.5;
                                    pixel *= 255;
                                    if (pixel < 0) pixel = 0;
                                    if (pixel > 255) pixel = 255;
                                    p[2] = (byte)pixel;

                                    pixel = green / 255.0;
                                    pixel -= 0.5;
                                    pixel *= contrast;
                                    pixel += 0.5;
                                    pixel *= 255;
                                    if (pixel < 0) pixel = 0;
                                    if (pixel > 255) pixel = 255;
                                    p[1] = (byte)pixel;

                                    pixel = blue / 255.0;
                                    pixel -= 0.5;
                                    pixel *= contrast;
                                    pixel += 0.5;
                                    pixel *= 255;
                                    if (pixel < 0) pixel = 0;
                                    if (pixel > 255) pixel = 255;
                                    p[0] = (byte)pixel;

                                    p += 3;
                                }
                                p += nOffset;
                            }
                            break;
                        }
                        #endregion
                    case BitmapFilterType.Gamma:
                        #region Adjust Gamma
                        {
                            double
                                red = (double)args[0],
                                green = (double)args[1],
                                blue = (double)args[2];

                            byte[] redGamma = new byte[256];
                            byte[] greenGamma = new byte[256];
                            byte[] blueGamma = new byte[256];

                            for (int i = 0; i < 256; ++i)
                            {
                                redGamma[i] = (byte)System.Math.Min(255, (int)((255.0 * System.Math.Pow(i / 255.0, 1.0 / red)) + 0.5));
                                greenGamma[i] = (byte)System.Math.Min(255, (int)((255.0 * System.Math.Pow(i / 255.0, 1.0 / green)) + 0.5));
                                blueGamma[i] = (byte)System.Math.Min(255, (int)((255.0 * System.Math.Pow(i / 255.0, 1.0 / blue)) + 0.5));
                            }

                            for (int y = 0; y < b.Height; ++y)
                            {
                                for (int x = 0; x < b.Width; ++x)
                                {
                                    p[2] = redGamma[p[2]];
                                    p[1] = greenGamma[p[1]];
                                    p[0] = blueGamma[p[0]];

                                    p += 3;
                                }
                                p += nOffset;
                            }
                            break;
                        }
                        #endregion
                    case BitmapFilterType.Color:
                        #region Adjust Color
                        {
                            int
                                red = (int)args[0],
                                green = (int)args[1],
                                blue = (int)args[2];
                            int nPixel;

                            for (int y = 0; y < b.Height; ++y)
                            {
                                for (int x = 0; x < b.Width; ++x)
                                {
                                    nPixel = p[2] + red;
                                    nPixel = System.Math.Max(nPixel, 0);
                                    p[2] = (byte)System.Math.Min(255, nPixel);

                                    nPixel = p[1] + green;
                                    nPixel = System.Math.Max(nPixel, 0);
                                    p[1] = (byte)System.Math.Min(255, nPixel);

                                    nPixel = p[0] + blue;
                                    nPixel = System.Math.Max(nPixel, 0);
                                    p[0] = (byte)System.Math.Min(255, nPixel);

                                    p += 3;
                                }
                                p += nOffset;
                            }
                            break;
                        }
                        #endregion
                }
            }
            b.UnlockBits(bmpData);
            return b;
        }
        private static Bitmap AdvConvFilter(Bitmap b, double[,] convMatrix, double factor = 1, int bias = 0)
        {
            // Obtain access to the unmanaged memory where the source bitmap is stored.
            BitmapData srcData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            // Create byte arrays to store image data.
            byte[] pxlBfr = new byte[srcData.Stride * srcData.Height];
            byte[] outBfr = new byte[srcData.Stride * srcData.Height];

            // Copy our source image into the source pixel buffer. ("Marshal.Copy" does this directly in unmanaged memory for performance).
            System.Runtime.InteropServices.Marshal.Copy(srcData.Scan0, pxlBfr, 0, pxlBfr.Length);

            // Now that we have the source image in a byte array[], but can release direct memory access to the Bitmap object itself.
            b.UnlockBits(srcData);

            // Remember, Windows does "little-endian", so the byte order is going to be BGR, not RGB.
            double
                blue = 0.0,
                green = 0.0,
                red = 0.0;

            int
                filterW = convMatrix.Length,
                filterH = convMatrix.Length,
                filterOffset = (filterW - 1) / 2,
                calcOffset = 0,
                byteOffset = 0;

            for (int y = filterOffset; y < b.Height - filterOffset; y++)
                for (int x = filterOffset; x < b.Width - filterOffset; x++)
                {
                    // Reset color values.
                    blue = green = red = 0;

                    // Find array position of the current byte.
                    byteOffset = y * srcData.Stride + x * 4;

                    for (int yf = -filterOffset; yf <= filterOffset; yf++)
                        for (int xf = -filterOffset; xf <= filterOffset; xf++)
                        {
                            calcOffset = byteOffset + (xf * 4) + (yf * srcData.Stride);

                            blue += (double)(pxlBfr[calcOffset]) * convMatrix[yf + filterOffset, xf + filterOffset];
                            green += (double)(pxlBfr[calcOffset + 1]) * convMatrix[yf + filterOffset, xf + filterOffset];
                            red += (double)(pxlBfr[calcOffset + 2]) * convMatrix[yf + filterOffset, xf + filterOffset];

                        }

                    // Adjust for factor/bias values.
                    blue = factor * blue + bias;
                    green = factor * green + bias;
                    red = factor * red + bias;

                    // Make sure that none of our color values are out of range.
                    blue = (blue > 255 ? 255 : (blue < 0 ? 0 : blue));
                    green = (green > 255 ? 255 : (green < 0 ? 0 : green));
                    red = (red > 255 ? 255 : (red < 0 ? 0 : red));

                    outBfr[byteOffset] = (byte)blue;
                    outBfr[byteOffset + 1] = (byte)green;
                    outBfr[byteOffset + 2] = (byte)red;
                    outBfr[byteOffset + 3] = 255;   // Alpha channel
                }

            // Now, we're ready to create the final (blurred) bitmap.
            Bitmap outBmp = new Bitmap(b.Width, b.Height);

            // Get access to the new Bitmap's unmanaged memory space.
            BitmapData outData = outBmp.LockBits(new Rectangle(0, 0, outBmp.Width, outBmp.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            // Copy the output buffer array into the new Bitmap's memory space.
            System.Runtime.InteropServices.Marshal.Copy(outBfr, 0, outData.Scan0, outBfr.Length);

            // Unlock the Bitmap's memory.
            outBmp.UnlockBits(outData);

            // And, finally, return the new bitmap object.
            return outBmp;
        }
        #endregion
    }
}
