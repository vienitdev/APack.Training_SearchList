using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;

namespace Archpack.Training.ArchUnits.Data.Sql.V1
{
    /// <summary>
    /// データベースの型と<see cref="DbType"/> をマップする機能を提供します。
    /// </summary>
    public static class DbTypeMaps
    {
        /// <summary>
        /// Oracleデータベースの種類
        /// </summary>
        private static readonly ReadOnlyDictionary<string, DbType> Oracle = new ReadOnlyDictionary<string, DbType>(
            new Dictionary<string, DbType>()
            {
                { "char", DbType.AnsiStringFixedLength },
                { "date", DbType.DateTime },
                { "float", DbType.Double },
                { "integer", DbType.Int64 },
                { "interval year to month", DbType.Int32 },
                { "interval day to second", DbType.Object },
                { "long", DbType.AnsiString },
                { "long raw", DbType.Binary },
                { "nchar", DbType.StringFixedLength },
                { "number", DbType.Decimal },
                { "nvarchar2", DbType.String },
                { "raw", DbType.Binary },
                { "rowid", DbType.AnsiString },
                { "timestamp", DbType.DateTime },
                { "timestamp with local time zone", DbType.DateTime },
                { "timestamp with time zone", DbType.DateTime },
                { "unsigned integer", DbType.UInt64 },
                { "varchar2", DbType.AnsiString }
            }
        );

        /// <summary>
        /// SQLデータベースの種類
        /// </summary>
        private static readonly ReadOnlyDictionary<string, DbType> SqlServer = new ReadOnlyDictionary<string, DbType>(
            new Dictionary<string, DbType>()
            {
                {"bigint", DbType.Int64 },
                {"binary", DbType.Binary },
                {"bit", DbType.Boolean },
                {"char", DbType.AnsiStringFixedLength },
                {"date", DbType.Date },
                {"datetime", DbType.DateTime },
                {"datetime2", DbType.DateTime2 },
                {"datetimeoffset", DbType.DateTimeOffset },
                {"decimal", DbType.Decimal },
                {"FILESTREAM", DbType.Binary },
                {"float", DbType.Double },
                {"image", DbType.Binary },
                {"int", DbType.Int32 },
                {"money", DbType.Decimal },
                {"nchar", DbType.StringFixedLength },
                {"ntext", DbType.String },
                {"numeric", DbType.Decimal },
                {"nvarchar", DbType.String },
                {"real", DbType.Single },
                {"rowversion", DbType.Binary },
                {"smalldatetime", DbType.DateTime },
                {"smallint", DbType.Int16 },
                {"smallmoney", DbType.Decimal },
                {"sql_variant", DbType.Object },
                {"text", DbType.String },
                {"time", DbType.Time },
                {"timestamp", DbType.Binary },
                {"tinyint", DbType.Byte },
                {"uniqueidentifier", DbType.Guid },
                {"varbinary", DbType.Binary },
                {"varchar", DbType.AnsiString },
                {"xml", DbType.Xml }
            }
        );

        /// <summary>
        /// 指定されたデータベース型名を <see cref="DbType"/> に変換します。
        /// </summary>
        /// <param name="type">変換するデータベース型名</param>
        /// <returns>変換された <see cref="DbType"/> </returns>
        public static DbType ConvertToDbType(string type)
        {
            string lowerType = type.ToLower();

            if (Oracle.ContainsKey(lowerType))
            {
                return Oracle[lowerType];
            }
            else if (SqlServer.ContainsKey(lowerType))
            {
                return SqlServer[lowerType];
            }

            throw new ArgumentException(string.Format("The type: {0} is not defined in type mappings.", type));
        }
    }
}