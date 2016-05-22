using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;

namespace Archpack.Training.ArchUnits.Data.Sql.V1
{
    /// <summary>
    /// SQL定義。
    /// </summary>
    public class SqlDefinition
    {

        public string Sql { get; set; }

        public Dictionary<string, ParameterDefinition> Parameters { get; private set; }

        /// <summary>
        /// SQL定義のコンストラクタ。
        /// </summary>
        public SqlDefinition()
            : this(null)
        {
        }

        /// <summary>
        /// SQL定義のコンストラクタ。
        /// </summary>
        /// <param name="sql"></param>
        public SqlDefinition(string sql)
        {
            this.Sql = sql;
            this.Parameters = new Dictionary<string, ParameterDefinition>();

            if (!string.IsNullOrEmpty(this.Sql))
            {
                this.Sql = ParseParameters(this.Sql);
            }
        }

        private const string ParameterRegexPattern = @"(?<prefix>\:|@|\?)(?<name>[A-Za-z_][A-Za-z0-9_]*)(/\*(?<type>[A-Za-z0-9]+)(\((?<size>[A-Za-z0-9]+)\))?\*/)?";

        /// <summary>
        /// パラメーターを解析。
        /// </summary>
        /// <param name="sql"></param>
        private string ParseParameters(string sql)
        {
            return Regex.Replace(sql, ParameterRegexPattern, match =>
            {
                string prefix = match.Groups["prefix"].Value;
                string name = match.Groups["name"].Value;
                string matchType = match.Groups["type"].Value;
                string matchSize = match.Groups["size"].Value;

                if (string.IsNullOrWhiteSpace(matchType))
                {
                    return prefix + name;
                }

                int dbSize = 0;
                if (!string.IsNullOrEmpty(matchSize))
                {
                    dbSize = matchSize.Equals("max", StringComparison.OrdinalIgnoreCase) ? ParameterDefinition.NVarcharMaxSize : Convert.ToInt32(matchSize);
                }

                DbType dbType = DbTypeMaps.ConvertToDbType(matchType);

                ParameterDefinition param = new ParameterDefinition(prefix, name, dbType, dbSize);

                if (this.Parameters.ContainsKey(name))
                {
                    ParameterDefinition sameParam = this.Parameters[name];
                    if (param.Type != sameParam.Type || param.Size != sameParam.Size)
                    {
                        throw new InvalidOperationException(string.Format("Already same name parameter is defined: {0}", name));
                    }
                }
                else
                {
                    this.Parameters.Add(name, param);
                }
                
                return prefix + name;
            }, RegexOptions.ExplicitCapture);
        }

        /// <summary>
        /// SQLマージ。
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public SqlDefinition Merge(SqlDefinition other)
        {
            this.Sql += (string.IsNullOrWhiteSpace(this.Sql) ? "" : " ") + other.Sql;
            foreach (var pair in other.Parameters)
            {
                this.Parameters[pair.Key] = pair.Value;
            }

            return this;
        }
    }
}