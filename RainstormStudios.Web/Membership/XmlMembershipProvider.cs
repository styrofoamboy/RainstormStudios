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
using System.Data;
using System.Configuration.Provider;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Web;
using System.Web.Configuration;
using System.Web.Security;
using RainstormStudios.Collections;

namespace RainstormStudios.Web.Membership
{
    [Author("Michael Unfried")]
    public sealed class XmlMembershipProvider : System.Web.Security.MembershipProvider
    {
        #region Nested Classes
        //***************************************************************************
        // Nested Classes
        // 
        private enum FailureType
        {
            Password,
            PasswordAnswer
        }
        #endregion

        #region Declartions
        //***************************************************************************
        // Private Fields
        // 
        const string
            eventSource = "XmlMembershipProvider",
            eventLog = "Application";
        private bool
            _pwRet,
            _pwReset,
            _pwQnA,
            _reqUniqEmail,
            _wrtEvntLog;
        private string
            _appNm,
            _pwRegEx,
            _xmlFn;
        private int
            _minPwLen,
            _minPwNonAlpha,
            _maxPwFail,
            _pwAtmpTmr;
        private MembershipPasswordFormat
            _pwFmt;
        private MachineKeySection
            _machKey;
        private System.Threading.Mutex
            _mut;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public string XmlFilename
        { get { return this._xmlFn; } }
        public bool WriteToEventLog
        {
            get { return this._wrtEvntLog; }
            set { this._wrtEvntLog = value; }
        }
        public override bool EnablePasswordRetrieval
        { get { return this._pwRet; } }
        public override bool EnablePasswordReset
        { get { return this._pwReset; } }
        public override bool RequiresQuestionAndAnswer
        { get { return this._pwQnA; } }
        public override string ApplicationName
        {
            get { return this._appNm; }
            set { this._appNm = value; }
        }
        public override int MaxInvalidPasswordAttempts
        { get { return this._maxPwFail; } }
        public override int PasswordAttemptWindow
        { get { return this._pwAtmpTmr; } }
        public override bool RequiresUniqueEmail
        { get { return this._reqUniqEmail; } }
        public override MembershipPasswordFormat PasswordFormat
        { get { return this._pwFmt; } }
        public override int MinRequiredPasswordLength
        { get { return this._minPwLen; } }
        public override int MinRequiredNonAlphanumericCharacters
        { get { return this._minPwNonAlpha; } }
        public override string PasswordStrengthRegularExpression
        { get { return this._pwRegEx; } }
        //***************************************************************************
        // Private Properties
        // 
        private string XmlPath
        { get { return HttpContext.Current.Server.MapPath(this._xmlFn); } }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public XmlMembershipProvider()
            : base()
        { }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            // Initialize values from Web.Config
            // NOTE: ProviderBase class internally keeps track of how many times this
            //   method is called and will throw an exception if the method is
            //   called more than once.
            if (config == null)
                throw new ArgumentNullException("config");

            // Create the named mutex used to make sure only one thread can access
            //   the XML file at a time.
            this._mut = new System.Threading.Mutex(false, "XmlProvider");

            // Set the default provider name, if it's blank.
            if (string.IsNullOrEmpty(name))
                name = "XmlMembershipProvider";

            // Set the default provider description, if it's blank.
            if (string.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "XML file-based Membership Provider");
            }

            // Initialize the abstract base class.
            base.Initialize(name, config);

            // Load the global variables from the initialization collection.
            this._appNm = GetConfigValue(config["applicationName"], System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath);
            this._maxPwFail = Convert.ToInt32(GetConfigValue(config["maxInvalidPasswordAttempts"], 5));
            this._pwAtmpTmr = Convert.ToInt32(GetConfigValue(config["passwordAttemptWindow"], 10));
            this._minPwNonAlpha = Convert.ToInt32(GetConfigValue(config["minRequiredNonAlphanumericCharacters"], 1));
            this._minPwLen = Convert.ToInt32(GetConfigValue(config["minRequiredPasswordLength"], 7));
            this._pwRegEx = GetConfigValue(config["passwordStringRegularExpression"], "");
            this._pwReset = Convert.ToBoolean(GetConfigValue(config["enablePasswordReset"], true));
            this._pwRet = Convert.ToBoolean(GetConfigValue(config["enablePasswordRetrieval"], true));
            this._pwQnA = Convert.ToBoolean(GetConfigValue(config["requiresQuestionAndAnswer"], false));
            this._reqUniqEmail = Convert.ToBoolean(GetConfigValue(config["requiresUniqueEmail"], true));
            this._wrtEvntLog = Convert.ToBoolean(GetConfigValue(config["writeExceptionsToEventLog"], true));
            this._xmlFn = GetConfigValue(config["xmlFile"], "XmlMembership.xml");

            // Grab the Web.Config file of the current application and try to
            //   determine the machine key settings.
            System.Configuration.Configuration cfg = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration(System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath + "/web.config");
            System.Configuration.ConfigurationSectionGroup sysWebSec = cfg.SectionGroups["system.web"];
            System.Configuration.ConfigurationSection machKeySec = null;
            if (sysWebSec != null)
                machKeySec = sysWebSec.Sections["machineKey"];
            if (machKeySec != null && machKeySec.GetType().IsAssignableFrom(typeof(MachineKeySection)))
                this._machKey = (MachineKeySection)machKeySec;

