namespace Archpack.Training.ArchUnits.RoleBaseAuth.V1.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class UserRoles
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(255)]
        public string UserID { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int RoleID { get; set; }

        [Key]
        [Column(Order = 2)]
        public DateTimeOffset CreatedDate { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(255)]
        public string CreatedUser { get; set; }

        [Key]
        [Column(Order = 4)]
        public DateTimeOffset UpdatedDate { get; set; }

        [Key]
        [Column(Order = 5)]
        [StringLength(255)]
        public string UpdatedUser { get; set; }

        [Key]
        [Column(Order = 6, TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] version { get; set; }

        public virtual Roles Roles { get; set; }
    }
}
