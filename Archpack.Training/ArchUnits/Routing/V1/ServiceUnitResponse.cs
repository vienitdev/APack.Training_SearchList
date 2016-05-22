using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace Archpack.Training.ArchUnits.Routing.V1
{
    /// <summary>
    /// サービスユニットの実行結果に関するレスポンス情報を保持します。
    /// </summary>
    public class ServiceUnitResponse
    {
        /// <summary>
        /// 指定された <see cref="HttpStatusCode"/> を利用してインスタンスを初期化します。
        /// </summary>
        /// <param name="statusCode"></param>
        public ServiceUnitResponse(HttpStatusCode statusCode)
        {
            this.StatusCode = statusCode;
        }
        /// <summary>
        /// 実行結果を表すステータスコードを取得します。
        /// </summary>
        public HttpStatusCode StatusCode { get; private set; }
        /// <summary>
        /// 実行結果の値を取得または設定します。
        /// </summary>
        public object Data { get; set; }
    }
}