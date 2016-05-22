using Archpack.Training.ArchUnits.Contracts.V1;
using Archpack.Training.ArchUnits.Logging.V1;
using Archpack.Training.ArchUnits.Routing.V1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace Archpack.Training.ArchUnits.Routing.V2
{
    public class UriResolver
    {
        public UriResolver()
        {

        }
        
        public ServiceUnitResponse Execute(ServiceUnitContext suContext, IUriProcessResolver resolver)
        {
            Contract.NotNull(suContext, "suContext");
            Contract.NotNull(resolver, "resolver");

            using (var logger = new TraceLogger(suContext))
            {
                return ExecuteInternal(suContext, resolver);
            }
        }

        private ServiceUnitResponse ExecuteInternal(ServiceUnitContext suContext, IUriProcessResolver resolver)
        {
            var executionType = resolver.GetExecutionType(suContext);
            ServiceUnitResponse response = null;

            if (executionType == null)
            {
                response = new ServiceUnitResponse(HttpStatusCode.NotFound);
                HandleError(suContext, resolver, response);
                return response;
            }

            var instance = resolver.CreateInstance(suContext, executionType);
            if (instance == null)
            {
                response = new ServiceUnitResponse(HttpStatusCode.NotFound);
                HandleError(suContext, resolver, response);
                return response;
            }

            var action = resolver.GetAction(suContext, instance);
            if (action == null)
            {
                response = new ServiceUnitResponse(HttpStatusCode.NotFound);
                HandleError(suContext, resolver, response);
                return response;
            }

            try
            {
                IRoutingAuthorization auth = suContext.ServiceContainer.GetService<IRoutingAuthorization>();
                var authResult = auth.Authorize();
                if (!authResult.IsAuthorized)
                {
                    response = new ServiceUnitResponse(authResult.Status);
                    HandleError(suContext, resolver, response);
                    return response;
                }

                response = resolver.InvokeAction(suContext, action);
                if (response == null)
                {
                    return new ServiceUnitResponse(HttpStatusCode.NotFound);
                }

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    HandleError(suContext, resolver, response);
                }
            }
            catch (Exception ex)
            {
                WriteErrorLog(suContext, ex);
                return HandleError(suContext, resolver, new ServiceUnitResponse(HttpStatusCode.InternalServerError) { Data = ex });
            }

            return response;
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

        private void WriteErrorLog(ServiceUnitContext serviceUnitContext, Exception ex)
        {
            var data = new LogData()
            {
                Exception = ex,
                Uri = serviceUnitContext.Request.Path,
                User = serviceUnitContext.User == null ? null : serviceUnitContext.User.Identity.Name,
                LogName = "error",
                LogId = serviceUnitContext.Id
            };
            serviceUnitContext.LogContext.Logger.Error(data);
        }
    }
}