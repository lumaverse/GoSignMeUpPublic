using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gsmu.Api.Data;

namespace Gsmu.Api.Networking.Mail
{
    public static class MailSettings
    {
        /// <summary>
        /// This is based on ASP to divide the password and the username as it is stored in 1 string.
        /// </summary>
        public static readonly string SmtpAccountDivider = "%@%@%";

        /// <summary>
        /// Ip / host of mail server
        /// </summary>
        public static string MailServer
        {
            get
            {
                return Settings.Instance.GetMasterInfo().Mailserver;
            }
        }

        /// <summary>
        /// Ip / host of alternative mail server
        /// </summary>
        public static string AlternativeMailServer
        {
            get
            {
                return Settings.Instance.GetMasterInfo().altmailserver;
            }
        }

        /// <summary>
        /// This is the email address used in the public e-mails.
        /// </summary>
        public static string PublicEmailFrom
        {
            get
            {
                return Settings.Instance.GetMasterInfo().PublicEmailAddress;
            }
        }

        /// <summary>
        /// This is the email address used in admin emails.
        /// </summary>
        public static string AdminEmailFrom
        {
            get
            {
                return Settings.Instance.GetMasterInfo().EmailAddress;
            }
        }

        /// <summary>
        /// This is the email address used in the public e-mails.
        /// </summary>
        public static AccountInfo SmtpAccountInfo
        {
            get
            {
                var value = Settings.Instance.GetMasterInfo().SMTPEmailAccount;
                var info = new AccountInfo();
                if (string.IsNullOrWhiteSpace(value) || value.IndexOf(SmtpAccountDivider) == -1)
                {
                    return info;
                }
                var split = new string[] { SmtpAccountDivider };
                var infoes = value.Split(split, StringSplitOptions.None);
                info.Username = infoes[0];
                info.Password = infoes[1];
                return info;
            }
        }

        public static MailHandler MailHandler
        {
            get
            {
                var value = Settings.Instance.GetMasterInfo().MailHandler;
                if (value == 1)
                {
                    return MailHandler.JMail;
                }
                else if (value == 7)
                {
                    return MailHandler.EAMail;
                }
                return MailHandler.DotNet;
            }
        }

        /// <summary>
        /// This is the server username and password.
        /// </summary>
        public static string SMTPEmailAccount
        {
            get
            {
                return Settings.Instance.GetMasterInfo().SMTPEmailAccount;
            }
        }

        public class NetworkingEmailResendingModel 
        {
            public bool SendEmailUsingPreview { get; set; }
            public string EmailAddressTo { get; set; }
            public bool InitiateEmailSending { get; set; }
            public bool InitiateAuditLogging { get; set; }
            public string Subject { get; set; }
            public string Body { get; set; }
        }
    }
}
