namespace iPE.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ipe.user")]
    public partial class TB_User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int u_id { get; set; }

        [Required]
        [StringLength(20)]
        public string username { get; set; }

        [Required]
        [StringLength(16)]
        public string pwd { get; set; }

        [StringLength(50)]
        public string name { get; set; }

        public int? gender { get; set; }

        [Column(TypeName = "char")]
        [StringLength(18)]
        public string card_id { get; set; }

        [Column(TypeName = "char")]
        [StringLength(11)]
        public string phone { get; set; }

        [Column(TypeName = "date")]
        public DateTime? birthday { get; set; }

        public int authority { get; set; }

        [StringLength(100)]
        public string organization { get; set; }
    }
}
