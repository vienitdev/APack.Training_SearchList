using Archpack.Training.ArchUnits.Configuration.V1;
using Archpack.Training.ArchUnits.Container.V1;
using Archpack.Training.ArchUnits.Environment.V1;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Archpack.Training.ArchUnits.Configuration.V2
{
    /// <summary>
    /// 外部から特殊設定情報を指定できるコンフィグレーションローダーを定義します。
    /// </summary>
    public class ServiceConfigurationLoader
    {
        private static readonly string ConfigFileName = "service-config.json";
        private static readonly string EnvConfigFileNameFormat = "service-config.{0}.json";
        private static readonly string ServiceUnitDir = "~/ServiceUnits/";

        private static ConcurrentDictionary<string, ServiceConfiguration> configurationPool = new ConcurrentDictionary<string, ServiceConfiguration>();
        private static ConcurrentDictionary<string, JToken> treeConfigurationPool = new ConcurrentDictionary<string, JToken>();

        private LoaderSettings settings;

        public ServiceConfigurationLoader(): this(LoaderSettings.Default)
        {

        }

        public ServiceConfigurationLoader(LoaderSettings settings)
        {
            if(settings == null)
            {
                throw new ArgumentNullException("settings");
            }
            this.settings = settings;
        }

        public ServiceConfiguration Load()
        {
            ServiceConfiguration config = null;

            var key = CreateKey(null, null, null);

            if (configurationPool.TryGetValue(key, out config))
            {
                return config;
            }
            var env = this.settings.EnvironmentName;
            var dic = Extend(JObject.Parse("{}"), LoadRoot(env), LoadServiceUnitRoot(env)).ToObject<Dictionary<string, JToken>>();

            config = new ServiceConfiguration(dic, "");

            configurationPool.AddOrUpdate(key, config, (k, old) => config);

            return config;
        }

        public ServiceConfiguration Load(string serviceUnitName, string version)
        {
            ServiceConfiguration config;
            var key = CreateKey(serviceUnitName, version, null);
            if (configurationPool.TryGetValue(key, out config))
            {
                return config;
            }
            var env = this.settings.EnvironmentName;
            var dic = Extend(JObject.Parse("{}"),
                LoadRoot(env),
                LoadServiceUnitRoot(env),
                LoadServiceUnit(serviceUnitName, env),
                LoadVersion(serviceUnitName, version, env)).ToObject<Dictionary<string, JToken>>();

            config = new ServiceConfiguration(dic, serviceUnitName, version);

            configurationPool.AddOrUpdate(key, config, (k, old) => config);
            return config;
        }

        public ServiceConfiguration Load(string serviceUnitName, string version, string role)
        {
            ServiceConfiguration config;
            var key = CreateKey(serviceUnitName, version, role);
            if (configurationPool.TryGetValue(key, out config))
            {
                return config;
            }
            var env = this.settings.EnvironmentName;
            var dic = Extend(JObject.Parse("{}"),
                LoadRoot(env),
                LoadServiceUnitRoot(env),
                LoadServiceUnit(serviceUnitName, env),
                LoadVersion(serviceUnitName, version, env),
                LoadRole(serviceUnitName, version, role, env)).ToObject<Dictionary<string, JToken>>();

            config = new ServiceConfiguration(dic, serviceUnitName, version, role, env);

            configurationPool.AddOrUpdate(key, config, (k, old) => config);
            return config;
        }

        private string CreateKey(string serviceUnitName, string version, string role)
        {
            if (string.IsNullOrWhiteSpace(serviceUnitName))
            {
                return string.Empty;
            }
            if (string.IsNullOrWhiteSpace(version))
            {
                return serviceUnitName;
            }
            if (string.IsNullOrWhiteSpace(role))
            {
                return serviceUnitName + "." + version;
            }
            return serviceUnitName + "." + version + "." + role;
        }

        private JObject LoadRoot(string env, bool createEmpty = true)
        {
            return LoadConfig("~", env, createEmpty);
        }

        private JObject LoadServiceUnitRoot(string env, bool createEmpty = true)
        {
            return LoadConfig(ServiceUnitDir, env, createEmpty);
        }

        private JObject LoadServiceUnit(string serviceUnitName, string env, bool createEmpty = true)
        {
            if (string.IsNullOrWhiteSpace(serviceUnitName))
            {
                return JObject.Parse("{}");
            }
            return LoadConfig(ServiceUnitDir + serviceUnitName, env, createEmpty);
        }

        private JObject LoadVersion(string serviceUnitName, string version, string env, bool createEmpty = true)
        {
            if (string.IsNullOrWhiteSpace(serviceUnitName) || string.IsNullOrWhiteSpace(version))
            {
                return JObject.Parse("{}");
            }
            return LoadConfig(ServiceUnitDir + serviceUnitName + "/" + version, env, createEmpty);
        }

        private JObject LoadRole(string serviceUnitName, string version, string role, string env, bool createEmpty = true)
        {
            if (string.IsNullOrWhiteSpace(serviceUnitName) || string.IsNullOrWhiteSpace(version) || string.IsNullOrWhiteSpace(role))
            {
                return JObject.Parse("{}");
            }
            return LoadConfig(ServiceUnitDir + serviceUnitName + "/" + version + "/" + role, env, createEmpty);
        }

        private JObject LoadConfig(string path, string env, bool createEmpty = true)
        {
            //HostingEnvironment
            var dir = GlobalContainer.GetService<IApplicationEnvironment>().MapPath(path);
            JObject envConfig = null;
            if (!string.IsNullOrEmpty(env))
            {
                var configFileName = System.IO.Path.Combine(dir, string.Format(EnvConfigFileNameFormat, env));
                if (File.Exists(configFileName))
                {
                    envConfig = JObject.Parse(File.ReadAllText(configFileName));
                }
                else
                {
                    if (createEmpty)
                    {
                        envConfig = JObject.Parse("{}");
                    }
                }
            }
            var config = LoadConfig(path, createEmpty);
            if(envConfig != null && config != null)
            {
                config.Merge(envConfig);
            }
            if(config == null && envConfig != null)
            {
                config = envConfig;
            }
            
            return config;
        }

        private JObject LoadConfig(string path, bool createEmpty = true)
        {
            //HostingEnvironment
            var dir = GlobalContainer.GetService<IApplicationEnvironment>().MapPath(path);
            var configFileName = System.IO.Path.Combine(dir, ConfigFileName);
            if (File.Exists(configFileName))
            {
                return JObject.Parse(File.ReadAllText(configFileName));
            }
            if (createEmpty)
            {
                return JObject.Parse("{}");
            }
            return null;
        }

        private JObject Extend(JObject source, params JObject[] targets)
        {
            var setting = new JsonMergeSettings() { MergeArrayHandling = MergeArrayHandling.Union };
            foreach (var target in targets)
            {
                source.Merge(target, setting);
            }
            return source;
        }
    }
}