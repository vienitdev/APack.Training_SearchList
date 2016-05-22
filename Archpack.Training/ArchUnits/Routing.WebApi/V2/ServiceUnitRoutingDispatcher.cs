using Archpack.Training.ArchUnits.Routing.V1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.Http.Hosting;
using System.Web.Http.Routing;
using Archpack.Training.ArchUnits.Routing.WebApi.V1;

namespace Archpack.Training.ArchUnits.Routing.WebApi.V2
{
    public class ServiceUnitRoutingDispatcher : HttpMessageHandler
    {
        private readonly HttpConfiguration _configuration;

        private readonly HttpMessageInvoker _defaultInvoker;

        public ServiceUnitRoutingDispatcher(HttpConfiguration configuration)
        {
            this._configuration = configuration;
            this._defaultInvoker = new HttpMessageInvoker(new HttpControllerDispatcher(configuration));
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var suContext = request.GetServiceUnitContext();
            if (suContext == null || suContext.Request == null ||
                !suContext.Request.ProcessType.Equals("api", StringComparison.InvariantCultureIgnoreCase))
            {
                request.Properties.Add(HttpPropertyKeys.NoRouteMatched, true);
                return Task.FromResult<HttpResponseMessage>(request.CreateErrorResponse(HttpStatusCode.NotFound, new Exception()));
            }

            using (var trace = new TraceLogger(suContext))
            {
                request.SetUriProcessResolver(new V2.WebApiProcessResolver());
                IHttpRouteData routeData = request.GetRouteData();
                if (routeData == null)
                {
                    var segments = suContext.Request.ProcessPath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

                    var route = new HttpRoute();
                    routeData = new HttpRouteData(route);
                    if (segments.Length > 1)
                    {
                        routeData.Values.Add("action", segments[1]);
                    }
                    request.SetRouteData(routeData);
                }
                routeData.RemoveOptionalRoutingParameters();

                return this._defaultInvoker.SendAsync(request, cancellationToken);

            }
        }
    }
}