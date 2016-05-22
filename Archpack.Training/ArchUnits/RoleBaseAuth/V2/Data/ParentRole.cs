using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Archpack.Training.ArchUnits.RoleBaseAuth.V2.Data
{
    public class ParentRole
    {
        public int RoleID { get; set; }
        public int? ParentRoleID { get; set; }
        public int Level { get; set; }
    }
}