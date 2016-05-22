using Archpack.Training.ArchUnits.Configuration.V1;
using Archpack.Training.ArchUnits.Data.Entity.V1;
using Archpack.Training.ArchUnits.Identity.V1;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Archpack.Training.ArchUnits.Arcs.Authentications.V2
{
    public class ActosDbAuthContext: IdentityDbContext<ActosDbAuthUser, string>
    {
        private ConnectionStringItem configuration;

        private ActosDbAuthContext(DbConnection connection)
            : base(connection, true)
        {
        }

        public static ActosDbAuthContext CreateContext()
        {
            var config = ServiceConfigurationLoader.Load();
            var configuration = config.ConnectionStrings["ActosDbAuth"];
            var context = new ActosDbAuthContext(ConnectionFactory.Create(configuration));
            context.configuration = configuration;
            return context;
        }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            if (configuration != null && !string.IsNullOrEmpty(configuration.DefaultScheme))
            {
                modelBuilder.HasDefaultSchema(string.Empty);
            }

            modelBuilder.Entity<ActosDbAuthUser>().ToTable("TACTS059");
            modelBuilder.Entity<ActosDbAuthUser>().HasKey(t => t.IdentityKey);
            modelBuilder.Entity<ActosDbAuthUser>().Property(t => t.IdentityKey).HasColumnName("AIDK").IsRequired().HasMaxLength(100);
            modelBuilder.Entity<ActosDbAuthUser>().Property(t => t.PassKey).HasColumnName("APASSHS").IsRequired().HasMaxLength(100);
        }
    }
}