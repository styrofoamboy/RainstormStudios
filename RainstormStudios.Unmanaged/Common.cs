using System;
using System.Collections;
using System.Text;
using System.Runtime.InteropServices;

namespace RainstormStudios.Unmanaged
{
    public static class InteropCommon
    {
        const int ERROR_SUCCESS = 0;
        const int ERROR_INSUFFICIENT_BUFFER = 122;

        /// <summary>
        /// Retrieves the IntPtr value of the currently active window.
        /// </summary>
        /// <returns>Returns the IntPtr value of the currently active user window.</returns>
        [DllImport("user32.dll")]
        public static extern IntPtr GetActiveWindow();

        /// <summary>
        /// Returns the position of the specified window.
        /// </summary>
        /// <param name="hwnd">The IntPtr value of the window you want to retrieve the rectangle of.</param>
        /// <param name="lpRect">A variable of type RECT struct to contain the window's rectangle coordinates.</param>
        /// <returns>Returns a bool value indicating success or failure of retrieving the window's rectangle.</returns>
        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);
        /// <summary>
        /// Provides a serialized struct to contain the returned window rectangle.  This struct is *not* binary compatible with System.Drawing.Rectangle.
        /// </summary>
        [Serializable, StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        /// <summary>
        /// Closes the specified user window.
        /// </summary>
        /// <param name="hwnd">The IntPtr value of the window you want to close.</param>
        /// <returns>A bool value indicating success or falure.</returns>
        [DllImport("user32.dll")]
        public static extern bool CloseWindow(IntPtr hwnd);

        /// <summary>
        /// Attempts to find an active window matching the Class and Window names given.
        /// </summary>
        /// <param name="lpClassName">A string value containing the class name of the window you want to find.</param>
        /// <param name="lpWindowName">A string value containing the window name of the window you want to find.</param>
        /// <returns>The IntPrt value of the window, if found.</returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        public static IntPtr FindWindow(string lpWindowName)
        {
            return FindWindow(null, lpWindowName);
        }

        /// <summary>
        /// Determines if a specified user window is visible.
        /// </summary>
        /// <param name="hwnd">The IntPtr value of the window to look for.</param>
        /// <returns>A bool value indicating if the window is visible.</returns>
        [DllImport("user32.dll")]
        public static extern bool IsWindowVisible(IntPtr hwnd);

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
        }

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool LogonUser(
            string lpszUsername,
            string lpszDomain,
            string lpszPassword,
            int dwLoginType,
            int dwLogonProvider,
            out IntPtr phToken);
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
