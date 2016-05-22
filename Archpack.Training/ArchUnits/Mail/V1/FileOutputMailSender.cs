using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Archpack.Training.ArchUnits.Configuration.V1;
using Archpack.Training.ArchUnits.Logging.V1;
using System.IO;
using Archpack.Training.ArchUnits.Contracts.V1;
using Archpack.Training.ArchUnits.Path.V1;
using Archpack.Training.ArchUnits.Environment.V1;
using Archpack.Training.ArchUnits.Container.V1;

namespace Archpack.Training.ArchUnits.Mail.V1
{
    public class FileOutputMailSender : IMailSender
    {
        private SmtpMailConfiguration smtpConfig;
        private string outputDirectory;

        public FileOutputMailSender(SmtpMailConfiguration smtpConfig)
        {
            Contract.NotNull(smtpConfig, "smtpConfig");
            Contract.NotNull(smtpConfig.FileOutputDir, "FileOutputDir");

            outputDirectory = GlobalContainer.GetService<IApplicationEnvironment>().MapPath(smtpConfig.FileOutputDir);
            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }
            this.smtpConfig = smtpConfig;
        }

        public void Send(MailRequestSetting mailSetting)
        {
            //Validation mailSetting
            Contract.NotNull(mailSetting, "mailSetting");
            Contract.NotNull(mailSetting.From, "From");
            Contract.NotNull(mailSetting.To, "To");
            Contract.Assert(mailSetting.To.Count() > 0, "To");

            DateTime now = DateTime.Now;
            string filename = String.Format("{0:yyyyMMddHHmmss}_{1}.txt", now, Guid.NewGuid().ToString().Substring(0, 6));

            //Format property to output
            string to = String.Join(",", mailSetting.To.Select(t => String.Format("<{0}/{1}>", t.Address, t.DisplayName)));
            string cc = mailSetting.Cc == null ? "" : String.Join(",", mailSetting.Cc.Select(t => String.Format("<{0}/{1}>", t.Address, t.DisplayName)));
            string bcc = mailSetting.Bcc == null ? "" : String.Join(",", mailSetting.Bcc.Select(t => String.Format("<{0}/{1}>", t.Address, t.DisplayName)));

            //Modify content to output file
            StringBuilder content = new StringBuilder();
            content.AppendFormat("RequestTime: {0}", now.ToString("yyyy/MM/dd HH:mm:ss"));
            content.AppendLine().AppendFormat("From: {0}", mailSetting.From.Address);
            content.AppendLine().AppendFormat("To: {0}", to);
            content.AppendLine().AppendFormat("CC: {0}", cc);
            content.AppendLine().AppendFormat("BCC: {0}", bcc);
            content.AppendLine().AppendFormat("Subject: {0}", mailSetting.Subject);
            content.AppendLine().AppendFormat("Body: {0}{1}", System.Environment.NewLine, mailSetting.Body);

            //Output File
            File.WriteAllText(System.IO.Path.Combine(outputDirectory, filename), content.ToString());
        }
    }
}