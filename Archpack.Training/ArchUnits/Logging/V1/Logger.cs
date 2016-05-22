using System.Collections.Generic;
using System.Linq;

namespace Archpack.Training.ArchUnits.Logging.V1
{
    /// <summary>
    /// 保持している<see cref="ILogAdapter"/>にログを出力するための機能を提供します。
    /// </summary>
    public class Logger
    {
        private IEnumerable<LogAdapterSetting> settings;
        private static readonly Dictionary<string, int> LogLevels = new Dictionary<string, int>()
        {
            {"trace", 1}, {"debug", 2}, {"info", 3}, {"warn", 4}, {"error", 5}, {"fatal", 6}
        };
        private const int DefaultLogLevel = 2;

        /// <summary>
        /// 指定されたURLおよび<see cref="LogAdapterSetting"/>を利用してインスタンスを初期化します。
        /// </summary>
        /// <param name="url">URL</param>
        /// <param name="settings">ログ出力情報</param>
        public Logger(string url, IEnumerable<LogAdapterSetting> settings)
        {
            this.settings = GetSettingsByUri(url, settings ?? Enumerable.Empty<LogAdapterSetting>());
        }

        private IEnumerable<LogAdapterSetting> GetSettingsByUri(string uri, IEnumerable<LogAdapterSetting> settings) 
        {
            return settings.Where(s => uri.Contains(s.Url));
        }

        private IEnumerable<ILogAdapter> GetTargetAdapters(LogData data, int logLevel)
        {
            if(data == null || string.IsNullOrWhiteSpace(data.LogName)){
                return Enumerable.Empty<ILogAdapter>();
            }
            return settings.Where(s => s.Name == data.LogName && GetLogLevelNum(s.LogLevel) <= logLevel).Select(s => s.Adapter);
        }

        private int GetLogLevelNum(string logLevel)
        {
            if (string.IsNullOrWhiteSpace(logLevel))
            {
                return DefaultLogLevel;
            }
            var level = logLevel.ToLowerInvariant();
            if (LogLevels.ContainsKey(level))
            {
                return LogLevels[level];
            }
            return DefaultLogLevel;
        }

        /// <summary>
        /// Info レベルのログを出力します。
        /// </summary>
        /// <param name="data">出力するログ内容</param>
        public void Info(LogData data)
        {
            foreach (var adapter in GetTargetAdapters(data, LogLevels["info"]))
	        {
                adapter.Info(data);
	        }
        }

        /// <summary>
        /// Debug レベルのログを出力します。
        /// </summary>
        /// <param name="data">出力するログ内容</param>
        public void Debug(LogData data)
        {
            foreach (var adapter in GetTargetAdapters(data, LogLevels["debug"]))
	        {
                adapter.Debug(data);
	        }
        }

        /// <summary>
        /// Trace レベルのログを出力します。
        /// </summary>
        /// <param name="data">出力するログ内容</param>
        public void Trace(LogData data)
        {
            foreach (var adapter in GetTargetAdapters(data, LogLevels["trace"]))
	        {
                adapter.Trace(data);
	        }
        }

        /// <summary>
        /// Warn レベルのログを出力します。
        /// </summary>
        /// <param name="data">出力するログ内容</param>
        public void Warn(LogData data)
        {
            foreach (var adapter in GetTargetAdapters(data, LogLevels["warn"]))
	        {
                adapter.Warn(data);
	        }
        }

        /// <summary>
        /// Error レベルのログを出力します。
        /// </summary>
        /// <param name="data">出力するログ内容</param>
        public void Error(LogData data)
        {
            foreach (var adapter in GetTargetAdapters(data, LogLevels["error"]))
	        {
                adapter.Error(data);
	        }
        }

        /// <summary>
        /// Fatal レベルのログを出力します。
        /// </summary>
        /// <param name="data">出力するログ内容</param>
        public void Fatal(LogData data)
        {
            foreach (var adapter in GetTargetAdapters(data, LogLevels["fatal"]))
	        {
                adapter.Fatal(data);
	        }
        }
    }
}