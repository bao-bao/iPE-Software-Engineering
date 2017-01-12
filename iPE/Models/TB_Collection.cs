namespace iPE.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ipe.collection")]
    public partial class TB_Collection
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int u_id { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(45)]
        public string nba_team { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(45)]
        public string football_team { get; set; }
    }
}
