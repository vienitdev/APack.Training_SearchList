using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Archpack.Training.ArchUnits.Routing.SignalR.V1
{
    public abstract class ServiceUnitPersistentConnection : PersistentConnection
    {
        protected override sealed bool AuthorizeRequest(IRequest request)
        {
            return base.AuthorizeRequest(request);
        }

    }
}