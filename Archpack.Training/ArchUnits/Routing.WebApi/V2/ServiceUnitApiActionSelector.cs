using Archpack.Training.ArchUnits.RoleBaseAuth.V2;
using Archpack.Training.ArchUnits.Routing.V1;
using Archpack.Training.ArchUnits.Routing.WebApi.V1;
using Archpack.Training.ArchUnits.WebApiModels.V1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Archpack.Training.ArchUnits.Routing.WebApi.V2
{
    public class ServiceUnitApiActionSelector : IHttpActionSelector
    {
        HttpConfiguration configuration;

        public ServiceUnitApiActionSelector(HttpConfiguration config) {
            this.configuration = config;
        }

        public ILookup<string, HttpActionDescriptor> GetActionMapping(HttpControllerDescriptor controllerDescriptor)
        {
            throw new NotImplementedException();
        }

        public HttpActionDescriptor SelectAction(HttpControllerContext controllerContext)
        {
            var suContext = controllerContext.Request.GetServiceUnitContext();

            Archpack.Training.ArchUnits.Routing.V2.IRoutingAuthorization auth = suContext.ServiceContainer.GetService<Archpack.Training.ArchUnits.Routing.V2.IRoutingAuthorization>();
            var authResult = auth.Authorize();
            if (!authResult.IsAuthorized)
            {
                throw new HttpException((int)authResult.Status, Resources.RoleAuthorizedErrorMessage);
            }

            return new ApiControllerActionSelector().SelectAction(controllerContext);
        }
        
        private ServiceUnitResponse HandleError(ServiceUnitContext context,
                                        IUriProcessResolver resolver,
                                        ServiceUnitResponse response)
        {
            if (resolver != null)
            {
                foreach (var handler in resolver.HandleErrorPipeline)
                {
                    handler.Invoke(context, response);
                }
            }
            return response;
        }

    }
}