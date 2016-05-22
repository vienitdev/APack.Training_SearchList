using Archpack.Training.ArchUnits.Routing.V1;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Archpack.Training.ArchUnits.Routing.Owin.V1
{
    public static class OwinRequestExtension
    {
        private static readonly string ServiceUnitContextKey = "archpack.serviceunitcontext";

        public static ServiceUnitContext GetServiceUnitContext(this IOwinRequest request)
        {
            return request.Get<ServiceUnitContext>(ServiceUnitContextKey);
        }
        public static void SetServiceUnitContext(this IOwinRequest request, ServiceUnitContext suContext)
        {
            request.Set<ServiceUnitContext>(ServiceUnitContextKey, suContext);
        }
    }
}