using Archpack.Training.ArchUnits.Contracts.V1;
using Archpack.Training.ArchUnits.Logging.V1;
using Archpack.Training.Properties;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Interception;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Web;

namespace Archpack.Training.ArchUnits.Logging.Entities.V1
{
    /// <summary>
    /// SQLの監査ログを出力するインターセプターを定義します。
    /// </summary>
    public class AuditLogInterceptor : IDbCommandInterceptor, IDbTransactionInterceptor, IDisposable
    {
        private WeakReference context;
        private WeakReference logContext;
        private bool disposed = false;

        private readonly Stopwatch stopwatch;

        private Regex updateRegex = new Regex("^UPDATE +?(.+)", RegexOptions.IgnoreCase);
        private Regex insertRegex = new Regex("^INSERT +?(.+) *?\\(", RegexOptions.IgnoreCase);
        private Regex deleteRegex = new Regex("^DELETE +?(.+)", RegexOptions.IgnoreCase);
        private Regex selectRegex = new Regex("^SELECT +?(.+)", RegexOptions.IgnoreCase);

        private delegate bool Retriver(DbCommand command, out string tableName);

        private List<Retriver> retrivers = new List<Retriver>();
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
        public AuditLogInterceptor(DbContext context, LogContext logContext)
        {
            Contract.NotNull(context, "context");
            this.context = new WeakReference(context);
            this.logContext = new WeakReference(logContext);

            this.stopwatch = new Stopwatch();

            this.retrivers.Add((DbCommand command, out string tableName) =>
            {
                return TryRetrieveTableNameFromCommand(command, updateRegex, out tableName);
            });
            this.retrivers.Add((DbCommand command, out string tableName) =>
            {
                return TryRetrieveTableNameFromCommand(command, insertRegex, out tableName);
            });
            this.retrivers.Add((DbCommand command, out string tableName) =>
            {
                return TryRetrieveTableNameFromCommand(command, deleteRegex, out tableName);
            });
            this.retrivers.Add((DbCommand command, out string tableName) =>
            {
                return TryRetrieveTableNameFromCommand(command, selectRegex, out tableName);
            });

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
        /// コマンドからテーブル名を取得します。
        /// </summary>
        private string RetrieveTableName(DbCommand command)
        {
            string tableName;
            foreach (var retriver in this.retrivers)
            {
                if (retriver(command, out tableName))
                {
                    return tableName;
                }
            }
            return null;
        }

        /// <summary>
        /// コマンドからテーブル名を取得します。
        /// </summary>
        private bool TryRetrieveTableNameFromCommand(DbCommand command, Regex regex, out string tableName)
        {
            tableName = null;
            var match = regex.Match(command.CommandText);
            if (match.Length > 0)
            {
                string entityName = match.Groups[1].Value;
                string targetName = entityName.Trim().Split('.').Last();
                if (targetName.StartsWith("[") && targetName.EndsWith("]"))
                {
                    targetName = targetName.Substring(1, targetName.Length - 2);
                }
                tableName = targetName;
                return true;
            }
            return false;
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
            var table = RetrieveTableName(command);
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
            if(logContext == null)
            {
                return;
            }
            foreach (var entry in entries)
            {
                string commandtext = "";
                string parameters = "Params: ";
                string tableName = "Table: ";
                LogData logData = new LogData();
                logData.LogName = "audit";
                logData.LogId = logContext.Id;
                logData.User = logContext.Identity.Name;
                
                foreach (CommandParameterLogEntry param in entry.Parameters)
                {
                    parameters += string.Format("[{0} - {1}] ", param.Name, param.Value);
                }
                tableName += RetrieveTableName(entry.DbCommand);
                commandtext = "SQL: " + entry.CommandText + " | " + tableName + " | " + parameters;
                logData.Message = commandtext.Trim();
                logContext.Logger.Info(logData);
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
            }
        }

        void IDbCommandInterceptor.ScalarExecuting(DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
        {
            WatchResetAndStart();
        }

        void IDbTransactionInterceptor.Committed(DbTransaction transaction, DbTransactionInterceptionContext interceptionContext)
        {
            if (this.IncludeTargetContext(interceptionContext.DbContexts))
            {
                FlushEntries();
            }
        }

        void IDbTransactionInterceptor.Committing(DbTransaction transaction, DbTransactionInterceptionContext interceptionContext)
        {
        }

        void IDbTransactionInterceptor.ConnectionGetting(DbTransaction transaction, DbTransactionInterceptionContext<DbConnection> interceptionContext)
        {
        }

        void IDbTransactionInterceptor.ConnectionGot(DbTransaction transaction, DbTransactionInterceptionContext<DbConnection> interceptionContext)
        {
        }

        void IDbTransactionInterceptor.Disposed(DbTransaction transaction, DbTransactionInterceptionContext interceptionContext)
        {
        }

        void IDbTransactionInterceptor.Disposing(DbTransaction transaction, DbTransactionInterceptionContext interceptionContext)
        {
        }

        void IDbTransactionInterceptor.IsolationLevelGetting(DbTransaction transaction, DbTransactionInterceptionContext<System.Data.IsolationLevel> interceptionContext)
        {
        }

        void IDbTransactionInterceptor.IsolationLevelGot(DbTransaction transaction, DbTransactionInterceptionContext<System.Data.IsolationLevel> interceptionContext)
        {
        }

        void IDbTransactionInterceptor.RolledBack(DbTransaction transaction, DbTransactionInterceptionContext interceptionContext)
        {
            if (this.IncludeTargetContext(interceptionContext.DbContexts))
            {
                ClearEntries();
            }
        }

        void IDbTransactionInterceptor.RollingBack(DbTransaction transaction, DbTransactionInterceptionContext interceptionContext)
        {
        }
    }
}