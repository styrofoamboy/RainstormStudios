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
using System.Xml;
using System.Data;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Configuration.Provider;

namespace RainstormStudios.Web.Membership
{
    [Author("Michael Unfried")]
    public sealed class XmlRoleProvider : System.Web.Security.RoleProvider
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        const string
            eventSource = "XmlRoleProvider",
            eventLog = "Application";
        private bool
            _wrtEvntLog;
        private string
            _xmlFn,
            _appName;
        private System.Threading.Mutex
            _mut;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public override string ApplicationName
        {
            get { return this._appName; }
            set { this._appName = value; }
        }
        public string XMLFileName
        {
            get { return this._xmlFn; }
            set { this._xmlFn = value; }
        }
        public bool WriteToEventLog
        {
            get { return this._wrtEvntLog; }
            set { this._wrtEvntLog = value; }
        }
        //***************************************************************************
        // Private Properties
        // 
        private string XmlPath
        { get { return System.Web.HttpContext.Current.Server.MapPath(this._xmlFn); } }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public XmlRoleProvider()
            : base()
        { }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            if (config == null)
                throw new ArgumentNullException("config");

            this._mut = new System.Threading.Mutex(false, "XmlProvider");

            if (string.IsNullOrEmpty(name))
                name = "XmlRoleProvider";

