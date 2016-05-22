namespace Archpack.Training.ArchUnits.RoleBaseAuth.V2.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TACTS031")]
    public partial class Permissions
    {
       
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column("ARLID")]        
        public int RoleID { get; set; }

        [Column("ATGTID")]
        public int TargetID { get; set; }

        [Column("APRMTFL")]
        public int PermitFlag { get; set; }

        [Column("APRMTPRCTYPE")]
        [Range(0, 1023)]
        public int PermittedProcType { get; set; }

        [Column("ACRDT")]
        public DateTime CreatedDate { get; set; }

        [Column("ACRID")]
        public string CreatedUser { get; set; }

        [Column("AUPDT")]
        public DateTime? UpdatedDate { get; set; }

        [Column("AUPID")]
        public string UpdateUser { get; set; }

        [Column("ADLDT")]
        public DateTime? DeleteDate { get; set; }

        [Column("ADLID")]
        public string DeleteUser { get; set; }

        [Column("ADLFL")]
        public string DeleteFlag { get; set; }
    }
}
