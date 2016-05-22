using System;
using System.Collections.Generic;
using System.Linq;

namespace Archpack.Training.ArchUnits.Collections.V1
{
    public static class IEnumerableExtensions
    {
        public static TSource Aggregate<TSource>(this IEnumerable<TSource> source, Func<TSource, TSource, int, TSource> func)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (func == null) throw new ArgumentNullException("func");
            var index = 1;
            using (var enumerator = source.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                {
                    throw new InvalidOperationException("Element not found.");
                }
                var current = enumerator.Current;
                while (enumerator.MoveNext())
                {
                    current = func(current, enumerator.Current, index++);
                }
                return current;
            }
        }

        public static TAccumulate Aggregate<TSource, TAccumulate>(this IEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, int, TAccumulate> func)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (func == null) throw new ArgumentNullException("func");
            var index = 0;
            var item = seed;
            foreach (var item1 in source)
            {
                item = func(item, item1, index++);
            }
            return item;
        }

        public static TResult Aggregate<TSource, TAccumulate, TResult>(this IEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, int, TAccumulate> func, Func<TAccumulate, TResult> resultSelector)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (func == null) throw new ArgumentNullException("func");
            if (resultSelector == null) throw new ArgumentNullException("resultSelector");
            var index = 0;
            var item = seed;
            foreach (var item1 in source)
            {
                item = func(item, item1, index++);
            }
            return resultSelector(item);
        }

        /// <summary>
        /// 指定された区切り文字で、コレクションのメンバーを連結した文字列を返します。
        /// </summary>
        /// <typeparam name="T">コレクションのタイプ</typeparam>
        /// <param name="source">連結するコレクション</param>
        /// <param name="separator">区切り文字</param>
        /// <returns>指定された区切り文字で、コレクションのメンバーを連結した文字列</returns>
        public static string ConcatWith<T>(this IEnumerable<T> source, string separator)
        {
            if (source == null) throw new ArgumentNullException("source");
            return string.Join(separator, source);
        }
        /// <summary>
        /// 指定された区切り文字とフォーマットでコレクションのメンバーを連結した文字列を返します。
        /// </summary>
        /// <typeparam name="T">コレクションのタイプ</typeparam>
        /// <param name="source">連結するコレクション</param>
        /// <param name="separator">区切り文字</param>
        /// <param name="format">フォーマット文字。必ず {0} を含む必要があります。</param>
        /// <param name="formatProvider">カルチャ固有の書式情報</param>
        /// <returns>指定された区切り文字とフォーマットでコレクションのメンバーを連結した文字列</returns>
        public static string ConcatWith<T>(this IEnumerable<T> source, string separator, string format, IFormatProvider formatProvider = null)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (format == null) throw new ArgumentNullException("format");
            if (formatProvider != null)
            {
                return source.Select(s => string.Format(formatProvider, format, s)).ConcatWith(separator);
            }
            return source.Select(s => string.Format(format, s)).ConcatWith(separator);
        }
        /// <summary>
        /// シーケンスが <see cref="null"/> もしくは要素が含まれていないかどうかを判断します。
        /// </summary>
        /// <typeparam name="T">コレクションのタイプ</typeparam>
        /// <param name="source">シーケンス</param>
        /// <returns>シーケンスが <see cref="null"/> もしくは要素が含まれていない場合は true , そうでない場合は false</returns>
        public static bool IsEmpty<T>(this IEnumerable<T> source)
        {
            return source == null || !source.Any();
        }
        /// <summary>
        /// 指定されたシーケンスが null だった場合に、空のシーケンスを返します。
        /// </summary>
        /// <typeparam name="T">コレクションのタイプ</typeparam>
        /// <param name="source">シーケンス</param>
        /// <returns>指定されたシーケンスが null だった場合は空のシーケンス、そうでない場合は指定されたシーケンスのインスタンス</returns>
        public static IEnumerable<T> ToSafe<T>(this IEnumerable<T> source)
        {
            return source == null ? Enumerable.Empty<T>() : source;
        }
    }
}