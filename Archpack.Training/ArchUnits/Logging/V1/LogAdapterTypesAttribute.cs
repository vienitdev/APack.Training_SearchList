using System;

namespace Archpack.Training.ArchUnits.Logging.V1
{
    /// <summary>
    /// <see cref="ILogAdapter"/> の実装クラスが出力可能なログタイプを定義します。
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class LogAdapterTypesAttribute : Attribute
    {
        private string[] types = new string[]{};

        /// <summary>
        /// 出力可能なログタイプを利用してインスタンスを初期化します。
        /// </summary>
        /// <param name="types">出力可能なログタイプ</param>
        public LogAdapterTypesAttribute(params string[] types)
        {
            this.types = types;
        }
        /// <summary>
        /// ログタイプを取得します。
        /// </summary>
        public string[] Types
        {
            get { return this.types; }
        }
    }
}