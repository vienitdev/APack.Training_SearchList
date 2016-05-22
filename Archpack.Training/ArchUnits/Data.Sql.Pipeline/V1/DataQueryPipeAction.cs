using Archpack.Training.ArchUnits.Contracts.V1;
using Archpack.Training.ArchUnits.Data.Sql.V1;
using Archpack.Training.ArchUnits.Pipeline.V1;
using Archpack.Training.ArchUnits.Routing.V1;
using Archpack.Training.ArchUnits.Routing.Pipeline.V1;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using Archpack.Training.ArchUnits.Logging.V1;

namespace Archpack.Training.ArchUnits.Data.Sql.Pipeline.V1
{
    /// <summary>
    /// 名前付けクエリの作成・実行を行うアクションクラスを生成する拡張メソッドです。
    /// </summary>
    public static class DataQueryPipeActionExtensions
    {
        /// <summary>
        /// DataQuery の初期化処理をパイプラインに追加します。
        /// </summary>
        /// <param name="pipe"></param>
        /// <param name="queryName"></param>
        /// <param name="context"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        public static Pipe DataQuery(this Pipe pipe, string queryName, DbContext context, ISqlDefinitionFactory factory)
        {
            Contract.NotNull(pipe, "pipe");
            Contract.NotEmpty(queryName, "queryName");
            Contract.NotNull(context, "context");
            Contract.NotNull(factory, "factory");

            pipe.UseDefaultAction(false, (request) =>
            {
                DataQuery query = new DataQuery(context, factory);
                query.AppendNamedQuery(queryName);
                return request.CreateResponse(HttpStatusCode.OK, query);
            });
            return pipe;
        }

        /// <summary>
        /// DataQuery の初期化処理をパイプラインに追加します。
        /// </summary>
        /// <param name="pipe"></param>
        /// <param name="queryName"></param>
        /// <param name="context"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        public static Pipe DataQuery(this Pipe pipe, string queryName)
        {
            Contract.NotNull(pipe, "pipe");
            Contract.NotEmpty(queryName, "queryName");

            pipe.UseDefaultAction(false, (request) =>
            {

                var parameter = request.Context.GetDataQueryParameters(queryName);

                DbContext context = parameter.DbContext;
                ISqlDefinitionFactory factory = new FileSqlDefinitionFactory(request.Context.GetVersionFisicalDirectory());
                LogContext logContext = request.Context.GetLogContext();

                DataQuery query = new DataQuery(context, factory, logContext, queryName);
                query.AppendNamedQuery(queryName);

                return request.CreateResponse(HttpStatusCode.OK, query);

            });

            return pipe;
        }

        public static Pipe DataQuery(this Pipe pipe, DbContext context, ISqlDefinitionFactory factory)
        {
            Contract.NotNull(pipe, "pipe");
            Contract.NotNull(context, "context");
            Contract.NotNull(factory, "factory");

            pipe.UseDefaultAction(false, (request) =>
            {
                LogContext logContext = request.Context.GetLogContext();
                DataQuery query = new DataQuery(context, factory, logContext);
                return request.CreateResponse(HttpStatusCode.OK, query);
            });
            return pipe;
        }

        /// <summary>
        /// 部分的な SQL の読み込み処理をパイプラインに追加します。
        /// </summary>
        /// <param name="pipe"></param>
        /// <param name="condition"></param>
        /// <param name="queryName"></param>
        /// <returns></returns>
        public static Pipe AppendQueryWithCondition(this Pipe pipe, bool condition, string queryName)
        {
            Contract.NotNull(pipe, "pipe");
            Contract.NotEmpty(queryName, "queryName");

            pipe.UseDefaultAction(false, (request) =>
            {
                DataQuery query = request.Data as DataQuery;
                if (query == null)
                {
                    throw new InvalidOperationException("DataQuery を指定して下さい");
                }

                if (condition)
                {
                    query.AppendQuery(queryName);
                }

                return request.CreateResponse(HttpStatusCode.OK, query);
            });

            return pipe;
        }

        /// <summary>
        /// 部分的な SQL の読み込み処理をパイプラインに追加します。
        /// </summary>
        /// <param name="pipe"></param>
        /// <param name="queryName"></param>
        /// <returns></returns>
        public static Pipe AppendQuery(this Pipe pipe, string queryName)
        {
            return pipe.AppendQueryWithCondition(true, queryName);
        }

        /// <summary>
        /// 部分的な SQL の読み込み処理をパイプラインに追加します。
        /// </summary>
        /// <param name="pipe"></param>
        /// <param name="condition"></param>
        /// <param name="queryName"></param>
        /// <param name="replaceValues"></param>
        /// <returns></returns>
        public static Pipe AppendNamedQueryWithCondition(this Pipe pipe, bool condition, string queryName, params object[] replaceValues)
        {
            Contract.NotNull(pipe, "pipe");
            Contract.NotEmpty(queryName, "queryName");

            pipe.UseDefaultAction(false, (request) =>
            {
                DataQuery query = request.Data as DataQuery;
                if (query == null)
                {
                    throw new InvalidOperationException("DataQuery を指定して下さい");
                }

                if (condition)
                {
                    query.AppendNamedQuery(queryName, replaceValues);
                }

                return request.CreateResponse(HttpStatusCode.OK, query);
            });

            return pipe;
        }

        /// <summary>
        /// 部分的な SQL の読み込み処理をパイプラインに追加します。
        /// </summary>
        /// <param name="pipe"></param>
        /// <param name="queryName"></param>
        /// <param name="replaceValues"></param>
        /// <returns></returns>
        public static Pipe AppendNamedQuery(this Pipe pipe, string queryName, params object[] replaceValues)
        {
            return pipe.AppendNamedQueryWithCondition(true, queryName, replaceValues);
        }

