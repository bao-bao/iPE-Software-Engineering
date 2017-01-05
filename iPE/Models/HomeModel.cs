using System.ComponentModel.DataAnnotations;

namespace iPE.Models
{
    public class HomeModel
    {

        [Required]
        public int id { get; set; }

        [Required]
        [StringLength(50)]
        public string name { get; set; }

        [Required]
        [StringLength(50)]
        public string sponsor { get; set; }

        [Required]
        [StringLength(50)]
        public string time { get; set; }

        [Required]
        [StringLength(50)]
        public string location { get; set; }
    }
}