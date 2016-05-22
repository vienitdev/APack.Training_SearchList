using Archpack.Training.ArchUnits.Contracts.V1;
using System;
using System.Security.Principal;

namespace Archpack.Training.ArchUnits.Logging.V1
{
    /// <summary>
    /// 一連のログ出力で一意なIDを保持するためのコンテキストを定義します。
    /// </summary>
    public class LogContext
    {
        /// <summary>
        /// 指定された <see cref="Logger"/> 、一意なIDおよび <see cref="IIdentity"/> を利用してインスタンスを初期化します。
        /// </summary>
        /// <param name="logger">ログ出力を行う <see cref="Logger"/> インスタンス</param>
        /// <param name="id">コンテキスト内で保持される一意なID</param>
        /// <param name="identity">ユーザー情報を保持する <see cref="IIdentity"/></param>
        public LogContext(Logger logger, Guid id, IIdentity identity)
        {
            Contract.NotNull(logger, "logger");

            this.Id = id;
            this.Identity = identity;
            this.Logger = logger;        
        }
        /// <summary>
        /// このコンテキストに紐づけられたIDを取得します。
        /// </summary>
        public Guid Id { get; private set; }
        /// <summary>
        /// ユーザー情報を保持する <see cref="IIdentity"/> を取得します。
        /// </summary>
        public IIdentity Identity { get; private set; }
        /// <summary>
        /// ログ出力を行う <see cref="Logger"/> インスタンスを取得します。
        /// </summary>
        public Logger Logger { get; private set; }

        /// <summary>
        /// <see cref="LogContext"/> が保持する情報をもとに <see cref="LogData"/> のインスタンスを作成します。
        /// </summary>
        /// <returns><see cref="LogContext"/> が保持する情報が設定された <see cref="LogData"/> のインスタンス</returns>
        public LogData CreateLogData()
        {
            var result = new LogData();
            result.LogId = this.Id;
            result.User = this.Identity == null ? null : this.Identity.Name;
            return result;
        }
        /// <summary>
        /// <see cref="LogContext"/> が保持する情報を<see cref="LogData"/>のインスタンスに設定します。
        /// </summary>
        /// <param name="data">設定する <see cref="LogData"/></param>
        public void ApplyTo(LogData data)
        {
            Contract.NotNull(data, "data");

            if (this.Identity != null)
            {
                data.User = this.Identity.Name;
            }
            data.LogId = this.Id;
        }

    }
}