using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace Archpack.Training.ArchUnits.RoleBaseAuth.V2
{
    public class AuthorizationResult
    {
        public AuthorizationResult() {
            this.IsAuthorized = false;
            this.Status = HttpStatusCode.OK;
        }

        public bool IsAuthorized { get; set; }

        public HttpStatusCode Status { get; set; }

        //パーミッション
    }
}