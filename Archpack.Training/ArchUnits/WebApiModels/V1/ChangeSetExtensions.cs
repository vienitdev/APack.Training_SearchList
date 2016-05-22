using Archpack.Training.ArchUnits.Contracts.V1;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Archpack.Training.ArchUnits.WebApiModels.V1
{


    public static class ChangeSetExtension
    {

        public static void AttachTo<T>(this IEnumerable<ChangeSetItem<T>> self, DbContext context) where T : class
        {
            var set = context.Set<T>();
            foreach (var item in self)
            {
                if (item.State == EntityState.Added)
                {
                    set.Add(item.Value);
                }
                if (item.State == EntityState.Modified)
                {
                    set.Attach(item.Value);
                    context.Entry<T>(item.Value).State = EntityState.Modified;

                }
                if (item.State == EntityState.Deleted)
                {
                    set.Attach(item.Value);
                    set.Remove(item.Value);
                }

            }
        }

        /// <summary>
        /// <see cref="ChangeSet(Of T)"/> の内容を整形します。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="prepare"></param>
        public static void Prepare<T>(this ChangeSet<T> self, Func<ChangeSetItem<T>, ChangeSetItem<T>> prepare) where T : class
        {
            Contract.NotNull(prepare, "prepare");
            var items = new List<ChangeSetItem<T>>();

            if (self.Created != null)
            {
                foreach (var item in self.Created)
                {
                    var state = new ChangeSetItem<T>(item, EntityState.Added);
                    var newState = prepare(state);
                    CheckPreparedState(state, newState, items);
                }
                self.Created.Clear();
            }

            if (self.Updated != null)
            {
                foreach (var item in self.Updated)
                {
                    var state = new ChangeSetItem<T>(item, EntityState.Modified);
                    var newState = prepare(state);
                    CheckPreparedState(state, newState, items);
                }
                self.Updated.Clear();
            }

            if (self.Deleted != null)
            {
                foreach (var item in self.Deleted)
                {
                    var state = new ChangeSetItem<T>(item, EntityState.Deleted);
                    var newState = prepare(state);
                    CheckPreparedState(state, newState, items);
                }
                self.Deleted.Clear();
            }
            foreach (var item in items)
            {
                if (item.State == EntityState.Added)
                {
                    if (self.Created == null)
                    {
                        self.Created = new List<T>();
                    }
                    self.Created.Add(item.Value);
                }
                if (item.State == EntityState.Modified)
                {
                    if (self.Updated == null)
                    {
                        self.Updated = new List<T>();
                    }
                    self.Updated.Add(item.Value);
                }
                if (item.State == EntityState.Deleted)
                {
                    if (self.Deleted == null)
                    {
                        self.Deleted = new List<T>();
                    }
                    self.Deleted.Add(item.Value);
                }
            }
        }

        private static void CheckPreparedState<T>(ChangeSetItem<T> oldState, ChangeSetItem<T> newState, List<ChangeSetItem<T>> items) where T : class
        {
            //newStateがnullだったら対象に含めない
            if (newState == null)
            {
                return;
            }
            //違うインスタンスの場合はエラー
            if (!Object.ReferenceEquals(oldState, newState))
            {
                throw new InvalidOperationException();
            }
            items.Add(newState);
        }

    }
}