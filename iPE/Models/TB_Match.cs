namespace iPE.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ipe.match")]
    public partial class TB_Match
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int m_id { get; set; }

        [Required]
        public int u_id { get; set; }

        [Required]
        public int t_id { get; set; }

        [Required]
        [StringLength(50)]
        public string name { get; set; }

        [Required]
        [StringLength(50)]
        public string sponsor { get; set; }

        [Required]
        public int m_num { get; set; }

        [Required]
        public int w_num { get; set; }

        [Required]
        public DateTime s_time { get; set; }

        [Required]
        public DateTime e_time { get; set; }

        [Required]
        public DateTime c_time { get; set; }

        [Required]
        [StringLength(50)]
        public string location { get; set; }

        [StringLength(256)]
        public string description { get; set; }
    }
}
