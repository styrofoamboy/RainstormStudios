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
using System.Runtime.InteropServices;

namespace RainstormStudios.Unmanaged
{
    class Api_NetApi32
    {
        const int ERROR_SUCCESS = 0;
        const int ERROR_INSUFFICIENT_BUFFER = 122;

        [DllImport("Netapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int NetGetJoinInformation(
            [In, MarshalAs(UnmanagedType.LPWStr)]string server,
            out IntPtr domain,
            out NetJoinStatus status);
        public enum NetJoinStatus
        {
            NetSetupUnknownStatus = 0,
            NetSetupUnjoined,
            NetSetupWorkgroupName,
            NetSetupDomainName
        }
        [DllImport("netapi32.dll")]
        static extern int NetApiBufferFree(IntPtr Buffer);
        public static string GetJoinedDomain()
        {
            int result = 0;
            string domain = null;
            IntPtr pDomain = IntPtr.Zero;
            NetJoinStatus status = NetJoinStatus.NetSetupUnknownStatus;
            try
            {
                result = NetGetJoinInformation(null, out pDomain, out status);
                if (result == ERROR_SUCCESS && status == NetJoinStatus.NetSetupDomainName)
                {
                    domain = Marshal.PtrToStringUni(pDomain);
                }
            }
            finally
            {
                if (pDomain != IntPtr.Zero) NetApiBufferFree(pDomain);
            }
            if (domain == null) domain = "";
            return domain;
        }
    }
}
