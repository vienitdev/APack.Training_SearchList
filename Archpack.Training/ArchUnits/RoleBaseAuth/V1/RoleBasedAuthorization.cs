using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Archpack.Training.ArchUnits.RoleBaseAuth.V1.Data;
using System.Data.Entity;

namespace Archpack.Training.ArchUnits.RoleBaseAuth.V1
{
    public class RoleBasedAuthorization
    {
        public UserRolePermission Authorize(string UserID, string Url)
        {
            if(string.IsNullOrEmpty(UserID) || string.IsNullOrEmpty(Url))
            {
                throw new ArgumentException("User Or Url is Null or Empty!");
            }
            AuthorizationEntities context = AuthorizationEntities.CreateContext();
            UserRolePermission result = (from rp in context.RolePermissions
                                         join r in context.Roles on rp.RoleID equals r.RoleID
                                         join p in context.Permissions on rp.PermissionID equals p.PermissionID
                                         join u in context.UserRoles on r.RoleID equals u.RoleID
                                         where u.UserID == UserID
                                         && p.URL == Url
                                         select new UserRolePermission
                                         {
                                             UserID = u.UserID,
                                             RoleID = r.RoleID,
                                             RoleName = r.RoleName,
                                             PermissionID = p.PermissionID,
                                             PermissionName = p.PermissionName,
                                             URL = p.URL
                                         }).FirstOrDefault();
            return result;
        }
    }
}