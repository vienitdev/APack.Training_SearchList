using Archpack.Training.ArchUnits.Collections.V1;
using Archpack.Training.ArchUnits.Configuration.V1;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Archpack.Training.ArchUnits.RoleBaseAuth.V2
{
    public class RoleBasedAuthorizationSetting
    {
        public RoleBasedAuthorizationSetting(IEnumerable<string> ignoreUrls)
        {
            this.IgnoreUrls = ignoreUrls.ToSafe().AsEnumerable();
        }

        public IEnumerable<string> IgnoreUrls { get; private set; }
    }

    public sealed class RoleBasedAuthorizationSettingJsonConverter : JsonConverter
    {

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(RoleBasedAuthorizationSetting);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var root = JObject.Load(reader);
            var items = new List<ConnectionStringItem>();

            var ignoreUrlsJson = root["ignoreUrls"] as JProperty;
            if(ignoreUrlsJson == null || !(ignoreUrlsJson.Value is JArray))
            {
                return new RoleBasedAuthorizationSetting(Enumerable.Empty<string>());
            }

            var ignores = new List<string>();
            foreach(var token in (ignoreUrlsJson.Value as JArray).Values())
            {
                if(token.Type == JTokenType.String)
                {
                    var url = token.Value<string>();
                    if (!url.StartsWith("/"))
                    {
                        url = "/" + url;
                    }
                    ignores.Add(url);
                }
            }
            return new RoleBasedAuthorizationSetting(ignores);
        }
        
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }

    public static class RoleBaseAuthorizationSettingExtensions
    {
        public static bool IsIgnoreUrl(this RoleBasedAuthorizationSetting self, string url)
        {
            var target = url ?? "";
            return self.IgnoreUrls.Any(ig => url.StartsWith(ig, true, null));
        }
    }
}