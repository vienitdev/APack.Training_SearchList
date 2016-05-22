using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Archpack.Training.ArchUnits.RoleBaseAuth.V1.Data;
using System.Data.Entity;
using Archpack.Training.ArchUnits.Contracts.V1;

namespace Archpack.Training.ArchUnits.RoleBaseAuth.V1
{
    public class RoleManager
    {
        public List<UserRolePermission> GetRoles(string UserID)
        {
            using (var context = AuthorizationEntities.CreateContext())
            {
                List<UserRolePermission> result = (from rp in context.RolePermissions
                                                   join r in context.Roles on rp.RoleID equals r.RoleID
                                                   join p in context.Permissions on rp.PermissionID equals p.PermissionID
                                                   join u in context.UserRoles on r.RoleID equals u.RoleID
                                                   where u.UserID == UserID
                                                   select new UserRolePermission
                                                   {
                                                       UserID = u.UserID,
                                                       RoleID = r.RoleID,
                                                       RoleName = r.RoleName,
                                                       PermissionID = p.PermissionID,
                                                       PermissionName = p.PermissionName,
                                                       URL = p.URL
                                                   })
                                        .ToList<UserRolePermission>();
                return result;
            }
        }

        public List<UserRolePermission> GetRoles()
        {
            using (var context = AuthorizationEntities.CreateContext())
            {
                List<UserRolePermission> result = (from rp in context.RolePermissions
                                                   join r in context.Roles on rp.RoleID equals r.RoleID
                                                   join p in context.Permissions on rp.PermissionID equals p.PermissionID
                                                   select new UserRolePermission
                                                   {
                                                       UserID = null,
                                                       RoleID = r.RoleID,
                                                       RoleName = r.RoleName,
                                                       PermissionID = p.PermissionID,
                                                       PermissionName = p.PermissionName,
                                                       URL = p.URL
                                                   }).ToList<UserRolePermission>();
                return result;
            }
        }

        public void AddRoles(Roles role)
        {
            Contract.NotNull(role, "role");
            using (var context = AuthorizationEntities.CreateContext())
            {
                context.Roles.Add(role);
                context.SaveChanges();
            }
        }

        public void UpdateRoles(Roles role)
        {
            Contract.NotNull(role, "role");
            using (var context = AuthorizationEntities.CreateContext())
            {
                context.Roles.Attach(role);
                context.Entry(role).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

    }

    public class UserRolePermission{
        public string UserID { get; set; }
        public int RoleID { get; set; }
        public string RoleName { get; set; }
        public int PermissionID { get; set; }
        public string PermissionName { get; set; }
        public string URL { get; set; }
    }
}