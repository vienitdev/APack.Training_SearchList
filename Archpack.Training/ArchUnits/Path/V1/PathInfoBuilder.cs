using Archpack.Training.ArchUnits.Configuration.V1;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Archpack.Training.ArchUnits.Path.V1
{
    /// <summary>
    /// <see cref="PathInfo"/> を構築する機能を提供します。
    /// </summary>
    public static class PathInfoBuilder
    {
        /// <summary>
        /// 指定されたパスを解析し、結果の <see cref="PathInfo"/> を返します。
        /// </summary>
        /// <param name="path">解析するパス</param>
        /// <returns>解析された情報を保持する <see cref="PathInfo"/></returns>
        public static PathInfo Build(String path)
        {
            var targetPath = path;
            if (string.IsNullOrWhiteSpace(targetPath))
            {
                throw new ArgumentNullException("path");
            }
            targetPath = targetPath.Trim();
            if (targetPath.StartsWith("/"))
            {
                targetPath = targetPath.Substring(1);
            }
            if (targetPath.EndsWith("/"))
            {
                targetPath = targetPath.Substring(0, targetPath.Length - 1);
            }
            if (string.IsNullOrEmpty(targetPath))
            {
                return null;
                //"ルートのみのパスは許可されません。"
            }

            var uri = new Uri("http://localhost/" + targetPath, UriKind.Absolute);

            var segments = uri.Segments.Skip(1).Select(seg => seg.TrimEnd('/')).ToArray();

            var baseSegments = segments;
            var specificSegments = Enumerable.Empty<string>();
            var specificSegmentIndex = baseSegments.FindIndex(s => s == "_");
            if (specificSegmentIndex > -1)
            {
                specificSegments = baseSegments.Skip(specificSegmentIndex + 1);
                if (specificSegments.Count() < 1)
                {
                    return null;
                    //"URLの末尾が特殊処理URLを表す _ で終わっています。"
                }
                baseSegments = baseSegments.Take(specificSegmentIndex).ToArray();
            }

            if (baseSegments.Length < 3)
            {
                return null;
                //"URLには処理の指定までが必要です。 {ServiceUnitName}/{Version}/{Role}/{ProcessType}"
            }

            //一覧の設定にない値の場合は ProcessType とみなす処理
            var baseSegmentList = baseSegments.ToList();
            var specificSegmentList = specificSegments.ToList();

            var config = ServiceConfigurationLoader.Load(baseSegmentList[0], baseSegmentList[1]);

            var result = new PathInfo();
            result.ServiceUnitName = baseSegmentList[0];
            result.Version = baseSegmentList[1];

            var pos = 1;
            if (config.AvailableRoles.Contains(baseSegmentList[2]))
            {
                if (baseSegments.Length < 4)
                {
                    return null;
                    //"URLには処理の指定までが必要です。 {ServiceUnitName}/{Version}/{Role}/{ProcessType}"
                }

                result.Role = baseSegmentList[2];
                config = ServiceConfigurationLoader.Load(baseSegmentList[0], baseSegmentList[1], baseSegmentList[2]);
                pos = 0;
            }
            result.Path = uri.AbsolutePath;

            result.ProcessType = baseSegmentList[3 - pos];
            result.ProcessPath = "/" + string.Join("/", baseSegmentList.Skip(4 - pos));

            if (specificSegmentList.Count > 0)
            {
                result.SpecificProcessPath = "/" + string.Join("/", specificSegmentList);
            }
            var queries = HttpUtility.ParseQueryString(uri.Query);
            result.Query = queries.AllKeys.Aggregate(new Dictionary<string, string>(), (dic, key) =>
            {
                dic.Add(key, queries.Get(key));
                return dic;
            });

            return result;
        }

        private static int FindIndex<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            var result = 0;
            foreach (var val in source)
            {
                if (predicate(val))
                {
                    return result;
                }
                result++;
            }
            return -1;
        }
    }
}