namespace Archpack.Training.ArchUnits.RoleBaseAuth.V1.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Roles
    {
        public Roles()
        {
            RoleInRoles = new HashSet<RoleInRoles>();
            RoleInRoles_ParentRole = new HashSet<RoleInRoles>();
            RolePermissions = new HashSet<RolePermissions>();
            UserRoles = new HashSet<UserRoles>();
        }

        [Key]
        public int RoleID { get; set; }

        [Required]
        [StringLength(255)]
        public string RoleName { get; set; }

        public DateTimeOffset CreatedDate { get; set; }

        [Required]
        [StringLength(255)]
        public string CreatedUser { get; set; }

        public DateTimeOffset UpdatedDate { get; set; }

        [Required]
        [StringLength(255)]
        public string UpdatedUser { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] version { get; set; }

        public virtual ICollection<RoleInRoles> RoleInRoles { get; set; }

        public virtual ICollection<RoleInRoles> RoleInRoles_ParentRole { get; set; }

        public virtual ICollection<RolePermissions> RolePermissions { get; set; }

        public virtual ICollection<UserRoles> UserRoles { get; set; }
    }
}
