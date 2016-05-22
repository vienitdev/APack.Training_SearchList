using Archpack.Training.ArchUnits.Logging.V1;
using Archpack.Training.ArchUnits.Routing.V1;
using Archpack.Training.ArchUnits.WebApiModels.V1;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;

namespace Archpack.Training.ArchUnits.Routing.WebApi.V1
{
    public class WebApiProcessResolver: IUriProcessResolver
    {
        public WebApiProcessResolver()
        {
            BuildHandleErrorPipeLine();
        }

        private void BuildHandleErrorPipeLine()
        {
            HandleErrorPipeline = new List<Func<ServiceUnitContext, ServiceUnitResponse, ServiceUnitResponse>>();

            
            HandleErrorPipeline.Add((suContext, response) => {
                Exception exception = response.Data as Exception;
                
                HttpStatusCode statusCode = HttpStatusCode.InternalServerError;

                if ((exception is DbUpdateConcurrencyException) || (exception.InnerException is DbUpdateConcurrencyException))
                {
                    statusCode = HttpStatusCode.Conflict;
                }
                else if (exception is DbEntityValidationException)
                {
                    statusCode = HttpStatusCode.BadRequest;
                }

                WebApiErrorResponse apiError = WebApiErrorResponse.Create(exception);
                ServiceUnitResponse newResponse = new ServiceUnitResponse(statusCode)
                {
                    Data = apiError
                };

                return newResponse;
            });
        }

        public List<Func<ServiceUnitContext, ServiceUnitResponse, ServiceUnitResponse>> HandleErrorPipeline { get; private set; }

        public Type GetExecutionType(ServiceUnitContext suContext)
        {
            return this.MapPath(suContext);
        }

        private Type MapPath(ServiceUnitContext suContext)
        {
            var classAbstractPath = string.Format("/{0}/{1}/{2}/Api/{3}Controller",
                suContext.ServiceUnitName, suContext.Version, suContext.Role,
                suContext.Request.ProcessPath.Split(new[] { '/' })[1]);
            var typePath = suContext.ServiceContainer.GetService<ITypePath>();
            return typePath.GetType(classAbstractPath);
        }

        public object CreateInstance(ServiceUnitContext suContext, Type instanceType)
        {
            var container = suContext.ServiceContainer;
            container.AddTransient(instanceType);
            return container.GetService(instanceType);
        }

        public ActionDefinition GetAction(ServiceUnitContext suContext, object instance)
        {
            throw new NotImplementedException();
        }

        public ServiceUnitResponse InvokeAction(ServiceUnitContext suContext, ActionDefinition action)
        {
            throw new NotImplementedException();
        }
    }
}