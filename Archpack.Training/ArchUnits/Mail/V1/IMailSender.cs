using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archpack.Training.ArchUnits.Logging.V1;

namespace Archpack.Training.ArchUnits.Mail.V1
{
    public interface IMailSender
    {
        void Send(MailRequestSetting mailSetting);
    }
}
