using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Archpack.Training.ArchUnits.Path.V1;
using System.Text.RegularExpressions;
using System.Collections.Concurrent;

namespace Archpack.Training.ArchUnits.Path.V1
{
    /// <summary>
    /// 指定されたパスを指定されたパターンのパスに変換する機能を提供します。
    /// </summary>
    public static class PathMapper
    {
        private static ConcurrentDictionary<string, string> regexPool = new ConcurrentDictionary<string, string>();

        /// <summary>
        /// 指定されたパスを指定されたパターンで解析し、パラメーターのコレクションを返します。
        /// </summary>
        /// <example>
        /// var result = PathMapper.Parse("/ServiceUnit/Member/V1/Admin", 
        ///         "/ServiceUnit/{ServiceUnitName}/{Version}/{Role}")
        /// //ServiceUnitName = Member
        /// //Version = V1
        /// //Role = Admin
        /// </example>
        /// <param name="path">解析対象のパス</param>
        /// <param name="pattern">解析するパターン</param>
        /// <returns>パラメーターのコレクションを含む <see cref="PathMapResult"/> </returns>
        public static PathMapResult Parse(string path, string pattern)
        {
            var result = new PathMapResult();
            result.Path = path;

            var regexText = regexPool.GetOrAdd(pattern, (p) =>
            {
                return CreateRegexFromPattern(p);
            });
            var regex = new Regex(regexText, RegexOptions.Compiled);
            var match = regex.Match(path);
            if (!match.Success)
            {
                result.Success = false;
                return result;
            }
            var groupNames = regex.GetGroupNames();
            foreach (var groupName in groupNames.Where(s => s != "0"))
            {
                result.Parameters.Add(groupName, match.Groups[groupName].Value);
            }
            result.Success = true;
            return result;
        }

        private static string CreateRegexFromPattern(string pattern)
        {
            var regex = new Regex("{(?<key>[A-Za-z]+?)(\\s*:\\*\\s*)?}", RegexOptions.Compiled);
            var indexAndValues = new List<Tuple<int, string>>();
            var replacedValue = regex.Replace(pattern, match =>
            {
                var value = match.Value;
                var index = match.Index;
                indexAndValues.Add(new Tuple<int, string>(index, value));
                return "";
            });

            var addendLength = 0;
            var resultStrings = new List<string>();
            foreach (var iv in indexAndValues)
            {
                if (iv.Item1 != 0)
                {
                    continue;
                }
                PrepareRegexTemplate(ref addendLength, iv, resultStrings);
            }
            for (var i = 0; i < replacedValue.Length; i++)
            {
                var escapedChar = Regex.Escape(replacedValue[i].ToString());
                resultStrings.Add(escapedChar);
                foreach (var iv in indexAndValues)
                {
                    var sourceIndex = iv.Item1 + addendLength - 1;
                    if (sourceIndex != i)
                    {
                        continue;
                    }
                    PrepareRegexTemplate(ref addendLength, iv, resultStrings);
                }
            }

            return String.Join("", resultStrings);

        }

        private static void PrepareRegexTemplate(ref int addendLength, Tuple<int, string> iv, List<string> resultStrings)
        {
            addendLength -= iv.Item2.Length;
            var plainValue = iv.Item2.Substring(1, iv.Item2.Length - 2).Trim();
            var hasAster = false;
            if (plainValue.Contains(":") && plainValue.EndsWith("*"))
            {
                plainValue = plainValue.Split(new[] { ':' })[0].Trim();
                hasAster = true;
            }
            if (hasAster)
            {
                resultStrings.Add(string.Format("(?<{0}>[\\w\\/]+)", plainValue));
            }
            else
            {
                resultStrings.Add(string.Format("(?<{0}>[\\w]+)", plainValue));
            }
        }
        /// <summary>
        /// 指定されたパラメーターの値をパターンに適用します。
        /// </summary>
        /// <example>
        /// var parameter = new Dictionary&lt;string, string&gt;()
        /// {
        ///     { "ServiceUnitName", "Member" },
        ///     { "Version", "V1" },
        ///     { "Role", "Admin" }
        /// };
        /// PathMapper.Map(parameter, "/ServiceUnit/{ServiceUnitName}/{Version}/{Role}");
        /// //"/ServiceUnit/Member/V1/Admin"
        /// </example>
        /// <param name="parameter">パラメーター</param>
        /// <param name="mapPattern">適用するパターン</param>
        /// <returns>適用後の値を含む <see cref="PathMapResult"/> </returns>
        public static PathMapResult Map(IDictionary<string,string> parameter, string mapPattern)
        {
            var result = new PathMapResult();
            var regex = new Regex("{(?<key>[A-Za-z]+?)(\\s*:\\*\\s*)?}", RegexOptions.Compiled);
            var hasParameter = true;
            var mapPath = regex.Replace(mapPattern, match =>
            {
                var key = match.Groups["key"].Value;
                if (parameter.ContainsKey(key))
                {
                    return parameter[key];
                }
                hasParameter = false;
                return match.Value;
            });
            
            result.MappedPath = mapPath;
            result.Success = hasParameter;
            return result;
        }
        /// <summary>
        /// 指定された変換元のパターンのパス文字列を、指定された変換先のパターンに変換します。
        /// </summary>
        /// <example>
        /// var result = PathMapper.Convert("/ServiceUnit/Member/V1/Admin", 
        ///         "/ServiceUnit/{ServiceUnitName}/{Version}/{Role}",
        ///         "{Role}/{ServiceUnitName}/{Version}.aspx")
        /// //Admin/Memver/V1.aspx
        /// </example>
        /// <param name="path">パス</param>
        /// <param name="pattern">変換元のパターン</param>
        /// <param name="mapPattern">変換先のパターン</param>
        /// <returns>変換後のパスを含む <see cref="PathMapResult"/> </returns>
        public static PathMapResult Convert(string path, string pattern, string mapPattern)
        {
            var result = PathMapper.Parse(path, pattern);
            if (result.Success)
            {
                return PathMapper.Map(result.Parameters, mapPattern);
            }
            return result;
        }
    }
}