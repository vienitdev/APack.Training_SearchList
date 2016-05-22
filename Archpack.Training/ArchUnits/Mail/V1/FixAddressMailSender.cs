using Archpack.Training.ArchUnits.Contracts.V1;
using Archpack.Training.ArchUnits.Logging.V1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;

namespace Archpack.Training.ArchUnits.Mail.V1
{
    public class FixAddressMailSender : IMailSender
    {
        private LogContext logcontext { get; set; }

        private SmtpMailConfiguration smtpConfig { get; set; }

        public FixAddressMailSender(SmtpMailConfiguration smtpConfig, LogContext logcontext)
        {
            Contract.NotNull(smtpConfig, "smtpConfig");
            Contract.NotNull(smtpConfig.FixAddresses, "FixAddresses");
            Contract.NotEmpty(smtpConfig.SmtpServer, "SmtpServer");
            Contract.Assert(smtpConfig.FixAddresses.Count() > 0, "FixAddresses");

            this.logcontext = logcontext;
            this.smtpConfig = smtpConfig;
        }

        public void Send(MailRequestSetting mailSetting)
        {
            //Validation mailSetting
            Contract.NotNull(mailSetting, "mailSetting");
            Contract.NotNull(mailSetting.From, "From");
            Contract.NotNull(mailSetting.To, "To");
            Contract.Assert(mailSetting.To.Count() > 0, "To");
            Contract.NotEmpty(mailSetting.Subject, "Subject");

            //Format property to output
            string to = String.Join(",", mailSetting.To.Select(t => String.Format("<{0}/{1}>", t.Address, t.DisplayName)));
            string cc = mailSetting.Cc == null ? "" : String.Join(",", mailSetting.Cc.Select(t => String.Format("<{0}/{1}>", t.Address, t.DisplayName)));
            string bcc = mailSetting.Bcc == null ? "" : String.Join(",", mailSetting.Bcc.Select(t => String.Format("<{0}/{1}>", t.Address, t.DisplayName)));

            StringBuilder content = new StringBuilder();
            content.AppendLine("-----------------------------------------------------");
            content.Append("このメールは固定アドレスメール機能により送信されました。").AppendLine();
            content.AppendFormat("To: {0}", to).AppendLine();
            content.AppendFormat("CC: {0}", cc).AppendLine();
            content.AppendFormat("BCC: {0}", bcc).AppendLine();
            content.AppendLine("-----------------------------------------------------");

            content.AppendLine().AppendLine();

            content.Append(mailSetting.Body);

            //Create fixAddress To from SmtpMailConfiguration
            List<EmailAddress> fixTo = new List<EmailAddress>();
            smtpConfig.FixAddresses.ForEach(f => fixTo.Add(new EmailAddress(f)));
            MailRequestSetting newSetting = new MailRequestSetting(mailSetting.From, fixTo, mailSetting.Subject, content.ToString(), properties: mailSetting.Properties);

            new SmtpMailSender(smtpConfig, logcontext).Send(newSetting);
        }


    }
}