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
using System.Web;

namespace RainstormStudios.Web.HttpHandlers
{
    [Author("Unfried, Michael")]
    public class ToolbarImageHandler : IHttpHandler
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

            string
                qsImgUrl = req.QueryString["src"],
                qsAct = req.QueryString["a"];

            bool buttonActive = (string.IsNullOrEmpty(qsAct) || qsAct == "1");
            string srcUrl = WebUtil.MapPath(WebUtil.UrlDecode(qsImgUrl));

            System.IO.FileInfo fiImg = new System.IO.FileInfo(srcUrl);
            if (!fiImg.Exists)
            { context.Response.StatusCode = 404; }
            else
            {
                byte[] imgData = null;
                string contentType = string.Format("image/{0}", fiImg.Extension.TrimStart('.'));
                using (System.IO.FileStream fs = fiImg.OpenRead())
                {
                    if (buttonActive)
                    {
                        // If the button is active, just load the image directly.
                        imgData = new byte[fs.Length];
                        fs.Read(imgData, 0, imgData.Length);
                    }
                    else
                    {
                        // If the button is disabled, load the image into a Bitmap object, then run it through the grayscale filter.
                        using (System.Drawing.Bitmap img = (System.Drawing.Bitmap)System.Drawing.Bitmap.FromStream(fs))
                        using (System.Drawing.Bitmap imgGs = RainstormStudios.Drawing.BitmapFilter.GrayScale(img))
                        using(System.IO.MemoryStream imgGsStrm = new System.IO.MemoryStream())
                        {
                            imgGs.Save(imgGsStrm, img.RawFormat);
                            imgData = new byte[imgGsStrm.Length];
                            imgGsStrm.Read(imgData, 0, imgData.Length);
                        }
                    }
                }

                context.Response.StatusCode = 200;
                context.Response.ContentType = contentType;
                context.Response.OutputStream.Write(imgData, 0, imgData.Length);

            }
        }
        #endregion
    }
}
