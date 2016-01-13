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
using System.Configuration;
using System.Configuration.Provider;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.Configuration;
using RainstormStudios.Web.UI.WebControls.Calendar;

namespace RainstormStudios.Providers
{
    public class CalendarProviderConfiguration : System.Configuration.ConfigurationSection
    {
        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        [ConfigurationProperty("providers")]
        public ProviderSettingsCollection Providers
        {
            get { return (ProviderSettingsCollection)base["providers"]; }
        }
        [StringValidator(MinLength = 1), ConfigurationProperty("default", DefaultValue = "SqlCalendarProvider")]
        public string Default
        {
            get { return (string)base["default"]; }
            set { base["default"] = value; }
        }
        #endregion
    }
    public class CalendarProviderManager
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private static CalendarProvider
            _defaultProvider;
        private static CalendarProviderCollection
            _providers;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public static CalendarProvider Provider
        {
            get { return _defaultProvider; }
        }
        public static CalendarProviderCollection Providers
        {
            get { return _providers; }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        static CalendarProviderManager()
        {
            Initialize();
        }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        private static void Initialize()
        {
            CalendarProviderConfiguration config =
                (CalendarProviderConfiguration)ConfigurationManager.GetSection("CalendarProvider");

            if (config == null)
                throw new ConfigurationErrorsException("Calendar provider configuration section is not set correctly.");

            _providers = new CalendarProviderCollection();

            System.Web.Configuration.ProvidersHelper.InstantiateProviders(config.Providers, _providers, typeof(CalendarProvider));

            _providers.SetReadOnly();

            _defaultProvider = _providers[config.Default];

            if (_defaultProvider == null)
                throw new ProviderException("No default provider sepecified.");
        }
        #endregion
    }
    public abstract class CalendarProvider : System.Configuration.Provider.ProviderBase
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        #endregion

