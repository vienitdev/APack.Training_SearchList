using Archpack.Training.ArchUnits.Routing.Owin.V1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Archpack.Training.ArchUnits.Routing.WebForm.Owin.V2
{
    public sealed class ServiceUnitForm : IMiddlewareRegisterSetting
    {
        void IMiddlewareRegisterSetting.Use(global::Owin.IAppBuilder bulder)
        {
            bulder.Use(typeof(ServiceUnitFormMiddleware));
        }
    }
}