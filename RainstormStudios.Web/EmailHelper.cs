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
using System.Configuration;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace RainstormStudios.Web
{
    [Author("Unfried, Michael")]
    public class EmailHelper
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        protected static EmailHelperConfiguration
            _config = null;
        //***************************************************************************
        // Delegates
        // 
        private delegate bool
            BeginSendEmailDelegate(string[] sendTo, string[] cc, string[] bcc, string subject, string body, bool isHtml);
        //***************************************************************************
        // Public Events
        // 
        public event EventHandler<SendEmailCompleteEventArgs>
            SendEmailComplete;
        #endregion

        #region Properties
        ////***************************************************************************
        //// Abstract Properties
        //// 
        //protected abstract string NoReplyEmailAddress { get; }
        //protected abstract string AdminAddresses { get; }
        //protected abstract string SupportAddresses { get; }
        //protected abstract string SupportAddressBCC { get; }
        //protected abstract string SiteUrl { get; }
        ////***************************************************************************
        //// Private Properties
        //// 
        //protected virtual bool WriteExceptionsToTextLog { get { return false; } }
        //protected virtual char[] AddressSplitCharacters { get { return new char[] { ';' }; } }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        static EmailHelper()
        {
            _config = GetConfig();
        }
        public EmailHelper()
        { }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public static bool SendAdminEmailNotification(string subject, string body, bool isHtml)
        {
            return SendEmail(GetAdminContacts(), subject, body, isHtml);
        }
        public static bool SendSupportEmailNotification(string subject, string body, bool isHtml)
        {
            Exception exDummy;
            return SendSupportEmailNotification(subject, body, isHtml, out exDummy);
        }
        public static bool SendSupportEmailNotification(string subject, string body, bool isHtml, out Exception sendException)
        {
            return SendSupportEmailNotification(subject, body, isHtml, string.Empty, out sendException);
        }
        public static bool SendSupportEmailNotification(string subject, string body, bool isHtml, string userEmail)
        {
            Exception exDummy;
            return SendSupportEmailNotification(subject, body, isHtml, userEmail, out exDummy);
        }
        public static bool SendSupportEmailNotification(string subject, string body, bool isHtml, string userEmail, out Exception sendException)
        {
            return SendEmail(GetSupportContacts(), GetSupportBccConctacs(), userEmail.Split(_config.EmailAddressSeperatorChars, StringSplitOptions.RemoveEmptyEntries), subject, body, isHtml, out sendException);
        }
        public static bool SendEmail(string sendTo, string subject, string body, bool isHtml)
        { return SendEmail(new string[] { sendTo }, subject, body, isHtml); }
        public static bool SendEmail(string[] sendTo, string subject, string body, bool isHtml)
        { return SendEmail(sendTo, new string[0], subject, body, isHtml); }
        public static bool SendEmail(string[] sendTo, string[] cc, string subject, string body, bool isHtml)
        {
            return SendEmail(sendTo, cc, new string[0], subject, body, isHtml);
        }
        public static bool SendEmail(string[] sendTo, string[] cc, string[] bcc, string subject, string body, bool isHtml)
        {
            Exception ex = null;
            bool sendSuccess = SendEmail(sendTo, cc, bcc, subject, body, isHtml, out ex);
            if (ex != null)
                throw ex;
            else
                return sendSuccess;
        }
        public static bool SendEmail(string[] sendTo, string[] cc, string subject, string body, bool isHtml, out Exception sendException)
        {
            return SendEmail(sendTo, cc, new string[0], subject, body, isHtml, out sendException);
        }
        public static bool SendEmail(string[] sendTo, string[] cc, string[] bcc, string subject, string body, bool isHtml, out Exception sendException)
        {
            try
            {
                bool msgSent = false;

                StringBuilder sbBody = new StringBuilder(body);
                if (_config.IncludeConfidentialityNotice)
                {
                    // Append the confidentiality notice to the body of the email.
                    if (isHtml)
                        sbBody.Append("<p><b>");
                    else
                        sbBody.Append("\r\n\r\n");
                    sbBody.Append("Internet e-mail confidentiality notice:");
                    if (isHtml)
                        sbBody.Append("</b><br />");
                    else
                        sbBody.Append("\r\n");
                    sbBody.AppendFormat("This email message is for the sole use of the intended recipient(s) and may contain confidential information. Any unauthorized review, use, disclosure or distribution is prohibited. If you are not the intended recipient, please contact the sender by reply email and destroy all copies of the original message. Note the information contain within or attached to this email is considered to be {0} - Confidential Information.", _config.CompanyName);
                    if (isHtml)
                        sbBody.AppendLine("</p>");
                }

                try
                {
                    System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient();
                    using (System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage())
                    {
                        msg.From = new System.Net.Mail.MailAddress(_config.FromEMailAddress);
                        for (int i = 0; i < sendTo.Length; i++)
                            if (!string.IsNullOrEmpty(sendTo[i]))
                                msg.To.Add(sendTo[i]);
                        for (int i = 0; i < cc.Length; i++)
                            if (!string.IsNullOrEmpty(cc[i]))
                                msg.CC.Add(cc[i]);
                        for (int i = 0; i < bcc.Length; i++)
                            if (!string.IsNullOrEmpty(bcc[i]))
                                msg.Bcc.Add(bcc[i]);
                        msg.Subject = subject;
                        msg.Body = sbBody.ToString();
                        msg.IsBodyHtml = isHtml;
                        msg.BodyEncoding = Encoding.ASCII;

                        if (HttpContext.Current != null && HttpContext.Current.IsDebuggingEnabled && _config.SuppressEmailsInDebugMode)
                        { }
                        else
                            client.Send(msg);
                        msgSent = true;
                    }
                    sendException = null;
                }
                catch (Exception ex)
                {
                    if (_config.WriteExceptionsToLog)
                        RainstormStudios.Web.LogManager.WriteToLog(ex, HttpContext.Current);
                    //try
                    //{
                    //    if (SendEmailNotificationOfErrors)
                    //        ErrorHandler.SendServerErrMsg("EMailHelper.SendEmail", ex, HttpContext.Current, this);
                    //}
                    //catch { }
                    sendException = ex;
                }

                if (!msgSent)
                {
                    // If the email message could not be sent, we want to inform the user
                    //   of this.
                    // Remember, any actual exception that occured is returned as an "out" parameter.
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                if (_config.WriteExceptionsToLog)
                    RainstormStudios.Web.LogManager.WriteToLog(ex, HttpContext.Current);
                sendException = ex;
                return false;
            }
        }
        public void BeginSendEmail(string[] sendTo, string[] cc, string[] bcc, string subject, string body)
        {
            this.BeginSendEmail(sendTo, cc, bcc, subject, body, true);
        }
        public void BeginSendEmail(string[] sendTo, string[] cc, string[] bcc, string subject, string body, bool isHtml)
        {
            BeginSendEmailDelegate del = new BeginSendEmailDelegate(SendEmail);
            del.BeginInvoke(sendTo, cc, bcc, subject, body, isHtml, new AsyncCallback(this.BeginSendEmailCallback), del);
        }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        protected virtual void OnSendEmailComplete(SendEmailCompleteEventArgs e)
        {
            if (this.SendEmailComplete != null)
                this.SendEmailComplete.Invoke(this, e);
        }
        protected static EmailHelperConfiguration GetConfig()
        {
            EmailHelperConfiguration config = EmailHelperConfiguration.config;
            if (config == null)
                throw new ConfigurationErrorsException("You must specify an 'EmailHelper' section in your web.config file.");
            else
                return config;
        }
        protected static string[] GetAdminContacts()
        {
            List<string> addr = new List<string>();
            foreach (EmailContactElement el in _config.AdminContacts)
                addr.Add(el.EmailAddress);
            return addr.ToArray();
        }
        protected static string[] GetSupportContacts()
        {
            List<string> addr = new List<string>();
            foreach (EmailContactElement el in _config.SupportContacts)
                addr.Add(el.EmailAddress);
            return addr.ToArray();
        }
        protected static string[] GetSupportBccConctacs()
        {
            List<string> addrBcc = new List<string>();
            foreach (EmailContactElement el in _config.SupportContactBCC)
                addrBcc.Add(el.EmailAddress);
            return addrBcc.ToArray();
        }
        //***************************************************************************
        // Thread Callbacks
        // 
        void BeginSendEmailCallback(IAsyncResult state)
        {
            BeginSendEmailDelegate del = (BeginSendEmailDelegate)state.AsyncState;
            bool success = del.EndInvoke(state);
            this.OnSendEmailComplete(new SendEmailCompleteEventArgs(success));
        }
        #endregion
    }
    [Author("Unfried, Michael")]
    public class SendEmailCompleteEventArgs : System.EventArgs
    {
        #region Declarations
        //***************************************************************************
        // Public Fields
        // 
        public readonly string
            EmailSubject,
            EmailBody;
        public readonly string[]
            Recipients,
            CC,
            BCC;
        public readonly bool
            Success,
            HTMLFormat,
            HasException;
        public readonly Exception
            Exception;
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public SendEmailCompleteEventArgs(bool success)
        {
            this.Success = success;
        }
        public SendEmailCompleteEventArgs(bool success, string[] sendTo, string[] cc, string[] bcc, string subject, string body, bool isHtml)
            : this(success)
        {
            this.EmailSubject = subject;
            this.EmailBody = body;
            this.Recipients = sendTo;
            this.CC = cc;
            this.BCC = bcc;
            this.HTMLFormat = isHtml;
            this.HasException = false;
        }
        public SendEmailCompleteEventArgs(bool success, string[] sendTo, string[] cc, string[] bcc, string subject, string body, bool isHtml, Exception ex)
            : this(success, sendTo, cc, bcc, subject, body, isHtml)
        {
            this.Exception = ex;
            this.HasException = (this.Exception != null);
        }
        #endregion
    }
    [Author("Unfried, Michael")]
    public class EmailHelperConfiguration : System.Configuration.ConfigurationSection
    {
        #region Declarations
        //***************************************************************************
        // Public Fields
        // 
        public readonly static EmailHelperConfiguration
            config = (ConfigurationManager.GetSection("EmailHelper") as EmailHelperConfiguration);
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        [ConfigurationProperty("fromEmailAddress", DefaultValue = "noreply@site.url", IsRequired = true)]
        [SettingsDescription("Specifies the 'from' address to show on outgoing emails sent by this class.")]
        //[RegexStringValidator(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$")]
        public string FromEMailAddress
        {
            get { return (string)base["fromEmailAddress"]; }
            set { base["fromEmailAddress"] = value; }
        }
        [ConfigurationProperty("adminContacts", IsDefaultCollection = true)]
        [SettingsDescription("Specifies a list of users who will be contacted when there are problems with the website.")]
        public EmailContactElementCollection AdminContacts
        {
            get { return (EmailContactElementCollection)base["adminContacts"]; }
            set { base["adminContacts"] = value; }
        }
        [ConfigurationProperty("supportContacts", IsDefaultCollection = false)]
        [SettingsDescription("Specifies a list of uers who will be contacted when an end user requests support.")]
        public EmailContactElementCollection SupportContacts
        {
            get { return (EmailContactElementCollection)base["supportContacts"]; }
            set { base["supportContacts"] = value; }
        }
        [ConfigurationProperty("supportContactsBcc", IsDefaultCollection = false)]
        [SettingsDescription("Specifies a list of users who users be will included as a BCC on messages sent to the 'supportContacts' list.")]
        public EmailContactElementCollection SupportContactBCC
        {
            get { return (EmailContactElementCollection)base["supportContactBcc"]; }
            set { base["supportContactBcc"] = value; }
        }
        [ConfigurationProperty("siteUrl", DefaultValue = "site.url", IsRequired = true)]
        [SettingsDescription("Specifies the client URL for this web application.  This will be used to provide links to the web site in the outgoing emails.")]
        //[RegexStringValidator(@"^((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$")]
        public string SiteUrl
        {
            get { return (string)base["siteUrl"]; }
            set { base["siteUrl"] = value; }
        }
        [ConfigurationProperty("writeExceptionsToLog", DefaultValue = "true", IsRequired = false)]
        [SettingsDescription("A boolean value indicating 'true' if errors that occur while trying to send email should be written to the a log using the ITCWebToolkit.Web.LogManager class.  Otherwise, 'false'.")]
        //[SubclassTypeValidator(typeof(Boolean))]
        public bool WriteExceptionsToLog
        {
            get { return (bool)base["writeExceptionsToLog"]; }
            set { base["writeExceptionsToLog"] = value; }
        }
        [ConfigurationProperty("includeConfidentialityNotice")]
        [SettingsDescription("A boolean value indicating 'true' if outgoing emails should have a confidentiality notice appended to the bottom.  Otherwise, 'false'.")]
        //[SubclassTypeValidator(typeof(Boolean))]
        public bool IncludeConfidentialityNotice
        {
            get { return (bool)base["includeConfidentialityNotice"]; }
            set { base["includeConfidentialityNotice"] = value; }
        }
        [ConfigurationProperty("companyName", DefaultValue = "Harris County", IsRequired = false)]
        [SettingsDescription("A string value containing the name of the company to use in the confidentiality notice.")]
        //[StringValidator(MinLength = 1)]
        public string CompanyName
        {
            get { return (string)base["companyName"]; }
            set { base["companyName"] = value; }
        }
        [ConfigurationProperty("suppressEmailsInDebugMode", DefaultValue = "true", IsRequired = false)]
        [SettingsDescription("A boolean value indicating 'true' if emails should not be sent when 'debug' mode is enabled for the application.  Otherwise, 'false'.")]
        //[SubclassTypeValidator(typeof(Boolean))]
        public bool SuppressEmailsInDebugMode
        {
            get { return (bool)base["suppressEmailsInDebugMode"]; }
            set { base["suppressEmailsInDebugMode"] = value; }
        }
        [ConfigurationProperty("addressSeperatorChars", DefaultValue = ";", IsRequired = false)]
        [SettingsDescription("A string value containing the characters that should be used to 'split' multiple email address from a single string value.  Default is ';'.")]
        //[StringValidator(MinLength = 1, InvalidCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789")]
        public char[] EmailAddressSeperatorChars
        {
            get { return ((string)base["addressSeperatorChars"]).ToCharArray(); }
            set { base["addressSeperatorChars"] = string.Concat(value); }
        }
        #endregion
    }
    [Author("Unfried, Michael")]
    public class EmailContactElement : System.Configuration.ConfigurationElement
    {
        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        [ConfigurationProperty("name", IsRequired = false)]
        [SettingsDescription("Specifies the real name of the recipient.  This is primarily used to identify recipients in the web.config file.")]
        //[StringValidator(MinLength = 1)]
        public string Name
        {
            get { return (string)base["name"]; }
            set { base["name"] = value; }
        }
        [ConfigurationProperty("emailAddress", IsRequired = true, IsKey = true)]
        [SettingsDescription("Specifies the email address where the error alert will be sent.")]
        //[RegexStringValidator(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$")]
        public string EmailAddress
        {
            get { return (string)base["emailAddress"]; }
            set { base["emailAddress"] = value; }
        }
        #endregion
    }
    [Author("Unfried, Michael")]
    [ConfigurationCollection(typeof(EmailContactElement))]
    public class EmailContactElementCollection : System.Configuration.ConfigurationElementCollection
    {
        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        protected override ConfigurationElement CreateNewElement()
        {
            return new EmailContactElement();
        }
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((EmailContactElement)element).EmailAddress;
        }
        #endregion
    }
}
