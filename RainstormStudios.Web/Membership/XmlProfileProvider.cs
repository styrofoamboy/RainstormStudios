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
using System.Configuration;
using System.Configuration.Provider;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Permissions;
using System.Text;
using System.Web;

namespace RainstormStudios.Web.Membership
{
    [SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.SerializationFormatter)]
    public sealed class XmlProfileProvider : System.Web.Profile.ProfileProvider
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private string
            _appName,
            _eventSrc = "XmlProfileProvider",
            _eventLog = "Application",
            _exMsg = "An exception occurred. Please check the event log.",
            _xmlFile;
        private bool
            _useEventLog;
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
            get { return this._xmlFile; }
            set { this._xmlFile = value; }
        }
        public bool WriteToEventLog
        {
            get { return this._useEventLog; }
            set { this._useEventLog = value; }
        }
        //***************************************************************************
        // Private Properties
        // 
        private string XmlPath
        { get { return HttpContext.Current.Server.MapPath(this._xmlFile); } }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public override void Initialize(string name, NameValueCollection config)
        {
            // If no configuration values were passed, throw an exception.
            if (config == null)
                throw new ArgumentNullException("config");

            this._mut = new System.Threading.Mutex(false, "XmlProvider");

            // Give the Provider a default name, if one was not specified.
            if (string.IsNullOrEmpty(name))
                name = "XmlProfileProvider";

            // Determine the Provider's "Description" property.
            if (string.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "XML Profile Provider Instance");
            }

            // Initialize the abstract base class.  Once called, this cannot be
            //   called a second time.
            base.Initialize(name, config);

            // Initialize the application name.
            this._appName = (config["applicationname"] == null || config["applicationname"].Trim() == "")
                                ? System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath
                                : config["applicationname"];

            // Determine if exceptions should be written to the event log.
            bool write = true;
            if (!string.IsNullOrEmpty(config["writeExceptionsToEventLog"]))
                if (!Boolean.TryParse(config["writeExceptionsToEventLog"], out write))
                    throw new ArgumentException("writeExceptionsToEventLog: Could not parse to a valid boolean value.");
            this._useEventLog = write;

            // Initialize the XML file name value.
            this._xmlFile = GetConfigValue(config["xmlFile"], "XmlMembership.xml");

            // Make sure we have permission to read/write to the XML file.
            FileIOPermission permission = new FileIOPermission(FileIOPermissionAccess.AllAccess, this.XmlPath);
            permission.Demand();
        }
        public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext context, SettingsPropertyCollection collection)
        {
            SettingsPropertyValueCollection settings = new SettingsPropertyValueCollection();

            // Do nothing if there are no properties to retreive.
            if (collection.Count < 1)
                return settings;

            // For properties lacking an explicit SerializeAs setting, set
            //   SerializeAs to 'String' for strings and primitives, and 'XML'
            //   for everything else.
            foreach (SettingsProperty prop in collection)
            {
                if (prop.SerializeAs == SettingsSerializeAs.ProviderSpecific)
                    if (prop.PropertyType.IsPrimitive || prop.PropertyType == typeof(System.String))
                        prop.SerializeAs = SettingsSerializeAs.String;
                    else
                        prop.SerializeAs = SettingsSerializeAs.Xml;

                // Add to SettingsPropertyValueCollection.
                settings.Add(new SettingsPropertyValue(prop));
            }

            // Get the user name or anonymous ID and authentication status.
            string username = (string)context["UserName"];
            bool isAuthenticated = (bool)context["IsAuthenticated"];

            // We'll populate these with values from the user's profile data.
            string[] names;
            string values;
            byte[] buf = null;

            // Load the Profile
            if (!string.IsNullOrEmpty(username))
            {
                // Load the XML file into a DataSet
                using (DataSet ds = this.OpenXml())
                {
                    // Find the specified user's profile.
                    DataRow[] drProf = ds.Tables["Profile"].Select(string.Format("UserName = '{0}' AND ApplicationName = '{1}' AND IsAnonymous = '{2}'", username, this.ApplicationName, !isAuthenticated));

                    // If we don't find a row, that doesn't mean there's an error.
                    //   Just return the empty PropertyValueCollection.
                    if (drProf.Length < 1)
                        return settings;

                    // Get the names of the individual properties stored for this
                    //   user's profile.
                    names = drProf[0]["PropertyNames"].ToString().Split(':');

                    // Get the string values data.
                    values = drProf[0]["PropertyValues"].ToString();
                    if (!string.IsNullOrEmpty(values))
                        values = Encoding.Unicode.GetString(Convert.FromBase64String(values));

                    // Get the binary values data.
                    string bufTemp = drProf[0]["PropertyvaluesBinary"].ToString();
                    if (!string.IsNullOrEmpty(bufTemp))
                        buf = Convert.FromBase64String(bufTemp);
                    else
                        buf = new byte[0];
                }
                this.DecodeProfileData(names, values, buf, settings);
            }
            return settings;
        }
        public override void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection collection)
        {
            // Get information about the user who owns the profile.
            string username = (string)context["UserName"];
            bool isAuthenticated = (bool)context["IsAuthenticated"];

            // Do nothing if there is no user name or no properties.
            if (string.IsNullOrEmpty(username) || collection.Count < 1)
                return;

            // These will hold the profile data we're going to save.
            string names = string.Empty;
            string values = string.Empty;
            byte[] buf = null;

            // Encode the profile data to prepate for saving.
            EncodeProfileData(ref names, ref values, ref buf, collection, isAuthenticated);

            // Do nothing if no properties need saving.
            if (string.IsNullOrEmpty(names))
                return;

            // We'll use this to update the activity fields.
            DateTime dt = DateTime.Now;

            // Save the profile data.
            DataRow dr = null;
            using (DataSet ds = this.OpenXml())
            {
                // Look for the specified user's profile data.
                DataRow[] drProf = ds.Tables["Profile"].Select(string.Format("UserName = '{0}' AND ApplicationName = '{1}' AND IsAnonymous = '{2}'", username, this.ApplicationName, !isAuthenticated));

                // If we found more than one record, there's something wrong with
                //   the XML file.
                if (drProf.Length > 1)
                    throw new ProviderException("More than one profile record found for this user. XML file integrity has been compromised!");

                // If we didn't find any records, then we need to create a new entry.
                if (drProf.Length < 1)
                {
                    DataRow[] drApp = ds.Tables["Applications"].Select("ApplicationName = '" + this.ApplicationName + "'");
                    if (drApp.Length < 1)
                    {
                        DataRow drNewApp = ds.Tables["Applications"].NewRow();
                        drNewApp["ApplicationName"] = this.ApplicationName;
                        drNewApp["LoweredApplicationName"] = this.ApplicationName.ToLower();
                        drNewApp["ApplicationID"] = Guid.NewGuid();
                        ds.Tables["Applications"].Rows.Add(drNewApp);
                    }

                    dr = ds.Tables["Profile"].NewRow();
                    dr["UserName"] = username;
                    dr["ApplicationName"] = this.ApplicationName;
                    dr["PropertyNames"] = names;
                    dr["PropertyValues"] = Convert.ToBase64String(Encoding.Unicode.GetBytes(values));
                    dr["PropertyValuesBinary"] = Convert.ToBase64String(buf);
                    dr["IsAnonymous"] = !isAuthenticated;
                    dr["LastActivityDate"] = dt;
                    dr["LastUpdatedDate"] = dt;
                    ds.Tables["Profile"].Rows.Add(dr);
                }

                // Otherwise, just grab the row we found and update it.
                else
                {
                    dr = drProf[0];
                    dr["PropertyNames"] = names;
                    dr["PropertyValues"] = Convert.ToBase64String(Encoding.Unicode.GetBytes(values));
                    dr["PropertyValuesBinary"] = Convert.ToBase64String(buf);
                    dr["LastActivityDate"] = dt;
                    dr["LastUpdatedDate"] = dt;
                }

                // Save the changes to the XML file.
                this.SaveXml(ds);
            }
        }
        public override int DeleteProfiles(string[] usernames)
        {
            using (DataSet ds = this.OpenXml())
            {
                try
                {
                    for (int i = 0; i < usernames.Length; i++)
                    {
                        object usrID = this.GetUserID(usernames[i], ds);
                        DataRow[] drUsr = ds.Tables["Profile"].Select(string.Format("UserID = '{0}' AND ApplicationName = '{1}'", usernames[i], this.ApplicationName));
                        if (drUsr.Length < 1)
                            throw new Exception("Specified username was not found: " + usernames[i]);
                        else if (drUsr.Length > 1)
                            throw new Exception("Specified username was found more than once: " + usernames[i] + ". Data integrity compromised!");
                        else
                            ds.Tables["Profile"].Rows.Remove(drUsr[0]);
                    }
                    this.SaveXml(ds);
                    // Not sure what we're expected to return here, so I'm just returning the
                    //   number of profiles deleted.
                    return usernames.Length;
                }
                catch (Exception ex)
                {
                    // We do this so that the XML file will only be updated if all
                    //   specified profiles are successfully deleted.
                    this.WriteEvent(ex, "DeleteProfiles(string[])");
                    throw;
                }
            }
        }
        public override int DeleteProfiles(System.Web.Profile.ProfileInfoCollection profiles)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        public override int DeleteInactiveProfiles(System.Web.Profile.ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        public override System.Web.Profile.ProfileInfoCollection FindProfilesByUserName(System.Web.Profile.ProfileAuthenticationOption authenticationOption, string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        public override System.Web.Profile.ProfileInfoCollection FindInactiveProfilesByUserName(System.Web.Profile.ProfileAuthenticationOption authenticationOption, string usernameToMatch, DateTime userInactiveSinceDate, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        public override System.Web.Profile.ProfileInfoCollection GetAllProfiles(System.Web.Profile.ProfileAuthenticationOption authenticationOption, int pageIndex, int pageSize, out int totalRecords)
        {
            System.Web.Profile.ProfileInfoCollection piCol = new System.Web.Profile.ProfileInfoCollection();
            using (DataSet ds = this.OpenXml())
            {
                DataTable dt = ds.Tables["Profile"];
                totalRecords = dt.Rows.Count;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow dr = dt.Rows[i];
                    DataRow[] drProfData = ds.Tables["ProfileData"].Select(string.Format("ProfileId = '{0}'", dr["ProfileId"]));
                    int profSz = 0;
                    for (int j = 0; j < drProfData.Length; j++)
                        profSz += drProfData[j]["PropertyValue"].ToString().Length;
                    System.Web.Profile.ProfileInfo pi = new System.Web.Profile.ProfileInfo((string)dr["UserName"], (bool)dr["IsAnonymous"], (DateTime)dr["LastActivityDate"], (DateTime)dr["LastUpdatedDate"], profSz);
                    piCol.Add(pi);
                }
            }
            return piCol;
        }
        public override System.Web.Profile.ProfileInfoCollection GetAllInactiveProfiles(System.Web.Profile.ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        public override int GetNumberOfInactiveProfiles(System.Web.Profile.ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate)
        {
            throw new Exception("The method or operation is not implemented.");
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
                ds.Tables["Profile"].PrimaryKey = new DataColumn[] { ds.Tables["Profile"].Columns["UserName"], ds.Tables["Profile"].Columns["ApplicationName"] };
                ds.Tables["Roles"].PrimaryKey = new DataColumn[] { ds.Tables["Roles"].Columns["ApplicationId"], ds.Tables["Roles"].Columns["RoleID"] };
                ds.Tables["UsersInRoles"].PrimaryKey = new DataColumn[] { ds.Tables["UsersInRoles"].Columns["RoleId"], ds.Tables["UsersInRoles"].Columns["UserId"], ds.Tables["UsersInRoles"].Columns["ApplicationId"] };
                ds.Tables["Applications"].PrimaryKey = new DataColumn[] { ds.Tables["Applications"].Columns["ApplicationId"] };
                ds.Tables["Membership"].PrimaryKey = new DataColumn[] { ds.Tables["Membership"].Columns["ApplicationId"], ds.Tables["Membership"].Columns["UserId"] };
                ds.Tables["Paths"].PrimaryKey = new DataColumn[] { ds.Tables["Paths"].Columns["ApplicationID"], ds.Tables["Paths"].Columns["PathId"] };

                // Setup Foreign Keys
                ds.Tables["Users"].ParentRelations.Add(ds.Tables["Applications"].Columns["ApplicationId"], ds.Tables["Users"].Columns["ApplicationId"]);
                ds.Tables["Profile"].ParentRelations.Add(ds.Tables["Users"].Columns["UserName"], ds.Tables["Profile"].Columns["UserName"]);
                ds.Tables["Profile"].ParentRelations.Add(ds.Tables["Applications"].Columns["ApplicationName"], ds.Tables["Profile"].Columns["ApplicationName"]);

                return ds;
            }
            catch (Exception ex)
            {
                this.WriteEvent(ex, "OpenXml");
                throw;
            }
            finally
            { this._mut.ReleaseMutex(); }
        }
        private void SaveXml(DataSet ds)
        {
            this._mut.WaitOne();
            bool writeSuccess = false;
            Exception lastEx = null;
            int retryCnt = 0;
            while (!writeSuccess)
            {
                try
                {
                    using (FileStream fs = new FileStream(this.XmlPath, FileMode.Create, FileAccess.Write))
                    {
                        using (XmlTextWriter tr = new XmlTextWriter(fs, Encoding.UTF8))
                        {
                            tr.Formatting = Formatting.Indented;
                            tr.Indentation = 2;
                            tr.IndentChar = ' ';
                            ds.WriteXml(tr, XmlWriteMode.IgnoreSchema);
                        }
                        ds.AcceptChanges();
                    }
                    writeSuccess = true;
                }
                catch (Exception ex)
                {
                    lastEx = ex;
                    retryCnt++;
                    if (retryCnt > 5)
                        break;
                    System.Threading.Thread.Sleep(200);
                }
                finally
                { this._mut.ReleaseMutex(); }
            }
            if (!writeSuccess)
            {
                this.WriteEvent(lastEx, "SaveXml");
                throw lastEx;
            }
        }
        private object GetUserID(string userName, DataSet ds)
        {
            bool closeDs = false;
            try
            {
                if (ds == null)
                {
                    closeDs = true;
                    ds = this.OpenXml();
                }

                // Get the GUID for the specified applicationID
                DataRow[] drApp = ds.Tables["Applications"].Select(string.Format("LoweredApplicationName = '{0}'", this._appName.ToLower()));
                if (drApp.Length < 1)
                    throw new Exception("Specified application name was not found.");
                object appID = drApp[0]["ApplicationId"];

                // Get the GUID for the specified userName
                DataRow[] drUsr = ds.Tables["Users"].Select(string.Format("UserName = '{0}' AND ApplicationId = '{1}'", userName, appID));
                if (drUsr.Length < 1)
                    throw new ArgumentException("Specified username was not found.");
                object userID = drUsr[0]["UserID"];

                return userID;
            }
            catch (Exception ex)
            {
                this.WriteEvent(ex, "GetUserID");
                throw;
            }
            finally
            {
                // If we opened the DataSet in this method, close it before leaving
                //   the method.
                if (closeDs && ds != null)
                    ds.Dispose();
            }
        }
        private object GetProfileID(string userName, bool isAuthenticated, DataSet ds)
        {
            bool closeDs = false;
            try
            {
                if (ds == null)
                {
                    closeDs = true;
                    ds = this.OpenXml();
                }

                // Get the ProfileID for the specified UserName.
                DataRow[] dr = ds.Tables["Profile"].Select(string.Format("UserName = '{0}' AND ApplicationName = '{1}' AND IsAnonymous = '{2}'", userName, this._appName, !isAuthenticated));
                if (dr.Length > 1)
                    throw new ProviderException("Specified UserName has multple profile entries for this ApplicationName. XML file integrity compromised!");
                else if (dr.Length < 1)
                    return null;
                else
                    return dr[0]["ProfileId"];
            }
            catch (Exception ex)
            {
                this.WriteEvent(ex, "GetProfileID");
                throw;
            }
            finally
            {
                // If we opened the DataSet in this method, close it before leaving
                //   the method.
                if (closeDs && ds != null)
                    ds.Dispose();
            }
        }
        private object CreateUserProfile(string userName, bool isAuthenticated, DataSet ds)
        {
            object profId = Guid.NewGuid();
            try
            {
                DataRow drNew = ds.Tables["Profile"].NewRow();
                drNew["ProfileId"] = profId;
                drNew["UserName"] = userName;
                drNew["ApplicationName"] = this._appName;
                drNew["IsAnonymous"] = !isAuthenticated;
                drNew["LastActivityDate"] = DateTime.Now;
                drNew["LastUpdatedDate"] = DateTime.MinValue;
                ds.Tables["Profile"].Rows.Add(drNew);
                return profId;
            }
            catch (Exception ex)
            {
                this.WriteEvent(ex, "CreateUserProfile");
                throw;
            }
        }
        private void DecodeProfileData(string[] names, string values, byte[] buf, SettingsPropertyValueCollection properties)
        {
            // If we got passed any null values, then we need to abort this.
            if (names == null || values == null || buf == null | properties == null)
                return;

            // Loop through the list of names in increments of 4 (Each property name
            //   contains 4 elements defining how the data is physically stored)
            for (int i = 0; i < names.Length; i += 4)
            {
                // Read the next property name from the "names" array and retrieve
                //   the corresponding SettingsPropertyValue from "properties"
                string name = names[i];
                SettingsPropertyValue pp = properties[name];

                // If we didn't find a property value with that name, skip to the
                //   next entry.
                if (pp == null)
                    continue;

                // Get the length and index of the persisted property value.
                int pos = Int32.Parse(names[i + 2], System.Globalization.CultureInfo.InvariantCulture);
                int len = Int32.Parse(names[i + 3], System.Globalization.CultureInfo.InvariantCulture);

                // If the length is '-1' and the property is a reference type, then
                //  the property value is NULL.
                if (len < 0 && !pp.Property.PropertyType.IsValueType)
                {
                    pp.PropertyValue = null;
                    pp.IsDirty = false;
                    pp.Deserialized = true;
                }

                // If the property value was persisted as a string, restore it
                //   from 'values'.
                else if (names[i + 1] == "S" && pos >= 0 && len > 0 && values.Length >= pos + len)
                    pp.SerializedValue = values.Substring(pos, len);

                // If the property value was persisted as a byte array, restore it
                //   from 'buf'.
                else if (names[i + 1] == "B" && pos >= 0 && len > 0 && buf.Length >= pos + len)
                {
                    byte[] buf2 = new byte[len];
                    Buffer.BlockCopy(buf, pos, buf2, 0, len);
                    pp.SerializedValue = buf2;
                }
            }
        }
        private void EncodeProfileData(ref string allNames, ref string allValues, ref byte[] buf, SettingsPropertyValueCollection properties, bool isAuthenticated)
        {
            StringBuilder names = new StringBuilder();
            StringBuilder values = new StringBuilder();
            MemoryStream stream = null;

            try
            {
                // Create the memory stream to store binary data.
                stream = new MemoryStream();

                // Loop through each property setting.
                foreach (SettingsPropertyValue pp in properties)
                {
                    // Ignore this property if the user is anonymous and the
                    //   property's AllowAnonymous property is false.
                    if (!isAuthenticated && !(bool)pp.Property.Attributes["AllowAnonymous"])
                        continue;

                    // Ignore this property if it's not dirty and is currently
                    //   assigned to the default value.
                    if (!pp.IsDirty && pp.UsingDefaultValue)
                        continue;

                    int len = 0, pos = 0;
                    string propVal = null;

                    // If Deserialized is true and PropertyValue is null,
                    //   then the property's current value is null (which we'll
                    //   represent by settings the length to -1
                    if (pp.Deserialized && pp.PropertyValue == null)
                        len = -1;

                    // Otherwise, get the property value from 'SerializedValue'
                    else
                    {
                        object sval = pp.SerializedValue;

                        // If SerializedValue is null, then the property's
                        //   current value is null.
                        if (sval == null)
                            len = -1;

                        // If sval is a string, then encode it as a string.
                        else if (sval is string)
                        {
                            propVal = (string)sval;
                            len = propVal.Length;
                            pos = values.Length;
                        }

                        // If sval is binary, then encode it as a byte array.
                        else
                        {
                            byte[] b2 = (byte[])sval;
                            pos = (int)stream.Position;
                            stream.Write(b2, 0, b2.Length);
                            stream.Position = pos + b2.Length;
                            len = b2.Length;
                        }
                    }

                    // Add a string conforming to the following format to 'names':
                    // 
                    // "name:B|S:pos:len"
                    //    ^   ^   ^   ^
                    //    |   |   |   |
                    //    |   |   |   +--- Length of Data
                    //    |   |   +------- Offset of Data
                    //    |   +----------- Location (B = 'buf', S = 'values')
                    //    +--------------- Property Name
                    names.Append(pp.Name + ":"
                        + ((propVal != null) ? "S" : "B") + ":"
                        + pos.ToString(System.Globalization.CultureInfo.InvariantCulture) + ":"
                        + len.ToString(System.Globalization.CultureInfo.InvariantCulture) + ":");

                    // If the property value is encoded as a string, add the string
                    //   to 'values'.
                    if (propVal != null)
                        values.Append(propVal);
                }
                // Copy the binary data from the memory stream into our byte buffer.
                buf = stream.ToArray();
            }
            finally
            {
                if (stream != null)
                    stream.Dispose();
            }
            allNames = names.ToString();
            allValues = values.ToString();
        }
        private void UpdateActivityDates(string userName, bool isAuthenticated, bool activityOnly, DataSet ds)
        {
            DateTime activityDate = DateTime.Now;
            try
            {
                DataRow[] dr = ds.Tables["Profile"].Select(string.Format("UserName = '{0}' AND ApplicationName = '{1}' AND IsAnonymous = '{2}'", userName, this._appName, !isAuthenticated));
                if (dr.Length > 1)
                    throw new Exception("Multiple profiles found for this user name.  XML file integrity compromised!");
                dr[0]["LastActivityDate"] = activityDate;
                if (!activityOnly)
                    dr[0]["LastUpdatedDate"] = activityDate;
            }
            catch (Exception ex)
            {
                this.WriteEvent(ex, "UpdateActivityDates");
                throw;
            }
        }
        private void WriteEvent(Exception ex, string action)
        {
            if (!this._useEventLog)
                return;

            try
            {
                System.Diagnostics.EventLog log = new System.Diagnostics.EventLog();
                log.Source = this._eventSrc;
                log.Log = this._eventLog;

                string msg = "An exception occured while comunicated with the data source.\n\n";
                msg += "Action: " + action + "\n\n";
                msg += "Exception: " + ex.ToString();

                log.WriteEntry(msg);
            }
            catch
            { this._useEventLog = false; }
        }
        #endregion
    }
}
