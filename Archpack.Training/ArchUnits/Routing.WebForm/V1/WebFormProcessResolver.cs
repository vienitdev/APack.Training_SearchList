using Archpack.Training.ArchUnits.Routing.V1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web;
using System.Web.Compilation;
using System.Web.UI;

namespace Archpack.Training.ArchUnits.Routing.WebForm.V1
{
    public class WebFormProcessResolver : IUriProcessResolver
    {

        public WebFormProcessResolver()
        {
            BuildHandleErrorPipeLine();
        }

        private void BuildHandleErrorPipeLine()
        {
            HandleErrorPipeline = new List<Func<ServiceUnitContext, ServiceUnitResponse, ServiceUnitResponse>>();

            HandleErrorPipeline.Add((suContext, response) =>
            {
                HttpContextBase httpContext = new HttpContextWrapper(HttpContext.Current);

                string statusCode = ((int)response.StatusCode).ToString();

                if (suContext.Configuration.ErrorPages.ContainsKey(statusCode)) 
                {                    
                    var errorPage = suContext.Configuration.ErrorPages[statusCode];
                    if (!string.IsNullOrEmpty(errorPage.RedirectUrl))
                    {
                        httpContext.Items["RedirectUrl"] = VirtualPathUtility.ToAbsolute(errorPage.RedirectUrl);                 
                    }
                    httpContext.Response.StatusCode = (int)response.StatusCode;
                    var redirectMode = (errorPage.RedirectMode ?? "").ToLower();
                    if (redirectMode == "rewrite")
                    {
                        httpContext.Server.Transfer(errorPage.Url);
                    }
                    else
                    {
                        httpContext.Response.Redirect(errorPage.Url);
                    }
                }

                return response;
            });
        }

        public List<Func<ServiceUnitContext, ServiceUnitResponse, ServiceUnitResponse>> HandleErrorPipeline { get; private set; }

        public Type GetExecutionType(ServiceUnitContext suContext)
        {
            var info = suContext;
            var pageName = string.Format("~/ServiceUnits/{0}/{1}/{2}/Pages/{3}.aspx",
                suContext.ServiceUnitName, suContext.Version,
                suContext.Role, suContext.Request.ProcessPath.Split(new[] { '/' })[1]);

            var factory = BuildManager.GetObjectFactory(pageName, false);
            if (factory != null)
            {
                return BuildManager.GetCompiledType(pageName);
            }
            return null;
        }

        public object CreateInstance(ServiceUnitContext suContext, Type instanceType)
        {
            suContext.ServiceContainer.AddTransient(instanceType);
            var result = suContext.ServiceContainer.GetService(instanceType) as IHttpHandler;
            return result;
        }

        public ActionDefinition GetAction(ServiceUnitContext suContext, object instance)
        {
            return new ActionDefinition()
            {
                Method = instance.GetType().GetMethod("ProcessRequest", BindingFlags.Public | BindingFlags.Instance),
                Instance = instance
            };
        }

        public ServiceUnitResponse InvokeAction(ServiceUnitContext suContext, ActionDefinition action)
        {
            var handler = action.Instance as IHttpHandler;
            handler.ProcessRequest(HttpContext.Current);
            return new ServiceUnitResponse(HttpStatusCode.OK);
        }

    }
}