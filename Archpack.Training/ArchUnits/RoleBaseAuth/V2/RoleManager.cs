using Archpack.Training.ArchUnits.Contracts.V1;
using Archpack.Training.ArchUnits.Data.Sql.V1;
using Archpack.Training.ArchUnits.Logging.V1;
using Archpack.Training.ArchUnits.Pipeline.V1;
using Archpack.Training.ArchUnits.RoleBaseAuth.V2.Data;
using Archpack.Training.ArchUnits.WebApiModels.V1;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;

namespace Archpack.Training.ArchUnits.RoleBaseAuth.V2
{
    public class RoleManager
    {
        private LogContext logContext = null;
        private UserRoleManager userRoleManager;        
        //private Employee employee;

        public RoleManager(LogContext logContext)
        {
            this.logContext = logContext;            
            userRoleManager = new UserRoleManager();            
        }

        public RoleManager()
        {           
            userRoleManager = new UserRoleManager();
        }

        public List<Roles> GetRolesById(int roleId)
        {
            using (AuthorizationContext context = AuthorizationContext.CreateContext())
            {
                var role = from a in context.Roles
                           where a.RoleID == roleId
                           && a.DeleteFlag == "0"
                           select a;
                return role.ToList();

            }
        }       

        /// <summary>
        /// ロールの登録を行える 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        public void AddRole(string name, string description)
        {
            Contract.NotEmpty(name, "name");
            using (AuthorizationContext context = AuthorizationContext.CreateContext())
            {
                CheckDuplicateRoleName(context, name);
                var newRole = new Roles();
                newRole.RoleID = GetNextRoleId(context);
                newRole.RoleName = name;
                newRole.Description = description;
                newRole.CreatedDate = DateTime.Now;
                newRole.CreatedUser = GetEmployeeID().ToString();
                context.Roles.Add(newRole);
                context.SaveChanges();

            }
        }

        /// <summary>
        /// ロールに紐づくロールを登録する
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="roleParentId"></param>
        public void AddRoleInRoles(int roleId, int roleParentId)
        {
            if (roleId == roleParentId)
            {
                throw new Exception("");
            }

            using (var context = AuthorizationContext.CreateContext())
            {
                var parentRoles = GetParentRoles(context);
                //紐付けする親ロールが1つ以上の場合はエラーメッセージを表示する
                CheckRoleHaveOneParent(parentRoles, roleId);
                CheckParentRoleIsChildOfRole(parentRoles, roleParentId, roleId);
                var newRole = new RoleInRoles();
                newRole.RoleID = roleId;
                newRole.ParentRoleID = roleParentId;
                newRole.CreatedUser = GetEmployeeID().ToString();
                newRole.CreatedDate = DateTime.Now;
                context.RoleInRoles.Add(newRole);
                context.SaveChanges();
            }
        }

        public void UpdateRole(int roleId, string roleName)
        {
            Contract.NotEmpty(roleName, "roleName");
            using (var context = AuthorizationContext.CreateContext())
            {
                CheckDuplicateRoleName(context, roleName);
                //Get exist role
                var existRole = context.Roles.SingleOrDefault(x => x.RoleID == roleId && x.DeleteFlag == "0");
                if (existRole != null)
                {
                    existRole.RoleName = roleName;
                    context.Roles.Attach(existRole);
                    context.Entry(existRole).State = EntityState.Modified;
                    context.SaveChanges();
                }                
            }
        }

