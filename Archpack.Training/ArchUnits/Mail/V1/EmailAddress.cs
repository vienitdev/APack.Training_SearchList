using System;
using System.Collections.Generic;
using System.Linq;

namespace Archpack.Training.ArchUnits.Mail.V1
{
    public class EmailAddress
    {
        public EmailAddress()
        {

        }

        public EmailAddress(string address): this(address, string.Empty)
        {
            
        }
        
        public EmailAddress(string address, string displayName)
        {
            this.Address = address;
            this.DisplayName = displayName;
        }

        public string Address { get; set; }

        public string DisplayName { get; set; }
    }
}