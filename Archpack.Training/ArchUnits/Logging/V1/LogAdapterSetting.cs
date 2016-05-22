namespace Archpack.Training.ArchUnits.Logging.V1
{
    /// <summary>
    /// ログ出力実装のインスタンスおよび設定情報を保持します。
    /// </summary>
    public class LogAdapterSetting
    {
        /// <summary>
        /// インスタンスを初期化します。
        /// </summary>
        public LogAdapterSetting()
        {
        }
        /// <summary>
        /// ログ出力を行う実装クラスの院スタンを取得または設定します。
        /// </summary>
        public ILogAdapter Adapter { get; set; }
        /// <summary>
        /// Urlを取得または設定します。
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 名前を取得または設定します。
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 出力レベルを取得または設定します。
        /// </summary>
        public string LogLevel { get; set; }

    }
}