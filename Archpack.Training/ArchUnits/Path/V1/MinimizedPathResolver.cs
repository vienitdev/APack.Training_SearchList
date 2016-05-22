using Archpack.Training.ArchUnits.Container.V1;
using Archpack.Training.ArchUnits.Environment.V1;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using NPath = System.IO.Path;

namespace Archpack.Training.ArchUnits.Path.V1
{
    /// <summary>
    /// 縮小化されたファイルのパスを返す機能を提供します。
    /// </summary>
    public sealed class MinimizedPathResolver: ISuitablePathResolver
    {
        /// <summary>
        /// インスタンスを初期化します。
        /// </summary>
        public MinimizedPathResolver()
        {
        }

        /// <summary>
        /// 指定されたルート相対パスをもとに縮小化されたファイルがある場合は、そのパスを返し、ない場合は指定されたルート相対パスを返します。
        /// </summary>
        /// <param name="rootRelativePath">ルート相対パス</param>
        /// <returns>縮小化されたファイルのパス</returns>
        public string Resolve(string rootRelativePath)
        {
            var env = GlobalContainer.GetService<IApplicationEnvironment>();
            var fileName = env.MapPath(rootRelativePath);
            var withOut = fileName.Remove(fileName.Length - NPath.GetExtension(fileName).Length);
            if (!withOut.EndsWith(".min", StringComparison.InvariantCultureIgnoreCase))
            {
                if (File.Exists(withOut + ".min" + NPath.GetExtension(fileName)))
                {
                    fileName = withOut + ".min" + NPath.GetExtension(fileName);
                }
            }
            var appRoot = env.ApplicationRoot;
            return (appRoot.EndsWith("/") ? appRoot : (appRoot + "/"))
                + fileName.Replace(env.RootDir, string.Empty).Replace('\\', '/');
        }
    }
}