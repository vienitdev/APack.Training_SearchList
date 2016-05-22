using Archpack.Training.ArchUnits.Contracts.V1;
using Archpack.Training.ArchUnits.Environment.V1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Archpack.Training.ArchUnits.Routing.V1
{
    /// <summary>
    /// <see cref="ServiceUnitContext"/> に関する拡張メソッドを定義します。
    /// </summary>
    public static class ServiceUnitContextExtensions
    {
        /// <summary>
        /// サービスユニットのルートの物理フォルダ名を取得します。
        /// </summary>
        /// <param name="context"><see cref="ServiceUnitContext"/></param>
        /// <returns>物理フォルダ名</returns>
        public static string GetFisicalDirectory(this ServiceUnitContext context)
        {
            Contract.NotNull(context, "context");
            if (string.IsNullOrEmpty(context.ServiceUnitName))
            {
                throw new InvalidOperationException();
            }
            var env = context.ServiceContainer.GetService<IApplicationEnvironment>();
            return env.MapPath("~/ServiceUnits/" + context.ServiceUnitName);
        }
        /// <summary>
        /// サービスユニットのバージョンの物理フォルダ名を取得します。
        /// </summary>
        /// <param name="context"><see cref="ServiceUnitContext"/></param>
        /// <returns>物理フォルダ名</returns>
        public static string GetVersionFisicalDirectory(this ServiceUnitContext context)
        {
            Contract.NotNull(context, "context");
            if (string.IsNullOrEmpty(context.ServiceUnitName))
            {
                throw new InvalidOperationException();
            }
            if (string.IsNullOrEmpty(context.Version))
            {
                throw new InvalidOperationException();
            }
            var env = context.ServiceContainer.GetService<IApplicationEnvironment>();
            return env.MapPath("~/ServiceUnits/" + context.ServiceUnitName + "/" + context.Version);
        }
        /// <summary>
        /// サービスユニットのロールの物理フォルダ名を取得します。
        /// </summary>
        /// <param name="context"><see cref="ServiceUnitContext"/></param>
        /// <returns>物理フォルダ名</returns>
        public static string GetRoleFisicalDirectory(this ServiceUnitContext context)
        {
            if (string.IsNullOrEmpty(context.ServiceUnitName))
            {
                throw new InvalidOperationException();
            }
            if (string.IsNullOrEmpty(context.Version))
            {
                throw new InvalidOperationException();
            }
            if (string.IsNullOrEmpty(context.Role))
            {
                throw new InvalidOperationException();
            }
            Contract.NotNull(context, "context");
            var env = context.ServiceContainer.GetService<IApplicationEnvironment>();
            return env.MapPath("~/ServiceUnits/" + context.ServiceUnitName + "/" + context.Version + "/" + context.Role);
        }
    }
}