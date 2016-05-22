using Newtonsoft.Json;

namespace Archpack.Training.ArchUnits.Configuration.V1
{
    /// <summary>
    /// エラー画面の定義を表します。
    /// </summary>
    public class ErrorPageConfiguration
    {
        /// <summary>
        /// 遷移方式を取得または設定します。
        /// </summary>
        [JsonProperty("redirectMode")]
        public string RedirectMode { get; set; }
        /// <summary>
        /// 遷移先のURLを取得または設定します。
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; set; }
        /// <summary>
        /// 遷移先で表示する戻り先のURLを取得または設定します。
        /// </summary>
        [JsonProperty("redirectUrl")]
        public string RedirectUrl { get; set; }


    }
}