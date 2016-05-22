using Archpack.Training.ArchUnits.Routing.V1;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace Archpack.Training.ArchUnits.Routing.WebApi.V1
{
    public static class HttpRequestMessageExtension
    {
        private static readonly string ServiceUnitContextKey = "archpack.serviceunitcontext";
        private static readonly string UriProcessResolverKey = "archpack.uriprocessresolver";

        public static ServiceUnitContext GetServiceUnitContext(this HttpRequestMessage request)
        {
            object target;
            if (!request.Properties.TryGetValue(ServiceUnitContextKey, out target))
            {
                return null;
            }
            return target as ServiceUnitContext;
        }
        public static void SetServiceUnitContext(this HttpRequestMessage request, ServiceUnitContext target)
        {
            request.Properties.Add(ServiceUnitContextKey, target);
        }

        public static IUriProcessResolver GetUriProcessResolver(this HttpRequestMessage request)
        {
            object target;
            if (!request.Properties.TryGetValue(UriProcessResolverKey, out target))
            {
                return null;
            }
            return target as IUriProcessResolver;
        }

        public static void SetUriProcessResolver(this HttpRequestMessage request, IUriProcessResolver target)
        {
            request.Properties.Add(UriProcessResolverKey, target);
        }
    }
}