using Archpack.Training.ArchUnits.Contracts.V1;
using Archpack.Training.ArchUnits.Data.Sql.V1;
using Archpack.Training.ArchUnits.RoleBaseAuth.V2.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Archpack.Training.ArchUnits.RoleBaseAuth.V2
{
    public class TargetManager
    {
        private string GetEmployeeID()
        {
            return new UserManager().GetUser(HttpContext.Current.User.Identity).EmployeeID.ToString();
        }

        /// <summary>
        /// Get target by targetId
        /// </summary>
        /// <param name="targetId"></param>
        /// <returns></returns>
        public Targets GetTargets(int targetId)
        {
            using (AuthorizationContext context = AuthorizationContext.CreateContext())
            {
                var target = from a in context.Targets
                             where a.TargetID == targetId && a.DeleteFlag == "0"
                             select a;
                return target.SingleOrDefault();
            }
        }

        /// <summary>
        /// Get target by Content and ContentType
        /// </summary>
        /// <param name="content"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public Targets GetTargets(string content, ContentTypes? contentType)
        {
            using (AuthorizationContext context = AuthorizationContext.CreateContext())
            {
                var target = from a in context.Targets
                             where a.Content.ToLower().Contains(content.ToLower()) 
                                && (contentType == null || a.ContentType == (int)contentType.Value) 
                                && a.DeleteFlag == "0"
                             select a;
                
                return target.FirstOrDefault();
            }
        }

        /// <summary>
        /// Return list target get by RoleId
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public List<Targets> GetTargetsByRoleId(int roleId)
        {
            using (var context = AuthorizationContext.CreateContext())
            {
                var targets = from t in context.Targets
                              join p in context.Permissions on t.TargetID equals p.TargetID
                              where p.RoleID == roleId
                              && t.DeleteFlag == "0"
                              && p.DeleteFlag == "0"
                              select t;
                return targets.ToList();
            }
        }

        /// <summary>
        /// Return list target get by UserId
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<Targets> GetTargetsByUserId(int userId)
        {
            UserRoleManager manager = new UserRoleManager();
            var roles = manager.GetRolesByUserId(userId);

            return GetTargetsByRoleIds(roles.Select(x => x.RoleID).ToList());
        }

        /// <summary>
        /// Return list target get by RoleId list.
        /// </summary>
        /// <param name="roleIds"></param>
        /// <returns></returns>
        public List<Targets> GetTargetsByRoleIds(List<int> roleIds)
        {
            using (var context = AuthorizationContext.CreateContext())
            {
                var targets = from t in context.Targets
                              join p in context.Permissions on t.TargetID equals p.TargetID
                              where roleIds.Contains(p.RoleID) 
                              && t.DeleteFlag == "0" 
                              && p.DeleteFlag == "0"
                              select t;
                return targets.ToList();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static List<Targets> GetPermittedTargetsByUserId(long userId)
        {
            UserRoleManager manager = new UserRoleManager();
            var roles = manager.GetRolesByUserId(userId);

            return GetPermittedTargetsByRoleIds(roles.Select(x => x.RoleID).ToList());            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="roleIds"></param>
        /// <returns></returns>
        public static List<Targets> GetPermittedTargetsByRoleIds(List<int> roleIds)
        {
            using (var context = AuthorizationContext.CreateContext())
            {
                var items = (from t in context.Targets
                             join p in context.Permissions on t.TargetID equals p.TargetID
                             where t.ContentType == (int)ContentTypes.Url 
                             && roleIds.Contains(p.RoleID) 
                             && p.PermitFlag == 1
                             && t.DeleteFlag == "0"
                             && p.DeleteFlag == "0"
                             select new PermittedTargets 
                             { 
                                 Target = t,
                                 Permission = p
                             }).ToList();

                foreach (var item in items)
                {
                    item.PermitProcTypes = PermissionManager.ConvertPermittedProcType(item.Permission.PermittedProcType);
                }
                var targets = items.Where(x => !x.PermitProcTypes.Contains(PermitProcType.None)).Select(x => x.Target).ToList();

                return targets;
            }
        }

        /// <summary>
        /// ターゲットの登録を行える 
        /// </summary>
        /// <param name="title">ターゲットに紐づくターゲットが存在する場合はエラーメッセージを表示する</param>
        /// <param name="content">ターゲットのコンテンツを登録する</param>
        /// <param name="contentType">ターゲットのコンテンツタイプを登録する（URL、Control、グループなど）</param>
        public void AddTargets(string title, string content, ContentTypes contentType)
        {
            Contract.NotEmpty(title, "title");
            Contract.NotNull(contentType, "contentType");
            using (AuthorizationContext context = AuthorizationContext.CreateContext())
            {                
                CheckTargetByTitleOrContent(context, title, content);
                var target = new Targets();
                target.TargetID = GetNextTargetId();
                target.Title = title;
                target.Content = content;
                target.ContentType = (short)contentType;
                target.CreatedDate = DateTime.Now;
                target.CreatedUser = GetEmployeeID();
                context.Targets.Add(target);                
                context.SaveChanges();
            }
        }

        /// <summary>
        /// ターゲットに紐づくターゲットを登録する 
        /// </summary>
        /// <param name="targetId"></param>
        /// <param name="parentTargetId"></param>
        public void AddTargetInTargets(int targetId, int parentTargetId)
        {
            if(targetId == parentTargetId)
            {
                throw new Exception("");
            }

            using (var context = AuthorizationContext.CreateContext())
            {
                var parentTargets = GetParentTargets(context);                
                CheckTargetHaveOneParent(parentTargets, targetId);
                CheckParentTargetIsChildOfTarget(parentTargets, parentTargetId, targetId);
                var newTarget = new TargetInTargets();
                newTarget.TargetID = targetId;
                newTarget.ParentTargetID = parentTargetId;
                newTarget.CreatedUser = GetEmployeeID();
                newTarget.CreatedDate = DateTime.Now;
                context.TargetInTargets.Add(newTarget);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// ターゲットに紐づくターゲットを削除する 
        /// </summary>
        /// <param name="targetId"></param>
        public void RemoveTarget(int targetId)
        {
            using (AuthorizationContext context = AuthorizationContext.CreateContext())
            {
                var parentTargets = GetParentTargets(context);
                CheckTargetHaveChildTargetBeforeDelete(parentTargets, targetId);
                CheckTargetHaveReferenceWithPermissionBeforeDelete(context, targetId);
                //get target exist
                var target = context.Targets.SingleOrDefault(x => x.TargetID == targetId && x.DeleteFlag == "0");
                if (target == null)
                {
                    throw new Exception("");
                }

                context.Targets.Remove(target);
                context.SaveChanges();
            }
        }

        public void RemoveTargetInTargets(int targetId, int parentTargetId)
        {
            using (AuthorizationContext context = AuthorizationContext.CreateContext())
            {
                //get exist target in targets
                var target = (from r in context.TargetInTargets
                            where r.TargetID == targetId 
                            && r.ParentTargetID == parentTargetId
                            && r.DeletedFlag == "0"
                            select r).SingleOrDefault();
                if (target == null)
                {
                    throw new Exception("");
                }
                context.TargetInTargets.Remove(target);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private int GetNextTargetId()
        {            
            using (var context = AuthorizationContext.CreateContext())
            {
                var id = (from t in context.Targets
                          where t.DeleteFlag == "0"                        
                         select t).Max(x => x.TargetID);

                return (id + 1); 
            }
        }

        /// <summary>
        /// Check target by title or content
        /// </summary>
        /// <param name="context"></param>
        /// <param name="title"></param>
        /// <param name="content"></param>
        private void CheckTargetByTitleOrContent(AuthorizationContext context, string title, string content)
        {
            var target = (from t in context.Targets
                         where (t.Title == title || t.Content == content)
                         && t.DeleteFlag == "0"
                         select t).SingleOrDefault();
            if (target != null)
            {
                throw new Exception("");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="targetId"></param>
        private void CheckTargetHaveReferenceWithPermissionBeforeDelete(AuthorizationContext context, int targetId)
        {
            var permission = (from p in context.Permissions
                             where p.TargetID == targetId
                              && p.DeleteFlag == "0"
                              select p).ToList();
            if (permission.Count > 0)
            {
                throw new Exception("");
            }
        }
      
        /// <summary>
        /// Get target and parent target
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public List<ParentTarget> GetParentTargets(AuthorizationContext context)
        {
            string sql = "SELECT t.ATGTID TargetID, tt.APRNTTGTID ParentTargetID, Level  " +
                         "   FROM TACTS032 t " +
                         "   LEFT JOIN TACTS033 tt ON tt.ATGTID = t.ATGTID " +
                         "   AND tt.ADLFL = '0' " +
                         "   WHERE t.ADLFL = '0' " +
                         "   START WITH tt.APRNTTGTID IS NULL " +
                         "   CONNECT BY PRIOR t.ATGTID = tt.APRNTTGTID " +
                         "   ORDER BY Level";

            FileSqlDefinitionFactory factory = new FileSqlDefinitionFactory("ArchUnits/RoleBaseAuth/V2/");
            DataQuery query = new DataQuery(context, factory).AppendQuery(sql);

            var parentTargetDetails = query.GetList<ParentTarget>();

            return parentTargetDetails;
        }

        /// <summary>
        /// 紐付けする親ターゲットが1つ以上の場合はエラーメッセージを表示する
        /// </summary>
        /// <param name="parentTargets"></param>
        /// <param name="targetId"></param>
        private void CheckTargetHaveOneParent(List<ParentTarget> parentTargets, int targetId)
        {
            var targets = parentTargets.Where(x => x.TargetID == targetId && x.ParentTargetID != null).ToList();
            if (targets.Count > 0)
            {
                throw new Exception("");
            }
        }

        /// <summary>
        /// 紐付けするターゲットのツリーにすでに、自ターゲットが登録されている場合はエラーメッセージを表示する（循環参照回避）
        /// </summary>
        /// <param name="parentTargets"></param>
        /// <param name="parentTargetId"></param>
        /// <param name="targetId"></param>
        private void CheckParentTargetIsChildOfTarget(List<ParentTarget> parentTargets, int parentTargetId, int targetId)
        {
            var targets = parentTargets.Where(x => x.ParentTargetID == targetId).ToList();
            if (targets.Count > 0)
            {
                for (var i = 0; i < targets.Count; i++)
                {
                    if (targets[i].TargetID == parentTargetId)
                    {
                        throw new Exception("");
                    }
                }
            }
        }

        /// <summary>
        /// ターゲットに紐づくターゲットが存在する場合はエラーメッセージを表示する
        /// </summary>
        /// <param name="parentTargets"></param>
        /// <param name="targetId"></param>
        private void CheckTargetHaveChildTargetBeforeDelete(List<ParentTarget> parentTargets, int targetId)
        {
            var targets = parentTargets.Where(x => x.ParentTargetID == targetId).ToList();
            if (targets.Count > 0)
            {
                throw new Exception("");
            }
        }
    }

    public class PermittedTargets
    {
        public Permissions Permission { get; set; }
        public Targets Target { get; set; }
        public IEnumerable<PermitProcType> PermitProcTypes { get; set; }
    }

    public enum ContentTypes
    {
        Url = 1,
        Category = 2
        //More...
    }
}