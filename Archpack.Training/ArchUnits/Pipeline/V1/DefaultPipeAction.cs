using Archpack.Training.ArchUnits.Contracts.V1;
using System;
using System.Net;

namespace Archpack.Training.ArchUnits.Pipeline.V1
{
    /// <summary>
    /// パイプアクションデフォルト。
    /// </summary>
    public class DefaultPipeAction : IPipeAction
    {

        public Action OnStart { get; set; }

        public Action OnEnd { get; set; }

        public Action OnError { get; set; }

        /// <summary>
        /// パイプアクションデフォルトのコンストラクター
        /// </summary>
        /// <param name="isResumePipeline"></param>
        /// <param name="action"></param>
        public DefaultPipeAction(bool isResumePipeline, Func<PipeRequest, PipeResponse> action)
        {
            this.IsResumePipe = isResumePipeline;
            this.Action = action;
        }

        /// <summary>
        /// 指定されたアクション（メソッド）をパイプライン処理として実行します。
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public PipeResponse Execute(PipeRequest request)
        {
            return Action(request);
        }

        public bool IsResumePipe { get; private set; }

        public Func<PipeRequest, PipeResponse> Action { get; private set; }

    }

    /// <summary>
    /// パイプアクションデフォルト拡張機能
    /// </summary>
    public static class DefaultPipeActionExtensions
    {
        public static Pipe UseDefaultAction(this Pipe pipe, bool isResumePipeline, Func<PipeRequest, PipeResponse> action)
        {
            Contract.NotNull(pipe, "pipe");

            DefaultPipeAction defaultAction = new DefaultPipeAction(isResumePipeline, action);
            pipe.Use(defaultAction);
            return pipe;
        }
        /// <summary>
        /// <see cref="PipeRequest"/>を受け取り、<see cref="PipeResponse"/>を返すメソッドを<see cref="Pipe"/>に登録します。
        /// </summary>
        /// <param name="source">登録先の<see cref="Pipe"/></param>
        /// <param name="func">登録するメソッド</param>
        /// <param name="resumeNext">メソッドが失敗を返したときに処理を継続するかどうか</param>
        /// <returns>登録先の<see cref="Pipe"/>のインスタンス</returns>
        public static Pipe Use(this Pipe source, Func<PipeRequest, PipeResponse> func, bool resumeNext = false)
        {
            return source.UseDefaultAction(resumeNext, func);
        }

        /// <summary>
        /// <see cref="PipeRequest"/>を受け取り、<see cref="PipeResponse"/>を返し引数を一つ受け取るメソッドを<see cref="Pipe"/>に登録します。
        /// </summary>
        /// <param name="source">登録先の<see cref="Pipe"/></param>
        /// <param name="func">登録するメソッド</param>
        /// <param name="value">引数1</param>
        /// <param name="resumeNext">メソッドが失敗を返したときに処理を継続するかどうか</param>
        /// <returns>登録先の<see cref="Pipe"/>のインスタンス</returns>
        public static Pipe Use<T1>(this Pipe source, Func<PipeRequest, T1, PipeResponse> func, T1 value, bool resumeNext = false)
        {
            return source.UseDefaultAction(resumeNext, req =>
            {
                return func(req, value);
            });
        }

        /// <summary>
        /// <see cref="PipeRequest"/>を受け取り、<see cref="PipeResponse"/>を返し引数を二つ受け取るメソッドを<see cref="Pipe"/>に登録します。
        /// </summary>
        /// <param name="source">登録先の<see cref="Pipe"/></param>
        /// <param name="func">登録するメソッド</param>
        /// <param name="value1">引数1</param>
        /// <param name="value2">引数2</param>
        /// <param name="resumeNext">メソッドが失敗を返したときに処理を継続するかどうか</param>
        /// <returns>登録先の<see cref="Pipe"/>のインスタンス</returns>
        public static Pipe Use<T1, T2>(this Pipe source, Func<PipeRequest, T1, T2, PipeResponse> func, T1 value1, T2 value2, bool resumeNext = false)
        {
            return source.UseDefaultAction(resumeNext, req =>
            {
                return func(req, value1, value2);
            });
        }

        /// <summary>
        /// <see cref="PipeRequest"/>を受け取り、<see cref="PipeResponse"/>を返し引数を三つ受け取るメソッドを<see cref="Pipe"/>に登録します。
        /// </summary>
        /// <param name="source">登録先の<see cref="Pipe"/></param>
        /// <param name="func">登録するメソッド</param>
        /// <param name="value1">引数1</param>
        /// <param name="value2">引数2</param>
        /// <param name="value3">引数3</param>
        /// <param name="resumeNext">メソッドが失敗を返したときに処理を継続するかどうか</param>
        /// <returns>登録先の<see cref="Pipe"/>のインスタンス</returns>
        public static Pipe Use<T1, T2, T3>(this Pipe source, Func<PipeRequest, T1, T2, T3, PipeResponse> func, T1 value1, T2 value2, T3 value3, bool resumeNext = false)
        {
            return source.UseDefaultAction(resumeNext, req =>
            {
                return func(req, value1, value2, value3);
            });
        }

        /// <summary>
        /// <see cref="PipeRequest"/>を受け取り、<see cref="PipeResponse"/>を返し引数を四つ受け取るメソッドを<see cref="Pipe"/>に登録します。
        /// </summary>
        /// <param name="source">登録先の<see cref="Pipe"/></param>
        /// <param name="func">登録するメソッド</param>
        /// <param name="value1">引数1</param>
        /// <param name="value2">引数2</param>
        /// <param name="value3">引数3</param>
        /// <param name="value4">引数4</param>
        /// <param name="resumeNext">メソッドが失敗を返したときに処理を継続するかどうか</param>
        /// <returns>登録先の<see cref="Pipe"/>のインスタンス</returns>
        public static Pipe Use<T1, T2, T3, T4>(this Pipe source, Func<PipeRequest, T1, T2, T3, T4, PipeResponse> func, T1 value1, T2 value2, T3 value3, T4 value4, bool resumeNext = false)
        {
            return source.UseDefaultAction(resumeNext, req =>
            {
                return func(req, value1, value2, value3, value4);
            });
        }

        public static Pipe Resume(this Pipe pipe, Func<PipeContext, Object, HttpStatusCode> func)
        {
            Contract.NotNull(pipe, "pipe");
            pipe.UseDefaultAction(false, (request) =>
            {
                var result = func(request.Context, request.Data);
                if (result != HttpStatusCode.OK)
                {
                    return request.CreateResponse(result);
                }
                return request.CreateResponse(HttpStatusCode.OK, request.Data);
            });
            return pipe;
        }
    }

}