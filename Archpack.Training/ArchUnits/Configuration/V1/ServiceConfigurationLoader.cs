using Archpack.Training.ArchUnits.Container.V1;
using Archpack.Training.ArchUnits.Environment.V1;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Archpack.Training.ArchUnits.Configuration.V1
{
    public static class ServiceConfigurationLoader
    {
        private static readonly string ConfigFileName = "service-config.json";
        private static readonly string EnvConfigFileNameFormat = "service-config.{0}.json";
        private static readonly string ServiceUnitDir = "~/ServiceUnits/";

        private static ConcurrentDictionary<string, ServiceConfiguration> configurationPool = new ConcurrentDictionary<string, ServiceConfiguration>();
        private static ConcurrentDictionary<string, JToken> treeConfigurationPool = new ConcurrentDictionary<string, JToken>();

        public static void ClearCache()
        {
            configurationPool = new ConcurrentDictionary<string, ServiceConfiguration>();
            treeConfigurationPool = new ConcurrentDictionary<string, JToken>();
        }

        public static ServiceConfiguration Load()
        {
            ServiceConfiguration config = null;

            var key = CreateKey(null, null, null);

            if (configurationPool.TryGetValue(key, out config))
            {
                return config;
            }
            var env = GlobalContainer.GetService<IApplicationEnvironment>().EnvironmentName;
            var dic = JObject.Parse("{}").Extend(LoadRoot(env), LoadServiceUnitRoot(env)).ToObject<Dictionary<string, JToken>>();

            config = new ServiceConfiguration(dic, "");

            configurationPool.AddOrUpdate(key, config, (k, old) => config);

            return config;
        }

        public static ServiceConfiguration Load(string serviceUnitName, string version)
        {
            ServiceConfiguration config;
            var key = CreateKey(serviceUnitName, version, null);
            if (configurationPool.TryGetValue(key, out config))
            {
                return config;
            }
            var env = GlobalContainer.GetService<IApplicationEnvironment>().EnvironmentName;
            var dic = JObject.Parse("{}").Extend(
                LoadRoot(env),
                LoadServiceUnitRoot(env),
                LoadServiceUnit(serviceUnitName, env),
                LoadVersion(serviceUnitName, version, env)).ToObject<Dictionary<string, JToken>>();

            config = new ServiceConfiguration(dic, serviceUnitName, version);

            configurationPool.AddOrUpdate(key, config, (k, old) => config);
            return config;
        }

        public static ServiceConfiguration Load(string serviceUnitName, string version, string role)
        {
            ServiceConfiguration config;
            var key = CreateKey(serviceUnitName, version, role);
            if (configurationPool.TryGetValue(key, out config))
            {
                return config;
            }
            var env = GlobalContainer.GetService<IApplicationEnvironment>().EnvironmentName;
            var dic = JObject.Parse("{}").Extend(
                LoadRoot(env),
                LoadServiceUnitRoot(env),
                LoadServiceUnit(serviceUnitName, env),
                LoadVersion(serviceUnitName, version, env),
                LoadRole(serviceUnitName, version, role, env)).ToObject<Dictionary<string, JToken>>();

            config = new ServiceConfiguration(dic, serviceUnitName, version, role, env);

            configurationPool.AddOrUpdate(key, config, (k, old) => config);
            return config;
        }

        private static string CreateKey(string serviceUnitName, string version, string role)
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

        private static JObject LoadRoot(string env, bool createEmpty = true)
        {
            return LoadConfig("~", env, createEmpty);
        }

        private static JObject LoadServiceUnitRoot(string env, bool createEmpty = true)
        {
            return LoadConfig(ServiceUnitDir, env, createEmpty);
        }

        private static JObject LoadServiceUnit(string serviceUnitName, string env, bool createEmpty = true)
        {
            if (string.IsNullOrWhiteSpace(serviceUnitName))
            {
                return JObject.Parse("{}");
            }
            return LoadConfig(ServiceUnitDir + serviceUnitName, env, createEmpty);
        }

        private static JObject LoadVersion(string serviceUnitName, string version, string env, bool createEmpty = true)
        {
            if (string.IsNullOrWhiteSpace(serviceUnitName) || string.IsNullOrWhiteSpace(version))
            {
                return JObject.Parse("{}");
            }
            return LoadConfig(ServiceUnitDir + serviceUnitName + "/" + version, env, createEmpty);
        }

        private static JObject LoadRole(string serviceUnitName, string version, string role, string env, bool createEmpty = true)
        {
            if (string.IsNullOrWhiteSpace(serviceUnitName) || string.IsNullOrWhiteSpace(version) || string.IsNullOrWhiteSpace(role))
            {
                return JObject.Parse("{}");
            }
            return LoadConfig(ServiceUnitDir + serviceUnitName + "/" + version + "/" + role, env, createEmpty);
        }

        private static JObject LoadConfig(string path, string env, bool createEmpty = true)
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

        private static JObject LoadConfig(string path, bool createEmpty = true)
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

        private static JObject Extend(this JObject source, params JObject[] targets)
        {
            var setting = new JsonMergeSettings() { MergeArrayHandling = MergeArrayHandling.Union };
            foreach (var target in targets)
            {
                source.Merge(target, setting);
            }
            return source;
        }


        public static JToken LoadTreeJson()
        {
            return LoadTreeJson(new string[] { });
        }

        public static JToken LoadTreeJson(string[] roles)
        {
            var cacheKey = string.Join(",", roles);
            JToken result;
            if (treeConfigurationPool.TryGetValue(cacheKey, out result))
            {
                return result;
            }
            var environment = GlobalContainer.GetService<IApplicationEnvironment>();
            var rootDir = new DirectoryInfo(environment.MapPath(ServiceUnitDir));
            var env = environment.EnvironmentName;
            var rootConfig = ServiceConfigurationLoader.Load();
            
            var rootJson = LoadConfig(ServiceUnitDir, env, false);
            if (rootJson == null)
            {
                result = JObject.Parse("{}");
                treeConfigurationPool.AddOrUpdate(cacheKey, result, (k, old) => result);
                return result;
            }
            AddItemsProperty(rootJson);

            //service unit
            foreach (var suDir in rootDir.GetDirectories())
            {
                var suJson = LoadServiceUnit(suDir.Name, env, false);
                if (suJson == null)
                {
                    continue;
                }
                AddItemsProperty(suJson);
                AddServicePath(((JArray)suJson["items"]), suDir.Name);
                AddServicePath(suJson);
                ((JArray)rootJson["items"]).Add(suJson);


                //version
                foreach (var vDir in suDir.GetDirectories())
                {
                    var vJson = LoadVersion(suDir.Name, vDir.Name, env, false);
                    if (vJson == null)
                    {
                        continue;
                    }
                    AddItemsProperty(vJson);
                    AddServicePath(((JArray)vJson["items"]), suDir.Name, vDir.Name);
                    AddServicePath(vJson, suDir.Name);
                    ((JArray)suJson["items"]).Add(vJson);

                    //role
                    foreach (var roleDir in vDir.GetDirectories())
                    {
                        if(!rootConfig.AvailableRoles.Any(r => r == roleDir.Name))
                        {
                            continue;
                        }
                        var roleJson = LoadRole(suDir.Name, vDir.Name, roleDir.Name, env, false);
                        if (roleJson == null)
                        {
                            continue;
                        }

                        if (roles.Length != 0 && !roles.Contains(roleDir.Name))
                        {
                            continue;
                        }
                        AddItemsProperty(roleJson);
                        AddServicePath(((JArray)roleJson["items"]), suDir.Name, vDir.Name, roleDir.Name);
                        AddServicePath(roleJson, suDir.Name, vDir.Name);
                        ((JArray)vJson["items"]).Add(roleJson);
                    }
                }
            }
            treeConfigurationPool.AddOrUpdate(cacheKey, rootJson, (k, old) => rootJson);
            return rootJson;
        }

        private static void AddItemsProperty(JObject target)
        {
            var items = target["items"];
            if (items == null)
            {
                items = new JArray();
                target.Add("items", items);
            }
        }

        private static void AddServicePath(JObject item, string serviceUnitName = null, string version = null, string role = null)
        {
            var path = "/";
            if (!string.IsNullOrEmpty(serviceUnitName))
            {
                path = path + serviceUnitName;
                if (!string.IsNullOrEmpty(version))
                {
                    path = path + "/" + version;
                    if (!string.IsNullOrEmpty(role))
                    {
                        path = path + "/" + role;
                    }
                }
            }
            item.Add("path", path);
        }

        private static void AddServicePath(JArray items, string serviceUnitName = null, string version = null, string role = null)
        {
            foreach (var token in items)
            {
                var item = token as JObject;
                if (item == null)
                {
                    continue;
                }
                AddServicePath(item, serviceUnitName, version, role);
            }
        }
    }
}