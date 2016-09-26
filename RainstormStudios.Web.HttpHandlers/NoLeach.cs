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
using System.Drawing;
using System.Web;
using System.Globalization;

namespace RainstormStudios.Web.HttpHandlers
{
    public class NoLeechImageHandler : IHttpHandler
    {
        public bool IsReusable
        { get { return false; } }

        public void ProcessRequest(HttpContext context)
        {
            HttpRequest req = context.Request;
            string imgPath = req.PhysicalPath;
            string extension = null;

            if (req.UrlReferrer != null && req.UrlReferrer.Host.Length > 0)
                if (CultureInfo.InvariantCulture.CompareInfo.Compare(req.Url.Host, req.UrlReferrer.Host, CompareOptions.IgnoreCase) != 0)
                {
                    context.Response.Status = "Image not found";
                    context.Response.StatusCode = 404;
                }
            extension = Path.GetExtension(imgPath).ToLower();
            string contentType = string.Empty;
            System.Drawing.Imaging.ImageFormat imgFmt = null;
            switch (extension)
            {
                case ".gif":
                    contentType = "image/gif";
                    imgFmt = System.Drawing.Imaging.ImageFormat.Gif;
                    break;
                case ".jpg":
                    contentType = "image/jpeg";
                    imgFmt = System.Drawing.Imaging.ImageFormat.Jpeg;
                    break;
                case ".png":
                    contentType = "image/png";
                    imgFmt = System.Drawing.Imaging.ImageFormat.Png;
                    break;
                default:
                    throw new NotSupportedException("Unrecognized image type.");
            }
            if (!File.Exists(imgPath))
            {
                context.Response.Status = "Image not found";
                context.Response.StatusCode = 404;
            }
            else
            {
                context.Response.StatusCode = 200;
                context.Response.ContentType = contentType;
                context.Response.WriteFile(imgPath);
            }
        }
    }
}