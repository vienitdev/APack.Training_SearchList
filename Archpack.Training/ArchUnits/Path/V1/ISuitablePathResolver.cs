using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Archpack.Training.ArchUnits.Path.V1
{
    /// <summary>
    /// 最適化されたファイルのパスを解決するためのインターフェイスを提供します。
    /// </summary>
    public interface ISuitablePathResolver
    {
        /// <summary>
        /// 指定されたルート相対パスをもとに最適化されたファイルのパスを返します。
        /// </summary>
        /// <param name="rootRelativePath">ルート相対パス</param>
        /// <returns>最適化されたファイルのパス</returns>
        string Resolve(string rootRelativePath);
    }
}