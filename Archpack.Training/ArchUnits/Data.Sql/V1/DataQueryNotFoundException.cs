using Archpack.Training.Properties;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using Archpack.Training.ArchUnits.Collections.V1;

namespace Archpack.Training.ArchUnits.Data.Sql.V1
{
    /// <summary>
    /// 名前付けされたクエリが見つからない場合に発行される例外を定義します。
    /// </summary>
    public class DataQueryNotFoundException : Exception
    {
        /// <summary>
        /// 指定されたクエリ名を使用してインスタンスを初期化します。
        /// </summary>
        /// <param name="queryName">クエリ名</param>
        public DataQueryNotFoundException(string queryName)
            : base(string.Format(Resources.DataQueryNotFoundExceptionMesage, queryName))
        {
            this.QueryName = queryName;
        }

        /// <summary>
        /// この例外を発生する元となった例外とクエリ名を使用してインスタンスを初期化します。
        /// </summary>
        /// <param name="innerException">この例外を発生する元となった例外</param>
        /// <param name="queryName">クエリ名</param>
        public DataQueryNotFoundException(Exception innerException, string queryName)
            : base(string.Format(Resources.DataQueryNotFoundExceptionMesage, queryName), innerException)
        {
            this.QueryName = queryName;
        }

        /// <summary>
        /// クエリ名を取得します。
        /// </summary>
        public string QueryName { get; private set; }
    }

    /// <summary>
    /// <see cref="DataQuery"/> の実行で例外が発生した場合に発行される例外を定義します。
    /// </summary>
    public class DataQueryException : Exception
    {
        /// <summary>
        /// インスタンスを初期化します。
        /// </summary>
        public DataQueryException() : base(Resources.DataQueryException) { }
        /// <summary>
        /// 指定されたSQL文とそのパラメーターを使用してインスタンスを初期化します。
        /// </summary>
        /// <param name="sql">SQL文</param>
        /// <param name="parameters">SQLパラメーター</param>
        public DataQueryException(string sql, IEnumerable<DbParameter> parameters)
            : this()
        {
            SetSqlAndParameter(sql, parameters);
        }

        /// <summary>
        /// この例外を発生する元となった例外を使用してインスタンスを初期化します。
        /// </summary>
        /// <param name="innerException">この例外を発生する元となった例外</param>
        public DataQueryException(Exception innerException) : base(Resources.DataQueryException, innerException) { }

        /// <summary>
        /// この例外を発生する元となった例外とSQL文およびそのパラメーターを使用してインスタンスを初期化します。
        /// </summary>
        /// <param name="innerException">この例外を発生する元となった例外</param>
        /// <param name="sql">SQL文</param>
        /// <param name="parameters">SQLパラメーター</param>
        public DataQueryException(Exception innerException, string sql, IEnumerable<DbParameter> parameters)
            : this(innerException)
        {
            SetSqlAndParameter(sql, parameters);
        }

        private void SetSqlAndParameter(string sql, IEnumerable<DbParameter> parameters)
        {
            this.Sql = sql;
            this.Parameters = parameters.ToSafe().Select(p => new KeyValuePair<string, object>(p.ParameterName, p.Value)).ToList();
        }
        /// <summary>
        /// SQL文を取得します。
        /// </summary>
        public string Sql { get; private set; }
        /// <summary>
        /// パラメーターを取得します。
        /// </summary>
        public IEnumerable<KeyValuePair<string, object>> Parameters { get; private set; }
    }
}