        /// <summary>
        /// パラメータの設定処理をパイプラインに追加します。
        /// </summary>
        /// <param name="pipe"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Pipe SetParameter(this Pipe pipe, string name, object value)
        {
            return pipe.SetParameterWithCondition(true, name, value);
        }

        /// <summary>
        /// パラメータの設定処理をパイプラインに追加します。
        /// </summary>
        /// <param name="pipe"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Pipe SetParameterWithCondition(this Pipe pipe, bool condition, string name, object value)
        {
            Contract.NotNull(pipe, "pipe");
            pipe.UseDefaultAction(false, (request) =>
            {
                DataQuery query = request.Data as DataQuery;
                if (query == null)
                {
                    throw new InvalidOperationException("DataQuery を指定して下さい");
                }

                if (condition)
                {
                    query.SetParameter(name, value);
                }

                return request.CreateResponse(HttpStatusCode.OK, query);
            });
            return pipe;
        }

        /// <summary>
        /// パラメータの設定処理をパイプラインに追加します。
        /// </summary>
        /// <param name="pipe"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Pipe SetParameters(this Pipe pipe, Action<PipeContext> action)
        {
            return pipe.SetParametersWithCondition(true, action);
        }

        /// <summary>
        /// パラメータの設定処理をパイプラインに追加します。
        /// </summary>
        /// <param name="pipe"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Pipe SetParametersWithCondition(this Pipe pipe, bool condition, Action<PipeContext>  action)
        {
            Contract.NotNull(pipe, "pipe");
            pipe.UseDefaultAction(false, (request) =>
            {

                if (condition)
                {
                    action(request.Context);
                }

                return request.CreateResponse(HttpStatusCode.OK, request.Data);
            });
            return pipe;
        }

        /// <summary>
        /// DataQuery の実行結果を取得し変換を行います。
        /// </summary>
        /// <param name="pipe"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static Pipe SetResults<T>(this Pipe pipe, Action<T> action) where T : class
        {
            Contract.NotNull(pipe, "pipe");
            pipe.UseDefaultAction(false, (request) =>
            {
                if (!request.Context.Items.ContainsKey(typeof(T).Name))
                {
                    throw new InvalidOperationException("DataQuery を指定して下さい");
                }

                var results = request.Context.Items[typeof(T).Name] as T;


                action(results);

                return request.CreateResponse(HttpStatusCode.OK, results);
            });
            return pipe;
        }


        /// <summary>
        /// SQL の実行処理をパイプラインに追加します。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pipe"></param>
        /// <returns></returns>
        public static Pipe List<T>(this Pipe pipe)
        {
            Contract.NotNull(pipe, "pipe");
            pipe.UseDefaultAction(false, (request) =>
            {
                DataQuery query = request.Data as DataQuery;

                if (query == null)
                {
                    throw new InvalidOperationException("DataQuery を指定して下さい");
                }

                // TODO: get parameter from context.
                var parameters = request.Context.GetDataQueryParameters(query.Name);

                if (parameters != null)
                {
                    foreach (var p in parameters.Parameters)
                    {
                        query.SetParameter(p.Key, p.Value);
                    }
                    
                    // TODO: set parameter from context request context.
                    var data = request.Context.RequestData;

                    if (data != null)
                    {
                        var paramNames = query.SqlDefinition.Parameters.Select((p) => p.Key);
                        var dataType = data.GetType();
                        var props = dataType.GetProperties()
                                            .Where((prop) => paramNames.Contains(prop.Name));

                        foreach (var prop in props)
                        {
                            query.SetParameter(prop.Name, prop.GetValue(data));
                        }
                    }

                }

                List<T> result = query.GetList<T>();

                // TODO: set data to context response data.
                if (!string.IsNullOrEmpty(query.Name))
                {
                    request.Context.ResponseData[query.Name] = result;
                }

                return request.CreateResponse(HttpStatusCode.OK, result);
            });

            return pipe;
        }

        /// <summary>
        /// SQL の実行処理をパイプラインに追加します。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pipe"></param>
        /// <returns></returns>
        public static Pipe SingleOrDefault<T>(this Pipe pipe)
        {
            Contract.NotNull(pipe, "pipe");
            pipe.UseDefaultAction(false, (request) =>
            {
                DataQuery query = request.Data as DataQuery;
                if (query == null)
                {
                    throw new InvalidOperationException("DataQuery を指定して下さい");
                }

                // TODO: get parameter from context.
                var parameters = request.Context.GetDataQueryParameters(query.Name);

                if (parameters != null)
                {
                    foreach (var p in parameters.Parameters)
                    {
                        query.SetParameter(p.Key, p.Value);
                    }
                    
                    // TODO: set parameter from context request context.
                    var data = request.Context.RequestData;

                    if (data != null)
                    {
                        var paramNames = query.SqlDefinition.Parameters.Select((p) => p.Key);
                        var dataType = data.GetType();
                        var props = dataType.GetProperties()
                                            .Where((prop) => paramNames.Contains(prop.Name));

                        foreach (var prop in props)
                        {
                            query.SetParameter(prop.Name, prop.GetValue(data));
                        }
                    }

                }

                T result = query.SingleOrDefault<T>();

                // TODO: set data to context response data.
                if (!string.IsNullOrEmpty(query.Name))
                {
                    request.Context.ResponseData[query.Name] = result;
                }

                return request.CreateResponse(HttpStatusCode.OK, result);
            });

            return pipe;
        }
    }

}