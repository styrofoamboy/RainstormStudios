using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace RainstormStudios.ActiveDirectory
{
    public class Impersonation
    {
        #region Declarations
        //***************************************************************************
        // Constants
        const int
            LOGON32_PROVIDER_DEFAULT = 0;
        // This parameter causes LogonUser to create a primary token.
        const int
            LOGIN32_LOGON_INTERACTIVE = 2;
        //***************************************************************************
        // Private Fields
        // 
        string
            _domainNm,
            _userNm,
            _password;
        WindowsImpersonationContext
            _winImpContext;
        IntPtr
            _userTokenHandle;
        bool
            _isAuthenticated = false;
        int 
            _userID;
        //***************************************************************************
        // External MFC Imports
        // 
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool LogonUser(String lpszUsername, String lpszDomain, String lpszPassword,
                int dwLogonType, int dwLogonProvider, ref IntPtr phToken);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public extern static bool CloseHandle(IntPtr handle);
        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public extern static bool DuplicateToken(IntPtr ExistingTokenHandle,
                int SECURITY_IMPERSONATION_LEVEL, ref IntPtr DuplicateTokenHandle);
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public bool IsAuthenticated
        { get { return this._isAuthenticated; } }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public Impersonation(string domainName, string userName, string password)
        {
            this._domainNm = domainName;
            this._userNm = userName;
            this._password = password;
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public void Impersonate()
        {
            IntPtr tokenHandle = IntPtr.Zero;
            IntPtr dupeTokenHandle = IntPtr.Zero;

            bool returnValue = LogonUser(this._userNm, this._domainNm, this._password,
                                            LOGIN32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT,
                                            ref tokenHandle);

            if (!returnValue)
            {
                // If the 'LogonUser' MFC call didn't return 'true', then we need to
                //   query for the last Win32 error and return it to the caller.
                int ret = Marshal.GetLastWin32Error();
                this._isAuthenticated = false;
                throw new System.ComponentModel.Win32Exception(ret);
            }

            this._userTokenHandle = tokenHandle;
            WindowsIdentity newId = new WindowsIdentity(tokenHandle);
            this._winImpContext = newId.Impersonate();
            this._isAuthenticated = true;
        }
        public void Unimpersonate()
        {
            if (this._winImpContext != null)
            {
                this._winImpContext.Undo();
                this._winImpContext.Dispose();
            }
            this._winImpContext = null;
            this._isAuthenticated = false;

            if (this._userTokenHandle != IntPtr.Zero)
                CloseHandle(this._userTokenHandle);
            this._userTokenHandle = IntPtr.Zero;
        }
        #endregion
    }
}
