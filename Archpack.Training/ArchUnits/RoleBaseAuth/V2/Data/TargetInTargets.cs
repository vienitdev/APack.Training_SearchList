using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Archpack.Training.ArchUnits.RoleBaseAuth.V2.Data
{
    [Table("TACTS033")]
    public class TargetInTargets
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column("ATGTID")]
        public int TargetID { get; set; }

        [Column("APRNTTGTID")]
        public int ParentTargetID { get; set; }

        [Column("ACRDT")]
        public DateTime CreatedDate { get; set; }

        [Column("ACRID")]
        public string CreatedUser { get; set; }

        [Column("AUPDT")]
        public DateTime? UpdatedDate { get; set; }

        [Column("AUPID")]
        public string UpdatedUser { get; set; }

        [Column("ADLDT")]
        public DateTime? DeletedDate { get; set; }

        [Column("ADLID")]
        public string DeletedUser { get; set; }

        [Column("ADLFL")]
        public string DeletedFlag { get; set; }
    }
}