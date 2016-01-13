using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace RainstormStudios.Web
{
    public class EmailHelper
    {
        protected virtual char[] AddrSplitChars
        { get { return new char[] { ';' }; } }
        protected virtual string NoReplyEmailAddress
        { get { return "noreply@webapp.com"; } }
        protected abstract string AdminAddresses{ get; }
        protected abstract string SupportAddresses { get; }
        protected abstract string SupportAddressesBCC { get; }
        protected abstract string SiteUrl { get; }
        protected virtual bool SendEmailNotificationOfErrors
        { get { return true; } }
        protected virtual bool WriteExceptionsToTextLog
        { get { return false; } }

        public bool SendAdminEmailNotification(string subject, string body, bool isHtml)
        {
            return SendEmail(AdminAddresses.Split(AddrSplitChars, StringSplitOptions.RemoveEmptyEntries), subject, body, isHtml);
        }
        public bool SendSupportEmailNotification(string subject, string body, bool isHtml)
        {
            Exception exDummy;
            return SendSupportEmailNotification(subject, body, isHtml, out exDummy);
        }
        public bool SendSupportEmailNotification(string subject, string body, bool isHtml, out Exception sendException)
        {
            return SendSupportEmailNotification(subject, body, isHtml, string.Empty, out sendException);
        }
        public bool SendSupportEmailNotification(string subject, string body, bool isHtml, string userEmail)
        {
            Exception exDummy;
            return SendSupportEmailNotification(subject, body, isHtml, userEmail, out exDummy);
        }
        public bool SendSupportEmailNotification(string subject, string body, bool isHtml, string userEmail, out Exception sendException)
        {
            return SendEmail(SupportAddresses.Split(AddrSplitChars, StringSplitOptions.RemoveEmptyEntries), SupportAddressesBCC.Split(AddrSplitChars, StringSplitOptions.RemoveEmptyEntries), userEmail.Split(AddrSplitChars, StringSplitOptions.RemoveEmptyEntries), subject, body, isHtml, out sendException);
        }
        public bool SendEmail(string sendTo, string subject, string body, bool isHtml)
        { return SendEmail(new string[] { sendTo }, subject, body, isHtml); }
        public bool SendEmail(string[] sendTo, string subject, string body, bool isHtml)
        { return SendEmail(sendTo, new string[0], subject, body, isHtml); }
        public bool SendEmail(string[] sendTo, string[] cc, string subject, string body, bool isHtml)
        {
            Exception ex = null;
            return SendEmail(sendTo, cc, subject, body, isHtml, out ex);
        }
        public bool SendEmail(string[] sendTo, string[] cc, string subject, string body, bool isHtml, out Exception sendException)
        {
            return SendEmail(sendTo, cc, new string[0], subject, body, isHtml, out sendException);
        }
        public bool SendEmail(string[] sendTo, string[] cc, string[] bcc, string subject, string body, bool isHtml, out Exception sendException)
        {
            try
            {
                bool msgSent = false;

                // Append the confidentiality notice to the body of the email.
                StringBuilder sbBody = new StringBuilder(body);
                if (isHtml)
                    sbBody.Append("<p><b>");
                else
                    sbBody.Append("\r\n\r\n");
                sbBody.Append("Internet e-mail confidentiality notice:");
                if (isHtml)
                    sbBody.Append("</b><br />");
                else
                    sbBody.Append("\r\n");
                sbBody.Append("This email message is for the sole use of the intended recipient(s) and may contain confidential information. Any unauthorized review, use, disclosure or distribution is prohibited. If you are not the intended recipient, please contact the sender by reply email and destroy all copies of the original message. Note the information contain within or attached to this email is considered to be Harris County - Confidential Information.");
                if (isHtml)
                    sbBody.AppendLine("</p>");

                try
                {
                    System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient();
                    using (System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage())
                    {
                        msg.From = new System.Net.Mail.MailAddress(NoReplyEmailAddress);
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

                        //if (!HttpContext.Current.IsDebuggingEnabled)
                        client.Send(msg);
                        msgSent = true;
                    }
                    sendException = null;
                }
                catch (Exception ex)
                {
                    try
                    {
                        if (WriteExceptionsToTextLog)
                            LogManager.WriteToTextLog(ex, HttpContext.Current);
                    } catch { }
                    try
                    {
                        if (SendEmailNotificationOfErrors)
                            ErrorHandler.SendServerErrMsg("EMailHelper.SendEmail", ex, HttpContext.Current);
                    } catch { }
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
                try
                {
                    if (WriteExceptionsToTextLog)
                        LogManager.WriteToTextLog(ex, HttpContext.Current);
                } catch { }
                sendException = ex;
                return false;
            }
        }
    }
}
