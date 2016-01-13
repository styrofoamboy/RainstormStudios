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

namespace RainstormStudios.Web.HttpHandlers
{
    [Author("Michael Unfried")]
    public interface iDynamicImageControl
    {
        string ContentType
        { get; }
        System.IO.MemoryStream RenderImage();
    }
    [Author("Michael Unfried")]
    public class HttpImageRenderStream : System.Web.IHttpModule
    {
        #region Declarations
        //***************************************************************************
        // Constants
        // 
        public const string
            ImageHandlerRequestFilename = "dyanmic_image_stream.aspx";
        public const string
            ImageNamePrefix = "i_m_g";
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public HttpImageRenderStream()
        { }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public virtual void Dispose()
        { }
        public virtual void Init(System.Web.HttpApplication httpApp)
        {
            httpApp.BeginRequest += new EventHandler(httpApp_BeginRequest);
        }
        #endregion

        #region Event Handlers
        //***************************************************************************
        // Event Handlers
        // 
        void httpApp_BeginRequest(object sender, EventArgs e)
        {
            System.Web.HttpApplication httpApp = (System.Web.HttpApplication)sender;

            iDynamicImageControl ctrl = null;
            // Check to see if the incoming HTTP request points at our "imaginary" web page.
            if (httpApp.Request.Path.ToLower().IndexOf(ImageHandlerRequestFilename) != -1)
            {
                ctrl = (iDynamicImageControl)httpApp.Application[ImageNamePrefix + httpApp.Request.QueryString["id"]];
                if (ctrl == null)
                {
                    return;     // 404 will be returned
                }
                else
                {
                    try
                    {
                        System.IO.MemoryStream mStrm = ctrl.RenderImage();
                        mStrm.WriteTo(httpApp.Context.Response.OutputStream);
                        mStrm.Close();

                        httpApp.Context.ClearError();
                        httpApp.Context.Response.ContentType = ctrl.ContentType;
                        httpApp.Response.StatusCode = 200;
                        httpApp.Application.Remove(ImageNamePrefix + httpApp.Request.QueryString["id"]);
                        httpApp.Response.End();
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }
        }
        #endregion
    }
}
