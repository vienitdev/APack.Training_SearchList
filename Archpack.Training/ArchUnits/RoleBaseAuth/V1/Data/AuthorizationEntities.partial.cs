using Archpack.Training.ArchUnits.Configuration.V1;
using Archpack.Training.ArchUnits.Data.Entity.V1;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Web;

namespace Archpack.Training.ArchUnits.RoleBaseAuth.V1.Data
{
    public partial class AuthorizationEntities
    {
        private AuthorizationEntities(DbConnection connection)
            : base(connection, true)
        {
        }

        public static AuthorizationEntities CreateContext()
        {
            var config = ServiceConfigurationLoader.Load();
            var context = new AuthorizationEntities(ConnectionFactory.Create(config.ConnectionStrings["Authorization"]));
            return context;
        }
    }
}