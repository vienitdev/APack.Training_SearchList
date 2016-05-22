using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Archpack.Training.ArchUnits.Configuration.V1
{
    /// <summary>
    /// キー指定された接続文字列定義
    /// </summary>
    [JsonConverter(typeof(ConnectionStringConfigurationJsonConverter))]
    public sealed class ConnectionStringConfiguration
    {
        private List<ConnectionStringItem> items = null;

        /// <summary>
        /// 接続文字列の定義を指定してインスタンスを初期化します。
        /// </summary>
        /// <param name="items">接続文字列の定義</param>
        public ConnectionStringConfiguration(IEnumerable<ConnectionStringItem> items)
        {
            this.items = items == null ? new List<ConnectionStringItem>() : items.ToList();
        }
        /// <summary>
        /// 接続文字列の定義の一覧を取得します。
        /// </summary>
        public IEnumerable<ConnectionStringItem> Items { get { return this.items; } }

        /// <summary>
        /// 指定されたキーと一致する接続文字列の定義を取得思案す。
        /// </summary>
        /// <param name="key">接続文字列のキー</param>
        /// <returns>接続文字列の定義</returns>
        public ConnectionStringItem this[string key]
        {
            get
            {
                return this.items.FirstOrDefault(i => i.Key == key);
            }
        }
    }

    /// <summary>
    /// 接続文字列の定義
    /// </summary>
    public sealed class ConnectionStringItem
    {
        /// <summary>
        /// キーと接続文字列を指定してインスタンスを初期化します。
        /// </summary>
        /// <param name="key">接続文字列のキー</param>
        /// <param name="connectionString">接続文字列</param>
        public ConnectionStringItem(string key, string connectionString)
        {
            this.Key = key;
            this.ConnectionString = connectionString;
        }
        /// <summary>
        /// キーと接続文字列およびプロバイダー名を指定してインスタンスを初期化します。
        /// </summary>
        /// <param name="key">接続文字列のキー</param>
        /// <param name="connectionString">接続文字列</param>
        /// <param name="providerName">プロバイダー名</param>
        public ConnectionStringItem(string key, string connectionString, string providerName, string defaultScheme)
        {
            this.Key = key;
            this.ConnectionString = connectionString;
            this.ProviderName = providerName;
            this.DefaultScheme = defaultScheme;
        }
        /// <summary>
        /// 接続文字列のキーを取得します。
        /// </summary>
        public string Key { get; private set; }
        /// <summary>
        /// プロバイダー名を取得します。
        /// </summary>
        public string ProviderName { get; private set; }
        /// <summary>
        /// 接続文字列を取得します。
        /// </summary>
        public string ConnectionString { get; private set; }
        /// <summary>
        /// スキーマ名を取得します。
        /// </summary>
        public string DefaultScheme { get; private set; }
    }

    public sealed class ConnectionStringConfigurationJsonConverter : JsonConverter
    {

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(ConnectionStringConfiguration);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var root = JObject.Load(reader);
            var items = new List<ConnectionStringItem>();
            foreach (var prop in root.Properties())
            {
                var name = prop.Name;
                var value = prop.Value;
                if (value.Type == JTokenType.String)
                {
                    items.Add(new ConnectionStringItem(name, value.Value<string>()));
                }
                else if (value.Type == JTokenType.Object)
                {
                    var ovalue = (JObject)value;
                    var connectionString = GetPropValue(ovalue, "connectionString");
                    var providerName = GetPropValue(ovalue, "providerName");
                    var defaultScheme = GetPropValue(ovalue, "defaultScheme");
                    if (!string.IsNullOrEmpty(connectionString))
                    {
                        items.Add(new ConnectionStringItem(name, connectionString, providerName, defaultScheme));
                    }
                }
            }
            return new ConnectionStringConfiguration(items);
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
            throw new NotImplementedException();
        }
    }
}