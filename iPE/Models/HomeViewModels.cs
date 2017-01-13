using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iPE.Models
{
    public class HomeSearchModel
    {
        public string name
        {
            get;
            set;
        }
    }

    public class Data5
    {
        public string time { get; set; }
        public string team1 { get; set; }
        public string team2 { get; set; }
        public string state { get; set; }
        public string score { get; set; }
        public string src { get; set; }
        public int status { get; set; }
    }
}