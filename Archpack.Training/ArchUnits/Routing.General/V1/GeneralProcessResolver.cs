using Archpack.Training.ArchUnits.Routing.V1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web;

namespace Archpack.Training.ArchUnits.Routing.General.V1
{
    public class GeneralProcessResolver : IUriProcessResolver
    {
        public List<Func<ServiceUnitContext, ServiceUnitResponse, ServiceUnitResponse>> HandleErrorPipeline { get; private set; }

        public Type GetExecutionType(ServiceUnitContext suContext)
        {
            return this.MapPath(suContext);
        }

        private Type MapPath(ServiceUnitContext suContext)
        {
            var classAbstractPath = string.Format("/{0}/{1}/Lib/{2}",
                suContext.ServiceUnitName, suContext.Version,
                suContext.Request.ProcessPath.Split(new[] { '/' })[1]);

            var typePath = suContext.ServiceContainer.GetService<ITypePath>();
            return typePath.GetType(classAbstractPath);
        }

        public object CreateInstance(ServiceUnitContext suContext, Type instanceType)
        {
            suContext.ServiceContainer.AddTransient(instanceType);
            return suContext.ServiceContainer.GetService(instanceType);
        }

        public ActionDefinition GetAction(ServiceUnitContext suContext, object instance)
        {
            var uriMethodName = suContext.Request.ProcessPath.Split(new[] { '/' })[2];
            var methodInfo = instance.GetType().GetMethod(uriMethodName, BindingFlags.Instance | BindingFlags.Public);
            if (methodInfo == null)
            {
                return null;
            }
            return new ActionDefinition()
            {
                Method = methodInfo,
                Instance = instance
            };
        }

        public ServiceUnitResponse InvokeAction(ServiceUnitContext suContext, ActionDefinition action)
        {
            var response = new ServiceUnitResponse(HttpStatusCode.OK);
            response.Data = action.Method.Invoke(action.Instance, new object[0]);

            return response;
        }
    }
}