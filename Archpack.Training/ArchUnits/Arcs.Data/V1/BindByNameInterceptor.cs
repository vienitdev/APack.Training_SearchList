using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Infrastructure.Interception;
using System.Linq;
using System.Web;

namespace Archpack.Training.ArchUnits.Arcs.Data.V1
{
    /// <summary>
    /// SQLパラメーターのバインドを名前でマッピングする設定にクエリ実行前に変更するインターセプターを定義します。
    /// </summary>
    public class BindByNameInterceptor: IDbCommandInterceptor
    {
        /// <summary>
        /// <see cref="DbCommand.ExecuteNonQuery"/> の発行後に実行されます。処理は含まれません。
        /// </summary>
        /// <param name="command">発行されるコマンド</param>
        /// <param name="interceptionContext">実行コンテキスト</param>
        public void NonQueryExecuted(DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
        {
        }
        /// <summary>
        /// <see cref="DbCommand.ExecuteNonQuery"/> の発行前に実行され、バインドをSQLパラメーター名でマッピングする設定を行います。
        /// </summary>
        /// <param name="command">発行されるコマンド</param>
        /// <param name="interceptionContext">実行コンテキスト</param>
        public void NonQueryExecuting(DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
        {
            if (command is OracleCommand)
            {
                ((OracleCommand)command).BindByName = true;
            }
        }

        /// <summary>
        /// <see cref="DbCommand.ExecuteReader"/> の発行後に実行されます。処理は含まれません。
        /// </summary>
        /// <param name="command">発行されるコマンド</param>
        /// <param name="interceptionContext">実行コンテキスト</param>
        public void ReaderExecuted(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
        {
        }

        /// <summary>
        /// <see cref="DbCommand.ExecuteReader"/> の発行前に実行され、バインドをSQLパラメーター名でマッピングする設定を行います。
        /// </summary>
        /// <param name="command">発行されるコマンド</param>
        /// <param name="interceptionContext">実行コンテキスト</param>
        public void ReaderExecuting(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
        {
            if (command is OracleCommand)
            {
                ((OracleCommand)command).BindByName = true;
            }
        }

        /// <summary>
        /// <see cref="DbCommand.ExecuteScalar"/> の発行後に実行されます。処理は含まれません。
        /// </summary>
        /// <param name="command">発行されるコマンド</param>
        /// <param name="interceptionContext">実行コンテキスト</param>
        public void ScalarExecuted(DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
        {
        }

        /// <summary>
        /// <see cref="DbCommand.ExecuteScalar"/> の発行前に実行され、バインドをSQLパラメーター名でマッピングする設定を行います。
        /// </summary>
        /// <param name="command">発行されるコマンド</param>
        /// <param name="interceptionContext">実行コンテキスト</param>
        public void ScalarExecuting(DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
        {
            if (command is OracleCommand)
            {
                ((OracleCommand)command).BindByName = true;
            }
        }
    }
}