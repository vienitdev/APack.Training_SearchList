using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Archpack.Training.ArchUnits.WebApiModels.V1
{
    /// <summary>
    /// 検索APIの引数に利用するオブジェクトを定義します。
    /// </summary>
    /// <typeparam name="T">検索条件の型</typeparam>
    public class SearchRequest<T>
    {
        /// <summary>
        /// 検索条件を取得または設定します。
        /// </summary>
        public T Criteria { get; set; }
        /// <summary>
        /// 検索範囲を取得または設定します。
        /// </summary>
        public SearchRange Range { get; set; }
        /// <summary>
        /// ソート条件を取得または設定します。
        /// </summary>
        public SortDefinition[] Sort { get; set; }
    }
    /// <summary>
    /// 検索範囲を定義します。
    /// </summary>
    public class SearchRange
    {
        /// <summary>
        /// 結果の件数を取得または設定します。
        /// </summary>
        public int Top { get; set; }
        /// <summary>
        /// 結果の先頭位置を取得または設定します。
        /// </summary>
        public int Skip { get; set; }
    }
    /// <summary>
    /// ソート条件でNULLの値の並び順の定義を表します。
    /// </summary>
    public enum SortNullOption
    {
        /// <summary>
        /// 指定なしを表します。
        /// </summary>
        Nothing = 0,
        /// <summary>
        /// NULLの値を先頭にします。
        /// </summary>
        NullsFirst = 1,
        /// <summary>
        /// NULLの値を末尾にします。
        /// </summary>
        NullsLast = 2
    }
    /// <summary>
    /// ソート条件を定義します。
    /// </summary>
    public class SortDefinition
    {
        /// <summary>
        /// ソートする対象の項目を取得または設定します。
        /// </summary>
        public string Target { get; set; }
        /// <summary>
        /// 降順かどうかを取得または設定します。
        /// </summary>
        public bool Desc { get; set; }
        /// <summary>
        /// NULLの値の並び順を取得または設定します。
        /// </summary>
        public SortNullOption SortNullOption { get; set; }
    }
}