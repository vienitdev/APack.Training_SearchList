using System;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Data.Common;
using Archpack.Training.ArchUnits.Data.Entity.V1;
using Archpack.Training.ArchUnits.Configuration.V1;

namespace Archpack.Training.ArchUnits.Arcs.Authentications.V1
{
    public partial class EmployeeInformationEntities : DbContext
    {
        private ConnectionStringItem configuration;

        private EmployeeInformationEntities(DbConnection connection)
            : base(connection, true)
        {
        }

        public static EmployeeInformationEntities CreateContext()
        {
            var config = ServiceConfigurationLoader.Load();
            var configuration = config.ConnectionStrings[typeof(EmployeeInformationEntities).Name];
            var context = new EmployeeInformationEntities(ConnectionFactory.Create(configuration));
            context.configuration = configuration;
            return context;
        }

        public virtual DbSet<Employee> Employee { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            if (configuration != null && !string.IsNullOrEmpty(configuration.DefaultScheme))
            {
                modelBuilder.HasDefaultSchema(configuration.DefaultScheme);
            }
        }
    }
}
