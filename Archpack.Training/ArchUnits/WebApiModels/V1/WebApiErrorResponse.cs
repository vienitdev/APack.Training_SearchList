using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Archpack.Training.ArchUnits.WebApiModels.V1
{
    /// <summary>
    /// Web API で発生したエラー情報を格納するクラスです。
    /// </summary>
    [JsonObject]
    public partial class WebApiErrorResponse
    {
        /// <summary>
        /// デフォルト コンストラクタです。
        /// </summary>
        public WebApiErrorResponse()
        {
            this.Errors = new List<WebApiErrorDetail>();
        }

        /// <summary>
        /// エラー種別に対応するコードを返します。
        /// </summary>
        [JsonProperty(PropertyName = "errorType")]
        public string ErrorType { get; set; }

        /// <summary>
        /// エラーメッセージを格納します。
        /// </summary>
        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

        /// <summary>
        /// システムエラーなどの場合には例外の情報を文字列として格納します。
        /// </summary>
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        /// <summary>
        /// エラー情報の詳細を取得します。
        /// </summary>
        [JsonProperty(PropertyName = "errors")]
        public List<WebApiErrorDetail> Errors { get; private set; }

        
    }

    public class WebApiErrorResponse<T> : WebApiErrorResponse
    {
        /// <summary>
        /// 付帯情報を取得または設定します。
        /// </summary>
        [JsonProperty(PropertyName = "additionalInfo")]
        public T AdditionalInfo { get; set; }
    }

    /// <summary>
    /// エラー情報の詳細クラスです。
    /// </summary>
    [JsonObject]
    public class WebApiErrorDetail
    {
        [JsonProperty(PropertyName = "objectId")]
        /// <summary>
        /// エラーが発生したプロパティを保持するオブジェクトのプログラム上のIDを格納します。
        /// </summary>
        public string ObjectId { get; set; }

        /// <summary>
        /// エラーが発生したプロパティのプログラム上のIDを格納します。
        /// </summary>
        [JsonProperty(PropertyName = "propertyId")]
        public string PropertyId { get; set; }

        /// <summary>
        /// エラーが発生したプロパティのデータを格納します。
        /// </summary>
        [JsonProperty(PropertyName = "data")]
        public object Data { get; set; }

        /// <summary>
        /// エラーが発生したプロパティの表示名を格納します。
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// エラーが発生したプロパティの値を格納します。
        /// </summary>
        [JsonProperty(PropertyName = "value")]
        public object Value { get; set; }

        /// <summary>
        /// エラー内容を示すメッセージを表示します。
        /// </summary>
        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }
    }

    /// <summary>
    /// エラーの種別を定義します。
    /// </summary>
    public static class WebApiErrorTypes
    {
        public const string InputError = "input_error";
        public const string DatabaseError = "db_error";
        public const string AuthenticationError = "authentication_error";
        public const string AuthorizationError = "authorization_error";
        public const string ConflictError = "conflict_error";
        public const string SystemError = "system_error";
        public const string ApiError = "api_error";
    }

}