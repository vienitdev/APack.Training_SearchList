using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Archpack.Training.ArchUnits.Routing.V1
{
    public interface IUriProcessResolver
    {
        Type GetExecutionType(ServiceUnitContext suContext);

        object CreateInstance(ServiceUnitContext suContext, Type instanceType);

        ActionDefinition GetAction(ServiceUnitContext suContext, object instance);

        ServiceUnitResponse InvokeAction(ServiceUnitContext suContext, ActionDefinition action);

        List<Func<ServiceUnitContext, ServiceUnitResponse, ServiceUnitResponse>> HandleErrorPipeline { get; }
    }
}