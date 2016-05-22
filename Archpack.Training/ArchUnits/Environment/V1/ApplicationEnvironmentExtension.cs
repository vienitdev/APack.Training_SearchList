using System;

namespace Archpack.Training.ArchUnits.Environment.V1
{
    public static class ApplicationEnvironmentExtension
    {
        public static bool IsProduction(this IApplicationEnvironment self)
        {
            return Is(self, "production");
        }

        public static bool IsDevelopment(this IApplicationEnvironment self)
        {
            return Is(self, "development");
        }

        private static bool Is(IApplicationEnvironment self, string envName)
        {
            return self.EnvironmentName.Equals(envName, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}