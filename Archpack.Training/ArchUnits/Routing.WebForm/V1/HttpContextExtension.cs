using Archpack.Training.ArchUnits.Routing.V1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Archpack.Training.ArchUnits.Routing.WebForm.V1
{
    public static class HttpContextExtension
    {
        private static readonly string ServiceUnitContextKey = "archpack.serviceunitcontext";

        public static ServiceUnitContext GetServiceUnitContext(this HttpContext context)
        {
            if (!context.Items.Contains(ServiceUnitContextKey))
            {
                return null;
            }
            return context.Items[ServiceUnitContextKey] as ServiceUnitContext;
        }

        public static ServiceUnitContext GetServiceUnitContext(this HttpContextBase context)
        {
            if (!context.Items.Contains(ServiceUnitContextKey))
            {
                return null;
            }
            return context.Items[ServiceUnitContextKey] as ServiceUnitContext;
        }

        public static void SetServiceUnitContext(this HttpContext context, ServiceUnitContext target)
        {
            context.Items.Add(ServiceUnitContextKey, target);
        }

        public static void SetServiceUnitContext(this HttpContextBase context, ServiceUnitContext target)
        {
            context.Items.Add(ServiceUnitContextKey, target);
        }
    }
}