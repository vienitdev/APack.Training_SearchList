using Archpack.Training.ArchUnits.Identity.V1;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Archpack.Training.ArchUnits.Arcs.Authentications.V2
{
    public class ActosDbAuthUser : IdentityUser<string>
    {

        [Display(Name = "作成日時")]
        [Column("ACRDT")]
        public DateTime? CreateDate { get; set; }

        [Display(Name = "作成者ID")]
        [StringLength(10)]
        [Column("ACRID")]
        public string CreatedById { get; set; }

        [Display(Name = "更新日時")]
        [Column("AUPDT")]
        [ConcurrencyCheck]
        public DateTime? UpdateDate { get; set; }

        [Display(Name = "更新者ID")]
        [StringLength(10)]
        [Column("AUPID")]
        public string UpdateId { get; set; }

        [Display(Name = "削除日時")]
        [Column("ADLDT")]
        public DateTime? DeleteDate { get; set; }

        [Display(Name = "削除者ID")]
        [StringLength(10)]
        [Column("ADLID")]
        public string DeleteById { get; set; }

        [Display(Name = "論理削除区分")]
        [StringLength(1)]
        [Column("ADLFL")]
        public string DeleteFlag { get; set; }
    }
}