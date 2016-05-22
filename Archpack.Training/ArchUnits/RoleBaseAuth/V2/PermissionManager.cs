using Archpack.Training.ArchUnits.Data.Sql.V1;
using Archpack.Training.ArchUnits.RoleBaseAuth.V2.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Archpack.Training.ArchUnits.RoleBaseAuth.V2
{
    public class PermissionManager
    {        
        public PermissionManager()
        {
        }

        private string GetEmployeeID()
        {
            return new UserManager().GetUser(HttpContext.Current.User.Identity).EmployeeID.ToString();
        }

        /// <summary>
        /// Get permissions base on list roleIds and targetId
        /// </summary>
        /// <param name="roleIds"></param>
        /// <param name="targetId"></param>
        /// <returns></returns>
        public PermissionResult GetPermissions(List<int> roleIds, int targetId)
        {
            using (AuthorizationContext context = AuthorizationContext.CreateContext())
            {
                var permissions = (from p in context.Permissions                                  
                                  where roleIds.Contains(p.RoleID) && p.TargetID == targetId && p.DeleteFlag == "0"
                                  orderby p.PermittedProcType descending
                                  select p).Take(1).SingleOrDefault();
                var result = new PermissionResult();
                if (permissions == null)
                {
                    result.PermitFlag = false;
                    result.PermitProcTypes = new List<PermitProcType>() {
                        PermitProcType.None
                    };
                }
                else
                {
                    result.PermitFlag = permissions.PermitFlag == 1 ? true : false;
                    result.PermitProcTypes = ConvertPermittedProcType(permissions.PermittedProcType);
                }        
                return result;
            }
        }

        /// <summary>
        /// Get permissions base on roleId and targetId
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="targetId"></param>
        /// <returns></returns>
        public PermissionResult GetPermissions(int roleId, int targetId)
        {
            using (var context = AuthorizationContext.CreateContext())
            {                
                var permissions = (from p in context.Permissions
                                   where p.RoleID == roleId && p.TargetID == targetId && p.DeleteFlag == "0"
                                   orderby p.PermittedProcType descending
                                   select p).Take(1).SingleOrDefault();
                var result = new PermissionResult();
                if (permissions == null)
                {
                    result.PermitFlag = true;
                    result.PermitProcTypes = new List<PermitProcType>() { PermitProcType.None };
                }
                else
                {
                    result.PermitFlag = permissions.PermitFlag == 1 ? true : false;
                    result.PermitProcTypes = ConvertPermittedProcType(permissions.PermittedProcType);
                }                
                
                return result;
            }
        }
        
        /// <summary>
        /// Convert PermittedProcType from number to binary
        /// </summary>
        /// <param name="permittedProcType"></param>
        /// <returns></returns>
        public static List<PermitProcType> ConvertPermittedProcType(int permittedProcType)
        {
            string permissions = Convert.ToString(permittedProcType, 2);
            char[] items = permissions.ToCharArray();
            List<PermitProcType> types = new List<PermitProcType>();
            for (var i = 0; i < items.Length; i++)
            {
                if (items[i].Equals('1'))
                {
                    types.Add((PermitProcType)(i + 1));
                }
            }
            if (types.Count == 0)
            {
                types.Add(PermitProcType.None);
            }
            return types;
        }

        /// <summary>
        /// パーミッションの登録を行える
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="targetId"></param>
        /// <param name="permitFlag"></param>
        /// <param name="permittedProcType"></param>
        public void AddPermission(int roleId, int targetId, bool permitFlag, int permittedProcType)
        {
            using (var context = AuthorizationContext.CreateContext())
            {
                CheckRoleAndTargetHasAlreadyRegister(context, roleId, targetId);
                var permission = new Permissions();
                permission.RoleID = roleId;
                permission.TargetID = targetId;
                permission.PermitFlag = permitFlag ? 1 : 0;
                permission.PermittedProcType = permittedProcType;
                permission.CreatedUser = GetEmployeeID();
                permission.CreatedDate = DateTime.Now;
                context.Permissions.Add(permission);
                context.SaveChanges();
            }
        }


        /// <summary>
        /// パーミッションの削除が行える 
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="targetId"></param>
        public void RemovePermission(int roleId, int targetId)
        {
            using (var context = AuthorizationContext.CreateContext())
            { 
                //get exist permission
                var permission = (from p in context.Permissions
                                 where p.RoleID == roleId && p.TargetID == targetId
                                 && p.DeleteFlag == "0"
                                 select p).SingleOrDefault();
                if (permission == null)
                {
                    throw new Exception("");
                }

                context.Permissions.Remove(permission);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// 同一のロールとターゲットの紐付けが1つ以上の場合はエラーメッセージを表示する
        /// </summary>
        /// <param name="context"></param>
        /// <param name="roleId"></param>
        /// <param name="targetId"></param>
        private void CheckRoleAndTargetHasAlreadyRegister(AuthorizationContext context, int roleId, int targetId)
        {
            var permission = (from p in context.Permissions
                              where p.RoleID == roleId && p.TargetID == targetId
                              && p.DeleteFlag == "0"
                              select p).SingleOrDefault();
            if (permission != null)
            {
                throw new Exception("");
            }
        }       
    }

    public class PermissionResult
    {
        public bool PermitFlag { get; set; }
        public IEnumerable<PermitProcType> PermitProcTypes { get; set; }
       
    }

    public enum PermitProcType
    {
        None = 0,
        Reference = 1,
        Update = 2,
        Approve = 3
    }
}