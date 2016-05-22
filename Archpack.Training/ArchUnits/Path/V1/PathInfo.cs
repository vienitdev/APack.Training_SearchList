using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Archpack.Training.ArchUnits.Path.V1
{
    /// <summary>
    /// サービスユニットに関するパス情報
    /// </summary>
    public class PathInfo
    {
        /// <summary>
        /// インスタンスを初期化します。
        /// </summary>
        public PathInfo()
        {

        }
        /// <summary>
        /// サービスユニット名を取得または設定します。
        /// </summary>
        public string ServiceUnitName { get; set; }
        /// <summary>
        /// バージョンを取得または設定します。
        /// </summary>
        public string Version { get; set; }
        /// <summary>
        /// ロールを取得または設定します。
        /// </summary>
        public string Role { get; set; }
        /// <summary>
        /// フルパスを取得または設定します。
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// 処理種別を取得または設定します。
        /// </summary>
        public string ProcessType { get; set; }
        /// <summary>
        /// 処理種別以降のパスを取得または設定します。
        /// </summary>
        public string ProcessPath { get; set; }
        /// <summary>
        /// 特殊処理以降のパスを取得または設定します。
        /// </summary>
        public string SpecificProcessPath { get; set; }
        /// <summary>
        /// URLのクエリの値を取得または設定します。
        /// </summary>
        public IDictionary<string, string> Query { get; set; }
    }
}