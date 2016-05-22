using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.DirectoryServices.ActiveDirectory;
using Archpack.Training.ArchUnits.Routing.WebApi.V1;
using Archpack.Training.ArchUnits.Contracts.V1;
using Archpack.Training.ArchUnits.Container.V1;
using Archpack.Training.ArchUnits.Environment.V1;

namespace Archpack.Training.ArchUnits.WebApiExtensions.V1
{

    public static class HttpRequestMessageExtensions
    {
        public static HttpContextBase GetHttpContext(this HttpRequestMessage request)
        {
            Contract.NotNull(request, "request");

            string key = "MS_HttpContext";
            if (!request.Properties.ContainsKey(key))
            {
                throw new InvalidOperationException("httpcontext is not available.");
            }

            HttpContextBase result = request.Properties[key] as HttpContextBase;

            return result;
        }

    }

    public static class FileHttpRequestMessageExtensions
    {
        static class MediaTypes
        {
            public const string OctetStream = "application/octet-stream";
            public const string ExcelXlsx = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        }

        private static readonly string RequestClientKeyName = Resources.RequestClientKeyName;
        private static readonly string ResponseHtmlFormat = Resources.ResponseHtmlFormat;


        /// <summary>
        /// 指定されたストリームをファイルとしてダウンロードするためのレスポンスを生成します。
        /// </summary>
        /// <param name="stream">コンテンツストリーム</param>
        /// <param name="fileName">ファイル名</param>
        /// <returns>レスポンス</returns>
        public static HttpResponseMessage CreateFileDownloadResponse(this HttpRequestMessage request, HttpStatusCode satusCode, Stream stream, string fileName, string mediaType = MediaTypes.OctetStream)
        {            
            HttpResponseMessage result = new HttpResponseMessage();
            result.StatusCode = satusCode;
            stream.Position = 0;
            result.Content = new StreamContent(stream);
            result.Content.Headers.ContentType = new MediaTypeHeaderValue(mediaType);
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            result.Content.Headers.ContentDisposition.FileName = HttpUtility.UrlEncode(fileName);

            result.Headers.CacheControl = new CacheControlHeaderValue { Private = true, MaxAge = TimeSpan.Zero };

            return result;
        }

        /// <summary>
        /// 指定されたストリームをファイルとしてブラウザに表示するためのレスポンスを生成します。
        /// </summary>
        /// <param name="stream">コンテンツストリーム</param>
        /// <param name="fileName">ファイル名</param>
        /// <returns>レスポンス</returns>
        public static HttpResponseMessage CreateFileResponse(this HttpRequestMessage request, HttpStatusCode satusCode, Stream stream, string fileName, string mediaType = MediaTypes.OctetStream)
        {
            HttpResponseMessage result = new HttpResponseMessage();
            result.StatusCode = satusCode;
            stream.Position = 0;
            result.Content = new StreamContent(stream);
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline");
            result.Content.Headers.ContentDisposition.FileName = HttpUtility.UrlEncode(fileName);
            result.Content.Headers.ContentType = new MediaTypeHeaderValue(mediaType);
            result.Headers.CacheControl = new CacheControlHeaderValue { Private = true, MaxAge = TimeSpan.Zero };

            return result;
        }

        public static HttpResponseMessage CreateFileDownloadErrorResponse(this HttpRequestMessage request, HttpStatusCode statusCode,string message, string detail)
        {
            HttpResponseMessage response = request.CreateResponse(statusCode);
            var env = GlobalContainer.GetService<IApplicationEnvironment>();
            var uri = env.ApplicationRoot + "/Shared/V1/Users/page/DownloadError?Message=" + message + "&Detail=" + detail;
            response.Headers.Location = new Uri(uri, UriKind.RelativeOrAbsolute);
            return response;
        }
        
        
        /// <summary>
        /// アップロードされるファイルからバイト配列のデータを読み込みます。
        /// </summary>
        /// <param name="file">アップロードされるファイル</param>
        /// <returns>読み込まれたバイト配列のデータ</returns>
        public static byte[] GetBytesFromMultipartFileData(this MultipartFileData file)
        {

            byte[] data = null;

            using (Stream stream = new FileStream(file.LocalFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    data = reader.ReadBytes((int)stream.Length);
                }
            }

            return data;
        }

