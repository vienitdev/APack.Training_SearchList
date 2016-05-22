using System;
using System.Collections.Generic;

namespace Archpack.Training.ArchUnits.Mail.V1
{
    public class SmtpMailConfiguration
    {
        public string SmtpServer { get; set; }

        public int SmtpPort { get; set; }

        public string SendType { get; set; }

        public string FileOutputDir { get; set; }

        public List<string> FixAddresses { get; set; }
    }
}