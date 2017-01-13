using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace iPE.Models
{
    public class HomePageViewModel
    {
        [Key]
        public int id { get; set; }

        [Required]
        public TB_User myself { get; set; }

        public TB_Match releasedMatch { get; set; }

        public List<MatchViewModels> enrollMatches { get; set; }

        public List<BuyDetail> boughtTickets { get; set; } 

        public List<TB_Collection> collection { get; set; }
        
        public List<Data5> data { get; set; }
    }
}