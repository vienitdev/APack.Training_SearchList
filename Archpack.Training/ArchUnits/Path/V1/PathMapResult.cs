using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Archpack.Training.ArchUnits.Path.V1
{
    /// <summary>
    /// パスのマッピング結果を表します。
    /// </summary>
    public class PathMapResult
    {
        /// <summary>
        /// インスタンスを初期化します。
        /// </summary>
        public PathMapResult()
        {
            Parameters = new Dictionary<string,string>();
        }
        /// <summary>
        /// マップ前の元となるパスを取得または設定します。
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// マッピングするパスのパラメーターを取得または設定します。
        /// </summary>
        public IDictionary<string, string> Parameters { get; private set; }
        /// <summary>
        /// マップ後のパスを取得または設定します。
        /// </summary>
        public string MappedPath { get; set; }
        /// <summary>
        /// 処理が成功したかどうかを取得または設定します。
        /// </summary>
        public bool Success { get; set; }
    }
}