            if (string.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "XML Role Provider");
            }

            // Initialize the abstract base class.
            base.Initialize(name, config);

            this._appName = (config["applicationName"] == null || config["applicationName"].Trim() == "")
                                ? System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath
                                : config["applicationName"];

            if (config["writeExceptionsToEventLog"] != null)
            {
                bool write = true;
                if (!Boolean.TryParse(config["writeExceptionsToEventLog"], out write))
                    throw new ArgumentException("writeExceptionsToEventLog: Could not parse to a valid boolean value.");
                this._wrtEvntLog = write;
            }

            this._xmlFn = this.GetConfigValue(config["xmlFile"], "XmlMembership.xml");
            if (this._xmlFn == Path.GetFileName(this._xmlFn))
                this._xmlFn = Path.Combine(System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath, this._xmlFn);

            //this._xmlFn = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + config["xmlFile"].TrimStart('~', '\\', '/').Replace('/', '\\');
            //this._xmlFn = config["xmlFile"].TrimStart('~', '\\', '/').Replace('/', '\\');
            //this._xmlFn = !string.IsNullOrEmpty(config["xmlFile"].ToString())
            //        ? config["xmlFile"].ToString()
            //        : Path.Combine(System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath, "XmlRoleProvider.xml");

            //if (string.IsNullOrEmpty(config["xmlFile"]))
            //    throw new ArgumentNullException("Filename cannot be blank", "xmlFile");
        }
        public void AddUserToRole(string username, string rolename)
        {
            this.AddUsersToRoles(new string[] { username }, new string[] { rolename });
        }
        public void AddUsersToRole(string[] usernames, string rolename)
        {
            this.AddUsersToRoles(usernames, new string[] { rolename });
        }
        public void AddUserToRoles(string username, string[] rolenames)
        {
            this.AddUsersToRoles(new string[] { username }, rolenames);
        }
        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            try
            {
                using (DataSet ds = this.OpenXml())
                {
                    foreach (string roleNm in roleNames)
                    {
                        if (!RoleExists(roleNm, ds))
                            throw new ProviderException("Role name not found: " + roleNm);
                        foreach (string userNm in usernames)
                        {
                            if (userNm.Contains(","))
                                throw new ArgumentException("User names cannot contain commas.");

                            if (IsUserInRole(userNm, roleNm, ds))
                                throw new ProviderException(string.Format("User '{0}' is already in role '{1}'.", userNm, roleNm));
                        }
                    }


                    try
                    {
                        object appID = this.GetApplicationID(this._appName, ds);
                        foreach (string roleNm in roleNames)
                        {
                            object roleID = this.GetRoleID(roleNm, ds);
                            foreach (string userNm in usernames)
                            {
                                DataRow drNew = ds.Tables["UsersInRoles"].NewRow();
                                drNew["RoleID"] = roleID;
                                drNew["UserID"] = this.GetUserID(userNm, ds);
                                drNew["ApplicationID"] = appID;
                                ds.Tables["UsersInRoles"].Rows.Add(drNew);
                            }
                        }
                        this.SaveXml(ds);
                    }
                    catch (Exception ex)
                    {
                        ds.RejectChanges();
                        if (this._wrtEvntLog)
                            this.WriteEvent(ex, "AddUsersToRoles");
                        else
                            throw;
                    }
                }
            }
            catch (IOException ex)
            {
                if (this._wrtEvntLog)
                    this.WriteEvent(ex, "AddUsersToRole");
                else
                    throw;
            }
            catch
            { throw; }
        }
        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            try
            {
                using (DataSet ds = this.OpenXml())
                {
                    if (!RoleExists(roleName, ds))
                        throw new ProviderException("Role does not exist.");

                    if (throwOnPopulatedRole && GetUsersInRole(roleName, ds).Length > 0)
                        throw new ProviderException("Cannot delete a populated role.");

                    try
                    {
                        DataRow[] dr = ds.Tables["Roles"].Select(string.Format("RoleID = '{0}' AND ApplicationID = '{1}'", this.GetRoleID(roleName, ds), this.GetApplicationID(this._appName, ds)));
                        for (int i = 0; i < dr.Length; i++)
                            ds.Tables["Roles"].Rows.Remove(dr[i]);
                        this.SaveXml(ds);
                    }
                    catch (Exception ex)
                    {
                        ds.RejectChanges();
                        if (this._wrtEvntLog)
                        {
                            this.WriteEvent(ex, "DeleteRole");
                            return false;
                        }
                        else
                            throw;
                    }
                    return true;
                }
            }
            catch (IOException ex)
            {
                if (this._wrtEvntLog)
                    this.WriteEvent(ex, "DeleteRole");
                else
                    throw;
            }
            catch
            {
                throw;
            }
            return false;
        }
        public override string[] GetAllRoles()
        {
            List<string> roles = new List<string>();
            try
            {
                using (DataSet ds = this.OpenXml())
                {
                    DataRow[] dr = ds.Tables["Roles"].Select(string.Format("ApplicationID = '{0}'", this.GetApplicationID(this._appName, ds)));
                    for (int i = 0; i < dr.Length; i++)
                        roles.Add(dr[i]["Rolename"].ToString());
                    return roles.ToArray();
                }
            }
            catch (Exception ex)
            {
                if (this._wrtEvntLog)
                    this.WriteEvent(ex, "GetAllRoles");
                else
                    throw;
            }
            return new string[0];
        }
        public override string[] GetRolesForUser(string username)
        {
            List<string> roles = new List<string>();
            try
            {
                using (DataSet ds = this.OpenXml())
                {
                    object usrID = this.GetUserID(username, ds);
                    if (usrID == null)
                        throw new ProviderException("Specified username not found in database.");

                    DataRow[] dr = ds.Tables["UsersInRoles"].Select(string.Format("UserID = '{0}' AND ApplicationID = '{1}'", usrID, this.GetApplicationID(this._appName, ds)));
                    for (int i = 0; i < dr.Length; i++)
                    {
                        DataRow[] drRole = ds.Tables["Roles"].Select(string.Format("RoleID = '{0}'", dr[i]["RoleID"]));
                        if (drRole.Length > 0)
                            roles.Add(drRole[0]["RoleName"].ToString());
                        else
                            throw new ProviderException("RoleID inconsistancy detected: " + dr[i]["RoleID"].ToString());
                    }
                }
                return roles.ToArray();
            }
            catch (Exception ex)
            {
                if (this._wrtEvntLog)
                    this.WriteEvent(ex, "GetRolesForUser");
                else
                    throw;
            }
            return new string[0];
        }
        public override string[] GetUsersInRole(string roleName)
        {
            List<string> users = new List<string>();
            try
            {
                using (DataSet ds = this.OpenXml())
                {
                    object appID = this.GetApplicationID(this._appName, ds);
                    DataRow[] dr = ds.Tables["UsersInRoles"].Select(string.Format("RoleID = '{0}' AND ApplicationID = '{1}", this.GetRoleID(roleName, ds), appID));
                    for (int i = 0; i < dr.Length; i++)
                    {
                        DataRow[] drUsr = ds.Tables["Users"].Select(string.Format("UserID = '{0}' AND ApplicationID = '{1}'", dr[i]["UserID"], appID));
                        if (drUsr.Length > 0)
                            users.Add(drUsr[0]["UserName"].ToString());
                        else
                            throw new ProviderException("UserID inconsistancy detected: " + dr[i]["UserID"].ToString());
                    }
                    return users.ToArray();
                }
            }
            catch (Exception ex)
            {
                if (this._wrtEvntLog)
                    this.WriteEvent(ex, "GetUsersInRole");
                else
                    throw;
            }
            return new string[0];
        }
        public override bool IsUserInRole(string username, string roleName)
        {
            try
            {
                using (DataSet ds = this.OpenXml())
                    return this.IsUserInRole(username, roleName, ds);
            }
            catch (Exception ex)
            {
                if (this._wrtEvntLog)
                    this.WriteEvent(ex, "IsUserInRole");
                else
                    throw;
            }
            return false;
        }
        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            try
            {

                using (DataSet ds = this.OpenXml())
                {
                    foreach (string roleNm in roleNames)
                    {
                        if (!RoleExists(roleNm, ds))
                            throw new ProviderException("Role name not found.");
                        foreach (string userNm in usernames)
                            if (!IsUserInRole(userNm, roleNm))
                                throw new ProviderException(string.Format("User '{0}' is not in role '{1}'.", userNm, roleNm));
                    }

                    try
                    {
                        object appID = this.GetApplicationID(this._appName, ds);
                        foreach (string roleNm in roleNames)
                        {
                            object roleID = this.GetRoleID(roleNm, ds);
                            foreach (string userNm in usernames)
                            {
                                DataRow[] dr = ds.Tables["UsersInRoles"].Select(string.Format("RoleID = '{0}' AND UserID = '{1}' AND ApplicationID = '{2}'", roleID, this.GetUserID(userNm, ds), appID));
                                for (int i = 0; i < dr.Length; i++)
                                    ds.Tables["UsersInRoles"].Rows.Remove(dr[i]);
                            }
                        }
                        this.SaveXml(ds);
                    }
                    catch (Exception ex)
                    {
                        if (this._wrtEvntLog)
                            this.WriteEvent(ex, "RemoveUsersFromRole");
                        else
                            throw;
                    }
                }
            }
            catch (IOException ex)
            {
                if (this._wrtEvntLog)
                    this.WriteEvent(ex, "RemoveUsersFromRoles");
                else
                    throw;
            }
            catch
            {
                throw;
            }
        }
        public override bool RoleExists(string roleName)
        {
            try
            {
                using (DataSet ds = this.OpenXml())
                    return this.RoleExists(roleName, ds);
            }
            catch (Exception ex)
            {
                if (this._wrtEvntLog)
                    this.WriteEvent(ex, "CheckRoleExists");
                else
                    throw;
            }
            return false;
        }
        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            List<string> users = new List<string>();
            try
            {
                using (DataSet ds = this.OpenXml())
                {
                    DataRow[] drUsrs = ds.Tables["Users"].Select(string.Format("UserName LIKE '%{0}%' AND ApplicationID = '{1}'", usernameToMatch, this.GetApplicationID(this._appName, ds)));
                    for (int i = 0; i < drUsrs.Length; i++)
                        if (this.IsUserInRole(drUsrs[i]["UserName"].ToString(), roleName))
                            users.Add(drUsrs[i]["UserName"].ToString()); 
                    return users.ToArray();
                }
            }
            catch (Exception ex)
            {
                if (this._wrtEvntLog)
                    this.WriteEvent(ex, "FindUsersInRole");
                else
                    throw;
            }
            return new string[0];
        }
        public override void CreateRole(string roleName)
        {
            this.CreateRole(roleName, string.Empty);
        }
        public void CreateRole(string roleName, string description)
        {
            if (roleName.Contains(","))
                throw new ArgumentException("Role names cannot contain commas.");

            try
            {
                using (DataSet ds = this.OpenXml())
                {
                    if (RoleExists(roleName))
                        throw new ProviderException("Role name already exists.");

                    DataRow drNew = ds.Tables["Roles"].NewRow();
                    drNew["RoleId"] = Guid.NewGuid();
                    drNew["RoleName"] = roleName;
                    drNew["LoweredRoleName"] = roleName.ToLower();
                    drNew["ApplicationId"] = this.GetApplicationID(this._appName, ds);
                    drNew["Description"] = description;
                    ds.Tables["Roles"].Rows.Add(drNew);
                    this.SaveXml(ds);
                }
            }
            catch (System.IO.IOException ex)
            {
                if (this._wrtEvntLog)
                    this.WriteEvent(ex, "CreateRole");
                else
                    throw;
            }
            catch
            {
                throw;
            }
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
        private bool RoleExists(string roleName, DataSet ds)
        {
            DataRow[] dr = ds.Tables["Roles"].Select(string.Format("RoleID = '{0}' AND ApplicationID = '{1}'", this.GetRoleID(roleName, ds), this.GetApplicationID(this._appName, ds)));
            return (dr.Length > 0);
        }
        private bool IsUserInRole(string userName, string roleName, DataSet ds)
        {
            DataRow[] dr = ds.Tables["UsersInRoles"].Select(string.Format("RoleID = '{0}' AND UserID = '{1}' AND ApplicationID = '{2}'", this.GetRoleID(roleName, ds), this.GetUserID(userName, ds), this.GetApplicationID(this._appName, ds)));
            return (dr.Length > 0);
        }
        private string[] GetUsersInRole(string roleName, DataSet ds)
        {
            object appID = this.GetApplicationID(this._appName, ds);
            List<string> users = new List<string>();
            DataRow[] dr = ds.Tables["UsersInRole"].Select(string.Format("RoleID = '{0}' AND ApplicationID = '{1}'", this.GetRoleID(roleName, ds), appID)); 
            for (int i = 0; i < dr.Length; i++)
            {
                DataRow[] drUsr = ds.Tables["Users"].Select(string.Format("UserID = '{0}' AND ApplicationID = '{1}'", dr[i]["UserID"], appID));
                if (drUsr.Length > 0)
                    users.Add(drUsr[0]["UserName"].ToString());
            } 
            return users.ToArray();
        }
        private void WriteEvent(Exception ex, string action)
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
                throw new ApplicationException("Unable to write to event log.", new ProviderException(ex2.Message, ex));
            }
        }
        private object GetRoleID(string roleName, DataSet ds)
        {
            DataRow[] dr = ds.Tables["Roles"].Select(string.Format("LoweredRolename = '{0}'", roleName.ToLower()));
            if (dr.Length < 1)
                return null;
            else
                return dr[0]["RoleID"];
        }
        private object GetApplicationID(string appName, DataSet ds)
        {
            //DataRow[] dr = ds.Tables["Applications"].Select(string.Format("LoweredApplicationName = '{0}'", appName.ToLower()));
            //if (dr.Length < 1)
            //    return null;
            //else
            //    return dr[0]["ApplicationId"];
            return XmlMembershipProvider.GetAppID(appName, ds);
        }
        private object GetUserID(string userName, DataSet ds)
        {
            DataRow[] dr = ds.Tables["Users"].Select(string.Format("LoweredUserName = '{0}'", userName.ToLower()));
            if (dr.Length < 1)
                return null;
            else
                return dr[0]["UserId"];
        }
        #endregion
    }
}
