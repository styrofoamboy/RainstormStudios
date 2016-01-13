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

// NOTE:  To use the ChevronHeader control, this handler must be referenced in the application's web.config file.
// 
// <add path="dbstrm.axd" verb="*" type="RainstormStudios.Web.HttpHandlers.DataStreamHandler, RainstormStudios.Web"/>
// 

namespace RainstormStudios.Web.HttpHandlers
{
    [Author("Unfried, Michael")]
    public class DatabaseStreamHandler : System.Web.IHttpHandler
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
                providerNm = req.QueryString["p"],
                key = req.QueryString["k"];

            RainstormStudios.Providers.DatabaseStreamProvider provider = null;
            if (string.IsNullOrEmpty(providerNm))
                provider = RainstormStudios.Providers.DatabaseStreamProviderManager.DefaultProvider;
            else
                provider = RainstormStudios.Providers.DatabaseStreamProviderManager.Providers[providerNm];

            if (provider == null)
                throw new Exception("Unable to determine database image provider.");

            RainstormStudios.Providers.DatabaseStreamData data = provider.GetImage(key);

            if (data.ImageData.Length > 0)
            {
                context.Response.StatusCode = 200;
                context.Response.ContentType = data.ContentType;
                context.Response.OutputStream.Write(data.ImageData, 0, data.ImageData.Length);
            }
            else
            { context.Response.StatusCode = 404; }
        }
        #endregion
    }
}
