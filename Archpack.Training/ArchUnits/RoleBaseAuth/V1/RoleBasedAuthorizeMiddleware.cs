using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using Microsoft.Owin.Extensions;
using Microsoft.Owin;
using Owin;
using System.Threading.Tasks;
using System.Web.Http.Controllers;

namespace Archpack.Training.ArchUnits.RoleBaseAuth.V1
{
    public class RoleBasedAuthorizeMiddleware : OwinMiddleware
    {
        public RoleBasedAuthorizeMiddleware(OwinMiddleware next) : base(next) { }

        public async override Task Invoke(IOwinContext context)
        {            
            string user = context.Request.User.Identity.Name;
            string url = context.Request.Uri.AbsolutePath;
            
            RoleBasedAuthorization roleauthattr = new RoleBasedAuthorization();
            roleauthattr.Authorize(user, url);
            await Next.Invoke(context);
        }
    }
}