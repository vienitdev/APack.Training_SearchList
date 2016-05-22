using Archpack.Training.ArchUnits.Logging.V1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archpack.Training.ArchUnits.Mail.V1
{
    interface IMailSenderFactory
    {
        IMailSender Create(LogContext logContext);
    }
}
