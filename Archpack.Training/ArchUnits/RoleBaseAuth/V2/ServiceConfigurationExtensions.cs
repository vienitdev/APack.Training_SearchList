using Archpack.Training.ArchUnits.Configuration.V1;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Archpack.Training.ArchUnits.RoleBaseAuth.V2
{
    public static class ServiceConfigurationExtensions
    {
        private static ConcurrentDictionary<string, RoleBasedAuthorizationSetting> configurationPool = new ConcurrentDictionary<string, RoleBasedAuthorizationSetting>();

        public static RoleBasedAuthorizationSetting GetRoleBaseAuthorizationSetting(this ServiceConfiguration self)
        {
            var fullName = self.FullName;
            RoleBasedAuthorizationSetting result = null;
            if(configurationPool.TryGetValue(fullName, out result))
            {
                return result;
            }

            if (self.Raw.ContainsKey("roleBasedAuthorization"))
            {
                result = self.Raw["roleBasedAuthorization"].ToObject<RoleBasedAuthorizationSetting>();
            }
            else
            {
                result = new RoleBasedAuthorizationSetting(Enumerable.Empty<string>());
            }

            configurationPool.AddOrUpdate(fullName, result, (k, old) => result);
            return result;
        }

    }
}