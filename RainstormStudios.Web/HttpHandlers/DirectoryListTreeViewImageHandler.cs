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
using System.Drawing;
using System.Linq;
using System.Text;
using System.Web;

namespace RainstormStudios.Web.HttpHandlers
{
    [Author("Michael Unfried")]
    class DirectoryListTreeViewImageHandler : IHttpHandler
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
                reqImg = req["imgNm"];

            try
            {
                object objImg = null;
                try
                {
                    if (!string.IsNullOrEmpty(reqImg))
                        objImg = global::RainstormStudios.Web.Properties.Resources.ResourceManager.GetObject(reqImg);

                    if (!string.IsNullOrEmpty(reqImg) && objImg == null)
                        context.AddError(new Exception("Unable to find requested image name.  Returning generic folder image."));

                    if (objImg == null)
                        objImg = global::RainstormStudios.Web.Properties.Resources.ResourceManager.GetObject("folder");

                    if (objImg == null)
                        throw new Exception("Unable to find any image resources.");

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
