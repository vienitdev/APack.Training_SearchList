using Archpack.Training.ArchUnits.Routing.V1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Archpack.Training.ArchUnits.Routing.WebForm.V1
{
    public static class ServiceUnitContextExtension
    {
        private const string ConfigKey = "availablePageMode";
        private const string PageTestModeKey = "Mode.Page.Test";

        public static bool IsTestMode(this ServiceUnitContext context)
        {
            if (context == null)
            {
                context.Properties[PageTestModeKey] = false;
                return false;
            }

            var result = false;
            if (context.Properties.ContainsKey(PageTestModeKey))
            {
                return (bool)context.Properties[PageTestModeKey];
            }
            if (CanTestMode(context) && IsNeedTestMode(context))
            {
                result = true;
            }
            context.Properties[PageTestModeKey] = result;
            return result;
            
        }

        private static bool IsNeedTestMode(ServiceUnitContext context)
        {
            var specifics = context.Request.SpecificProcessPath;
            if (string.IsNullOrWhiteSpace(specifics))
            {
                return false;
            }
            return "/TEST".Equals(specifics, StringComparison.InvariantCultureIgnoreCase);
        }

        private static bool CanTestMode(ServiceUnitContext context)
        {
            var config = context.Configuration;
            if (!config.Items.ContainsKey(ConfigKey) || !(config.Items[ConfigKey] is IEnumerable<string>))
            {
                if (!config.Raw.Keys.Contains(ConfigKey))
                {
                    return false;
                }
                var modes = config.Raw[ConfigKey].ToObject<string[]>();
                config.Items[ConfigKey] = modes;
            }
            return ((IEnumerable<string>)config.Items[ConfigKey]).Contains("test", StringComparer.InvariantCultureIgnoreCase);
        }

    }
}