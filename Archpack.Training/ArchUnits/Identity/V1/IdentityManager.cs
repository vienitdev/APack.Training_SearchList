using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace Archpack.Training.ArchUnits.Identity.V1
{
    public abstract class IdentityManager<TContext, TIdentity, TKey> where TContext : IdentityDbContext<TIdentity, TKey> where TIdentity : IdentityUser<TKey>, new()
    {
        protected IdentityManager(TContext dbContext)
        {
            this.DbContext = dbContext;
        }

        public TContext DbContext { get; private set; }

        public virtual TIdentity FindUser(TKey key)
        {
            var arg = Expression.Parameter(typeof(TIdentity), "i");
            var predicate =
                Expression.Lambda<Func<TIdentity, bool>>(
                    Expression.Equal(
                        Expression.Property(arg, "IdentityKey"),
                        Expression.Constant(key)),
                    arg);
            return this.DbContext.User.Where(predicate).FirstOrDefault();
        }
    }
}