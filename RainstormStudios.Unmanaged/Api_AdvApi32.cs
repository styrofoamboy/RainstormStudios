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
    [Author("Unfried, Michael")]
    class Api_AdvApi32
    {
        const int ERROR_SUCCESS = 0;
        const int ERROR_INSUFFICIENT_BUFFER = 122;

        [DllImport("kernel32.dll")]
        static extern IntPtr LocalFree(IntPtr hMem);
        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool ConvertSidToStringSid(
            [MarshalAs(UnmanagedType.LPArray)] byte[] pSID,
            out IntPtr ptrSid);
        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool LookupAccountName(
            string lpSystemName,
            string lpAccountName,
            [MarshalAs(UnmanagedType.LPArray)] byte[] Sid,
            ref uint cbSid,
            StringBuilder ReferencedDomainName,
            ref uint cchReferencedDomainName,
            out SID_NAME_USE peUse);
        [Author("Unfried, Michael")]
        enum SID_NAME_USE
        {
            SidTypeUser = 1,
            SidTypeGroup,
            SidTypeDomain,
            SidTypeAlias,
            SidTypeWellKnownGroup,
            SidTypeDeletedAccount,
            SidTypeInvalid,
            SidTypeUnknown,
            SidTypeComputer
        }
        /// <summary>
        /// Searches for the given account name and returns the SID associated with that account.  Local machine accounts are searched first, then the primary domain, then trusted domains.  Using fully-qualified account names is highly recommended, but not required.
        /// </summary>
        /// <param name="AccountName">A string value containing the account name to search for.</param>
        /// <returns>A string value containing the SID for the specified account, or an error msg if the SID could not be retrieved.</returns>
        public static string GetUserSid(string AccountName)
        {
            byte[] Sid = null;
            uint cbSid = 0;
            StringBuilder refDomainName = new StringBuilder();
            uint cchRefDomainName = (uint)refDomainName.Capacity;
            SID_NAME_USE sidUse;

            int err = ERROR_SUCCESS;
            if (!LookupAccountName(null, AccountName, Sid, ref cbSid, refDomainName, ref cchRefDomainName, out sidUse))
            {
                err = Marshal.GetLastWin32Error();
                if (err == ERROR_INSUFFICIENT_BUFFER)
                {
                    Sid = new byte[cbSid];
                    refDomainName.EnsureCapacity((int)cchRefDomainName);
                    err = ERROR_SUCCESS;
                    if (!LookupAccountName(null, AccountName, Sid, ref cbSid, refDomainName, ref cchRefDomainName, out sidUse))
                        err = Marshal.GetLastWin32Error();
                }
            }
            if (err == 0)
            {
                IntPtr ptrSid;
                if (!ConvertSidToStringSid(Sid, out ptrSid))
                {
                    err = Marshal.GetLastWin32Error();
                    return "Could not convert SID to string. Error : " + err.ToString();
                }
                else
                {
                    string sidString = Marshal.PtrToStringAuto(ptrSid);
                    LocalFree(ptrSid);
                    return sidString;
                }
            }
            else
                return "Error : " + err.ToString();
        }

        [DllImport("userenv.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool LoadUserProfile(IntPtr hToken, ref PROFILEINFO lpProfileInfo);
        [Author("Unfried, Michael")]
        public struct PROFILEINFO
        {
            public int dwSize;
            public int dwFlags;
            public string lpUserName;
            public string lpProfilePath;
            public string lpDefaultPath;
            public string lpServerName;
            public string lpPolicyPath;
            public IntPtr hProfile;

            public PROFILEINFO(int dwSz, int dwFg, string lpUn, string lpPp, string lpDp, string lpSn, string lpPp2, IntPtr hProf)
            {
                this.dwSize = dwSz;
                this.dwFlags = dwFg;
                this.lpUserName = lpUn;
                this.lpProfilePath = lpPp;
                this.lpDefaultPath = lpDp;
                this.lpServerName = lpSn;
                this.lpPolicyPath = lpPp2;
                this.hProfile = hProf;
            }
        }

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool LogonUser(
            string lpszUsername,
            string lpszDomain,
            string lpszPassword,
            int dwLoginType,
            int dwLogonProvider,
            out IntPtr phToken);
        [Author("Unfried, Michael")]
        public enum LogonType : int
        {
            /// <summary>
            /// This logon type is intended for users who will be interactively using the computer, such as a user being logged on by a terminal server, remote shell, or similar process.
            /// This logon type has the additional expense of caching logon information for disconnecting operations;  therefore, it is inappropriate for some client/server applications, such as a mail server.
            /// </summary>
            LOGON32_LOGON_INTERACTIVE = 2,
            /// <summary>
            /// This logon type is intended for high-perfomance servers to authenticate plaintext passwords.
            /// The LogonUser function does not cache credentials for this logon type.
            /// </summary>
            LOGON32_LOGON_NETWORK = 3,
            /// <summary>
            /// This logon type is intended for batch servers, where processes may be executing on behalf of a user without their direct intervention.
            /// This type is also for higher performance servers that process many plaintext authentication attempts at a time, such as mail or web servers.
            /// The LogonUser function does not cache credentials for this logon type.
            /// </summary>
            LOGON32_LOGON_BATCH = 4,
            /// <summary>
            /// Indicates a service-type logon. The account provided must have the service privilege enabled.
            /// </summary>
            LOGON32_LOGON_SERVICE = 5,
            /// <summary>
            /// This logon type is for GINA DLL's that logon users who will be interactively using the computer.
            /// This logon type can generate a unique audit record that shows when the workstation was unlocked.
            /// </summary>
            LOGON32_LOGON_UNLOCK = 7,
            /// <summary>
            /// This logon type preserves the name and password in the authentication package, which allows the server to make connections to other network servers while impersonating the client.
            /// A server can accept plaintext credentials from a client, call LogonUser, verify that the user can access the system access the network, and still communicated with other servers.
            /// NOTE:  This value is not supported by Windows NT.
            /// </summary>
            LOGON32_LOGON_CLEARTEXT = 8,
            /// <summary>
            /// This logon type allows the caller to clone its current token and specify new credentials for outbound connections.
            /// The new logon session has the same local identifier but uses different credentials for other network connections.
            /// NOTE: This logon type is supported only by the LOGIN32_PROVIDER_WINNT50 logon provider.
            /// NOTE: This value is not supported by Windows NT.
            /// </summary>
            LOGON32_LOGON_NEW_CREDENTIALS = 9
        }
        [Author("Unfried, Michael")]
        public enum LogonProvider : int
        {
            /// <summary>
            /// Use the standard logon provider for the system.
            /// The default security provider is negotiate, unless you pass NULL for the domain name and the user name, is not in UPN format. In this case, the default provider is NTLM.
            /// NOTE: For Windows 2000/NT, the default security provider is NTLM.
            /// </summary>
            LOGON32_PROVIDER_DEFAULT = 0
        }
    }
}
