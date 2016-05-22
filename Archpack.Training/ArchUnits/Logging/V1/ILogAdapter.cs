namespace Archpack.Training.ArchUnits.Logging.V1
{
    /// <summary>
    /// ログ出力する実装クラスが提供するインターフェイスを定義します。
    /// </summary>
    public interface ILogAdapter
    {
        /// <summary>
        /// Info レベルのログを出力します。
        /// </summary>
        /// <param name="data">出力するログ内容</param>
        void Info(LogData data);

        /// <summary>
        /// Debug レベルのログを出力します。
        /// </summary>
        /// <param name="data">出力するログ内容</param>
        void Debug(LogData data);

        /// <summary>
        /// Trace レベルのログを出力します。
        /// </summary>
        /// <param name="data">出力するログ内容</param>
        void Trace(LogData data);

        /// <summary>
        /// Warn レベルのログを出力します。
        /// </summary>
        /// <param name="data">出力するログ内容</param>
        void Warn(LogData data);

        /// <summary>
        /// Error レベルのログを出力します。
        /// </summary>
        /// <param name="data">出力するログ内容</param>
        void Error(LogData data);

        /// <summary>
        /// Fatal レベルのログを出力します。
        /// </summary>
        /// <param name="data">出力するログ内容</param>
        void Fatal(LogData data);
    }
}
