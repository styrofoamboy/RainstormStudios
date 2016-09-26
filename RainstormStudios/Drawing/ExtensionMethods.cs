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
using System.Linq;
using System.Text;
using System.Drawing;

namespace RainstormStudios.Drawing
{
    public static class ColorExtensionMethods
    {
        /// <summary>
        /// Coverts this <see cref="T:System.Drawing.Color"/> value into it's equivalent hexidecimal (or named) web color.
        /// </summary>
        /// <param name="clr">The <see cref="T:System.Drawing.Color"/> value to convert.</param>
        /// <returns>A <see cref="T:System.String"/> value containing the hexidecimal color representation.</returns>
        public static string GetWebColor(this Color clr)
        {
            return RainstormStudios.Hex.GetWebColor(clr);
        }
        /// <summary>
        /// Converts a hexidecimal web color value contained within a <see cref="T:System.String"/> into the equivalent <see cref="T:System.Drawing.Color"/> value.
        /// </summary>
        /// <param name="clr"></param>
        /// <param name="webColorString">A <see cref="T:System.String"/> value containing a hexidecimal web color.</param>
        /// <returns>A <see cref="T:System.Drawing.Color"/> value equivalent to the specified hexidecimal web color.</returns>
        public static Color GetSystemColor(this Color clr, string webColorString)
        {
            return RainstormStudios.Hex.GetSystemColor(webColorString);
        }
        /// <summary>
        /// Converts a three-part hexidecimal web color value into the equivalent <see cref="T:System.Drawing.Color"/> value.
        /// </summary>
        /// <param name="clr"></param>
        /// <param name="hexRed">A <see cref="T:System.String"/> value containing the amount of red as a hexidecimal string.</param>
        /// <param name="hexGreen">A <see cref="T:System.String"/> value containing the amount of green as a hexidecimal string.</param>
        /// <param name="hexBlue">A <see cref="T:System.String"/> value containing the amount of blue as a hexidecimal string.</param>
        /// <returns>A <see cref="T:System.Drawing.Color"/> value equivalent to the specified hexidecimal web color.</returns>
        public static Color GetSystemColor(this Color clr, string hexRed, string hexGreen, string hexBlue)
        {
            return RainstormStudios.Hex.GetSystemColor(hexRed, hexGreen, hexBlue);
        }
        /// <summary>
        /// Converts a three-part hexidecimal web color value into the equivalent <see cref="T:System.Drawing.Color"/> value.
        /// </summary>
        /// <param name="clr"></param>
        /// <param name="hexRed">A <see cref="T:System.Byte"/> value containing the amount of red (0-255).</param>
        /// <param name="hexGreen">A <see cref="T:System.Byte"/> value containing the amount of green (0-255).</param>
        /// <param name="hexBlue">A <see cref="T:System.Byte"/> value containing the amount of blue (0-255).</param>
        /// <returns>A <see cref="T:System.Drawing.Color"/> value equivalent to the specified byte values.</returns>
        public static Color GetSystemColor(this Color clr, byte red, byte green, byte blue)
        {
            return RainstormStudios.Hex.GetSystemColor(red, green, blue);
        }
    }
}
