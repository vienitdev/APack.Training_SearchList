using System.IO;
using Newtonsoft.Json.Linq;

namespace Archpack.Training.ArchUnits.Environment.V1
{
    public class EnvironmentSelector
    {
        private const string EnvironmentFileName = "service-config.json";
        private const string DefaultEnvironment = "";
        public string GetEnvironment(string rootDir)
        {
            var fileName = System.IO.Path.Combine(rootDir, EnvironmentFileName);
            if (File.Exists(fileName)) {
                var config = JObject.Parse(File.ReadAllText(fileName));
                var envConfig = config.Property("environment");
                if (envConfig == null)
                {
                    return DefaultEnvironment;
                }
                if(envConfig.Value.Type != JTokenType.String)
                {
                    return DefaultEnvironment;
                }
                return envConfig.Value.Value<string>();
            }
            return DefaultEnvironment;
        }
    }
}