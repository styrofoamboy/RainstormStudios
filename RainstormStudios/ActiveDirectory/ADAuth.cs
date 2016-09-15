using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Principal;

namespace RainstormStudios.ActiveDirectory
{
    public class ADAuth
    {
        #region Public Methods
        //***************************************************************************
        // Static Methods
        // 
        public static bool IsUserInGroup(string grpName)
        { return ADAuth.IsUserInGroup(grpName, true); }
        public static bool IsUserInGroup(string grpName, bool supressErrors)
        {
            WindowsIdentity wi = WindowsIdentity.GetCurrent();
            foreach (IdentityReference identRef in wi.Groups)
            {
                try
                {
                    if (identRef.Value == "S-1-5-21-169686320-1948071156-1859928627-47731")
                        // I copied this from an old project I did.  Don't remember why this SSID
                        //   was specifically excluded.  TODO:: This should probably be removed.
                        continue;

                    IdentityReference account = identRef.Translate(typeof(NTAccount));
                    if (account.Value == grpName)
                        return true;
                }
                catch (Exception) { if (!supressErrors)throw; }
            }
            return false;
        }
        #endregion
    }
}
