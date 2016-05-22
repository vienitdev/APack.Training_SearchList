using Archpack.Training.ArchUnits.Pipeline.V1;
using Archpack.Training.ArchUnits.Validations.V1;
using Archpack.Training.ArchUnits.WebApiModels.V1;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;

namespace Archpack.Training.ArchUnits.Data.Entity.V1
{
    /// <summary>
    /// DbContext と変更セット <see cref="ChangeSet"/> の操作に関連するパイプラインの拡張メソッドを提供します。
    /// </summary>
    public static class PipeExtensions
    {
        /// <summary>
        /// 変更セットに格納されたエンティティに対する既定値、固定値を設定するアクションをパイプラインに追加します。
        /// </summary>
        /// <typeparam name="T">変更セットに格納されているエンティティの型</typeparam>
        /// <param name="source">パイプライン</param>
        /// <param name="prepare">変更セットに格納されたエンティティに対する操作</param>
        /// <returns>パイプライン</returns>
        public static Pipe PrepareChangeSetItem<T>(this Pipe source, Func<ChangeSetItem<T>, ChangeSetItem<T>> prepare) where T : class
        {
            return source.UseDefaultAction(false, (request) =>
            {
                var changeSet = request.Data as ChangeSet<T>;
                changeSet.Prepare(prepare);
                return request.CreateResponse(HttpStatusCode.OK, changeSet);
            });
        }

        /// <summary>
        /// 変更セットに格納されたエンティティに対する入力検証を行うアクションをパイプラインに追加します。
        /// </summary>
        /// <typeparam name="T">変更セットに格納されているエンティティの型<</typeparam>
        /// <param name="source">パイプライン</param>
        /// <param name="createRuleAction">入力検証を追加するアクションメソッド</param>
        /// <returns>パイプライン</returns>
        public static Pipe ValidateChangeSet<T>(this Pipe source, Func<ValidationRules<T>> createRuleAction) where T : class
        {
            return source.UseDefaultAction(false, (request) =>
            {
                var changeSet = request.Data as ChangeSet<T>;
                var rules = createRuleAction();

                var result = rules.Validate(changeSet.GetValues());
                if (!result.IsValid)
                {
                    return request.CreateResponse(HttpStatusCode.BadRequest, result);
                }
                return request.CreateResponse(HttpStatusCode.OK, changeSet);
            });
        }

        /// <summary>
        /// 変更セットのデータ保存処理をパイプラインに追加します。
        /// </summary>
        /// <typeparam name="T">変更セットに格納されているエンティティの型<</typeparam>
        /// <param name="source">パイプライン</param>
        /// <param name="context">変更セットの保存を行う DbContext のインスタンス</param>
        /// <returns>パイプライン</returns>
        public static Pipe SaveChangesChangeSet<T>(this Pipe source, DbContext context) where T : class
        {
            return source.UseDefaultAction(false, (request) =>
            {
                var changeSet = request.Data as ChangeSet<T>;
                var response = new List<T>();
                response.AddRange(changeSet.AttachTo(context));
                context.SaveChanges();
                return request.CreateResponse(HttpStatusCode.OK, response);
            });
        }

        /// <summary>
        /// 変更セットを DbContext に適用する処理をパイプラインに追加します。
        /// </summary>
        /// <typeparam name="T">変更セットに格納されているエンティティの型</typeparam>
        /// <param name="source">パイプライン</param>
        /// <param name="context">変更セットの保存を行う DbContext のインスタンス</param>
        /// <returns>パイプライン</returns>
        public static Pipe AttachToDbContext<T>(this Pipe source, DbContext context) where T : class
        {
            return source.UseDefaultAction(false, (request) =>
            {
                var changeSet = request.Data as ChangeSet<T>;

                var response = new List<T>();
                response.AddRange(changeSet.AttachTo(context));

                return request.CreateResponse(HttpStatusCode.OK, response);
            });
        }

        /// <summary>
        /// データ保存処理をパイプラインに追加します。
        /// </summary>
        /// <param name="source">パイプライン</param>
        /// <param name="context">変更セットの保存を行う DbContext のインスタンス</param>
        /// <returns>パイプライン</returns>
        public static Pipe SaveChange(this Pipe source, DbContext context)
        {
            return source.UseDefaultAction(false, (request) =>
            {
                var response = request.Data;
                context.SaveChanges();

                return request.CreateResponse(HttpStatusCode.OK, response);
            });
        }
    }
}