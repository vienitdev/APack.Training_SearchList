using System.IO;
using Archpack.Training.ArchUnits.Contracts.V1;

namespace Archpack.Training.ArchUnits.Data.Sql.V1
{
    /// <summary>
    /// ファイルからSQL ファイルの定義の読み込みを行います
    /// </summary>
    public class FileSqlDefinitionFactory : ISqlDefinitionFactory
    {
        private string serviceUnitDir = "";
        /// <summary>
        /// ファイルからSQL ファイルの定義の読み込みを行いますのコンストラクタ。
        /// </summary>
        /// <param name="serviceUnitDir"></param>
        public FileSqlDefinitionFactory(string serviceUnitDir)
        {
            Contract.NotEmpty(serviceUnitDir, "serviceUnitDir");
            this.serviceUnitDir = serviceUnitDir;
        }

        /// <summary>
        /// 指定したクエリ名で定義された文字列を利用した <see cref="SqlDefinition"/> のインスタンスを作成します。
        /// </summary>
        /// <param name="queryName">クエリ名</param>
        /// <param name="replaceParams">置換パラメーターに適用する値</param>
        /// <returns>読み込まれたクエリが設定された <see cref="SqlDefinition"/> インスタンス</returns>
        public SqlDefinition Create(string queryName, params object[] replaceParams)
        {
            var sql = this.ReplacePlaceholder(LoadSQLFromFile(queryName), replaceParams);
            var definition = new SqlDefinition(sql);

            return definition;
        }
        /// <summary>
        /// 指定したクエリ名で定義された文字列を取得します。
        /// </summary>
        /// <param name="queryName">クエリ名</param>
        /// <returns>指定したクエリ名で定義された文字列</returns>
        public string GetPlainText(string queryName)
        {
            return LoadSQLFromFile(queryName);
        }

        /// <summary>
        /// ファイルからSQLを取得します。
        /// </summary>
        private string LoadSQLFromFile(string queryName)
        {
            var path = System.IO.Path.Combine(serviceUnitDir, "Admin","Data", queryName);

            var targetFile = PrepareFilePath(path);
            if (string.IsNullOrEmpty(targetFile))
            {
                throw new DataQueryNotFoundException(queryName);
            }
        
            using (Stream stream = new FileStream(targetFile, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (TextReader r = new StreamReader(stream))
                {
                    return r.ReadToEnd();
                }
            }
        }

        private string PrepareFilePath(string path)
        {
            if (File.Exists(path))
            {
                return path;
            }
            if (string.IsNullOrEmpty(System.IO.Path.GetExtension(path)))
            {
                path = path + ".sql";
                if (File.Exists(path))
                {
                    return path;
                }
            }
            return null;
        }
    }
}