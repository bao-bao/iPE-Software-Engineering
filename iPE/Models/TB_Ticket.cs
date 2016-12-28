namespace iPE.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ipe.ticket")]
    public partial class TB_Ticket
    {
        [Key]
        public int t_id { get; set; }

        public int u_id { get; set; }

        public int m_id { get; set; }

        [Required]
        [StringLength(100)]
        public string name { get; set; }

        public int type { get; set; }

        public decimal price { get; set; }

        public int max { get; set; }

        public int sell { get; set; }

        [StringLength(256)]
        public string description { get; set; }
    }
}
