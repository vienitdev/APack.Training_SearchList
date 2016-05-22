using Archpack.Training.ArchUnits.Container.V1;
using Archpack.Training.ArchUnits.Contracts.V1;
using System.Collections.Generic;
using System.Net;

namespace Archpack.Training.ArchUnits.Pipeline.V1
{
    /// <summary>
    /// パイプライン処理のコンテキストクラスです。
    /// </summary>
    public class PipeContext
    {
        /// <summary>
        /// パイプライン処理のコンテキストを <see cref="Archpack.Training.ArchUnits.Container.V1.IServiceContainer"/> のインスタンスを指定して初期化します。
        /// </summary>
        /// <param name="container"></param>
        public PipeContext(IServiceContainer container)
        {
            Contract.NotNull(container, "container");
            this.Container = container;
            this.Items = new Dictionary<string, object>();
            this.ResponseData = new Dictionary<string, object>();
        }

        /// <summary>
        /// インスタンスを管理する<see cref="IServiceContainer"/>を取得します。
        /// </summary>
        public IServiceContainer Container { get; private set; }


        public object RequestData { get; set; }

        public Dictionary<string, object> ResponseData { get; private set; }

        /// <summary>
        /// パイプライン中に受け渡しをするカスタムデータのコレクションを取得します。
        /// </summary>
        public Dictionary<string, object> Items { get; private set; }
        
    }

    /// <summary>
    /// パイプライン処理に渡すリクエスト情報を定義するクラスです。
    /// </summary>
    public class PipeRequest
    {
        /// <summary>
        /// パイプライン処理に渡すリクエスト情報を定義するクラスのコンストラクター。
        /// </summary>
        /// <param name="context"></param>
        /// <param name="data"></param>
        public PipeRequest(PipeContext context, object data)
        {
            Contract.NotNull(context, "context");
            this.Context = context;
            this.Data = data;
        }

        public PipeRequest(PipeContext context)
        {
            Contract.NotNull(context, "context");
            this.Context = context;
        }

        public PipeContext Context { get; private set; }

        public object Data { get; set; }

        /// <summary>
        /// 応答を作成する。
        /// </summary>
        /// <param name="statusCode"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public PipeResponse CreateResponse(HttpStatusCode statusCode, object data = null)
        {
            PipeResponse response = new PipeResponse(this.Context, statusCode);
            response.Data = data;

            return response;
        }
    }

    /// <summary>
    /// パイプライン処理結果のレスポンス情報を定義するクラスです。
    /// </summary>
    public class PipeResponse
    {
        /// <summary>
        /// パイプライン処理結果のレスポンス情報を定義するクラスのコンストラクター。
        /// </summary>
        /// <param name="context"></param>
        /// <param name="statusCode"></param>
        public PipeResponse(PipeContext context, HttpStatusCode statusCode)
        {
            Contract.NotNull(context, "context");
            this.Context = context;
            this.StatusCode = statusCode;
        }

        public PipeContext Context { get; private set; }

        public HttpStatusCode StatusCode { get; private set; }

        public object Data { get; set; }

    }
}