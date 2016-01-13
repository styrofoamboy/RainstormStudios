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
using System.Drawing;
using System.Linq;
using System.Text;
using System.Web;

namespace RainstormStudios.Web.HttpHandlers
{
    public class ImageButtonHandler : System.Web.IHttpHandler
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

            string imgUrl = context.Server.UrlDecode(req.QueryString["img"]);
            string imgLcPath = context.Server.MapPath(imgUrl);
            using (System.Drawing.Bitmap bmpOrig = (Bitmap)System.Drawing.Image.FromFile(imgLcPath))
            {
                using (Bitmap bmp = RainstormStudios.Drawing.BitmapFilter.GrayScale(bmpOrig))
                {
                    // Convert the image to a PNG, and return the bytes into the response output stream.
                    using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                    {
                        bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                        context.Response.StatusCode = 200;
                        context.Response.ContentType = "image/png";
                        byte[] imgBts = ms.ToArray();
                        context.Response.OutputStream.Write(imgBts, 0, imgBts.Length);
                    }
                }
            }
        }
        #endregion
    }
}
