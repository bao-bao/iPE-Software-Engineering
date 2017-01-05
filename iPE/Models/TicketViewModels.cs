using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace iPE.Models {
    public class TicketInfo {
        [Key]
        [Required]
        public int id { get; set; }
        [Required]
        [Display(Name ="比赛名称")]
        public string name { get; set; }
        [Required]
        [Display(Name = "比赛类型")]
        public int type { get; set; }

        [Display(Name = "票价")]
        public decimal price { get; set; }

        [Display(Name = "剩余票数")]
        public int surplus { get; set; }

        [Display(Name = "比赛描述")]
        public string description { get; set; }
    }

    public class TicketCreateModel {
        [Required]
        [Display(Name = "比赛名称")]
        public string name { get; set; }
        [Required]
        [Display(Name = "票价")]
        public decimal price { get; set; }
        [Required]
        [Display(Name = "总票数")]
        public int max { get; set; }
        [Required]
        [Display(Name = "比赛描述")]
        public string description { get; set; }
    }

}