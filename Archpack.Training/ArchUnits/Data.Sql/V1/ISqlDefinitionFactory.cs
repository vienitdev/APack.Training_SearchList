using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Archpack.Training.ArchUnits.Data.Sql.V1
{
    /// <summary>
    /// クエリ名から <see cref="SqlDefinition"/> を生成する機能を提供するためのインターフェイスを定義します。
    /// </summary>
    public interface ISqlDefinitionFactory
    {
        /// <summary>
        /// 指定したクエリ名で定義された文字列を利用した <see cref="SqlDefinition"/> のインスタンスを作成します。
        /// </summary>
        /// <param name="queryName">クエリ名</param>
        /// <param name="replaceValues">置換プレースホルダーに適用する値</param>
        /// <returns>読み込まれたクエリが設定された <see cref="SqlDefinition"/> インスタンス</returns>
        SqlDefinition Create(string queryName, params object[] replaceValues);

        /// <summary>
        /// 指定したクエリ名で定義された文字列を取得します。
        /// </summary>
        /// <param name="queryName">クエリ名</param>
        /// <returns>指定したクエリ名で定義された文字列</returns>
        string GetPlainText(string queryName);
    }

    public static class ISqlDefinitionFactoryExtensions
    {
        /// <summary>
        /// 指定したSQL文字列に定義された置換プレースホルダーを指定された値で置換します。
        /// </summary>
        /// <param name="factory"><see cref="ISqlDefinitionFactory"/> のインスタンス</param>
        /// <param name="sql">SQL文字列</param>
        /// <param name="replaceValues">置換する値</param>
        /// <returns>置換プレースホルダーを置換する値で置換されたSQL文字列</returns>
        public static string ReplacePlaceholder(this ISqlDefinitionFactory factory, string sql, params object[] replaceValues)
        {
            if (string.IsNullOrEmpty(sql))
            {
                return sql;
            }
            if (replaceValues == null || replaceValues.Length < 1)
            {
                return sql;
            }
            return Regex.Replace(sql, @"/\*\{(?<param>(?<index>0|([1-9][0-9]*))(?<option>.*?))\}\*/", match =>
            {
                var index = int.Parse(match.Groups["index"].Value);
                if (replaceValues.Length <= index)
                {
                    return string.Empty;
                }
                return string.Format(string.Concat("{", 0, match.Groups["option"].Value, "}"), replaceValues[index]);
            }, RegexOptions.ExplicitCapture);
        }

        /// <summary>
        /// 指定したクエリ名で定義された文字列に置換プレースホルダーを指定された値で置換した文字列を取得します。
        /// </summary>
        /// <param name="factory"><see cref="ISqlDefinitionFactory"/> のインスタンス</param>
        /// <param name="queryName">クエリ名</param>
        /// <param name="replaceValues">置換する値</param>
        /// <returns>指定したクエリ名で定義された文字列に置換プレースホルダーを指定された値で置換した文字列</returns>
        public static string GetReplacedText(this ISqlDefinitionFactory factory, string queryName, params object[] replaceValues)
        {
            var query = factory.GetPlainText(queryName);
            return factory.ReplacePlaceholder(query, replaceValues);
        }

        public static IEnumerable<string> GetDependNames(this ISqlDefinitionFactory factory, string sql)
        {
            foreach (Match match in Regex.Matches(sql, @"/\*\*\s*DependOn\((?<queryName>.+?)\)\s*\*/"))
            {
                yield return match.Groups["queryName"].Value;
            }
        }
    }
}
