using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Archpack.Training.ArchUnits.Logging.V1;
using System.Net.Mail;
using System.IO;
using Archpack.Training.ArchUnits.Contracts.V1;
using Archpack.Training.ArchUnits.Collections.V1;

namespace Archpack.Training.ArchUnits.Mail.V1
{
    public class SmtpMailSender : IMailSender
    {
        private LogContext logcontext { get; set; }

        private SmtpMailConfiguration smtpConfig { get; set; }

        public SmtpMailSender(SmtpMailConfiguration smtpConfig, LogContext logcontext)
        {
            Contract.NotNull(smtpConfig, "smtpConfig");
            Contract.NotEmpty(smtpConfig.SmtpServer, "SmtpServer");
            Contract.NotNull(smtpConfig.SmtpPort, "SmtpPort");
            this.logcontext = logcontext;
            this.smtpConfig = smtpConfig;
        }

        public void Send(MailRequestSetting mailSetting)
        {
            Contract.NotNull(mailSetting, "mailSetting");
            Contract.NotNull(mailSetting.From, "From");
            Contract.NotNull(mailSetting.To, "To");
            Contract.Assert(mailSetting.To.Count() > 0, "To");
            Contract.NotEmpty(mailSetting.Subject, "Subject");
            SendMail(mailSetting.From, mailSetting.To, mailSetting.Subject, mailSetting.Body, mailSetting.Cc, mailSetting.Bcc);
        }

        private void SendMail(EmailAddress from, List<EmailAddress> to, string subject, string body, List<EmailAddress> cc = null, List<EmailAddress> bcc = null)
        {
            SmtpClient client = new SmtpClient();
            client.Host = this.smtpConfig.SmtpServer;
            client.Port = this.smtpConfig.SmtpPort;
            
            MailMessage message = new MailMessage();
            message.From = new MailAddress(from.Address, from.DisplayName);
            to.ForEach(x => message.To.Add(new MailAddress(x.Address, x.DisplayName)));
            message.Subject = subject;
            message.Body = body;
            message.BodyEncoding = System.Text.Encoding.GetEncoding("ISO-2022-JP");
            if (cc != null)
            {
                cc.ForEach(x => message.CC.Add(new MailAddress(x.Address, x.DisplayName)));
            }
            if (bcc != null)
            {
                bcc.ForEach(x => message.Bcc.Add(new MailAddress(x.Address, x.DisplayName)));
            }
            if (this.logcontext != null)
            {
                var logData = this.logcontext.CreateLogData();
                logData.LogName = "trace";
                logData.Message = string.Format("MailLog: From={0} To={1} Cc={2} Bcc={3} Subject={4}",
                                                            from.Address,
                                                            to.ToSafe().Select(m => m.Address).ConcatWith(","),
                                                            cc.ToSafe().Select(m => m.Address).ConcatWith(","),
                                                            bcc.ToSafe().Select(m => m.Address).ConcatWith(","),
                                                            subject);
                this.logcontext.Logger.Trace(logData);
            }
            client.Send(message);
        }
    }
}