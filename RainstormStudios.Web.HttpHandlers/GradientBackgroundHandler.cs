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
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace RainstormStudios.Web.HttpHandlers
{
    [Author("Unfried, Michael")]
    public class GradientBackgroundHandler : System.Web.IHttpHandler
    {
        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public bool IsReusable
        { get { return false; } }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public void ProcessRequest(HttpContext context)
        {
            HttpRequest req = context.Request;
            HttpResponse res = context.Response;

            string
                sTopClr = req.QueryString["tc"],
                sBtmClr = req.QueryString["bc"],
                sAlpha = req.QueryString["a"],
                sHeight = req.QueryString["h"];

            int iHeight;
            if (string.IsNullOrEmpty(sHeight))
                sHeight = "60";
            if (!int.TryParse(sHeight, out iHeight))
                throw new Exception("Specified height is not a valid integer value.");

            int iAlpha;
            if (string.IsNullOrEmpty(sAlpha))
                sAlpha = "255";
            if (!int.TryParse(sAlpha, out iAlpha))
                throw new Exception("Specified alpha is not a valid integer value.");
            iAlpha = System.Math.Max(0, System.Math.Min(255, iAlpha));

            Color topColor = RainstormStudios.Hex.GetSystemColor(sTopClr);
            Color btmColor = RainstormStudios.Hex.GetSystemColor(sBtmClr);

            if (iAlpha < 255)
            {
                topColor = Color.FromArgb(iAlpha, topColor);
                btmColor = Color.FromArgb(iAlpha, btmColor);
            }

            using (Bitmap bmp = new Bitmap(1, iHeight))
            {
                using (Graphics gBmp = Graphics.FromImage(bmp))
                {
                    gBmp.Clear(Color.FromArgb(0, Color.White));
                    gBmp.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
                    gBmp.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                    gBmp.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    gBmp.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    gBmp.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

                    using (LinearGradientBrush bgBrush = new LinearGradientBrush(
                                                                new Point(0, 0),
                                                                new Point(0, iHeight),
                                                                topColor, btmColor))
                        gBmp.FillRectangle(bgBrush, new Rectangle(0, 0, 1, iHeight));
                }

                // We're not doing anything fancy, here.  Just rendering a 1px wide gradient image at the specified height.
                // Time to output the image...
                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                {
                    bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    context.Response.StatusCode = 200;
                    context.Response.ContentType = "image/png";
                    byte[] imgBits = ms.ToArray();
                    context.Response.OutputStream.Write(imgBits, 0, imgBits.Length);
                }
            }
        }
        #endregion
    }
}
