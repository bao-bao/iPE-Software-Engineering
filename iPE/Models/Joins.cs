using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web;

namespace iPE.Models
{
    public partial class Joins : DbContext
    {
        public Joins()
            : base("name=Join")
        {
        }

        public virtual DbSet<TB_Join> TB_Join { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}