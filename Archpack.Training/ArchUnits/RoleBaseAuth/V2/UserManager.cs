using Archpack.Training.ArchUnits.Arcs.Authentications.V2;
using Archpack.Training.ArchUnits.Contracts.V1;
using Archpack.Training.ArchUnits.Data.Sql.V1;
using Archpack.Training.ArchUnits.RoleBaseAuth.V2.Data;
using Archpack.Training.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace Archpack.Training.ArchUnits.RoleBaseAuth.V2
{
    public class UserManager
    {
        public UserManager()
        {
                
        }

        public Employee GetUser(IIdentity identity)
        {
            EmployeeInformation empInfo = new EmployeeInformation();
            var emp = empInfo.GetUserInfo(identity);

            if (emp == null)
            {
                throw new InvalidOperationException(Resources.UserNotExist);
            }
            return emp;
        }
    }
}