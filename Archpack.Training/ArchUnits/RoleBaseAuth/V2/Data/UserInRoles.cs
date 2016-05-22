using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Archpack.Training.ArchUnits.RoleBaseAuth.V2.Data
{
    [Table("TACTS028")]
    public class UserInRoles
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column("AUSRID")]
        public int UserID { get; set; }

        [Column("ARLID")]
        public int RoleID { get; set; }

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
        public string DeleteFlag { get; set; }
    }
}