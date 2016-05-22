namespace Archpack.Training.ArchUnits.RoleBaseAuth.V1.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Permissions
    {
        public Permissions()
        {
            RolePermissions = new HashSet<RolePermissions>();
        }

        [Key]
        public int PermissionID { get; set; }

        [Required]
        [StringLength(255)]
        public string PermissionName { get; set; }

        [Required]
        [StringLength(1024)]
        public string URL { get; set; }

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

        public virtual ICollection<RolePermissions> RolePermissions { get; set; }
    }
}
