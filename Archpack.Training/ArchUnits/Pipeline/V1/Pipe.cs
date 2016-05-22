using Archpack.Training.ArchUnits.Container.V1;
using Archpack.Training.ArchUnits.Contracts.V1;
using System;
using System.Collections.Generic;
using System.Net;
using Archpack.Training.Properties;

namespace Archpack.Training.ArchUnits.Pipeline.V1
{

    /// <summary>
    /// パイプラインの登録、実行を行います。
    /// </summary>
    public class Pipe
    {
        private List<IPipeAction> actions = new List<IPipeAction>();
        private PipeContext context;

        /// <summary>
        /// 指定された <see cref="IServiceContainer"/> を利用してインスタンスを初期化します。 
        /// </summary>
        /// <param name="container">インスタンスを管理する <see cref="IServiceContainer"/></param>
        public Pipe(IServiceContainer container)
        {
            this.context = new PipeContext(container);
        }
        /// <summary>
        /// 指定された <see cref="PipeContext"/>を利用してインスタンスを初期化します。
        /// </summary>
        /// <param name="context"><see cref="PipeContext"/></param>
        public Pipe(PipeContext context)
        {
            Contract.NotNull(context, "context");

            this.context = context;
        }

        /// <summary>
        /// 指定されたアクション（メソッド）をパイプライン処理として登録します。
        /// </summary>
        /// <typeparam name="T">アクションの型</typeparam>
        /// <returns>自身のインスタンス</returns>
        public Pipe Use<T>() where T : IPipeAction, new()
        {
            IPipeAction action = new T();
            actions.Add(action);

            return this;
        }

        /// <summary>
        /// 指定されたアクション（メソッド）をパイプライン処理として登録します。
        /// </summary>
        /// <param name="action">アクション</param>
        /// <returns>自身のインスタンス</returns>
        public Pipe Use(IPipeAction action)
        {
            Contract.NotNull(action, "action");
            actions.Add(action);

            return this;
        }

        /// <summary>
        /// <see cref="PipeRequest"/> のインスタンスを作成します。
        /// </summary>
        /// <param name="data">最初の要求で渡されるデータ</param>
        /// <returns><see cref="PipeRequest"/></returns>
        public PipeRequest CreateRequest(object data)
        {
            return new PipeRequest(this.context, data);
        }

        /// <summary>
        /// <see cref="PipeRequest"/> のインスタンスを作成します。
        /// </summary>
        /// <returns><see cref="PipeRequest"/><see cref="PipeRequest"/></returns>
        public PipeRequest CreateRequest()
        {
            return new PipeRequest(this.context);
        }

        /// <summary>
        /// パイプライン処理として登録されたアクション（メソッド）を実行します。
        /// </summary>
        /// <param name="request"><see cref="PipeRequest"/></param>
        /// <returns>アクション実行結果を含んだ <see cref="PipeResponse"/></returns>
        public PipeResponse Execute(PipeRequest request)
        {
            Contract.NotNull(request, "request");
            if (actions.Count == 0)
            {
                throw new InvalidOperationException(Resources.NoActionRegister);
            }
            PipeRequest nextRequest = request;
            PipeResponse response = null;

            foreach (var action in actions)
            {
                if (action.OnStart != null)
                {
                    action.OnStart();
                }

                response = action.Execute(nextRequest);

                if (response.StatusCode != HttpStatusCode.OK && !action.IsResumePipe)
                {
                    if (action.OnError != null)
                    {
                        action.OnError();
                    }

                    return response;
                }
                else
                {
                    if (action.OnEnd != null)
                    {
                        action.OnEnd();
                    }
                }

                nextRequest = CreateRequestFromResponse(response);
            }

            return response;
        }

        /// <summary>
        /// レスポンスからリクエストを作成します。
        /// </summary>
        /// <param name="response">リクエストを作成するもととなる<see cref="PipeResponse"/></param>
        /// <returns><see cref="PipeRequest"/></returns>
        public PipeRequest CreateRequestFromResponse(PipeResponse response)
        {
            Contract.NotNull(response, "response");
            PipeRequest request = new PipeRequest(response.Context, response.Data);
            return request;
        }
    }
}