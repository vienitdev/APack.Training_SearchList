using Archpack.Training.ArchUnits.Routing.Owin.V1;
using Archpack.Training.ArchUnits.Routing.V1;
using Archpack.Training.ArchUnits.Path.V1;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Archpack.Training.ArchUnits.Routing.SignalR.V1;
using Archpack.Training.ArchUnits.Container.V1;

namespace Archpack.Training.ArchUnits.Routing.SignalR.Owin.V1
{
    public sealed class ServiceUnitSignalR : IMiddlewareRegisterSetting
    {
        void IMiddlewareRegisterSetting.Use(global::Owin.IAppBuilder builder)
        {
            ITypePath typePath = GlobalContainer.GetService<ITypePath>();
            List<TypePathItem> items = typePath.Items.Where(i => i.TargetType.IsSubclassOf(typeof(ServiceUnitPersistentConnection))).ToList();
            foreach(TypePathItem item in items)
            {
                PathMapResult pathconvert = PathMapper.Convert(
                    item.Path,
                    "/{ServiceUnitName}/{Version}/{Role}/Socket/{Name}Socket",
                    "/{ServiceUnitName}/{Version}/{Role}/socket/{Name}");
                if(pathconvert.Success)
                {
                    builder.MapSignalR(pathconvert.MappedPath, item.TargetType, new ConnectionConfiguration());
                }
            }
        }
    }


}