            // Determine the format used for storing passwords.
            string temp_format = config["passwordFormat"];
            if (string.IsNullOrEmpty(temp_format))
                temp_format = "Hashed";
            this._pwFmt = (MembershipPasswordFormat)Enum.Parse(typeof(MembershipPasswordFormat), temp_format, true);
        }
        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out System.Web.Security.MembershipCreateStatus status)
        {
            ValidatePasswordEventArgs args =
                new ValidatePasswordEventArgs(username, password, true);

            OnValidatingPassword(args);

            if (args.Cancel)
            {
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }

            if (RequiresUniqueEmail && !string.IsNullOrEmpty(GetUserNameByEmail(email)))
            {
                status = MembershipCreateStatus.DuplicateEmail;
                return null;
            }

            MembershipUser u = GetUser(username, false);

            if (u == null)
            {
                DateTime createDate = DateTime.Now;

                if (providerUserKey == null)
                {
                    providerUserKey = Guid.NewGuid();
                }
                else
                {
                    if (!(providerUserKey is Guid))
                    {
                        status = MembershipCreateStatus.InvalidProviderUserKey;
                        return null;
                    }
                }

                try
                {
                    using (DataSet ds = this.OpenXml())
                    {
                        DataRow drNewUser = ds.Tables["Users"].NewRow();
                        drNewUser["UserID"] = providerUserKey;
                        drNewUser["UserName"] = username;
                        drNewUser["LoweredUserName"] = username.ToLower();
                        drNewUser["ApplicationID"] = XmlMembershipProvider.GetAppID(this._appNm, ds);
                        drNewUser["IsAnonymous"] = false;
                        drNewUser["LastActivityDate"] = createDate;

                        DataRow drNewMbr = ds.Tables["Membership"].NewRow();
                        drNewMbr["ApplicationiD"] = drNewUser["ApplicationID"];
                        drNewMbr["UserID"] = providerUserKey;
                        string hashKey = null;
                        drNewMbr["Password"] = this.EncodePassword(password, ref hashKey);
                        drNewMbr["PasswordSalt"] = hashKey;
                        drNewMbr["PasswordFormat"] = (int)this._pwFmt;
                        drNewMbr["Email"] = email;
                        drNewMbr["LoweredEmail"] = email.ToLower();
                        drNewMbr["PasswordQuestion"] = passwordQuestion;
                        drNewMbr["PasswordAnswer"] = this.EncodePassword(passwordAnswer, ref hashKey);
                        drNewMbr["IsApproved"] = isApproved;
                        drNewMbr["LastLoginDate"] = DateTime.MinValue;
                        drNewMbr["Comment"] = "";
                        drNewMbr["CreatedDate"] = createDate;
                        drNewMbr["LastPasswordChangedDate"] = createDate;
                        drNewMbr["IsLockedOut"] = false;
                        drNewMbr["LastLockedOutDate"] = DateTime.MinValue;
                        drNewMbr["FailedPasswordAttemptCount"] = 0;
                        drNewMbr["FailedPasswordAttemptWindowStart"] = createDate;
                        drNewMbr["FailedPasswordAnswerAttemptCount"] = 0;
                        drNewMbr["FailedPasswordAnswerAttemptWindowStart"] = createDate;

                        if (ds.Tables["Users"].LoadDataRow(drNewUser.ItemArray, false) == null || ds.Tables["Membership"].LoadDataRow(drNewMbr.ItemArray, false) == null)
                        {
                            ds.RejectChanges();
                            status = MembershipCreateStatus.UserRejected;
                            return null;
                        }
                        else
                            status = MembershipCreateStatus.Success;

                        this.SaveXml(ds);
                        return this.PopulateMembershipUser(drNewUser, drNewMbr);
                    }
                }
                catch (Exception ex)
                {
                    WriteEvent(ex, "Create User");
                    status = MembershipCreateStatus.ProviderError;
                }
            }
            else
            {
                status = MembershipCreateStatus.DuplicateUserName;
            }
            return null;
        }
        public override string GetPassword(string username, string answer)
        {
            // If the password format specifies that passwords are "Hashed", then
            //   there is no way to retrieve a user's password so we throw an
            //   Exception. This is the same behavior as the SqlMembershipProvider.
            if (this._pwFmt == MembershipPasswordFormat.Hashed)
                throw new MembershipPasswordException("Cannot retrieve password when PasswordFormat is set to 'Hashed'.");

            string sPW;
            try
            {
                using (DataSet ds = this.OpenXml())
                {
                    // Now we retrieve the user's password value.
                    sPW = this.GetUserPassValue(username, ds);
                }
            }
            catch (Exception ex)
            {
                this.WriteEvent(ex, "Get Password");
                throw;
            }

            return this.UnEncodePassword(sPW);
        }
        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            try
            {
                using (DataSet ds = this.OpenXml())
                {
                    DataRow drMbr = this.GetMembershipEntry(username, ds);

                    string oldPw = oldPassword;
                    if (!this.ValidateUser(username, oldPassword))
                        throw new ArgumentException("Supplied current password does not match password in membership XML file.", "oldPassword");

                    //if (this._pwFmt == MembershipPasswordFormat.Hashed)
                    //{
                    //    string pwHashType = Membership.HashAlgorithmType;
                    //    byte[] pwHash = null;
                    //    switch (pwHashType.ToUpper())
                    //    {
                    //        case "SHA1":
                    //            System.Security.Cryptography.SHA1 sha = System.Security.Cryptography.SHA1.Create();
                    //            pwHash = sha.ComputeHash(Encoding.Unicode.GetBytes(oldPw));
                    //            oldPw = Encoding.Unicode.GetString(pwHash);
                    //            break;
                    //        default:
                    //            throw new Exception("XmlMembershipProvider supports only SHA password hash algorythm.  Current value is " + pwHashType);
                    //    }
                    //}
                    //else
                    //    oldPw = oldPassword;

                    //string curPw = drMbr["Password"].ToString();
                    //if (oldPw != curPw)
                    //    throw new ArgumentException("Supplied current password does not match password in membership XML file.", "oldPassword");

                    string hashKey = drMbr["PasswordSalt"].ToString();
                    string newPw = this.EncodePassword(newPassword, ref hashKey);
                    //switch (this._pwFmt)
                    //{
                    //    case MembershipPasswordFormat.Clear:
                    //        newPw = newPassword;
                    //        break;
                    //    case MembershipPasswordFormat.Encrypted:
                    //        newPw = Encoding.Unicode.GetString(base.EncryptPassword(Encoding.Unicode.GetBytes(newPassword)));
                    //        break;
                    //    case MembershipPasswordFormat.Hashed:
                    //        System.Security.Cryptography.SHA1 sha = System.Security.Cryptography.SHA1.Create();
                    //        newPw = Encoding.Unicode.GetString(sha.ComputeHash(Encoding.Unicode.GetBytes(newPassword)));
                    //        break;
                    //}

                    drMbr["Password"] = newPw;
                    //ds.Tables["Membership"].LoadDataRow(drMbr.ItemArray, LoadOption.Upsert);

                    this.SaveXml(ds);
                }
                return true;
            }
            catch (Exception ex)
            {
                this.WriteEvent(ex, "Change Password");
                return false;
            }
        }
        public override string ResetPassword(string username, string answer)
        {
            try
            {
                if (!this.EnablePasswordReset)
                    throw new NotSupportedException("Password reset is not enabled.");

                using (DataSet ds = this.OpenXml())
                {
                    DataRow drUsr = null;
                    string
                        pwAnswer = string.Empty,
                        newPw = string.Empty;

                    try
                    {
                        drUsr = this.GetMembershipEntry(username, ds);

                        if (string.IsNullOrEmpty(answer) && this.RequiresQuestionAndAnswer)
                        {
                            this.UpdateFailureCount(drUsr, FailureType.PasswordAnswer, ds);
                            throw new ProviderException("Password answer is required for password reset.");
                        }

                        newPw = System.Web.Security.Membership.GeneratePassword(this.MinRequiredPasswordLength, this.MinRequiredNonAlphanumericCharacters);

                        ValidatePasswordEventArgs args =
                            new ValidatePasswordEventArgs(username, newPw, true);

                        OnValidatingPassword(args);

                        if (args.Cancel)
                            if (args.FailureInformation != null)
                                throw args.FailureInformation;
                            else
                                throw new MembershipPasswordException("Reset password canceled due to password validation failure.");


                        if (Convert.ToBoolean(drUsr["IsLockedOut"].ToString()))
                            throw new MembershipPasswordException("The specified user is locked out.");

                        pwAnswer = drUsr["PasswordAnswer"].ToString();
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message == "Selected user does not have a entry in the XML files's \"Membership\" section.")
                            throw new MembershipPasswordException("The specified user was not found.");
                        else
                            throw;
                    }

                    if (this.RequiresQuestionAndAnswer && !this.CheckPassword(answer, pwAnswer, drUsr["PasswordSalt"].ToString()))
                    {
                        this.UpdateFailureCount(drUsr, FailureType.PasswordAnswer, ds);
                        throw new MembershipPasswordException("Incorrect password answer.");
                    }

                    if (drUsr != null)
                    {
                        string hashKey = drUsr["PasswordSalt"].ToString();
                        drUsr["Password"] = this.EncodePassword(newPw, ref hashKey);
                        drUsr["LastPasswordChangedDate"] = DateTime.Now;
                        //ds.Tables["Membership"].LoadDataRow(drUsr.ItemArray, false);
                        this.SaveXml(ds);
                        return newPw;
                    }
                    else
                        throw new MembershipPasswordException("User not found, or user is locked out. Password not reset.");
                }
            }
            catch
            {
                throw;
            }
        }
        public override void UpdateUser(System.Web.Security.MembershipUser user)
        {
            DataRow
                drUsr = null,
                drMbr = null;
            try
            {
                DateTime dtNow = DateTime.Now;
                using (DataSet ds = this.OpenXml())
                {
                    drUsr = this.GetUserEntry(user.ProviderUserKey, ds);
                    drUsr.BeginEdit();
                    drUsr["LastActivityDate"] = dtNow;
                    //ds.Tables["Users"].LoadDataRow(drUsr.ItemArray, false);

                    drMbr = this.GetMembershipEntry(user.ProviderUserKey, ds);
                    drMbr.BeginEdit();
                    drMbr["Email"] = user.Email;
                    drMbr["IsApproved"] = user.IsApproved;
                    drMbr["IsLockedOut"] = user.IsLockedOut;
                    drMbr["Comment"] = user.Comment;
                    //ds.Tables["Membership"].LoadDataRow(drMbr.ItemArray, false);

                    drUsr.EndEdit();
                    drMbr.EndEdit();
                    this.SaveXml(ds);
                }
            }
            catch (Exception ex)
            {
                if (drUsr != null)
                    drUsr.CancelEdit();
                if (drMbr != null)
                    drMbr.CancelEdit();
                this.WriteEvent(ex, "Update User");
                throw;
            }
        }
        public override bool ValidateUser(string username, string password)
        {
            bool
                isValid = false,
                isApproved = false;
            string
                pwd = string.Empty;
            try
            {
                using (DataSet ds = this.OpenXml())
                {
                    DataRow dr = this.GetMembershipEntry(username, ds);
                    if (dr != null)
                    {
                        pwd = dr["Password"].ToString();
                        isApproved = Convert.ToBoolean(dr["IsApproved"].ToString());
                    }
                    else
                        return false;

                    if (isApproved)
                    {
                        if (this.CheckPassword(password, pwd, dr["PasswordSalt"].ToString()))
                        {
                            isValid = true;
                            dr["LastLoginDate"] = DateTime.Now;
                            //ds.Tables["Membership"].LoadDataRow(dr.ItemArray, false);
                            this.SaveXml(ds);
                        }
                        else
                        {
                            this.UpdateFailureCount(dr, FailureType.Password, ds);
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.WriteEvent(ex, "Validate User");
                return false;
            }
            return isValid;
        }
        public override bool UnlockUser(string userName)
        {
            try
            {
                using (DataSet ds = this.OpenXml())
                {
                    DataRow dr = this.GetMembershipEntry(userName, ds);
                    dr["IsLockedOut"] = false;

                    //if (ds.Tables["Membership"].LoadDataRow(dr.ItemArray, false) == null)
                    //    throw new ProviderException("Unable to update \"Membership\" table.");

                    this.SaveXml(ds);
                    return true;
                }
            }
            catch (Exception ex)
            {
                this.WriteEvent(ex, "Unlock User");
                return false;
            }
        }
        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            if (!ValidateUser(username, password))
                return false;

            try
            {
                using (DataSet ds = this.OpenXml())
                {
                    DataRow dr = this.GetMembershipEntry(username, ds);
                    string hashKey = dr["PasswordSalt"].ToString();
                    dr["PasswordQuestion"] = newPasswordQuestion;
                    dr["PasswordAnswer"] = this.EncodePassword(newPasswordAnswer, ref hashKey);

                    //if (ds.Tables["Membership"].LoadDataRow(dr.ItemArray, false) == null)
                    //    return false;

                    this.SaveXml(ds);
                    return true;
                }
            }
            catch (Exception ex)
            {
                this.WriteEvent(ex, "Change Password Question and Answer");
                return false;
            }
        }
        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            try
            {
                using (DataSet ds = this.OpenXml())
                {
                    DataRow drUsr = null;
                    try
                    {
                        drUsr = this.GetUserEntry(username, ds);
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message == "Selected username was not found.")
                            return null;
                        else
                            throw new Exception("Unable to retrieve user.", ex);
                    }
                    return this.PopulateMembershipUser(drUsr);
                }
            }
            catch
            { throw; }
        }
        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            try
            {
                using (DataSet ds = this.OpenXml())
                {
                    DataRow drUsr = null;
                    try
                    {
                        drUsr = this.GetUserEntry(providerUserKey, ds);
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message == "Selected username was not found.")
                            return null;
                        else
                            throw new Exception("Unable to retrieve user.", ex);
                    }
                    return this.PopulateMembershipUser(drUsr);
                }
            }
            catch
            { throw; }
        }
        public override string GetUserNameByEmail(string email)
        {
            try
            {
                using (DataSet ds = this.OpenXml())
                {
                    DataRow[] dr = ds.Tables["Membership"].Select(string.Format("Email = '{0}' AND ApplicationId = '{1}'", email, XmlMembershipProvider.GetAppID(this._appNm, ds)));
                    if (dr.Length > 0)
                    {
                        DataRow[] drUsr = ds.Tables["Users"].Select(string.Format("UserID = '{0}'", dr[0]["UserID"]));
                        if (drUsr.Length > 0)
                            return drUsr[0]["UserName"].ToString();
                        else
                            throw new ApplicationException("Email address found in Membership table, but no matching record for UserID value. Data integrity has been compromised!");
                    }
                    else
                        return string.Empty;
                        //throw new ProviderException("Specified email address was not found.");
                }
            }
            catch
            { throw; }
        }
        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            try
            {
                using (DataSet ds = this.OpenXml())
                {
                    DataRow dr = this.GetUserEntry(username, ds);
                    if (dr != null)
                    {
                        if (deleteAllRelatedData)
                        {
                            // Delete any profile data.
                            DataRow[] drProf = this.SearchByFieldValue(ds, "Profile", "UserName", dr["UserName"].ToString(), 0, 0);
                            for (int i = 0; i < drProf.Length; i++)
                                ds.Tables["Profile"].Rows.Remove(drProf[i]);

                            // Delete any role memberships.
                            DataRow[] drRoles = this.SearchByFieldValue(ds, "UsersInRoles", "UserId", dr["UserId"].ToString(), 0, 0);
                            for (int i = 0; i < drRoles.Length; i++)
                                ds.Tables["UsersInRoles"].Rows.Remove(drRoles[i]);
                        }

                        // Delete the user's membership information.
                        DataRow[] drMbrs = this.SearchByFieldValue(ds, "Membership", "UserId", dr["UserId"].ToString(), 0, 0);
                        for (int i = 0; i < drMbrs.Length; i++)
                            ds.Tables["Membership"].Rows.Remove(drMbrs[i]);

                        // Delete the user entry.
                        ds.Tables["Users"].Rows.Remove(dr);
                        this.SaveXml(ds);
                        return true;
                    }
                    else
                        return false;
                }
            }
            catch (Exception ex)
            {
                this.WriteEvent(ex, "Delete User");
                return false;
            }
        }
        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            try
            {
                MembershipUserCollection users = new MembershipUserCollection();
                using (DataSet ds = this.OpenXml())
                {
                    StringCollection usrNms = new StringCollection();
                    for (int i = 0; i < ds.Tables["Users"].Rows.Count; i++)
                        usrNms.Add(ds.Tables["Users"].Rows[i]["LoweredUserName"].ToString(), i.ToString());
                    usrNms.Sort(Collections.SortDirection.Ascending);
                    for (int i = 0; i < usrNms.Count; i++)
                    {
                        DataRow drUsr = ds.Tables["Users"].Rows[int.Parse(usrNms.GetKey(i))];
                        users.Add(this.PopulateMembershipUser(drUsr, this.GetMembershipEntry(drUsr["UserId"], ds)));
                    }
                    //for (int i = 0; i < ds.Tables["Users"].Rows.Count; i++)
                    //    users.Add(this.PopulateMembershipUser(ds.Tables["Users"].Rows[i], this.GetMembershipEntry(ds.Tables["Users"].Rows[i]["UserId"], ds)));
                    totalRecords = ds.Tables["Users"].Rows.Count;
                }
                return users;
            }
            catch
            { throw; }
        }
        public override int GetNumberOfUsersOnline()
        {
            TimeSpan onlineSpan = new TimeSpan(0, System.Web.Security.Membership.UserIsOnlineTimeWindow, 0);
            DateTime compareTime = DateTime.Now.Subtract(onlineSpan);

            try
            {
                using (DataSet ds = this.OpenXml())
                {
                    DataRow[] drCnt = ds.Tables["Users"].Select(string.Format("LastActivityDate > '{0}' AND ApplicationName = '{1}'", compareTime, this._appNm));
                    return drCnt.Length;
                }
            }
            catch
            { throw; }
        }
        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            MembershipUserCollection users = new MembershipUserCollection();
            try
            {
                using (DataSet ds = this.OpenXml())
                {
                    DataRow[] dr = this.SearchByFieldValue(ds, "Users", "Username", usernameToMatch, pageIndex, pageSize);
                    StringCollection usrNms = new StringCollection();
                    for (int i = 0; i < dr.Length; i++)
                        usrNms.Add(dr[i]["LoweredUserName"].ToString(), i.ToString());
                    usrNms.Sort(Collections.SortDirection.Ascending);
                    for (int i = 0; i < usrNms.Count; i++)
                    {
                        DataRow drUsr = dr[int.Parse(usrNms.GetKey(i))];
                        users.Add(this.PopulateMembershipUser(drUsr, this.GetMembershipEntry(drUsr["UserId"], ds)));
                    }
                    //for (int i = 0; i < dr.Length; i++)
                    //    users.Add(this.PopulateMembershipUser(dr[i], this.GetMembershipEntry(dr[i]["UserId"], ds)));
                    totalRecords = ds.Tables["Users"].Rows.Count;
                }
            }
            catch
            { throw; }

            return users;
        }
        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            MembershipUserCollection users = new MembershipUserCollection();
            try
            {
                using (DataSet ds = this.OpenXml())
                {
                    DataRow[] dr = this.SearchByFieldValue(ds, "Membership", "Email", emailToMatch, pageIndex, pageSize);
                    for (int i = 0; i < dr.Length; i++)
                        users.Add(this.PopulateMembershipUser(this.GetUserEntry(dr[i]["UserId"], ds), dr[i]));
                    totalRecords = ds.Tables["Users"].Rows.Count;
                }
            }
            catch
            { throw; }

            return users;
        }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        private string GetConfigValue(string configValue, object defaultValue)
        {
            if (string.IsNullOrEmpty(configValue))
                return defaultValue.ToString();

            return configValue;
        }
        private DataSet OpenXml()
        {
            this._mut.WaitOne();
            try
            {
                DataSet ds = new DataSet();
                //using (UnmanagedMemoryStream xsdStrm = global::ElementControls.Properties.Resources.ResourceManager.GetStream("XmlMembership"))
                //    ds.ReadXmlSchema(xsdStrm);
                string xsdSchema = (string)global::RainstormStudios.Web.Properties.Resources.ResourceManager.GetObject("XmlMembership");
                using (MemoryStream xsdStrm = new MemoryStream(Encoding.ASCII.GetBytes(xsdSchema)))
                    ds.ReadXmlSchema(xsdStrm);
                if (System.IO.File.Exists(this.XmlPath))
                    using (XmlTextReader xmlRdr = new XmlTextReader(this.XmlPath))
                        ds.ReadXml(xmlRdr, XmlReadMode.IgnoreSchema);

                // Setup PrimaryKey information.
                ds.Tables["Users"].PrimaryKey = new DataColumn[] { ds.Tables["Users"].Columns["UserId"], ds.Tables["Users"].Columns["ApplicationId"] };
                ds.Tables["Profile"].PrimaryKey = new DataColumn[] { ds.Tables["Profile"].Columns["UserId"], ds.Tables["Profile"].Columns["ApplicationName"], ds.Tables["Profile"].Columns["PropertyName"] };
                ds.Tables["Roles"].PrimaryKey = new DataColumn[] { ds.Tables["Roles"].Columns["ApplicationId"], ds.Tables["Roles"].Columns["RoleID"] };
                ds.Tables["UsersInRoles"].PrimaryKey = new DataColumn[] { ds.Tables["UsersInRoles"].Columns["RoleId"], ds.Tables["UsersInRoles"].Columns["UserId"], ds.Tables["UsersInRoles"].Columns["ApplicationId"] };
                ds.Tables["Applications"].PrimaryKey = new DataColumn[] { ds.Tables["Applications"].Columns["ApplicationId"] };
                ds.Tables["Membership"].PrimaryKey = new DataColumn[] { ds.Tables["Membership"].Columns["ApplicationId"], ds.Tables["Membership"].Columns["UserId"] };
                ds.Tables["Paths"].PrimaryKey = new DataColumn[] { ds.Tables["Paths"].Columns["ApplicationID"], ds.Tables["Paths"].Columns["PathId"] };

                return ds;
            }
            catch
            { throw; }
            finally
            { this._mut.ReleaseMutex(); }
        }
        private void SaveXml(DataSet ds)
        {
            this._mut.WaitOne();
            try
            {
                using (FileStream fs = new FileStream(this.XmlPath, FileMode.Create, FileAccess.Write))
                using (XmlTextWriter tr = new XmlTextWriter(fs, Encoding.UTF8))
                {
                    tr.Formatting = Formatting.Indented;
                    tr.Indentation = 2;
                    tr.IndentChar = ' ';
                    ds.WriteXml(tr, XmlWriteMode.IgnoreSchema);
                }
                ds.AcceptChanges();
            }
            catch
            { throw; }
            finally
            { this._mut.ReleaseMutex(); }
        }
        private string GetUserId(string username, DataSet ds)
        {
            try
            {
                //DataRow drUsr = this.GetUserEntry(username, ds);
                DataRow[] drUsr = ds.Tables["Users"].Select("UserName = '" + username + "' AND ApplicationId = '" + XmlMembershipProvider.GetAppID(this._appNm, ds) + "'");
                if (drUsr.Length < 1)
                    throw new Exception("Selected username was not found.");
                else if (drUsr.Length > 1)
                    throw new Exception("Selected user returned more than one result from membership XML. XML file integrity may have been compromised.");
                return drUsr[0]["UserId"].ToString();
            }
            catch
            { throw; }
        }
        private string GetUserPassValue(string username, DataSet ds)
        {
            try
            {
                DataRow drPass = this.GetMembershipEntry(username, ds);
                return drPass["Password"].ToString();
            }
            catch
            { throw; }
        }
        private DataRow GetUserEntry(string username, DataSet ds)
        {
            try
            {
                string usrId = this.GetUserId(username, ds);
                return this.GetUserEntry(new Guid(usrId), ds);
            }
            catch
            { throw; }
        }
        private DataRow GetUserEntry(object providerUserKey, DataSet ds)
        {
            try
            {
                DataRow[] drUsr = ds.Tables["Users"].Select("UserId = '" + providerUserKey.ToString() + "' AND ApplicationId = '" + XmlMembershipProvider.GetAppID(this._appNm, ds) + "'");
                if (drUsr.Length < 1)
                    throw new Exception("Selected username was not found.");
                else if (drUsr.Length > 1)
                    throw new Exception("Selected user returned more than one result from membership XML. XML file integrity may have been compromised.");
                return drUsr[0];
            }
            catch
            { throw; }
        }
        private DataRow GetMembershipEntry(string username, DataSet ds)
        {
            try
            {
                string usrId = this.GetUserId(username, ds);
                return this.GetMembershipEntry(new Guid(usrId), ds);
            }
            catch
            { throw; }
        }
        private DataRow GetMembershipEntry(object providerUserKey, DataSet ds)
        {
            try
            {
                DataRow[] drMbr = ds.Tables["Membership"].Select("UserId = '" + providerUserKey.ToString() + "' AND ApplicationId = '" + XmlMembershipProvider.GetAppID(this._appNm, ds) + "'");
                if (drMbr.Length < 1)
                    throw new Exception("Selected user does not have a entry in the XML files's \"Membership\" section. XML file integrity has been compromised.");
                else if (drMbr.Length > 1)
                    throw new Exception("Selected user matches multple entries in the XML file's \"Membership\" section. XML file integrity has been compromised.");
                return drMbr[0];
            }
            catch
            { throw; }
        }
        private MembershipUser PopulateMembershipUser(DataRow drUser)
        {
            DataRow drMbr = this.GetMembershipEntry(drUser["UserId"], drUser.Table.DataSet);
            return this.PopulateMembershipUser(drUser, drMbr);
        }
        private MembershipUser PopulateMembershipUser(DataRow drUser, DataRow drMembership)
        {
            return new MembershipUser(
                    this.Name,
                    drUser["Username"].ToString(),
                    drUser["UserId"],
                    drMembership["Email"].ToString(),
                    drMembership["PasswordQuestion"].ToString(),
                    drMembership["Comment"].ToString(),
                    Convert.ToBoolean(drMembership["IsApproved"].ToString()),
                    Convert.ToBoolean(drMembership["IsLockedOut"].ToString()),
                    DateTime.Parse(drMembership["CreatedDate"].ToString()),
                    DateTime.Parse(drMembership["LastLoginDate"].ToString()),
                    DateTime.Parse(drUser["LastActivityDate"].ToString()),
                    DateTime.Parse(drMembership["LastPasswordChangedDate"].ToString()),
                    DateTime.Parse(drMembership["LastLockedOutDate"].ToString()));
        }
        private bool CheckPassword(string password, string dbpassword, string hashSalt)
        {
            string
                pass1 = password,
                pass2 = dbpassword;

            switch (this.PasswordFormat)
            {
                case MembershipPasswordFormat.Encrypted:
                    pass2 = this.UnEncodePassword(dbpassword);
                    break;
                case MembershipPasswordFormat.Hashed:
                    pass1 = this.EncodePassword(password, ref hashSalt);
                    break;
                default:
                    break;
            }

            return (pass1 == pass2);
        }
        private string EncodePassword(string password, ref string hashSalt)
        {
            string encodedPassword = string.Empty;

            switch (this.PasswordFormat)
            {
                case MembershipPasswordFormat.Clear:
                    encodedPassword = password;
                    break;
                case MembershipPasswordFormat.Encrypted:
                    encodedPassword=
                        Convert.ToBase64String(this.EncryptPassword(Encoding.Unicode.GetBytes(password)));
                    break;
                case MembershipPasswordFormat.Hashed:
                    //if (!Membership.HashAlgorithmType.EndsWith("SHA1"))
                    //    throw new ProviderException("XmlMembershipProvider only supports SHA1 hashing algorythm.");
                    System.Security.Cryptography.HMACSHA1 hash = new System.Security.Cryptography.HMACSHA1();
                    if (this._machKey != null && !this._machKey.ValidationKey.Contains("AutoGenerate") && this._machKey.Validation == MachineKeyValidation.SHA1)
                        hash.Key = RainstormStudios.Hex.GetBytes(this._machKey.ValidationKey);
                    else if (hashSalt != null)
                        hash.Key = Convert.FromBase64String(hashSalt);
                    else
                        hashSalt = Convert.ToBase64String(hash.Key);
                    encodedPassword =
                        Convert.ToBase64String(hash.ComputeHash(Encoding.Unicode.GetBytes(password)));
                    break;
                default:
                    throw new ProviderException("Unsupported password format.");
            }
            return encodedPassword;
        }
        private string UnEncodePassword(string encodedPassword)
        {
            string password = string.Empty;

            switch (this.PasswordFormat)
            {
                case MembershipPasswordFormat.Clear:
                    password = encodedPassword;
                    break;
                case MembershipPasswordFormat.Encrypted:
                    password =
                        Encoding.Unicode.GetString(this.DecryptPassword(Convert.FromBase64String(encodedPassword)));
                    break;
                case MembershipPasswordFormat.Hashed:
                    throw new ProviderException("Cannot unencode a hashed password.");
                default:
                    throw new ProviderException("Unsupported password format.");
            }
            return password;
        }
        private DataRow[] SearchByFieldValue(DataSet ds, string table, string fieldName, string fieldValue, int pageIndex, int pageSize)
        {
            try
            {
                List<DataRow> rVals = new List<DataRow>();

                StringBuilder sbQry = new StringBuilder("{2} LIKE '{0}' AND ");
                if (table.ToLower() == "profile")
                    sbQry.Append("ApplicationName");
                else
                    sbQry.Append("ApplicationId");
                sbQry.Append(" = '{1}'");

                DataRow[] dr = ds.Tables[table].Select(string.Format(sbQry.ToString(), fieldValue, (table.ToLower() == "profile") ? this.ApplicationName : XmlMembershipProvider.GetAppID(this._appNm, ds), fieldName));
                int startIndex = pageSize * pageIndex,
                    endIndex = startIndex + pageSize - 1;

                for (int i = startIndex; (i < endIndex || (pageIndex == 0 && pageSize == 0)) && i < dr.Length; i++)
                    rVals.Add(dr[i]);

                return rVals.ToArray();
            }
            catch
            { throw; }
        }
        private void UpdateFailureCount(DataRow membershipRecord, FailureType failureType, DataSet ds)
        {
            DataRow dr = membershipRecord;
            DateTime windowStart = new DateTime();
            int failureCount = 0;
            try
            {
                switch (failureType)
                {
                    case FailureType.Password:
                        failureCount = int.Parse(dr["FailedPasswordAttemptCount"].ToString());
                        windowStart = DateTime.Parse(dr["FailedPasswordAttemptWindowStart"].ToString());
                        break;
                    case FailureType.PasswordAnswer:
                        failureCount = int.Parse(dr["FailedPasswordAnswerAttemptCount"].ToString());
                        windowStart = DateTime.Parse(dr["FailedPasswordAnswerAttemptWindowStart"].ToString());
                        break;
                }

                DateTime windowEnd = windowStart.AddMinutes(PasswordAttemptWindow);

                if (failureCount == 0 || DateTime.Now > windowEnd)
                {
                    // First password failure or outside of PasswordAttemptWindow.
                    // Start a new password failure count from 1 and a new window
                    //   starting now.

                    if (failureType == FailureType.Password)
                    {
                        dr["PasswordAttemptCount"] = 1;
                        dr["PasswordAttemptWindowStart"] = DateTime.Now;
                    }
                    else if (failureType == FailureType.PasswordAnswer)
                    {
                        dr["PasswordAnswerAttemptCount"] = 1;
                        dr["PasswordAnswerAttemptWindowStart"] = DateTime.Now;
                    }
                }
                else
                {
                    if (failureCount++ >= this.MaxInvalidPasswordAttempts)
                    {
                        // Password attempts have exceeded the failure threshold.
                        //   Lock out the user.
                        dr["IsLockedOut"] = true;
                    }
                    else
                    {
                        // Password attempts have not exceeded the failure
                        //   threshold. Update the failure counts. Leave the
                        //   window the same.
                        if (failureType == FailureType.Password)
                            dr["PasswordAttemptCount"] = failureCount;

                        else if (failureType == FailureType.PasswordAnswer)
                            dr["PasswordAnswerAttemptCount"] = failureCount;
                    }
                }
                //ds.Tables["Membership"].LoadDataRow(membershipRecord.ItemArray, false);
                this.SaveXml(ds);
            }
            catch
            {
                throw;
            }
        }
        private void WriteEvent(Exception ex, string action)
        {
            if (this._wrtEvntLog)
            {
                try
                {
                    System.Diagnostics.EventLog log = new System.Diagnostics.EventLog();
                    log.Source = eventSource;
                    log.Log = eventLog;

                    if (!System.Diagnostics.EventLog.SourceExists(eventSource))
                        System.Diagnostics.EventLog.CreateEventSource(eventSource, eventLog);

                    string msg = "An exception occured communicating with the data source.\n\n";
                    msg += "Action: " + action + "\n\n";
                    msg += "Exception: " + ex.ToString();

                    log.WriteEntry(msg, System.Diagnostics.EventLogEntryType.Error);
                }
                catch (Exception ex2)
                {
                    throw new Exception("Unable to write exception to event log.", ex2);
                }
            }
        }
        //***************************************************************************
        // Static Methods
        // 
        internal static string GetAppID(string appName, DataSet ds)
        {
            try
            {
                DataRow[] drApp = ds.Tables["Applications"].Select(string.Format("LoweredApplicationName = '{0}'", appName.ToLower()));
                if (drApp.Length > 1)
                    // If the application name exists multiple times in the XML file,
                    //   then something has gone wrong.
                    throw new ProviderException("Specified application name appears more than once in XML.  File integrity compromised!");
                else if (drApp.Length < 1)
                {
                    Guid appGuid = Guid.NewGuid();
                    DataRow drNewApp = ds.Tables["Applications"].NewRow();
                    drNewApp["ApplicationName"] = appName;
                    drNewApp["ApplicationId"] = appGuid;
                    drNewApp["LoweredApplicationName"] = appName.ToLower();
                    ds.Tables["Applications"].Rows.Add(drNewApp);
                    return appGuid.ToString();

                    // ! ! ! NOTE ! ! !
                    // We don't save here, because if we're permforming a data update
                    //   to one of the other tables, the calling method will save
                    //   the XML changes. And if we're not running a data-updating
                    //   method, then it doesn't matter anyway.
                }
                else
                    return drApp[0]["ApplicationId"].ToString();
            }
            catch
            { throw; }
        }
        #endregion
    }
}
