namespace Archpack.Training.ArchUnits.Arcs.Logging.V2
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Data.Common;
    using Configuration.V1;
    using ArchUnits.Data.Entity.V1;
    public partial class ArcsAuditLogEntities : DbContext
    {
        private ConnectionStringItem configuration;

        private ArcsAuditLogEntities(DbConnection connection)
            : base(connection, true)
        {
        }

        public static ArcsAuditLogEntities CreateContext()
        {
            var config = ServiceConfigurationLoader.Load();
            var configuration = config.ConnectionStrings[typeof(ArcsAuditLogEntities).Name];
            var context = new ArcsAuditLogEntities(ConnectionFactory.Create(configuration));
            context.configuration = configuration;
            return context;
        }


        public virtual DbSet<TSYS001> TSYS001 { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            if (configuration != null && !string.IsNullOrEmpty(configuration.DefaultScheme))
            {
                modelBuilder.HasDefaultSchema(configuration.DefaultScheme);
            }

            modelBuilder.Entity<TSYS001>()
                .Property(e => e.AEXEFLENM)
                .IsUnicode(false);

            modelBuilder.Entity<TSYS001>()
                .Property(e => e.ACRID)
                .IsUnicode(false);

            modelBuilder.Entity<TSYS001>()
                .Property(e => e.AUPID)
                .IsUnicode(false);

            modelBuilder.Entity<TSYS001>()
                .Property(e => e.ADLID)
                .IsUnicode(false);

            modelBuilder.Entity<TSYS001>()
                .Property(e => e.ADLFL)
                .IsUnicode(false);
        }
    }
}
