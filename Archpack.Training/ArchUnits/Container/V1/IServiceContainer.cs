using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Archpack.Training.ArchUnits.Container.V1
{
    public interface IServiceContainer: IServiceProvider, IDisposable
    {
        IServiceContainer AddTransient(Type service);

        IServiceContainer AddTransient(Type service, Type implementationType);

        IServiceContainer AddSingleton(Type service);

        IServiceContainer AddSingleton(Type service, Type implementationType);

        IServiceContainer AddInstance(object instance);

        IServiceContainer AddInstance(Type service, object instance);

        IServiceContainer CreateChild();
    }
}