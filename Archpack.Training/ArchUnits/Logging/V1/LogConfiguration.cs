using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;

namespace Archpack.Training.ArchUnits.Logging.V1
{
    /// <summary>
    /// ログの構成情報を保持します。
    /// </summary>
    public class LogConfiguration
    {
        /// <summary>
        /// JSONの構成情報から<see cref="LogAdapterSetting"/>をインスタンス化します。
        /// </summary>
        /// <param name="source">JSONの構成情報</param>
        /// <returns><see cref="LogAdapterSetting"/>のインスタンス</returns>
        public static IEnumerable<LogAdapterSetting> CreateLogAdapterSetting(IDictionary<string, JToken> source)
        {
            var config = new LogConfiguration(source);
            var results = new List<LogAdapterSetting>();

            foreach (var pattern in config.LogSettings)
            {
                var assemblytype = Assembly.GetExecutingAssembly().GetTypes();

                foreach (var type in assemblytype
                    .Where(t => t.GetInterface("ILogAdapter") == typeof(ILogAdapter)
                        && t.GetCustomAttributes<LogAdapterTypesAttribute>().Any(x => x.Types.Contains(pattern.Resource.Type))))
                {
                    var adapter = Activator.CreateInstance(type, config) as ILogAdapter;
                    results.Add(new LogAdapterSetting() { 
                        Url = pattern.Uri, 
                        Adapter = adapter,
                        LogLevel = pattern.Resource.LogLevel,
                        Name = pattern.Resource.Name
                    });
                }
            }

            return results;
        }
        /// <summary>
        /// JSONの構成情報を利用してインスタンスを初期化します。
        /// </summary>
        /// <param name="source"></param>
        public LogConfiguration(IDictionary<string, JToken> source)
        {
            if (source.ContainsKey("logSettings"))
            {
                LogSettings = source["logSettings"].ToObject<List<LogSetting>>();
            }
            else
            {
                LogSettings = new List<LogSetting>();
            }
        }
        /// <summary>
        /// ログ設定の一覧を取得または設定します。
        /// </summary>
        [JsonProperty("logSettings")]
        public List<LogSetting> LogSettings { get; set; }
    }
    /// <summary>
    /// ログ設定を保持します。
    /// </summary>
    public class LogSetting {
        /// <summary>
        /// URIを取得または設定します。
        /// </summary>
        [JsonProperty("uri")]
        public string Uri { get; set; }
        /// <summary>
        /// 詳細情報を取得または設定します。
        /// </summary>
        [JsonProperty("resource")]
        public Resource Resource { get; set; }
    }
    /// <summary>
    /// ログの詳細設定を保持します。
    /// </summary>
    public class Resource
    {
        /// <summary>
        /// ログの名前を取得または設定します。
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }
        /// <summary>
        /// 出力レベルを取得または設定します。
        /// </summary>
        [JsonProperty("level")]
        public string LogLevel { get; set; }
        /// <summary>
        /// 出力タイプを取得または設定します。
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }
        /// <summary>
        /// 出力ファイル名を取得または設定します。
        /// </summary>
        [JsonProperty("filename")]
        public string FileName { get; set; }
        /// <summary>
        /// Database出力時の接続文字列を取得または設定します。
        /// </summary>
        [JsonProperty("connection")]
        public string Connection { get; set; }
        /// <summary>
        /// ログのフォーマットを取得または設定します。
        /// </summary>
        [JsonProperty("format")]
        public string Format { get; set; }
    }
    /// <summary>
    /// ログのリソースタイプを定義します。
    /// </summary>
    public class LogResourceTypes
    {
        /// <summary>
        /// データベース出力のリソースタイプ
        /// </summary>
        public const string Database = "db";
        /// <summary>
        /// ファイル出力のリソースタイプ
        /// </summary>
        public const string File = "file";
    }
}