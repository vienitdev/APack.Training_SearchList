using Archpack.Training.ArchUnits.Logging.V1;
using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archpack.Training.ArchUnits.Logging.NLog.V1
{
    /// <summary>
    /// NLogを利用してログを出力する機能を提供します。
    /// </summary>
    [LogAdapterTypes(LogResourceTypes.Database, LogResourceTypes.File)]
    public class NLogAdapter : ILogAdapter
    {
        private LogConfiguration config;
        /// <summary>
        /// 指定された構成情報を利用してインスタンスを初期化します。
        /// </summary>
        /// <param name="config">構成情報</param>
        public NLogAdapter(LogConfiguration config)
        {
            this.config = config;
            var nlogConfig = new LoggingConfiguration();

            foreach (var pattern in config.LogSettings)
            {
                if (pattern.Resource.Type == "file")
                {
                    var target = new FileTarget();
                    target.Name = pattern.Resource.Name;
                    target.Layout = pattern.Resource.Format;
                    target.FileName = pattern.Resource.FileName;
                    target.ArchiveEvery = FileArchivePeriod.Day;
                    target.Encoding = System.Text.Encoding.UTF8;
                    nlogConfig.AddTarget(target.Name, target);
                    nlogConfig.LoggingRules.Add(new LoggingRule(pattern.Resource.Name, GetLogLevel(pattern), target));
                }

                if (pattern.Resource.Type == "db")
                {
                    var target = new DatabaseTarget();
                    target.Name = pattern.Resource.Name;
                    target.ConnectionString = pattern.Resource.Connection;
                    nlogConfig.AddTarget(target.Name, target);
                    nlogConfig.LoggingRules.Add(new LoggingRule(pattern.Resource.Name, GetLogLevel(pattern), target));
                }
            }

            LogManager.Configuration = nlogConfig;
        }

        private LogLevel GetLogLevel(LogSetting pattern)
        {
            var logLevel = pattern.Resource.LogLevel;
            if (string.IsNullOrWhiteSpace(logLevel))
            {
                return LogLevel.Debug;
            }
            return LogLevel.FromString(logLevel);
        }

        /// <summary>
        /// Info レベルのログを出力します。
        /// </summary>
        /// <param name="data">出力するログ内容</param>
        public void Info(LogData data)
        {
            OutputLog(data, LogLevel.Info);
        }

        /// <summary>
        /// Debug レベルのログを出力します。
        /// </summary>
        /// <param name="data">出力するログ内容</param>
        public void Debug(LogData data)
        {
            OutputLog(data, LogLevel.Debug);
        }

        /// <summary>
        /// Trace レベルのログを出力します。
        /// </summary>
        /// <param name="data">出力するログ内容</param>
        public void Trace(LogData data)
        {
            OutputLog(data, LogLevel.Trace);
        }

        /// <summary>
        /// Warn レベルのログを出力します。
        /// </summary>
        /// <param name="data">出力するログ内容</param>
        public void Warn(LogData data)
        {
            OutputLog(data, LogLevel.Warn);
        }

        /// <summary>
        /// Error レベルのログを出力します。
        /// </summary>
        /// <param name="data">出力するログ内容</param>
        public void Error(LogData data)
        {
            OutputLog(data, LogLevel.Error);
        }

        /// <summary>
        /// Fatal レベルのログを出力します。
        /// </summary>
        /// <param name="data">出力するログ内容</param>
        public void Fatal(LogData data)
        {
            OutputLog(data, LogLevel.Fatal);
        }

        private void OutputLog(LogData data, LogLevel logLevel)
        {
            var logger = LogManager.GetLogger(data.LogName);
            var eventInfo = CreateLogEventInfo(data, logLevel);
            logger.Log(this.GetType(), eventInfo);
        }

        private LogEventInfo CreateLogEventInfo(LogData data, LogLevel level)
        {
            var logEvent = new LogEventInfo(level, data.LogName, data.Message);
            logEvent.Exception = data.Exception;

            foreach (var key in data.Items.Keys)
            {
                logEvent.Properties.Add(key.ToLowerInvariant(), data.Items[key]);
            }
            return logEvent;
        }
    }


}
