using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Archpack.Training.ArchUnits.Routing.Owin.V1;
using System.Web.Http;
using Archpack.Training.ArchUnits.Routing.WebApi.V1;

namespace Archpack.Training.ArchUnits.Routing.WebApi.Owin.V1
{

    public class ServiceUnitRequestDispatcher : HttpMessageHandler
    {
        private const string HttpContextEnvironmentKey = "System.Web.HttpContextBase";
        
        private readonly HttpConfiguration _configuration;

        private readonly HttpMessageInvoker _defaultInvoker;

        public ServiceUnitRequestDispatcher(HttpConfiguration configuration)
		{
			this._configuration = configuration;
            this._defaultInvoker = new HttpMessageInvoker(new ServiceUnitRoutingDispatcher(configuration));
		}

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            //Owin の Request から ServiceUnitRequest を取り出して
            //HttpRequestMessage に設定する処理
            //Owin への依存の分離点
            var owinContext = request.GetOwinContext();
            var suContext = owinContext.Request.GetServiceUnitContext();
            var httpContext = owinContext.Environment.Keys.Contains(HttpContextEnvironmentKey) ?
                owinContext.Environment[HttpContextEnvironmentKey] as HttpContextBase : null;
            if (httpContext == null && HttpContext.Current != null)
            {
                httpContext = new HttpContextWrapper(HttpContext.Current);
            }
            if (httpContext != null)
            {
                request.Properties.Add("MS_HttpContext", new HttpContextWrapper(HttpContext.Current));
            }
            request.SetServiceUnitContext(suContext);
            return this._defaultInvoker.SendAsync(request, cancellationToken);
        }
    }
}