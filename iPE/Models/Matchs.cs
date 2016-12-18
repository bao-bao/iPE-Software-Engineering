using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web;

namespace iPE.Models
{
    public partial class Matchs : DbContext
    {
        public Matchs()
            : base("name=Matchs")
        {
        }

        public virtual DbSet<TB_Match> TB_Match { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelbuilder)
        {
            modelbuilder.Entity<TB_Match>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelbuilder.Entity<TB_Match>()
                .Property(e => e.location)
                .IsUnicode(false);
        }
    }
}