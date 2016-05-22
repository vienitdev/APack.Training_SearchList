using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;

namespace Archpack.Training.ArchUnits.Identity.V1
{
    public abstract class IdentityDbContext<TIdentity, TKey> : DbContext where TIdentity : IdentityUser<TKey>
    {

        public IdentityDbContext(string nameOrConnectionString) : base(nameOrConnectionString)
        {

        }
        public IdentityDbContext(string nameOrConnectionString, DbCompiledModel model) : base(nameOrConnectionString, model)
        {

        }
        public IdentityDbContext(DbConnection existingConnection, bool contextOwnsConnection) : base(existingConnection, contextOwnsConnection)
        {

        }
        public IdentityDbContext(ObjectContext objectContext, bool dbContextOwnsObjectContext) : base(objectContext, dbContextOwnsObjectContext)
        {

        }
        public IdentityDbContext(DbConnection existingConnection, DbCompiledModel model, bool contextOwnsConnection) : base(existingConnection, model, contextOwnsConnection)
        {
        }


        public DbSet<TIdentity> User { get; set; }

    }
}