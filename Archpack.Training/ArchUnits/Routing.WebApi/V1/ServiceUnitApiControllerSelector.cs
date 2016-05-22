using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;

namespace Archpack.Training.ArchUnits.Routing.WebApi.V1
{
    public sealed class ServiceUnitApiControllerSelector: IHttpControllerSelector
    {
        private readonly HttpConfiguration configuration;

        public ServiceUnitApiControllerSelector(HttpConfiguration config)
        {
            this.configuration = config;
        }

        public IDictionary<string, HttpControllerDescriptor> GetControllerMapping()
        {
            throw new NotImplementedException();
        }

        public HttpControllerDescriptor SelectController(HttpRequestMessage request)
        {
            var suContext = request.GetServiceUnitContext();
            var resolver = request.GetUriProcessResolver();
            if (suContext == null || resolver == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }

            var type = resolver.GetExecutionType(suContext);
            return new HttpControllerDescriptor(this.configuration, type.FullName, type);

        }
    }
}