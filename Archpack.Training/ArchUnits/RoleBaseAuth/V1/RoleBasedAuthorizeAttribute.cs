using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Archpack.Training.ArchUnits.RoleBaseAuth.V1.Data;
using Archpack.Training.ArchUnits.RoleBaseAuth.V1;
using System.Web.Http.Controllers;
using System.Web.Http;

namespace Archpack.Training.ArchUnits.RoleBaseAuth.V1
{
    public class RoleBasedAuthorizeAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            string url = actionContext.Request.RequestUri.AbsolutePath;
            string userid = actionContext.RequestContext.Principal.Identity.Name;
            RoleBasedAuthorization roleauth = new RoleBasedAuthorization();
            UserRolePermission result = roleauth.Authorize(userid, url);
            if(result == null)
            {
                throw new Exception("Not Allow to Access!");
            }
        }
    }
}