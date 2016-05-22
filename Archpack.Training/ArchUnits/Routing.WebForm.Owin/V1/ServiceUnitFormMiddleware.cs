using Archpack.Training.ArchUnits.Routing.V1;
using Archpack.Training.ArchUnits.Routing.Owin.V1;

using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Compilation;
using AppFunc = System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>;
using Archpack.Training.ArchUnits.Routing.WebForm.V1;

namespace Archpack.Training.ArchUnits.Routing.WebForm.Owin.V1
{
    public class ServiceUnitFormMiddleware
    {
        private const string HttpContextEnvironmentKey = "System.Web.HttpContextBase";

        private AppFunc next;

        public ServiceUnitFormMiddleware(AppFunc next)
        {
            this.next = next;
        }

        public async Task Invoke(IDictionary<string, object> environment)
        {
            var context = new OwinContext(environment);
            var suContext = context.Request.GetServiceUnitContext();
            if (suContext == null || suContext.Request == null|| !suContext.Request.ProcessType.Equals("page", StringComparison.InvariantCultureIgnoreCase))
            {
                await next.Invoke(environment);
                return;
            }

            var httpContext = context.Environment.Keys.Contains(HttpContextEnvironmentKey) ?
                              context.Environment[HttpContextEnvironmentKey] as HttpContextBase : null;
            if (httpContext == null && HttpContext.Current != null)
            {
                httpContext = new HttpContextWrapper(HttpContext.Current);
            }
            if (httpContext != null)
            {
                httpContext.SetServiceUnitContext(suContext);
            }

            var resolver = new UriResolver();
            var response = resolver.Execute(suContext, new WebFormProcessResolver());
            if (response != null)
            {
                return;
            }

            await next.Invoke(environment);
        }

    }
}