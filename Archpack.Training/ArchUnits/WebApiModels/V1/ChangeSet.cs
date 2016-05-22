using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;

namespace Archpack.Training.ArchUnits.WebApiModels.V1
{
    public class ChangeSet<T> where T : class
    {
        /// <summary>
        /// デフォルト コンストラクタです。
        /// </summary>
        public ChangeSet()
        {
            this.Created = new List<T>();
            this.Updated = new List<T>();
            this.Deleted = new List<T>();
        }

        /// <summary>
        /// 追加されたエンティティのリストを取得または設定します。
        /// </summary>
        public List<T> Created { get; set; }
        /// <summary>
        /// 変更されたエンティティのリストを取得または設定します。
        /// </summary>
        public List<T> Updated { get; set; }
        /// <summary>
        /// 削除されたエンティティのリストを取得または設定します。
        /// </summary>
        public List<T> Deleted { get; set; }

        /// <summary>
        /// ChangeSet<T> の値をもとに DbContext に値を追加します。
        /// </summary>
        /// <typeparam name="T">変更セットが保持するエンティティ型</typeparam>
        /// <param name="self">対象とするチェンジセット</param>
        /// <param name="context">値を設定する DbContext</param>
        /// <returns>CreatedまたはUpdatedとマークされていたエンティティのリスト</returns>
        public IEnumerable<T> AttachTo(DbContext context, Action<T, EntityState> beforeAttach = null)
        {
            var set = context.Set<T>();
            var results = new List<T>();
            if (this.Created != null)
            {
                foreach (var created in this.Created)
                {
                    if (beforeAttach != null)
                    {
                        beforeAttach(created, EntityState.Added);
                    }
                    set.Add(created);
                    results.Add(created);
                }
            }

            if (this.Updated != null)
            {
                foreach (var updated in this.Updated)
                {
                    if (beforeAttach != null)
                    {
                        beforeAttach(updated, EntityState.Modified);
                    }

                    var objectContext = ((IObjectContextAdapter)context).ObjectContext;
                    var objectSet = objectContext.CreateObjectSet<T>();
                    var entityKey = objectContext.CreateEntityKey(objectSet.EntitySet.Name, updated);
                    object entity;
                    var exists = objectContext.TryGetObjectByKey(entityKey, out entity);
                    if (!exists)
                    {
                        set.Attach(updated);
                    }
                    context.Entry<T>(updated).State = EntityState.Modified;
                    results.Add(updated);
                }
            }

            if (this.Deleted != null)
            {
                foreach (var deleted in this.Deleted)
                {
                    if (beforeAttach != null)
                    {
                        beforeAttach(deleted, EntityState.Deleted);
                    }

                    set.Attach(deleted);
                    set.Remove(deleted);
                }
            }
            return results;
        }


        //以降は、移行前にChangeSetに実装されていたもの
        /// <summary>
        /// 追加・更新・削除されたエンティティの一覧を取得します。
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> GetValues()
        {
            return this.Items.Select(i => i.Value);
        }

        /// <summary>
        /// ChangeSet<T> の値をもとに DbContext に値を追加します。
        /// </summary>
        /// <typeparam name="T">変更セットが保持するエンティティ型</typeparam>
        /// <param name="self">対象とするチェンジセット</param>
        /// <param name="context">値を設定する DbContext</param>
        /// <returns>CreatedまたはUpdatedとマークされていたエンティティのリスト</returns>
        //public void AttachTo(DbContext context)
        //{
        //    this.Items.AttachTo(context);
        //}

        public IEnumerable<ChangeSetItem<T>> Items
        {
            get
            {
                if (this.Created != null)
                {
                    foreach (var v in this.Created)
                    {
                        yield return new ChangeSetItem<T>(v, EntityState.Added);
                    }
                }
                if (this.Updated != null)
                {
                    foreach (var v in this.Updated)
                    {
                        yield return new ChangeSetItem<T>(v, EntityState.Modified); ;
                    }
                }
                if (this.Deleted != null)
                {
                    foreach (var v in this.Deleted)
                    {
                        yield return new ChangeSetItem<T>(v, EntityState.Deleted); ;
                    }
                }
            }
        }
    }
}