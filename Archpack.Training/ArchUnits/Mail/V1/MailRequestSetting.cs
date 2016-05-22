using System;
using System.Collections.Generic;
using System.Linq;

namespace Archpack.Training.ArchUnits.Mail.V1
{
    public class MailRequestSetting
    {
        public MailRequestSetting()
        {
            this.To = new List<EmailAddress>();
            this.Cc = new List<EmailAddress>();
            this.Bcc = new List<EmailAddress>();
            this.Properties = new Dictionary<string, object>();
        }

        public MailRequestSetting(EmailAddress from, List<EmailAddress> to, string subject, string body, List<EmailAddress> cc = null, List<EmailAddress> bcc = null, Dictionary<string, object> properties = null)
        {
            this.From = from;
            this.To = to;
            this.Cc = cc ?? new List<EmailAddress>();
            this.Bcc = bcc ?? new List<EmailAddress>();
            this.Subject = subject;
            this.Body = body;
            this.Properties = properties ?? new Dictionary<string, object>();
        }

        public EmailAddress From { get; set; }

        public List<EmailAddress> To { get; private set; }

        public List<EmailAddress> Cc { get; private set; }

        public List<EmailAddress> Bcc { get; private set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        public Dictionary<string, object> Properties { get; private set; }
    }
}