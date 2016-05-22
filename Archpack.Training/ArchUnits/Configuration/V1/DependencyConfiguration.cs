using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Archpack.Training.ArchUnits.Configuration.V1
{

    /// <summary>
    /// サービスユニットの依存関係定義情報を格納します。
    /// </summary>
    [JsonConverter(typeof(DependencyConfigurationJsonCoverter))]
    public sealed class DependencyConfiguration
    {
        public DependencyConfiguration()
        {
            this.Api = new Dictionary<string, string>();
            this.Pages = new Dictionary<string, string>();
        }

        public string Key { get; set; }

        /// <summary>
        /// サービスユニットのバージョン情報を取得または設定します
        /// </summary>
        [JsonProperty("version", Required = Required.Always)]
        public string Version { get; set; }

        /// <summary>
        /// サービスユニットの URL を取得または設定します
        /// </summary>
        [JsonProperty("url", Required = Required.Always)]
        public string Url { get; set; }

        /// <summary>
        /// API の参照（名称、 URL ）を格納します。
        /// </summary>
        [JsonProperty("api")]
        public Dictionary<string, string> Api { get; set; }

        /// <summary>
        /// ページの参照（名称、 URL ）を格納します。
        /// </summary>
        [JsonProperty("pages")]
        public Dictionary<string, string> Pages { get; set; }
    }

    public sealed class DependencyConfigurationJsonCoverter : JsonConverter
    {

        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(DependencyConfiguration));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var root = JObject.Load(reader);
            var config = new DependencyConfiguration();

            config.Key = reader.Path;
            config.Version = GetPropValue(root, "version");
            config.Url = GetPropValue(root, "url");

            var pages = root["pages"] as JObject;
            if (pages != null && pages.HasValues)
            {
                config.Pages = pages.Properties().Aggregate(
                    new Dictionary<string, string>(),
                    (seed, p) =>
                    {
                        seed.Add(p.Name, UrlFormat(config, p));
                        return seed;
                    }
                );
            }

            var api = root["api"] as JObject;
            if (api != null && api.HasValues)
            {
                config.Api = api.Properties().Aggregate(
                    new Dictionary<string, string>(),
                    (seed, p) =>
                    {
                        seed.Add(p.Name, UrlFormat(config, p));
                        return seed;
                    }
                );
            }

            return config;
        }

        private static string UrlFormat(DependencyConfiguration config, JProperty p)
        {
            return string.Format("{0}/{1}/{2}/{3}", (config.Url == "/") ? string.Empty : config.Url, config.Key, config.Version, p.Value);
        }

        private string GetPropValue(JObject jobj, string propertyName)
        {
            var val = jobj[propertyName];
            if (val == null)
            {
                return null;
            }

            if (val.Type == JTokenType.String)
            {
                return val.Value<string>();
            }

            return null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (CanConvert(value.GetType()))
            {
                var config = value as DependencyConfiguration;
                var jobj = new JObject();
                jobj["version"] = config.Version;
                jobj["url"] = config.Url;

                jobj["pages"] = JObject.FromObject(config.Pages);
                jobj["api"] = JObject.FromObject(config.Api);

                jobj.WriteTo(writer);
            }
        }
    }
}