        /// <summary>
        /// ロールの削除が行える 
        /// </summary>
        /// <param name="roleId"></param>
        public void RemoveRole(int roleId)
        {           
            using (AuthorizationContext context = AuthorizationContext.CreateContext())
            {
                var parentRoles = GetParentRoles(context);
                //Cannot delete role if this role have child-role
                CheckRoleHaveChildRoleBeforeDelete(parentRoles, roleId);
                //Cannot delete role if this role have reference with user.
                CheckRoleHaveReferenceWithUserBeforeDelete(roleId);
                //Get exist role
                var role = context.Roles.SingleOrDefault(x => x.RoleID == roleId && x.DeleteFlag == "0");
                if (role == null)
                {
                    throw new Exception("");    
                }

                context.Roles.Remove(role);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Remove role out of roles
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="parentRoleId"></param>
        public void RemoveRoleInRoles(int roleId, int parentRoleId)
        {
            using (AuthorizationContext context = AuthorizationContext.CreateContext())
            { 
                //get exist role in roles
                var role = (from r in context.RoleInRoles
                            where r.RoleID == roleId 
                            && r.ParentRoleID == parentRoleId
                            && r.DeleteFlag == "0"
                            select r).SingleOrDefault();
                if (role == null)
                {
                    throw new Exception("");
                }
                context.RoleInRoles.Remove(role);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Get next roleId in the database.
        /// </summary>
        /// <returns></returns>
        private int GetNextRoleId(AuthorizationContext context)
        {
            var id = (from t in context.Roles
                      where t.DeleteFlag == "0"
                      select t).Max(x => x.RoleID);

            return (id + 1);
        }

        /// <summary>
        /// すでに登録されているロール名を登録した場合にはエラーメッセージを表示する
        /// </summary>
        /// <param name="context"></param>
        /// <param name="name"></param>
        private void CheckDuplicateRoleName(AuthorizationContext context, string name)
        {
            var role = (from r in context.Roles
                       where r.RoleName.Equals(name)
                       && r.DeleteFlag == "0"
                       select r).SingleOrDefault();
            if (role != null)
            {
                throw new DuplicateNameException("");
            }
        }

        /// <summary>
        /// 削除するロールに紐づくロール（自分を親として指定しているロール）が存在する場合はエラーメッセージを表示する
        /// </summary>
        /// <param name="parentRoles"></param>
        /// <param name="roleId"></param>
        private void CheckRoleHaveChildRoleBeforeDelete(List<ParentRole> parentRoles, int roleId)
        {
            if (parentRoles.Where(x => x.ParentRoleID == roleId).Any())
            {
                throw new Exception("");
            }
        }

        /// <summary>
        /// 削除するロールに紐づくユーザーが存在する場合はエラーメッセージを表示する
        /// </summary>
        /// <param name="roleId"></param>
        private void CheckRoleHaveReferenceWithUserBeforeDelete(int roleId)
        {
            var userInRole = userRoleManager.GetRolesByUserId(GetEmployeeID());
            var roles = userInRole.Where(x => x.RoleID == roleId).ToList();
            if (userInRole.Where(x => x.RoleID == roleId).Any())
            {
                throw new Exception("");
            }
        }

        /// <summary>
        /// 紐付けする親ロールが1つ以上の場合はエラーメッセージを表示する
        /// </summary>
        /// <param name="parentRoles"></param>
        /// <param name="roleId"></param>
        private void CheckRoleHaveOneParent(List<ParentRole> parentRoles, int roleId)
        {
            if (parentRoles.Where(x => x.RoleID == roleId && x.ParentRoleID != null).Any())
            {
                throw new Exception("");
            }
        }

        /// <summary>
        /// 紐付けするロールのツリーにすでに、自ロールが登録されている場合はエラーメッセージを表示する（循環参照回避）
        /// </summary>
        /// <param name="parentRoles"></param>
        /// <param name="parentRoleId"></param>
        /// <param name="roleId"></param>
        public void CheckParentRoleIsChildOfRole(List<ParentRole> parentRoles, int parentRoleId, int roleId)
        {
            var roles = parentRoles.Where(x => x.ParentRoleID == roleId).ToList();
            if (roles.Count > 0)
            {
                for (var i = 0; i < roles.Count; i++)
                {
                    if (roles[i].RoleID == parentRoleId)
                    {
                        throw new Exception("");
                    }
                }
            }
        }
        
        /// <summary>
        /// Return roles and parent role.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public List<ParentRole> GetParentRoles(AuthorizationContext context)
        {
            string sql = "SELECT r.ARLID RoleID, rr.APRNTRLID ParentRoleID, Level " +
                         "   FROM TACTS029 r " +
                         "   LEFT JOIN TACTS030 rr ON rr.ARLID = r.ARLID " +
                         "   AND rr.ADLFL = '0' " +
                         "   WHERE r.ADLFL = '0' " +
                         "   START WITH rr.APRNTRLID IS NULL " +
                         "   CONNECT BY PRIOR r.ARLID = rr.APRNTRLID " +
                         "   ORDER BY Level";

            FileSqlDefinitionFactory factory = new FileSqlDefinitionFactory("ArchUnits/RoleBaseAuth/V2/");
            DataQuery query = new DataQuery(context, factory).AppendQuery(sql);

            var parentRoleDetails = query.GetList<ParentRole>();

            return parentRoleDetails;
        }


        private int GetEmployeeID()
        {
            return (int)(new UserManager().GetUser(HttpContext.Current.User.Identity).EmployeeID);
        }
    }
}