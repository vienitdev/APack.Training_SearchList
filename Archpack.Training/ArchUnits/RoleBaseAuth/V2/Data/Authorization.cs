using Archpack.Training.ArchUnits.Configuration.V1;
using Archpack.Training.ArchUnits.Data.Entity.V1;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Archpack.Training.ArchUnits.RoleBaseAuth.V2.Data
{
    public partial class AuthorizationContext : DbContext
    {
        private ConnectionStringItem configuration;

        private AuthorizationContext(DbConnection connection)
            : base(connection, true)
        {
        }

        public static AuthorizationContext CreateContext()
        {
            var config = ServiceConfigurationLoader.Load();
            var configuration = config.ConnectionStrings["Authorization"];
            var context = new AuthorizationContext(ConnectionFactory.Create(configuration));
            context.configuration = configuration;
            return context;
        }

        public virtual DbSet<Roles> Roles { get; set; }
        public virtual DbSet<Targets> Targets { get; set; }
        public virtual DbSet<Permissions> Permissions { get; set; }
        public virtual DbSet<TargetInTargets> TargetInTargets { get; set; }
        public virtual DbSet<RoleInRoles> RoleInRoles { get; set; }        
        public virtual DbSet<UserInRoles> UserInRoles { get; set; }
        //public virtual DbSet<Employee> Employees { get; set; }
        //public virtual DbSet<AuthorizationResult> AuthorizationResult { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            if (configuration != null && !string.IsNullOrEmpty(configuration.DefaultScheme))
            {
                modelBuilder.HasDefaultSchema(configuration.DefaultScheme);
            }
        }
    }
}