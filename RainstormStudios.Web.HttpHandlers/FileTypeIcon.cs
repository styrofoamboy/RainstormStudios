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
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Drawing;

namespace RainstormStudios.Web.HttpHandlers
{
    public class FileTypeIconHandler : IHttpHandler
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
                strImgPath = req["imgpth"],
                strExt = req["ext"];

            try
            {
                if (!string.IsNullOrEmpty(strImgPath))
                {
                    // If the application provided an alternate image file path, look
                    //   there first.
                    string lcPath = System.IO.Path.Combine(context.Server.MapPath(strImgPath), strExt + ".gif");
                    if (System.IO.File.Exists(lcPath))
                        context.Response.WriteFile(lcPath);
                    return;
                }

                object objImg = null;
                try
                {
                    // If we didn't find the icon in the alternate image path, then
                    //   check the local resources.
                    objImg = global::RainstormStudios.Web.HttpHandlers.Properties.Resources.ResourceManager.GetObject(strExt);

                    if (objImg == null)
                    {
                        // If we didn't found it with any method above, just pull the
                        //   'generic' file icon from the resource collection.
                        objImg = global::RainstormStudios.Web.HttpHandlers.Properties.Resources.ResourceManager.GetObject("file");
                    }

                    if (objImg == null)
                        throw new Exception("Unable to determine any images for file type icon.");

                    using (MemoryStream ms = new MemoryStream())
                    {
                        ((Bitmap)objImg).Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
                        context.Response.StatusCode = 200;
                        context.Response.ContentType = "image/gif";
                        byte[] imgBts = ms.ToArray();
                        context.Response.OutputStream.Write(imgBts, 0, imgBts.Length);
                    }

                }
                finally
                {
                    if (objImg != null && objImg is IDisposable)
                        ((IDisposable)objImg).Dispose();
                }
            }
            catch (Exception ex)
            {
                context.AddError(ex);
            }
        }
        #endregion
    }
}
