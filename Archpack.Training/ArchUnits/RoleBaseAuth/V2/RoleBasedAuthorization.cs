using Archpack.Training.ArchUnits.Container.V1;
using Archpack.Training.ArchUnits.Contracts.V1;
using Archpack.Training.ArchUnits.Environment.V1;
using Archpack.Training.ArchUnits.RoleBaseAuth.V2.Data;
using Archpack.Training.ArchUnits.Routing.V1;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Archpack.Training.ArchUnits.RoleBaseAuth.V2
{
    public class RoleBasedAuthorization : Archpack.Training.ArchUnits.Routing.V2.IRoutingAuthorization
    {
        ServiceUnitContext context;
        RoleManager roleManager;
        PermissionManager permissionManager;
        TargetManager targetManager;
        UserRoleManager userRoleManager;

        public RoleBasedAuthorization(ServiceUnitContext context)
        {
            Contract.NotNull(context, "context");
            this.context = context;
            roleManager = new RoleManager();
            permissionManager = new PermissionManager();
            targetManager = new TargetManager();
            userRoleManager = new UserRoleManager();
        }

        public AuthorizationResult Authorize()
        {
            AuthorizationResult result = new AuthorizationResult();

            //チェック対象のURLかどうかを判断
            var requestPath = context.Request.Path;
            if (string.IsNullOrEmpty(requestPath))
            {
                return result;
            }
            var setting = context.Configuration.GetRoleBaseAuthorizationSetting();
            if (setting.IsIgnoreUrl(requestPath))
            {
                context.ServiceContainer.AddInstance(
                            new PermissionResult()
                            {
                                PermitFlag = true,
                                PermitProcTypes = new List<PermitProcType>() {
                                    PermitProcType.Approve,
                                    PermitProcType.Reference,
                                    PermitProcType.Update
                                }
                            }
                        );
                result.IsAuthorized = true;
                return result;
            }

            //認証されたユーザーに紐付くロールの検証            
            var roles = userRoleManager.GetRolesByUserId(GetEmployeeID());
            if (roles.Count == 0)
            {
                result.Status = System.Net.HttpStatusCode.Unauthorized;
                return result;
            }

            //ロールに紐づくターゲットのアクセス権の検証            
            var target = targetManager.GetTargets(context.Request.Path, ContentTypes.Url);

            if (target == null)
            {
                result.Status = System.Net.HttpStatusCode.Forbidden;
                return result;
            }

            var permissionResults = permissionManager.GetPermissions(roles.Select(x => x.RoleID).ToList(), target.TargetID);
            if (!permissionResults.PermitFlag)
            {
                result.Status = System.Net.HttpStatusCode.Forbidden;
                return result;
            }

            if (permissionResults.PermitProcTypes.ToList()[0] == PermitProcType.None)
            {
                result.Status = System.Net.HttpStatusCode.Forbidden;
                return result;
            }

            //ロールに紐づくパーミッション情報のサービスユニットコンテキストへの格納
            //TODO: ServiceContainer に　AddInstance(パーミッション情報)
            context.ServiceContainer.AddInstance(permissionResults);
            result.IsAuthorized = true;
            return result;
        }

        private int GetEmployeeID()
        {
            return (int)(new UserManager().GetUser(HttpContext.Current.User.Identity).EmployeeID);
        }
    }
}