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
using System.Collections;
using System.Drawing;
using System.Drawing.Text;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;

namespace RainstormStudios.Drawing
{
    [Author("Unfried, Michael")]
    public abstract class Imaging
    {
        #region Public Methods
        //***************************************************************************
        // Static Methods
        // 
        /// <summary>
        /// Calculates the bounds for an image to fill an area as completely as possible while still retaining is aspect ratio.
        /// </summary>
        /// <param name="img">The image to resize.</param>
        /// <param name="bounds">The bounds the image should fill.</param>
        /// <returns>A rectangle struct containing the new image size.</returns>
        public static Rectangle ZoomImage(Image img, Rectangle bounds)
        {
            return ZoomImage(img, bounds.X, bounds.Y, bounds.Width, bounds.Height);
        }
        public static RectangleF ZoomImage(Image img, RectangleF bounds)
        {
            return ZoomImage(img, bounds.Left, bounds.Top, bounds.Width, bounds.Height);
        }
        public static Rectangle ZoomImage(Image img, Point location, Size size)
        {
            return ZoomImage(img, location.X, location.Y, size.Width, size.Height);
        }
        public static RectangleF ZoomImage(Image img, PointF location, SizeF size)
        {
            return ZoomImage(img, location.X, location.Y, size.Width, size.Height);
        }
        public static Rectangle ZoomImage(Image img, int x, int y, int width, int height)
        {
            RectangleF rslt = ZoomImage(img, (float)x, (float)y, (float)width, (float)height);
            return new Rectangle((int)rslt.X, (int)rslt.Y, (int)rslt.Width, (int)rslt.Height);
        }
        public static RectangleF ZoomImage(Image img, float x, float y, float width, float height)
        {
            float iX = 0, iY = 0, iW = 0, iH = 0;
            // First, we have to determine whether the width or the height
            //   will get precidence over the final size.
            if ((width - img.Width) > (height - img.Height))
            {
                if (width > height)
                {
                    // Image height gets precidence
                    iW = (height * img.Width) / img.Height;
                    iH = height;
                    iX = (width / 2) - (iW / 2);
                    iY = 0;
                }
                else
                {
                    // Image width gets precidence
                    iW = 0;
                    iH = (width * img.Height) / img.Width;
                    iX = 0;
                    iY = (height / 2) - (iH / 2);
                }
            }
            else
            {
                if (width > height)
                {
                    // Image height gets precidence
                    iW = (height * img.Width) / img.Height;
                    iH = height;
                    iX = (width / 2) - (iW / 2);
                    iY = 0;
                }
                else
                {
                    // Image width gets precidence
                    iW = 0;
                    iH = (width * img.Height) / img.Width;
                    iX = 0;
                    iY = (height / 2) - (iH / 2);
                }
            }
            return new RectangleF(iX + x, iY + y, iW, iH);
        }
        /// <summary>
        /// Calculates the drawing point to center an image within the given rectangle.
        /// </summary>
        /// <param name="img">The image to center.</param>
        /// <param name="bounds">The rectangle to center the image within.</param>
        /// <returns>A point struct with the location at which the image should be drawn.</returns>
        public static Point CenterImage(Image img, Rectangle bounds)
        {
            return CenterImage(img, bounds.X, bounds.Y, bounds.Width, bounds.Height);
        }
        public static PointF CenterImage(Image img, RectangleF bounds)
        {
            return CenterImage(img, bounds.X, bounds.Y, bounds.Width, bounds.Height);
        }
        public static Point CenterImage(Image img, Point location, Size size)
        {
            return CenterImage(img, location.X, location.Y, size.Width, size.Height);
        }
        public static PointF CenterImage(Image img, PointF location, SizeF size)
        {
            return CenterImage(img, location.X, location.Y, size.Width, size.Height);
        }
        public static Point CenterImage(Image img, int x, int y, int width, int height)
        {
            return new Point(((width / 2) - (img.Width / 2)) + x, ((height / 2) - (img.Height / 2)) + y);
        }
        public static PointF CenterImage(Image img, float x, float y, float width, float height)
        {
            return new PointF(((width / 2) - (img.Width / 2)) + x, ((height / 2) - (img.Height / 2)) + y);
        }
        /// <summary>
        /// Returns the web hex color value for the given color.
        /// </summary>
        /// <param name="value">A color value to convert to web hex value.</param>
        /// <returns>A string object containing the color's hex value.</returns>
        public static string GetWebColor(Color value)
        {
            if (value == Color.Empty)
                throw new ArgumentException("Supplied color value cannot be empty.", "value");

            return Hex.ToHex(value.R) + Hex.ToHex(value.G) + Hex.ToHex(value.B);
        }
        public static Bitmap GetFontChar(string fontFileName, byte[] charCode, Color clr)
        {
            return Imaging.GetFontChar(fontFileName, charCode, Encoding.Default, clr);
        }
        public static Bitmap GetFontChar(string fontFileName, byte[] charCode, Encoding encoding, Color clr)
        {
            PrivateFontCollection fonts = new PrivateFontCollection();
            fonts.AddFontFile(fontFileName);

            // Determine given unicode character to draw.
            char[] charToDraw = encoding.GetChars(charCode);

            // Read font-family name from the font file.
            FontFamily fontfam = fonts.Families[0];

            // Create a new bitmap to return.
            using (Bitmap bmp = new Bitmap(32, 32))
            {
                // Create the graphics object used to draw.
                using (Graphics gBmp = Graphics.FromImage(bmp))
                using (SolidBrush brush = new SolidBrush(clr))
                using (Font font = new Font(fontfam, 12, FontStyle.Regular))
                {
                    // Draw the string.
                    gBmp.DrawString(
                        new string(charToDraw),
                        font,
                        brush,
                        6, 6);
                }

                // Make the background transparent.
                Color backClr = bmp.GetPixel(0, 0);
                bmp.MakeTransparent(backClr);

                // Putting your return value in a using tag has detrimental effects
                //   if you return the actual object, since it will be disposed as
                //   it leaves the method.  For this reason, I will return a copy
                //   of the actual object.  It makes execution take a few extra
                //   ms, but guarantees no memory leaks from this library.
                return bmp.Clone(new Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.PixelFormat.DontCare);
            }
        }
        /// <summary>
        /// Searches a 8bpp ColorPallette object and returns the index of the entry which most closely matches the specified color.
        /// </summary>
        /// <param name="palette">A ColorPalette object containing an 8bpp color palette.</param>
        /// <param name="clr">The color to match in the color palette.</param>
        /// <returns>A Byte value containing the index of the palette entry.</returns>
        public static byte GetSimilarColor(ColorPalette palette, Color clr)
        {
            byte minDiff = byte.MinValue;
            byte index = 0;

            for (int i = 0; i < palette.Entries.Length - 1; i++)
            {
                byte currentDiff = Imaging.GetMaxDiff(clr, palette.Entries[i]);
                if (currentDiff < minDiff)
                {
                    minDiff = currentDiff;
                    index = (byte)i;
                }
            }
            return index;
        }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Static Methods
        // 
        private static byte GetMaxDiff(Color a, Color b)
        {
            byte bDiff = System.Convert.ToByte(
                System.Math.Abs((short)(a.B - b.B)));

            byte gDiff = System.Convert.ToByte(
                System.Math.Abs((short)(a.G - b.G)));

            byte rDiff = System.Convert.ToByte(
                System.Math.Abs((short)(a.R - b.R)));

            return System.Math.Max(rDiff, System.Math.Max(bDiff, gDiff));
        }
        #endregion
    }
}
