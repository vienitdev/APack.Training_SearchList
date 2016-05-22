using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Archpack.Training.ArchUnits.RoleBaseAuth.V2.Data
{
    [Table("TACTS032")]
    public class Targets
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column("ATGTID")]
        public int TargetID { get; set; }

        [Required]
        [Column("ATITLE")]
        [StringLength(256)]
        public string Title { get; set; }

        [Column("ACNTNT")]
        [StringLength(1024)]
        public string Content { get; set; }

        [Required]
        [Column("ACNTNTTYPE")]
        public short ContentType { get; set; }

        [Column("ACRDT")]
        public DateTime? CreatedDate { get; set; }

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
        public string DeleteFlag { get; set; }
    }
}