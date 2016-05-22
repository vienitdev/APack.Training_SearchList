using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Archpack.Training.ArchUnits.Logging.V1;
using Archpack.Training.ArchUnits.Contracts.V1;
using System.Runtime.CompilerServices;
using System.Security.Principal;

namespace Archpack.Training.ArchUnits.Logging.Entities.V1
{
    /// <summary>
    /// ログ出力に関する <see cref="DbContext"/> の拡張メソッドを定義します。
    /// </summary>
    public static class DbContextExtensions
    {
        private static readonly ConditionalWeakTable<DbContext, Dictionary<string, object>> table =
           new ConditionalWeakTable<DbContext, Dictionary<string, object>>();

        private const string AuditInterceptorKey = "AuditInterceptor";
        private const string TraceInterceptorKey = "TraceInterceptor";

        /// <summary>
        /// プロパティを設定します
        /// </summary>
        private static void SetProperty<T>(DbContext self, string key, T value)
        {
            var dic = table.GetOrCreateValue(self);
            if (dic.ContainsKey(key))
            {
                dic[key] = value;
            }
            else
            {
                dic.Add(key, value);
            }
        }

        /// <summary>
        /// プロパティを取得します
        /// </summary>
        private static T GetProperty<T>(DbContext self, string key)
        {
            var dic = table.GetOrCreateValue(self);
            if (dic.ContainsKey(key))
            {
                return (T)dic[key];
            }
            return default(T);
        }


        /// <summary>
        /// 監査ログの出力を有効化します。
        /// </summary>
        /// <param name="self">監査ログ対象となるクエリを実行する <see cref="DbContext"/></param>
        /// <param name="identity">処理を実行しているユーザー</param>
        /// <returns>監査ログの出力を実行する <see cref="AuditLogInterceptor"/> </returns>
        public static AuditLogInterceptor EnableAuditLog(this DbContext self, LogContext logContext)
        {
            Contract.NotNull(self, "self");

            var interceptor = GetProperty<AuditLogInterceptor>(self, AuditInterceptorKey);
            if (interceptor == null)
            {
                interceptor = new AuditLogInterceptor(self, logContext);
                SetProperty(self, AuditInterceptorKey, interceptor);
            }
            return interceptor;
        }


        /// <summary>
        /// トレースログの出力を有効化します。
        /// </summary>
        /// <param name="self">トレースログ対象となるクエリを実行する <see cref="DbContext"/></param>
        /// <param name="identity">処理を実行しているユーザー</param>
        /// <returns>トレースログの出力を実行する <see cref="TraceLogInterceptor"/> </returns>
        public static TraceLogInterceptor EnableTraceLog(this DbContext self, LogContext logContext)
        {
            Contract.NotNull(self, "self");

            var interceptor = GetProperty<TraceLogInterceptor>(self, TraceInterceptorKey);
            if (interceptor == null)
            {
                interceptor = new TraceLogInterceptor(self, logContext);
                SetProperty(self, TraceInterceptorKey, interceptor);
            }
            return interceptor;
        }
    }
}