        #region Public Methods
        //***************************************************************************
        // Abstract Methods
        // 
        public abstract CalendarEvent[] GetEvents(object userKey, DateTime calDate);
        public abstract CalendarEvent[] GetEvents(object userKey, DateTime from, DateTime to);
        public abstract EventInviteCollection GetEventInvites(object eventProviderKey);
        public abstract CalendarEvent CreateEvent(object calendarProviderKey, System.Net.Mail.MailAddress host, string name, string location, DateTime start, DateTime end);
        public abstract CalendarEvent CreateEvent(object calendarProviderKey, CalendarEvent calEvent);
        public abstract object CreateCalendar(string calName, object ownerKey);
        public abstract UserCalendar[] GetCalendars();
        public abstract UserCalendar[] GetCalendars(object userKey);
        #endregion

        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        
        #endregion
    }
    public class CalendarProviderCollection : ProviderCollection
    {
        // Return an instance of CalendarProviderBase
        //   for a specified provider name.
        public new CalendarProvider this[string name]
        {
            get { return (CalendarProvider)base[name]; }
        }
    }
    public class SqlCalendarProvider : CalendarProvider
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private bool
            _writeExEventLog;
        private string
            _connStr;
        private int
            _connRetryDelay;
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            // Init values from web.config
            if (config == null)
                throw new ArgumentNullException("config", "No configuration data was provided.");

            if (name == null || name.Length == 0)
                name = "SqlCalendarProvider";

            if (string.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "Rainstorm Studios default SQL Calendar Provider");
            }

            base.Initialize(name, config);

            this._writeExEventLog = Boolean.Parse(this.GetConfigurationValue(config["WriteExceptionsToEventLog"], "true"));
            ConnectionStringSettings connStrSettings = ConfigurationManager.ConnectionStrings[config["connectionStringName"]];
            if (connStrSettings == null || connStrSettings.ConnectionString.Trim() == "")
                throw new ProviderException("You must specify a SQL connection string.");
            this._connStr = connStrSettings.ConnectionString;
            this._connRetryDelay = int.Parse(this.GetConfigurationValue(config["SqlConnectionRetryDelay"], "3000"));
        }
        public override object CreateCalendar(string calName, object ownerKey)
        {
            throw new NotImplementedException();
        }
        public override CalendarEvent CreateEvent(object calendarProviderKey, CalendarEvent calEvent)
        {
            throw new NotImplementedException();
        }
        public override CalendarEvent CreateEvent(object calendarProviderKey, System.Net.Mail.MailAddress host, string name, string location, DateTime start, DateTime end)
        {
            throw new NotImplementedException();
        }
        public override UserCalendar[] GetCalendars()
        {
            throw new NotImplementedException();
        }
        public override UserCalendar[] GetCalendars(object ownerKey)
        {
            throw new NotImplementedException();
        }
        public override EventInviteCollection GetEventInvites(object eventProviderKey)
        {
            throw new NotImplementedException();
        }
        public override CalendarEvent[] GetEvents(object userKey, DateTime calDate)
        {
            throw new NotImplementedException();
        }
        public override CalendarEvent[] GetEvents(object userKey, DateTime from, DateTime to)
        {
            List<CalendarEvent> foundEvents = new List<CalendarEvent>();
            using (SqlConnection conn = this.GetOpenConnection())
            {
                string sql = "SELECT c.[CalendarName] ,c.[CalendarOwnerID] ,e.* ,uc.[Color] FROM [dbo].[Calendars] c  INNER JOIN [dbo].[UserCalendars] uc ON uc.[CalendarID] = c.[CalendarID]  INNER JOIN [dbo].[Events] e ON e.[ParentCalendarID] = c.[CalendarID]  WHERE (e.[EventStart] BETWEEN @d1 AND @d2 OR e.[EventEnd] BETWEEN @d1 AND @d2)  AND uc.[UserKey] = @userKey  ORDER BY e.[EventStart]";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add(new SqlParameter { ParameterName = "userKey", Value = userKey, DbType = DbType.Guid });
                    cmd.Parameters.Add(new SqlParameter { ParameterName = "d1", Value = from, DbType = DbType.DateTime });
                    cmd.Parameters.Add(new SqlParameter { ParameterName = "d2", Value = to, DbType = DbType.DateTime });
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                        while (rdr.Read())
                            foundEvents.Add(this.GetEventFromReader(rdr));
                }
            }
            return foundEvents.ToArray();
        }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        private string GetConfigurationValue(string configValue, string defaultValue)
        {
            if (string.IsNullOrEmpty(configValue))
                return defaultValue;
            return configValue;
        }
        private SqlConnection GetOpenConnection()
        {
            SqlConnection conn = new SqlConnection(this._connStr);
            conn.Open();
            int attemptCnt = 0;
            while (conn.State != ConnectionState.Open && attemptCnt++ < 5)
            {
                System.Threading.Thread.Sleep(this._connRetryDelay);
                conn.Open();
            }
            if (conn.State != ConnectionState.Open)
                throw new Exception("Unable to open connection to database using specified connection string.");
            return conn;
        }
        private CalendarEvent GetEventFromReader(SqlDataReader rdr)
        {
            CalendarEvent rdrEvent = new CalendarEvent((string)rdr["HostEmail"]);
            rdrEvent.ProviderEventKey = rdr["EventID"];
            rdrEvent.AllDayEvent = (bool)rdr["AllDay"];
            if (rdr["Category"] != System.DBNull.Value)
                rdrEvent.Category = (string)rdr["Category"];
            rdrEvent.DateCreated = (DateTime)rdr["DateCreated"];
            object stDate = rdr["EventStart"], edDate = rdr["EventEnd"];
            if (stDate != System.DBNull.Value)
                rdrEvent.EventStartDate = (DateTime)stDate;
            if (edDate != System.DBNull.Value)
                rdrEvent.EventEndDate = (DateTime)edDate;
            rdrEvent.Reoccuring = (bool)rdr["Reoccuring"];
            rdrEvent.Subject = (string)rdr["Subject"];
            rdrEvent.Description = (string)rdr["Description"];
            rdrEvent.Location = (string)rdr["Location"];
            rdrEvent.ParentCalendar = new UserCalendar { ProviderCalendarKey = rdr["ParentCalendarID"], CalendarName = (string)rdr["CalendarName"] };
            rdrEvent.Priority = (EventPriority)int.Parse(rdr["Priority"].ToString());

            return rdrEvent;
        }
        #endregion
    }
}
