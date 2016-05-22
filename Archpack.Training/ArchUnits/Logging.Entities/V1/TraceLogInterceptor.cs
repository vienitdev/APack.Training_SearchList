using Archpack.Training.ArchUnits.Contracts.V1;
using Archpack.Training.ArchUnits.Logging.V1;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Interception;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace Archpack.Training.ArchUnits.Logging.Entities.V1
{
    /// <summary>
    /// SQLのトレースログを出力するインターセプターを定義します。
    /// </summary>
    public class TraceLogInterceptor : IDbCommandInterceptor, IDisposable
    {
        private WeakReference context;
        private WeakReference logContext;
        private bool disposed = false;

        private readonly Stopwatch stopwatch;
        private List<CommandLogEntry> entries = new List<CommandLogEntry>();

        private DbContext databaseContext
        {
            get
            {
                if (this.context == null || !this.context.IsAlive)
                {
                    return null;
                }
                return (DbContext)this.context.Target;
            }
        }

        private LogContext cmdLogContext
        {
            get
            {
                if (this.logContext == null || !this.logContext.IsAlive)
                {
                    return null;
                }
                return (LogContext)this.logContext.Target;
            }
        }

        /// <summary>
        /// 指定された <see cref="DbContext"/> と <see cref="LogContext"/> を利用してインスタンスを初期化します。
        /// </summary>
        /// <param name="context">出力対象の <see cref="DbContext"/> </param>
        /// <param name="logContext">出力先となる <see cref="LogContext"/> </param>
        public TraceLogInterceptor(DbContext context, LogContext logContext)
        {
            Contract.NotNull(context, "context");
            this.context = new WeakReference(context);
            this.logContext = new WeakReference(logContext);

            this.stopwatch = new Stopwatch();

            this.StartInterception();
        }

        /// <summary>
        /// ストップウォッチをリセットして開始します。
        /// </summary>
        private void WatchResetAndStart()
        {
            stopwatch.Reset();
            stopwatch.Start();
        }


        /// <summary>
        /// インターセプトした内容が対象となる DbContext かどうかを確認します。
        /// </summary>
        private bool IncludeTargetContext(IEnumerable<DbContext> contexts)
        {
            var target = this.databaseContext;
            if (target == null)
            {
                //参照が取れない = 今後利用できない
                this.StopInterception();
                return false;
            }

            return contexts.Any(d =>
            {
                return Object.ReferenceEquals(d, target);
            });
        }

        /// <summary>
        /// 外部定義に設定されている監査ログ対象のテーブルへのクエリかをチェックし、対象の場合はエントリーを作成します。
        /// </summary>
        private void CreateLogEntryIfTarget(DbCommand command)
        {
            var entry = CommandLogEntry.Create(command);
            entry.ElapsedMilliseconds = stopwatch.ElapsedMilliseconds;
            entry.ExecutedDateTime = DateTimeOffset.Now;
            entries.Add(entry);
        }

        /// <summary>
        /// 保持しているエントリーを監査ログに出力します。
        /// </summary>
        private void FlushEntries()
        {
            var flushTargets = entries.ToList();
            entries = new List<CommandLogEntry>();

            var logContext = this.cmdLogContext;
            if (logContext == null)
            {
                return;
            }
            foreach (var entry in flushTargets)
            {
                string commandtext = "";
                string parameters = "Params: ";
                LogData logData = new LogData();
                logData.LogName = "trace";
                logData.LogId = logContext.Id;
                logData.User = logContext.Identity.Name;

                foreach (CommandParameterLogEntry param in entry.Parameters)
                {
                    parameters += string.Format("[{0} - {1}] ", param.Name, param.Value);
                }

                commandtext = string.Format("Elapsed: {0}ms, SQL: {1} | {2}", entry.ElapsedMilliseconds ?? -1, entry.CommandText, parameters);
                logData.Message = commandtext.Trim(); ;
                logContext.Logger.Trace(logData);
            }
        }

        /// <summary>
        /// 保持しているエントリーをクリアします。
        /// </summary>
        private void ClearEntries()
        {
            entries.Clear();
        }

        /// <summary>
        /// インターセプトを開始します。
        /// </summary>
        private void StartInterception()
        {
            if (disposed)
            {
                return;
            }
            if (stopwatch.IsRunning)
            {
                stopwatch.Stop();
            }
            stopwatch.Reset();
            DbInterception.Add(this);
        }

        /// <summary>
        /// インターセプトを停止します。
        /// </summary>
        private void StopInterception()
        {
            if (disposed)
            {
                return;
            }

            if (stopwatch.IsRunning)
            {
                stopwatch.Stop();

            }
            stopwatch.Reset();

            DbInterception.Remove(this);
        }

        /// <summary>
        /// 利用されているリソースを解放し、インターセプトを終了します。
        /// </summary>
        public void Dispose()
        {
            try
            {
                this.StopInterception();
                this.context = null;
                this.logContext = null;
                if (stopwatch.IsRunning)
                {
                    stopwatch.Stop();
                }
                disposed = true;
            }
            catch { }
        }

        void IDbCommandInterceptor.NonQueryExecuted(DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
        {
            if (this.IncludeTargetContext(interceptionContext.DbContexts))
            {
                stopwatch.Stop();
                CreateLogEntryIfTarget(command);
                FlushEntries();
            }
        }

        void IDbCommandInterceptor.NonQueryExecuting(DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
        {
            WatchResetAndStart();
        }

        void IDbCommandInterceptor.ReaderExecuted(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
        {
            if (this.IncludeTargetContext(interceptionContext.DbContexts))
            {
                stopwatch.Stop();
                CreateLogEntryIfTarget(command);
                FlushEntries();
            }
        }

        void IDbCommandInterceptor.ReaderExecuting(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
        {
            WatchResetAndStart();
        }

        void IDbCommandInterceptor.ScalarExecuted(DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
        {
            if (this.IncludeTargetContext(interceptionContext.DbContexts))
            {
                stopwatch.Stop();
                CreateLogEntryIfTarget(command);
                FlushEntries();
            }
        }

        void IDbCommandInterceptor.ScalarExecuting(DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
        {
            WatchResetAndStart();
        }
    }
}