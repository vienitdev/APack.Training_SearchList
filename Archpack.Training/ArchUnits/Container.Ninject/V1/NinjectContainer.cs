using Archpack.Training.ArchUnits.Container.V1;
using Archpack.Training.ArchUnits.Contracts.V1;
using Ninject;
using Ninject.Extensions.ChildKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Archpack.Training.ArchUnits.Container.Ninject.V1
{
    public sealed class NinjectContainer : IServiceContainer
    {
        private IKernel sourceKernel = null;

        public NinjectContainer(IKernel kernel)
        {
            Contract.NotNull(kernel, "kernel");
            this.sourceKernel = kernel;
        }

        public NinjectContainer()
        {
            this.sourceKernel = new StandardKernel();
        }

        public IServiceContainer AddInstance(object instance)
        {
            Contract.NotNull(instance, "instance");
            sourceKernel.Bind(instance.GetType()).ToConstant(instance);
            return this;
        }

        public IServiceContainer AddInstance(Type service, object instance)
        {
            Contract.NotNull(service, "service");
            Contract.NotNull(instance, "instance");
            CheckAssignable(service, instance.GetType());
            sourceKernel.Bind(service).ToConstant(instance);
            return this;
        }

        public IServiceContainer AddSingleton(Type service)
        {
            Contract.NotNull(service, "service");
            sourceKernel.Bind(service).ToSelf().InSingletonScope();
            return this;
        }

        public IServiceContainer AddSingleton(Type service, Type implementationType)
        {
            Contract.NotNull(service, "service");
            Contract.NotNull(implementationType, "implementationType");
            CheckAssignable(service, implementationType);
            sourceKernel.Bind(service).To(implementationType).InSingletonScope();
            return this;
        }

        public IServiceContainer AddTransient(Type service)
        {
            Contract.NotNull(service, "service");
            sourceKernel.Bind(service).ToSelf().InTransientScope();
            return this;
        }

        public IServiceContainer AddTransient(Type service, Type implementationType)
        {
            Contract.NotNull(service, "service");
            Contract.NotNull(implementationType, "implementationType");
            CheckAssignable(service, implementationType);
            sourceKernel.Bind(service).To(implementationType).InTransientScope();
            return this;
        }

        public T GetService<T>()
        {
            if (sourceKernel.CanResolve<T>())
            {
                return sourceKernel.Get<T>();
            }
            return default(T);
        }

        public object GetService(Type service)
        {
            Contract.NotNull(service, "service");
            if ((bool)sourceKernel.CanResolve(service))
            {
                return sourceKernel.Get(service);
            }
            return null;
        }

        public T GetService<T>(Type service)
        {
            Contract.NotNull(service, "service");
            var result = this.GetService(service);
            if (result != null)
            {
                return (T)result;
            }
            return default(T);
        }


        public IEnumerable<T> GetServices<T>()
        {
            return sourceKernel.GetAll<T>();
        }


        public IEnumerable<object> GetServices(Type service)
        {
            Contract.NotNull(service, "service");
            return sourceKernel.GetAll(service);
        }

        public IEnumerable<T> GetServices<T>(Type service)
        {
            Contract.NotNull(service, "service");
            return this.GetServices(service).Cast<T>();
        }


        public IServiceContainer CreateChild()
        {
            return new NinjectContainer(new ChildKernel(sourceKernel));
        }

        private void CheckAssignable(Type sourceType, Type implementationType)
        {
            if (!sourceType.IsAssignableFrom(implementationType))
            {
                throw new InvalidOperationException();
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // 重複する呼び出しを検出するには

        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (sourceKernel != null)
                    {
                        sourceKernel.Dispose();
                    }
                }
                disposedValue = true;
            }
        }

        void IDisposable.Dispose()
        {
            Dispose(true);
        }

        #endregion
       
    }
}