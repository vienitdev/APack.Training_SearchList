using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Archpack.Training.ArchUnits.Routing.Owin.V1
{
    public interface IMiddlewareRegisterSetting
    {
        void Use(IAppBuilder bulder);
    }
}