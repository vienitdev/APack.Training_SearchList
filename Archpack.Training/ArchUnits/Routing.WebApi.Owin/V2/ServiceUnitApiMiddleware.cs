using Archpack.Training.ArchUnits.Routing.V2;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Owin;

using AppFunc = System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>;
using Archpack.Training.ArchUnits.Routing.WebApi.V2;
using Newtonsoft.Json;
using Archpack.Training.ArchUnits.Routing.Owin.V1;
using Archpack.Training.ArchUnits.Routing.WebApi.V1;

namespace Archpack.Training.ArchUnits.Routing.WebApi.Owin.V2
{

    public class ServiceUnitApiMiddleware
    {
        private AppFunc next;
        private HttpMessageHandlerAdapter adapter;

        public ServiceUnitApiMiddleware(AppFunc next)
        {
            this.next = next;
            this.adapter = new HttpMessageHandlerAdapter(new DelegateOwinMiddleware(next),
                CreateHttpMesageHandlerOptions());
        }

        public async Task Invoke(IDictionary<string, object> environment)
        {
            var context = new OwinContext(environment);
            var suContext = context.Request.GetServiceUnitContext();
            if (suContext == null || suContext.Request == null || 
                !suContext.Request.ProcessType.Equals("api", StringComparison.InvariantCultureIgnoreCase))
            {
                await next.Invoke(environment);
                return;
            }

            await this.adapter.Invoke(new OwinContext(environment));
        }

        private static HttpMessageHandlerOptions CreateHttpMesageHandlerOptions()
        {
            var config = new HttpConfiguration();


            config.Services.Replace(typeof(IExceptionHandler), new WebApi.V2.ApiExceptionHandler());
            config.Services.Replace(typeof(IHttpControllerSelector), new ServiceUnitApiControllerSelector(config));
            config.Services.Replace(typeof(IHttpControllerActivator), new ServiceUnitApiControllerActivator(config));
            //authorize
            config.Services.Replace(typeof(IHttpActionSelector), new ServiceUnitApiActionSelector(config));

            config.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            config.Formatters.JsonFormatter.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;


            HttpServer httpServer = new HttpServer(config, new ServiceUnitRequestDispatcher(config));
            ServicesContainer services = config.Services;

            return new HttpMessageHandlerOptions()
            {
                MessageHandler = httpServer,
                BufferPolicySelector = services.GetHostBufferPolicySelector() ?? new OwinBufferPolicySelector(),
                ExceptionLogger = ExceptionServices.GetLogger(services), 
                ExceptionHandler = ExceptionServices.GetHandler(services), 
            };
        }

        private class DelegateOwinMiddleware : OwinMiddleware
        {
            private AppFunc selfNext;
            public DelegateOwinMiddleware(AppFunc next)
                : base(null)
            {
                this.selfNext = next;
            }
            public override Task Invoke(IOwinContext context)
            {
                return selfNext.Invoke(context.Environment);
            }
        }
    }
}