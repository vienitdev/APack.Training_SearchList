using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Archpack.Training.ArchUnits.RoleBaseAuth.V2.Data
{
    [Table("TACTS030")]
    public class RoleInRoles
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column("ARLID")]
        public int RoleID { get; set; }

        [Column("APRNTRLID")]
        public int ParentRoleID { get; set; }

        [Column("ACRDT")]
        public DateTime CreatedDate { get; set; }

        [Column("ACRID")]
        public string CreatedUser { get; set; }

        [Column("AUPDT")]
        public DateTime UpdatedDate { get; set; }

        [Column("AUPID")]
        public string UpdateUser { get; set; }

        [Column("ADLDT")]
        public DateTime DeleteDate { get; set; }

        [Column("ADLID")]
        public string DeleteUser { get; set; }

        [Column("ADLFL")]
        public string DeleteFlag { get; set; }
    }
}