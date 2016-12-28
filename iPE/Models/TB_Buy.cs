namespace iPE.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ipe.buy")]
    public partial class TB_Buy
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int u_id { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int t_id { get; set; }

        [Key]
        [Column(Order = 2)]
        public DateTime time { get; set; }

        public int number { get; set; }

        public decimal price { get; set; }

        public int state { get; set; }
    }
}
