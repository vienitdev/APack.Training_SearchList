using Archpack.Training.ArchUnits.Routing.V1;
using Archpack.Training.ArchUnits.Validations.V1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace Archpack.Training.ArchUnits.Pipeline.V1
{
    /// <summary>
    /// サーバーサイドバリデーションのアクションクラスです。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ValidationPipeAction<T> : IPipeAction where T : class
    {
        ValidationRules<T> rules;

        public Action OnStart { get; set; }

        public Action OnEnd { get; set; }

        public Action OnError { get; set; }

        /// <summary>
        /// サーバーサイドバリデーションのアクションクラスのコンストラクター。
        /// </summary>
        public ValidationPipeAction()
        {
            rules = new ValidationRules<T>();
            rules.Add((target) => target.ValidateDataAnnotations());
        }

        /// <summary>
        /// 検証ルールを追加する。
        /// </summary>
        /// <param name="rule"></param>
        public void AddValidationRule(Func<T, ValidationResult> rule)
        {
            rules.Add(rule);
        }

        /// <summary>
        /// 指定されたアクション（メソッド）をパイプライン処理として実行します。
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public PipeResponse Execute(PipeRequest request)
        {
            PipeResponse response = request.CreateResponse(HttpStatusCode.OK);
            response.Data = request.Data;

            var result = rules.Validate(request.Data as T);

            if (!result.IsValid)
            {
                response = request.CreateResponse(HttpStatusCode.BadRequest);
                response.Data = result;
            }

            return response;
        }

        public bool IsResumePipe
        {
            get { return false; }
        }

    }

    /// <summary>
    /// 検証パイプアクション拡張機能
    /// </summary>
    public static class ValidationPipeActionExtensions
    {
        public static Pipe ValidateRequest(this Pipe pipeline)
        {
            pipeline.Use<ValidationPipeAction<object>>();
            return pipeline;
        }

        public static Pipe ValidateParameters<T>(this Pipe pipeline) where T : class
        {
            pipeline.Use<ValidationPipeAction<T>>();
            return pipeline;
        }

        public static Pipe ValidateSingleValues<T1>(this Pipe source, Func<T1, ValidationResult> validator,
            T1 value1)
        {
            return source.UseDefaultAction(false, (req) =>
            {
                var result = validator(value1);
                if (result.IsValid)
                {
                    return req.CreateResponse(HttpStatusCode.OK);
                }
                return req.CreateResponse(HttpStatusCode.BadRequest, result);
            });
        }

        public static Pipe ValidateSingleValues<T1, T2>(this Pipe source, Func<T1, T2, ValidationResult> validator,
            T1 value1, T2 value2)
        {
            return source.UseDefaultAction(false, (req) =>
            {
                var result = validator(value1, value2);
                if (result.IsValid)
                {
                    return req.CreateResponse(HttpStatusCode.OK);
                }
                return req.CreateResponse(HttpStatusCode.BadRequest, result);
            });
        }

        public static Pipe ValidateSingleValues<T1, T2, T3>(this Pipe source, Func<T1, T2, T3, ValidationResult> validator,
            T1 value1, T2 value2, T3 value3)
        {
            return source.UseDefaultAction(false, (req) =>
            {
                var result = validator(value1, value2, value3);
                if (result.IsValid)
                {
                    return req.CreateResponse(HttpStatusCode.OK);
                }
                return req.CreateResponse(HttpStatusCode.BadRequest, result);
            });
        }

        public static Pipe ValidateSingleValues<T1, T2, T3, T4>(this Pipe source, Func<T1, T2, T3, T4, ValidationResult> validator,
            T1 value1, T2 value2, T3 value3, T4 value4)
        {
            return source.UseDefaultAction(false, (req) =>
            {
                var result = validator(value1, value2, value3, value4);
                if (result.IsValid)
                {
                    return req.CreateResponse(HttpStatusCode.OK);
                }
                return req.CreateResponse(HttpStatusCode.BadRequest, result);
            });
        }
    }
}