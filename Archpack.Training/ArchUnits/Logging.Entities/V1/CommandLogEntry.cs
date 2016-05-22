using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Security.Principal;
using System.Text;

namespace Archpack.Training.ArchUnits.Logging.Entities.V1
{
    /// <summary>
    /// ログに出力するコマンドのエントリー
    /// </summary>
    public class CommandLogEntry
    {
        /// <summary>
        /// 出力するSQL文を取得または設定します。
        /// </summary>
        public string CommandText { get; set; }
        /// <summary>
        /// SQLの実行時間を取得または設定します。
        /// </summary>
        public long? ElapsedMilliseconds { get; set; }
        /// <summary>
        /// SQLが実行されたローカル時間を取得または設定します。
        /// </summary>
        public DateTimeOffset ExecutedDateTime { get; set; }
        /// <summary>
        /// SQLを実行したアプリケーションユーザーを取得または設定します。
        /// </summary>
        public IIdentity UserIdentity { get; set; }

        public DbCommand DbCommand { get; set; }

        /// <summary>
        /// 出力するSQLのパラメーターセットを取得または設定します。
        /// </summary>
        public IEnumerable<CommandParameterLogEntry> Parameters { get; private set; }
        /// <summary>
        /// 指定された <see cref="DbCommand"/> から <see cref="CommandLogEntry"/> のインスタンスを生成します。
        /// </summary>
        /// <param name="command">エントリーを作成する元となる <see cref="DbCommand"/> </param>
        /// <returns><see cref="DbCommand"/> から作成された <see cref="CommandLogEntry"/> のインスタンス</returns>
        public static CommandLogEntry Create(DbCommand command)
        {
            var entry = new CommandLogEntry();
            entry.DbCommand = command;
            entry.CommandText = command.CommandText.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Trim();

            var paramEntires = new List<CommandParameterLogEntry>();
            foreach (DbParameter param in command.Parameters)
            {
                var paramEntry = new CommandParameterLogEntry();
                paramEntry.Name = param.ParameterName;
                paramEntry.Value = param.Value == null || DBNull.Value == param.Value ? "[null]" : param.Value.ToString();
                paramEntry.DbType = param.DbType.ToString();
                paramEntry.Direction = param.Direction.ToString();
                paramEntry.IsNullable = param.IsNullable;
                paramEntry.Size = param.Size;
                paramEntry.Precision = ((IDbDataParameter)param).Precision;
                paramEntry.Scale = ((IDbDataParameter)param).Scale;
                paramEntires.Add(paramEntry);
            }
            entry.Parameters = paramEntires;

            return entry;
        }
        /// <summary>
        /// エントリーの内容をログに出力する文字列に変換して返します。
        /// </summary>
        /// <returns>ログ出力用に文字列化されたエントリーの内容</returns>
        public string ToLogString()
        {
            var builder = new StringBuilder();
            builder.AppendFormat("executedDate: {1}, ellapsed: {2}ms, user: {3}, sql: {0} ",
                CommandText, ExecutedDateTime.ToString("yyyy-MM-dd HH:mm:ss.fffzzz"), ElapsedMilliseconds, UserIdentity.Name);

            foreach (var paramEntry in Parameters)
            {
                builder.AppendFormat(" {0}", paramEntry.ToLogString());
            }

            return builder.ToString();
        }
    }
    /// <summary>
    /// ログに出力するコマンドパラメーターのエントリー
    /// </summary>
    public class CommandParameterLogEntry
    {
        /// <summary>
        /// パラメーター名を取得または設定します。
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// パラメーター値を取得または設定します。
        /// </summary>
        public object Value { get; set; }
        /// <summary>
        /// DB上の型を取得または設定します。
        /// </summary>
        public string DbType { get; set; }
        /// <summary>
        /// パラメーターの方向を取得または設定します。
        /// </summary>
        public string Direction { get; set; }
        /// <summary>
        /// パラメーターが null 値を受け入れるかどうかを取得または設定します。
        /// </summary>
        public bool IsNullable { get; set; }
        /// <summary>
        /// パラメーターの値として有効な最大サイズ
        /// </summary>
        public int Size { get; set; }
        /// <summary>
        /// 数値パラメーターの有効桁数
        /// </summary>
        public int Precision { get; set; }
        /// <summary>
        /// 数値パラメーターの小数部桁数
        /// </summary>
        public int Scale { get; set; }
        /// <summary>
        /// エントリーの内容をログに出力する文字列に変換して返します。
        /// </summary>
        /// <returns>ログ出力用に文字列化されたエントリーの内容</returns>
        public string ToLogString()
        {
            return String.Format("parameter: [{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}]", Name, Value, DbType, Direction, IsNullable, Size, Precision, Scale);
        }
    }
}
