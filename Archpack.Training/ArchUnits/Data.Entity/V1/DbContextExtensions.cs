using System.Data.Entity;

namespace Archpack.Training.ArchUnits.Data.Entity.V1
{
    /// <summary>
    /// <see cref="DbContext"/> の拡張メソッドを定義します。
    /// </summary>
    public static class DbContextExtensions
    {
        /// <summary>
        /// 変更の追跡を行わないように <see cref="DbContext"/> を設定します。
        /// </summary>
        /// <typeparam name="T">設定する <see cref="DbContext"/> の型</typeparam>
        /// <param name="context">設定する <see cref="DbContext"/>　のインスタンス</param>
        /// <returns>引数で指定された <see cref="DbContext"/> インスタンス</returns>
        public static T ToReadOnly<T>(this T context) where T : DbContext, new()
        {
            context.Configuration.ProxyCreationEnabled = false;
            context.Configuration.AutoDetectChangesEnabled = false;

            return context;
        }
    }
}