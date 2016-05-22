using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Archpack.Training.ArchUnits.RoleBaseAuth.V2.Data
{
    public class ParentTarget
    {
        public int TargetID { get; set; }
        public int? ParentTargetID { get; set; }
        public int Level { get; set; }
    }
}