using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace iPE.Models
{
    public class BuyDetail
    {
        [Key]
        public int t_id { get; set; }
        public string time { get; set; }
        public string name { get; set; }
        public int price { get; set; }
        public int number { get; set; }
        public string ticket { get; set; }
    }
}