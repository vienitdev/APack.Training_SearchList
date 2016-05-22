using Archpack.Training.ArchUnits.Contracts.V1;
using Archpack.Training.ArchUnits.Pipeline.V1;
using Archpack.Training.ArchUnits.Validations.V1;
using Archpack.Training.ArchUnits.WebApiModels.V1;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;

namespace Archpack.Training.ArchUnits.WebApiExtensions.V1
{
    public static class PipeResponseExtensions
    {
        /// <summary>
        /// <see cref="PipeResponse"/> から <see cref="HttpResponseMessage"/> を作成します。
        /// </summary>
        /// <param name="pipeResponse"><see cref="HttpResponseMessage"/> を作成するための <see cref="PipeResponse"/></param>
        /// <param name="request"><see cref="HttpResponseMessage"/> を作成するための <see cref="HttpRequestMessage"/></param>
        /// <param name="checkAction">カスタムで処理を行う場合のアクション</param>
        /// <remarks>
        /// デフォルトで以下の判定を行います。
        /// <list type="list">
        ///     <item><see cref="PipeResponse.Data"/>が<see cref="null"/>の場合は、HTTPステータスコード Not Found の <see cref="HttpResponseMessage"/>を返します。</item>
        ///     <item>結果が<see cref="ValidationResult"/>で<see cref="ValidationResult.IsValid"/>が <see cref="false"/> の場合は、 HTTPステータスコード で BadRequest を返します。</item>
        ///     <item>上記以外の場合は<see cref="HttpResponseMessage" /> に <see cref="PipeResponse.StatusCode"/> と、<see cref="PipeResponse.Data"/>の値を設定してかえします。</item>
        /// </list>
        /// </remarks>
        /// <returns><see cref="HttpResponseMessage"/></returns>
        public static HttpResponseMessage CreateHttpResponse(this PipeResponse pipeResponse, HttpRequestMessage request, Func<PipeResponse, Func<HttpResponseMessage>, HttpResponseMessage> checkAction = null)
        {
            Contract.NotNull(pipeResponse, "pipeResponse");
            Contract.NotNull(request, "request");
            var defaultAction = new DefaultResponseAction(request, pipeResponse);

            if(checkAction != null)
            {
                return checkAction(pipeResponse, defaultAction.CreateHttpResponse);
            }
            return defaultAction.CreateHttpResponse();
        }

        private class DefaultResponseAction
        {
            private HttpRequestMessage request;
            private PipeResponse pipeResponse;

            public DefaultResponseAction(HttpRequestMessage request, PipeResponse pipeResponse)
            {
                this.request = request;
                this.pipeResponse = pipeResponse;
            }

            public HttpResponseMessage CreateHttpResponse()
            {
                if (pipeResponse.Data == null)
                {
                    return this.request.CreateResponse(HttpStatusCode.NotFound);
                }

                var validationResult = pipeResponse.Data as ValidationResult;
                if(validationResult != null && !validationResult.IsValid)
                {
                    return this.request.CreateResponse(HttpStatusCode.BadRequest, 
                        validationResult.CreateWebApiErrorResponse());
                }
                return this.request.CreateResponse(pipeResponse.StatusCode, pipeResponse.Data);
            }
        }
    }
}