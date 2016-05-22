using Archpack.Training.ArchUnits.Contracts.V1;
using Archpack.Training.ArchUnits.Logging.V1;
using Archpack.Training.ArchUnits.Routing.V1;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace Archpack.Training.ArchUnits.Routing.V1
{
    /// <summary>
    /// ルーティング後の処理実行のトレースログを出力します。
    /// </summary>
    internal class TraceLogger : IDisposable
    {

        private Logger logger;
        private ServiceUnitContext context;
        private DateTime start;
        private Stopwatch stopwatch = new Stopwatch();

        /// <summary>
        /// <see cref="ServiceUnitContext"/> を指定してインスタンスを初期化します。
        /// </summary>
        /// <param name="context"><see cref="ServiceUnitContext"/></param>
        public TraceLogger(ServiceUnitContext context)
        {
            Contract.NotNull(context, "context");

            this.context = context;
            if (context.LogContext.Logger == null)
            {
                return;
            }
            this.logger = context.LogContext.Logger;
            this.start = DateTime.Now;
            stopwatch.Start();
        }

        private void Output(DateTime? start, DateTime? end)
        {
            if (logger == null) {
                return;
            }

            var logData = new LogData();
            logData.LogId = context.Id;
            logData.User = context.User == null ? "" : context.User.Identity.Name;
            if (start.HasValue) {
                logData.Items.Add("StartTime", start.Value);
            }
            if (end.HasValue)
            {
                logData.Items.Add("EndTime", end.Value);
            }
            logData.LogName = "trace";
            logData.Uri = context.Request.Path;
            logData.Message = string.Format("Start = {0}, End = {1}", start.HasValue ? start.Value.ToString("yyyy/MM/dd HH:mm:ss.fff") : "",
                                                                      end.HasValue? end.Value.ToString("yyyy/MM/dd HH:mm:ss.fff") : "");
            if (start.HasValue && end.HasValue)
            {
                stopwatch.Stop();
                logData.Message += string.Format(", Ellapsed = {0} ms", stopwatch.ElapsedMilliseconds);
            }
            logger.Trace(logData);

        }

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Output(start, DateTime.Now);
                }
                disposedValue = true;
            }
        }
        /// <summary>
        /// インスタンスの破棄時に実行されます。
        /// </summary>
   
        public void Dispose()
        {
            Dispose(true);
        }
    }
}