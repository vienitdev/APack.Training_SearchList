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
    public sealed class ServiceUnitApiControllerActivator : IHttpControllerActivator
    {

        private readonly HttpConfiguration configuration;

        public ServiceUnitApiControllerActivator(HttpConfiguration config)
        {
            this.configuration = config;
        }


        public IHttpController Create(HttpRequestMessage request, HttpControllerDescriptor controllerDescriptor, Type controllerType)
        {
            var suContext = request.GetServiceUnitContext();
            var resolver = request.GetUriProcessResolver();
            if (suContext == null || resolver == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }

            var instance = resolver.CreateInstance(suContext, controllerType);
            return (IHttpController)instance;
        }
    }
}