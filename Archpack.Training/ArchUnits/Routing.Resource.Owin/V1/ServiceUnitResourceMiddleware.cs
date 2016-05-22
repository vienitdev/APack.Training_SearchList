using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Compilation;
using AppFunc = System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>;
using System.Globalization;
using System.Text.RegularExpressions;
using Archpack.Training.ArchUnits.Routing.V1;
using Archpack.Training.ArchUnits.Routing.Resource.V1;
using Archpack.Training.ArchUnits.Routing.Owin.V1;

namespace Archpack.Training.ArchUnits.Routing.Resource.Owin.V1
{
    public class ServiceUnitResourceMiddleware
    {
        private AppFunc next;
        private Func<string, CultureInfo, string, string> javaScriptFormatter;
        public ServiceUnitResourceMiddleware(AppFunc next,
            Func<string, CultureInfo, string, string> javaScriptFormatter)
        {
            this.next = next;
            this.javaScriptFormatter = javaScriptFormatter;
        }

        public async Task Invoke(IDictionary<string, object> environment)
        {
            var context = new OwinContext(environment);
            var suContext = context.Request.GetServiceUnitContext();
            if (suContext == null || suContext.Request == null || !suContext.Request.ProcessType.Equals("resource", StringComparison.InvariantCultureIgnoreCase))
            {
                await next.Invoke(environment);
                return;
            }

            var resolver = new UriResolver();
            var response = resolver.Execute(suContext, new ResourceProcessResolver());
            if (response == null)
            {
                await next.Invoke(environment);
                return;
            }
            var data = response.Data as ResourceResponseData ?? new ResourceResponseData(null, null);
            if (data.Resources == null || data.Culture == null)
            {
                context.Response.StatusCode = 404;
                return;
            }
            var resultString = CreateJsonString(data.Resources, data.Culture);
            context.Response.StatusCode = 200;
            context.Response.ContentType = "application/json";

            if (this.javaScriptFormatter != null)
            {
                var resourceName = suContext.Request.ProcessPath.Split('/')[1];
                resultString = this.javaScriptFormatter(resultString, data.Culture, resourceName);
                context.Response.ContentType = "application/javascript";
            }

            await context.Response.WriteAsync(resultString);
        }

        private string CreateJsonString(IDictionary<string, string> strings, CultureInfo culture)
        {
            var keyValues = new List<string>();
            foreach (var keyValue in strings)
            {
                keyValues.Add(string.Format("\"{0}\":\"{1}\"", NormalizeToJsString(keyValue.Key), NormalizeToJsString(keyValue.Value)));
            }
            return "{" + String.Join(",", keyValues) + "}";
        }

        private string NormalizeToJsString(string value)
        {
            return Regex.Replace(value, "[\\\\\"']", match =>
            {
                return "\\" + match.Value;
            }).Replace("\r", "\\r").Replace("\n", "\\n");
        }
    }
}