using Archpack.Training.ArchUnits.Contracts.V1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Archpack.Training.ArchUnits.Container.V1
{
    public class GlobalContainer
    {
        private static IServiceContainer rootContianer = null;

        public static IServiceContainer AddInstance(object instance)
        {
            CheckInitialized();
            return rootContianer.AddInstance(instance);
        }

        public static IServiceContainer AddInstance(Type service, object instance)
        {
            CheckInitialized();
            return rootContianer.AddInstance(service, instance);
        }

        public static IServiceContainer AddSingleton(Type service)
        {
            CheckInitialized();
            return rootContianer.AddSingleton(service);
        }

        public static IServiceContainer AddSingleton(Type service, Type implementationType)
        {
            CheckInitialized();
            return rootContianer.AddSingleton(service, implementationType);
        }

        public static IServiceContainer AddTransient(Type service)
        {
            CheckInitialized();
            return rootContianer.AddTransient(service);
        }

        public static IServiceContainer AddTransient(Type service, Type implementationType)
        {
            CheckInitialized();
            return rootContianer.AddTransient(service, implementationType);
        }

        public static T GetService<T>()
        {
            CheckInitialized();
            return rootContianer.GetService<T>();
        }

        public static IEnumerable<T> GetServices<T>()
        {
            CheckInitialized();
            return rootContianer.GetServices<T>();
        }

        public static IServiceContainer CreateChild()
        {
            CheckInitialized();
            return rootContianer.CreateChild();
        }

        public static void Initialize(IServiceContainer container, bool force = false)
        {
            Contract.NotNull(container, "container");
            if (!force && rootContianer != null)
            {
                throw new InvalidOperationException("Already initialized");
            }

            rootContianer = container;
        }

        public static bool IsInitialzied()
        {
            return rootContianer != null;
        }

        private static void CheckInitialized()
        {
            if (!IsInitialzied())
            {
                throw new InvalidOperationException("Not initialized");
            }
        }

        public static void Release()
        {
            if(!IsInitialzied()){
                return;
            }
            rootContianer.Dispose();
            rootContianer = null;
        }
    }
}