namespace Archpack.Training.ArchUnits.Arcs.Logging.V1
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TSYS001")]
    public partial class TSYS001
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long AADTLOGID { get; set; }

        public long? AEMPID { get; set; }

        [StringLength(100)]
        public string AEXEFLENM { get; set; }

        public byte? AOPEFL { get; set; }

        public long? AGETRSLID { get; set; }

        public DateTime? ABTDT { get; set; }

        public DateTime? ACRDT { get; set; }

        [StringLength(10)]
        public string ACRID { get; set; }

        public DateTime? AUPDT { get; set; }

        [StringLength(10)]
        public string AUPID { get; set; }

        public DateTime? ADLDT { get; set; }

        [StringLength(10)]
        public string ADLID { get; set; }

        [StringLength(1)]
        public string ADLFL { get; set; }
    }
}
