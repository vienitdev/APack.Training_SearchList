using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Archpack.Training.ArchUnits.Identity.V1
{
    public abstract class IdentityUser<T>
    {

        public T IdentityKey { get; set; }

        public string PassKey { get; set; }

    }
}