namespace Archpack.Training.ArchUnits.RoleBaseAuth.V1.Data
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class AuthorizationEntities : DbContext
    {
        public AuthorizationEntities()
            : base("name=AuthorizationEntities")
        {
        }

        public virtual DbSet<Permissions> Permissions { get; set; }
        public virtual DbSet<Roles> Roles { get; set; }
        public virtual DbSet<RoleInRoles> RoleInRoles { get; set; }
        public virtual DbSet<RolePermissions> RolePermissions { get; set; }
        public virtual DbSet<UserRoles> UserRoles { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Permissions>()
                .Property(e => e.version)
                .IsFixedLength();

            modelBuilder.Entity<Permissions>()
                .HasMany(e => e.RolePermissions)
                .WithRequired(e => e.Permissions)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Roles>()
                .Property(e => e.version)
                .IsFixedLength();

            modelBuilder.Entity<Roles>()
                .HasMany(e => e.RoleInRoles)
                .WithRequired(e => e.Roles)
                .HasForeignKey(e => e.RoleID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Roles>()
                .HasMany(e => e.RoleInRoles_ParentRole)
                .WithRequired(e => e.Roles_ParentRole)
                .HasForeignKey(e => e.ParentRoleID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Roles>()
                .HasMany(e => e.RolePermissions)
                .WithRequired(e => e.Roles)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Roles>()
                .HasMany(e => e.UserRoles)
                .WithRequired(e => e.Roles)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<RoleInRoles>()
                .Property(e => e.version)
                .IsFixedLength();

            modelBuilder.Entity<RolePermissions>()
                .Property(e => e.version)
                .IsFixedLength();

            modelBuilder.Entity<UserRoles>()
                .Property(e => e.version)
                .IsFixedLength();
        }
    }
}
