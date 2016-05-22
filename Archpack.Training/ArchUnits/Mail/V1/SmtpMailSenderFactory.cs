using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Archpack.Training.ArchUnits.Logging.V1;
using Archpack.Training.ArchUnits.Configuration.V1;
using Archpack.Training.ArchUnits.Contracts.V1;

namespace Archpack.Training.ArchUnits.Mail.V1
{
    public class SmtpMailSenderFactory : IMailSenderFactory
    {
        public IMailSender Create(LogContext logContext)
        {
            var config = ServiceConfigurationLoader.Load();
            var smtpConfig = config.Raw["smtpMail"].ToObject<SmtpMailConfiguration>();
            if(smtpConfig == null || string.IsNullOrWhiteSpace(smtpConfig.SendType))
            {
                return null;
            }
            if (smtpConfig.SendType.ToUpper() == "SEND")
            {
                return new SmtpMailSender(smtpConfig, logContext);                
            }
            else if (smtpConfig.SendType.ToUpper() == "FIX")
            {
                return new FixAddressMailSender(smtpConfig, logContext);
            }
            else if (smtpConfig.SendType.ToUpper() == "FILE")
            {
                return new FileOutputMailSender(smtpConfig);
            }
            return null;
        }
    }
}