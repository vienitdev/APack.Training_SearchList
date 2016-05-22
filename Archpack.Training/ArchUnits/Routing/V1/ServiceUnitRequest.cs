using Archpack.Training.ArchUnits.Contracts.V1;
using Archpack.Training.ArchUnits.Path.V1;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;

namespace Archpack.Training.ArchUnits.Routing.V1
{
    /// <summary>
    /// サービスユニットの実行に関するリクエスト情報を保持します。
    /// </summary>
    public class ServiceUnitRequest
    {
        /// <summary>
        /// 指定された <see cref="ServiceUnitContext"/> および <see cref="PathInfo"/> を利用してインスタンスを初期化します。
        /// </summary>
        /// <param name="context">実行コンテキスト</param>
        /// <param name="info">パス情報</param>
        public ServiceUnitRequest(ServiceUnitContext context, PathInfo info)
        {
            Contract.NotNull(context, "context");
            Contract.NotNull(info, "info");

            this.Path = info.Path;
            this.ProcessPath = info.ProcessPath;
            this.ProcessType = info.ProcessType;
            this.SpecificProcessPath = info.SpecificProcessPath;
            this.Context = context;
            this.Query = new ReadOnlyDictionary<string, string>(info.Query);
        }
        /// <summary>
        /// フルパスを取得します。
        /// </summary>
        public string Path { get; private set; }
        /// <summary>
        /// 処理種別以降のパスを取得します。
        /// </summary>
        public string ProcessPath { get; private set; }
        /// <summary>
        /// 処理種別を取得します。
        /// </summary>
        public string ProcessType { get; private set; }
        /// <summary>
        /// 特殊処理以降のパスを取得します。
        /// </summary>
        public string SpecificProcessPath { get; private set; }
        /// <summary>
        /// リクエストデータを取得または設定します。
        /// </summary>
        public object Data { get; set; }
        /// <summary>
        /// URLのクエリの値を取得します。
        /// </summary>
        public IDictionary<string, string> Query { get; private set; }
        /// <summary>
        /// 所属する実行コンテキストである <see cref="ServiceUnitContext"/>を取得または設定します。
        /// </summary>
        public ServiceUnitContext Context { get; private set; }
    }
}