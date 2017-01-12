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
        public TB_User myself { get; set; }

        public TB_Match releasedMatch { get; set; }

        public List<MatchViewModels> enrollMatches { get; set; }

        public List<TB_Ticket> boughtTickets { get; set; } 
        
    }
}