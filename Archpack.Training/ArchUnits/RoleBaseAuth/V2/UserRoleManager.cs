using Archpack.Training.ArchUnits.RoleBaseAuth.V2.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Archpack.Training.ArchUnits.RoleBaseAuth.V2
{
    public class UserRoleManager
    {
        public UserRoleManager()
        {
        }

        private string GetEmployeeID()
        {
            return new UserManager().GetUser(HttpContext.Current.User.Identity).EmployeeID.ToString();
        }

        /// <summary>
        /// ユーザーに対してロールの紐づけの登録を行える 
        /// </summary>
        public void AddRoleToUser(int roleId, int userId)
        {
            using (var context = AuthorizationContext.CreateContext())
            {
                CheckRoleMustExist(context, roleId);
                CheckUserInRole(context, roleId, userId);
                var userInRole = new UserInRoles();
                userInRole.RoleID = roleId;
                userInRole.UserID = userId;
                userInRole.CreatedUser = GetEmployeeID();
                userInRole.CreatedDate = DateTime.Now;
                context.UserInRoles.Add(userInRole);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// ユーザーに紐づくロールを削除する
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="userId"></param>
        public void RemoveRoleFromUser(int roleId, int userId)
        {
            using (var context = AuthorizationContext.CreateContext())
            { 
                //get user exist in role
                var userInRole = (from ur in context.UserInRoles
                                 where ur.RoleID == roleId 
                                 && ur.UserID == userId
                                 && ur.DeleteFlag == "0"
                                 select ur).SingleOrDefault();
                if (userInRole == null)
                {
                    throw new Exception("");
                }
                context.UserInRoles.Remove(userInRole);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// ユーザーに紐づくロール情報を取得する 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<Roles> GetRolesByUserId(long userId)
        {
            using (AuthorizationContext context = AuthorizationContext.CreateContext())
            {
                var role = from r in context.Roles
                           join ur in context.UserInRoles on r.RoleID equals ur.RoleID
                           where ur.UserID == userId
                           && r.DeleteFlag == "0"
                           && ur.DeleteFlag == "0"
                           select r;
                return role.ToList();
            }
        }
        
        /// <summary>
        /// すでに登録されているロールを登録した場合にはエラーメッセージを表示する
        /// </summary>
        /// <param name="context"></param>
        /// <param name="roleId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        private void CheckUserInRole(AuthorizationContext context, int roleId, int userId)
        {
            var userInRole = (from ur in context.UserInRoles
                             where ur.RoleID == roleId 
                             && ur.UserID == userId
                             && ur.DeleteFlag == "0"
                             select ur).SingleOrDefault();

            if (userInRole != null)
            {
                throw new Exception("");
            }
        }


        private void CheckRoleMustExist(AuthorizationContext context, int roleId)
        {
            var role = (from r in context.Roles
                       where r.RoleID == roleId
                       && r.DeleteFlag == "0"
                       select r).SingleOrDefault();
            if (role == null)
            {
                throw new ArgumentNullException("Not exist role.");
            }
        }
    }
}