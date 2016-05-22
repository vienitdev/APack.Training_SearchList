using Archpack.Training.ArchUnits.Contracts.V1;
using System.Data;

namespace Archpack.Training.ArchUnits.Data.Sql.V1
{
    /// <summary>
    /// パラメーター定義
    /// </summary>
    public class ParameterDefinition
    {
        /// <summary>
        /// パラメーター定義のコンストラクタ。
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="size"></param>
        public ParameterDefinition(string prefix, string name, DbType type, int size)
        {
            Contract.NotEmpty(prefix, "prefix");
            Contract.NotEmpty(name, "name");

            this.Prefix = prefix;
            this.Name = name;
            this.Type = type;
            this.Size = size;
        }

        public static int NVarcharMaxSize = int.MaxValue;

        public string Name { get; private set; }

        public DbType Type { get; private set; }

        public int Size { get; private set; }

        public string Prefix { get; private set; }
    }
}