using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web;

namespace iPE.Models
{
    public partial class Tickets : DbContext
    {
        public Tickets()
            : base("name=Ticket")
        {
        }

        public virtual DbSet<TB_Ticket> TB_Match { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelbuilder)
        {
            modelbuilder.Entity<TB_Ticket>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelbuilder.Entity<TB_Ticket>()
                .Property(e => e.describe)
                .IsUnicode(false);

            modelbuilder.Entity<TB_Ticket>()
                .Property(e => e.price)
                .HasPrecision(10, 2);
        }
    }
}