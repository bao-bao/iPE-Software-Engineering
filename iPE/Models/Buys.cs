using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web;

namespace iPE.Models
{
    public partial class Buys : DbContext
    {
        public Buys()
            : base("name=Buys")
        {
        }

        public virtual DbSet<TB_Buy> TB_Buy { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelbuilder)
        {
            modelbuilder.Entity<TB_Buy>()
                .Property(e => e.price)
                .HasPrecision(10, 2);
        }
    }
}