        /// <summary>
        /// マルチパートでアップロードされたファイルをローカルの一時フォルダに格納します。
        /// </summary>
        /// <param name="request">アップロードでクライアントから送信された HttpRequestMessage </param>
        /// <param name="path">一時フォルダのパス</param>
        /// <returns>MultipartFormDataStreamProvider のインスタンス</returns>
        public static MultipartFormDataStreamProvider ReadAsMultiPart(this HttpRequestMessage request, string path)
        {

            if (!request.Content.IsMimeMultipartContent("form-data"))
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            MultipartFormDataStreamProvider streamProvider = new MultipartFormDataStreamProvider(path);

            Task.Factory.StartNew(() => streamProvider = request.Content.ReadAsMultipartAsync(streamProvider).Result,
                                  CancellationToken.None,
                                  TaskCreationOptions.LongRunning,
                                  TaskScheduler.Default)
                                  .Wait();

            return streamProvider;
        }


        /// <summary>
        /// 成功のレスポンスを作成します。ステータスは自動的に OK に設定されます。
        /// </summary>
        /// <param name="message">クライアントに返すメッセージ</param>
        /// <param name="data">クライアントに返す付属データ</param>
        /// <returns>レスポンス</returns>
        public static HttpResponseMessage CreateFileUploadResponse(this HttpRequestMessage request, string message, object data = null)
        {
            return CreateResponse(request, HttpStatusCode.OK, message, data);
        }


        /// <summary>
        /// 失敗のレスポンスを作成します。
        /// </summary>
        /// <param name="statusCode">HTTPステータス</param>
        /// <param name="message">クライアントに返すメッセージ</param>
        /// <param name="data">クライアントに返す付属データ</param>
        /// <returns>レスポンス</returns>
        public static HttpResponseMessage CreateFileErrorResponse(this HttpRequestMessage request, HttpStatusCode statusCode, string message, object data = null)
        {
            return CreateResponse(request, statusCode, message, data);
        }

        /// <summary>
        /// レスポンスを生成します。
        /// </summary>
        /// <param name="status">HTTPステータス</param>
        /// <param name="message">クライアントに返すメッセージ</param>
        /// <param name="data">クライアントに返す付属データ</param>
        /// <returns>レスポンス</returns>
        private static HttpResponseMessage CreateResponse(this HttpRequestMessage request, HttpStatusCode status, string message, object data)
        {
            HttpResponseMessage response = request.CreateResponse(status);
            HttpRequestBase httpRequest = request.GetHttpContext().Request;

            string key = httpRequest.Params[RequestClientKeyName];
            string targetOrigion = "*";

            if (httpRequest.UrlReferrer != null)
            {
                targetOrigion = string.Format("{0}{1}{2}", httpRequest.UrlReferrer.Scheme, Uri.SchemeDelimiter, httpRequest.UrlReferrer.Host);
                if (!httpRequest.UrlReferrer.IsDefaultPort)
                {
                    targetOrigion += ":" + httpRequest.UrlReferrer.Port.ToString();
                }
            }

            bool result = (HttpStatusCode.OK == status);

            Dictionary<string, object> resultData = new Dictionary<string, object>(){
                    {"key", key},
				    {"result", result},
				    {"message", message},
				    {"data", data}
			    };

            JsonSerializer serializer = new JsonSerializer();

            string postMessageJson = null;
            string dataJson = null;
            using (StringWriter writer = new StringWriter())
            {
                serializer.Serialize(writer, resultData);
                postMessageJson = HttpUtility.JavaScriptStringEncode(writer.ToString());
            }

            using (StringWriter writer = new StringWriter())
            {
                serializer.Serialize(writer, data);
                dataJson = writer.ToString();
            }

            response.Content = new StringContent(string.Format(ResponseHtmlFormat, postMessageJson, targetOrigion, key, result, ToNumericCharacterReference(message), dataJson), Encoding.UTF8, "text/html");

            return response;
        }

        /// <summary>
        /// 文字列を数値実体参照文字列に変換します。
        /// </summary>
        /// <param name="value">変換する文字列</param>
        /// <returns>変換された数値実体参照文字列</returns>
        private static string ToNumericCharacterReference(string value)
        {
            StringBuilder result = new StringBuilder();
            foreach (int v in value)
            {
                result.Append("&#x").Append(v.ToString("x")).Append(";");
            }

            return result.ToString();
        }
    }
}