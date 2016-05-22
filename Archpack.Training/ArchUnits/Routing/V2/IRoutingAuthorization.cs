using Archpack.Training.ArchUnits.RoleBaseAuth.V2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archpack.Training.ArchUnits.Routing.V2
{
    interface IRoutingAuthorization
    {
        AuthorizationResult Authorize();
    }
}
